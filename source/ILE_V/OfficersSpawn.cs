/* 
 * Copyright Vampire
 * Made by VampireLazy 
 * Spawns Police, NOOSE, FIB, MERRYWEATHER, Security, as well as Marines 
 * when situation is out of control.
 * 
 * Police can be seen patroling sometimes. 1 Car per interval.
 * Star 1 Police can be dispatched with a Dog and Stun. 1 
 * Star 2 Police can be dispatched with a Dog and Pistol. 1
 * Star 3 Police will be dispatched with NOOSE as Air Support and Shotguns will be used. 
 * Dogs will also chase you. (NOOSE, Police)
 * Star 4 Requests NOOSE and FIB in the Scene of Crime. 3 (Police, NOOSE, FIB)
 * Star 5 will Request the National Army, the Marines and the Private Army, Merryweather Corps. 5 (Police, MERRYWEATHER, Marine, NOOSE, FIB)
 * If you are near Bank, Government Building then Security and Police and sometimes NOOSE will be dispatched.
 * If you are near IAA, FIB Building, they will destroy you.
 * If you are inside Fort Zacundo then NOOSE, will join Marines to hunt you down.
 * If you are in Merryweather Dockyard or Lab then they will request FIB to hunt you as well as LSPD in the scene.
 * 
 */

using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;



namespace ILE_V
{
    public class OfficerSpawn : Script
    {
        //Health ranges from 100-150
        //Armour ranges from 50-100
        //Varies with Wanted Levels

        public static Random Rand = new Random();
        public static string[] PED_MODELS;
        public static string[] VEHICLE_MODELS;

        public OfficerSpawn()
        {
            Tick += OnTick;
        }

        public void OnTick(object sender, EventArgs e)
        {
            //Vehicle Spawns every x Seconds.
            if (Game.Player.WantedLevel >= 3)
                Wait(ConfigLoader.SPAWN_GAP + ConfigLoader.SPAWN_GAP / 4);
            else Wait(ConfigLoader.SPAWN_GAP);

            var wl = Game.Player.WantedLevel;
            //Will Keep Spawning all the time.
            Officers();

            //Spawns NOOSE at Star 3
            if (wl == 3)
            {
                Wait(200);
                TacticalSpawn.Tactical_NOOSE();
            }

            //NOOSE and Either FIB or IAA will Spawn at Star 4
            if (wl == 4)
            {
                int a = Rand.Next(0, 2);
                Wait(200);
                TacticalSpawn.Tactical_NOOSE();
                Wait(200);
                if (a == 0) TacticalSpawn.FIB_Officers();
                if (a == 1) TacticalSpawn.IAA_Officers();
            }

            //Marines, MerryWeather and NOOSE alongside FIB and IAA spawns.
            if (wl == 5)
            {
                int a = Rand.Next(0, 2);
                int b = Rand.Next(0, 2);
                Wait(200);
                TacticalSpawn.Tactical_NOOSE();
                Wait(200);
                if (a == 0) TacticalSpawn.FIB_Officers();
                if (a == 1) TacticalSpawn.IAA_Officers();
                Wait(200);
                if (b == 0) TacticalSpawn.Tactical_MerryWeather();
                if (b == 1) TacticalSpawn.Tactical_Marines();
            }
        }

