//using GTA;
//using GTA.Math;
//using System;
//using GTA.Native;
//using System.Collections.Generic;
//using System.Linq;
//using GTA.UI;

//public class DropOffHelicopter
//{
//    public Vehicle Heli { get; set; }
//    public bool Rappel { get; set; }
//    public bool Land { get; set; }
//    public bool CanRappel { get; private set; }
//    private Ped Pilot;
//    private Dictionary<VehicleWeaponHash, bool> _weaponStates;

//    private bool _isRappelComplete;
//    private Vector3 _positionToReach;
//    private bool _hasLanded;

//    private enum HeliStates
//    {
//        None,
//        GoToPosition,
//        RappelSoldier,
//        WaitingRappel,
//        Leave,
//        StartLanding = RappelSoldier,
//        Landed = WaitingRappel
//    }

//    private const int LowHealthThreshold = 50;
//    private const int CriticalHealthThreshold = 30;

//    private const float PursuitSpeed = 100f;
//    private const float RappelSlowdownSpeed = 15f;
//    private const float RappelStableSpeed = 5f;
//    private const int FleeSpeed = 120;

//    private const float RappelRadius = 15f;
//    private const float RappelSlowdownRadius = 50f;

//    private const int PursuitHeight = 80;
//    private const float RappelHeight = 20;
//    private const float FleeHeight = 50f;
//    private const int DistReached = 0;
//    private Vector3 _deployZone = Vector3.Zero;
//    private bool _forceHeightMapAvoidance;
//    //private Task _taskStates;
//    private Random _random = new Random();
//    private bool _hasAnyPassengersCache; // Cache for passenger check
//    private DateTime _lastPassengerCheckTime;
//    private const double PassengerCheckCacheIntervalSeconds = 0.5; // Check passengers every half second

//    // New fields for player condition check
//    private bool _isPlayerConditionMet = false;
//    private DateTime _lastPlayerCheckForDeploymentTime;
//    private const double PlayerCheckForDeploymentIntervalSeconds = 1.0; // Check player condition every 1 second
//    private const float MaxPlayerVelocityForDeployment = 2.0f; // Max velocity to consider player "not moving crazily"


//    public DropOffHelicopter(GTA.Vehicle existingHeli, Vector3 dropzone)
//    {
//        if (existingHeli == null || !existingHeli.Exists())
//        {
//            throw new ArgumentException("Existing helicopter is null or invalid.", nameof(existingHeli));
//        }

//        Heli = existingHeli;
//        _deployZone = dropzone;

//        Pilot = Heli.Driver; // Get Pilot here
//        StartTask(Pilot);
//    }
//    private bool HasAnyPassengers
//    {
//        get
//        {
//            if ((DateTime.Now - _lastPassengerCheckTime).TotalSeconds < PassengerCheckCacheIntervalSeconds)
//            {
//                return _hasAnyPassengersCache;
//            }
//            _hasAnyPassengersCache = false;
//            for (int i = 0; i < Heli.PassengerCapacity; i++)
//            {
//                VehicleSeat seat = (VehicleSeat)i;
//                if (seat == VehicleSeat.Driver || seat == VehicleSeat.RightFront) continue;
//                if (!Heli.IsSeatFree(seat))
//                {
//                    _hasAnyPassengersCache = true;
//                    break;
//                }
//            }
//            _lastPassengerCheckTime = DateTime.Now;
//            return _hasAnyPassengersCache;
//        }
//    }


//    private void StartTask(Ped pilot)
//    {
//        if ((Game.Player.WantedLevel == 0) || _deployZone == Vector3.Zero || !HasAnyPassengers)
//        {
//            FleeTask(pilot);
//            //_taskStates = Task.Flee;
//        }
//        else
//        {
//            pilot.BlockPermanentEvents = false;
//            pilot.Task.ClearAll();
//            pilot.Task.StartHeliMission(Heli, _deployZone, VehicleMissionType.GoTo, 20f, 0f, -1, 30, -1f, -1f, _forceHeightMapAvoidance ? HeliMissionFlags.HeightMapOnlyAvoidance : HeliMissionFlags.DontDoAvoidance);
//            //_taskStates = Task.GoToStagingArea;
//        }
//    }

//    public bool ApplyWeaponAmmo(VehicleWeaponHash weaponHash, int numammo)
//    {
//        Heli.SetWeaponRestrictedAmmo((int)weaponHash, numammo);
//        return true;
//    }

