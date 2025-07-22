using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Vector3 = GTA.Math.Vector3;

//make a possible searching of height at 20 -30f, means make a possible choice that helito reach that position but of height 20-30 of found point, means try to find the grounded position.
public class TacticalHelicopter
{
    public bool Rappel { get; set; }
    public bool Land { get; set; }
    public List<Ped> Crew = new List<Ped>();
    public bool CanRappel { get; private set; }
    public Vehicle Helicopter { get; private set; }
    public Ped Pilot { get; private set; }

    private Dictionary<VehicleWeaponHash, bool> _weaponStates;
    private DispatchVehicleInfo Info;
    private Vector3 DropZone { get; set; }
    private Task CurrentTask { get; set; } = Task.None;
    private Task PreviousTask { get; set; } = Task.None; // Track previous task for resumption
    private DateTime lastStateChange = DateTime.Now;
    private DateTime rappelCooldown = DateTime.MinValue;
    private DateTime landingCooldown;
    private DateTime lastHeightCheck = DateTime.Now;
    private DateTime lastSpeedUpdate = DateTime.Now;
    private DateTime lastPlayerSightCheck = DateTime.Now;
    private DateTime searchStartTime = DateTime.MinValue;
    private float lastRecordedHeight = 0f;
    private int stuckHeightCounter = 0;
    private bool shouldForceHeightMapAvoidance;
    private bool isPlayerOnFoot = true;
    private bool troopsDeploying = false;
    private bool playerInSight = true;
    private bool isSearching = false;
    private bool missionInterrupted = false;
    bool createExtra = false;

    public enum CrewLeaveOption
    {
        OnlyCrew,
        CoPilotAndCrew,
        All
    }

    public struct HeliRappelData
    {
        public static readonly CrClipDictionary EntryLeftSeatDictionary = new CrClipDictionary("veh@helicopter@rds@enter_exit");
        public static readonly CrClipDictionary EntryRightSeatDictionary = new CrClipDictionary("veh@helicopter@rps@enter_exit");
        public static readonly CrClipAsset EntryLeftSeatAnimation = new CrClipAsset(EntryLeftSeatDictionary, "get_in_extra");
        public static readonly CrClipAsset EntryRightSeatAnimation = new CrClipAsset(EntryRightSeatDictionary, "get_in_extra");
    }

    private enum Task
    {
        None,
        GoToPosition,
        WaitingForRappel,
        RappelingInProgress,
        RappelComplete,
        StartLanding,
        LandingInProgress,
        CooldownBeforeCrewExit, // New - cooldown phase before crew exits
        LandingComplete,
        Paratrooping,
        ParatroopInProgress,
        ParatroopComplete,
        Flee,
        EmergencyBailout,
        Following,
        Searching,
        Intercepting
    }
    private enum LandingState
    {
        GoToPosition,
        CheckReadyToLand,
        HoverAboveLanding,       // New - approach + hover point above target
        ExecuteLanding,
        LandingInProgress,
        CooldownBeforeCrewExit,  // New - cooldown after touching down
        LandingComplete,
        CrewDeployment
    }

    private enum RappelState
    {
        GoToPosition,
        CheckReadyToRappel,
        PositionForRappel,
        ExecuteRappel,
        RappelInProgress,
        RappelComplete
    }


    public TacticalHelicopter(Vehicle helicopter, DispatchVehicleInfo info)
    {
        Helicopter = helicopter ?? throw new ArgumentNullException(nameof(helicopter));
        Info = info ?? throw new ArgumentNullException(nameof(info));
        Vector3 dropzone = Vector3.Zero;

        if (Rappel)
            FindLocationForDeployment(ImportantChecks.LastKnownLocation.Around(10), out dropzone, true, false);
        else if (Land)
            FindLocationForDeployment(ImportantChecks.LastKnownLocation.Around(10), out dropzone, false);
        else
            dropzone = ImportantChecks.LastKnownLocation.Around(25f); // or use fallback logic

        DropZone = dropzone;

        //test cases, now works so let it be commented or deleted makes no sense
        //switch (new Random().Next(0, 2))
        //{
        //    case 0:
        //        Land = true;
        //        break;
        //    case 1:
        //        Rappel = true;
        //        break;
        //}

        Pilot = Helicopter.Driver;
        InitializeHelicopter();
        DetermineInitialTask();
    }
    private void InitializeHelicopter()
    {
        Vector3 testStart = Helicopter.Position;
        Vector3 testEnd = new Vector3(DropZone.X, DropZone.Y, DropZone.Z);

        ShapeTestHandle shapeTest = ShapeTest.StartTestLOSProbe(testEnd, testStart);
        ShapeTestResult result;

        while (shapeTest.GetResult(out result) == ShapeTestStatus.NotReady)
        {
            Script.Yield();
        }

        createExtra = Helicopter.PassengerCapacity >= 3 ? true : false;

        shouldForceHeightMapAvoidance = result.DidHit;
        CanRappel = Helicopter.AllowRappel;
        Crew = Helicopter.Passengers.ToList();
        Pilot.SeeingRange = 200f;
        lastRecordedHeight = GetHeightAboveGround();
    }

