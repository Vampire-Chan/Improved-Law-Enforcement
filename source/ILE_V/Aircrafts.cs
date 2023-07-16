using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ILE_V
{
    public class Aircrafts : Script
    {
        public Aircrafts()
        {
            Tick += OnTick;
        }

        private bool helialive = false;
        private bool tacthelialive = false;
        private bool planealive = false;

        Vehicle heli;
        Vehicle tacticalheli;
        Vehicle plane;

        Ped helipilot;

        public void OnTick(object sender, EventArgs e)
        {
            //Half Implementation 
            var wl = Game.Player.WantedLevel;

            if (wl >= 3)
            {
                //OfficerHeli();
            }
            if (wl >= 4)
            {
                //TacticalHeli();
            }
            if (wl == 5)
            {
                //Planes();
            }
        }

        private Random rand = new Random();

        //Only FIB/IAA/POLICE and LOCAL POLICE
        //For now IAA Officers can use this,
        public void OfficerHeli()
        {
            //Helicopter wasn't created or it was destroyed in action or flew away.
            //We call this code.
            if (helialive == false)
            {
                heli = Helpers.SpawnVehicle(ConfigLoader.HELICOPTERS[rand.Next(0, ConfigLoader.HELICOPTERS.Length)]);

                heli.LandingGearState = VehicleLandingGearState.Retracted;
                Helpers.VehicleModifications(heli, "LSPD");
                for (int i = -1; i < heli.PassengerCapacity; i++)
                {
                    var p1 = Helpers.SpawnPed(ConfigLoader.IAA_OFFICERS[rand.Next(0, ConfigLoader.IAA_OFFICERS.Length)]);
                    p1.Task.WarpIntoVehicle(heli, (VehicleSeat)i);
                }

                Ped[] passengers = heli.Occupants;

                foreach (var passenger in passengers)
                {
                    Helpers.GiveWeaponWithAttachments(passenger, ConfigLoader.IAA_FIB_WEAPON[rand.Next(0, ConfigLoader.IAA_FIB_WEAPON.Length)], ConfigLoader.SIDEARMS[rand.Next(0, ConfigLoader.SIDEARMS.Length)], true);
                    Helpers.PedFunctions(passenger, rand.Next(60, 80), FiringPattern.FullAuto, rand.Next(150, 200), 200);
                    Function.Call<bool>(Hash.CONTROL_MOUNTED_WEAPON, passenger);
                }
                //we get a heli pilot;
                helipilot = heli.Driver;

                //We mark the Heli is alive.
                helialive = true;
            }
            //We check if player wanted level is 0
            //if yes then we make heli run away if heli exists.
            if (Game.Player.WantedLevel == 0)
            {
                if (heli.Exists())
                {
                var driver = heli.Driver;
                driver.Task.FleeFrom(Game.Player.Character, 99999999);
                helialive = false;
                }
            }
            //if heli was destroyed or went null (fleed away)
            if (heli.IsDead == true || heli == null)
            {
                helialive = false;
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
