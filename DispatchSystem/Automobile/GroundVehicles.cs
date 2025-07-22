using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;

public class GroundVehicle
{
    public enum VehicleTask
    {
        Idle,
        Goto,
        Going,
        Follow,
        Following,
        Search,
        Searching,
        Flee,
        Fleeing,
    }

    public Ped Driver { get; set; }
    public Vehicle Vehicle { get; set; }
    private Dictionary<VehicleWeaponHash, bool> VehicleWeapon { get; set; }
    private VehicleTask Task { get; set; }
    private VehicleTask PreviousTask { get; set; }
    private int TaskStartTime { get; set; }
    private bool TaskAssigned { get; set; }

    public List<Ped> Crew = new List<Ped>();

    VehicleDrivingFlags fast = (VehicleDrivingFlags)835636u;

    public GroundVehicle(Vehicle vehicle, DispatchVehicleInfo info)
    {
        Vehicle = vehicle;
        Driver = vehicle.Driver;
        Crew = vehicle.Passengers.ToList();
        VehicleWeapon = new Dictionary<VehicleWeaponHash, bool>();
        TaskStartTime = Game.GameTime;
        TaskAssigned = false;

        Driver.SetCombatAttribute(CombatAttributes.CanInvestigate, true);
        Driver.SetCombatAttribute(CombatAttributes.UseVehicleAttack, info.Tasks.Contains("harass"));

        Initialize();
    }

    private void TaskSearch()
    {
        if (TaskAssigned) return;

        Vector3 searchPos = HelperClass.FindSearchPointForAutomobile(ImportantChecks.LastKnownLocation, 50);

        TaskSequence ts = new TaskSequence();
        ts.AddTask.StartVehicleMission(Vehicle, searchPos, VehicleMissionType.Cruise, 50f, VehicleDrivingFlags.DrivingModePloughThrough, -1, -1);
        //ts.AddTask.StartVehicleMission(Vehicle, searchPos.Around(40), VehicleMissionType.Cruise, 30f, VehicleDrivingFlags.SwerveAroundAllVehicles, -1, -1);
        ts.AddTask.LeaveVehicle();
        ts.Close();

        Driver.Task.PerformSequence(ts);
        SetTask(VehicleTask.Searching);
        TaskAssigned = true;
    }

    private void SetTask(VehicleTask newTask)
    {
        if (Task != newTask)
        {
            PreviousTask = Task;
            Task = newTask;
            TaskStartTime = Game.GameTime;
            TaskAssigned = false;
        }
    }

    private void UpdateCrewCombatAttributes()
    {
        bool vehicleOnFire = Vehicle.Health < 200;

        foreach (var crewMember in Crew)
        {
            if (crewMember.Exists() && crewMember.IsAlive)
            {
                crewMember.SetCombatAttribute(CombatAttributes.CanLeaveVehicle, vehicleOnFire);
            }
            if (Game.Player.Character.IsEnteringVehicle && Game.Player.Character.VehicleTryingToEnter == Vehicle)
                crewMember.Task.LeaveVehicle(LeaveVehicleFlags.None);
        }
    }

    public void Initialize()
    {
        InitializeWeaponCrew();

        if (!IsVehicleValid())
        {
            SetTask(VehicleTask.Idle);
            return;
        }

        if (Game.Player.Wanted.WantedLevel == 0)
        {
            SetTask(VehicleTask.Flee);
            return;
        }

        if (Game.Player.Wanted.WantedLevel > 0)
        {
            Vehicle.IsSirenActive = true;
            SetTask(VehicleTask.Goto);
        }
    }

    private void InitializeWeaponCrew()
    {
        
        for (int i = 0; i < Crew.Count; i++)
        {
            var crewMember = Crew[i];
            if (crewMember.Exists() && crewMember.IsAlive)
            {
                VehicleSeat seat = (VehicleSeat)i;
                bool isWeaponSeat = Vehicle.IsTurretSeat(seat);

                if (isWeaponSeat && seat != VehicleSeat.Driver)
                {
                    crewMember.SetCombatAttribute(CombatAttributes.CanLeaveVehicle, false);

                }
            }
        }
    }

    private void CheckGlobalStateChanges()
    {
        if (Game.Player.Wanted.WantedLevel == 0 &&
            Task != VehicleTask.Flee && Task != VehicleTask.Fleeing && Task != VehicleTask.Idle)
        {
            SetTask(VehicleTask.Flee);
        }
        else if (Game.Player.Wanted.WantedLevel > 0)
        {
           // Vehicle.IsSirenActive = true;

            if (Game.Player.Wanted.HasGrayedOutStars &&
                (Task == VehicleTask.Following || Task == VehicleTask.Going))
            {
                SetTask(VehicleTask.Search);
            }

            else if (!Game.Player.Wanted.HasGrayedOutStars)
            {
                if (Game.Player.Character.IsInVehicle())
                {
                    SetTask(VehicleTask.Follow);
                }
                else
                {
                    SetTask(VehicleTask.Goto);
                }
            }
        }
    }