    private void DetermineInitialTask()
    {
        if (!IsHelicopterValid() || Game.Player?.Character == null || Game.Player.Wanted.WantedLevel == 0 || DropZone == Vector3.Zero)
        {
            StartFleeTask();
            return;
        }

        if (!HasAnyPassengers)
        {
            StartFleeTask();
            return;
        }

        isPlayerOnFoot = !Game.Player.Character.IsInVehicle();
        Pilot.BlockPermanentEvents = true;

        if (Land)
        {
            StartLandingSequence();
        }
        else if (CanRappel)
        {
            StartRappelSequence();
        }
        else
        {
            ConvertToAttackHelicopter();
        }
    }
    public void Update()
    {
        if (!IsHelicopterValid())
        {
            return;
        }

        // Check for damage first - highest priority
        if (ShouldFleeFromDamage() || !HasAnyPassengers)
        {
            CurrentTask = Task.None;
            currentLandingState = LandingState.LandingComplete;
            if(currentRappelState != RappelState.RappelInProgress) currentRappelState = RappelState.RappelComplete;
            StartFleeTask();
            return;
        }

        float heightAboveGround = GetHeightAboveGround();
     

        UpdatePlayerState();
        UpdatePlayerSightTracking();
        UpdateHelicopterState();
    }

    private bool ShouldFleeFromDamage()
    {
        float healthPercentage = (float)Helicopter.Health / Helicopter.MaxHealth;
        return healthPercentage < .6f;
    }

    private void UpdatePlayerState()
    {
        if (Game.Player?.Character == null) return;

        bool playerCurrentlyOnFoot = !Game.Player.Character.IsInVehicle();
        Vector3 currentPlayerPosition = Game.Player.Character.Position;

        // Update last known position
       
        // Player state change detection
        if (isPlayerOnFoot && !playerCurrentlyOnFoot && !troopsDeploying)
        {
            bool shouldInterrupt = (CurrentTask == Task.GoToPosition ||
                                  CurrentTask == Task.WaitingForRappel ||
                                  CurrentTask == Task.StartLanding ||
                                  CurrentTask == Task.LandingInProgress) &&
                                 !Game.Player.Wanted.HasGrayedOutStars;

            if (shouldInterrupt)
            {
                FindLocationForDeployment(ImportantChecks.LastKnownLocation.Around(10), out var dropZone, true);
                DropZone = dropZone;
                InterruptMissionForPursuit();
            }
        }

        isPlayerOnFoot = playerCurrentlyOnFoot;
    }

    private void UpdatePlayerSightTracking()
    {
        if ((DateTime.Now - lastPlayerSightCheck).TotalSeconds < 5f)
            return;

        lastPlayerSightCheck = DateTime.Now;
        bool currentSightStatus = IsPlayerInSight();

        if (playerInSight && !currentSightStatus)
        {
            // Player lost from sight
            playerInSight = false;
            if (CurrentTask != Task.Searching && CurrentTask != Task.Flee && !troopsDeploying)
            {
                StartSearchTask();
            }
        }
        else if (!playerInSight && currentSightStatus)
        {
            // Player found again
            playerInSight = true;
            if (isSearching)
            {
                ResumeInterruptedMission();
            }
        }

        playerInSight = currentSightStatus;
    }

    private bool IsPlayerInSight()
    {
        return !Game.Player.Wanted.HasGrayedOutStars;
    }

