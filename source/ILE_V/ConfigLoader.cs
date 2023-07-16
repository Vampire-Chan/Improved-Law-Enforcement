using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ILE_V
{
    public class ConfigLoader : Script
    {

        public static int SPAWN_GAP;

        public static bool DEBUGGING;
        //Peds
        public static string[] NOOSE_SOLDIERS;
        public static string[] MARINE_SOLDIERS;
        public static string[] MERRYW_SOLDIERS;
        public static string[] IAA_OFFICERS;
        public static string[] FIB_OFFICERS;
        public static string[] POLICE_OFFICERS;
        public static string[] POLICE_OFFICERS_LSSD;
        public static string[] POLICE_OFFICERS_BCSO;
        public static string[] POLICE_OFFICERS_SAPR;
        public static string[] POLICE_OFFICERS_SAHP;
        public static string[] POLICE_DOGS;
        public static string[] LIFEGUARDS;
        public static string[] COASTGUARDS;

        //Vehicles and Helicopters
        public static string[] POLICE_HELICOPERS;
        public static string[] POLICE_VEHICLES;
        public static string[] POLICE_VEHICLES_LSSD;
        public static string[] POLICE_VEHICLES_BCSO;
        public static string[] POLICE_VEHICLES_SAPR;
        public static string[] POLICE_VEHICLES_SAHP;
        public static string[] HELICOPTERS;
        public static string[] VEHICLES;
        public static string[] FIB_VEHICLES;
        public static string[] FIB_HELICOPTERS;
        public static string[] IAA_HELICOPTERS;
        public static string[] IAA_VEHICLES;
        public static string[] NOOSE_HELICOPTERS;
        public static string[] MARINE_HELICOPTERS;
        public static string[] NOOSE_VEHICLES;
        public static string[] MARINE_VEHICLES;
        public static string[] ATTACK_HELICOPTERS;
        public static string[] FIGHTER_PLANES;
        public static string[] ARMOURED_VEHICLES;
        public static string[] MERRYW_VEHICLES;
        public static string[] MERRYW_HELICOPTERS;
        public static string[] LIFEGUARD_VEHICLES;
        public static string[] COASTGUARD_VEHICLES;


        //Weapons and Explosives
        public static string[] POLICE_WEAPON_HEAVY; //Star 4 amd 5
        public static string[] POLICE_WEAPON_LIGHT; //Star 2 and 3
        public static string[] NOOSE_WEAPON;
        public static string[] MARINE_WEAPON;
        public static string[] MARINE_EXPLOSIVES;
        public static string[] MERRYW_WEAPON;
        public static string[] MERRYW_EXPLOSIVES;
        public static string[] IAA_FIB_WEAPON;

        public static string[] SIDEARMS;

        public static bool hasloaded;
		
		public ConfigLoader()
        {
            Tick += LoadValues;
            var IniFile = new IniFile("./scripts/ILE_V.ini");

            //Officers and Soldiers....
            POLICE_OFFICERS = Helpers.ToArray(IniFile.GetValue("POLICE OFFICERS", "UNITS", "S_M_Y_COP_01, S_F_Y_COP_01"));
            POLICE_OFFICERS_SAHP = Helpers.ToArray(IniFile.GetValue("HIGHWAY POLICE OFFICERS", "UNITS", "S_M_Y_COP_01, S_F_Y_COP_01, S_M_Y_HWayCOP_01"));
            POLICE_OFFICERS_LSSD = Helpers.ToArray(IniFile.GetValue("SHERIFF DEPUTIES", "UNITS", "S_M_Y_SHERIFF_01, S_F_Y_SHERIFF_01"));
            POLICE_OFFICERS_BCSO = Helpers.ToArray(IniFile.GetValue("BLAINE SHERIFF DEPUTIES", "UNITS", "S_M_Y_COP_01, S_F_Y_COP_01, S_M_Y_SHERIFF_01, S_F_Y_SHERIFF_01"));
            POLICE_OFFICERS_SAPR = Helpers.ToArray(IniFile.GetValue("PARK RANGERS", "UNITS", "S_M_Y_RANGER_01, S_F_Y_RANGER_01"));
            NOOSE_SOLDIERS = Helpers.ToArray(IniFile.GetValue("NOOSE SOLDIERS", "UNITS", "S_M_Y_SWAT_01, S_M_Y_SWAT_01"));
            FIB_OFFICERS = Helpers.ToArray(IniFile.GetValue("FIB OFFICERS", "UNITS", "s_m_m_fibsec_01, s_m_m_fiboffice_01, s_m_m_fiboffice_02"));
            IAA_OFFICERS = Helpers.ToArray(IniFile.GetValue("IAA OFFICERS", "UNITS", "s_m_m_ciasec_01, S_M_Y_SWAT_01"));
            MARINE_SOLDIERS = Helpers.ToArray(IniFile.GetValue("MARINE SOLDIERS", "UNITS", "s_m_m_marine_01, s_m_m_marine_02, s_m_y_marine_01, s_m_y_marine_02, s_m_y_marine_03"));
            MERRYW_SOLDIERS = Helpers.ToArray(IniFile.GetValue("MERRYWEATHER SOLDIERS", "UNITS", "s_m_y_blackops_01, s_m_y_blackops_02, s_m_y_blackops_03, s_m_m_chemsec_01"));
            POLICE_DOGS = Helpers.ToArray(IniFile.GetValue("POLICE DOGS", "UNITS", "A_C_shepherd, A_C_Retriever, A_C_Rottweiler, A_C_Husky"));
            LIFEGUARDS = Helpers.ToArray(IniFile.GetValue("LIFE GUARDS", "UNITS", "s_f_y_baywatch_01, s_m_y_baywatch_01"));
            COASTGUARDS = Helpers.ToArray(IniFile.GetValue("COAST GUARDS", "UNITS", "s_m_y_uscg_01, s_m_y_baywatch_01, s_m_y_cop_01"));

            //Vehicles.....
            VEHICLES = Helpers.ToArray(IniFile.GetValue("UNIVERSALLY USABLE", "VEHICLES", "POLICET, RIOT, INSURGENT2"));
            ARMOURED_VEHICLES = Helpers.ToArray(IniFile.GetValue("ARMOURED VEHICLES", "VEHICLES", "BARRAGE, RIOT2, INSURGENT"));
            HELICOPTERS = Helpers.ToArray(IniFile.GetValue("UNIVERSALLY USABLE HELI", "VEHICLES", "ANNIHILATOR, ANNIHILATOR2, BUZZARD2, POLMAV"));
            FIGHTER_PLANES = Helpers.ToArray(IniFile.GetValue("FIGHTERS", "VEHICLES", "LAZER, HYDRA"));
            ATTACK_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("ATTACK HELI", "VEHICLES", "HUNTER, SAVAGE, BUZZARD"));
            POLICE_HELICOPERS = Helpers.ToArray(IniFile.GetValue("POLICE HELI", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            POLICE_VEHICLES = Helpers.ToArray(IniFile.GetValue("POLICE CARS", "VEHICLES", "POLICE, POLICE2, POLICE3, POLICET"));
            POLICE_VEHICLES_BCSO = Helpers.ToArray(IniFile.GetValue("BLAINE SHERIFF CARS", "VEHICLES", "SHERIFF, SHERIFF2, GRANGER"));
            POLICE_VEHICLES_LSSD = Helpers.ToArray(IniFile.GetValue("SHERIFF CARS", "VEHICLES", "SHERIFF, SHERIFF2, GRANGER"));
            POLICE_VEHICLES_SAPR = Helpers.ToArray(IniFile.GetValue("PARK RANGER CARS", "VEHICLES", "PRANGER, SHERIFF, SHERIFF2, GRANGER"));
            POLICE_VEHICLES_SAHP = Helpers.ToArray(IniFile.GetValue("HIGHWAY PATROL CARS", "VEHICLES", "POLICE2, POLICE, POLICEB"));
            NOOSE_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("NOOSE HELI", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER, ANNIHILATOR2, BUZZARD, BUZZARD2"));
            NOOSE_VEHICLES = Helpers.ToArray(IniFile.GetValue("NOOSE VEHICLES", "VEHICLES", "RIOT, FBI2, RIOT2, BRICKADE2, BRICKADE, POLICET, INSURGENT, INSURGENT2, INSURGENT3"));
            FIB_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("FIB HELI", "VEHICLES", "BUZZARD, ANNIHILATOR, FROGGER"));
            FIB_VEHICLES = Helpers.ToArray(IniFile.GetValue("FIB CARS", "VEHICLES", "FBI, FBI2, POLICE4, CHEETAH, INFERNUS, COMET, TURISMO"));
            IAA_VEHICLES = Helpers.ToArray(IniFile.GetValue("IAA CARS", "VEHICLES", "FBI, FBI2, POLICE4, CHEETAH, INFERNUS, COMET, TURISMO"));
            IAA_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("IAA HELI", "VEHICLES", "BUZZARD, ANNIHILATOR, FROGGER"));
            MARINE_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("MARINE HELI", "VEHICLES", "ANNIHILATOR2, ANNIHILATOR, BUZZARD, VALKYRIE, VALKYRIE2, SAVAGE, CARGOBOB, "));
            MARINE_VEHICLES = Helpers.ToArray(IniFile.GetValue("MARINE VEHICLES", "VEHICLES", "CRUSADER, INSURGENT, INSURGENT2, INSURGENT3, BARRAGE, TECHNICAL, TECHNICAL2, BARRACKS, BARRACKS2, BARRACKS3, WINKY"));
            MERRYW_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("MERRYWEATHER HELI", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            MERRYW_VEHICLES = Helpers.ToArray(IniFile.GetValue("MERRYWEATHER VEHICLES", "VEHICLES", "MESA3, CRUSADER, INSURGENT"));
            COASTGUARD_VEHICLES = Helpers.ToArray(IniFile.GetValue("COAST GUARD VEHICLES", "VEHICLES", "LGUARD, BLAZER, POLICE"));
            LIFEGUARD_VEHICLES = Helpers.ToArray(IniFile.GetValue("LIFE GUARD VEHICLES", "VEHICLES", "LGUARD, BLAZER, POLICE"));

            //Weapons.....
            POLICE_WEAPON_LIGHT = Helpers.ToArray(IniFile.GetValue("POLICE LIGHT WEAPONS", "WEAPONS", "WEAPON_PUMPSHOTGUN, WEAPON_MICROSMG, WEAPON_SMG"));
            POLICE_WEAPON_HEAVY = Helpers.ToArray(IniFile.GetValue("POLICE HEAVY WEAPONS", "WEAPONS", "WEAPON_CARBINERIFLE, WEAPON_SPECIALCARBINE, WEAPON_SMG"));
            NOOSE_WEAPON = Helpers.ToArray(IniFile.GetValue("NOOSE WEAPONS", "WEAPONS", "WEAPON_PUMPSHOTGUN, WEAPON_MICROSMG, WEAPON_SMG"));
            IAA_FIB_WEAPON = Helpers.ToArray(IniFile.GetValue("FIB/IAA WEAPONS", "WEAPONS", "WEAPON_PUMPSHOTGUN, WEAPON_MICROSMG, WEAPON_SMG"));
            MERRYW_WEAPON = Helpers.ToArray(IniFile.GetValue("MERRYWEATHER WEAPONS", "WEAPONS", "WEAPON_PUMPSHOTGUN, WEAPON_MICROSMG, WEAPON_SMG"));
            MERRYW_EXPLOSIVES = Helpers.ToArray(IniFile.GetValue("MERRYWEATHER EXPLOSIVE WEAPONS", "WEAPONS", "WEAPON_GRENADELAUNCHER, WEAPON_RPG, WEAPON_GRENADELAUNCHER_SMOKE, WEAPON_BZGAS"));
            MARINE_WEAPON = Helpers.ToArray(IniFile.GetValue("MARINE WEAPONS", "WEAPONS", "WEAPON_PUMPSHOTGUN, WEAPON_MICROSMG, WEAPON_SMG"));
            MARINE_EXPLOSIVES = Helpers.ToArray(IniFile.GetValue("MARINE EXPLOSIVE WEAPONS", "WEAPONS", "WEAPON_GRENADELAUNCHER, WEAPON_RPG, WEAPON_GRENADELAUNCHER_SMOKE, WEAPON_BZGAS"));

            SPAWN_GAP = IniFile.GetInteger("SETTINGS", "SPAWNING GAP", 20000);
            SIDEARMS = Helpers.ToArray(IniFile.GetValue("SIDEARMS", "WEAPONS", "WEAPON_PISTOL, WEAPON_COMBATPISTOL, WEAPON_APPISTOL, WEAPON_SNS"));
            DEBUGGING = IniFile.GetBoolean("DEBUGGING", "SETTINGS", true);

        }

        public static void LoadValues(object sender, EventArgs e)
        {
            
               
        }
    }
}