    public void Update()
    {
        if (!IsVehicleValid())
        {
            SetTask(VehicleTask.Idle);
            return;
        }

        UpdateCrewCombatAttributes();
        CheckGlobalStateChanges();
       
        HelperClass.Subtitle($"Ground State: {Task} | Dist: {Vehicle.Position.DistanceTo(ImportantChecks.LastKnownLocation):F1}m | Speed: {Vehicle.Speed:F1}mph");

        // Combined State Machine
        switch (Task)
        {
            case VehicleTask.Goto:
                if (!TaskAssigned && Driver.IsInVehicle())
                {
                    Driver.Task.ClearAll();
                    Driver.Task.StartVehicleMission(Vehicle, ImportantChecks.LastKnownLocation,
                        VehicleMissionType.GoTo, 90f, fast, -1, -1);
                    SetTask(VehicleTask.Going);
                    TaskAssigned = true;
                }
                break;

            case VehicleTask.Going:
                {
                    float distanceToTarget = Vector3.Distance(Vehicle.Position, ImportantChecks.LastKnownLocation);

                    if (distanceToTarget < 40f)
                    {
                        foreach (var c in Crew)
                        {
                            c.PopulationType = EntityPopulationType.RandomAmbient;
                        }
                        Vehicle.PopulationType = EntityPopulationType.RandomPatrol;
                    }
                    else if (Driver.TaskSequenceProgress == -1 ||
                            (Game.GameTime - TaskStartTime > 30000 && Vehicle.Speed < 5f))
                    {
                        SetTask(VehicleTask.Goto);
                    }
                }
                break;

            case VehicleTask.Follow:
                if (!TaskAssigned && Game.Player.Character.IsInVehicle() && Driver.IsInVehicle())
                {
                    Driver.Task.ClearAll();

                    VehicleMissionType[] followMissions = new VehicleMissionType[]
                    {
                        VehicleMissionType.Follow,
                        VehicleMissionType.Ram,
                        VehicleMissionType.Block,
                        VehicleMissionType.PoliceBehaviour
                    };

                    VehicleMissionType selectedMission = followMissions[HelperClass.SharedRandom.Next(followMissions.Length)];

                    Driver.Task.StartVehicleMission(Vehicle, Game.Player.Character,
                        selectedMission, 90f, fast, 0, -1);
                    Driver.Task.ChaseWithGroundVehicle(Game.Player.Character);
                    SetTask(VehicleTask.Following);
                    TaskAssigned = true;
                }
                else if (!Game.Player.Character.IsInVehicle())
                {
                    SetTask(VehicleTask.Search);
                }
                break;

            case VehicleTask.Following:
                if (!Game.Player.Character.IsInVehicle())
                {
                    SetTask(VehicleTask.Search);
                }
                else if (Driver.TaskSequenceProgress == -1 ||
                        (Game.GameTime - TaskStartTime > 15000 && Vehicle.Speed < 5f))
                {
                    SetTask(VehicleTask.Follow);
                }
                break;

            case VehicleTask.Search:
                if (!TaskAssigned)
                {
                    if (Driver.IsInVehicle())
                    {
                        TaskSearch();
                    }
                    else
                    {
                        Driver.Task.ClearAll();
                        Driver.Task.CombatHatedTargetsAroundPed(100f, TaskCombatFlags.ArrestTarget);
                        SetTask(VehicleTask.Searching);
                        TaskAssigned = true;
                    }
                }
                break;

            case VehicleTask.Searching:
                if (Driver.TaskSequenceProgress == -1 || Game.GameTime - TaskStartTime > 20000)
                {
                    if (Game.Player.Character.IsInVehicle())
                    {
                        if (Driver.IsInVehicle() || Vector3.Distance(Driver.Position, Vehicle.Position) < 10f)
                        {
                            SetTask(VehicleTask.Follow);
                        }
                        else
                        {
                            SetTask(VehicleTask.Goto);
                        }
                    }
                    else
                    {
                        SetTask(VehicleTask.Search);
                    }
                }
                break;

            case VehicleTask.Flee:
                if (!TaskAssigned)
                {
                    Driver.Task.ClearAll();

                    if (Driver.IsInVehicle())
                    {
                        Vector3 fleeDirection = (Vehicle.Position - Game.Player.Character.Position).Normalized;
                        Vector3 fleePosition = Vehicle.Position + (fleeDirection * 500f);

                        Driver.Task.StartVehicleMission(Vehicle, fleePosition,
                            VehicleMissionType.Flee, 80f, VehicleDrivingFlags.DrivingModeAvoidVehicles, -1, -1);
                    }
                    else
                    {
                        //Driver.Task.FleeFrom(Game.Player.Character);
                        Driver.MarkAsNoLongerNeeded();
                    }

                    Driver.BlockPermanentEvents = false;
                    SetTask(VehicleTask.Fleeing);
                    TaskAssigned = true;
                }
                break;

            case VehicleTask.Fleeing:
                if (Game.GameTime - TaskStartTime > 15000 ||
                    Vector3.Distance(Vehicle.Position, Game.Player.Character.Position) > 300f)
                {
                    SetTask(VehicleTask.Idle);
                }
                break;

            case VehicleTask.Idle:
            default:
                // Vehicle is idle, minimal processing
                break;
        }
    }

    public bool IsVehicleValid()
    {
        return Vehicle != null && Vehicle.Exists() && !Vehicle.IsDead &&
               Driver != null && Driver.Exists() && !Driver.IsDead;
    }

    public bool ApplyWeaponAmmo(VehicleWeaponHash weaponHash, int numammo)
    {
        if (Vehicle != null && Vehicle.Exists())
        {
            Vehicle.SetWeaponRestrictedAmmo((int)weaponHash, numammo);
            return true;
        }
        return false;
    }
}