        public static void Officers()
        {
            //Get Zone Name using GetJurisdiction()
            string[] CURRENT_AREA = Zones.GetJurisdiction(Game.Player.Character.Position);

            //Checking Player Current Zone and apply Functions

            //Alamo
            if (CURRENT_AREA.SequenceEqual(Zones.ALAMO))
            {
                PED_MODELS = ConfigLoader.POLICE_VEHICLES_BCSO;
                VEHICLE_MODELS = ConfigLoader.POLICE_VEHICLES_BCSO;
            }

            //Joint Operation by LG and CG. Beach
            if (CURRENT_AREA.SequenceEqual(Zones.BEACH))
            {
                if (Game.Player.WantedLevel >= 3)
                {
                    PED_MODELS = ConfigLoader.LIFEGUARDS.Concat(ConfigLoader.COASTGUARDS).ToArray();
                    VEHICLE_MODELS = ConfigLoader.LIFEGUARD_VEHICLES.Concat(ConfigLoader.COASTGUARD_VEHICLES).ToArray();
                }
                else
                {
                    PED_MODELS = ConfigLoader.COASTGUARDS;
                    VEHICLE_MODELS = ConfigLoader.COASTGUARD_VEHICLES;
                }
            }
            if (CURRENT_AREA.SequenceEqual(Zones.BCSO))
            {
                PED_MODELS = ConfigLoader.POLICE_OFFICERS_BCSO;
                VEHICLE_MODELS = ConfigLoader.POLICE_VEHICLES_BCSO;
            }
            if (CURRENT_AREA.SequenceEqual(Zones.LSIA))
            {
                PED_MODELS = ConfigLoader.POLICE_OFFICERS;
                VEHICLE_MODELS = ConfigLoader.POLICE_VEHICLES;
            }
            if (CURRENT_AREA.SequenceEqual(Zones.LSPD))
            {
                PED_MODELS = ConfigLoader.POLICE_OFFICERS;
                VEHICLE_MODELS = ConfigLoader.POLICE_VEHICLES;
            }
            if (CURRENT_AREA.SequenceEqual(Zones.LSSD))
            {
                PED_MODELS = ConfigLoader.POLICE_OFFICERS_LSSD;
                VEHICLE_MODELS = ConfigLoader.POLICE_VEHICLES_LSSD;
            }
            if (CURRENT_AREA.SequenceEqual(Zones.MERRYWEATHER))
            {
                PED_MODELS = ConfigLoader.MERRYW_SOLDIERS;
                VEHICLE_MODELS = ConfigLoader.MERRYW_VEHICLES;
            }
            if (CURRENT_AREA.SequenceEqual(Zones.NOOSEHQ))
            {
                PED_MODELS = ConfigLoader.NOOSE_SOLDIERS;
                VEHICLE_MODELS = ConfigLoader.NOOSE_VEHICLES;
            }
            if (CURRENT_AREA.SequenceEqual(Zones.SAHP))
            {
                PED_MODELS = ConfigLoader.POLICE_OFFICERS_SAHP;
                VEHICLE_MODELS = ConfigLoader.POLICE_VEHICLES_SAHP;
            }
            if (CURRENT_AREA.SequenceEqual(Zones.SAPR))
            {
                PED_MODELS = ConfigLoader.POLICE_OFFICERS_SAPR;
                VEHICLE_MODELS = ConfigLoader.POLICE_VEHICLES_SAPR;
            }
            if (CURRENT_AREA.SequenceEqual(Zones.SASPR))
            {
                PED_MODELS = ConfigLoader.NOOSE_SOLDIERS;
                VEHICLE_MODELS = ConfigLoader.NOOSE_VEHICLES;
            }
            if (CURRENT_AREA.SequenceEqual(Zones.ZANCUDO))
            {
                PED_MODELS = ConfigLoader.MARINE_SOLDIERS;
                VEHICLE_MODELS = ConfigLoader.MARINE_VEHICLES;
            }

            //Spawning Vehicles
            var car = Helpers.SpawnVehicle(VEHICLE_MODELS[Rand.Next(0, VEHICLE_MODELS.Length)]);
            if (car.Exists() && car != null)
            {
                var wl = Game.Player.WantedLevel;
                Helpers.VehicleModifications(car, "POLICE");


                //Star 0 and 1. Dogs and Officers with Stungun and Nightstick. 100% Accuracy for Stuns. 
                if (wl <= 1)
                {
                    if (car.PassengerCapacity == 1)
                    {
                        var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, VehicleSeat.Driver);
                        p1.DrivingStyle = DrivingStyle.SometimesOvertakeTraffic;
                        Helpers.PedFunctions(p1, 100, FiringPattern.Default, 0, 125);
                        var driver = car.Driver;
                        driver.Task.GoTo(Game.Player.Character.Position);
                        Helpers.GiveWeaponWithAttachments(p1, "WEAPON_STUNGUN", "WEAPON_NIGHTSTICK", false);
                    }
                    else
                    {
                        var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, VehicleSeat.Driver);
                        var p2 = Helpers.SpawnPed(ConfigLoader.POLICE_DOGS[Rand.Next(0, ConfigLoader.POLICE_DOGS.Length)], car, VehicleSeat.RightFront);
                        p1.DrivingStyle = DrivingStyle.SometimesOvertakeTraffic;
                        var driver = car.Driver;
                        driver.Task.GoTo(Game.Player.Character.Position);
                        Helpers.PedFunctions(p1, 100, FiringPattern.Default, 0, 125);
                        Helpers.PedFunctions(p2, 100, FiringPattern.Default, 0, 80);
                        Helpers.GiveWeaponWithAttachments(p1, "WEAPON_STUNGUN", "WEAPON_NIGHTSTICK", false);
                    }
                }