    private void InterruptMissionForPursuit()
    {
        PreviousTask = CurrentTask;
        missionInterrupted = true;

        Helicopter.IsSirenActive = true;
        //Pilot.Task.ClearAll();
        Pilot.Task.StartHeliMission(
            Helicopter,
            Game.Player.Character,
            VehicleMissionType.Follow,
            80f,
            0,
            -1,
            20, -1, -1, HeliMissionFlags.HeightMapOnlyAvoidance
        );

        CurrentTask = Task.Following;
        //HelperClass.Subtitle("Target entered vehicle - switching to pursuit mode");
    }

    private void StartSearchTask()
    {
        PreviousTask = CurrentTask;
        missionInterrupted = true;
        isSearching = true;
        searchStartTime = DateTime.Now;

        CurrentTask = Task.Searching;
       // HelperClass.Subtitle("Target lost - initiating search pattern");

        SearchForPlayer();
    }

    private void SearchForPlayer()
    {
        // Placeholder for search implementation
        // You can implement a search pattern here (spiral, grid, etc.)
        Vector3 searchCenter = ImportantChecks.LastKnownLocation;
        Vector3 searchPosition = searchCenter.Around(50f);

        //Pilot.Task.ClearAll();
        Pilot.Task.StartHeliMission(
            Helicopter,
            searchPosition,
            VehicleMissionType.Circle,
            40f,
            30,
            -1,
            40
        );

    //    HelperClass.Subtitle($"Searching area around last known position...");
    }

    private void ResumeInterruptedMission()
    {
        if (!missionInterrupted) return;

        isSearching = false;
        missionInterrupted = false;
        Helicopter.IsSirenActive = false;

        // Resume previous task
        CurrentTask = PreviousTask;

        // Re-initialize the appropriate sequence
        if (PreviousTask == Task.GoToPosition || PreviousTask == Task.WaitingForRappel)
        {
            if (Rappel)
            {
                FindLocationForDeployment(ImportantChecks.LastKnownLocation.Around(10), out var dropZone, true);

                 DropZone = dropZone;

                currentRappelState = RappelState.GoToPosition;
                HelperClass.Subtitle("Resuming rappel mission at new location");
            }
            else if (Land)
            {
                FindLocationForDeployment(ImportantChecks.LastKnownLocation.Around(10), out var dropZone, false);

                DropZone = dropZone;
                currentLandingState = LandingState.GoToPosition;
               HelperClass.Subtitle("Resuming landing mission at new location");
            }
        }
    }


    // Add these fields to your Properties and Fields section
    private RappelState currentRappelState = RappelState.GoToPosition;
    private LandingState currentLandingState = LandingState.GoToPosition;

    private void UpdateHelicopterState()
    {
        // Handle search timeout
        if (isSearching && (DateTime.Now - searchStartTime).TotalSeconds > 30)
        {
            HelperClass.Subtitle("Search timeout - helicopter fleeing");
            StartFleeTask();
            return;
        }

        // Handle active tasks
        switch (CurrentTask)
        {
            case Task.Following:
                if (isPlayerOnFoot && playerInSight)
                {
                    ResumeInterruptedMission();
                }
                break;

            case Task.Searching:
                if (!playerInSight)
                {
                    SearchForPlayer();
                }
                break;

            default:

                if (Rappel && CanRappel)
                {
                    ProcessRappel();
                }
                else if (Land)
                {
                    ProcessLanding();
                }
                else
                {
                    ConvertToAttackHelicopter();
                }
                break;
        }
    }


    private DateTime lastRappelAttemptTime = DateTime.MinValue;


