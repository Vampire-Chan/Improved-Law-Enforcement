using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace ILE_V
{
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
        public void Debug(string text)
        {
            if (ConfigLoader.DEBUGGING == true)
                WriteFormattedLog(LogLevel.DEBUG, text);

        }

        /// <summary>
        /// Log an ERROR message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Error(string text)
        {
            if (ConfigLoader.DEBUGGING == true)
                WriteFormattedLog(LogLevel.ERROR, text);
        }

        /// <summary>
        /// Log a FATAL ERROR message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Fatal(string text)
        {
            if (ConfigLoader.DEBUGGING == true)
                WriteFormattedLog(LogLevel.FATAL, text);
        }

        /// <summary>
        /// Log an INFO message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Info(string text)
        {
            if (ConfigLoader.DEBUGGING == true)
                WriteFormattedLog(LogLevel.INFO, text);
        }

        /// <summary>
        /// Log a TRACE message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Trace(string text)
        {
            if (ConfigLoader.DEBUGGING == true)
                WriteFormattedLog(LogLevel.TRACE, text);
        }

        /// <summary>
        /// Log a WARNING message
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="enabled">If log mode is on or off</param>
        public void Warning(string text)
        {
            if (ConfigLoader.DEBUGGING == true)
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


    public class Helpers
    {
        public static Random rand = new Random();

        public static string[] ToArray(string input)
        {
            Logger log = new Logger("./scripts/ILE_V.log");

            string[] array = input.Split(',');
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = string.Join("", array[i].Split((string[])null, StringSplitOptions.RemoveEmptyEntries));
            }
            log.Info("Reading String: [" + input + "].");
            return array;
        }

        public static Vehicle SpawnVehicle(String car)
        {
            Logger log = new Logger("./scripts/ILE_V.log");
            Vehicle veh;
            Vector3 v;
            Model mdl = new Model(car);
            mdl = RequestModel(car);
            Vector3 vac = Game.Player.Character.ForwardVector * 50;

            if (mdl.IsHelicopter == true)
            {
                if (Game.Player.WantedLevel >= 3)
                    v = Game.Player.Character.UpVector + Game.Player.Character.Position.Around(100) + vac;
                else 
                    v = Game.Player.Character.UpVector + Game.Player.Character.Position.Around(100);
            }
            else
            {
                if (Game.Player.Character.IsOnFoot == true)
                {
                    if (Game.Player.WantedLevel >= 3)
                        v =  Game.Player.Character.Position.Around(100) + vac;
                    else
                        v = vac + Game.Player.Character.Position.Around(100);
                   
                }
                else
                {
                    if (Game.Player.WantedLevel >= 3)
                        v = Game.Player.Character.Position.Around(30) + Game.Player.Character.ForwardVector * 80 + vac;
                    else 
                        v = Game.Player.Character.Position.Around(30) + Game.Player.Character.ForwardVector * 80;
                }
            }

            if (mdl.IsLoaded)
            {
                veh = World.CreateVehicle(car, v);
                log.Info("Spawning Vehicle: [" + car + "]. The location is: [X:" + v.X + " Y:" + v.Y + " Z:" + v.Z + "]");
                return veh;
            }
            else
                log.Error("Spawning Vehicle: [" + car + "] failed because Vehicle failed to load into memory.");
            return null;
        }

        public static Ped SpawnPed(String ped)
        {
            Logger log = new Logger("./scripts/ILE_V.log");
            var v = Game.Player.Character.Position.Around(30) + Game.Player.Character.ForwardVector * 355;
            Ped npc;
            var model = RequestModel(ped);
            if (model.IsLoaded)
            {
                npc = World.CreatePed(RequestModel(ped), v);
                log.Info("Spawning Ped: [" + ped + "]. The location is: [X:" + v.X + " Y:" + v.Y + " Z:" + v.Z + "]");
                return npc;
            }
            else
                log.Error("Spawning Pedestrain: [" + ped + "] failed because Pedestrain failed to load into memory.");
            return null;
        }

        /// <summary>
        /// Creates Peds inside vehicle.
        /// </summary>
        /// <param name="ped">Ped Model.</param>
        /// <param name="car">Vehicle Model.</param>
        /// <param name="seat">Seat ID.</param>
        /// <returns>Returns Ped.</returns>
        public static Ped SpawnPed(String ped, Vehicle car, VehicleSeat seat)
        {
            Logger log = new Logger("./scripts/ILE_V.log");
            Ped npc;

            var model = RequestModel(ped);
            if (car != null)
            {
                if (model.IsLoaded && car.Exists())
                {
                    npc = car.CreatePedOnSeat(seat, RequestModel(ped));
                    log.Info("Spawning Ped: [" + ped + "] inside Vehicle");
                    return npc;
                }
                else
                    log.Error("Spawning Pedestrain: [" + ped + "] failed because Pedestrain failed to load into memory.");
                return null;
            }
            else
                log.Error("Spawning Pedestrain: [" + ped + "] failed because Vehicle [" + car.Model.ToString()+"] found was null.");
            return null;
        }

        /// <summary>
        /// Request Model from Game into Memory.
        /// </summary>
        /// <param name="mdl">Model name.</param>
        /// <returns>Returns the Model.</returns>
        public static Model RequestModel(string mdl)
        {
            Logger log = new Logger("./scripts/ILE_V.log");
            Model model = new Model(mdl);

            log.Info("Requesting Model: [" + mdl + "] in Memory.");

            //Checking if Model is Valid or not before requesting.
            if (model.IsValid == true && model.IsInCdImage == true)
            {
                //We will ask the Game to load model in memory.
                model.Request(1500);
            }

            //Checking if Model is Valid but failed to Load.
            if (model.IsInCdImage && model.IsValid && model.IsLoaded == false)
            {
                log.Error("Failed to Request Model: [" + mdl + "] in Memory.");
                return null;
            }

            //Checking if Model exists in Game's RPFs.
            if (model.IsInCdImage == false)
            {
                log.Fatal("Model: [" + mdl + "] failed to load in Memory as it does not exist in Game files.");
                return null;
            }
            return model;
        }

        /// <summary>
        /// Unload Model from Game memory to free up resources.
        /// </summary>
        /// <param name="mdl">Model name.</param>
        public static void RequestUnloadModel(Model mdl)
        {
            Logger log = new Logger("./scripts/ILE_V.log");
            log.Info("Marking Model: [" + mdl + "] to be Disposed off Memory.");
            mdl.MarkAsNoLongerNeeded();
        }


        public static void PedFunctions(Ped ped, int accuracy, FiringPattern type, int armour, int health)
        {
            Logger log = new Logger("./scripts/ILE_V.log");
            ped.Accuracy = accuracy;
            ped.FiringPattern = type;
            ped.MaxHealth = health;
            ped.Armor = armour;
            ped.Heading = health;
            Function.Call(Hash.SET_PED_RANDOM_PROPS, ped);
            Function.Call(Hash.SET_PED_AS_COP, ped, true);
            ped.MarkAsNoLongerNeeded();
            //Function.Call(Hash.Weapon)
            log.Info("Ped Functions Applied to: [" + ped.Model.ToString() + "].");
        }

        public static void VehicleModifications(Vehicle car, String platetext)
        {
            //InstallModKit
            Function.Call(Hash.SET_VEHICLE_MOD_KIT, car, 0);

            
            for (int i = 0; i < 49; i++)
            {
                //GetNumMod
                var mods = car.GetModCount((VehicleMod)i);
                //SetNumMod
                car.SetMod((VehicleMod)i, mods, false);
            }

            //SetVehicleTint
            car.WindowTint = (VehicleWindowTint)(rand.Next(0, 7));

            //NumberPlate
            Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT, car, platetext);
            car.PlaceOnNextStreet();
            car.EngineRunning = true;
            car.Speed = 30;
            car.MarkAsNoLongerNeeded();

        }

        /// <summary>
        /// GunGunGun
        /// </summary>
        /// <param name="ped">Ped model</param>
        /// <param name="weap">Primary Weapon</param>
        /// <param name="weap2">Sidearm</param>
        /// <param name="givegun">Give a Backup weapon? like sidearm or not.</param>
        public static void GiveWeaponWithAttachments(Ped ped, String weap, String weap2, bool explosive)
        {
            ped.Weapons.Give(weap2, 100, true, true);

            if (explosive == true) ped.Weapons.Give(weap, 15, true, true);
            else ped.Weapons.Give(weap, 400, true, true);

            ped.CanSwitchWeapons = true;

            /*
            if (ped.Weapons.HasWeapon(WeaponHash.Pistol))
            {
                Random rand = new Random();
                int a = rand.Next(0, 2);

                if (a == 0) 
                if (a == 1) Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, new InputArgument[2]
                {
                WeaponHash.Pistol, 
                WeaponComponent.PistolClip02 
                }
                );
            }

            if (ped.Weapons.HasWeapon(WeaponHash.CombatPistol))
            {
                Random rand = new Random();
                int a = rand.Next(0, 2);

                if (a == 0) GTA.Native.Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatPistol, WeaponComponent.CombatPistolClip01);
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
		*/
        }
    }
}