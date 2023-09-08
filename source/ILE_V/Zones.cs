using GTA.Math;
using GTA.Native;
using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ILE_V
{
    public class Zones : Script
    {
        public static string[] CURRENT_ZONE;

        public static string[] LSPD = 
        {
        "FRANI", "BEECW", "BEGGA", "BOAB", "BOTU", "BOULE", "BRBRO", "BRALG", "BREBB", "BRDBB",
        "NOWOB", "HIBRG", "CERHE", "CHAPO", "CITH", "DOWTW", "ESHOO", "EISLC", "FIREP", "FORSI",
        "FIISL", "HOBEH", "HAPIN", "INSTI", "LTBAY", "LEAPE", "MEADH", "MEADP", "NRTGA", "OUTL",
        "ALSCF", "ROTTH", "SCHOL", "SUTHS", "STHBO", "STARJ", "STEIN", "WILLI", "ACTRR", "ACTIP",
        "ALDCI", "BERCH", "CASGR", "CASGC", "CHITO", "CHISL", "COISL", "EASON", "EAHOL", "THXCH",
        "FISSN", "FISSO", "HATGA", "LITAL", "LANCA", "LANCE", "LEFWO", "LOWEA", "MIDPW", "TMEQU",
        "MIDPE", "MIDPA", "NOHOL", "NORMY", "NORWO", "PORTU", "THPRES", "PUGAT", "SUFFO", "THTRI",
        "TUDOR", "VASIH", "WESDY", "WESMI", "BANHAMCA", "ALTA", "BANNING", "BURTON", "CHAMH", "CYPRE",
        "DAVIS", "DELPE", "DELBE", "DOWNT", "DTVINE", "EAST_V", "golf", "HAWICK", "LMESA", "LOSPUER",
        "LEGSQU", "KOREAT", "STAD", "MIRR", "SKID", "MORN", "MURRI", "PBLUFF", "PBOX", "ZP_ORT",
        "DELSOL", "RANCHO", "MOVIE", "RICHM", "ROCKF", "STRAW", "TERMINA", "TEXTI", "TONGVAH", "TONGVAV",
        "VESP", "BEACH", "VCANA", "VINE", "HORS", "WVINE", "OBSERV"
        };

        public static string[] LSSD = 
        {
        "BHAMCA", "BANHAMC", "CHU", "ZQ_UAR", "EBURO", "DESRT", "GREATC", "LDAM", "LACT", "PALMPOW",
        "RGLEN", "WINDF", "TONGVAV", "CHIL", "BAYTRE", "GALLI"
        };

        public static string[] BCSO = 
        {
        "BRADP", "ELGORL", "GALFISH", "GRAPES", "HARMO", "LAGO", "MTJOSE", "NCHU", "PALETO", "PALCOV",
        "PROCOB", "SANCHIA", "SANDY", "SLAB", "BRADT", "ZANCUDO"
        };

        public static string[] SAPR = 
        { 
        "CALAFB", "CCREAK", "CMSW", "MTCHIL", "MTGORDO", "PALFOR", "RTRAK", "TATAMO", "PALHIGH", "CANNY" 
        };

        public static string[] SAHP = 
        { 
        "Route 68", "Olympic Fwy", "Del Perro Fwy", "Senora Fwy", "Great Ocean Hwy", "La Puerta Fwy", "Los Santos Freeway", "Elysian Fields Fwy", "Palomino Fwy" };

        public static string[] NOOSEHQ =  { "NOOSE" };

        public static string[] BEACH =  { "OCEANA", "SanAnd" };

        public static string[] SASPA = { "JAIL" };

        public static string[] ZANCUDO = { "ARMYB", "LAGO", "ZANCUDO" };

        public static string[] ALAMO = { "ALAMO" };

        public static string[] MERRYWEATHER = { "HUMLAB", "ELYSIAN" };

        public static string[] LSIA = { "AIRP" };

        public static string[] FIB_IAA = { };

        public static string[] GetJurisdiction(Vector3 zone)
        {
            string value = Function.Call<string>(Hash.GET_NAME_OF_ZONE, new InputArgument[3] { zone.X, zone.Y, zone.Z });
            string streetName = World.GetStreetName(Game.Player.Character.Position);

            if (ALAMO.Contains(value))
            {
                return ALAMO;
            }
            if (SASPA.Contains(value))
            {
                return SASPA;
            }
            if (ZANCUDO.Contains(value))
            {
                return ZANCUDO;
            }
            if (BEACH.Contains(value))
            {
                return BEACH;
            }
            if (NOOSEHQ.Contains(value))
            {
                return NOOSEHQ;
            }
            if (MERRYWEATHER.Contains(value))
            {
                return MERRYWEATHER;
            }
            if (LSIA.Contains(value))
            {
                return LSIA;
            }
            if (SAPR.Contains(value))
            {
                return SAPR;
            }
            if (SAHP.Contains(streetName))
            {
                return SAHP;
            }
            if (BCSO.Contains(value))
            {
                return BCSO;
            }
            if (LSSD.Contains(value))
            {
                return LSSD;
            }
            if (LSPD.Contains(value))
            {
                return LSPD;
            }
            return LSPD;
        }
    }
}