    // Main Rappel Processing Method
    private void ProcessRappel()
    {
        float distanceToTarget = GetHorizontalDistanceToDropZone();
        float currentSpeed = Helicopter.Speed;
        float heightAboveGround = GetHeightAboveGround();

        //HelperClass.Subtitle($"Rappel State: {currentRappelState} | Dist: {distanceToTarget:F1}m | Speed: {currentSpeed:F1}mph | Height: {heightAboveGround:F1}m");

        switch (currentRappelState)
        {
            case RappelState.GoToPosition:
                Pilot.Task.StartHeliMission(Helicopter, DropZone, VehicleMissionType.GoTo, 75f, 0, 20, (int)20, -1, 100, HeliMissionFlags.HeightMapOnlyAvoidance);
                currentRappelState = RappelState.PositionForRappel;
                break;

            case RappelState.PositionForRappel:
                
                if (distanceToTarget <= 40f)
                {
                    //Pilot.Task.StartHeliMission(Helicopter, DropZone, VehicleMissionType.GoTo, 20, 0, 15, (int)15f, -1, -1, HeliMissionFlags.None);
                    Pilot.BlockPermanentEvents = true;
                    currentRappelState = RappelState.CheckReadyToRappel;
                }
                break;

            
            case RappelState.CheckReadyToRappel:
                if (distanceToTarget <= 10f)
                {
                    float maxsafeRappelHeight = 20f;

                    float actualHeight = GetHeightAboveGround();

                    if (actualHeight < maxsafeRappelHeight)
                    {
                        HelperClass.Subtitle("Too low to rappel – ascending to safe height");

                        Vector3 ascendPosition = Helicopter.Position + Vector3.WorldUp * 20f;
                        Pilot.Task.StartHeliMission(
                            Helicopter,
                            ascendPosition,
                            VehicleMissionType.GoTo,
                            30f, 0f, 20, 20
                        );

                        break; // stay in CheckReadyToRappel until we ascend
                    }

                    //if (actualHeight > minsafeRappelHeight)
                    //{
                    //    HelperClass.Subtitle("Too high to rappel – ascending to safe height");

                    //    Vector3 ascendPosition = Helicopter.Position + Vector3.WorldDown * 10f;
                    //    Pilot.Task.StartHeliMission(
                    //        Helicopter,
                    //        ascendPosition,
                    //        VehicleMissionType.GoTo,
                    //        30f, 0f, -1, 20
                    //    );

                    //    break; // stay in CheckReadyToRappel until we ascend
                    //}


                    Pilot.Task.StartHeliMission(
                        Helicopter,
                        DropZone,
                        VehicleMissionType.Stop,
                        10f, 0, 20, 20
                    );
                    currentRappelState = RappelState.ExecuteRappel;

                }
                else
                {
                    currentRappelState = RappelState.GoToPosition;
                }
                break;


            case RappelState.ExecuteRappel:

                foreach (Ped crew in Helicopter.Passengers)
                {
                    if (crew.SeatIndex == VehicleSeat.Driver || crew.SeatIndex == VehicleSeat.RightFront) { continue; }

                    if (crew.IsInVehicle(Helicopter))
                    {
                        TaskSequence ts = new TaskSequence();
                        //Function.Call(Hash.TASK_SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, 0, 1);
                        ts.AddTask.RappelFromHelicopter();
                        //Function.Call(Hash.TASK_SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, 0, 0);
                        ts.AddTask.CombatHatedTargetsAroundPed(80, TaskCombatFlags.None);
                        ts.Close();
                        crew.Task.PerformSequence(ts);
                        //crew.Task.RappelFromHelicopter();
                    }

                }

                currentRappelState = RappelState.RappelInProgress;
                TransitionToTask(Task.RappelingInProgress);
                troopsDeploying = true;
                break;

            case RappelState.RappelInProgress:
                bool allDeployed = true;

                foreach (Ped crew in Helicopter.Passengers)
                {
                    if (!crew.Exists() || crew.IsDead) continue;
                    if (crew.SeatIndex == VehicleSeat.Driver || crew.SeatIndex == VehicleSeat.RightFront) continue;

                    if (crew.IsInVehicle(Helicopter))
                    {
                        if (crew.GetScriptTaskStatus(ScriptTaskNameHash.RappelFromHeli) != ScriptTaskStatus.Performing || crew.GetScriptTaskStatus(ScriptTaskNameHash.RappelFromHeli) != ScriptTaskStatus.WaitingToStart) 
                        {
                            currentRappelState = RappelState.ExecuteRappel;
                            TransitionToTask(Task.WaitingForRappel);
                        }
                        allDeployed = false;
                        break;
                    }
                }

                if (Helicopter.IsPedRappelingFromHelicopter())
                { // 4-second cooldown + only one-time creation flag
                    if ((DateTime.Now - lastRappelAttemptTime).TotalSeconds >= 8 && createExtra) //forcing only the annihilator types only
                    {
                        lastRappelAttemptTime = DateTime.Now;

                        // Fill remaining seats (excluding driver/copilot)
                        bool spawnedAnyone = false;

                        for (int seat = 1; seat < Helicopter.PassengerCapacity; seat++)
                        {
                            if (!Helicopter.IsSeatFree((VehicleSeat)seat)) continue;

                            var pedModel = Info.Soldiers[HelperClass.SharedRandom.Next(Info.Soldiers.Count)];
                            var weapons = new HelperClass.DualWeapons(Info.PrimaryWeapons, Info.SecondaryWeapons);
                            int pedIndex = HelperClass.SharedRandom.Next(pedModel.Peds.Count);

                            var p = Helicopter.CreatePed(pedModel.Peds[pedIndex], weapons, (VehicleSeat)seat, PedType.Cop, Vector3.Zero);
                            p.SetConfigFlag(PedConfigFlagToggles.AdditionalRappellingPed, true);
                            //p.SetConfigFlag(PedConfigFlagToggles.ragdoll)
                            spawnedAnyone = true;
                        }

                        if (spawnedAnyone)
                        {
                            createExtra = false;
                            currentRappelState = RappelState.ExecuteRappel;
                        }
                    }
                    allDeployed = false;
                }

                if (allDeployed)
                {
                    currentRappelState = RappelState.RappelComplete;
                    rappelCooldown = DateTime.Now;
                }
                break;



            case RappelState.RappelComplete:
                troopsDeploying = false;
                StartFleeTask();
                TransitionToTask(Task.Flee);
                break;
        }
    }