//    public void FleeTask(Ped pilot)
//    {
//        Heli.IsSirenActive = false;
//        pilot.BlockPermanentEvents = true;
//        if (Heli.GetActiveMissionType() != VehicleMissionType.Flee)
//        {
//            Heli.ClearPrimaryTask();
//            pilot.Task.ClearAll();
//            pilot.Task.StartHeliMission(Heli, Heli, VehicleMissionType.Flee, 80, 0f, -1, 100);
//        }
//    }

//    public void UpdateProcess()
//    {
//        if (!IsHelicopterValid())
//            return;

//        CheckPlayerDeploymentCondition(); // Check player condition first

//        if (_isPlayerConditionMet) // Proceed with deployment only if player condition is met
//        {
//            RappelPeds();
//            LandHelicopter();
//        }
//    }

//    private void CheckPlayerDeploymentCondition()
//    {
//        if ((DateTime.Now - _lastPlayerCheckForDeploymentTime).TotalSeconds < PlayerCheckForDeploymentIntervalSeconds)
//        {
//            return; // Check condition only at intervals
//        }
//        _lastPlayerCheckForDeploymentTime = DateTime.Now;

//        if (Game.Player.Character.IsInVehicle() || Game.Player.Character.Velocity.Length() > MaxPlayerVelocityForDeployment)
//        {
//            _isPlayerConditionMet = false; // Player in vehicle or moving fast, condition not met
//        }
//        else
//        {
//            _isPlayerConditionMet = true; // Player on foot and not moving crazily, condition met
//        }
//    }

//    private Vector3 _potentialLandingPosition;
//    private bool _isLandingPositionFound = false;
//    private bool _isHoverPositionReached = false;

//    public void LeaveLandedHelicopter(CrewLeaveOption leaveOption)
//    {
//        foreach (var crewMember in Heli.Passengers)
//        {
//            if (!crewMember.IsExitingVehicle)
//            {
//                switch (leaveOption)
//                {
//                    case CrewLeaveOption.OnlyCrew:
//                        if (crewMember.SeatIndex != VehicleSeat.Driver && crewMember.SeatIndex != VehicleSeat.RightFront)
//                        {
//                            crewMember.Task.LeaveVehicle();
//                        }
//                        break;

//                    case CrewLeaveOption.CoPilotAndCrew:
//                        if (crewMember.SeatIndex == VehicleSeat.Driver)
//                        {
//                            crewMember.SetCombatAttribute(CombatAttributes.CanLeaveVehicle, false);
//                        }
//                        else
//                        {
//                            crewMember.Task.LeaveVehicle();
//                        }
//                        break;

//                    case CrewLeaveOption.All:
//                        crewMember.Task.LeaveVehicle();
//                        crewMember.BlockPermanentEvents = false;
//                        break;

//                    default:
//                        break;
//                }
//            }
//        }
//    }

//    private bool _isDeparting = false;
//    private bool _hasCooldownStarted = false;
//    private DateTime _cooldownStartTime;


//    public void LandHelicopter(CrewLeaveOption option = CrewLeaveOption.OnlyCrew)
//    {
//        if (!IsHelicopterValid() || Pilot == null || !Land || !_isPlayerConditionMet) // Added player condition check here
//            return;

//        Pilot.BlockPermanentEvents = true;
//        if (!_hasLanded)
//        {
//            if (!_isLandingPositionFound)
//            {
//                _potentialLandingPosition = World.GetNextPositionOnStreet(_deployZone);

//                if (_potentialLandingPosition.DistanceTo(Game.Player.Character.Position) > 60)
//                {
//                    Rappel = true;
//                    Land = false;
//                    if (!CanRappel) AIManager.AttackHelicopters.Add(new AttackHelicopter(Heli, _potentialLandingPosition));
//                    return;
//                }

//                _isLandingPositionFound = true;
//                _positionToReach = _potentialLandingPosition + Vector3.WorldUp * 40;
//                Pilot.Task.StartHeliMission(Heli, _positionToReach, VehicleMissionType.GoTo, 70, 10, 100, 0);
//                return;
//            }

//            if (!_isHoverPositionReached)
//            {
//                if (Heli.Position.DistanceTo(_positionToReach) >= 20)
//                {
//                    return;
//                }
//                _isHoverPositionReached = true;
//            }

