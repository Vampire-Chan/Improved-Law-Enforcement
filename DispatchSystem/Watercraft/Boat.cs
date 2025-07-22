using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;

public class WaterVehicle : IDisposable
{
    #region Configuration Constants
    private const float ARRIVAL_DISTANCE = 45f;
    private const float FLEE_DISTANCE = 200f;
    private const int FLEE_TIMEOUT = 10000; // 10 seconds
    private const float SEARCH_RADIUS = 50f;
    private const float DEFAULT_SPEED = 50f;
    private const int ENGINE_FIRE_THRESHOLD = 200;
    private const int UPDATE_INTERVAL = 100; // milliseconds
    private const int TASK_COMPLETION_CHECK = -1;
    #endregion

    #region Enums
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
    #endregion

    #region Properties
    public Ped Driver { get; set; }
    public Vehicle Vehicle { get; set; }
    public List<Ped> Crew { get; private set; } = new List<Ped>();
    #endregion

    #region Private Fields
    private readonly DispatchVehicleInfo _info;
    private readonly Dictionary<VehicleWeaponHash, bool> _vehicleWeapon = new Dictionary<VehicleWeaponHash, bool>();
    private readonly List<Ped> _weaponCrewMembers = new List<Ped>();
    private readonly BoatMissionFlags _missionFlags = BoatMissionFlags.DefaultSettings |
                                                      BoatMissionFlags.PreferForward |
                                                      BoatMissionFlags.NeverNavMesh |
                                                      BoatMissionFlags.NeverPause;

    private VehicleTask _currentTask = VehicleTask.Idle;
    private VehicleTask _previousTask = VehicleTask.Idle;
    private int _taskStartTime;
    private int _lastUpdateTime;
    private bool _disposed;

    // Cached values for performance
    private Vector3 _lastPlayerPosition;
    private int _lastPositionUpdate;
    private const int POSITION_CACHE_DURATION = 200;
    #endregion

    #region Constructor
    public WaterVehicle(Vehicle vehicle, DispatchVehicleInfo info)
    {
        Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
        _info = info ?? throw new ArgumentNullException(nameof(info));

        if (!IsVehicleValid())
        {
            throw new ArgumentException("Invalid vehicle provided", nameof(vehicle));
        }

        Driver = vehicle.Driver;
        RefreshCrew();

        _taskStartTime = Game.GameTime;
        ConfigureDriver();
        Initialize();
    }
    #endregion

    #region Initialization
    private void ConfigureDriver()
    {
        if (Driver?.Exists() == true)
        {
            Driver.SetCombatAttribute(CombatAttributes.CanInvestigate, true);
        }
    }

    private void RefreshCrew()
    {
        if (!IsVehicleValid()) return;

        Crew.Clear();
        var passengers = Vehicle.Passengers?.ToList();
        if (passengers != null)
        {
            Crew.AddRange(passengers.Where(p => p?.Exists() == true && p.IsAlive));
        }
    }

    private void Initialize()
    {
        InitializeWeaponCrew();

        if (!IsVehicleValid())
        {
            SetTask(VehicleTask.Idle);
            return;
        }

        DetermineInitialTask();
    }

    private void DetermineInitialTask()
    {
        int wantedLevel = Game.Player.Wanted.WantedLevel;

        if (wantedLevel == 0)
        {
            SetTask(VehicleTask.Flee);
            return;
        }

        Vehicle.IsSirenActive = true;

        if (!Game.Player.Wanted.PoliceBackOff)
        {
            SetTask(VehicleTask.Goto);
        }
        else if (Driver.IsInVehicle())
        {
            SetTask(VehicleTask.Search);
        }
    }

    private void InitializeWeaponCrew()
    {
        _weaponCrewMembers.Clear();

        for (int i = 0; i < Crew.Count; i++)
        {
            var crewMember = Crew[i];
            if (!IsValidCrewMember(crewMember)) continue;

            VehicleSeat seat = (VehicleSeat)i;
            if (Vehicle.IsTurretSeat(seat))
            {
                _weaponCrewMembers.Add(crewMember);
                crewMember.SetCombatAttribute(CombatAttributes.CanLeaveVehicle, false);
            }
        }
    }
    #endregion

    #region Main Update Loop
    public void Update()
    {
        if (_disposed || !IsVehicleValid())
        {
            SetTask(VehicleTask.Idle);
            return;
        }

        int currentTime = Game.GameTime;

        // Throttle updates for performance
        if (currentTime - _lastUpdateTime < UPDATE_INTERVAL) return;
        _lastUpdateTime = currentTime;

        RefreshCrew();
        UpdateCrewCombatAttributes();
        CheckGlobalStateChanges();
        ExecuteCurrentTask();
    }

