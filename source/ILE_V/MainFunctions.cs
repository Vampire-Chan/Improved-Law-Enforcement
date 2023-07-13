using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;

namespace ILE_V
{
    public class MainFunctions : Script
    {
        public MainFunctions()
        {
            Tick += OnTick;
            Interval = 50000;
        }

        public void OnTick(object sender, EventArgs e)
        {
            //Scans the PedPool and Stores the peds in array and give them Parachute
            //when in Flying Vehicle.

            Ped[] peds = World.GetAllPeds ();
            foreach (var ped in peds)
            {
                if (ped.IsInFlyingVehicle)
                    ped.SetConfigFlag(188, true);
                    ped.Weapons.Give(WeaponHash.Parachute, 1, false, true);
            }

        }
    }
}