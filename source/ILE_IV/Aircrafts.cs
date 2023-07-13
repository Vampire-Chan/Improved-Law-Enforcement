using IVSDKDotNet;
using IVSDKDotNet.Enums;
using static IVSDKDotNet.Native.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILE_IV
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
        {/*
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
            }*/
        }

        private Random rand = new Random();

        //Only FIB/IAA/POLICE and LOCAL POLICE
        public void OfficerHeli()
        {
            
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