                //Star 2 - 20% Accuracy - Dogs/Handguns - (Dog - 2 Officer)
                if (wl == 2)
                {
                    if (car.PassengerCapacity == 1)
                    {
                        var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, VehicleSeat.Driver);
                        p1.DrivingStyle = DrivingStyle.SometimesOvertakeTraffic;
                        Helpers.PedFunctions(p1, 100, FiringPattern.Default, 0, 125);
                        var driver = car.Driver;
                        driver.Task.GoTo(Game.Player.Character.Position);
                        Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.POLICE_WEAPON_LIGHT[Rand.Next(0, ConfigLoader.POLICE_WEAPON_LIGHT.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    }
                    else
                    {
                        var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, VehicleSeat.Driver);
                        var p2 = Helpers.SpawnPed(ConfigLoader.POLICE_DOGS[Rand.Next(0, ConfigLoader.POLICE_DOGS.Length)], car, VehicleSeat.RightFront);
                        var p3 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, VehicleSeat.LeftRear);
                        p1.DrivingStyle = DrivingStyle.AvoidTraffic;
                        var driver = car.Driver;
                        driver.Task.GoTo(Game.Player.Character.Position);
                        Helpers.PedFunctions(p1, 20, FiringPattern.BurstFirePistol, 10, 125);
                        Helpers.PedFunctions(p3, 20, FiringPattern.BurstFirePistol, 10, 125);
                        Helpers.PedFunctions(p2, 100, FiringPattern.Default, 0, 100);
                        Helpers.GiveWeaponWithAttachments(p3, ConfigLoader.POLICE_WEAPON_LIGHT[Rand.Next(0, ConfigLoader.POLICE_WEAPON_LIGHT.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                        Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.POLICE_WEAPON_LIGHT[Rand.Next(0, ConfigLoader.POLICE_WEAPON_LIGHT.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    }
                }

                //Star 3 - 40% Accuracy - Dogs/Shotguns - (2 Dog - 2 Officer - 10% Armour - Burst Pistol
                if (wl == 3)
                {
                    if (car.PassengerCapacity == 1)
                    {
                        var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, VehicleSeat.Driver);
                        p1.DrivingStyle = DrivingStyle.SometimesOvertakeTraffic;
                        Helpers.PedFunctions(p1, 100, FiringPattern.Default, 0, 125);
                        var driver = car.Driver;
                        driver.Task.GoTo(Game.Player.Character.Position);
                        Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.POLICE_WEAPON_LIGHT[Rand.Next(0, ConfigLoader.POLICE_WEAPON_LIGHT.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    }
                    else
                    {
                        var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, VehicleSeat.Driver);
                        var p2 = Helpers.SpawnPed(ConfigLoader.POLICE_DOGS[Rand.Next(0, ConfigLoader.POLICE_DOGS.Length)], car, VehicleSeat.RightFront);
                        var p3 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, VehicleSeat.LeftRear);
                        var p4 = Helpers.SpawnPed(ConfigLoader.POLICE_DOGS[Rand.Next(0, ConfigLoader.POLICE_DOGS.Length)], car, VehicleSeat.RightRear);
                        Helpers.PedFunctions(p1, 40, FiringPattern.BurstFirePistol, 30, 125);
                        Helpers.PedFunctions(p3, 40, FiringPattern.BurstFirePistol, 30, 125);
                        var driver = car.Driver;
                        driver.Task.GoTo(Game.Player.Character.Position);
                        Helpers.PedFunctions(p2, 100, FiringPattern.Default, 0, 120);
                        p1.DrivingStyle = DrivingStyle.Rushed;
                        Helpers.PedFunctions(p4, 100, FiringPattern.Default, 0, 125);
                        Helpers.GiveWeaponWithAttachments(p3, ConfigLoader.POLICE_WEAPON_LIGHT[Rand.Next(0, ConfigLoader.POLICE_WEAPON_LIGHT.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                        Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.POLICE_WEAPON_LIGHT[Rand.Next(0, ConfigLoader.POLICE_WEAPON_LIGHT.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                    }
                }

                //Star 4 - 60% Accuracy - Shotguns/Rifles 
                if (wl == 4)
                {
                    if (car.PassengerCapacity == 1)
                    {
                        var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, VehicleSeat.Driver);
                        p1.DrivingStyle = DrivingStyle.SometimesOvertakeTraffic;
                        Helpers.PedFunctions(p1, 100, FiringPattern.Default, 0, 125);
                        var driver = car.Driver;
                        driver.Task.GoTo(Game.Player.Character.Position);
                        Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.POLICE_WEAPON_LIGHT[Rand.Next(0, ConfigLoader.POLICE_WEAPON_LIGHT.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    }
                    else
                    {
                        Ped p1;
                        for (int i = -1; i < car.PassengerCapacity; i++)
                        {
                            p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, (VehicleSeat)i);
                            Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.POLICE_WEAPON_HEAVY[Rand.Next(0, ConfigLoader.POLICE_WEAPON_HEAVY.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                            Helpers.PedFunctions(p1, Rand.Next(40, 60), FiringPattern.BurstFireRifle, 70, 200);
                            var driver = car.Driver;
                            driver.Task.GoTo(Game.Player.Character.Position);
                        }
                    }
                }

                //Star 5 - 80-100% Accuracy - Rifles/Shotguns 
                if (wl == 5)
                {
                    if (car.PassengerCapacity == 1)
                    {
                        var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, VehicleSeat.Driver);
                        p1.DrivingStyle = DrivingStyle.SometimesOvertakeTraffic;
                        Helpers.PedFunctions(p1, 100, FiringPattern.Default, 0, 125);
                        var driver = car.Driver;
                        driver.Task.GoTo(Game.Player.Character.Position);
                        Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.POLICE_WEAPON_LIGHT[Rand.Next(0, ConfigLoader.POLICE_WEAPON_LIGHT.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    }
                    else
                    {
                        Ped p1;
                        for (int i = -1; i < car.PassengerCapacity; i++)
                        {
                            p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], car, (VehicleSeat)i);
                            Helpers.GiveWeaponWithAttachments(p1, ConfigLoader.POLICE_WEAPON_HEAVY[Rand.Next(0, ConfigLoader.POLICE_WEAPON_HEAVY.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                            Helpers.PedFunctions(p1, Rand.Next(70, 90), FiringPattern.BurstFireRifle, 100, 200);
                            var driver = car.Driver;
                            driver.Task.GoTo(Game.Player.Character.Position);
                        }
                    }
                }
            }
        }
    }
}