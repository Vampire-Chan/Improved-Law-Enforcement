/* 
 * Copyright Vampire
 * Made by VampireLazy 
 * Spawns Police, NOOSE, FIB, MERRYWEATHER, Security, as well as Marines 
 * when situation is out of control.
 * 
 * IDEA IS LIKE:
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

using IVSDKDotNet;
using IVSDKDotNet.Enums;
using static IVSDKDotNet.Native.Natives;
using System;
using System.Timers;
using System.Numerics;
using CCL.GTAIV;
using CCL.GTAIV.TaskController;

namespace ILE_IV
{
    public class OfficerSpawn : Script
    {
        //Health ranges from 100-150
        //Armour ranges from 50-100
        //Varies with Wanted Levels

        public static Random Rand = new Random();
        public static string[] PED_MODELS = ConfigLoader.POLICE_OFFICERS;
        public static string[] VEHICLE_MODELS = ConfigLoader.POLICE_VEHICLES;

        public OfficerSpawn()
        {
            Tick += OnTick;
        }

        public void OnTick(object sender, EventArgs e)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            STORE_WANTED_LEVEL(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out uint wl);

            if (wl >= 3)
                timer.Interval = ConfigLoader.SPAWN_GAP + ConfigLoader.SPAWN_GAP / 4;
            else timer.Interval = ConfigLoader.SPAWN_GAP;

            timer.Elapsed += Dispatching;
            timer.Start();
        }

        public void Dispatching(object sender, ElapsedEventArgs e)
        {
            STORE_WANTED_LEVEL(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out uint wl);

            //Will Keep Spawning all the time.
            Officers(ConfigLoader.DEBUGGING);

            //Spawns NOOSE at Star 3
            if (wl == 3)
            {
                TacticalSpawn.Tactical_NOOSE(ConfigLoader.DEBUGGING);
            }

            //NOOSE and Either FIB or IAA will Spawn at Star 4
            if (wl == 4)
            {
                int a = Rand.Next(0, 2);
                TacticalSpawn.Tactical_NOOSE(ConfigLoader.DEBUGGING);
                if (a == 0) TacticalSpawn.FIB_Officers(ConfigLoader.DEBUGGING);
                if (a == 1) TacticalSpawn.IAA_Officers(ConfigLoader.DEBUGGING);
            }

            //Marines, MerryWeather and NOOSE alongside FIB and IAA spawns.
            if (wl == 5)
            {
                int a = Rand.Next(0, 2);
                int b = Rand.Next(0, 2);
                TacticalSpawn.Tactical_NOOSE(ConfigLoader.DEBUGGING);
                if (a == 0) TacticalSpawn.FIB_Officers(ConfigLoader.DEBUGGING);
                if (a == 1) TacticalSpawn.IAA_Officers(ConfigLoader.DEBUGGING);
                if (b == 0) TacticalSpawn.Tactical_MerryWeather(ConfigLoader.DEBUGGING);
                if (b == 1) TacticalSpawn.Tactical_Marines(ConfigLoader.DEBUGGING);
            }
        }

        public static void Officers(bool log)
        {
            STORE_WANTED_LEVEL(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out uint wl);

            //Get Zone Name using GetJurisdiction()
            GET_CHAR_COORDINATES(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out Vector3 loc);
            string[] CURRENT_AREA = Zones.GetJurisdiction(loc);


            //Will setup this Zone thing later

            //Checking Player Current Zone and apply Functions
            /*
            //Alamo
            if (CURRENT_AREA.SequenceEqual(Zones.ALAMO))
            {
                PED_MODELS = ConfigLoader.POLICE_VEHICLES_BCSO;
                VEHICLE_MODELS = ConfigLoader.POLICE_VEHICLES_BCSO;
            }

            //Joint Operation by LG and CG. Beach
            if (CURRENT_AREA.SequenceEqual(Zones.BEACH))
            {
                if (wl >= 3)
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
            }*/

            //Spawning Vehicles

            GET_CHAR_COORDINATES(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out Vector3 pos);
            GET_CHAR_HEADING(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out float heading);

            var pos2 = CWorld.GetNextPositionOnStreet(Helper.GetPositionInFrontOfEntity(pos, Helper.HeadingToDirection(heading), 150));

            var car = Helpers.SpawnVehicle(VEHICLE_MODELS[Rand.Next(0, VEHICLE_MODELS.Length)], pos2, log);

            if (DOES_VEHICLE_EXIST(car.GetHandle()) && car.GetHandle() != 0)
            {
                //Star 0 and 1
                if (wl <= 1)
                {
                    var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p2 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);

                    p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.Driver);
                    p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);

                    Helpers.PedFunctions(p1, 100, 30, 0, 125);
                    Helpers.PedFunctions(p2, 100, 30, 0, 120);
                }
                //Star 2 - 20% Accuracy
                if (wl == 2)
                {
                    var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p2 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);

                    p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.LeftRear);
                    p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);

                    Helpers.PedFunctions(p1, 100, 30, 0, 125);
                    Helpers.PedFunctions(p2, 100, 30, 0, 120);

                }
                //Star 3 - 40% Accuracy - Dogs/Shotguns - (2 Dog - 2 Officer - 10% Armour - Burst Pistol
                if (wl == 3)
                {
                    var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p2 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p3 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);

                    p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.Driver);
                    p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);
                    p3.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.LeftRear);

                    Helpers.PedFunctions(p1, 40, 30, 0, 125);
                    Helpers.PedFunctions(p2, 50, 30, 0, 120);
                    Helpers.PedFunctions(p3, 50, 30, 0, 120);

                }
                //Star 4 - 60% Accuracy - Shotguns/Rifles 
                if (wl == 4)
                {
                    var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p2 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p3 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p4 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);

                    p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.Driver);
                    p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);
                    p3.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightRear);
                    p4.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.LeftRear);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                }

                //Star 5 - 80-100% Accuracy - Rifles/Shotguns 
                if (wl == 5)
                {
                    var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p2 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p3 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p4 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);

                    p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.Driver);
                    p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);
                    p3.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightRear);
                    p4.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.LeftRear);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                }

                //Star 5 - 80-100% Accuracy - Rifles/Shotguns 
                if (wl == 6)
                {
                    var p1 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p2 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p3 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);
                    var p4 = Helpers.SpawnPed(PED_MODELS[Rand.Next(0, PED_MODELS.Length)], pos2 * 4, log);

                    p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.Driver);
                    p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);
                    p3.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightRear);
                    p4.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.LeftRear);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(130, 150), Rand.Next(100, 150));
                }
            }
        }
    }
}