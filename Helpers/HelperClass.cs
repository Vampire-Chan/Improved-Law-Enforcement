using GTA.Native;
using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA.Math;
using System.Text.RegularExpressions;
using DispatchSystem;


public static class HelperClass
{
    #region Shared Resources and Caching
    // Consolidated random instance
    public static readonly Random SharedRandom = new Random();
    [Obsolete("Use SharedRandom instead")]
    public static readonly Random rand = SharedRandom; // Backward compatibility

    // Hash caching for performance
    private static readonly Dictionary<string, uint> _hashCache = new Dictionary<string, uint>();
    private static readonly Dictionary<string, Model> _modelCache = new Dictionary<string, Model>();

    // Reusable OutputArguments to reduce allocations
    private static readonly OutputArgument _reusableOutputArg = new OutputArgument();
    private static readonly OutputArgument _reusableOutputArg2 = new OutputArgument();
    #endregion

    #region Utility Methods with Caching
    private static uint GetHashCached(string input)
    {
        if (!_hashCache.TryGetValue(input, out uint hash))
        {
            hash = (uint)Function.Call<int>(Hash.GET_HASH_KEY, input);
            _hashCache[input] = hash;
        }
        return hash;
    }

    private static Model GetModelCached(string modelName)
    {
        if (!_modelCache.TryGetValue(modelName, out Model model))
        {
            model = new Model(modelName);
            _modelCache[modelName] = model;
        }
        return model;
    }

