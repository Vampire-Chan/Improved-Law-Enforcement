using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DispatchManager : Script
{
    private Dictionary<string, WantedStarBase> _dispatchConfig;
    public static List<GroundVehicle> _activeGroundUnits;
    public static List<AttackHelicopter> _activeAttackHelis;
    public static List<TacticalHelicopter> _activeDropOffHelis;
    public static List<WaterVehicle> _activeBoatUnits;

    public static List<Ped> Police;

    private DateTime _lastDispatchTime;
    private const int DISPATCH_COOLDOWN_MS = 10000;
    private const float MIN_SPAWN_DISTANCE = 230f;
    private const float MAX_SPAWN_DISTANCE = 290f;
    private const float HELI_SPAWN_HEIGHT = 70f;
    private const int SUBTITLE_DURATION = 1000;

    public DispatchManager()
    {
        try
        {
            _activeGroundUnits = new List<GroundVehicle>();
            _activeAttackHelis = new List<AttackHelicopter>();
            _activeDropOffHelis = new List<TacticalHelicopter>();
            _activeBoatUnits = new List<WaterVehicle>();
            Police = new List<Ped>();
            _lastDispatchTime = DateTime.MinValue;

            Tick += OnTick;
            Aborted += OnAbort;
            HelperClass.Notification("WOI Ready.");
            LoadConfiguration();
            }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"Error: \n{ex.Message}, \n{ex.InnerException}, \n{ex.Source}, \n{ex.StackTrace}, \n{ex.Data}, \n{ex.TargetSite}.");
        }
    }

    public void Update()
    {
        try
        {
            if (Loaded)
            {
                if ((DateTime.Now - _lastDispatchTime).TotalMilliseconds >= DISPATCH_COOLDOWN_MS)
                {
                    ManageDispatch();
                    _lastDispatchTime = DateTime.Now;
                }
            }
            DisableDefaultDispatch();
        }

        catch (Exception ex)
        {
            Logger.Log.Fatal($"Update error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public void Cleanup()
    {
        OnAbort(this, EventArgs.Empty);
    }

    bool Loaded = false;
    private void LoadConfiguration()
    {
        try
        {
            if (!Loaded)
            {
                XMLReader.LoadAllInfos();
                _dispatchConfig = XMLReader.WantedLevels;
                ImportantChecks.RegionMappings = XMLReader.Regions;

                HelperClass.Notification("Loaded Everything!");
                Loaded = true;
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"Configuration loading error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        try
        {
            UpdateActiveUnits();
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"Tick error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void DispatchUnitsBasedOnEnvironment(WantedStarBase config)
    {
        try
        {
            Vector3 playerPos = Game.Player.Character.Position;

            bool nearWater = IsWaterDispatchFeasible(ImportantChecks.LastKnownLocation);
            bool inVehicle = Game.Player.Character.IsInVehicle();

            var seaUnits = config.Dispatches[DispatchType.Sea].DispatchSets;
            var groundUnits = config.Dispatches[DispatchType.Ground].DispatchSets;
            var airUnits = config.Dispatches[DispatchType.Air].DispatchSets;

            int maxSeaUnits = config.Dispatches[DispatchType.Sea].MaxUnits;
            int maxGroundUnits = config.GetMaxUnits(DispatchType.Ground);
            int maxAirUnits = config.GetMaxUnits(DispatchType.Air);

            Logger.Log.Info($@"
                  [Dispatch Config Dump]
                  Ground Units: {groundUnits.Count} / Max: {maxGroundUnits}
                    -> [{string.Join(", ", groundUnits.Select(g => g.Name))}]
                  Air Units: {airUnits.Count} / Max: {maxAirUnits}
                    -> [{string.Join(", ", airUnits.Select(a => a.Name))}]
                  Sea Units: {seaUnits.Count} / Max: {maxSeaUnits}
                    -> [{string.Join(", ", seaUnits.Select(s => s.Name))}]
                ");


            if (nearWater)
            {
                if (seaUnits.Any() && _activeBoatUnits.Count < maxSeaUnits)
                    DispatchBoatUnits(seaUnits, playerPos, maxSeaUnits);
            }

            bool isOnGround = World.GetGroundHeight(playerPos, out float groundZ, GetGroundHeightMode.Normal);

            if (isOnGround && Math.Abs(playerPos.Z - groundZ) < 40f) // optional: Z tolerance
            {
                if (groundUnits.Any() && _activeGroundUnits.Count < maxGroundUnits)
                    DispatchGroundUnits(groundUnits, playerPos, maxGroundUnits);
            }

            if (airUnits.Any() && (_activeAttackHelis.Count + _activeDropOffHelis.Count) < maxAirUnits)
                DispatchAirUnits(airUnits, playerPos, maxAirUnits);

        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"DispatchUnitsBasedOnEnvironment error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void DispatchGroundUnits(List<DispatchVehicleInfo> groundUnits, Vector3 targetPos, int maxUnits)
    {
        try
        {
            string currentRegion = ImportantChecks.CurrentJurisdiction;

            // Filter vehicle sets by region
            var regionFiltered = groundUnits
                .Where(set =>
                    set.Regions == null || set.Regions.Count == 0 ||
                    set.Regions.Contains("ALL", StringComparer.OrdinalIgnoreCase) ||
                    set.Regions.Contains(currentRegion, StringComparer.OrdinalIgnoreCase))
                .ToList();
            Logger.Log.Info($"Ground units available for region: {currentRegion}");
            if (!regionFiltered.Any())
            {
                Logger.Log.Fatal($"No ground units available for region: {currentRegion}");
                return;
            }

            int currentCount = _activeGroundUnits.Count;
            Logger.Log.Info($"Ground dispatch - Current: {currentCount}, Max: {maxUnits}, Available sets: {regionFiltered.Count}");

            // Track recent spawn points to avoid overlap
            List<Vector3> usedSpawnPoints = new List<Vector3>();

            while (currentCount < maxUnits && regionFiltered.Count > 0)
            {
                // Pick a random dispatch set
                var randomSet = regionFiltered[_random.Next(regionFiltered.Count)];
                
                // Try 5 times to get a good spawn
                bool spawnSuccess = false;
                for (int attempt = 0; attempt < 5 && !spawnSuccess; attempt++)
                {
                    // Apply position jitter
                    var offset = new Vector3(_random.Next(-15, 16), _random.Next(-15, 16), 0f);

                    var jitteredPos = targetPos + offset;
                    var minPos = 150f; var maxPos = 180f;
                    if (!Game.Player.Character.IsInVehicle())
                    {
                        minPos = MIN_SPAWN_DISTANCE;
                        maxPos = MAX_SPAWN_DISTANCE;
                    }
                    if (HelperClass.FindSpawnPointForAutomobile(Game.Player.Character, jitteredPos, minPos, maxPos, out Vector3 spawnPoint, out float heading))
                    {
                        // Check distance from previous spawns
                        bool tooClose = usedSpawnPoints.Any(p => p.DistanceTo(spawnPoint) < 20f);
                        if (tooClose)
                            continue;

                        // Create and register the vehicle
                        var vehicle = HelperClass.CreateVehicle(randomSet, spawnPoint, heading);
                        if (vehicle != null)
                        {
                            _activeGroundUnits.Add(new GroundVehicle(vehicle, randomSet));
                            usedSpawnPoints.Add(spawnPoint);
                            NotifyDispatch("Ground", spawnPoint, randomSet.Name);
                            currentCount++;
                            spawnSuccess = true;
                        }
                    }
                }

                if (!spawnSuccess)
                {
                    Logger.Log.Fatal("Failed to spawn ground unit after 5 attempts");
                    break; // Exit loop if we can't spawn
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"DispatchGroundUnits error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void DispatchAirUnits(List<DispatchVehicleInfo> airUnits, Vector3 targetPos, int maxUnits)
    {
        try
        {
            string currentRegion = ImportantChecks.CurrentJurisdiction;

            // Filter vehicle sets by region
            var regionFiltered = airUnits
                .Where(set =>
                    set.Regions == null || set.Regions.Count == 0 ||
                    set.Regions.Contains("ALL", StringComparer.OrdinalIgnoreCase) ||
                    set.Regions.Contains(currentRegion, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (!regionFiltered.Any())
            {
                Logger.Log.Fatal($"No air units available for region: {currentRegion}");
                return;
            }

            int currentCount = _activeAttackHelis.Count + _activeDropOffHelis.Count;
            Logger.Log.Info($"Air dispatch - Current: {currentCount}, Max: {maxUnits}, Available sets: {regionFiltered.Count}");

            while (currentCount < maxUnits && regionFiltered.Count > 0)
            {
                var vehicleInfo = regionFiltered[_random.Next(regionFiltered.Count)];

                if (HelperClass.FindSpawnPointForAircraft(Game.Player.Character, targetPos, MIN_SPAWN_DISTANCE, MAX_SPAWN_DISTANCE, HELI_SPAWN_HEIGHT, out Vector3 spawnPoint, out float heading))
                {
                    var heli = HelperClass.CreateVehicle(vehicleInfo, spawnPoint, heading);
                    if (heli != null)
                    {
                        if ((ShouldBeDropOffHeli(vehicleInfo, out string type) && !ImportantChecks.IsInOrAroundWater))
                        {
                            var tacticalHeli = new TacticalHelicopter(heli, vehicleInfo);
                            tacticalHeli.Land = type == "land";
                            tacticalHeli.Rappel = type == "rappel";
                            _activeDropOffHelis.Add(tacticalHeli);
                            NotifyDispatch("Tact", spawnPoint, vehicleInfo.Name);
                        }
                        else
                        {
                            var attackHeli = new AttackHelicopter(heli, vehicleInfo);
                            _activeAttackHelis.Add(attackHeli);
                            NotifyDispatch("Air", spawnPoint, vehicleInfo.Name);
                        }
                        currentCount++;
                    }
                }
                else
                {
                    Logger.Log.Fatal("Failed to find spawn point for air unit");
                    break; // Exit loop if we can't spawn
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"DispatchAirUnits error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void DispatchBoatUnits(List<DispatchVehicleInfo> boatUnits, Vector3 targetPos, int maxUnits)
    {
        try
        {
            string currentRegion = ImportantChecks.CurrentJurisdiction;

            // Filter vehicle sets by region
            var regionFiltered = boatUnits
                .Where(set =>
                    set.Regions == null || set.Regions.Count == 0 ||
                    set.Regions.Contains("ALL", StringComparer.OrdinalIgnoreCase) ||
                    set.Regions.Contains(currentRegion, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (!regionFiltered.Any())
            {
                Logger.Log.Fatal($"No boat units available for region: {currentRegion}");
                return;
            }

            int currentCount = _activeBoatUnits.Count;
            Logger.Log.Info($"Boat dispatch - Current: {currentCount}, Max: {maxUnits}, Available sets: {regionFiltered.Count}");

            while (currentCount < maxUnits && regionFiltered.Count > 0)
            {
                var vehicleInfo = regionFiltered[_random.Next(regionFiltered.Count)];
                var minPos = 150f; var maxPos = 180f;
                if (!Game.Player.Character.IsInVehicle())
                {
                    minPos = MIN_SPAWN_DISTANCE;
                    maxPos = MAX_SPAWN_DISTANCE;
                }
                if (HelperClass.FindSpawnPointForBoat(Game.Player.Character, targetPos, minPos, maxPos, out Vector3 spawnPoint, out float heading))
                {
                    var boat = HelperClass.CreateVehicle(vehicleInfo, spawnPoint, heading);
                    if (boat != null)
                    {
                        _activeBoatUnits.Add(new WaterVehicle(boat, vehicleInfo));
                        NotifyDispatch("Marine", spawnPoint, vehicleInfo.Name);
                        currentCount++;
                    }
                }
                else
                {
                    Logger.Log.Fatal("Failed to find spawn point for boat unit");
                    break; // Exit loop if we can't spawn
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"DispatchBoatUnits error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private const float MAX_ALLOWED_DISTANCE = 450f;

    private void UpdateActiveUnits()
    {
        Vector3 playerPos = Game.Player.Character.Position;

        // Cleanup inactive ground units
        _activeGroundUnits.RemoveAll(unit =>
        {
            bool remove = unit == null ||
                          !unit.IsVehicleValid() ||
                          unit.Vehicle.IsDead ||
                          unit.Driver?.IsDead == true ||
                          unit.Vehicle.Position.DistanceTo(playerPos) > MAX_ALLOWED_DISTANCE;

            if (remove)
                CleanupUnit(unit?.Vehicle, unit?.Driver, unit?.Crew);
            return remove;
        });

        // Cleanup inactive helis and boats
        _activeAttackHelis.RemoveAll(heli =>
        {
            bool remove = heli == null ||
                          !heli.IsHelicopterValid() ||
                          heli.Helicopter.IsDead ||
                          heli.Pilot?.IsDead == true ||
                          heli.Helicopter.Position.DistanceTo(playerPos) > MAX_ALLOWED_DISTANCE;

            if (remove)
                CleanupUnit(heli?.Helicopter, heli?.Pilot, heli?.Crew);
            return remove;
        });

        _activeDropOffHelis.RemoveAll(heli =>
        {
            bool remove = heli == null ||
                          !heli.IsHelicopterValid() ||
                          heli.Helicopter.IsDead ||
                          heli.Pilot?.IsDead == true ||
                          heli.Helicopter.Position.DistanceTo(playerPos) > MAX_ALLOWED_DISTANCE;

            if (remove)
                CleanupUnit(heli?.Helicopter, heli?.Pilot, heli?.Crew);
            return remove;
        });

        _activeBoatUnits.RemoveAll(boat =>
        {
            bool remove = boat == null ||
                          !boat.IsVehicleValid() ||
                          boat.Vehicle.IsDead ||
                          boat.Driver?.IsDead == true ||
                          boat.Vehicle.Position.DistanceTo(playerPos) > MAX_ALLOWED_DISTANCE;

            if (remove)
                CleanupUnit(boat?.Vehicle, boat?.Driver, boat?.Crew);
            return remove;
        });


        // Cleanup dead or faraway police peds
        Police.RemoveAll(ped =>
        {
            bool remove = ped == null || !ped.Exists() || ped.IsDead ||
                          ped.Position.DistanceTo(Game.Player.Character.Position) > MAX_ALLOWED_DISTANCE;

            if (remove)
            {
                CleanupUnit(null, ped, null); // Clean just the ped, no vehicle or crew
            }

            return remove;
        });

        // Update units
        foreach (var heli in _activeAttackHelis) heli.Update();
        foreach (var heli in _activeDropOffHelis) heli.Update();
        foreach (var car in _activeGroundUnits) car.Update();
        foreach (var boat in _activeBoatUnits) boat.Update();

        // Handle foot patrol tasking only once per star state switch
        bool hasGray = Game.Player.Wanted.HasGrayedOutStars;

        if (hasGray != _lastHadGrayedStars)
        {
            _lastHadGrayedStars = hasGray;

            Vector3 gotoPos = hasGray ? ImportantChecks.LastKnownLocation : Game.Player.Character.Position.Around(20);

            // Combine all crew members from all types
            var allFootCrew = _activeGroundUnits.SelectMany(x => x.Crew)
                .Concat(_activeAttackHelis.SelectMany(x => x.Crew))
                .Concat(_activeDropOffHelis.SelectMany(x => x.Crew))
                .Concat(_activeBoatUnits.SelectMany(x => x.Crew))
                .Where(p => p != null && p.Exists() && !p.IsDead && !p.IsInVehicle())
                .ToList();
            
            TaskGoto(gotoPos, allFootCrew);

            if(Game.Player.Wanted.WantedLevel ==0)
            foreach(var p in allFootCrew)
            {
                    if (allFootCrew.Count > 0)
                        p.MarkAsNoLongerNeeded();
            }

        }
    }

    public void SetupWantedLevelActivity(int wantedLevel)
    {
        // Combine all units
        var allCrew = _activeGroundUnits.SelectMany(x => x.Crew)
            .Concat(_activeAttackHelis.SelectMany(x => x.Crew))
            .Concat(_activeDropOffHelis.SelectMany(x => x.Crew))
            .Concat(_activeBoatUnits.SelectMany(x => x.Crew))
            .Where(p => p != null && p.Exists() && !p.IsDead)
            .ToList();

        //bool isPlayerShoot = false;

        //if (Game.Player.Character.IsShooting)
        //    isPlayerShoot = true;
        //else if (Game.Player.Wanted.WantedLevel == 0)
        //    isPlayerShoot = false;

        //    foreach (var ped in allCrew)
        //    {
        //        //bool hasWeapon = ped.Weapons.Current != null && ped.Weapons.Current.Hash != WeaponHash.Unarmed;

        //        if (wantedLevel < 4 && !isPlayerShoot)
        //        {
        //            ped.SetCombatAttribute(CombatAttributes.CanDoDrivebys, false);
        //        }
        //        else
        //        {
        //            ped.SetCombatAttribute(CombatAttributes.CanDoDrivebys, true);
        //        }
        //    }
    }


    private bool _lastHadGrayedStars = false;

    private void TaskGoto(Vector3 pos, List<Ped> crew)
    {
        foreach (var ped in crew)
        {
            if (ped != null && ped.Exists() && !ped.IsDead && !ped.IsInVehicle())
            {
                ped.MarkAsNoLongerNeeded();
                ped.Task.FollowNavMeshTo(pos, PedMoveBlendRatio.Sprint, 10000);
               // PathFind.ArePathNodesLoadedForArea(ped.Position)
            }
        }
    }


    public string ConvertToString(int wl)
        {
        string levelKey = wl switch
        {
            1 => "One",
            2 => "Two",
            3 => "Three",
            4 => "Four",
            5 => "Five",
            _ => string.Empty
        };

            return levelKey;
        }
    public void ManageDispatch()
    {
        
            if (!ShouldDispatch()) return;

            int currentWantedLevel = Game.Player.Wanted.WantedLevel;
            if (currentWantedLevel == 0) return;

        SetupWantedLevelActivity(currentWantedLevel);
            string levelKey = ConvertToString(currentWantedLevel);
        try
        {
            if (string.IsNullOrEmpty(levelKey) || _dispatchConfig == null || !_dispatchConfig.TryGetValue(levelKey, out var wantedConfig))
            {
                    Logger.Log.Fatal($"No configuration found for wanted level: {currentWantedLevel}");
                return;
            }
            DispatchUnitsBasedOnEnvironment(wantedConfig);
            _lastDispatchTime = DateTime.Now;
        }
        catch (Exception ex) {

            Logger.Log.Fatal($"Managing dispatch for wanted level: {currentWantedLevel}");
            
        }
    }

    private bool ShouldDispatch()
    {
        return (DateTime.Now - _lastDispatchTime).TotalMilliseconds >= DISPATCH_COOLDOWN_MS && Police.Count < 30;
    }

    private static readonly Random _random = new Random();

    private bool ShouldBeDropOffHeli(DispatchVehicleInfo vehicleInfo, out string type)
    {
        type = null;

        var tasks = vehicleInfo?.Tasks;
        if (tasks == null || tasks.Count == 0)
            return false;

        // Drop-off types
        var dropOffTasks = new List<string> { "rappel", "land" };

        // Pick one randomly
        type = tasks[_random.Next(tasks.Count)];

        // Return true if it matches a drop-off type
        return dropOffTasks.Contains(type);
    }

    
    private void NotifyDispatch(string unitType, Vector3 spawnPoint, string modelName)
    {
        var playerPos = Game.Player.Character?.Position;
        if (playerPos == null) return;

        float distance = playerPos.Value.DistanceTo(spawnPoint);
        string direction = GetCardinalDirection(playerPos.Value, spawnPoint);

        string msg = unitType switch
        {
            "Ground" => DispatchMessages.GetRandomMessage(DispatchMessages.Ground, modelName),
            "Air" => DispatchMessages.GetRandomMessage(DispatchMessages.Air, modelName),
            "Marine" => DispatchMessages.GetRandomMessage(DispatchMessages.Sea, modelName),
            _ => null
        };

        if (!string.IsNullOrEmpty(msg))
        {
            string street = World.GetStreetName(spawnPoint);
            HelperClass.Notification($"~y~{msg}~w~~n~~o~Coming from {direction} near {street}~w~.");
        }

        else
            HelperClass.Notification($"~y~{unitType} Units~w~ dispatched");

        Logger.Log.Info($"[Dispatch] {unitType} dispatched from {modelName}, {distance:F0}m {direction}");
    }


    private string GetCardinalDirection(Vector3 from, Vector3 to)
    {
        Vector3 diff = to - from;
        float angle = (float)Math.Atan2(diff.X, diff.Y) * 57.295776f;
        angle = (angle + 360) % 360;

        if (angle >= 337.5 || angle < 22.5) return "North";
        if (angle < 67.5) return "Northeast";
        if (angle < 112.5) return "East";
        if (angle < 157.5) return "Southeast";
        if (angle < 202.5) return "South";
        if (angle < 247.5) return "Southwest";
        if (angle < 292.5) return "West";
        return "Northwest";
    }

    public enum di
    {
        DT_Invalid,
        DT_PoliceAutomobile,
        DT_PoliceHelicopter,
        DT_FireDepartment,
        DT_SwatAutomobile,
        DT_AmbulanceDepartment,
        DT_PoliceRiders,
        DT_PoliceVehicleRequest,
        DT_PoliceRoadBlock,
        DT_PoliceAutomobileWaitPulledOver,
        DT_PoliceAutomobileWaitCruising,
        DT_Gangs,
        DT_SwatHelicopter,
        DT_PoliceBoat,
        DT_ArmyVehicle,
        DT_BikerBackup
    };

    private void DisableDefaultDispatch(bool abort = false)
    {
        if (Game.IsMissionActive || abort)
        {
            var dispatchServices = new int[] { 1, 2, 4, 12, 13,};

            foreach (var service in dispatchServices)
            {
                Function.Call(Hash.ENABLE_DISPATCH_SERVICE, service, true);
            }
        }
        
        else 
        {
            var dispatchServices = new int[] { 1, 2, 4, 12, 13, };

            foreach (var service in dispatchServices)
            {
               // Game.Player.Wanted.
                Function.Call(Hash.ENABLE_DISPATCH_SERVICE, service, false);
            }
        }
    }

    private void OnAbort(object sender, EventArgs e)
    {
        foreach (var unit in _activeGroundUnits)
        {
            CleanupUnit(unit?.Vehicle, unit?.Driver, unit?.Crew);
            unit.Crew.Clear();
        }

        _activeGroundUnits.Clear();

        foreach (var heli in _activeAttackHelis)
        {
            CleanupUnit(heli?.Helicopter, heli?.Pilot, heli?.Crew);
            heli.Crew.Clear();
        }
        _activeAttackHelis.Clear();

        foreach (var heli in _activeDropOffHelis)
        {
            CleanupUnit(heli?.Helicopter, heli?.Pilot, heli?.Crew);
            heli.Crew.Clear();
        }
        _activeDropOffHelis.Clear();

        foreach (var boat in _activeBoatUnits)
        {
            CleanupUnit(boat?.Vehicle, boat?.Driver, boat?.Crew);
            boat.Crew.Clear();
        }
        _activeBoatUnits.Clear();

        CleanupUnit(null, null, Police);

        DisableDefaultDispatch(true);
    }

    private void CleanupUnit(Vehicle vehicle, Ped driver, List<Ped> passengers = null)
    {
        if (passengers != null)
        {
            foreach (var p in passengers)
            {
                if (p?.Exists() == true)
                {
                    p.MarkAsNoLongerNeeded();
                    // p.Delete();
                }
            }
            passengers.Clear();
        }

        if (driver?.Exists() == true)
        {
            driver.MarkAsNoLongerNeeded();
            // driver.Delete();
        }

        if (vehicle?.Exists() == true)
        {
            vehicle.MarkAsNoLongerNeeded();
            // vehicle.Delete();
        }
    }

    private bool IsWaterDispatchFeasible(Vector3 playerPos)
    {
        float waterHeight = HelperClass.GetWaterHeight(playerPos);
        float verticalDifference = playerPos.Z - waterHeight;

        // Must be over valid water
        bool isAboveWater = waterHeight > 0f && verticalDifference >= 0f && verticalDifference <= 50f;

        // Refined "on surface" check (for swimming/beach logic)
        bool isAtWaterSurface = verticalDifference <= 5.5f;
       // bool 
        // Check if boats can be spawned around the player
        bool hasSpawnPoint = HelperClass.FindNearbyWaterArea(playerPos, 20, 40);

        Logger.Log.Fatal($"[WaterCheck] WaterHeight: {waterHeight:F2}, PlayerZ: {playerPos.Z:F2}, ΔZ: {verticalDifference:F2}, NearSurface: {isAtWaterSurface}, SpawnAvailable: {hasSpawnPoint}");

        return isAboveWater && hasSpawnPoint;
    }

}