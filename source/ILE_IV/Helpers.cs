using IVSDKDotNet;
using IVSDKDotNet.Enums;
using static IVSDKDotNet.Native.Natives;
using System;
using System.Numerics;
using CCL.GTAIV;

namespace ILE_IV
{
    public static class GTAEnum
    {
        public enum WeaponType
        {
            WEAPON_UNARMED,
            WEAPON_BASEBALLBAT,
            WEAPON_POOLCUE,
            WEAPON_KNIFE,
            WEAPON_GRENADE,
            WEAPON_MOLOTOV,
            WEAPON_ROCKET,
            WEAPON_PISTOL,
            WEAPON_UNUSED0,
            WEAPON_DEAGLE,
            WEAPON_SHOTGUN,
            WEAPON_BARETTA,
            WEAPON_MICRO_UZI,
            WEAPON_MP5,
            WEAPON_AK47,
            WEAPON_M4,
            WEAPON_SNIPERRIFLE,
            WEAPON_M40A1,
            WEAPON_RLAUNCHER,
            WEAPON_FTHROWER,
            WEAPON_MINIGUN,
            WEAPON_EPISODIC_1,//Grenade Launcher TLAD/TBOGT
            WEAPON_EPISODIC_2,//Sweeper Shotgun TLAD
            WEAPON_EPISODIC_3,
            WEAPON_EPISODIC_4,//POOL CUE TLAD
            WEAPON_EPISODIC_5,//Grenade From the Grenade Launcher TLAD/TBOGT
            WEAPON_EPISODIC_6,//Sawnoff Shotgun TLAD
            WEAPON_EPISODIC_7,//Automatic 9mm TLAD
            WEAPON_EPISODIC_8,//Pipe Bomb TLAD
            WEAPON_EPISODIC_9,//.44 Pistol TBOGT
            WEAPON_EPISODIC_10,//Explosive AA12 TBOGT
            WEAPON_EPISODIC_11,//AA12 TBOGT
            WEAPON_EPISODIC_12,//P-90 TBOGT
            WEAPON_EPISODIC_13,//Golden Uzi TBOGT
            WEAPON_EPISODIC_14,//M249 TBOGT
            WEAPON_EPISODIC_15,//Explosive Sniper TBOGT
            WEAPON_EPISODIC_16,//Sticky Bombs TBOGT
            WEAPON_EPISODIC_17,//BUZZARD (heli) rocket launcher/null TBOGT
            WEAPON_EPISODIC_18,//BUZZARD (heli) rocket for BUZZARD rocket launcher/freeze TBOGT
            WEAPON_EPISODIC_19,//BUZZARD (heli) minigun TBOGT
            WEAPON_EPISODIC_20,//APC cannon TBOGT
            WEAPON_EPISODIC_21,//Parachute TBOGT
            WEAPON_EPISODIC_22,
            WEAPON_EPISODIC_23,
            WEAPON_EPISODIC_24,
            WEAPON_CAMERA,
            WEAPON_OBJECT,
            WEAPON_WEAPONTYPE_LAST_WEAPONTYPE,
            WEAPON_ARMOUR,
            WEAPON_RAMMEDBYCAR,
            WEAPON_RUNOVERBYCAR,
            WEAPON_EXPLOSION,
            WEAPON_UZI_DRIVEBY,
            WEAPON_DROWNING,
            WEAPON_FALL,
            WEAPON_UNIDENTIFIED,
            WEAPON_ANYMELEE,
            WEAPON_ANYWEAPON,
            WEAPON_ADDONWEAPON_1, //Addon Weapons, From IV Tweaker & Modloader
            WEAPON_ADDONWEAPON_2,
            WEAPON_ADDONWEAPON_3,
            WEAPON_ADDONWEAPON_4,
            WEAPON_ADDONWEAPON_5,
            WEAPON_ADDONWEAPON_6,
            WEAPON_ADDONWEAPON_7,
            WEAPON_ADDONWEAPON_8,
            WEAPON_ADDONWEAPON_9,
            WEAPON_ADDONWEAPON_10,
            WEAPON_ADDONWEAPON_11,
            WEAPON_ADDONWEAPON_12,
            WEAPON_ADDONWEAPON_13,
            WEAPON_ADDONWEAPON_14,
            WEAPON_ADDONWEAPON_15,
            WEAPON_ADDONWEAPON_16,

        };
        public enum VehicleSeat
        {
            None = -3,
            AnyPassengerSeat = -2,
            Driver = -1,
            RightFront = 0,
            LeftRear = 1,
            RightRear = 2,
        };
    }