    private void ExecuteCurrentTask()
    {
        switch (_currentTask)
        {
            case VehicleTask.Goto:
                ExecuteGotoTask();
                break;
            case VehicleTask.Going:
                ExecuteGoingTask();
                break;
            case VehicleTask.Follow:
                ExecuteFollowTask();
                break;
            case VehicleTask.Following:
                // Logic handled in CheckGlobalStateChanges
                break;
            case VehicleTask.Search:
                ExecuteSearchTask();
                break;
            case VehicleTask.Searching:
                ExecuteSearchingTask();
                break;
            case VehicleTask.Flee:
                ExecuteFleeTask();
                break;
            case VehicleTask.Fleeing:
                ExecuteFleeingTask();
                break;
            case VehicleTask.Idle:
            default:
                // Minimal processing for idle state
                break;
        }
    }
    #endregion

    #region Task Execution Methods
    private void ExecuteGotoTask()
    {
        Vector3 targetLocation = GetCurrentPlayerPosition();
        ExecuteBoatMission(targetLocation, VehicleMissionType.GoTo, DEFAULT_SPEED, VehicleDrivingFlags.None, 30);
        SetTask(VehicleTask.Going);
    }

    private void ExecuteGoingTask()
    {
        Vector3 targetLocation = GetCurrentPlayerPosition();
        float distance = Vector3.Distance(Vehicle.Position, targetLocation);

        if (distance < ARRIVAL_DISTANCE)
        {
            SetTask(VehicleTask.Follow);
        }
    }

    private void ExecuteFollowTask()
    {
        if (Game.Player.Character.IsInVehicle())
        {
            ExecuteBoatMission(Game.Player.Character, VehicleMissionType.GoTo, DEFAULT_SPEED,
                              VehicleDrivingFlags.UseSwitchedOffNodes, -1);
        }
        else
        {
            Vector3 targetLocation = GetCurrentPlayerPosition();
            ExecuteBoatMission(targetLocation, VehicleMissionType.GoTo, DEFAULT_SPEED,
                              VehicleDrivingFlags.UseSwitchedOffNodes, -1);
        }
        SetTask(VehicleTask.Following);
    }

    private void ExecuteSearchTask()
    {
        Vector3 searchPos = HelperClass.FindSearchPointForBoat(GetCurrentPlayerPosition(), SEARCH_RADIUS);
        ExecuteSearchSequence(searchPos);
        SetTask(VehicleTask.Searching);
    }

    private void ExecuteSearchingTask()
    {
        if (Driver.TaskSequenceProgress == TASK_COMPLETION_CHECK)
        {
            // Task sequence completed - logic handled in CheckGlobalStateChanges
        }
    }

    private void ExecuteFleeTask()
    {
        Vector3 fleeLocation = GetCurrentPlayerPosition();
        ExecuteBoatMission(fleeLocation, VehicleMissionType.Flee, DEFAULT_SPEED,
                          VehicleDrivingFlags.UseSwitchedOffNodes, -1);

        Driver.BlockPermanentEvents = true;
        SetTask(VehicleTask.Fleeing);
    }

    private void ExecuteFleeingTask()
    {
        int elapsedTime = Game.GameTime - _taskStartTime;
        float distanceFromPlayer = Vector3.Distance(Vehicle.Position, Game.Player.Character.Position);

        if (elapsedTime > FLEE_TIMEOUT || distanceFromPlayer > FLEE_DISTANCE)
        {
            SetTask(VehicleTask.Idle);
        }
    }
    #endregion

    #region Mission Execution Helpers
    private void ExecuteBoatMission(Vector3 target, VehicleMissionType missionType, float speed,
                                   VehicleDrivingFlags flags, int timeout)
    {
        if (!IsVehicleValid()) return;

        try
        {
            Driver.Task.StartBoatMission(Vehicle, target, missionType, speed, flags, timeout, _missionFlags);
        }
        catch (Exception)
        {
            // Fallback if mission fails
        }
    }

    private void ExecuteBoatMission(Entity target, VehicleMissionType missionType, float speed,
                                   VehicleDrivingFlags flags, int timeout)
    {
        if (!IsVehicleValid() || target?.Exists() != true) return;

        try
        {
            if(Game.Player.Character.IsInVehicle())
                Driver.Task.StartBoatMission(Vehicle, (Vehicle)target, missionType, speed, flags, timeout, _missionFlags);
            else
                Driver.Task.StartBoatMission(Vehicle, (Ped)target, missionType, speed, flags, timeout, _missionFlags);
        }
        catch (Exception)
        {
            // Fallback if mission fails
        }
    }