    public static Vector3 GetCrosshairCoords()
    {
        try
        {
            Logger.Log.Info("Getting Crosshair Coordinates!!");
            return World.Raycast(GameplayCamera.Position, GameplayCamera.Direction, 1000f, IntersectFlags.Everything, Game.Player.Character).HitPosition;
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"Error getting crosshair coordinates: {ex.Message}");
            return Vector3.Zero;
        }
    }

    public static void Subtitle(string msg)
    {
        GTA.UI.Screen.ShowSubtitle(msg);
    }

    public static void Notification(string msg)
    {
        GTA.UI.Notification.PostTicker(msg, false);
    }

    public static float GetRandomFloat(double min, double max)
    {
        return (float)(SharedRandom.NextDouble() * (max - min) + min);
    }

    public static double GetDouble()
    {
        return SharedRandom.NextDouble();
    }

    public static bool GetBool()
    {
        return GetDouble() >= 0.5;
    }

    public static Vector3 RandomPointInsideCircle(Vector3 center, float radius)
    {
        double distance = (double)radius * Math.Sqrt(GetDouble());
        double angle = GetDouble() * 2.0 * Math.PI;
        return new Vector3(center.X + (float)(distance * Math.Cos(angle)), center.Y + (float)(distance * Math.Sin(angle)), center.Z);
    }

    public static float NormalizeAngle(float value)
    {
        const float fullCircle = 360f;
        return value - (float)Math.Floor(value / fullCircle) * fullCircle;
    }

    public static Vector3 GetPointBetweenTwoVectors(Vector3 start, Vector3 end, float ratio)
    {
        return new Vector3(
            start.X + (end.X - start.X) * ratio,
            start.Y + (end.Y - start.Y) * ratio,
            start.Z + (end.Z - start.Z) * ratio
        );
    }

    public static float GetAngleBetweenTwoPoints(Vector3 source, Vector3 target)
    {
        return (target - source).Normalized.ToHeading();
    }
    #endregion

    #region Ped Extensions with Error Handling
    public static int GetLastDamageBone(this Ped ped)
    {
        try
        {
            Function.Call<bool>(Hash.GET_PED_LAST_DAMAGE_BONE, ped, _reusableOutputArg);
            return _reusableOutputArg.GetResult<int>();
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"Error getting last damage bone: {ex.Message}");
            return -1;
        }
    }

    public static void ClearLastDamageBone(this Ped ped)
    {
        try
        {
            Function.Call(Hash.CLEAR_PED_LAST_DAMAGE_BONE, ped);
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"Error clearing last damage bone: {ex.Message}");
        }
    }

    public static void PlayAmbientSpeech(this Ped ped, string speechFile, bool immediately)
    {
        try
        {
            if (immediately)
            {
                Function.Call(Hash.STOP_CURRENT_PLAYING_AMBIENT_SPEECH, ped);
            }
            Function.Call(Hash.SET_AUDIO_FLAG, "IsDirectorModeActive", 1);
            Function.Call(Hash.PLAY_PED_AMBIENT_SPEECH_NATIVE, ped, speechFile, "SPEECH_PARAMS_FORCE");
            Function.Call(Hash.SET_AUDIO_FLAG, "IsDirectorModeActive", 0);
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"Error playing ambient speech: {ex.Message}");
        }
    }

    public static bool HasBeenDamagedByWeapon(this Ped ped, WeaponHash weapon)
    {
        try
        {
            return Function.Call<bool>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, ped, weapon.GetHashCode(), 0);
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"Error checking weapon damage: {ex.Message}");
            return false;
        }
    }

    public static void ClearLastWeaponDamage(this Ped ped)
    {
        try
        {
            Function.Call(Hash.CLEAR_PED_LAST_WEAPON_DAMAGE, ped);
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"Error clearing last weapon damage: {ex.Message}");
        }
    }

    public static bool IsTaskActive(this Ped ped, PedTask taskId)
    {
        return Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped.Handle, (int)taskId);
    }

    public static void SetPedCycleVehicleWeapon(this Ped ped)
    {
        Function.Call(Hash.SET_PED_CYCLE_VEHICLE_WEAPONS_ONLY, ped);
    }

    public static void SetDriverAbility(this Ped ped, float value)
    {
        Function.Call(Hash.SET_DRIVER_ABILITY, ped, value);
    }

    public static void AssignDefaultTask(this Ped ped)
    {
        Function.Call(Hash.CLEAR_DEFAULT_PRIMARY_TASK, ped.Handle);
    }

    public static void StandGuard(this Ped ped, Vector3 defend, float heading, string animScenario)
    {
        Function.Call(Hash.TASK_STAND_GUARD, ped, defend.X, defend.Y, defend.Z, heading, animScenario);
    }

    public static void GuardCurrentPosition(this Ped ped, bool defensive)
    {
        Function.Call(Hash.TASK_GUARD_CURRENT_POSITION, ped, 40f, 35f, defensive);
    }
    #endregion

    #region Weapon Extensions with Cached Hashes
    public static void GiveWeaponWithComponent(this WeaponHash weapon, Ped ped, WeaponComponentHash component)
    {
        Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, weapon, component);
    }

    public static void RemoveWeaponWithComponent(this WeaponHash weapon, Ped ped, WeaponComponentHash component)
    {
        Function.Call(Hash.REMOVE_WEAPON_COMPONENT_FROM_PED, ped, weapon, component);
    }

    public static void GiveSpecialAmmo(this WeaponHash weapon, Ped ped, string ammoType)
    {
        Function.Call(Hash.ADD_PED_AMMO_BY_TYPE, ped, StringHash.AtStringHash(ammoType));
    }

    public static int GetWeaponComponentExtraCount(this WeaponComponent component, WeaponHash weapon)
    {
        return Function.Call<int>(Hash.GET_WEAPON_COMPONENT_VARIANT_EXTRA_COUNT, weapon);
    }
    #endregion

    #region Prop Extensions
    public static bool PlaceOnGround(this Prop prop)
    {
        return Function.Call<bool>(Hash.PLACE_OBJECT_ON_GROUND_PROPERLY, prop.Handle);
    }

    public static void ForceVehiclesToAvoid(this Prop prop, bool toggle)
    {
        Function.Call(Hash.SET_OBJECT_FORCE_VEHICLES_TO_AVOID, prop.Handle, toggle);
    }
    #endregion

    #region Relationship Management
    public static Relationship GetRelationshipBetweenGroups(int group1, int group2)
    {
        return (Relationship)Function.Call<int>(Hash.GET_RELATIONSHIP_BETWEEN_GROUPS, group1, group2);
    }

    public static void SetRelationshipBetweenGroups(Relationship relationship, int group1, int group2)
    {
        Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, (int)relationship, group1, group2);
        Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, (int)relationship, group2, group1);
    }
    #endregion

    #region Vehicle Creation and Configuration
    public static Vehicle CreateVehicle(DispatchVehicleInfo info, Vector3 pos, float head)
    {
        if (!ValidateDispatchInfo(info)) return null;

        try
        {
            var vehicleData = SelectRandomVehicleData(info);
            if (vehicleData == null) return null;

            Vehicle vehicle = CreateAndConfigureVehicle(vehicleData, pos, head);
            if (vehicle == null) return null;

            if (!PopulateVehicleWithCrew(vehicle, info))
            {
                vehicle.Delete();
                return null;
            }

            return vehicle;
        }
        catch (Exception ex)
        {
            Notification($"[VehicleFactory] Critical error: {ex.Message}");
            Logger.Log.Fatal($"[VehicleFactory ERROR] {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }

    private static bool ValidateDispatchInfo(DispatchVehicleInfo info)
    {
        if (info == null)
        {
            Notification("[VehicleFactory] Info is null");
            return false;
        }

        if (info.Vehicles?.Count == 0)
        {
            Notification("[VehicleFactory] No vehicles in config");
            return false;
        }

        return true;
    }

    private static VehicleInfo SelectRandomVehicleData(DispatchVehicleInfo info)
    {
        var group = info.Vehicles[SharedRandom.Next(info.Vehicles.Count)];
        var vehicleData = group?.Vehicles?[SharedRandom.Next(group.Vehicles.Count)];

        if (vehicleData?.Model == null)
        {
            Notification("[VehicleFactory] Invalid vehicle data");
            return null;
        }

        return vehicleData;
    }

    private static Vehicle CreateAndConfigureVehicle(VehicleInfo vehicleData, Vector3 pos, float head)
    {
        Vehicle vehicle = World.CreateVehicle(vehicleData.Model, pos, head);

        if (!vehicle.Exists())
        {
            Notification($"[VehicleFactory] {vehicleData.Model}Failed to create vehicle");
            return null;
        }

        ConfigureVehicle(vehicle, vehicleData);
        return vehicle;
    }

    private static void ConfigureVehicle(Vehicle vehicle, VehicleInfo vehicleData)
    {
        vehicle.InstallModKit();
        ApplyVehicleMods(vehicle, vehicleData);
        ApplyVehicleLiveries(vehicle, vehicleData);
        ApplyVehicleHealth(vehicle, vehicleData);

        if (vehicle.IsAircraft || vehicle.IsHelicopter || vehicle.IsPlane)
        {
            vehicle.HeliBladesSpeed = 1f;
        }
    }

    private static bool PopulateVehicleWithCrew(Vehicle vehicle, DispatchVehicleInfo info)
    {
        if (!CreatePilot(vehicle, info))
        {
            Notification("[VehicleFactory] Failed to create pilot");
            return false;
        }

        if(vehicle.PassengerCapacity != 1) CreatePassengers(vehicle, info);
        return true;
    }

    private static bool CreatePilot(Vehicle vehicle, DispatchVehicleInfo info)
    {
        if (info.Pilots?.Count == 0)
        {
            Notification("[VehicleFactory] No pilot config");
            return false;
        }

        try
        {
            var pilotData = info.Pilots[SharedRandom.Next(info.Pilots.Count)];
            var pilotModel = pilotData?.Peds?[SharedRandom.Next(pilotData.Peds.Count)];

            if (pilotModel == null)
            {
                Notification("[VehicleFactory] Invalid pilot model");
                return false;
            }

            var driver = vehicle.CreatePed(
                pilotModel,
                new DualWeapons(info.PrimaryWeapons, info.SecondaryWeapons),
                VehicleSeat.Driver,
                PedType.Cop,
                Vector3.Zero
            );

            return driver?.Exists() == true;
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"[VehicleFactory] Pilot creation error: {ex.Message}");
            return false;
        }
    }

    private static void CreatePassengers(Vehicle vehicle, DispatchVehicleInfo info)
    {
        if (vehicle.PassengerCapacity <= 0 || info.Soldiers?.Count == 0) return;

        for (int i = 0; i < vehicle.PassengerCapacity; i++)
        {
            CreatePassenger(vehicle, info, (VehicleSeat)i);
        }
    }

    private static void CreatePassenger(Vehicle vehicle, DispatchVehicleInfo info, VehicleSeat seat)
    {
        try
        {
            var soldierData = info.Soldiers[SharedRandom.Next(info.Soldiers.Count)];
            var soldierModel = soldierData?.Peds?[SharedRandom.Next(soldierData.Peds.Count)];

            if (soldierModel == null) return;

            vehicle.CreatePed(
                soldierModel,
                new DualWeapons(info.PrimaryWeapons, info.SecondaryWeapons),
                seat,
                PedType.Cop,
                Vector3.Zero
            );
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"[VehicleFactory] Passenger creation error: {ex.Message}");
        }
    }
    #endregion

    #region Vehicle Modification Methods
    private static readonly Random _rng = SharedRandom; // Use shared instance

    private static void ApplyVehicleMods(Vehicle vehicle, VehicleInfo details)
    {
        var mods = details?.Attributes?.Mods;
        if (mods?.Count == 0) return;

        foreach (var mod in mods)
        {
            ApplyVehicleMod(vehicle, mod);
        }
    }

    private static void ApplyVehicleMod(Vehicle vehicle, VehicleMod mod)
    {
        if (!VehicleExtensions.TryParseModType(mod.Type, out var modType)) return;

        int modCount = vehicle.GetModCount(modType);
        if (modCount <= 0) return;

        int modIndex = DetermineModIndex(mod, modCount);
        vehicle.SetVehicleMod(modType, modIndex, false);
    }

    private static int DetermineModIndex(VehicleMod mod, int modCount)
    {
        if (!mod.Index.HasValue)
        {
            return 0; // null index → default to 0
        }

        if (mod.Index.Value == -1)
        {
            return _rng.Next(modCount); // -1 → random
        }

        // Use provided index, clamp to valid range
        return mod.Index.Value >= modCount ? 0 : mod.Index.Value;
    }

    private static void ApplyVehicleLiveries(Vehicle vehicle, VehicleInfo details)
    {
        var attributes = details?.Attributes;
        if (attributes == null) return;

        if (attributes.LiveryIndex.HasValue)
        {
            vehicle.SetLivery(0, attributes.LiveryIndex.Value);
        }

        if (attributes.LiveryIndex2.HasValue)
        {
            vehicle.SetLivery(1, attributes.LiveryIndex2.Value);
        }
    }

    private static void ApplyVehicleHealth(Vehicle vehicle, VehicleInfo details)
    {
        var health = details?.Attributes?.Health;
        if (health == null) return;

        if (health.Body.HasValue)
        {
            vehicle.BodyHealth = health.Body.Value;
        }

        ApplyCategoryBonuses(vehicle, details);
    }

    public static void ApplyCategoryBonuses(Vehicle vehicle, VehicleInfo details)
    {
        var categories = details?.Attributes?.VehicleCategory;
        if (categories == null) return;

        foreach (var categoryType in categories)
        {
            ApplyCategoryBonus(vehicle, categoryType);
        }
    }

    private static void ApplyCategoryBonus(Vehicle vehicle, VehicleCategoryType categoryType)
    {
        switch (categoryType)
        {
            case VehicleCategoryType.Medium:
                vehicle.MaxHealth += 500;
                vehicle.EngineHealth += 500;
                break;

            case VehicleCategoryType.Heavy:
                vehicle.MaxHealth += 1000;
                vehicle.EngineHealth += 1000;
                vehicle.IsAxlesStrong = true;
                vehicle.IsCollisionProof = true;
                break;

            case VehicleCategoryType.ExplosiveProof:
                vehicle.IsExplosionProof = true;
                vehicle.IsBulletProof = true;
                vehicle.IsCollisionProof = true;
                break;

            case VehicleCategoryType.BulletProof:
                vehicle.CanTiresBurst = false;
                vehicle.IsBulletProof = true;
                break;

            case VehicleCategoryType.FireProof:
                vehicle.IsFireProof = true;
                vehicle.CanTiresBurst = false;
                break;
        }
    }
    #endregion

    #region Ped Creation and Configuration
    public static Ped CreatePed(this Vehicle vehicle, PedModel pedModel, DualWeapons weaponInfo, VehicleSeat seat, PedType pedType, Vector3 spawnPos)
    {
        if (!ValidatePedCreationParameters(vehicle, pedModel)) return null;

        try
        {
            var model = LoadPedModel(pedModel.Model);
            if (model == null) return null;

            Ped ped = CreatePedAtPosition(model, vehicle, seat, pedType, spawnPos);
            if (ped?.Exists() != true) return null;

            ConfigurePed(ped, vehicle, pedModel, weaponInfo, seat);
            RegisterPed(ped);

            return ped;
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"[CreatePed] Error: {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }

    private static bool ValidatePedCreationParameters(Vehicle vehicle, PedModel pedModel)
    {
        if (!vehicle.Exists())
        {
            Logger.Log.Fatal("[CreatePed] Vehicle does not exist.");
            return false;
        }

        if (string.IsNullOrEmpty(pedModel?.Model))
        {
            Logger.Log.Fatal("[CreatePed] Invalid ped model.");
            return false;
        }

        return true;
    }

    private static Model LoadPedModel(string modelName)
    {
        try
        {
            var pedModel = GetModelCached(modelName);
            if (!pedModel.IsLoaded)
            {
                pedModel.Request(1000);
            }

            if (!pedModel.IsValid || !pedModel.IsInCdImage)
            {
                Logger.Log.Fatal($"[CreatePed] Invalid ped model: {modelName}");
                return null;
            }

            return pedModel;
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"[CreatePed] Model loading error: {ex.Message}");
            return null;
        }
    }

    private static Ped CreatePedAtPosition(Model model, Vehicle vehicle, VehicleSeat seat, PedType pedType, Vector3 spawnPos)
    {
        Ped ped;

        if (seat == (VehicleSeat)(-2)) // Special cargo bay seat
        {
            ped = Function.Call<Ped>(Hash.CREATE_PED, pedType, model, spawnPos.X, spawnPos.Y, spawnPos.Z, -90, true, true);
        }
        else
        {
            ped = Function.Call<Ped>(Hash.CREATE_PED_INSIDE_VEHICLE, vehicle, (int)pedType, model, seat, true, true);
        }

        if (ped?.Exists() == true)
        {
            model.MarkAsNoLongerNeeded();
            Logger.Log.Info($"[CreatePed] Created ped: {ped.Handle}");
        }
        else
        {
            Logger.Log.Fatal("[CreatePed] Failed to create ped.");
        }

        return ped;
    }

    private static void ConfigurePed(Ped ped, Vehicle vehicle, PedModel pedModel, DualWeapons weaponInfo, VehicleSeat seat)
    {
        ConfigureBasicPedAttributes(ped, vehicle);
        ApplyPedVariants(ped, pedModel);
        ConfigureAdvancedPedSettings(ped);
        AssignPedWeapons(ped, weaponInfo, seat);
    }

    private static void ConfigureBasicPedAttributes(Ped ped, Vehicle vehicle)
    {
        // Basic combat attributes
        ped.SetAsCop(true);
        ped.SetCombatAttribute(CombatAttributes.CanFightArmedPedsWhenNotArmed, false);
        ped.SetCombatAttribute(CombatAttributes.AlwaysFlee, false);
        ped.SetCombatAttribute(CombatAttributes.CanCommandeerVehicles, true);
        ped.SetCombatAttribute(CombatAttributes.CanTauntInVehicle, true);
        ped.SetCombatAttribute(CombatAttributes.UseVehicleAttackIfVehicleHasMountedGuns, true);
        ped.SetCombatAttribute(CombatAttributes.CanChaseTargetOnFoot, true);
        ped.SetCombatAttribute(CombatAttributes.CanUseCover, true);
        ped.SetCombatAttribute(CombatAttributes.SwitchToDefensiveIfInCover, true);
        ped.SetCombatAttribute(CombatAttributes.BlindFireWhenInCover, true);
        ped.SetCombatAttribute(CombatAttributes.CanUsePeekingVariations, true);
        ped.SetConfigFlag(PedConfigFlagToggles.AIDriverAllowFriendlyPassengerSeatEntry, true);
        
       // ped.SetConfigFlag(PedConfigFlagToggles.ai, true);
        
        ped.TargetLossResponse = TargetLossResponse.NeverLoseTarget;
        // Vehicle-specific attributes
        ConfigureVehicleSpecificAttributes(ped, vehicle);
    }

    private static void ConfigureVehicleSpecificAttributes(Ped ped, Vehicle vehicle)
    {
        if (vehicle.IsAircraft)
        {
            ped.SetCombatAttribute(CombatAttributes.CanLeaveVehicle, false);
            ped.SetCombatAttribute(CombatAttributes.ForceCheckAttackAngleForMountedGuns, true);
            ped.SetCombatAttribute(CombatAttributes.PreferAirCombatWhenInAircraft, true);
        }
        else
        {
            ped.SetCombatAttribute(CombatAttributes.RequiresLosToAim, true);
            ped.SetCombatAttribute(CombatAttributes.PreferNonAircraftTargets, true);
        }

        if (vehicle.IsBoat || vehicle.IsAmphibiousAutomobile || vehicle.IsAmphibiousQuadBike || vehicle.IsSubmarine)
        {
            ped.SetCombatAttribute(CombatAttributes.CanSeeUnderwaterPeds, true);
        }

        ped.SetCombatAttribute(CombatAttributes.UseVehicleAttack, true);
    }

    private static void ApplyPedVariants(Ped ped, PedModel pedModel)
    {
        if (pedModel?.Variants == null) return;

        ApplyPedClothing(ped, pedModel.Variants);
        ApplyPedAttributes(ped, pedModel.Variants);
    }

    private static void ApplyPedClothing(Ped ped, PedVariant variants)
    {
        if (variants.Clothing?.Count == 0)
        {
            ApplyRandomClothing(ped);
            return;
        }

        bool hasClothingComponents = false;
        bool hasPropComponents = false;

        foreach (var clothing in variants.Clothing)
        {
            if (ApplyClothingComponent(ped, clothing))
                hasClothingComponents = true;

            if (ApplyPropComponent(ped, clothing))
                hasPropComponents = true;
        }

        // Apply random for missing categories
        if (!hasClothingComponents)
        {
            Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, ped, 0);
        }

        if (!hasPropComponents)
        {
            Function.Call(Hash.SET_PED_RANDOM_PROPS, ped);
        }
    }

    private static void ApplyRandomClothing(Ped ped)
    {
        Function.Call(Hash.SET_PED_RANDOM_PROPS, ped);
        Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, ped, 0);
    }

    private static bool ApplyClothingComponent(Ped ped, ClothComponents clothing)
    {
        if (clothing.Type.HasValue && clothing.TypeIndex.HasValue && clothing.Index.HasValue)
        {
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, clothing.Type.Value, clothing.TypeIndex.Value, clothing.Index.Value, 1);
            return true;
        }
        return false;
    }

    private static bool ApplyPropComponent(Ped ped, ClothComponents clothing)
    {
        if (clothing.PropType.HasValue && clothing.PropTypeIndex.HasValue && clothing.PropIndex.HasValue)
        {
            Function.Call(Hash.SET_PED_PROP_INDEX, ped, clothing.PropType.Value, clothing.PropTypeIndex.Value, clothing.PropIndex.Value, true);
            return true;
        }
        return false;
    }

    private static void ApplyPedAttributes(Ped ped, PedVariant variants)
    {
        var attributes = variants.Attributes;
        if (attributes == null) return;

        ApplyJuggernautAttributes(ped, attributes);
        ApplyBasicAttributes(ped, attributes);
        ApplyEnumAttributes(ped, attributes);
        ApplySpecialAttributes(ped, attributes);
    }

    private static void ApplyJuggernautAttributes(Ped ped, PedAttributes attributes)
    {
        if (!attributes.Juggernaut) return;

        ped.SetCombatAttribute(CombatAttributes.AlwaysEquipBestWeapon, true);
        ped.SetConfigFlag(PedConfigFlagToggles.DisableHelmetArmor, false);
        ped.SetConfigFlag(PedConfigFlagToggles.BlockCoweringInCover, true);
        ped.SetConfigFlag(PedConfigFlagToggles.DisableLadderClimbing, true);
        ped.SetCombatAttribute(CombatAttributes.DisableInjuredOnGround, true);
        ped.SetConfigFlag(PedConfigFlagToggles.CanPerformArrest, true);
        ped.SetConfigFlag(PedConfigFlagToggles.HasBulletProofVest, true);
        ped.SetConfigFlag(PedConfigFlagToggles.DisableInjuredCryForHelpEvents, false);
        ped.IsBulletProof = true;
        ped.IsMeleeProof = true;
        ped.IsSmokeProof = true;
        ped.CanSufferCriticalHits = true;
        ped.DiesOnLowHealth = false;
        ped.CanWrithe = false;
        MainClass.BlacklistedPeds.Add(ped.Model);
        ped.SetRagdollBlockingFlags(RagdollBlockingFlags.BulletImpact | RagdollBlockingFlags.Electrocution |
                                   RagdollBlockingFlags.Melee | RagdollBlockingFlags.RubberBullet | RagdollBlockingFlags.SmokeGrenade);
    }

    private static void ApplyBasicAttributes(Ped ped, PedAttributes attributes)
    {
        if (attributes.HearingRange.HasValue)
            ped.HearingRange = attributes.HearingRange.Value;

        if (attributes.Health.HasValue)
        {
            ped.MaxHealth = attributes.Health.Value;
            ped.Health = attributes.Health.Value;
        }

        if (attributes.Armour.HasValue)
            ped.Armor = attributes.Armour.Value;

        if (attributes.Accuracy.HasValue)
            ped.Accuracy = attributes.Accuracy.Value;

        if (attributes.SeeingRange.HasValue)
            ped.SeeingRange = attributes.SeeingRange.Value;

        if (attributes.RateOfFire.HasValue)
            ped.ShootRate = attributes.RateOfFire.Value;

        ped.IsSmokeProof = attributes.SmokeProof;
    }

    private static void ApplyEnumAttributes(Ped ped, PedAttributes attributes)
    {
        if (Enum.TryParse(attributes.Ability, true, out CombatAbility ability))
            ped.CombatAbility = ability;

        if (Enum.TryParse(attributes.Movement, true, out CombatMovement movement))
            ped.CombatMovement = movement;

        if (Enum.TryParse(attributes.FightRange, true, out CombatRange range))
            ped.CombatRange = range;

        if (Enum.TryParse(attributes.FiringPattern, true, out FiringPattern pattern))
            ped.FiringPattern = pattern;
    }

    private static void ApplySpecialAttributes(Ped ped, PedAttributes attributes)
    {
        if (ped.PedType == PedType.Swat || ped.PedType == PedType.Army)
        {
            ped.SetCombatAttribute(CombatAttributes.CanThrowSmokeGrenade, true);
        }
    }

    private static void ConfigureAdvancedPedSettings(Ped ped)
    {
        Function.Call(Hash.SET_PED_CAN_LOSE_PROPS_ON_DAMAGE, ped, true, 0);

        // Configure combat and behavior attributes
        ped.SetCombatAttribute(CombatAttributes.CanDoDrivebys, true);
        ped.SetCombatAttribute(CombatAttributes.CanChaseTargetOnFoot, true);
        ped.SetCombatAttribute(CombatAttributes.CanUseVehicles, true);
        ped.SetCombatAttribute(CombatAttributes.MoveToLocationBeforeCoverSearch, true);
        ped.SetCombatAttribute(CombatAttributes.WillDragInjuredPedsToSafety, true);
        ped.SetCombatAttribute(CombatAttributes.UseRocketsAgainstVehiclesOnly, true);

        // Configure flags efficiently
        var flagsToSet = new[]
        {
            PedConfigFlagToggles.CreatedByDispatch,
            PedConfigFlagToggles.LawWillOnlyAttackIfPlayerIsWanted,
            PedConfigFlagToggles.KeepRelationshipGroupAfterCleanUp,
            PedConfigFlagToggles.KeepTasksAfterCleanUp,
            PedConfigFlagToggles.KeepTargetLossResponseOnCleanup,
            PedConfigFlagToggles.DontAttackPlayerWithoutWantedLevel,
            PedConfigFlagToggles.DisableGoToWritheWhenInjured,
            PedConfigFlagToggles.AllowMedicsToReviveMe,
            PedConfigFlagToggles.AvoidTearGas,
            PedConfigFlagToggles.CanPerformArrest
        };

        foreach (var flag in flagsToSet)
        {
            ped.SetConfigFlag(flag, true);
        }

        var flagsToUnset = new[]
        {
            PedConfigFlagToggles.TargetWhenInjuredAllowed,
            PedConfigFlagToggles.CanAttackFriendly,
            PedConfigFlagToggles.DisableBlindFiringInShotReactions
        };

        foreach (var flag in flagsToUnset)
        {
            ped.SetConfigFlag(flag, false);
        }

        ped.ControlMountedWeapon();
        ped.SetDriverAbility(1f);
        ped.DrivingAggressiveness = 2f;
        
        //ped.VehicleDrivingFlags = VehicleDrivingFlags.
    }

    private static void AssignPedWeapons(Ped ped, DualWeapons weaponInfo, VehicleSeat seat)
    {
        if (seat == VehicleSeat.Driver)
        {
            AssignWeapons(ped, null, weaponInfo.Secondary); // Pilot gets only secondary
        }
        else
        {
            AssignWeapons(ped, weaponInfo.Primary, weaponInfo.Secondary);
        }
    }

    private static void RegisterPed(Ped ped)
    {
        DispatchManager.Police.Add(ped);
        Logger.Log.Info($"[CreatePed] Successfully created and registered ped: {ped.Handle}");
    }
    #endregion

    #region Weapon Assignment with Optimized Hash Handling
    public static void AssignWeapons(Ped ped, List<WeaponData> primaryWeapons, List<WeaponData> secondaryWeapons)
    {
        if (ped?.Exists() != true)
        {
            Logger.Log.Fatal("[AssignWeapons] Ped is null or doesn't exist.");
            return;
        }

        AssignWeaponCategory(ped, primaryWeapons, "primary");
        AssignWeaponCategory(ped, secondaryWeapons, "secondary");
    }

    private static void AssignWeaponCategory(Ped ped, List<WeaponData> weapons, string categoryName)
    {
        if (weapons?.Any(w => w.Weapons?.Count > 0) != true)
        {
            Logger.Log.Info($"[AssignWeapons] No {categoryName} weapons available.");
            return;
        }

        try
        {
            var allWeapons = weapons.SelectMany(w => w.Weapons).ToList();
            if (allWeapons.Count == 0) return;

            var chosenWeapon = allWeapons[SharedRandom.Next(allWeapons.Count)];
            int ammo = chosenWeapon.Ammo ?? (categoryName == "primary" ? 340 : 30);

            ped.Weapons.Give(chosenWeapon.Model, ammo, true, true);
            ApplyWeaponAttachments(ped, chosenWeapon.Model, chosenWeapon.Attachments);

            ped.CanSwitchWeapons = true;

            ped.Weapons.Select(ped.Weapons.BestWeapon, true);
            //ped.SetConfigFlag(PedConfigFlagToggles.weapon)
            Logger.Log.Info($"[AssignWeapons] Assigned {categoryName} weapon: {chosenWeapon.Model} with {ammo} ammo");
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"[AssignWeapons] Error assigning {categoryName} weapons: {ex.Message}");
        }
    }

    private static void ApplyWeaponAttachments(Ped ped, string weaponName, List<string> attachments)
    {
        if (ped?.Exists() != true || attachments?.Count == 0)
        {
            return;
        }

        if (!ped.Weapons.HasWeapon(weaponName))
        {
            Logger.Log.Info($"[ApplyWeaponAttachments] Ped does not have weapon: {weaponName}");
            return;
        }

        try
        {
            uint weaponHash = GetHashCached(weaponName);

            foreach (string attachment in attachments)
            {
                ApplySingleAttachment(ped, weaponHash, weaponName, attachment);
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"[ApplyWeaponAttachments] Error applying attachments to {weaponName}: {ex.Message}");
        }
    }

    private static void ApplySingleAttachment(Ped ped, uint weaponHash, string weaponName, string attachmentName)
    {
        try
        {
            uint attachmentHash = GetHashCached(attachmentName);
            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped.Handle, weaponHash, attachmentHash);
        }
        catch (Exception ex)
        {
            Logger.Log.Info($"[ApplyWeaponAttachments] Failed to apply attachment {attachmentName} on {weaponName}: {ex.Message}");
        }
    }
    #endregion

    #region Water and Height Utilities with Reusable OutputArguments
    public static float GetWaterHeight(Vector3 position)
    {
        try
        {
            Function.Call(Hash.GET_WATER_HEIGHT, position.X, position.Y, position.Z, _reusableOutputArg);
            return _reusableOutputArg.GetResult<float>();
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"GetWaterHeight error: {ex.Message}");
            return 0f;
        }
    }

    public static unsafe bool GetWaterLevelNoWaves(Vector3 startPoint, out float height)
    {
        height = 0f;
        try
        {
            if (Function.Call<bool>(Hash.GET_WATER_HEIGHT_NO_WAVES, startPoint.X, startPoint.Y, startPoint.Z, _reusableOutputArg))
            {
                height = _reusableOutputArg.GetResult<float>();
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"GetWaterLevelNoWaves error: {ex.Message}");
        }
        return false;
    }
    #endregion

    #region Native Function Utilities with Optimized OutputArguments
    public static Vector3 GetHeliSpawnCoordinates(this Ped ped)
    {
        return Function.Call<Vector3>(Hash.FIND_SPAWN_COORDINATES_FOR_HELI, ped);
    }

    public static int GetWantedLevelThreshold(int wantedLevel)
    {
        return Function.Call<int>(Hash.GET_WANTED_LEVEL_THRESHOLD, wantedLevel);
    }

    public static int GetNumberOfResourcesAssignedToWantedLevel(DispatchType dispatchType)
    {
        return Function.Call<int>(Hash.GET_NUMBER_RESOURCES_ALLOCATED_TO_WANTED_LEVEL, (int)dispatchType);
    }

    public static bool DoesScenarioExistInArea(Vector3 searchArea, float radius, bool unoccupied)
    {
        return Function.Call<bool>(Hash.DOES_SCENARIO_EXIST_IN_AREA, searchArea.X, searchArea.Y, searchArea.Z, radius, unoccupied);
    }

    public static bool IsSphereVisibleToPlayer(Vector3 center, float radius)
    {
        return Function.Call<bool>(Hash.IS_SPHERE_VISIBLE, center.X, center.Y, center.Z, radius);
    }

    public static int AddSpeedZone(Vector3 position, float radius, float speed, bool affectsMissionVehs = false)
    {
        return Function.Call<int>(Hash.ADD_ROAD_NODE_SPEED_ZONE, position.X, position.Y, position.Z, radius, speed, affectsMissionVehs);
    }

    public static void RemoveSpeedZone(int id)
    {
        Function.Call(Hash.REMOVE_ROAD_NODE_SPEED_ZONE, id);
    }

    public static unsafe bool FindSpawnPointInDirection(Vector3 position, Vector3 direction, float idealDistance, out Vector3 spawnPoint)
    {
        if (Function.Call<bool>(Hash.FIND_SPAWN_POINT_IN_DIRECTION,
            position.X, position.Y, position.Z,
            direction.X, direction.Y, direction.Z,
            idealDistance, _reusableOutputArg))
        {
            spawnPoint = _reusableOutputArg.GetResult<Vector3>();
            return true;
        }
        spawnPoint = Vector3.Zero;
        return false;
    }

    public static unsafe void GetSpawnCoordsForVehicleNode(int nodeAddress, Vector3 targetDirection, out Vector3 spawnPosition, out float heading)
    {
        Function.Call(Hash.GET_SPAWN_COORDS_FOR_VEHICLE_NODE, nodeAddress,
            targetDirection.X, targetDirection.Y, targetDirection.Z,
            _reusableOutputArg, _reusableOutputArg2);
        spawnPosition = _reusableOutputArg.GetResult<Vector3>();
        heading = _reusableOutputArg2.GetResult<float>();
    }

    public static unsafe bool GetRandomVehicleNode(Vector3 center, float radius, int minLanes, bool avoidDeadEnds, bool avoidHighways, out Vector3 nodePosition, out int nodeAddress)
    {
        nodeAddress = 0;
        nodePosition = Vector3.Zero;

        if (Function.Call<bool>(Hash.GET_RANDOM_VEHICLE_NODE,
            center.X, center.Y, center.Z, radius, minLanes, avoidDeadEnds, avoidHighways,
            _reusableOutputArg, _reusableOutputArg2))
        {
            nodePosition = _reusableOutputArg.GetResult<Vector3>();
            nodeAddress = _reusableOutputArg2.GetResult<int>();
            return true;
        }
        return false;
    }
    #endregion

    #region Search Point Methods with Consistent Random Usage
    public static Vector3 FindSearchPointForAutomobile(Vector3 startPosition, float maxRadius, bool useLastSeenPosition = false)
    {
        Vector3 position = RandomPointInsideCircle(startPosition, maxRadius);
        if (useLastSeenPosition)
        {
            Vector3 lastKnown = ImportantChecks.LastKnownLocation.Around(20);
            if (lastKnown.DistanceTo2D(startPosition) < maxRadius)
            {
                position = lastKnown;
            }
            else
            {
                Vector3 playerPos = Game.Player.Character.Position;
                float distance = Math.Max(maxRadius / 2f, 120f);
                position = GetPointBetweenTwoVectors(playerPos, lastKnown, distance / playerPos.DistanceTo2D(lastKnown));
            }
        }
        return World.GetNextPositionOnStreet(position);
    }

    public static Vector3 FindSearchPointForBoat(Vector3 startPosition, float maxRadius, bool useLastSeenPosition = false)
    {
        Vector3 result = RandomPointInsideCircle(startPosition, maxRadius);
        if (useLastSeenPosition)
        {
            Vector3 lastKnown = ImportantChecks.LastKnownLocation.Around(50);
            if (lastKnown.DistanceTo2D(startPosition) < maxRadius)
            {
                result = lastKnown;
            }
            else
            {
                Vector3 playerPos = Game.Player.Character.Position;
                float distance = Math.Max(maxRadius / 2f, 120f);
                result = GetPointBetweenTwoVectors(playerPos, lastKnown, distance / playerPos.DistanceTo2D(lastKnown));
            }
        }
        result.Z = startPosition.Z;
        if (GetWaterLevelNoWaves(new Vector3(result.X, result.Y, 200f), out var waterHeight))
        {
            result.Z = waterHeight;
        }
        return result;
    }

    public static Vector3 FindSearchPointForHelicopter(Vector3 startPosition, float maxRadius, float height, bool useLastSeenPosition = false)
    {
        Vector3 result = RandomPointInsideCircle(startPosition, maxRadius);
        if (useLastSeenPosition)
        {
            Vector3 lastKnown = ImportantChecks.LastKnownLocation.Around(60);
            if (lastKnown.DistanceTo2D(startPosition) < maxRadius)
            {
                result = lastKnown;
            }
            else
            {
                Vector3 playerPos = Game.Player.Character.Position;
                float distance = Math.Max(maxRadius / 2f, 120f);
                result = GetPointBetweenTwoVectors(playerPos, lastKnown, distance / playerPos.DistanceTo2D(lastKnown));
            }
        }
        result.Z += height;
        return result;
    }

    public static Vector3 FindSearchPointForPlane(Vector3 startPosition, float maxRadius, float height, bool useLastSeenPosition = false)
    {
        Vector3 result = FindSearchPointForHelicopter(startPosition, maxRadius, height, useLastSeenPosition);
        Vector3 testPoint = new Vector3(result.X, result.Y, 1000f);
        if (!World.GetGroundHeight(testPoint, out var groundHeight, GetGroundHeightMode.ConsiderWaterAsGroundNoWaves))
        {
            groundHeight = World.GetApproxHeightForPoint(testPoint);
        }
        result.Z = Math.Max(groundHeight + height, result.Z);
        return result;
    }

    public static Vector3 FindSearchPointForSubmarine(Vector3 startPosition, float maxRadius, bool useLastSeenPosition = false)
    {
        Vector3 result = RandomPointInsideCircle(startPosition, maxRadius);
        if (useLastSeenPosition)
        {
            Vector3 lastKnown = ImportantChecks.LastKnownLocation.Around(50f) + Vector3.WorldDown * 20;
            if (lastKnown.DistanceTo2D(startPosition) < maxRadius)
            {
                result = lastKnown;
            }
            else
            {
                Vector3 playerPos = Game.Player.Character.Position;
                float distance = Math.Max(maxRadius / 2f, 120f);
                result = GetPointBetweenTwoVectors(playerPos, lastKnown, distance / playerPos.DistanceTo2D(lastKnown));
            }
        }
        Vector3 testPoint = new Vector3(result.X, result.Y, 200f);
        if (!GetWaterLevelNoWaves(testPoint, out var waterHeight))
        {
            return Vector3.Zero;
        }
        result.Z = Math.Min(result.Z, waterHeight - 10f);
        if (World.GetGroundHeight(testPoint, out var groundHeight))
        {
            result.Z = Math.Max(result.Z, groundHeight + 15f);
        }
        return result;
    }
    #endregion

    #region Spawn Point Finding Methods with Optimized Performance
    public static bool FindNearbyWaterArea(Vector3 fromPos, float minDistance, float maxDistance)
    {
        Vector3[] directions = new Vector3[]
        {
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, -1, 0),
            new Vector3(1, 1, 0).Normalized,
            new Vector3(-1, 1, 0).Normalized,
            new Vector3(1, -1, 0).Normalized,
            new Vector3(-1, -1, 0).Normalized,
        };

        for (float dist = minDistance; dist <= maxDistance; dist += 10f)
        {
            foreach (var dir in directions)
            {
                Vector3 checkPos = fromPos + dir * dist;
                float waterHeight = GetWaterHeight(checkPos);

                if (waterHeight > 0f && (waterHeight - checkPos.Z) >= -1f)
                    return true;
            }
        }

        return false;
    }

    public static bool FindSpawnPointForAutomobile(Ped target, Vector3 startPosition, float minDistance, float maxDistance, out Vector3 spawnPoint, out float spawnHeading, int maxTries = 5)
    {
        float speed = target.Speed;
        Vector3 targetPos = target.Position;

        if (speed >= 14f)
        {
            return FindSpawnPointForFastMovingTarget(target, startPosition, minDistance, maxDistance, out spawnPoint, out spawnHeading, maxTries);
        }
        else
        {
            return FindSpawnPointForSlowMovingTarget(target, startPosition, minDistance, maxDistance, out spawnPoint, out spawnHeading, maxTries);
        }
    }

    private static bool FindSpawnPointForFastMovingTarget(Ped target, Vector3 startPosition, float minDistance, float maxDistance, out Vector3 spawnPoint, out float spawnHeading, int maxTries)
    {
        Vector2 velocity = target.Velocity;
        spawnHeading = 0f;
        spawnPoint = Vector3.Zero;

        for (int i = 0; i < maxTries; i++)
        {
            if (!FindSpawnPointInDirection(startPosition, new Vector3(velocity.X, velocity.Y, 0f), GetRandomFloat(minDistance, maxDistance), out var candidateSpawn))
            {
                continue;
            }

            if (IsSpawnPointValid(candidateSpawn))
            {
                spawnPoint = candidateSpawn;
                break;
            }
        }

        if (spawnPoint == Vector3.Zero)
            return false;

        spawnHeading = GetAngleBetweenTwoPoints(spawnPoint, target.Position);
        OptimizeSpawnPointWithVehicleNode(ref spawnPoint, ref spawnHeading, target.Position);

        return true;
    }

    private static bool FindSpawnPointForSlowMovingTarget(Ped target, Vector3 startPosition, float minDistance, float maxDistance, out Vector3 spawnPoint, out float spawnHeading, int maxTries)
    {
        spawnHeading = 0f;
        spawnPoint = Vector3.Zero;

        int attempts = 0;
        bool allowSwitchedOff = false;
        PathNode pathNode = null;

        while (attempts < maxTries)
        {
            Vector3 searchPos = startPosition.Around(GetRandomFloat(minDistance, maxDistance));
            pathNode = PathFind.GetClosestVehicleNode(searchPos, maxDistance,
                (flags) => allowSwitchedOff || (!flags.HasFlag(VehiclePathNodePropertyFlags.SwitchedOff) &&
                                              !flags.HasFlag(VehiclePathNodePropertyFlags.Boat) &&
                                              !flags.HasFlag(VehiclePathNodePropertyFlags.LeadsToDeadEnd)));

            if (pathNode != null)
            {
                Vector3 nodePos = pathNode.Position;
                if (IsSpawnPointValid(nodePos))
                {
                    spawnPoint = nodePos;
                    break;
                }
            }

            attempts++;
            allowSwitchedOff = attempts > maxTries / 2;
        }

        if (spawnPoint == Vector3.Zero)
            return false;

        spawnHeading = GetAngleBetweenTwoPoints(spawnPoint, target.Position);
        if (pathNode != null)
        {
            OptimizeSpawnPointWithVehicleNode(pathNode.Handle, ref spawnPoint, ref spawnHeading, target.Position);
        }

        return true;
    }

    private static bool IsSpawnPointValid(Vector3 position)
    {
        ShapeTestHandle shapeTest = ShapeTest.StartTestCapsule(position, position, 5f, IntersectFlags.Vehicles);
        ShapeTestResult result;

        while (shapeTest.GetResult(out result) == ShapeTestStatus.NonExistent)
        {
            Script.Yield();
        }

        return !result.DidHit && !IsSphereVisibleToPlayer(position, 5f);
    }

    private static void OptimizeSpawnPointWithVehicleNode(ref Vector3 spawnPoint, ref float spawnHeading, Vector3 targetPosition)
    {
        if (GetRandomVehicleNode(spawnPoint, 5f, 0, true, false, out _, out var nodeAddress))
        {
            GetSpawnCoordsForVehicleNode(nodeAddress, targetPosition, out var optimizedPos, out var optimizedHeading);
            if (optimizedPos != Vector3.Zero)
            {
                spawnPoint = optimizedPos;
                spawnHeading = optimizedHeading;
            }
        }
    }

    private static void OptimizeSpawnPointWithVehicleNode(int nodeHandle, ref Vector3 spawnPoint, ref float spawnHeading, Vector3 targetPosition)
    {
        GetSpawnCoordsForVehicleNode(nodeHandle, targetPosition, out var optimizedPos, out var optimizedHeading);
        if (optimizedPos != Vector3.Zero)
        {
            spawnPoint = optimizedPos;
            spawnHeading = optimizedHeading;
        }
    }

    public static bool FindSpawnPointForAircraft(Ped target, Vector3 startPosition, float minDistance, float maxDistance, float height, out Vector3 spawnPoint, out float spawnHeading, int maxTries = 3)
    {
        spawnPoint = Vector3.Zero;

        for (int i = 0; i < maxTries; i++)
        {
            Vector3 candidatePos = startPosition.Around(GetRandomFloat(minDistance, maxDistance));
            float targetHeight = candidatePos.Z + height;

            if (World.GetGroundHeight(new Vector3(candidatePos.X, candidatePos.Y, 1000f), out var groundHeight, GetGroundHeightMode.ConsiderWaterAsGroundNoWaves))
            {
                candidatePos.Z = Math.Max(groundHeight + Math.Max(height / 2f, 20f), targetHeight);
            }
            else
            {
                float approxHeight = World.GetApproxHeightForPoint(candidatePos);
                candidatePos.Z = Math.Max(approxHeight + Math.Max(height / 2f, 20f), targetHeight);
            }

            spawnPoint = candidatePos;
            if (!IsSphereVisibleToPlayer(candidatePos, 5f))
            {
                break;
            }
        }

        spawnHeading = GetAngleBetweenTwoPoints(spawnPoint, target.Position);
        return spawnPoint != Vector3.Zero;
    }

    public static bool FindSpawnPointForBoat(Ped target, Vector3 startPosition, float minDistance, float maxDistance, out Vector3 spawnPoint, out float spawnHeading, int maxTries = 30)
    {
        spawnPoint = Vector3.Zero;

        for (int i = 0; i < maxTries; i++)
        {
            Vector3 candidatePos = startPosition.Around(GetRandomFloat(minDistance, maxDistance));
            if (GetWaterLevelNoWaves(new Vector3(candidatePos.X, candidatePos.Y, 200f), out var waterHeight))
            {
                if (World.GetGroundHeight(new Vector3(candidatePos.X, candidatePos.Y, 1000f), out var groundHeight))
                {
                    // Ensure sufficient water depth
                    if (waterHeight - groundHeight >= 1f)
                    {
                        candidatePos.Z = waterHeight;
                        spawnPoint = candidatePos;
                        break;
                    }
                }
            }
        }

        spawnHeading = GetAngleBetweenTwoPoints(spawnPoint, target.Position);
        return spawnPoint != Vector3.Zero;
    }

    public static bool FindSpawnPointForSubmarine(Ped target, Vector3 startPosition, float minDistance, float maxDistance, out Vector3 spawnPoint, out float spawnHeading, int maxTries = 5)
    {
        spawnPoint = Vector3.Zero;

        for (int i = 0; i < maxTries; i++)
        {
            Vector3 candidatePos = startPosition.Around(GetRandomFloat(minDistance, maxDistance));
            if (GetWaterLevelNoWaves(new Vector3(candidatePos.X, candidatePos.Y, 200f), out var waterHeight))
            {
                if (World.GetGroundHeight(new Vector3(candidatePos.X, candidatePos.Y, 1000f), out var groundHeight))
                {
                    // Need deeper water for submarines
                    if (waterHeight - groundHeight >= 10f)
                    {
                        candidatePos.Z = Math.Max(groundHeight + 4f, Math.Min(candidatePos.Z, waterHeight - 4f));
                        spawnPoint = candidatePos;
                        break;
                    }
                }
            }
        }

        spawnHeading = GetAngleBetweenTwoPoints(spawnPoint, target.Position);
        return spawnPoint != Vector3.Zero;
    }
    #endregion

    #region DualWeapons Class
    public class DualWeapons
    {
        public List<WeaponData> Primary { get; }
        public List<WeaponData> Secondary { get; }

        public DualWeapons(List<WeaponData> primary, List<WeaponData> secondary)
        {
            Primary = primary ?? new List<WeaponData>();
            Secondary = secondary ?? new List<WeaponData>();
        }
    }
    #endregion
}
