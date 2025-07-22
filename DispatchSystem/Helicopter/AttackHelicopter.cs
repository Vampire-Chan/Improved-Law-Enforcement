using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AttackHelicopter
{
    // --- Configuration Constants ---
    private const float DETECTION_RADIUS = 300f;
    private const float ATTACK_DISTANCE = 70f;
    private const float PATROL_DISTANCE = 50f;

    // Heights for different operations
    private const int ATTACK_HEIGHT = 50;
    private const int PATROL_HEIGHT = 30;
    private const int SEARCH_HEIGHT = 80;
    private const int FLEE_HEIGHT = 120;

    // Speeds for different operations
    private const float ATTACK_SPEED = 55;
    private const float PATROL_SPEED = 40;
    private const float SEARCH_SPEED = 30;
    private const float FLEE_SPEED = 65;

    // Update intervals
    private const double STATE_UPDATE_INTERVAL = 0.5;
    private const double MISSION_CHECK_INTERVAL = 2.0;

    // Health and damage thresholds
    private const float CRITICAL_HEALTH_THRESHOLD = 0.6f;
    private const float INTENSE_CRITICAL_HEALTH_THRESHOLD = 0.3f;
    private const float CRASH_HEIGHT_THRESHOLD = 5f;
    private const float ENGAGEMENT_RANGE = 200f;

    public enum HelicopterState
    {
        Idle,
        ReadyToInitial,
        GoToInitial,
        ReadyToPatrol,
        Patrol,
        ReadyToEngage,
        Engage,
        ReadyToSearch,
        Search,
        ReadyToFlee,
        Flee,
        GoingToCrash, // heli is fleeing but not crashing yet
        ReadyToJump, //critical stance jump out everyone...
    }

    // --- Properties ---
    public Vehicle Helicopter { get; }
    public Ped Pilot { get; }
    public HelicopterState CurrentState { get; private set; } = HelicopterState.Idle;
    public bool IsArmed { get; private set; } = false;
    public bool HasArmedPassengers { get; private set; } = false;
    public Vector3 LastKnownPlayerPosition { get; private set; }
    public float SearchRadius { get; set; } = 250f;
    public bool IsAnnihilatorType = false;

    // --- Private Fields ---
    private DispatchVehicleInfo Info;
    private DateTime _lastStateUpdate = DateTime.MinValue;
    public List<Ped> Crew = new List<Ped>();
    private bool _playerInVehicle = false;
    private Random _random = new Random();
    private VehicleMissionType _currentMissionType = VehicleMissionType.None;
    private Vector3 _lastSearchPoint = Vector3.Zero;
    private int _searchPointIndex = 0;
    private List<Vector3> _searchPoints = new List<Vector3>();

    // --- Constructor ---
    public AttackHelicopter(Vehicle helicopter, DispatchVehicleInfo info)
    {
        Helicopter = helicopter ?? throw new ArgumentNullException(nameof(helicopter));
        Info = info ?? throw new ArgumentNullException(nameof(info));

        if (Game.Player?.Character == null)
        {
            throw new InvalidOperationException("Player character not available");
        }

        LastKnownPlayerPosition = ImportantChecks.LastKnownLocation;
        Pilot = Helicopter.Driver;
        InitializeHelicopter();
    }

    // --- Initialization ---
    private void InitializeHelicopter()
    {
        if (!IsHelicopterValid()) return;

        DetermineWeaponCapabilities();
        CurrentState = HelicopterState.ReadyToInitial;
        Crew = Helicopter.Passengers.ToList();

        // Enhanced helicopter setup
        Helicopter.SetFoldingWingsDeployed(true);
        Helicopter.SetArriveDistanceOverrideForVehiclePersuitAttack(50);

        // Improved crew setup
        SetupCrew();
        GenerateSearchPoints();
    }

    private void SetupCrew()
    {
        foreach (var crewMember in Crew)
        {
            if (crewMember == null || !crewMember.Exists()) continue;

            crewMember.CanSwitchWeapons = true;
            crewMember.SeeingRange = ENGAGEMENT_RANGE;
            crewMember.HearingRange = ENGAGEMENT_RANGE;
            crewMember.SetCombatAttribute(CombatAttributes.CanFightArmedPedsWhenNotArmed, true);
            crewMember.SetCombatAttribute(CombatAttributes.CanLeaveVehicle, false); // Initially prevent leaving
            //crewMember.BlockPermanentEvents = true;

        }
    }

    private void GenerateSearchPoints()
    {
        _searchPoints.Clear();
        Vector3 center = LastKnownPlayerPosition;

        // Generate 6 search points in a hexagonal pattern
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * (float)Math.PI / 180f;
            Vector3 searchPoint = new Vector3(
                center.X + (float)Math.Cos(angle) * SearchRadius,
                center.Y + (float)Math.Sin(angle) * SearchRadius,
                center.Z + SEARCH_HEIGHT
            );
            _searchPoints.Add(searchPoint);
        }
    }

    private void DetermineWeaponCapabilities()
    {
        if (!IsHelicopterValid()) return;

        // Check pilot weapon capabilities
        IsArmed = Pilot.GetVehicleWeaponHash(out var weaponHash);

        // Enhanced weapon setup
        if (IsArmed)
        {
            //Pilot.CanSwitchWeapons = true;
            Pilot.SetPedCycleVehicleWeapon();

            // Set appropriate combat attributes for armed helicopters
            Pilot.SetCombatAttribute(CombatAttributes.UseVehicleAttack, false); // Will be enabled when 
            Pilot.SetConfigFlag(PedConfigFlagToggles.CanAttackFriendly, false);
            Pilot.FiringPattern = FiringPattern.FullAuto;
        }

        // Check if should flee immediately
        if (Game.Player.Wanted.WantedLevel == 0 ||
            (!HasAnyPassengers && !IsArmed))
        {
            CurrentState = HelicopterState.Flee;
        }

        Pilot.SeeingRange = ENGAGEMENT_RANGE;
        _playerInVehicle = Game.Player.Character.IsInVehicle();
    }

    float heightAboveGround;
    // --- Main Update Loop ---
    public void Update()
    {
        if (!IsHelicopterValid()) return;

        heightAboveGround = GetHeightAboveGround();

        UpdatePlayerStatus();
        UpdateHelicopterBehavior();

    }


    private float GetHeightAboveGround()
    {
        OutputArgument groundZ = new OutputArgument();
        Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD,
            Helicopter.Position.X, Helicopter.Position.Y, Helicopter.Position.Z, groundZ);

        return Helicopter.Position.Z - groundZ.GetResult<float>();
    }

    // --- Player Status Updates ---
    private void UpdatePlayerStatus()
    {
        if (Game.Player?.Character == null) return;

        _playerInVehicle = Game.Player.Character.IsInVehicle();
    }


    // --- Main State Management ---
    private void UpdateHelicopterBehavior()
    {
        DateTime now = DateTime.Now;
        if ((now - _lastStateUpdate).TotalSeconds < STATE_UPDATE_INTERVAL) return;
        _lastStateUpdate = now;

        int wantedLevel = Game.Player.Wanted.WantedLevel;
        bool hasGreyStars = Game.Player.Wanted.HasGrayedOutStars;

        // Critical override conditions - but not during initial approach
        if (wantedLevel == 0 || ShouldFleeFromDamage() || (!HasAnyPassengers && !IsArmed))
        {
            CurrentState = HelicopterState.ReadyToFlee;
        }

        float distanceToTarget = Helicopter.Position.DistanceTo(LastKnownPlayerPosition);
        float currentSpeed = Helicopter.Speed;

        if(CriticalHealth())
        {
            CurrentState = HelicopterState.GoingToCrash;
            if (Helicopter.IsEngineRunning == false) CurrentState = HelicopterState.ReadyToJump;
        }

        //HelperClass.Subtitle($"Heli State: {CurrentState} | Mission: {Helicopter.GetActiveMissionType()} | Dist: {distanceToTarget:F1}m | Speed: {(currentSpeed * 3.6f):F1}km/h | Height: {heightAboveGround:F1}m");

        ExecuteCurrentState(wantedLevel, hasGreyStars, distanceToTarget);
    }

    private void ExecuteCurrentState(int wantedLevel, bool hasGreyStars, float distanceToTarget)
    {
        // State machine logic within the same method
        switch (CurrentState)
        {
            case HelicopterState.ReadyToInitial:
                // Execute state action
                Helicopter.IsSirenActive = true;
                DisableWeapons();

                if (Helicopter.Model.IsHelicopter) Pilot.Task.StartHeliMission(
                    Helicopter,
                    LastKnownPlayerPosition,
                    VehicleMissionType.GoTo,
                    PATROL_SPEED,
                    40,
                    -1,
                    PATROL_HEIGHT
                );

                if (Helicopter.Model.IsPlane) Pilot.Task.StartPlaneMission(Helicopter, LastKnownPlayerPosition, VehicleMissionType.GoTo, 50, 30, -1, 70, -1, true);

               // if(Helicopter.)
                    _currentMissionType = VehicleMissionType.GoTo;

                // State transition
                CurrentState = HelicopterState.GoToInitial;
                break;

            case HelicopterState.GoToInitial:
                // Check flee conditions
                if (wantedLevel == 0)
                {
                    CurrentState = HelicopterState.ReadyToFlee;
                    return;
                }

                // State transition based on conditions
                if (distanceToTarget < 80f || Helicopter.GetActiveMissionType() == VehicleMissionType.None)
                {
                    if (hasGreyStars)
                        CurrentState = HelicopterState.ReadyToSearch;
                    else if (wantedLevel >= 4 && IsArmed)
                        CurrentState = HelicopterState.ReadyToEngage;
                    else
                        CurrentState = HelicopterState.ReadyToPatrol;
                }
                break;

            case HelicopterState.ReadyToPatrol:
                // Execute state action
                Helicopter.IsSirenActive = true;
                DisableWeapons();

                VehicleMissionType missionType = _playerInVehicle ? VehicleMissionType.Follow : VehicleMissionType.Circle;

               if(Helicopter.IsHelicopter) Pilot.Task.StartHeliMission(
                    Helicopter,
                    LastKnownPlayerPosition,
                    missionType,
                    PATROL_SPEED,
                    PATROL_DISTANCE,
                    -1,
                    PATROL_HEIGHT
                );

                if (Helicopter.Model.IsPlane) Pilot.Task.StartPlaneMission(Helicopter, LastKnownPlayerPosition, VehicleMissionType.Circle, 50, 30, -1, 70, -1, true);


                _currentMissionType = missionType;

                // State transition
                CurrentState = HelicopterState.Patrol;
                break;

            case HelicopterState.Patrol:
                // Execute state action
                Pilot.Task.StartHeliMission(
                    Helicopter,
                    Game.Player.Character,
                    VehicleMissionType.Circle,
                    PATROL_SPEED,
                    PATROL_DISTANCE,
                    -1,
                    PATROL_HEIGHT
                );

                // State transitions
                if (hasGreyStars)
                    CurrentState = HelicopterState.ReadyToSearch;
                else if (wantedLevel >= 4 && IsArmed)
                    CurrentState = HelicopterState.ReadyToEngage;
                break;

            case HelicopterState.ReadyToEngage:
                // Execute state action
                Helicopter.IsSirenActive = true;
                EnableWeapons();

                VehicleMissionType engageMissionType;
                float speed, distance;
                int height;

                if (IsArmed)
                {
                    engageMissionType = VehicleMissionType.Attack;
                    speed = ATTACK_SPEED;
                    distance = ATTACK_DISTANCE;
                    height = ATTACK_HEIGHT;
                }
                else
                {
                    engageMissionType = VehicleMissionType.Circle;
                    speed = ATTACK_SPEED;
                    distance = 60f;
                    height = ATTACK_HEIGHT;
                }

               if(Helicopter.IsHelicopter) Pilot.Task.StartHeliMission(
                    Helicopter,
                    Game.Player.Character,
                    engageMissionType,
                    speed,
                    distance,
                    -1,
                    height
                );

                if (Helicopter.Model.IsPlane) Pilot.Task.StartPlaneMission(Helicopter, Game.Player.Character, VehicleMissionType.Attack, 50, 30, -1, 70, -1);

                _currentMissionType = engageMissionType;

                // State transition
                CurrentState = HelicopterState.Engage;
                break;

            case HelicopterState.Engage:
                // Check if mission is still active, restart if needed
                if (Helicopter.GetActiveMissionType() == VehicleMissionType.None)
                {
                    CurrentState = HelicopterState.ReadyToEngage;
                    return;
                }

                // Execute state action
                if (!IsArmed)
                {
                    Pilot.Task.StartHeliMission(
                        Helicopter,
                        Game.Player.Character,
                        VehicleMissionType.GoTo,
                        60f,
                        PATROL_DISTANCE,
                        -1,
                        PATROL_HEIGHT
                    );
                }

                // State transitions
                if (hasGreyStars)
                    CurrentState = HelicopterState.ReadyToSearch;
                else if (!IsArmed)
                    CurrentState = HelicopterState.ReadyToPatrol;

                if (wantedLevel < 4)
                    CurrentState = HelicopterState.ReadyToPatrol;
                break;

            case HelicopterState.ReadyToSearch:
                // Execute state action
                DisableWeapons();
                Helicopter.IsSirenActive = false;
                 

                Vector3 searchPoint = GetNextSearchPoint();

                TaskSequence seq = new TaskSequence();
               if(Helicopter.Model.IsHelicopter) seq.AddTask.StartHeliMission(Helicopter, searchPoint, VehicleMissionType.GoTo, SEARCH_SPEED, 0, -1, SEARCH_HEIGHT);
                if (Helicopter.Model.IsHelicopter) seq.AddTask.StartHeliMission(Helicopter, searchPoint, VehicleMissionType.Circle, SEARCH_SPEED, 50f, -1, SEARCH_HEIGHT);
                if (Helicopter.Model.IsPlane) seq.AddTask.StartPlaneMission(Helicopter, searchPoint, VehicleMissionType.Circle, SEARCH_SPEED, -1, 20, 80);
                    if (Helicopter.Model.IsPlane)
                        seq.Close();

                Pilot.Task.PerformSequence(seq);
                _currentMissionType = VehicleMissionType.GoTo;
                _lastSearchPoint = searchPoint;

                // State transition
                CurrentState = HelicopterState.Search;
                break;

            case HelicopterState.Search:
                // Check if current search point is completed
                if (Vector3.Distance(Helicopter.Position, _lastSearchPoint) < 30f &&
                    Helicopter.GetActiveMissionType() == VehicleMissionType.None)
                {
                    CurrentState = HelicopterState.ReadyToSearch;
                    return;
                }

                // State transitions
                if (!hasGreyStars && wantedLevel > 0)
                {
                    if (wantedLevel >= 4 && IsArmed)
                        CurrentState = HelicopterState.ReadyToEngage;
                    else
                        CurrentState = HelicopterState.ReadyToPatrol;
                }
                break;

            case HelicopterState.ReadyToFlee:
                // Execute state action
                DisableWeapons();
                Helicopter.IsSirenActive = false;
                Pilot.BlockPermanentEvents = true;

                if (Helicopter.Model.IsHelicopter) Pilot.Task.StartHeliMission(
                    Helicopter,
                    LastKnownPlayerPosition,
                    VehicleMissionType.Flee,
                    FLEE_SPEED,
                    0,
                    -1,
                    FLEE_HEIGHT
                );

                if (Helicopter.Model.IsPlane) Pilot.Task.StartPlaneMission(Helicopter, LastKnownPlayerPosition, VehicleMissionType.Flee, 70, -1, -1, 90);
                    _currentMissionType = VehicleMissionType.Flee;

                // State transition
                CurrentState = HelicopterState.Flee;
                break;

            case HelicopterState.Flee:
                // State transition
                CurrentState = HelicopterState.Idle;
                break;

            case HelicopterState.GoingToCrash:
                CurrentState = HelicopterState.ReadyToFlee;
                // Execute state action
                foreach (var crewMember in Crew)
                {
                    if (crewMember == null || !crewMember.Exists()) continue;

                    crewMember.SetCombatAttribute(CombatAttributes.CanLeaveVehicle, true);
                    crewMember.BlockPermanentEvents = false;
                }

                Helicopter.LandingGearState = VehicleLandingGearState.Deployed;
                Helicopter.ClearPrimaryTask();

                // State transition
                CurrentState = HelicopterState.Idle;
                break;

            case HelicopterState.ReadyToJump:
                // Execute state action
                if (heightAboveGround < 20f)
                {
                    CurrentState = HelicopterState.GoingToCrash;
                    return;
                }

                foreach (Ped soldier in Crew)
                {
                    if (soldier == null || !soldier.Exists() || soldier.IsDead) continue;

                    soldier.BlockPermanentEvents = false;
                    soldier.SetCombatAttribute(CombatAttributes.CanLeaveVehicle, true);
                    soldier.Weapons.Give(WeaponHash.Parachute, 1, true, true);

                    World.GetSafePositionForPed(Helicopter.Position.Around(30), out var landingPos, GetSafePositionFlags.NotWater);

                    TaskSequence jumpSequence = new TaskSequence();
                    jumpSequence.AddTask.LeaveVehicle(LeaveVehicleFlags.BailOut);
                    jumpSequence.AddTask.Pause(2000);
                    jumpSequence.AddTask.UseParachute();
                    jumpSequence.AddTask.ParachuteTo(landingPos);
                    jumpSequence.AddTask.CombatHatedTargetsAroundPed(80);
                    jumpSequence.Close();

                    soldier.Task.PerformSequence(jumpSequence);
                }

                // State transition
                CurrentState = HelicopterState.Idle;
                break;

            case HelicopterState.Idle:
            default:
                // No action required for idle state
                break;
        }
    }


    // --- Enhanced Helper Methods ---
    private Vector3 GetNextSearchPoint()
    {
        if (_searchPoints.Count == 0)
        {
            GenerateSearchPoints();
        }

        Vector3 searchPoint = _searchPoints[_searchPointIndex];
        _searchPointIndex = (_searchPointIndex + 1) % _searchPoints.Count;

        // Update search points if player moved significantly
        if (Vector3.Distance(LastKnownPlayerPosition, _searchPoints[0]) > SearchRadius * 0.5f)
        {
            GenerateSearchPoints();
            _searchPointIndex = 0;
            searchPoint = _searchPoints[0];
        }

        return searchPoint;
    }

    private bool ShouldFleeFromDamage()
    {
        if (!IsHelicopterValid()) return true;

        float healthPercentage = (float)Helicopter.Health / Helicopter.MaxHealth;
        return healthPercentage < CRITICAL_HEALTH_THRESHOLD;
    }

    private bool CriticalHealth()
    {
        if (!IsHelicopterValid()) return true;

        float healthPercentage = (float)Helicopter.Health / Helicopter.MaxHealth;
        return healthPercentage < CRITICAL_HEALTH_THRESHOLD;
    }

    // --- Enhanced Weapon Management ---
    private void DisableWeapons()
    {
        if (!IsHelicopterValid()) return;

        foreach (var crewMember in Crew)
        {
            if (crewMember == null || !crewMember.Exists()) continue;

            crewMember.SetCombatAttribute(CombatAttributes.UseVehicleAttack, false);
        }
    }

    private void EnableWeapons()
    {
        if (!IsHelicopterValid()) return;

        foreach (var crewMember in Crew)
        {
            if (crewMember == null || !crewMember.Exists()) continue;

            crewMember.SetCombatAttribute(CombatAttributes.UseVehicleAttack, true);

        }
    }

    // --- Utility Methods ---
    public bool IsHelicopterValid()
    {
        return Helicopter != null && Helicopter.Exists() && !Helicopter.IsDead &&
               Pilot != null && Pilot.Exists() && !Pilot.IsDead;
    }

    //while ignoring the pilotseats
    public bool HasAnyPassengers
    {
        get
        {
            if (Crew == null || Crew.Count == 0) return false;

            foreach (Ped soldier in Crew)
            {
                if (soldier == null || !soldier.Exists() || soldier.IsDead || soldier.SeatIndex == VehicleSeat.LeftFront)
                    continue;

                if (soldier.IsInVehicle(Helicopter))
                    return true;
            }

            return false;
        }
    }

    // --- Enhanced Weapon Setup Methods ---


    public bool ApplyWeaponAmmo(VehicleWeaponHash weaponHash, int ammoCount)
    {
        if (!IsHelicopterValid()) return false;

        try
        {
            Helicopter.SetWeaponRestrictedAmmo((int)weaponHash, ammoCount);
            return true;
        }
        catch
        {
            return false;
        }
    }
}