    public class Logger
    {
        private readonly object fileLock = new object();
        private readonly string datetimeFormat;
        private readonly string logFilename;

        /// <summary>
        /// Initiate an instance of Logger class constructor.
        /// If log file does not exist, it will be created automatically.
        /// </summary>
        public Logger(string name)
        {
            datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            logFilename = name;

            // Log file header line
            string logHeader = logFilename + " is created.";
            if (!System.IO.File.Exists(logFilename))
            {
                WriteLine(System.DateTime.Now.ToString(datetimeFormat) + " " + logHeader);
            }
        }

        /// <summary>
        /// Log a DEBUG message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Debug(string text, bool enabled)
        {
            if (enabled == true)
                WriteFormattedLog(LogLevel.DEBUG, text);

        }

        /// <summary>
        /// Log an ERROR message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Error(string text, bool enabled)
        {
            if (enabled == true)
                WriteFormattedLog(LogLevel.ERROR, text);
        }

        /// <summary>
        /// Log a FATAL ERROR message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Fatal(string text, bool enabled)
        {
            if (enabled == true)
                WriteFormattedLog(LogLevel.FATAL, text);
        }

        /// <summary>
        /// Log an INFO message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Info(string text, bool enabled)
        {
            if (enabled == true)
                WriteFormattedLog(LogLevel.INFO, text);
        }

        /// <summary>
        /// Log a TRACE message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Trace(string text, bool enabled)
        {
            if (enabled == true)
                WriteFormattedLog(LogLevel.TRACE, text);
        }

        /// <summary>
        /// Log a WARNING message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Warning(string text, bool enabled)
        {
            if (enabled == true)
                WriteFormattedLog(LogLevel.WARNING, text);
        }

