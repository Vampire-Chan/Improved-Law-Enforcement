using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILE_V
{
    public class TacticalSpawn
    {
        public static Random Rand = new Random();

        public static void Tactical_Marines(bool log)
        {
            ConfigLoader.LoadValues();
            var car = Helpers.SpawnVehicle(ConfigLoader.MARINE_VEHICLES[Rand.Next(0, ConfigLoader.MARINE_VEHICLES.Length)], Game.Player.Character.ForwardVector * 140, log);
            if (car.Exists() && car != null)
            {
                Helpers.VehicleModifications(car, "MARINES", log);
                Ped p1;
                for (int i = -1; i < car.PassengerCapacity; i++)
                {
                    p1 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], Game.Player.Character.ForwardVector * 160, log);
                    p1.Task.WarpIntoVehicle(car, (VehicleSeat)i);
                }
                Ped[] passengers = car.Occupants;
                foreach (var passenger in passengers)
                {
                    int weap_exp = Rand.Next(0, 5);

                    //Explosives
                    if (weap_exp >= 3 && weap_exp <= 4)
                    {
                        passenger.Weapons.Give(ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], 30, true, false);
                        Helpers.GiveWeaponWithAttachments(passenger, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                        Helpers.PedFunctions(passenger, Rand.Next(30, 50), FiringPattern.FullAuto, Rand.Next(200, 300), 200);
                    }
                    if (weap_exp < 3)
                    {
                        Helpers.GiveWeaponWithAttachments(passenger, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                        Helpers.PedFunctions(passenger, Rand.Next(70, 100), FiringPattern.FullAuto, Rand.Next(200, 250), 200);
                    }
                }
            }
        }
        public static void Tactical_MerryWeather(bool log)
        {
            ConfigLoader.LoadValues();
            var car = Helpers.SpawnVehicle(ConfigLoader.MERRYW_VEHICLES[Rand.Next(0, ConfigLoader.MERRYW_VEHICLES.Length)], Game.Player.Character.ForwardVector * 140, log);
            if (car.Exists() && car != null)
            {
                Helpers.VehicleModifications(car, "BLACKOPS", log);
                Ped p1;
                for (int i = -1; i < car.PassengerCapacity; i++)
                {
                    p1 = Helpers.SpawnPed(ConfigLoader.MERRYW_SOLDIERS[Rand.Next(0, ConfigLoader.MERRYW_SOLDIERS.Length)], Game.Player.Character.ForwardVector * 160, log);
                    p1.Task.WarpIntoVehicle(car, (VehicleSeat)i);
                }
                Ped[] passengers = car.Occupants;
                foreach (var passenger in passengers)
                {
                    int weap_exp = Rand.Next(0, 5);

                    //Explosives
                    if (weap_exp >= 3 && weap_exp <= 4)
                    {
                        passenger.Weapons.Give(ConfigLoader.MERRYW_EXPLOSIVES[Rand.Next(0, ConfigLoader.MERRYW_EXPLOSIVES.Length)], 30, true, false);
                        Helpers.GiveWeaponWithAttachments(passenger, ConfigLoader.MERRYW_WEAPON[Rand.Next(0, ConfigLoader.MERRYW_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                        Helpers.PedFunctions(passenger, Rand.Next(40, 60), FiringPattern.FullAuto, Rand.Next(200, 300), 200);
                    }
                    if (weap_exp < 3)
                    {
                        Helpers.GiveWeaponWithAttachments(passenger, ConfigLoader.MERRYW_WEAPON[Rand.Next(0, ConfigLoader.MERRYW_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                        Helpers.PedFunctions(passenger, Rand.Next(80, 100), FiringPattern.FullAuto, Rand.Next(200, 250), 200);
                    }
                }
            }
        }

        public static void Tactical_NOOSE(bool log)
        {
            ConfigLoader.LoadValues();
            var car = Helpers.SpawnVehicle(ConfigLoader.NOOSE_VEHICLES[Rand.Next(0, ConfigLoader.NOOSE_VEHICLES.Length)], Game.Player.Character.ForwardVector * 140, log);
            if (car.Exists() && car != null)
            {
                Helpers.VehicleModifications(car, "NOOSE", log);
                Ped p1;
                for (int i = -1; i < car.PassengerCapacity; i++)
                {
                    p1 = Helpers.SpawnPed(ConfigLoader.NOOSE_SOLDIERS[Rand.Next(0, ConfigLoader.NOOSE_SOLDIERS.Length)], Game.Player.Character.ForwardVector * 160, log);
                    p1.Task.WarpIntoVehicle(car, (VehicleSeat)i);
                }
                Ped[] passengers = car.Occupants;
                foreach (var passenger in passengers)
                {
                        Helpers.GiveWeaponWithAttachments(passenger, ConfigLoader.NOOSE_WEAPON[Rand.Next(0, ConfigLoader.NOOSE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                        Helpers.PedFunctions(passenger, Rand.Next(80, 100), FiringPattern.FullAuto, Rand.Next(200, 250), 200); 
                }
            }
        }

        public static void IAA_Officers(bool log)
        {
            ConfigLoader.LoadValues();
            var car = Helpers.SpawnVehicle(ConfigLoader.IAA_VEHICLES[Rand.Next(0, ConfigLoader.IAA_VEHICLES.Length)], Game.Player.Character.ForwardVector * 140, log);
            if (car.Exists() && car != null)
            {
                Helpers.VehicleModifications(car, "IAA", log);
                Ped p1;
                for (int i = -1; i < car.PassengerCapacity; i++)
                {
                    p1 = Helpers.SpawnPed(ConfigLoader.IAA_OFFICERS[Rand.Next(0, ConfigLoader.IAA_OFFICERS.Length)], Game.Player.Character.ForwardVector * 160, log);
                    p1.Task.WarpIntoVehicle(car, (VehicleSeat)i);
                }
                Ped[] passengers = car.Occupants;
                foreach (var passenger in passengers)
                {
                    Helpers.GiveWeaponWithAttachments(passenger, ConfigLoader.IAA_FIB_WEAPON[Rand.Next(0, ConfigLoader.IAA_FIB_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                    Helpers.PedFunctions(passenger, Rand.Next(60, 80), FiringPattern.FullAuto, Rand.Next(150, 200), 200);
                }
            }
        }

        public static void FIB_Officers(bool log)
        {
            ConfigLoader.LoadValues();
            var car = Helpers.SpawnVehicle(ConfigLoader.FIB_VEHICLES[Rand.Next(0, ConfigLoader.NOOSE_VEHICLES.Length)], Game.Player.Character.ForwardVector * 140, log);
            if (car.Exists() && car != null)
            {
                Helpers.VehicleModifications(car, "FEDERAL", log);
                Ped p1;
                for (int i = -1; i < car.PassengerCapacity; i++)
                {
                    p1 = Helpers.SpawnPed(ConfigLoader.FIB_OFFICERS[Rand.Next(0, ConfigLoader.FIB_OFFICERS.Length)], Game.Player.Character.ForwardVector * 160, log);
                    p1.Task.WarpIntoVehicle(car, (VehicleSeat)i);
                }
                Ped[] passengers = car.Occupants;
                foreach (var passenger in passengers)
                {
                    Helpers.GiveWeaponWithAttachments(passenger, ConfigLoader.IAA_FIB_WEAPON[Rand.Next(0, ConfigLoader.IAA_FIB_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                    Helpers.PedFunctions(passenger, Rand.Next(80, 90), FiringPattern.FullAuto, Rand.Next(150, 200), 200);
                }
            }
        }
    }
}
