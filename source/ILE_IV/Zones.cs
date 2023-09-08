using IVSDKDotNet;
using IVSDKDotNet.Enums;
using static IVSDKDotNet.Native.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using CCL.GTAIV;

namespace ILE_IV
{
	//WIP to do with Zone Names
    public class Zones : Script
    {
        public static string[] CURRENT_ZONE;

        public static string[] Alderney = 
        {
			
		};

        public static string[] Algonquin = 
        {
			
		};

        public static string[] Dukes = 
        {
			
		};

        public static string[] Broker = 
        { 
		
		};

        public static string[] Bohan = 
        { 
		
		};
		
        public static string[] NOOSEHQ =  { "NOOSE" };

        public static string[] LibertyCity =  {  };

        public static string[] ASCF = { "JAIL" };

        public static string[] FIBHQ = {  };

        public static string[] ColonyIsland = {  };

        public static string[] ChargeIsland = {  };

        public static string[] FIA = { "AIRP" };

        public Zones()
        {
            Tick += OnTick;
        }
        public void OnTick(object sender, EventArgs e)
        {
           
        }

        public static string[] GetJurisdiction(Vector3 zone)
        {
            string value = NativeWorld.GetZoneName(zone);

            if (ColonyIsland.Contains(value))
            {
                return ColonyIsland;
            }
            if (ASCF.Contains(value))
            {
                return ASCF;
            }
            if (FIBHQ.Contains(value))
            {
                return FIBHQ;
            }
            if (LibertyCity.Contains(value))
            {
                return LibertyCity;
            }
            if (NOOSEHQ.Contains(value))
            {
                return NOOSEHQ;
            }
            if (ChargeIsland.Contains(value))
            {
                return ChargeIsland;
            }
            if (FIA.Contains(value))
            {
                return FIA;
            }
            if (Broker.Contains(value))
            {
                return Broker;
            }
            if (Dukes.Contains(value))
            {
                return Dukes;
            }
            if (Algonquin.Contains(value))
            {
                return Algonquin;
            }
            if (Alderney.Contains(value))
            {
                return Alderney;
            }
            return Alderney;
        }
    }
}