        private void WriteLine(string text, bool append = false)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }
                lock (fileLock)
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilename, append, System.Text.Encoding.UTF8))
                    {
                        writer.WriteLine(text);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void WriteFormattedLog(LogLevel level, string text)
        {
            string pretext;
            switch (level)
            {
                case LogLevel.TRACE:
                    pretext = System.DateTime.Now.ToString(datetimeFormat) + " [TRACE]   ";
                    break;
                case LogLevel.INFO:
                    pretext = System.DateTime.Now.ToString(datetimeFormat) + " [INFO]    ";
                    break;
                case LogLevel.DEBUG:
                    pretext = System.DateTime.Now.ToString(datetimeFormat) + " [DEBUG]   ";
                    break;
                case LogLevel.WARNING:
                    pretext = System.DateTime.Now.ToString(datetimeFormat) + " [WARNING] ";
                    break;
                case LogLevel.ERROR:
                    pretext = System.DateTime.Now.ToString(datetimeFormat) + " [ERROR]   ";
                    break;
                case LogLevel.FATAL:
                    pretext = System.DateTime.Now.ToString(datetimeFormat) + " [FATAL]   ";
                    break;
                default:
                    pretext = "";
                    break;
            }

            WriteLine(pretext + text, true);
        }

        [System.Flags]
        private enum LogLevel
        {
            TRACE,
            INFO,
            DEBUG,
            WARNING,
            ERROR,
            FATAL
        }
    }

    public class Helpers : Script
    {
        public static Random rand = new Random();

        public static string[] ToArray(string input)
        {
            string[] array = input.Split(',');
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = string.Join("", array[i].Split((string[])null, StringSplitOptions.RemoveEmptyEntries));
            }
            return array;
        }

        public static CVehicle SpawnVehicle(String car, Vector3 v, bool logg)
        {
            Logger log = new Logger(ConfigLoader.LOGGING_LOCATION);
            var veh = NativeWorld.SpawnVehicle(car, v, out int veh2);
            log.Info("Spawning Vehicle: [" + car + "]. The location is: [X:" + v.X + "Y:" + v.Y + "Z:" + v.Z + "]", logg);

            return veh;
        }

        public static CPed SpawnPed(String ped, Vector3 v, bool logg)
        {
            Logger log = new Logger(ConfigLoader.LOGGING_LOCATION);
            var npc = NativeWorld.SpawnPed(ped, v, out int npc2);

            log.Info("Spawning Ped: [" + ped + "]. The location is: [X:" + v.X + "Y:" + v.Y + "Z:" + v.Z + "]", logg);
            return npc;
        }

        /*
        public static void RequestModel(string mdl, bool logg)
        {
            Logger log = new Logger(ConfigLoader.LOGGING_LOCATION);
            Model model = new Model(mdl);
            if (!model.Request(1500))
            {
                log.Info("Requesting Model: [" + mdl + "] in Memory.", logg);

                if (model.IsInCdImage && model.IsValid)
                {
                    log.Info("Failed to Request Model: [" + mdl + "] in Memory.", logg);
                    return null;
                }
                if (model.IsInCdImage == false)
                {
                    log.Info("Model: [" + mdl + "] failed to load in Memory as it does not exist in Game files.", logg);
                }
                return null;
            }
            return model;
        }
        public static void RequestUnloadModel(Model mdl, bool logg)
        {
            Logger log = new Logger(ConfigLoader.LOGGING_LOCATION);
            log.Info("Marking Model: [" + mdl + "] to be Disposed off Memory.", logg);
            mdl.MarkAsNoLongerNeeded();
        }
        */

        public static void PedFunctions(CPed ped, byte accuracy, byte shootrate, uint armour, int health)
        {
            ped.Accuracy = accuracy;
            ped.ShootRate = shootrate;
            ped.MaxHealth = health;
            ped.AddArmourToChar(armour);
            ped.Health = health;
            //SET_PED
            //Function.Call(Hash.SET_PED_RANDOM_PROPS, ped);
            //Function.Call(Hash.SET_PED_AS_COP, ped, true);
            MARK_CHAR_AS_NO_LONGER_NEEDED(ped.GetHandle());
        }

        /*        public static void VehicleModifications(Vehicle car, String platetext, bool log)
                {
                    //InstallModKit
                   CVehicle.PhysicalFlags.

                    //GetNumMod
                    var spoiler = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.Spoilers);
                    var armor = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.Armor);
                    var brake = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.Brakes);
                    var engine = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.Engine);
                    var skirt = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.SideSkirt);
                    var suspense = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.Suspension);
                    var bumpf = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.FrontBumper);
                    var bumpr = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.RearBumper);
                    var exhaust = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.Exhaust);
                    var frame = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.Frame);
                    var grill = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.Grille);
                    var horn = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.Horns);
                    var hood = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, car, VehicleModType.Hood);

                    //SetMod
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.Spoilers, rand.Next(0, spoiler), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.Armor, rand.Next(0, armor), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.Brakes, rand.Next(0, brake), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.Engine, rand.Next(0, engine), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.SideSkirt, rand.Next(0, skirt), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.Suspension, rand.Next(0, suspense), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.FrontBumper, rand.Next(0, bumpf), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.RearBumper, rand.Next(0, bumpr), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.Exhaust, rand.Next(0, exhaust), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.Frame, rand.Next(0, frame), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.Grille, rand.Next(0, grill), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.Horns, rand.Next(0, horn), true);
                    Function.Call(Hash.SET_VEHICLE_MOD, car, VehicleModType.Hood, rand.Next(0, hood), true);
                    //maybe more? idk

                    //SetVehicleTint
                    Function.Call(Hash.SET_VEHICLE_WINDOW_TINT, car, (VehicleWindowTint)rand.Next(0, 7));
                    //NumberPlate
                    Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT, car, platetext);

                    car.MarkAsNoLongerNeeded();
                }
        */
        
        public static void GiveWeapon(CPed ped, String weap, String weap2, bool explosive)
        {
            var hash = ped.GetHandle();

            var wp1 = (GTAEnum.WeaponType)Enum.Parse(typeof(GTAEnum.WeaponType), weap);
var wp2 = (GTAEnum.WeaponType)Enum.Parse(typeof(GTAEnum.WeaponType), weap2);

            GIVE_WEAPON_TO_CHAR(hash, (uint)wp2, 100, false);
            if (explosive == true)
            {
                GIVE_WEAPON_TO_CHAR(hash, (uint)wp1, 10, false);
            }
            else 
            { 
                GIVE_WEAPON_TO_CHAR(hash, (uint)wp1, 300, false); 
            }
        }

        public static void GiveWeapon(CPed ped, eWeaponType weap, eWeaponType weap2, bool explosive)
        {
            var hash = ped.GetHandle();

            GIVE_WEAPON_TO_CHAR(hash, (uint)weap2, 100, false);
            if (explosive == true)
            {
                GIVE_WEAPON_TO_CHAR(hash, (uint)weap, 10, false);
            }
            else
            {
                GIVE_WEAPON_TO_CHAR(hash, (uint)weap, 300, false);
            }
        }

        /*
        if (ped.Weapons.HasWeapon(WeaponHash.Pistol))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.Pistol, WeaponComponentHash.PistolClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.Pistol, WeaponComponentHash.PistolClip02);
        }

        if (ped.Weapons.HasWeapon(WeaponHash.CombatPistol))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.CombatPistolClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.CombatPistolClip02);
        }

        if (ped.Weapons.HasWeapon(WeaponHash.APPistol))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.APPistolClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.APPistolClip02);
        }

        if (ped.Weapons.HasWeapon(WeaponHash.Pistol50))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.Pistol50Clip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.Pistol50Clip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.SNSPistol))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SNSPistolClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SNSPistolClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.SNSPistolMk2))
        { 
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SNSPistolMk2Clip02);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SNSPistolMk2Clip01);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.HeavyPistol))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.HeavyPistolClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.HeavyPistolClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.PistolMk2))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.PistolMk2Clip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.PistolMk2Clip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.CeramicPistol))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.CeramicPistolClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.CeramicPistolClip02);
        }
        //revolver need to be done
        if (ped.Weapons.HasWeapon(WeaponHash.VintagePistol))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.VintagePistolClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.VintagePistolClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.MicroSMG))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.MicroSMGClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.MicroSMGClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.SMG))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);
            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.SMGMk2))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip02);
        }if (ped.Weapons.HasWeapon(WeaponHash.AssaultSMG))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultSMGClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultSMGClip02);
        }if (ped.Weapons.HasWeapon(WeaponHash.CombatPDW))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.CombatPDWClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.CombatPDWClip02);
        }if (ped.Weapons.HasWeapon(WeaponHash.MachinePistol))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.MachinePistolClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.MachinePistolClip02);
        }if (ped.Weapons.HasWeapon(WeaponHash.MiniSMG))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.MiniSMGClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.MiniSMGClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.PumpShotgun))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.PumpShotgunMk2))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            //if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip01);
            //if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.SawnOffShotgun))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            //if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip01);
           // if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.AssaultShotgun))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultShotgunClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultShotgunClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.BullpupShotgun))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            //if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip01);
            //if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.HeavyShotgun))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.HeavyShotgunClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.HeavyShotgunClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.DoubleBarrelShotgun))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.DBShotgunClip01);
            //if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.SweeperShotgun))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SweeperShotgunClip01);
            //if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.CombatShotgun))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.CombatShotgunClip01);
            //if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.SMGMk2Clip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.AssaultRifle))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.CarbineRifle))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.CarbineRifleMk2))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.AdvancedRifle))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.SpecialCarbine))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.BullpupRifle))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.BullpupRifleMk2))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.CompactRifle))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.MilitaryRifle))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.HeavyRifle))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.ServiceCarbine))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.ServiceCarbine, WeaponComponentHash.ServiceCarbineClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.ServiceCarbine, WeaponComponentHash.ServiceCarbineClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.MG))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.CombatMG))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.AssaultrifleMk2))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.AssaultrifleMk2, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.AssaultrifleMk2, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.CombatMGMk2))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.Gusenberg))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.SniperRifle))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.HeavySniper))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.HeavySniperMk2))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.MarksmanRifle))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.MarksmanRifleMk2))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponentHash.AssaultRifleClip02);
        }
        if (ped.Weapons.HasWeapon(WeaponHash.PrecisionRifle))
        {
            Random rand = new Random();
            int a = rand.Next(0, 2);

            if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.PrecisionRifle, WeaponComponentHash.AssaultRifleClip01);
            if (a == 1) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.PrecisionRifle, WeaponComponentHash.AssaultRifleClip02);
        }

    }*/
    }
}
