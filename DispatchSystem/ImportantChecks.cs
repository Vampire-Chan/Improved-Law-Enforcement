using GTA;
using GTA.Chrono;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;

internal class ImportantChecks : Script
{
    // Cache values to reduce redundant checks
    private static Vector3 _lastKnownLocation;
    private static bool _cachedWaterResult;
    private static DateTime _lastWaterCheck = DateTime.MinValue;
    private const int WATER_CHECK_INTERVAL_MS = 500;

    public ImportantChecks()
    {
        Tick += OnTick;
        _lastKnownLocation = Game.Player.Character.Position.Around(40); //set initial known position as around.
    }

    public static Vector3 LastKnownLocation => _lastKnownLocation;

    public static bool IsInOrAroundWater
    {
        get
        {
            TimeSpan timeSinceLastCheck = DateTime.Now - _lastWaterCheck;
            if (timeSinceLastCheck.TotalMilliseconds < WATER_CHECK_INTERVAL_MS)
                return _cachedWaterResult;

            var character = Game.Player.Character;
            _cachedWaterResult = character.IsInBoat ||
                                 character.IsInSub ||
                                 character.IsSwimming ||
                                 character.IsSwimmingUnderWater ||
                                 character.IsInWaterStrict ||
                                 IsInWaterBasedOnSurroundingCheck();

            _lastWaterCheck = DateTime.Now;
            return _cachedWaterResult;
        }
    }

    private static bool IsInWaterBasedOnSurroundingCheck()
    {
        Vector3 playerPos = Game.Player.Character.Position;

        // Check if player is directly in water
        if (GetWaterHeight(playerPos) > playerPos.Z)
            return true;

        // Check surrounding positions in a more efficient way
        const float checkDistance = 25f;
        Vector3[] checkDirections = {
            new Vector3(0, checkDistance, 0),  // North
            new Vector3(checkDistance, 0, 0),  // East
            new Vector3(0, -checkDistance, 0), // South
            new Vector3(-checkDistance, 0, 0)  // West
        };

        foreach (var direction in checkDirections)
        {
            Vector3 checkPos = playerPos + direction;
            if (GetWaterHeight(checkPos) > checkPos.Z)
                return true;
        }

        return false;
    }

    public static List<Regions> RegionMappings { get; set; } = new List<Regions>();

    public static string CurrentJurisdiction
    {
        get
        {
            try
            {
                Vector3 pos = LastKnownLocation;
                string zoneName = Function.Call<string>(Hash.GET_NAME_OF_ZONE, pos.X, pos.Y, pos.Z);
                string streetName = World.GetStreetName(pos);

                // 1. Priority: Check if any region includes this street name
                var byStreet = RegionMappings.FirstOrDefault(r =>
                    r.StreetNames.Any(st => st.Equals(streetName, StringComparison.OrdinalIgnoreCase)));

                if (byStreet != null)
                {
                    HelperClass.Notification($"~b~Jurisdiction: ~y~{byStreet.Name}");
                    return byStreet.Name;
                }

                // 2. Fallback: Check if any region includes this zone name
                var byZone = RegionMappings.FirstOrDefault(r =>
                    r.ZoneName.Any(z => z.Equals(zoneName, StringComparison.OrdinalIgnoreCase)));

                if (byZone != null)
                {
                    HelperClass.Notification($"~b~Jurisdiction: ~y~{byZone.Name}");
                    return byZone.Name;
                }

                // 3. Unknown -> fallback to all, making every all or null setted vehiclesets will come underthis
                HelperClass.Notification("~b~Jurisdiction: ~r~Unknown");
                return "all";
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal($"CurrentJurisdiction error: {ex.Message}\n{ex.StackTrace}");
                return "all";
            }
        }
    }


    private static float GetWaterHeight(Vector3 position)
    {
        try
        {
            OutputArgument waterHeight = new OutputArgument();
            Function.Call(Hash.GET_WATER_HEIGHT, position.X, position.Y, position.Z, waterHeight);
            return waterHeight.GetResult<float>();
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"GetWaterHeight error: {ex.Message}\n{ex.StackTrace}");
            return 0f;
        }
    }

    public enum TimeOfDay
    {
        Dawn,      // 5:00 - 6:59
        Morning,   // 7:00 - 11:59
        Afternoon, // 12:00 - 17:59
        Evening,   // 18:00 - 20:59
        Night      // 21:00 - 4:59
    }

    public static TimeOfDay CurrentTimeOfDay
    {
        get
        {
            int hour = GameClock.Hour;

            if (hour >= 5 && hour < 7)
                return TimeOfDay.Dawn;
            else if (hour >= 7 && hour < 12)
                return TimeOfDay.Morning;
            else if (hour >= 12 && hour < 18)
                return TimeOfDay.Afternoon;
            else if (hour >= 18 && hour < 21)
                return TimeOfDay.Evening;
            else
                return TimeOfDay.Night;
        }
    }

    public static bool IsOutside
    {
        get
        {
            try
            {
                Vector3 position = _lastKnownLocation;
                return Function.Call<int>(Hash.GET_INTERIOR_AT_COORDS, position.X, position.Y, position.Z) == 0 ||
                       Function.Call<bool>(Hash.IS_POINT_ON_ROAD, position.X, position.Y, position.Z);
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal($"IsOutside error: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        try
        {
            UpdateLastKnownLocation();
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"OnTick error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static void UpdateLastKnownLocation()
    {
        try
        {
            if (Game.Player.Wanted.WantedLevel == 0 || !Game.Player.Wanted.HasGrayedOutStars)
            {
                _lastKnownLocation = Game.Player.Character.Position;
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"UpdateLastKnownLocation error: {ex.Message}\n{ex.StackTrace}");
        }
    }
}