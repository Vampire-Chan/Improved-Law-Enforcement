using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILE_V
{
    public class Aircrafts : Script
    {
        public Aircrafts()
        {
            Tick += OnTick;
        }

        private bool helialive;
        private bool tacthelialive;
        private bool planealive;


        public void OnTick(object sender, EventArgs e)
        {
            var wl = Game.Player.WantedLevel;
            if (wl >= 3)
            {
                OfficerHeli();
            }
            if (wl >= 4)
            {
                TacticalHeli();
            }
            if (wl == 5)
            {
                Planes();
            }
        }

        private Random rand = new Random();

        //Only FIB/IAA/POLICE and LOCAL POLICE
        public void OfficerHeli()
        {
            ConfigLoader.LoadValues();
            if (helialive == false)
            {
                helialive = true;
                Vehicle heli = Helpers.SpawnVehicle(ConfigLoader.HELICOPTERS[rand.Next(0, ConfigLoader.HELICOPTERS.Length)], Game.Player.Character.Position.Around(160) + Vector3.WorldUp * 70, ConfigLoader.DEBUGGING);

                heli.LandingGearState = VehicleLandingGearState.Retracted;
                Helpers.VehicleModifications(heli, "LSPD", ConfigLoader.DEBUGGING);
                for (int i = -1; i < heli.PassengerCapacity; i++)
                {
                    var p1 = Helpers.SpawnPed(ConfigLoader.IAA_OFFICERS[rand.Next(0, ConfigLoader.IAA_OFFICERS.Length)], Game.Player.Character.ForwardVector * 160, ConfigLoader.DEBUGGING);
                    p1.Task.WarpIntoVehicle(heli, (VehicleSeat)i);
                }
                Ped[] passengers = heli.Occupants;
                foreach (var passenger in passengers)
                {
                    Helpers.GiveWeaponWithAttachments(passenger, ConfigLoader.IAA_FIB_WEAPON[rand.Next(0, ConfigLoader.IAA_FIB_WEAPON.Length)], ConfigLoader.SIDEARMS[rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                    Helpers.PedFunctions(passenger, rand.Next(60, 80), FiringPattern.FullAuto, rand.Next(150, 200), 200);
                    Function.Call<bool>(Hash.CONTROL_MOUNTED_WEAPON, passenger);
                    

                    if (Game.Player.WantedLevel == 0)
                    {
                        var driver = heli.Driver;
                        driver.Task.FleeFrom(Game.Player.Character, 99999999);
                        helialive = false;
                    }
                    if (heli.IsDead ==true || heli ==null)
                    {
                        helialive = false;
                    }
                }
            }
        }
        //NOOSE/Merryweather and Marines
        public void TacticalHeli()
        {
        
        }
        //Marines and Merryweather Deployer
        public void Planes()
        { 
        
        }
    }
}
