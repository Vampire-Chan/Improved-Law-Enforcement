using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;
using System;
using CCL.GTAIV;
using System.Numerics;

namespace ILE_IV
{
    public class TacticalSpawn
    {
        public static Random Rand = new Random();

        public static void Tactical_Marines(bool log)
        {
            GET_CHAR_COORDINATES(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out Vector3 pos);
            GET_CHAR_HEADING(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out float heading);

            var pos2 = CWorld.GetNextPositionOnStreet(Helper.GetPositionInFrontOfEntity(pos, Helper.HeadingToDirection(heading), 150));

            var car = Helpers.SpawnVehicle(ConfigLoader.MARINE_VEHICLES[Rand.Next(0, ConfigLoader.MARINE_VEHICLES.Length)], pos2, log);

            if (DOES_VEHICLE_EXIST(car.GetHandle()) && car.GetHandle() != 0)
            {
                var p1 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p2 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p3 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p4 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);

                p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.Driver);
                p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);
                p3.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightRear);
                p4.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.LeftRear);


                int weap_exp = Rand.Next(0, 5);

                //Explosives
                if (weap_exp >= 3 && weap_exp <= 4)
                {
                    Helpers.GiveWeapon(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p2, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], false);
                    Helpers.GiveWeapon(p3, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], true);
                    Helpers.GiveWeapon(p4, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], true);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(200, 250), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(40, 60), 100, (uint)Rand.Next(230, 320), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(20, 80), 100, (uint)Rand.Next(90, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(10, 20), 100, (uint)Rand.Next(30, 150), Rand.Next(100, 150));


                }
                if (weap_exp < 3)
                {
                    Helpers.GiveWeapon(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p2, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p3, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p4, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));

                }
            }
        }

        public static void Tactical_MerryWeather(bool log)
        {
            GET_CHAR_COORDINATES(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out Vector3 pos);
            GET_CHAR_HEADING(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out float heading);

            var pos2 = CWorld.GetNextPositionOnStreet(Helper.GetPositionInFrontOfEntity(pos, Helper.HeadingToDirection(heading), 150));

            var car = Helpers.SpawnVehicle(ConfigLoader.MARINE_VEHICLES[Rand.Next(0, ConfigLoader.MARINE_VEHICLES.Length)], pos2, log);

            if (DOES_VEHICLE_EXIST(car.GetHandle()) && car.GetHandle() != 0)
            {
                var p1 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p2 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p3 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p4 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);

                p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.Driver);
                p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);
                p3.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightRear);
                p4.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.LeftRear);


                int weap_exp = Rand.Next(0, 5);

                //Explosives
                if (weap_exp >= 3 && weap_exp <= 4)
                {
                    Helpers.GiveWeapon(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p2, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], false);
                    Helpers.GiveWeapon(p3, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], true);
                    Helpers.GiveWeapon(p4, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], true);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(200, 250), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(40, 60), 100, (uint)Rand.Next(230, 320), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(20, 80), 100, (uint)Rand.Next(90, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(10, 20), 100, (uint)Rand.Next(30, 150), Rand.Next(100, 150));


                }
                if (weap_exp < 3)
                {
                    Helpers.GiveWeapon(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p2, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p3, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p4, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                }

            }
        }
        public static void Tactical_NOOSE(bool log)
        {
            GET_CHAR_COORDINATES(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out Vector3 pos);
            GET_CHAR_HEADING(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out float heading);

            var pos2 = CWorld.GetNextPositionOnStreet(Helper.GetPositionInFrontOfEntity(pos, Helper.HeadingToDirection(heading), 150));

            var car = Helpers.SpawnVehicle(ConfigLoader.MARINE_VEHICLES[Rand.Next(0, ConfigLoader.MARINE_VEHICLES.Length)], pos2, log);

            if (DOES_VEHICLE_EXIST(car.GetHandle()) && car.GetHandle() != 0)
            {
                var p1 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p2 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p3 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p4 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);

                p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.Driver);
                p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);
                p3.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightRear);
                p4.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.LeftRear);


                int weap_exp = Rand.Next(0, 5);

                //Explosives
                if (weap_exp >= 3 && weap_exp <= 4)
                {
                    Helpers.GiveWeapon(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p2, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], false);
                    Helpers.GiveWeapon(p3, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], true);
                    Helpers.GiveWeapon(p4, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], true);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(200, 250), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(40, 60), 100, (uint)Rand.Next(230, 320), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(20, 80), 100, (uint)Rand.Next(90, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(10, 20), 100, (uint)Rand.Next(30, 150), Rand.Next(100, 150));


                }
                if (weap_exp < 3)
                {
                    Helpers.GiveWeapon(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p2, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p3, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p4, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));

                }

            }
        }

        public static void IAA_Officers(bool log)
        {
            GET_CHAR_COORDINATES(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out Vector3 pos);
            GET_CHAR_HEADING(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out float heading);

            var pos2 = CWorld.GetNextPositionOnStreet(Helper.GetPositionInFrontOfEntity(pos, Helper.HeadingToDirection(heading), 150));

            var car = Helpers.SpawnVehicle(ConfigLoader.MARINE_VEHICLES[Rand.Next(0, ConfigLoader.MARINE_VEHICLES.Length)], pos2, log);

            if (DOES_VEHICLE_EXIST(car.GetHandle()) && car.GetHandle() != 0)
            {
                var p1 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p2 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p3 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p4 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);

                p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.Driver);
                p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);
                p3.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightRear);
                p4.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.LeftRear);


                int weap_exp = Rand.Next(0, 5);

                //Explosives
                if (weap_exp >= 3 && weap_exp <= 4)
                {
                    Helpers.GiveWeapon(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p2, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], false);
                    Helpers.GiveWeapon(p3, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], true);
                    Helpers.GiveWeapon(p4, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], true);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(200, 250), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(40, 60), 100, (uint)Rand.Next(230, 320), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(20, 80), 100, (uint)Rand.Next(90, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(10, 20), 100, (uint)Rand.Next(30, 150), Rand.Next(100, 150));


                }
                if (weap_exp < 3)
                {
                    Helpers.GiveWeapon(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p2, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p3, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p4, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));

                }
            }
        }


        public static void FIB_Officers(bool log)
        {
            GET_CHAR_COORDINATES(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out Vector3 pos);
            GET_CHAR_HEADING(CONVERT_INT_TO_PLAYERINDEX(GET_PLAYER_ID()), out float heading);

            var pos2 = CWorld.GetNextPositionOnStreet(Helper.GetPositionInFrontOfEntity(pos, Helper.HeadingToDirection(heading), 150));

            var car = Helpers.SpawnVehicle(ConfigLoader.MARINE_VEHICLES[Rand.Next(0, ConfigLoader.MARINE_VEHICLES.Length)], pos2, log);

            if (DOES_VEHICLE_EXIST(car.GetHandle()) && car.GetHandle() != 0)
            {
                var p1 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p2 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p3 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);
                var p4 = Helpers.SpawnPed(ConfigLoader.MARINE_SOLDIERS[Rand.Next(0, ConfigLoader.MARINE_SOLDIERS.Length)], pos2 * 4, log);

                p1.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.Driver);
                p2.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightFront);
                p3.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.RightRear);
                p4.GetTaskController().WarpIntoVehicle(car, (uint)GTAEnum.VehicleSeat.LeftRear);


                int weap_exp = Rand.Next(0, 5);

                //Explosives
                if (weap_exp >= 3 && weap_exp <= 4)
                {
                    Helpers.GiveWeapon(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p2, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], false);
                    Helpers.GiveWeapon(p3, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], true);
                    Helpers.GiveWeapon(p4, ConfigLoader.MARINE_EXPLOSIVES[Rand.Next(0, ConfigLoader.MARINE_EXPLOSIVES.Length)], ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], true);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(200, 250), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(40, 60), 100, (uint)Rand.Next(230, 320), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(20, 80), 100, (uint)Rand.Next(90, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(10, 20), 100, (uint)Rand.Next(30, 150), Rand.Next(100, 150));


                }
                if (weap_exp < 3)
                {
                    Helpers.GiveWeapon(p1, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p2, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p3, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);
                    Helpers.GiveWeapon(p4, ConfigLoader.MARINE_WEAPON[Rand.Next(0, ConfigLoader.MARINE_WEAPON.Length)], ConfigLoader.SIDEARMS[Rand.Next(0, ConfigLoader.SIDEARMS.Length)], false);

                    Helpers.PedFunctions(p1, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p2, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p3, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));
                    Helpers.PedFunctions(p4, (byte)Rand.Next(60, 80), 100, (uint)Rand.Next(170, 200), Rand.Next(100, 150));

                }
            }
        }
    }
}