    private float GetHorizontalDistanceToDropZone()
    {
        return Helicopter.Position.DistanceTo2D(DropZone);
    }

    Vector3 PositionToReach;
    // Main Landing Processing Method
    private void ProcessLanding()
    {
        float distanceToTarget = Helicopter.Position.DistanceTo2D(DropZone);
        float currentSpeed = Helicopter.Speed;
        float heightAboveGround = GetHeightAboveGround();

        HelperClass.Subtitle($"Landing State: {currentLandingState} | Dist: {distanceToTarget:F1}m | Speed: {currentSpeed:F1}mph | Height: {heightAboveGround:F1}m");

        switch (currentLandingState)
        {
            case LandingState.GoToPosition:
                // Send helicopter to position with good speed
                Pilot.Task.StartHeliMission(Helicopter, DropZone, VehicleMissionType.GoTo, 70, 30, -1, 30, -1, 50, HeliMissionFlags.HeightMapOnlyAvoidance);
                currentLandingState = LandingState.CheckReadyToLand;
                break;

            case LandingState.CheckReadyToLand:
                // Check if we're close enough to start landing approach
                if (distanceToTarget < 60f) // Increased speed threshold
                {
                    Pilot.Task.StartHeliMission(Helicopter, DropZone, VehicleMissionType.GoTo, 20, -1, -1, 30, -1, -1, HeliMissionFlags.None);
                    Pilot.BlockPermanentEvents = true;
                    currentLandingState = LandingState.HoverAboveLanding; 
                }
                break;

            case LandingState.HoverAboveLanding:
                // Slow approach to landing zone
                if (distanceToTarget < 25f)
                {
                    Pilot.Task.StartHeliMission(Helicopter, DropZone, VehicleMissionType.GoTo, 10, -1, -1, 30);
                    currentLandingState = LandingState.ExecuteLanding;
                }
                else
                {
                    currentLandingState = LandingState.CheckReadyToLand;
                }
                break;

            case LandingState.ExecuteLanding:
                // Execute the actual landing
                if (Helicopter.GetActiveMissionType() != VehicleMissionType.Land && Helicopter.Model.IsHelicopter) Pilot.Task.StartHeliMission(Helicopter, DropZone, VehicleMissionType.Land, 15f, 0, 0, 0);

                if (Helicopter.GetActiveMissionType() != VehicleMissionType.Land && Helicopter.Model.IsPlane)  Pilot.Task.LandPlane(DropZone, DropZone);
                currentLandingState = LandingState.LandingInProgress;
                TransitionToTask(Task.LandingInProgress);
                break;

            case LandingState.LandingInProgress:
                CheckLandingStuck();
                if (IsHelicopterLanded())
                {
              //      HelperClass.Subtitle("Helicopter successfully landed");
                    currentLandingState = LandingState.LandingComplete;
                    TransitionToTask(Task.LandingComplete);
                }
                break;

            case LandingState.LandingComplete:
                currentLandingState = LandingState.CrewDeployment;
                landingCooldown = DateTime.Now;
                troopsDeploying = true;
                break;

            case LandingState.CrewDeployment:
                if (Pilot.IsDead)
                {
                    DeployCrew(CrewLeaveOption.CoPilotAndCrew);
              //      HelperClass.Subtitle("Pilot down – copilot and crew evacuating");
                    StartFleeTask();
                }
                else if (Helicopter.Health <= 500)
                {
                    DeployCrew(CrewLeaveOption.All);
                    Pilot.BlockPermanentEvents = false;
               //     HelperClass.Subtitle("Helicopter damaged – all crew evacuating");
                }
                else
                {
                    bool allCrewExited = CheckAllCrewExited(CrewLeaveOption.OnlyCrew);
                    if (allCrewExited)
                    {
                        StartFleeTask();
                //        HelperClass.Subtitle("Mission complete – helicopter fleeing");
                    }
                }
                break;
        }
    }
    int Interval = 5000;
    int Timer = 0;


