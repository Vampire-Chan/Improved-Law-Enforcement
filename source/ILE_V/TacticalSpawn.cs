
using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;



namespace ILE_V
{
    public class TacticalForce : Script
    {
        //Health ranges from 100-150
        //Armour ranges from 50-100
        //Varies with Wanted Levels

        public static Random Rand = new Random();
        public static string[] PED_MODELS;
        public static string[] VEHICLE_MODELS;

        public TacticalForce()
        {
            Tick += OnTick;
            Interval = 15000;
        }

        public void OnTick(object sender, EventArgs e)
        {
            try
            {
                var wl = Game.Player.WantedLevel;

                if (OfficerSpawn.Pedarray.Count <= 30)
                {
                    //Spawns NOOSE at Star 3
                    if (wl == 3)
                    {
                        Wait(200);
                        Tactical_NOOSE();
                    }

                    //NOOSE and Either FIB or IAA will Spawn at Star 4
                    if (wl == 4)
                    {
                        int a = Rand.Next(0, 2);
                        Wait(200);
                        Tactical_NOOSE();
                        Wait(1500);
                        if (a == 0) FIB_Officers();
                        if (a == 1) IAA_Officers();
                    }

                    //Marines, MerryWeather and NOOSE alongside FIB and IAA spawns.
                    if (wl == 5)
                    {
                        int a = Rand.Next(0, 2);
                        int b = Rand.Next(0, 2);
                        Wait(200);
                        Tactical_NOOSE();
                        Wait(1500);
                        if (a == 0) FIB_Officers();
                        if (a == 1) IAA_Officers();
                        Wait(1500);
                        if (b == 0) Tactical_MerryWeather();
                        if (b == 1) Tactical_Marines();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger log = new Logger("./scripts/ILE_V.log");
                log.Fatal("An error occured while executing the code. See this for more: [" + ex.GetType() + "] " + ex.ToString());
            }
        }
        public static void Tactical_Marines()
        {
            try
            {
                var car = Helpers.SpawnVehicle(ConfigLoader.MARINE_VEHICLES[Rand.Next(0, ConfigLoader.MARINE_VEHICLES.Length)]);
                if (car.Exists() && car != null)
                {
                    int weap_exp = Rand.Next(0, 5);
                    Helpers.VehicleModifications(car, "MARINES");
                    //Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT_INDEX, )
                    Ped p1;

                    for (int i = -1; i < car.PassengerCount; i++)
                    {
                        p1 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], car, (VehicleSeat)i);
                        //Explosives
                        if (weap_exp >= 3 && weap_exp <= 4)
                        {
                            p1.Weapons.Give(ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], 30, true, false);
                            Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                            Helpers.PedFunctions(p1, Rand.Next(30, 50), FiringPattern.FullAuto, Rand.Next(200, 300), 200);
                        }
                        if (weap_exp < 3)
                        {
                            Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                            Helpers.PedFunctions(p1, Rand.Next(70, 100), FiringPattern.FullAuto, Rand.Next(200, 250), 200);
                        }
                        OfficerSpawn.Pedarray.Add(p1);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger log = new Logger("./scripts/ILE_V.log");

                log.Fatal("An error occured while executing the code. See this for more: [" + ex.GetType() + "] " + ex.ToString());
            }
        }
        public static void Tactical_MerryWeather()
        {
            try
            {
                var car = Helpers.SpawnVehicle(ConfigLoader.MERRYW_VEHICLES[Rand.Next(0, ConfigLoader.MERRYW_VEHICLES.Length)]);
                if (car.Exists() && car != null)
                {
                    Helpers.VehicleModifications(car, "BLACKOPS");
                    Ped p1; int weap_exp = Rand.Next(0, 5);
                    for (int i = -1; i < car.PassengerCount; i++)
                    {
                        p1 = Helpers.SpawnPed(ConfigLoader.MERRYW_SOLDIERS[Rand.Next(0, ConfigLoader.MERRYW_SOLDIERS.Length)], car, (VehicleSeat)i);
                        //Explosives
                        if (weap_exp >= 3 && weap_exp <= 4)
                        {
                            p1.Weapons.Give(ConfigLoader.MERRYW_EXPLOSIVES[Rand.Next(0, ConfigLoader.MERRYW_EXPLOSIVES.Length)], 30, true, false);
                            Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.MERRYW_WEAPON[Rand.Next(0, ConfigLoader.MERRYW_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                            Helpers.PedFunctions(p1, Rand.Next(40, 60), FiringPattern.FullAuto, Rand.Next(200, 300), 200);
                        }
                        if (weap_exp < 3)
                        {
                            Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.MERRYW_WEAPON[Rand.Next(0, ConfigLoader.MERRYW_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                            Helpers.PedFunctions(p1, Rand.Next(80, 100), FiringPattern.FullAuto, Rand.Next(200, 250), 200);
                        }
                        OfficerSpawn.Pedarray.Add(p1);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger log = new Logger("./scripts/ILE_V.log");

                log.Fatal("An error occured while executing the code. See this for more: [" + ex.GetType() + "] " + ex.ToString());
            }
        }

        public static void Tactical_NOOSE()
        {
            try
            {
                var car = Helpers.SpawnVehicle(ConfigLoader.NOOSE_VEHICLES[Rand.Next(0, ConfigLoader.NOOSE_VEHICLES.Length)]);
                if (car.Exists() && car != null)
                {
                    Helpers.VehicleModifications(car, "NOOSE");
                    Ped p1;
                    for (int i = -1; i < car.PassengerCount; i++)
                    {
                        p1 = Helpers.SpawnPed(ConfigLoader.NOOSE_SOLDIERS[Rand.Next(0, ConfigLoader.NOOSE_SOLDIERS.Length)], car, (VehicleSeat)i);
                        Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.NOOSE_WEAPON[Rand.Next(0, ConfigLoader.NOOSE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                        Helpers.PedFunctions(p1, Rand.Next(80, 100), FiringPattern.FullAuto, Rand.Next(200, 250), 200);
                        OfficerSpawn.Pedarray.Add(p1);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger log = new Logger("./scripts/ILE_V.log");

                log.Fatal("An error occured while executing the code. See this for more: [" + ex.GetType() + "] " + ex.ToString());
            }
        }

        public static void IAA_Officers()
        {
            try
            {
                var car = Helpers.SpawnVehicle(ConfigLoader.IAA_VEHICLES[Rand.Next(0, ConfigLoader.IAA_VEHICLES.Length)]);
                if (car.Exists() && car != null)
                {
                    Helpers.VehicleModifications(car, "IAA");
                    Ped p1;
                    for (int i = -1; i < car.PassengerCount; i++)
                    {
                        p1 = Helpers.SpawnPed(ConfigLoader.IAA_OFFICERS[Rand.Next(0, ConfigLoader.IAA_OFFICERS.Length)], car, (VehicleSeat)i);
                        Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.IAA_FIB_WEAPON[Rand.Next(0, ConfigLoader.IAA_FIB_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                        Helpers.PedFunctions(p1, Rand.Next(60, 80), FiringPattern.FullAuto, Rand.Next(150, 200), 200);
                        OfficerSpawn.Pedarray.Add(p1);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger log = new Logger("./scripts/ILE_V.log");

                log.Fatal("An error occured while executing the code. See this for more: [" + ex.GetType() + "] " + ex.ToString());
            }
        }

        public static void FIB_Officers()
        {
            try
            {
                var car = Helpers.SpawnVehicle(ConfigLoader.FIB_VEHICLES[Rand.Next(0, ConfigLoader.NOOSE_VEHICLES.Length)]);
                if (car.Exists() && car != null)
                {
                    Helpers.VehicleModifications(car, "FEDERAL");
                    Ped p1;
                    for (int i = -1; i < car.PassengerCount; i++)
                    {
                        p1 = Helpers.SpawnPed(ConfigLoader.FIB_OFFICERS[Rand.Next(0, ConfigLoader.FIB_OFFICERS.Length)], car, (VehicleSeat)i);
                        Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.IAA_FIB_WEAPON[Rand.Next(0, ConfigLoader.IAA_FIB_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                        Helpers.PedFunctions(p1, Rand.Next(80, 90), FiringPattern.FullAuto, Rand.Next(150, 200), 200);
                        OfficerSpawn.Pedarray.Add(p1);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger log = new Logger("./scripts/ILE_V.log");

                log.Fatal("An error occured while executing the code. See this for more: [" + ex.GetType() + "] " + ex.ToString());
            }
        }
    }
}