//            if (Heli.GetActiveMissionType() != VehicleMissionType.Land)
//            {
//                Pilot.Task.StartHeliMission(
//                    Heli,
//                    _potentialLandingPosition,
//                    VehicleMissionType.Land,
//                    50, 0, -1, 0
//                );
//                return;
//            }

//            float groundHeight;
//            OutputArgument unk = new OutputArgument();
//            Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, Heli.Position.X, Heli.Position.Y, Heli.Position.Z, unk);
//            float heliHeight = Heli.Position.Z;
//            groundHeight = unk.GetResult<float>();

//            if (heliHeight - groundHeight < 3f)
//            {
//                LeaveLandedHelicopter(option);

//                if (!_hasCooldownStarted)
//                {
//                    _hasCooldownStarted = true;
//                    _cooldownStartTime = DateTime.Now;
//                    return;
//                }

//                if ((DateTime.Now - _cooldownStartTime).TotalSeconds < 2)
//                    return;

//                _hasCooldownStarted = false;
//                _hasLanded = true;

//                _isDeparting = true;
//                if (option == CrewLeaveOption.OnlyCrew || option == CrewLeaveOption.CoPilotAndCrew) TaskFlee();
//                if (Pilot.IsDead) LeaveLandedHelicopter(CrewLeaveOption.All);
//                return;
//            }
//        }
//        else if (_isDeparting)
//        {
//            return;
//        }
//    }

//    private bool _isRappelPositionFound = false;
//    private bool _isRappelPositionReached = false;


//    public void RappelPeds()
//    {
//        if (!IsHelicopterValid() || !Rappel || !_isPlayerConditionMet) // Added player condition check here
//        {
//            Pilot.BlockPermanentEvents = true;
//            if (!CanRappel)
//            {
//                Rappel = false;
//                AIManager.AttackHelicopters.Add(new AttackHelicopter(Heli, _deployZone));
//                AIManager.DropOffHelicopters.Remove(this);
//                // Handle case where helicopter cannot rappel, possibly switching to a generic helicopter type in a management class.
//                return;
//            }
//            return;
//        }

//        if (_isRappelComplete)
//        {
//            return;
//        }

//        if (!_isRappelPositionFound)
//        {
//            _positionToReach = _deployZone;
//            _isRappelPositionFound = true;
//            Pilot.Task.StartHeliMission(
//                Heli,
//                _positionToReach,
//                VehicleMissionType.GoTo,
//                FleeSpeed,
//                30,
//                -1,
//                (int)RappelHeight
//            );
//            return;
//        }

//        if (!_isRappelPositionReached)
//        {
//            float distanceToTarget = Heli.Position.DistanceTo(_positionToReach);
//            if (distanceToTarget >= 5)
//            {
//                Pilot.Task.StartHeliMission(
//                    Heli,
//                    _positionToReach,
//                    VehicleMissionType.GoTo,
//                    0,
//                    0,
//                    -1,
//                    (int)RappelHeight
//                );
//                return;
//            }
//            _isRappelPositionReached = true;
//        }


//        if (Heli.GetActiveMissionType() != VehicleMissionType.GoTo)
//        {
//            if (Heli.Position.DistanceTo(_positionToReach) < 5)
//            {
//                Pilot.Task.StartHeliMission(
//                    Heli,
//                    _positionToReach,
//                    VehicleMissionType.GoTo,
//                    RappelStableSpeed,
//                    0,
//                    (int)RappelHeight,
//                    (int)RappelHeight
//                );
//                return;
//            }
//        }


//        if (Heli.Position.DistanceTo(_positionToReach) < 5)
//        {
//            bool allCrewOutside = true;
//            foreach (var crewMember in Heli.Passengers)
//            {
//                VehicleSeat seatIndex = crewMember.SeatIndex;
//                if (seatIndex == VehicleSeat.Driver || seatIndex == VehicleSeat.RightFront)
//                {
//                    continue;
//                }

//                if (crewMember.IsInVehicle(Heli))
//                {
//                    TaskSequence sequence = new TaskSequence();
//                    Function.Call(Hash.TASK_SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, 0, 1);
//                    sequence.AddTask.RappelFromHelicopter();
//                    Function.Call(Hash.TASK_SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, 0, 0);
//                    sequence.AddTask.CombatHatedTargetsAroundPed(80, TaskCombatFlags.None);
//                    sequence.Close();
//                    crewMember.Task.PerformSequence(sequence);
//                    allCrewOutside = false;
//                }
//                else if (Heli.IsPedRappelingFromHelicopter())
//                {
//                    allCrewOutside = false;
//                }
//            }