    private void CheckLandingStuck()
    {
        float currentHeight = GetHeightAboveGround();

        // Wait for interval before rechecking
        if (Game.GameTime < Timer) return;
        Timer = Game.GameTime + Interval;

        var speed = Helicopter.Speed;
        // Helicopter is suspiciously low, but not landed 

        //use the time counter as well
        if (currentHeight >= 2 && currentHeight < 10f && speed < 0.3f)
        {
            if (!CanRappel)
            {
                //     HelperClass.Subtitle("Helicopter stuck below 5m – but rappel unsupported. Fleeing.");
                ConvertToAttackHelicopter();
                return;
            }

            //   HelperClass.Subtitle("Landing stuck – switching to rappel mode (with height correction)");

            // Update mode
            Land = false;
            Rappel = true;
            troopsDeploying = false;

            // Ascend first
            Vector3 safeAscendPosition = Helicopter.Position + Vector3.WorldUp * 30f;

            Pilot.Task.StartHeliMission(
                Helicopter,
                safeAscendPosition,
                VehicleMissionType.GoTo,
                50f, 5f, -1, 40
            );

            // Begin proper rappel flow
            currentRappelState = RappelState.CheckReadyToRappel;
            CurrentTask = Task.GoToPosition;
            TransitionToTask(Task.GoToPosition);
        }
    }

    private void StartRappelSequence()
    {
        TransitionToTask(Task.GoToPosition);
        Pilot.Task.StartHeliMission(Helicopter, DropZone, VehicleMissionType.GoTo, 100f, 0f, -1, 20);
    }

    private void StartLandingSequence()
    {
        TransitionToTask(Task.GoToPosition);
        Pilot.Task.StartHeliMission(Helicopter, DropZone, VehicleMissionType.GoTo, 100f, 0f, -1, 20);
    }

    private bool IsHelicopterLanded()
    {
        return GetHeightAboveGround() < 2f;
    }

    private float GetHeightAboveGround()
    {
        OutputArgument groundZ = new OutputArgument();
        Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD,
            Helicopter.Position.X, Helicopter.Position.Y, Helicopter.Position.Z, groundZ);

