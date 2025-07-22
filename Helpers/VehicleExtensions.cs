using GTA.Native;
using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA.Math;

public static class VehicleExtensions
    {
        public static void InstallModKit(this Vehicle cars)
        {
            Function.Call(Hash.SET_VEHICLE_MOD_KIT, cars, 0);
        }

        public static int GetModCount(this Vehicle cars, ModType type)
        {
            return Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, cars, type);
        }

        public static void SetMod(this Vehicle cars, ModType type, int index, bool variations)
        {
            Function.Call(Hash.SET_VEHICLE_MOD, cars, (int)type, index, variations);
        }

        public static void SetWindowTint(this Vehicle cars, VehicleWindowTint value)
        {
            Function.Call(Hash.SET_VEHICLE_WINDOW_TINT, cars, (int)value);
        }
        public static void SetLivery(this Vehicle cars, int liverynum, int index)
        {
            if (liverynum == 0) Function.Call(Hash.SET_VEHICLE_LIVERY, cars, index);
            else if (liverynum == 1) Function.Call(Hash.SET_VEHICLE_LIVERY2, cars, index);
        }

        public static int GetLivery(this Vehicle cars, int liverynum)
        {
            if (liverynum == 0) return Function.Call<int>(Hash.GET_VEHICLE_LIVERY, cars);
            else if (liverynum == 1) return Function.Call<int>(Hash.GET_VEHICLE_LIVERY2, cars);
            return 0;
        }
        public static bool ControlMountedWeapon(this Ped ped)
        {
            return Function.Call<bool>(Hash.CONTROL_MOUNTED_WEAPON, ped);
        }

        public static bool SetVehicleWeaponHash(this Ped ped, VehicleWeaponHash WeapHash)
        {
            return Function.Call<bool>(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, ped, WeapHash);
        }

        public static void DisableVehicleWeapon(this Ped ped, VehicleWeaponHash weaponHash, Vehicle veh)
        {
            Function.Call(Hash.DISABLE_VEHICLE_WEAPON, true, weaponHash, veh, ped);
        }

        public static void EnableVehicleWeapon(this Ped ped, VehicleWeaponHash weaponHash, Vehicle veh)
        {
            Function.Call(Hash.DISABLE_VEHICLE_WEAPON, false, weaponHash, veh, ped); //Note: despite the name, second argument false means enable weapon.
        }

        public static void AssignWeapons(Ped ped, List<string> primaryWeapons, List<string> secondaryWeapons)
        {
            var random = new Random();

            // Assign a random primary weapon if available
            if (primaryWeapons != null && primaryWeapons.Count > 0)
            {
                var primaryWeaponName = primaryWeapons[random.Next(primaryWeapons.Count)];
                ped.Weapons.Give(primaryWeaponName, 900, true, true);
            }

            // Assign a random secondary weapon if available
            if (secondaryWeapons != null && secondaryWeapons.Count > 0)
            {
                var secondaryWeaponName = secondaryWeapons[random.Next(secondaryWeapons.Count)];
                ped.Weapons.Give(secondaryWeaponName, 200, false, true);
            }
        }

    public static void EnableHeliWeapons(this Vehicle helicopter)
    {
        // we give and then allow to use them.
        helicopter.Driver.SetCombatAttribute(CombatAttributes.PreferAirCombatWhenInAircraft, true); //this flag is set when player is in aircraft
        helicopter.Driver.SetCombatAttribute(CombatAttributes.UseVehicleAttackIfVehicleHasMountedGuns, true);
        helicopter.Driver.SetCombatAttribute(CombatAttributes.ForceCheckAttackAngleForMountedGuns, true);
        helicopter.Driver.SetCombatAttribute(CombatAttributes.UseRocketsAgainstVehiclesOnly, true);
        helicopter.Driver.SetCombatAttribute(CombatAttributes.UseVehicleAttack, true);
    }

    public static void DisableHeliWeapons(this Vehicle helicopter)
        {
            //remove their ability to use them
            helicopter.Driver.SetCombatAttribute(CombatAttributes.PreferAirCombatWhenInAircraft, false); //this flag is set when player is in aircraft
            helicopter.Driver.SetCombatAttribute(CombatAttributes.UseVehicleAttackIfVehicleHasMountedGuns, false);
            helicopter.Driver.SetCombatAttribute(CombatAttributes.UseRocketsAgainstVehiclesOnly, false);
            helicopter.Driver.SetCombatAttribute(CombatAttributes.UseVehicleAttack, false);
        }

        public static bool GetVehicleWeaponHash(this Ped ped, out VehicleWeaponHash WeapHash)
        {
            OutputArgument arg = new OutputArgument();
            bool nat = Function.Call<bool>(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, ped, arg);
            WeapHash = arg.GetResult<VehicleWeaponHash>();
            return nat;
        }

        public static void EnableAttack(this Vehicle helicopter)
        {
            EnableHeliWeapons(helicopter);
        }

        public static void DisableAttack(this Vehicle helicopter)
        {
            DisableHeliWeapons(helicopter);
        }

        public static bool IsPedRappelingFromHelicopter(this Vehicle helicopter)
        {
            return Function.Call<bool>(Hash.IS_ANY_PED_RAPPELLING_FROM_HELI, helicopter);  //is used to check if rappel is happening by ped or not.
        }

        #region ModKit Natives as Extensions

        /// <summary>
        /// Checks if vehicle has searchlight.
        /// </summary>
        public static bool DoesVehicleHaveSearchlight(this Vehicle vehicle)
        {
            return Function.Call<bool>(Hash.DOES_VEHICLE_HAVE_SEARCHLIGHT, vehicle);
        }

        /// <summary>
        /// Returns number of mod kits available.
        /// </summary>
        public static int GetNumModKits(this Vehicle vehicle)
        {
            return Function.Call<int>(Hash.GET_NUM_MOD_KITS, vehicle);
        }

        /// <summary>
        /// Activates a mod kit.
        /// </summary>
        public static void SetVehicleModKit(this Vehicle vehicle, int kitIndex)
        {
            Function.Call(Hash.SET_VEHICLE_MOD_KIT, vehicle, kitIndex);
        }

        /// <summary>
        /// Removes active mod kit.
        /// </summary>
        public static void RemoveVehicleModKit(this Vehicle vehicle)
        {
            Function.Call((Hash)0xee0098dcb7b9c9ef, vehicle);
        }

        /// <summary>
        /// Returns current mod kit index.
        /// </summary>
        public static int GetVehicleModKit(this Vehicle vehicle)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_MOD_KIT, vehicle);
        }

        /// <summary>
        /// Returns current mod kit type.
        /// </summary>
        public static ModKitType GetVehicleModKitType(this Vehicle vehicle)
        {
            return (ModKitType)Function.Call<int>(Hash.GET_VEHICLE_MOD_KIT_TYPE, vehicle);
        }

        #endregion ModKit Natives as Extensions

        #region ModWheelType Natives as Extensions

        /// <summary>
        /// Returns wheel pack type.
        /// </summary>
        public static ModWheelType GetVehicleWheelType(this Vehicle vehicle)
        {
            return (ModWheelType)Function.Call<int>(Hash.GET_VEHICLE_WHEEL_TYPE, vehicle);
        }

        /// <summary>
        /// Sets wheel type.
        /// </summary>
        public static void SetVehicleWheelType(this Vehicle vehicle, ModWheelType type)
        {
            Function.Call(Hash.SET_VEHICLE_WHEEL_TYPE, vehicle, (int)type);
        }

        #endregion ModWheelType Natives as Extensions

        #region ModColor Natives as Extensions

        /// <summary>
        /// Returns number of colors for type.
        /// </summary>
        public static int GetNumModColors(VehicleColor colorType, bool isBaseColor = true)
        {
            return Function.Call<int>(Hash.GET_NUM_MOD_COLORS, (int)colorType, isBaseColor);
        }

        /// <summary>
        /// Sets mod color 1.
        /// </summary>
        public static void SetVehicleModColor1(this Vehicle vehicle, VehicleColor colorType, int baseColIndex, int specColIndex = 0)
        {
            Function.Call(Hash.SET_VEHICLE_MOD_COLOR_1, vehicle, (int)colorType, baseColIndex, specColIndex);
        }

        /// <summary>
        /// Sets mod color 2.
        /// </summary>
        public static void SetVehicleModColor2(this Vehicle vehicle, VehicleColor colorType, int baseColIndex)
        {
            Function.Call(Hash.SET_VEHICLE_MOD_COLOR_2, vehicle, (int)colorType, baseColIndex);
        }

        /// <summary>
        /// Gets mod color 1 info.
        /// </summary>
        public static void GetVehicleModColor1(this Vehicle vehicle, out VehicleColor colorType, out int baseColIndex, out int specColIndex)
        {
            OutputArgument colorTypeArg = new OutputArgument();
            OutputArgument baseColIndexArg = new OutputArgument();
            OutputArgument specColIndexArg = new OutputArgument();

            Function.Call(Hash.GET_VEHICLE_MOD_COLOR_1, vehicle, colorTypeArg, baseColIndexArg, specColIndexArg);

            colorType = (VehicleColor)colorTypeArg.GetResult<int>();
            baseColIndex = baseColIndexArg.GetResult<int>();
            specColIndex = specColIndexArg.GetResult<int>();
        }

        /// <summary>
        /// Gets mod color 2 info.
        /// </summary>
        public static void GetVehicleModColor2(this Vehicle vehicle, out VehicleColor colorType, out int baseColIndex)
        {
            OutputArgument colorTypeArg = new OutputArgument();
            OutputArgument baseColIndexArg = new OutputArgument();

            Function.Call(Hash.GET_VEHICLE_MOD_COLOR_2, vehicle, colorTypeArg, baseColIndexArg);

            colorType = (VehicleColor)colorTypeArg.GetResult<int>();
            baseColIndex = baseColIndexArg.GetResult<int>();
        }

        /// <summary>
        /// Returns mod color 1 name.
        /// </summary>
        public static string GetVehicleModColor1Name(this Vehicle vehicle, bool specular)
        {
            return Function.Call<string>(Hash.GET_VEHICLE_MOD_COLOR_1_NAME, vehicle, specular);
        }

        /// <summary>
        /// Returns mod color 2 name.
        /// </summary>
        public static string GetVehicleModColor2Name(this Vehicle vehicle)
        {
            return Function.Call<string>(Hash.GET_VEHICLE_MOD_COLOR_2_NAME, vehicle);
        }

        #endregion ModColor Natives as Extensions

        #region Vehicle Mod Natives as Extensions

        /// <summary>
        /// Sets a vehicle mod.
        /// </summary>
        public static void SetVehicleMod(this Vehicle vehicle, ModType modSlot, int modIndex, bool variation = false)
        {
            Function.Call(Hash.SET_VEHICLE_MOD, vehicle, modSlot, modIndex, variation);
        }

        /// <summary>
        /// Gets current mod index in slot.
        /// </summary>
        public static int GetVehicleMod(this Vehicle vehicle, ModType modSlot)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_MOD, vehicle, modSlot);
        }

        /// <summary>
        /// Gets current mod variation in slot.
        /// </summary>
        public static int GetVehicleModVariation(this Vehicle vehicle, ModType modSlot)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_MOD_VARIATION, vehicle, modSlot);
        }

        /// <summary>
        /// Gets number of mods in slot.
        /// </summary>
        public static int GetNumVehicleMods(this Vehicle vehicle, ModType modSlot)
        {
            return Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, vehicle, modSlot);
        }

        /// <summary>
        /// Removes mod from slot.
        /// </summary>
        public static void RemoveVehicleMod(this Vehicle vehicle, ModType modSlot)
        {
            Function.Call(Hash.REMOVE_VEHICLE_MOD, vehicle, modSlot);
        }

        /// <summary>
        /// Toggles a toggle mod.
        /// </summary>
        public static void ToggleVehicleMod(this Vehicle vehicle, ModType modSlot, bool toggleOn)
        {
            Function.Call(Hash.TOGGLE_VEHICLE_MOD, vehicle, modSlot, toggleOn);
        }

        /// <summary>
        /// Checks if toggle mod is on.
        /// </summary>
        public static bool IsToggleModOn(this Vehicle vehicle, ModType modSlot)
        {
            return Function.Call<bool>(Hash.IS_TOGGLE_MOD_ON, vehicle, modSlot);
        }

        /// <summary>
        /// Returns mod text label.
        /// </summary>
        public static string GetModTextLabel(this Vehicle vehicle, ModType modSlot, int modIndex)
        {
            return Function.Call<string>(Hash.GET_MOD_TEXT_LABEL, vehicle, modSlot, modIndex);
        }

        /// <summary>
        /// Returns mod slot name.
        /// </summary>
        public static string GetModSlotName(this Vehicle vehicle, ModType modSlot)
        {
            return Function.Call<string>(Hash.GET_MOD_SLOT_NAME, vehicle, modSlot);
        }

        /// <summary>
        /// Returns livery name.
        /// </summary>
        public static string GetLiveryName(this Vehicle vehicle, int livery)
        {
            return Function.Call<string>(Hash.GET_LIVERY_NAME, vehicle, livery);
        }

        /// <summary>
        /// Returns livery2 name.
        /// </summary>
        public static string GetLivery2Name(this Vehicle vehicle, int livery2)
        {
            return null;
            //return Function.Call<string>(Hash.GET_LIVERY2_NAME, vehicle, livery2);
        }

        /// <summary>
        /// Returns mod modifier value.
        /// </summary>
        public static int GetVehicleModModifierValue(this Vehicle vehicle, ModType modSlot, int modIndex)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_MOD_MODIFIER_VALUE, vehicle, modSlot, modIndex);
        }

        /// <summary>
        /// Returns mod identifier hash.
        /// </summary>
        public static int GetVehicleModIdentifierHash(this Vehicle vehicle, ModType modSlot, int modIndex)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_MOD_IDENTIFIER_HASH, vehicle, modSlot, modIndex);
        }

        /// <summary>
        /// Returns camera position for mod.
        /// </summary>
        public static ModCameraPos GetCameraPositionForMod(this Vehicle vehicle, ModType modSlot)
        {
            return 0;
            //return (ModCameraPos)Function.Call<int>(Hash.GET_CAMERA_POSITION_FOR_MOD, vehicle, modSlot);
        }

        /// <summary>
        /// Preloads vehicle mod.
        /// </summary>
        public static void PreloadVehicleMod(this Vehicle vehicle, ModType modSlot, int modIndex)
        {
            Function.Call(Hash.PRELOAD_VEHICLE_MOD, vehicle, modSlot, modIndex);
        }

        /// <summary>
        /// Checks if preload mods finished.
        /// </summary>
        public static bool HasPreloadModsFinished(this Vehicle vehicle)
        {
            return Function.Call<bool>(Hash.HAS_PRELOAD_MODS_FINISHED, vehicle);
        }

        /// <summary>
        /// Releases preloaded mods.
        /// </summary>
        public static void ReleasePreloadMods(this Vehicle vehicle)
        {
            Function.Call(Hash.RELEASE_PRELOAD_MODS, vehicle);
        }

        /// <summary>
        /// Checks if vehicle can be modded.
        /// </summary>
        public static bool CanVehicleBeModded(this Vehicle vehicle)
        {
            return false;
            //return Function.Call<bool>(Hash.CAN_VEHICLE_BE_MODDED, vehicle);
        }

        /// <summary>
        /// Sets tire smoke color.
        /// </summary>
        public static void SetVehicleTyreSmokeColor(this Vehicle vehicle, int red, int green, int blue)
        {
            Function.Call(Hash.SET_VEHICLE_TYRE_SMOKE_COLOR, vehicle, red, green, blue);
        }

        /// <summary>
        /// Gets tire smoke color.
        /// </summary>
        public static void GetVehicleTyreSmokeColor(this Vehicle vehicle, out int red, out int green, out int blue)
        {
            OutputArgument redArg = new OutputArgument();
            OutputArgument greenArg = new OutputArgument();
            OutputArgument blueArg = new OutputArgument();

            Function.Call(Hash.GET_VEHICLE_TYRE_SMOKE_COLOR, vehicle, redArg, greenArg, blueArg);

            red = redArg.GetResult<int>();
            green = greenArg.GetResult<int>();
            blue = blueArg.GetResult<int>();
        }

        /// <summary>
        /// Sets window tint color index.
        /// </summary>
        public static void SetVehicleWindowTintIndex(this Vehicle vehicle, int colorIndex)
        {
            Function.Call(Hash.SET_VEHICLE_WINDOW_TINT, vehicle, colorIndex);
        }

        /// <summary>
        /// Gets window tint color index.
        /// </summary>
        public static int GetVehicleWindowTintIndex(this Vehicle vehicle)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_WINDOW_TINT, vehicle);
        }

        /// <summary>
        /// Gets number of window tints.
        /// </summary>
        public static int GetNumVehicleWindowTints()
        {
            return Function.Call<int>(Hash.GET_NUM_VEHICLE_WINDOW_TINTS);
        }

        /// <summary>
        /// Gets vehicle color in RGB.
        /// </summary>
        public static void GetVehicleColor(this Vehicle vehicle, out int Red, out int Green, out int Blue)
        {
            OutputArgument RedArg = new OutputArgument();
            OutputArgument GreenArg = new OutputArgument();
            OutputArgument BlueArg = new OutputArgument();
            Function.Call(Hash.GET_VEHICLE_COLOR, vehicle, RedArg, GreenArg, BlueArg);
            Red = RedArg.GetResult<int>();
            Green = GreenArg.GetResult<int>();
            Blue = BlueArg.GetResult<int>();
        }

        /// <summary>
        /// Gets bitset of settable colors.
        /// </summary>
        public static int GetVehicleColoursWhichCanBeSet(this Vehicle vehicle)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_COLOURS_WHICH_CAN_BE_SET, vehicle);
        }

        #endregion Vehicle Mod Natives as Extensions


        #region New Vehicle Mod Natives as Extensions

        /// <summary>
        /// Returns current mod index in slot, -1 if none.
        /// </summary>
        public static int GetVehicleModIndex(this Vehicle vehicle, ModType modSlot)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_MOD, vehicle, modSlot);
        }

        /// <summary>
        /// Returns current mod variation in slot, 0 if none.
        /// </summary>
        public static int GetVehicleModVariationIndex(this Vehicle vehicle, ModType modSlot)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_MOD_VARIATION, vehicle, modSlot);
        }

        /// <summary>
        /// Removes mod from slot.
        /// </summary>
        public static void RemoveVehicleComponentMod(this Vehicle vehicle, ModType modSlot)
        {
            Function.Call(Hash.REMOVE_VEHICLE_MOD, vehicle, modSlot);
        }

        /// <summary>
        /// Toggles a toggle mod on/off.
        /// </summary>
        public static void ToggleVehicleComponentMod(this Vehicle vehicle, ModType modSlot, bool toggleOn)
        {
            Function.Call(Hash.TOGGLE_VEHICLE_MOD, vehicle, modSlot, toggleOn);
        }

        /// <summary>
        /// Checks if toggle mod is on/off.
        /// </summary>
        public static bool IsVehicleToggleModOn(this Vehicle vehicle, ModType modSlot)
        {
            return Function.Call<bool>(Hash.IS_TOGGLE_MOD_ON, vehicle, modSlot);
        }

        /// <summary>
        /// Returns mod name text label.
        /// </summary>
        public static string GetVehicleModComponentName(this Vehicle vehicle, ModType modSlot, int modIndex)
        {
            return Function.Call<string>(Hash.GET_MOD_TEXT_LABEL, vehicle, modSlot, modIndex);
        }

        /// <summary>
        /// Returns mod slot name for UI.
        /// </summary>
        public static string GetVehicleModSlotDisplayName(this Vehicle vehicle, ModType modSlot)
        {
            return Function.Call<string>(Hash.GET_MOD_SLOT_NAME, vehicle, modSlot);
        }

        /// <summary>
        /// Returns livery name.
        /// </summary>
        public static string GetVehicleLiveryDisplayName(this Vehicle vehicle, int livery)
        {
            return Function.Call<string>(Hash.GET_LIVERY_NAME, vehicle, livery);
        }

        /// <summary>
        /// Returns livery2 name.
        /// </summary>
        public static string GetVehicleLivery2DisplayName(this Vehicle vehicle, int livery2)
        {
            return null;
            //return Function.Call<string>(Hash.GET_LIVERY2_NAME, vehicle, livery2);
        }

        /// <summary>
        /// Returns modifier value for mod.
        /// </summary>
        public static int GetVehicleModComponentModifierValue(this Vehicle vehicle, ModType modSlot, int modIndex)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_MOD_MODIFIER_VALUE, vehicle, modSlot, modIndex);
        }

        /// <summary>
        /// Returns identifier hash for mod.
        /// </summary>
        public static int GetVehicleModComponentIdentifierHash(this Vehicle vehicle, ModType modSlot, int modIndex)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_MOD_IDENTIFIER_HASH, vehicle, modSlot, modIndex);
        }

        /// <summary>
        /// Returns camera position for mod in slot.
        /// </summary>
        public static ModCameraPos GetVehicleModCameraPosition(this Vehicle vehicle, ModType modSlot)
        {
            return 0;
            //return (ModCameraPos)Function.Call<int>(Hash.GET_CAMERA_POSITION_FOR_MOD, vehicle, modSlot);
        }

        /// <summary>
        /// Preloads vehicle mod (no visual).
        /// </summary>
        public static void PreloadVehicleComponentMod(this Vehicle vehicle, ModType modSlot, int modIndex)
        {
            Function.Call(Hash.PRELOAD_VEHICLE_MOD, vehicle, modSlot, modIndex);
        }

        /// <summary>
        /// Checks if preloaded mods finished.
        /// </summary>
        public static bool HasVehiclePreloadModsFinished(this Vehicle vehicle)
        {
            return Function.Call<bool>(Hash.HAS_PRELOAD_MODS_FINISHED, vehicle);
        }

        /// <summary>
        /// Releases preloaded mods.
        /// </summary>
        public static void ReleaseVehiclePreloadMods(this Vehicle vehicle)
        {
            Function.Call(Hash.RELEASE_PRELOAD_MODS, vehicle);
        }

        /// <summary>
        /// Checks if vehicle can be modded.
        /// </summary>
        public static bool CanVehicleBeComponentModded(this Vehicle vehicle)
        {
            return Function.Call<bool>((Hash)0xe0cc93e11fa599c5, vehicle);
        }

        /// <summary>
        /// Sets tire smoke color.
        /// </summary>
        public static void SetVehicleComponentTyreSmokeColor(this Vehicle vehicle, int red, int green, int blue)
        {
            Function.Call(Hash.SET_VEHICLE_TYRE_SMOKE_COLOR, vehicle, red, green, blue);
        }

        /// <summary>
        /// Gets tire smoke color.
        /// </summary>
        public static void GetVehicleComponentTyreSmokeColor(this Vehicle vehicle, out int red, out int green, out int blue)
        {
            OutputArgument redArg = new OutputArgument();
            OutputArgument greenArg = new OutputArgument();
            OutputArgument blueArg = new OutputArgument();

            Function.Call(Hash.GET_VEHICLE_TYRE_SMOKE_COLOR, vehicle, redArg, greenArg, blueArg);

            red = redArg.GetResult<int>();
            green = greenArg.GetResult<int>();
            blue = blueArg.GetResult<int>();
        }

        /// <summary>
        /// Sets window tint color index.
        /// </summary>
        public static void SetVehicleComponentWindowTint(this Vehicle vehicle, int colorIndex)
        {
            Function.Call(Hash.SET_VEHICLE_WINDOW_TINT, vehicle, colorIndex);
        }

        /// <summary>
        /// Gets window tint color index.
        /// </summary>
        public static int GetVehicleComponentWindowTint(this Vehicle vehicle)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_WINDOW_TINT, vehicle);
        }

        /// <summary>
        /// Gets number of window tints.
        /// </summary>
        public static int GetVehicleComponentNumWindowTints()
        {
            return Function.Call<int>(Hash.GET_NUM_VEHICLE_WINDOW_TINTS);
        }

        /// <summary>
        /// Gets vehicle color in RGB.
        /// </summary>
        public static void GetVehicleComponentColor(this Vehicle vehicle, out int Red, out int Green, out int Blue)
        {
            OutputArgument RedArg = new OutputArgument();
            OutputArgument GreenArg = new OutputArgument();
            OutputArgument BlueArg = new OutputArgument();
            Function.Call(Hash.GET_VEHICLE_COLOR, vehicle, RedArg, GreenArg, BlueArg);
            Red = RedArg.GetResult<int>();
            Green = GreenArg.GetResult<int>();
            Blue = BlueArg.GetResult<int>();
        }

        /// <summary>
        /// Gets bitset of settable colors.
        /// </summary>
        public static int GetVehicleComponentColoursWhichCanBeSet(this Vehicle vehicle)
        {
            return Function.Call<int>(Hash.GET_VEHICLE_COLOURS_WHICH_CAN_BE_SET, vehicle);
        }


        #endregion New Vehicle Mod Natives as Extensions


        /// <summary>
        /// Enumeration representing different types of vehicle modifications (PascalCase member names).
        /// </summary>
        public enum ModType
        {
            Spoiler = 0,      // MOD_SPOILER
            BumperFront,      // MOD_BUMPER_F
            BumperRear,       // MOD_BUMPER_R
            Skirt,            // MOD_SKIRT
            Exhaust,          // MOD_EXHAUST
            Chassis,          // MOD_CHASSIS
            Grill,            // MOD_GRILL
            Bonnet,           // MOD_BONNET
            WingLeft,         // MOD_WING_L
            WingRight,        // MOD_WING_R
            Roof,             // MOD_ROOF

            Engine,           // MOD_ENGINE
            Brakes,           // MOD_BRAKES
            Gearbox,          // MOD_GEARBOX
            Horn,             // MOD_HORN
            Suspension,       // MOD_SUSPENSION
            Armour,           // MOD_ARMOUR

            ToggleNitrous,    // MOD_TOGGLE_NITROUS
            ToggleTurbo,      // MOD_TOGGLE_TURBO
            ToggleSubwoofer,  // MOD_TOGGLE_SUBWOOFER
            ToggleTyreSmoke,  // MOD_TOGGLE_TYRE_SMOKE
            ToggleHydraulics, // MOD_TOGGLE_HYDRAULICS
            ToggleXenonLights, // MOD_TOGGLE_XENON_LIGHTS

            Wheels,           // MOD_WHEELS
            RearWheels,       // MOD_REAR_WHEELS

            // Lowrider Mods
            PlateHolder,      // MOD_PLTHOLDER
            PlateVanity,      // MOD_PLTVANITY

            Interior1,        // MOD_INTERIOR1
            Interior2,        // MOD_INTERIOR2
            Interior3,        // MOD_INTERIOR3
            Interior4,        // MOD_INTERIOR4
            Interior5,        // MOD_INTERIOR5
            Seats,            // MOD_SEATS
            SteeringWheel,    // MOD_STEERING (Renamed for clarity - "Steering" is a bit ambiguous)
            Knob,             // MOD_KNOB
            Plaque,           // MOD_PLAQUE
            Ice,              // MOD_ICE

            Trunk,            // MOD_TRUNK
            Hydro,            // MOD_HYDRO

            EngineBay1,       // MOD_ENGINEBAY1
            EngineBay2,       // MOD_ENGINEBAY2
            EngineBay3,       // MOD_ENGINEBAY3

            Chassis2,         // MOD_CHASSIS2
            Chassis3,         // MOD_CHASSIS3
            Chassis4,         // MOD_CHASSIS4
            Chassis5,         // MOD_CHASSIS5

            DoorLeft,         // MOD_DOOR_L
            DoorRight,        // MOD_DOOR_R

            Livery            // MOD_LIVERY
        }


        public static string ToStringValue(this ModType modType)
        {
            // Use a switch statement to map PascalCase enum members back to their original string names.
            switch (modType)
            {
                case ModType.Spoiler: return "MOD_SPOILER";
                case ModType.BumperFront: return "MOD_BUMPER_F";
                case ModType.BumperRear: return "MOD_BUMPER_R";
                case ModType.Skirt: return "MOD_SKIRT";
                case ModType.Exhaust: return "MOD_EXHAUST";
                case ModType.Chassis: return "MOD_CHASSIS";
                case ModType.Grill: return "MOD_GRILL";
                case ModType.Bonnet: return "MOD_BONNET";
                case ModType.WingLeft: return "MOD_WING_L";
                case ModType.WingRight: return "MOD_WING_R";
                case ModType.Roof: return "MOD_ROOF";

                case ModType.Engine: return "MOD_ENGINE";
                case ModType.Brakes: return "MOD_BRAKES";
                case ModType.Gearbox: return "MOD_GEARBOX";
                case ModType.Horn: return "MOD_HORN";
                case ModType.Suspension: return "MOD_SUSPENSION";
                case ModType.Armour: return "MOD_ARMOUR";

                case ModType.ToggleNitrous: return "MOD_TOGGLE_NITROUS";
                case ModType.ToggleTurbo: return "MOD_TOGGLE_TURBO";
                case ModType.ToggleSubwoofer: return "MOD_TOGGLE_SUBWOOFER";
                case ModType.ToggleTyreSmoke: return "MOD_TOGGLE_TYRE_SMOKE";
                case ModType.ToggleHydraulics: return "MOD_TOGGLE_HYDRAULICS";
                case ModType.ToggleXenonLights: return "MOD_TOGGLE_XENON_LIGHTS";

                case ModType.Wheels: return "MOD_WHEELS";
                case ModType.RearWheels: return "MOD_REAR_WHEELS";

                // Lowrider Mods
                case ModType.PlateHolder: return "MOD_PLTHOLDER";
                case ModType.PlateVanity: return "MOD_PLTVANITY";

                case ModType.Interior1: return "MOD_INTERIOR1";
                case ModType.Interior2: return "MOD_INTERIOR2";
                case ModType.Interior3: return "MOD_INTERIOR3";
                case ModType.Interior4: return "MOD_INTERIOR4";
                case ModType.Interior5: return "MOD_INTERIOR5";
                case ModType.Seats: return "MOD_SEATS";
                case ModType.SteeringWheel: return "MOD_STEERING"; // Using "MOD_STEERING" for SteeringWheel
                case ModType.Knob: return "MOD_KNOB";
                case ModType.Plaque: return "MOD_PLAQUE";
                case ModType.Ice: return "MOD_ICE";

                case ModType.Trunk: return "MOD_TRUNK";
                case ModType.Hydro: return "MOD_HYDRO";

                case ModType.EngineBay1: return "MOD_ENGINEBAY1";
                case ModType.EngineBay2: return "MOD_ENGINEBAY2";
                case ModType.EngineBay3: return "MOD_ENGINEBAY3";

                case ModType.Chassis2: return "MOD_CHASSIS2";
                case ModType.Chassis3: return "MOD_CHASSIS3";
                case ModType.Chassis4: return "MOD_CHASSIS4";
                case ModType.Chassis5: return "MOD_CHASSIS5";

                case ModType.DoorLeft: return "MOD_DOOR_L";
                case ModType.DoorRight: return "MOD_DOOR_R";

                case ModType.Livery: return "MOD_LIVERY";

                default: return modType.ToString(); // Fallback to default ToString() if not explicitly handled (shouldn't happen for valid enum values)
            }
        }



        public static bool TryParseModType(string value, out ModType modType)
        {
            // We'll handle parsing manually using a switch or if-else chain for the original string names.
            string upperValue = value.ToUpperInvariant(); // Case-insensitive comparison

            if (upperValue == "MOD_SPOILER") modType = ModType.Spoiler;
            else if (upperValue == "MOD_BUMPER_F") modType = ModType.BumperFront;
            else if (upperValue == "MOD_BUMPER_R") modType = ModType.BumperRear;
            else if (upperValue == "MOD_SKIRT") modType = ModType.Skirt;
            else if (upperValue == "MOD_EXHAUST") modType = ModType.Exhaust;
            else if (upperValue == "MOD_CHASSIS") modType = ModType.Chassis;
            else if (upperValue == "MOD_GRILL") modType = ModType.Grill;
            else if (upperValue == "MOD_BONNET") modType = ModType.Bonnet;
            else if (upperValue == "MOD_WING_L") modType = ModType.WingLeft;
            else if (upperValue == "MOD_WING_R") modType = ModType.WingRight;
            else if (upperValue == "MOD_ROOF") modType = ModType.Roof;

            else if (upperValue == "MOD_ENGINE") modType = ModType.Engine;
            else if (upperValue == "MOD_BRAKES") modType = ModType.Brakes;
            else if (upperValue == "MOD_GEARBOX") modType = ModType.Gearbox;
            else if (upperValue == "MOD_HORN") modType = ModType.Horn;
            else if (upperValue == "MOD_SUSPENSION") modType = ModType.Suspension;
            else if (upperValue == "MOD_ARMOUR") modType = ModType.Armour;

            else if (upperValue == "MOD_TOGGLE_NITROUS") modType = ModType.ToggleNitrous;
            else if (upperValue == "MOD_TOGGLE_TURBO") modType = ModType.ToggleTurbo;
            else if (upperValue == "MOD_TOGGLE_SUBWOOFER") modType = ModType.ToggleSubwoofer;
            else if (upperValue == "MOD_TOGGLE_TYRE_SMOKE") modType = ModType.ToggleTyreSmoke;
            else if (upperValue == "MOD_TOGGLE_HYDRAULICS") modType = ModType.ToggleHydraulics;
            else if (upperValue == "MOD_TOGGLE_XENON_LIGHTS") modType = ModType.ToggleXenonLights;

            else if (upperValue == "MOD_WHEELS") modType = ModType.Wheels;
            else if (upperValue == "MOD_REAR_WHEELS") modType = ModType.RearWheels;

            // Lowrider Mods
            else if (upperValue == "MOD_PLTHOLDER") modType = ModType.PlateHolder;
            else if (upperValue == "MOD_PLTVANITY") modType = ModType.PlateVanity;

            else if (upperValue == "MOD_INTERIOR1") modType = ModType.Interior1;
            else if (upperValue == "MOD_INTERIOR2") modType = ModType.Interior2;
            else if (upperValue == "MOD_INTERIOR3") modType = ModType.Interior3;
            else if (upperValue == "MOD_INTERIOR4") modType = ModType.Interior4;
            else if (upperValue == "MOD_INTERIOR5") modType = ModType.Interior5;
            else if (upperValue == "MOD_SEATS") modType = ModType.Seats;
            else if (upperValue == "MOD_STEERING") modType = ModType.SteeringWheel; // Parsing "MOD_STEERING" to SteeringWheel
            else if (upperValue == "MOD_KNOB") modType = ModType.Knob;
            else if (upperValue == "MOD_PLAQUE") modType = ModType.Plaque;
            else if (upperValue == "MOD_ICE") modType = ModType.Ice;

            else if (upperValue == "MOD_TRUNK") modType = ModType.Trunk;
            else if (upperValue == "MOD_HYDRO") modType = ModType.Hydro;

            else if (upperValue == "MOD_ENGINEBAY1") modType = ModType.EngineBay1;
            else if (upperValue == "MOD_ENGINEBAY2") modType = ModType.EngineBay2;
            else if (upperValue == "MOD_ENGINEBAY3") modType = ModType.EngineBay3;

            else if (upperValue == "MOD_CHASSIS2") modType = ModType.Chassis2;
            else if (upperValue == "MOD_CHASSIS3") modType = ModType.Chassis3;
            else if (upperValue == "MOD_CHASSIS4") modType = ModType.Chassis4;
            else if (upperValue == "MOD_CHASSIS5") modType = ModType.Chassis5;

            else if (upperValue == "MOD_DOOR_L") modType = ModType.DoorLeft;
            else if (upperValue == "MOD_DOOR_R") modType = ModType.DoorRight;

            else if (upperValue == "MOD_LIVERY") modType = ModType.Livery;

            else
            {
                modType = default; // Set to default enum value if not found
                return false; // Parsing failed
            }

            return true; // Parsing succeeded
        }

        public static ModType ParseModType(string value)
        {
            if (TryParseModType(value, out ModType modTypeValue))
            {
                return modTypeValue;
            }
            else
            {
                throw new ArgumentException($"Unknown ModType: {value}", nameof(value));
            }
        }


        public static ModType GetModTypeFromString(string modTypeString)
        {
            return ParseModType(modTypeString); // Simply call ParseModType, which already handles error and exception
        }

        public enum ModKitType
        {
            Standard = 0,     // MKT_STANDARD
            Sport,              // MKT_SPORT
            Suv,                // MKT_SUV
            Special             // MKT_SPECIAL
        }


        public static string ToStringValue(this ModKitType modKitType)
        {
            switch (modKitType)
            {
                case ModKitType.Standard: return "MKT_STANDARD";
                case ModKitType.Sport: return "MKT_SPORT";
                case ModKitType.Suv: return "MKT_SUV";
                case ModKitType.Special: return "MKT_SPECIAL";
                default: return modKitType.ToString();
            }
        }


        public static bool TryParseModKitType(string value, out ModKitType modKitType)
        {
            string upperValue = value.ToUpperInvariant();

            if (upperValue == "MKT_STANDARD") modKitType = ModKitType.Standard;
            else if (upperValue == "MKT_SPORT") modKitType = ModKitType.Sport;
            else if (upperValue == "MKT_SUV") modKitType = ModKitType.Suv;
            else if (upperValue == "MKT_SPECIAL") modKitType = ModKitType.Special;
            else
            {
                modKitType = default;
                return false;
            }
            return true;
        }


        public static ModKitType ParseModKitType(string value)
        {
            if (TryParseModKitType(value, out ModKitType modKitTypeValue))
            {
                return modKitTypeValue;
            }
            else
            {
                throw new ArgumentException($"Unknown ModKitType: {value}", nameof(value));
            }
        }

        public static ModKitType GetModKitTypeFromString(string modKitTypeString)
        {
            return ParseModKitType(modKitTypeString);
        }


        public enum ModCameraPos
        {
            Default = 0,      // MCP_DEFAULT
            Front,              // MCP_FRONT
            FrontLeft,          // MCP_FRONT_LEFT
            FrontRight,         // MCP_FRONT_RIGHT
            Rear,               // MCP_REAR
            RearLeft,           // MCP_REAR_LEFT
            RearRight,          // MCP_REAR_RIGHT
            Left,               // MCP_LEFT
            Right,              // MCP_RIGHT
            Top,                // MCP_TOP
            Bottom              // MCP_BOTTOM
        }


        public static string ToStringValue(this ModCameraPos modCameraPos)
        {
            switch (modCameraPos)
            {
                case ModCameraPos.Default: return "MCP_DEFAULT";
                case ModCameraPos.Front: return "MCP_FRONT";
                case ModCameraPos.FrontLeft: return "MCP_FRONT_LEFT";
                case ModCameraPos.FrontRight: return "MCP_FRONT_RIGHT";
                case ModCameraPos.Rear: return "MCP_REAR";
                case ModCameraPos.RearLeft: return "MCP_REAR_LEFT";
                case ModCameraPos.RearRight: return "MCP_REAR_RIGHT";
                case ModCameraPos.Left: return "MCP_LEFT";
                case ModCameraPos.Right: return "MCP_RIGHT";
                case ModCameraPos.Top: return "MCP_TOP";
                case ModCameraPos.Bottom: return "MCP_BOTTOM";
                default: return modCameraPos.ToString();
            }
        }


        public static bool TryParseModCameraPos(string value, out ModCameraPos modCameraPos)
        {
            string upperValue = value.ToUpperInvariant();

            if (upperValue == "MCP_DEFAULT") modCameraPos = ModCameraPos.Default;
            else if (upperValue == "MCP_FRONT") modCameraPos = ModCameraPos.Front;
            else if (upperValue == "MCP_FRONT_LEFT") modCameraPos = ModCameraPos.FrontLeft;
            else if (upperValue == "MCP_FRONT_RIGHT") modCameraPos = ModCameraPos.FrontRight;
            else if (upperValue == "MCP_REAR") modCameraPos = ModCameraPos.Rear;
            else if (upperValue == "MCP_REAR_LEFT") modCameraPos = ModCameraPos.RearLeft;
            else if (upperValue == "MCP_REAR_RIGHT") modCameraPos = ModCameraPos.RearRight;
            else if (upperValue == "MCP_LEFT") modCameraPos = ModCameraPos.Left;
            else if (upperValue == "MCP_RIGHT") modCameraPos = ModCameraPos.Right;
            else if (upperValue == "MCP_TOP") modCameraPos = ModCameraPos.Top;
            else if (upperValue == "MCP_BOTTOM") modCameraPos = ModCameraPos.Bottom;
            else
            {
                modCameraPos = default;
                return false;
            }
            return true;
        }

        public static ModCameraPos ParseModCameraPos(string value)
        {
            if (TryParseModCameraPos(value, out ModCameraPos modCameraPosValue))
            {
                return modCameraPosValue;
            }
            else
            {
                throw new ArgumentException($"Unknown ModCameraPos: {value}", nameof(value));
            }
        }


        public static ModCameraPos GetModCameraPosFromString(string modCameraPosString)
        {
            return ParseModCameraPos(modCameraPosString);
        }


        public enum ModWheelType
        {
            Invalid = -1,       // MWT_INVALID
            Sport = 0,          // MWT_SPORT
            Muscle,             // MWT_MUSCLE
            Lowrider,           // MWT_LOWRIDER
            Suv,                // MWT_SUV
            Offroad,            // MWT_OFFROAD
            Tuner,              // MWT_TUNER
            Bike,               // MWT_BIKE
            HiEnd,              // MWT_HIEND
            SuperMod1,          // MWT_SUPERMOD1
            SuperMod2,          // MWT_SUPERMOD2
            SuperMod3,          // MWT_SUPERMOD3
            SuperMod4,          // MWT_SUPERMOD4
            SuperMod5           // MWT_SUPERMOD5
        }


        public static string ToStringValue(this ModWheelType modWheelType)
        {
            switch (modWheelType)
            {
                case ModWheelType.Invalid: return "MWT_INVALID";
                case ModWheelType.Sport: return "MWT_SPORT";
                case ModWheelType.Muscle: return "MWT_MUSCLE";
                case ModWheelType.Lowrider: return "MWT_LOWRIDER";
                case ModWheelType.Suv: return "MWT_SUV";
                case ModWheelType.Offroad: return "MWT_OFFROAD";
                case ModWheelType.Tuner: return "MWT_TUNER";
                case ModWheelType.Bike: return "MWT_BIKE";
                case ModWheelType.HiEnd: return "MWT_HIEND";
                case ModWheelType.SuperMod1: return "MWT_SUPERMOD1";
                case ModWheelType.SuperMod2: return "MWT_SUPERMOD2";
                case ModWheelType.SuperMod3: return "MWT_SUPERMOD3";
                case ModWheelType.SuperMod4: return "MWT_SUPERMOD4";
                case ModWheelType.SuperMod5: return "MWT_SUPERMOD5";
                default: return modWheelType.ToString();
            }
        }

        public static bool TryParseModWheelType(string value, out ModWheelType modWheelType)
        {
            string upperValue = value.ToUpperInvariant();

            if (upperValue == "MWT_INVALID") modWheelType = ModWheelType.Invalid;
            else if (upperValue == "MWT_SPORT") modWheelType = ModWheelType.Sport;
            else if (upperValue == "MWT_MUSCLE") modWheelType = ModWheelType.Muscle;
            else if (upperValue == "MWT_LOWRIDER") modWheelType = ModWheelType.Lowrider;
            else if (upperValue == "MWT_SUV") modWheelType = ModWheelType.Suv;
            else if (upperValue == "MWT_OFFROAD") modWheelType = ModWheelType.Offroad;
            else if (upperValue == "MWT_TUNER") modWheelType = ModWheelType.Tuner;
            else if (upperValue == "MWT_BIKE") modWheelType = ModWheelType.Bike;
            else if (upperValue == "MWT_HIEND") modWheelType = ModWheelType.HiEnd;
            else if (upperValue == "MWT_SUPERMOD1") modWheelType = ModWheelType.SuperMod1;
            else if (upperValue == "MWT_SUPERMOD2") modWheelType = ModWheelType.SuperMod2;
            else if (upperValue == "MWT_SUPERMOD3") modWheelType = ModWheelType.SuperMod3;
            else if (upperValue == "MWT_SUPERMOD4") modWheelType = ModWheelType.SuperMod4;
            else if (upperValue == "MWT_SUPERMOD5") modWheelType = ModWheelType.SuperMod5;
            else
            {
                modWheelType = default;
                return false;
            }
            return true;
        }


        public static ModWheelType ParseModWheelType(string value)
        {
            if (TryParseModWheelType(value, out ModWheelType modWheelTypeValue))
            {
                return modWheelTypeValue;
            }
            else
            {
                throw new ArgumentException($"Unknown ModWheelType: {value}", nameof(value));
            }
        }


        public static ModWheelType GetModWheelTypeFromString(string modWheelTypeString)
        {
            return ParseModWheelType(modWheelTypeString);
        }

        public static void SetFoldingWingsDeployed(this Vehicle veh, bool desiredState)
        {
            Function.Call(Hash.SET_DEPLOY_FOLDING_WINGS, veh, desiredState);
        }
        public static bool AreFoldingWingsDeployed(this Vehicle veh)
        {
            return Function.Call<bool>(Hash.ARE_FOLDING_WINGS_DEPLOYED, veh);
        }
        public static void SetHeightAvoidance(this Vehicle car, bool heightAvoidance)
        {
            Function.Call(Hash.SET_VEHICLE_DISABLE_HEIGHT_MAP_AVOIDANCE, car, heightAvoidance);
        }
        public static void SetHasBeenDrivenFlag(this Vehicle vehicle, bool flag)
        {
            Function.Call(Hash.SET_VEHICLE_HAS_BEEN_DRIVEN_FLAG, vehicle.Handle, flag);
        }
    public static bool IsTurretSeat(this Vehicle veh, VehicleSeat seat)
    {
        return Function.Call<bool>(Hash.IS_TURRET_SEAT, veh, seat);
    }

        public static void SetWeaponRestrictedAmmo(this Vehicle vehicle, int weaponIndex, int ammoConstraint)
        {
            Function.Call(Hash.SET_VEHICLE_WEAPON_RESTRICTED_AMMO, vehicle.Handle, weaponIndex, ammoConstraint);
        }

        public static void SetAllowHomingMissileLockon(this Vehicle vehicle, bool toggle)
        {
            Function.Call(Hash.SET_VEHICLE_ALLOW_HOMING_MISSLE_LOCKON, vehicle.Handle, toggle);
        }

        public static void SetArriveDistanceOverrideForVehiclePersuitAttack(this Vehicle vehicle, float distance)
        {
            Function.Call(Hash.SET_ARRIVE_DISTANCE_OVERRIDE_FOR_VEHICLE_PERSUIT_ATTACK, vehicle.Handle, distance);
        }

        public static void SetExclusiveDriver(this Vehicle vehicle, Ped driver, int index = 0)
        {
            Function.Call(Hash.SET_VEHICLE_EXCLUSIVE_DRIVER, vehicle.Handle, driver.Handle, index);
        }

        public static void ClearPrimaryTask(this Vehicle vehicle)
        {
            Function.Call(Hash.CLEAR_PRIMARY_VEHICLE_TASK, vehicle.Handle);
        }
    }