    private void ExecuteSearchSequence(Vector3 searchPos)
    {
        if (!IsVehicleValid()) return;

        try
        {
            TaskSequence ts = new TaskSequence();
            ts.AddTask.StartBoatMission(Vehicle, searchPos, VehicleMissionType.GoTo, DEFAULT_SPEED,
                                       VehicleDrivingFlags.None, -1, _missionFlags);
            ts.AddTask.StartBoatMission(Vehicle, searchPos, VehicleMissionType.Circle, DEFAULT_SPEED,
                                       VehicleDrivingFlags.None, -1, _missionFlags);
            ts.AddTask.LeaveVehicle();
            ts.Close();

            Driver.Task.PerformSequence(ts);
        }
        catch (Exception)
        {
            // Fallback to simple goto if sequence fails
            ExecuteBoatMission(searchPos, VehicleMissionType.GoTo, DEFAULT_SPEED, VehicleDrivingFlags.None, -1);
        }
    }
    #endregion

    #region State Management
    private void SetTask(VehicleTask newTask)
    {
        if (_currentTask != newTask)
        {
            _previousTask = _currentTask;
            _currentTask = newTask;
            _taskStartTime = Game.GameTime;
        }
    }

    private void CheckGlobalStateChanges()
    {
        int wantedLevel = Game.Player.Wanted.WantedLevel;
        bool hasGrayedOutStars = Game.Player.Wanted.HasGrayedOutStars;

        if (wantedLevel == 0 && !IsFleeingOrIdle())
        {
            SetTask(VehicleTask.Flee);
        }
        else if (wantedLevel > 0)
        {
            HandleActiveWantedLevel(hasGrayedOutStars);
        }
    }

    private bool IsFleeingOrIdle()
    {
        return _currentTask == VehicleTask.Flee ||
               _currentTask == VehicleTask.Fleeing ||
               _currentTask == VehicleTask.Idle;
    }

    private void HandleActiveWantedLevel(bool hasGrayedOutStars)
    {
        if (hasGrayedOutStars && (_currentTask == VehicleTask.Following || _currentTask == VehicleTask.Going))
        {
            SetTask(VehicleTask.Search);
        }
        else if (!hasGrayedOutStars && _currentTask == VehicleTask.Searching)
        {
            SetTask(VehicleTask.Follow);
        }
    }
    #endregion

    #region Crew Management
    private void UpdateCrewCombatAttributes()
    {
        bool vehicleOnFire = Vehicle.EngineHealth < ENGINE_FIRE_THRESHOLD;

        foreach (var crewMember in _weaponCrewMembers.ToList())
        {
            if (!IsValidCrewMember(crewMember))
            {
                _weaponCrewMembers.Remove(crewMember);
                continue;
            }

            crewMember.SetCombatAttribute(CombatAttributes.CanLeaveVehicle, vehicleOnFire);
        }
    }

    private bool IsValidCrewMember(Ped crewMember)
    {
        return crewMember?.Exists() == true && crewMember.IsAlive;
    }
    #endregion

    #region Position Caching
    private Vector3 GetCurrentPlayerPosition()
    {
        int currentTime = Game.GameTime;

        if (currentTime - _lastPositionUpdate < POSITION_CACHE_DURATION)
        {
            return _lastPlayerPosition;
        }

        _lastPlayerPosition = ImportantChecks.LastKnownLocation;
        _lastPositionUpdate = currentTime;
        return _lastPlayerPosition;
    }
    #endregion

    #region Public Methods
    public bool IsVehicleValid()
    {
        return Vehicle?.Exists() == true && !Vehicle.IsDead &&
               Driver?.Exists() == true && !Driver.IsDead;
    }

    public bool ApplyWeaponAmmo(VehicleWeaponHash weaponHash, int numAmmo)
    {
        if (!IsVehicleValid() || numAmmo < 0) return false;

        try
        {
            Vehicle.SetWeaponRestrictedAmmo((int)weaponHash, numAmmo);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public VehicleTask GetCurrentTask()
    {
        return _currentTask;
    }

    public bool IsTaskInProgress()
    {
        return _currentTask != VehicleTask.Idle;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Dispose Pattern
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            Crew?.Clear();
            _weaponCrewMembers?.Clear();
        }

        _disposed = true;
    }
    #endregion
}
