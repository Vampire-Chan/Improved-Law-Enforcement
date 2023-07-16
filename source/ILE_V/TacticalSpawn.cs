using GTA;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILE_V
{
    public class TacticalSpawn
    {
        public static Random Rand = new Random();

        public static void Tactical_Marines()
        {
            var car = Helpers.SpawnVehicle(ConfigLoader.MARINE_VEHICLES[Rand.Next(0, ConfigLoader.MARINE_VEHICLES.Length)]);
            if (car.Exists() && car != null)
            {
                int weap_exp = Rand.Next(0, 5);
                Helpers.VehicleModifications(car, "MARINES");
                Ped p1;

                for (int i = -1; i < car.PassengerCapacity; i++)
                {
                    p1 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)]);
                    p1.Task.WarpIntoVehicle(car, (VehicleSeat)i);
                    var driver = car.Driver;
                    driver.Task.GoTo(Game.Player.Character.Position);

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
                }
            }
        }
        public static void Tactical_MerryWeather()
        {
            var car = Helpers.SpawnVehicle(ConfigLoader.MERRYW_VEHICLES[Rand.Next(0, ConfigLoader.MERRYW_VEHICLES.Length)]);
            if (car.Exists() && car != null)
            {
                Helpers.VehicleModifications(car, "BLACKOPS");
                Ped p1; int weap_exp = Rand.Next(0, 5);
                for (int i = -1; i < car.PassengerCapacity; i++)
                {
                    p1 = Helpers.SpawnPed(ConfigLoader.MERRYW_SOLDIERS[Rand.Next(0, ConfigLoader.MERRYW_SOLDIERS.Length)]);
                    p1.Task.WarpIntoVehicle(car, (VehicleSeat)i); var driver = car.Driver;
                    driver.Task.GoTo(Game.Player.Character.Position);

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
                }
            }
        }

        public static void Tactical_NOOSE()
        {
            var car = Helpers.SpawnVehicle(ConfigLoader.NOOSE_VEHICLES[Rand.Next(0, ConfigLoader.NOOSE_VEHICLES.Length)]);
            if (car.Exists() && car != null)
            {
                Helpers.VehicleModifications(car, "NOOSE");
                Ped p1;
                for (int i = -1; i < car.PassengerCapacity; i++)
                {
                    p1 = Helpers.SpawnPed(ConfigLoader.NOOSE_SOLDIERS[Rand.Next(0, ConfigLoader.NOOSE_SOLDIERS.Length)]);
                    p1.Task.WarpIntoVehicle(car, (VehicleSeat)i); var driver = car.Driver;
                    driver.Task.GoTo(Game.Player.Character.Position);

                    Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.NOOSE_WEAPON[Rand.Next(0, ConfigLoader.NOOSE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                    Helpers.PedFunctions(p1, Rand.Next(80, 100), FiringPattern.FullAuto, Rand.Next(200, 250), 200);
                }
            }
        }

        public static void IAA_Officers()
        {
            var car = Helpers.SpawnVehicle(ConfigLoader.IAA_VEHICLES[Rand.Next(0, ConfigLoader.IAA_VEHICLES.Length)]);
            if (car.Exists() && car != null)
            {
                Helpers.VehicleModifications(car, "IAA");
                Ped p1;
                for (int i = -1; i < car.PassengerCapacity; i++)
                {
                    p1 = Helpers.SpawnPed(ConfigLoader.IAA_OFFICERS[Rand.Next(0, ConfigLoader.IAA_OFFICERS.Length)]);
                    p1.Task.WarpIntoVehicle(car, (VehicleSeat)i);
                    var driver = car.Driver;
                    driver.Task.GoTo(Game.Player.Character.Position);

                    Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.IAA_FIB_WEAPON[Rand.Next(0, ConfigLoader.IAA_FIB_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                    Helpers.PedFunctions(p1, Rand.Next(60, 80), FiringPattern.FullAuto, Rand.Next(150, 200), 200);
                }
            }
        }   

        public static void FIB_Officers()
        {
            var car = Helpers.SpawnVehicle(ConfigLoader.FIB_VEHICLES[Rand.Next(0, ConfigLoader.NOOSE_VEHICLES.Length)]);
            if (car.Exists() && car != null)
            {
                Helpers.VehicleModifications(car, "FEDERAL");
                Ped p1;
                for (int i = -1; i < car.PassengerCapacity; i++)
                {
                    p1 = Helpers.SpawnPed(ConfigLoader.FIB_OFFICERS[Rand.Next(0, ConfigLoader.FIB_OFFICERS.Length)]);
                    p1.Task.WarpIntoVehicle(car, (VehicleSeat)i);
                    var driver = car.Driver;
                    driver.Task.GoTo(Game.Player.Character.Position);

                    Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.IAA_FIB_WEAPON[Rand.Next(0, ConfigLoader.IAA_FIB_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                    Helpers.PedFunctions(p1, Rand.Next(80, 90), FiringPattern.FullAuto, Rand.Next(150, 200), 200);
                }
            }
        }
    }
}