//            if (!Heli.IsPedRappelingFromHelicopter() && allCrewOutside)
//            {
//                _isRappelComplete = true;
//                Script.Wait(500);
//                TaskFlee();
//                return;
//            }

//            if (!allCrewOutside)
//            {
//                return;
//            }
//        }
//    }

//    public static bool IsDeploymentPointValid(Vector3 position, Vector3 targetPosition)
//    {
//        if (position == Vector3.Zero)
//        {
//            return false;
//        }
//        float heightDifference = position.Z - targetPosition.Z;
//        if (heightDifference < -10f || heightDifference > 20f)
//        {
//            return false;
//        }
//        if (position.DistanceTo(AIManager.PlayerPed.Position) > 100f)
//        {
//            return false;
//        }
//        Vector3 startPosition = new Vector3(position.X, position.Y, position.Z + 40f);
//        Vector3 endPosition = new Vector3(position.X, position.Y, position.Z + 8.5f);
//        ShapeTestHandle shapeTestHandle = ShapeTest.StartTestCapsule(startPosition, endPosition, 8f, IntersectFlags.LosToEntity | IntersectFlags.Vehicles | IntersectFlags.Foliage);
//        if (shapeTestHandle.IsRequestFailed)
//        {
//            return true;
//        }
//        ShapeTestResult result;
//        while (shapeTestHandle.GetResult(out result) == ShapeTestStatus.NotReady)
//        {
//            Script.Yield();
//        }
//        return !result.DidHit;
//    }

//    public static bool FindLocationForDeployment(Vector3 targetPosition, out Vector3 deploymentPosition)
//    {
//        int attempts = 20;
//        bool foundPosition = false;
//        deploymentPosition = Vector3.Zero;
//        while (attempts-- > -20)
//        {
//            float distance = ((attempts < 0) ? GetRandomFloat(15.0, 40.0) : GetRandomFloat(7.5, 15.0));
//            if (foundPosition = World.GetSafePositionForPed(targetPosition.Around(GetRandomFloat(0.0, 2.5)).Around(distance), out deploymentPosition, GetSafePositionFlags.NotIsolated | GetSafePositionFlags.NotInterior | GetSafePositionFlags.NotWater))
//            {
//                if (IsDeploymentPointValid(deploymentPosition, targetPosition))
//                {
//                    break;
//                }
//                foundPosition = false;
//            }
//        }
//        if (!foundPosition)
//        {
//            float distance2 = ((attempts < 0) ? GetRandomFloat(15.0, 40.0) : GetRandomFloat(7.5, 15.0));
//            Vector3 position = targetPosition.Around(GetRandomFloat(0.0, 2.5)).Around(distance2);
//            deploymentPosition = World.GetNextPositionOnStreet(position);
//            return IsDeploymentPointValid(deploymentPosition, targetPosition);
//        }
//        return true;
//    }

//    public static float GetRandomFloat(double min, double max)
//    {
//        return (float)(new Random().NextDouble() * (max - min) + min);
//    }

//    public static double GetDouble()
//    {
//        return new Random().NextDouble();
//    }

//    public static bool GetBool()
//    {
//        return GetDouble() >= 0.5;
//    }

//    public enum CrewLeaveOption
//    {
//        OnlyCrew,         // Only non-pilot/co-pilot crew leaves
//        CoPilotAndCrew,         // Co-pilot and other crew leave (pilot stays)
//        All             // All crew members leave the vehicle
//    }


//    public void TaskFlee(bool force = true)
//    {
//        Heli.IsSirenActive = false;
//        Pilot.BlockPermanentEvents = force;
//        if (Heli.GetActiveMissionType() != VehicleMissionType.Flee)
//        {
//            Heli.ClearPrimaryTask();
//            Pilot.Task.ClearAll();
//            Pilot.Task.StartHeliMission(Heli, Game.Player.Character, VehicleMissionType.Flee, 40f, 0f, -1, 100);
//        }
//    }

//    public bool IsHelicopterValid()
//    {
//        return Heli != null &&
//                   Heli.Exists() &&
//                   !Heli.IsDead &&
//                   Pilot != null && //Check for pilot validity instead of driver from vehicle
//                   Pilot.Exists() &&
//                   !Pilot.IsDead;
//    }
//}