        return Helicopter.Position.Z - groundZ.GetResult<float>();
    }

    // CLASS VARIABLES
    private DateTime _cargobobTimer = DateTime.MinValue;
    private DateTime _cargoDoorOpenedAt = DateTime.MinValue;

    private bool _cargoTroopsSpawned = false;
    private bool _cargoDoorOpen = false;

    private const int CARGO_DOOR_INDEX = 2; // rear-left ramp
    private const int CARGO_TIMEOUT_SEC = 10;

    // ============================
    // Main Setup Call
    // ============================
    public void SetupCargobob()
    {
        if (!Helicopter.Model.IsCargobob || _cargoTroopsSpawned) return;

        var pedMdl = Info.Soldiers[HelperClass.SharedRandom.Next(Info.Soldiers.Count)];
        var weapons = new HelperClass.DualWeapons(Info.PrimaryWeapons, Info.SecondaryWeapons);
        int pedIdx = HelperClass.SharedRandom.Next(pedMdl.Peds.Count);

        // Open rear cargo ramp
        Function.Call(Hash.SET_VEHICLE_DOOR_OPEN, Helicopter, CARGO_DOOR_INDEX, false, false);
        _cargoDoorOpen = true;
        _cargoDoorOpenedAt = DateTime.UtcNow;

        // Create 8 troopers inside bay
        for (int i = 0; i < 8; i++)
        {
            Vector3 offset = new Vector3(0f, -2.5f + i * 0.6f, -0.6f);
            Vector3 spawnPos = Helicopter.GetOffsetPosition(offset);
            Ped trooper = Helicopter.CreatePed(pedMdl.Peds[pedIdx], weapons, (VehicleSeat)(-2), PedType.Cop, spawnPos);

            Vector3 exitPos = Helicopter.GetOffsetPosition(new Vector3(0f, -7f, -1.2f));
            if (World.GetGroundHeight(exitPos, out float z)) exitPos.Z = z;

            trooper.Task.RunTo(exitPos);
            trooper.Task.CombatHatedTargetsAroundPed(80f, TaskCombatFlags.None);

            Crew.Add(trooper);
        }

        _cargoTroopsSpawned = true;
    }

    // ============================
    // Deploy Crew by Mode
    // ============================
    public void DeployCrew(CrewLeaveOption leaveOption)
    {
        if (leaveOption == CrewLeaveOption.OnlyCrew)
            SetupCargobob();

        foreach (var p in Helicopter.Passengers)
        {
            if (p == null || !p.Exists() || p.IsDead || p.IsExitingVehicle) continue;

            switch (leaveOption)
            {
                case CrewLeaveOption.OnlyCrew:
                    if (p.SeatIndex == VehicleSeat.Driver || p.SeatIndex == VehicleSeat.RightFront) break;
                    goto case CrewLeaveOption.All;

                case CrewLeaveOption.CoPilotAndCrew:
                    if (p.SeatIndex == VehicleSeat.Driver) break;
                    goto case CrewLeaveOption.All;

                case CrewLeaveOption.All:
                    p.SetCombatAttribute(CombatAttributes.CanLeaveVehicle, true);
                    p.Task.LeaveVehicle();
                    p.Task.CombatHatedTargetsAroundPed(80, TaskCombatFlags.None);
                    break;
            }
        }
    }

    // ============================
    // Check if All Non-pilot Crew Exited
    // ============================
    private bool CheckAllCrewExited(CrewLeaveOption crew)
    {
        DeployCrew(crew);

        foreach (var p in Helicopter.Passengers)
        {
            if (p == null || !p.Exists() || p.IsDead) continue;
            if (p.SeatIndex == VehicleSeat.Driver || p.SeatIndex == VehicleSeat.RightFront)
                continue;

            if (!p.IsExitingVehicle || (p.IsInVehicle() && !Helicopter.Model.IsCargobob))
                return false;
        }

        if (Helicopter.Model.IsCargobob)
        {
            // Start cooldown when first detected
            if (_cargobobTimer == DateTime.MinValue)
                _cargobobTimer = DateTime.UtcNow;

            bool timeout = (DateTime.UtcNow - _cargobobTimer).TotalSeconds >= CARGO_TIMEOUT_SEC;
            if (!timeout)
                return false;

            // After cooldown — CLOSE ramp door
            if (_cargoDoorOpen)
            {
                Function.Call(Hash.SET_VEHICLE_DOOR_SHUT, Helicopter, CARGO_DOOR_INDEX, false);
                _cargoDoorOpen = false;
            }
        }
        else
        {
            _cargobobTimer = DateTime.MinValue; // reset
        }

        return true;
    }


    private void TransitionToTask(Task newTask)
    {
        if (CurrentTask != newTask)
        {
            CurrentTask = newTask;
            lastStateChange = DateTime.Now;
        }
    }

    private void StartFleeTask()
    {
        TransitionToTask(Task.Flee);
        troopsDeploying = false;

        if (!IsHelicopterValid()) return;

        Helicopter.IsSirenActive = false;
        Pilot.BlockPermanentEvents = true;

        Vector3 fleeTarget = Helicopter.Position;
        if (Helicopter.GetActiveMissionType() != VehicleMissionType.Flee && Helicopter.Model.IsHelicopter)  Pilot.Task.StartHeliMission(Helicopter, fleeTarget+Vector3.WorldEast*40+Vector3.WorldUp*60, VehicleMissionType.Flee, 120f, -1, 90, 100);
        if (Helicopter.GetActiveMissionType() != VehicleMissionType.Flee && Helicopter.Model.IsPlane) Pilot.Task.StartPlaneMission(Helicopter, fleeTarget, VehicleMissionType.Flee, 80, -1, -1, 80);
       // HelperClass.Subtitle("Helicopter fleeing the area");
    }

    private void ConvertToAttackHelicopter()
    {
        if (DispatchManager._activeAttackHelis != null && DispatchManager._activeDropOffHelis != null)
        {DispatchManager._activeDropOffHelis.Remove(this);
            
            DispatchManager._activeAttackHelis.Add(new AttackHelicopter(Helicopter, Info));
        //    HelperClass.Subtitle("Converting to attack helicopter");
        }
    }

    public bool HasAnyPassengers
    {
        get
        {
            if (Crew == null || Crew.Count == 0) return false;

            foreach (Ped soldier in Crew)
            {
                if (soldier == null || !soldier.Exists() || soldier.IsDead)
                    continue;

                if (soldier.IsInVehicle(Helicopter))
                    return true;
            }

            return false;
        }
    }


    public bool IsHelicopterValid()
    {
        return Helicopter != null &&
               Helicopter.Exists() &&
               !Helicopter.IsDead &&
               Pilot != null &&
               Pilot.Exists() &&
               !Pilot.IsDead;
    }

    public bool ApplyWeaponAmmo(VehicleWeaponHash weaponHash, int numammo)
    {
        if (!IsHelicopterValid()) return false;

        Helicopter.SetWeaponRestrictedAmmo((int)weaponHash, numammo);
        return true;
    }

    public static bool FindLocationForDeployment(Vector3 targetPosition, out Vector3 deploymentPosition, bool pedPath, bool doHeightFix = true)
    {
        deploymentPosition = Vector3.Zero;

        // Try up to 10 times to find a valid position
        for (int attempt = 0; attempt < 10; attempt++)
        {
            // First: Try finding position on street
            deploymentPosition = World.GetNextPositionOnStreet(targetPosition, unoccupied: true);
            if (deploymentPosition != Vector3.Zero)
            {
                if (doHeightFix)
                    GroundZFix(ref deploymentPosition);
                return true;
            }

            // Second: Try safe ped path
            if (pedPath && World.GetSafePositionForPed(targetPosition.Around(20f), out deploymentPosition,
                GetSafePositionFlags.NotIsolated | GetSafePositionFlags.NotInterior | GetSafePositionFlags.NotWater))
            {
                if (doHeightFix)
                    GroundZFix(ref deploymentPosition);
                return true;
            }

            // Third: Try custom random position with ground check
            Vector3 randomVec = targetPosition.Around(GetRandomFloat(40.0f, 50.0f));
            OutputArgument zArg = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, randomVec.X, randomVec.Y, randomVec.Z + 100f, zArg))
            {
                randomVec.Z = zArg.GetResult<float>();
                deploymentPosition = randomVec;
                return true;
            }
        }

        // Final fallback after 10 failed attempts
        deploymentPosition = targetPosition.Around(45f);
        if (doHeightFix)
            GroundZFix(ref deploymentPosition);

        return false; // Clearly mark that nothing ideal was found
    }


    private static void GroundZFix(ref Vector3 position)
    {
        OutputArgument groundZ = new OutputArgument();
        bool foundGround = Function.Call<bool>(
            Hash.GET_GROUND_Z_FOR_3D_COORD,
            position.X, position.Y, position.Z + 100f,
            groundZ
        );

        position.Z = foundGround ? groundZ.GetResult<float>() : position.Z - 1.5f;
    }


    public static float GetRandomFloat(double min, double max)
    {
        return (float)(new Random().NextDouble() * (max - min) + min);
    }

    public static double GetDouble()
    {
        return new Random().NextDouble();
    }

    public static bool GetBool()
    {
        return GetDouble() >= 0.5;
    }
}