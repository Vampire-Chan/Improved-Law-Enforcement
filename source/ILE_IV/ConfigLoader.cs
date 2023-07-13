using IVSDKDotNet;
using IVSDKDotNet.Enums;
using static IVSDKDotNet.Native.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ILE_IV
{
    public class ConfigLoader : Script
    {
        public ConfigLoader()
        {
            
        }

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
        public static string LOGGING_LOCATION;

        public static void LoadValues()
        {
            var IniFile = new IniFile(".scripts/ILE.ini");
            LOGGING_LOCATION = "./scripts/ILE.log";

            //Officers and Soldiers....
            POLICE_OFFICERS = Helpers.ToArray(IniFile.GetValue("POLICE OFFICERS", "UNITS", "M_Y_COP, F_Y_COP, M_M_FATCOP_01, M_Y_COP_TRAFFIC"));
            POLICE_OFFICERS_SAHP = Helpers.ToArray(IniFile.GetValue("HIGHWAY POLICE OFFICERS", "UNITS", "S_M_Y_COP_01, S_F_Y_COP_01"));
            POLICE_OFFICERS_LSSD = Helpers.ToArray(IniFile.GetValue("SHERIFF DEPUTIES", "UNITS", "S_M_Y_SHERIFF_01, S_F_Y_SHERIFF_01"));
            POLICE_OFFICERS_BCSO = Helpers.ToArray(IniFile.GetValue("BLAINE SHERIFF DEPUTIES", "UNITS", "S_M_Y_COP_01, S_F_Y_COP_01, S_M_Y_SHERIFF_01, S_F_Y_SHERIFF_01"));
            POLICE_OFFICERS_SAPR = Helpers.ToArray(IniFile.GetValue("PARK RANGERS", "UNITS", "S_M_Y_RANGER_01, S_F_Y_RANGER_01"));
            NOOSE_SOLDIERS = Helpers.ToArray(IniFile.GetValue("NOOSE SOLDIERS", "UNITS", "M_Y_SWAT, M_Y_MILMAN_01"));
            FIB_OFFICERS = Helpers.ToArray(IniFile.GetValue("FIB OFFICERS", "UNITS", "S_M_Y_SWAT_01, S_M_Y_SWAT_01"));
            IAA_OFFICERS = Helpers.ToArray(IniFile.GetValue("IAA OFFICERS", "UNITS", "S_M_Y_SWAT_01, S_M_Y_SWAT_01"));
            MARINE_SOLDIERS = Helpers.ToArray(IniFile.GetValue("MARINE SOLDIERS", "UNITS", "S_M_Y_SWAT_01, S_M_Y_SWAT_01"));
            MERRYW_SOLDIERS = Helpers.ToArray(IniFile.GetValue("MERRYWEATHER SOLDIERS", "UNITS", "S_M_Y_SWAT_01, S_M_Y_SWAT_01"));
            POLICE_DOGS = Helpers.ToArray(IniFile.GetValue("POLICE DOGS", "UNITS", "S_M_Y_SWAT_01, S_M_Y_SWAT_01"));
            LIFEGUARDS = Helpers.ToArray(IniFile.GetValue("LIFE GUARDS", "UNITS", "S_M_Y_SWAT_01, S_M_Y_SWAT_01"));
            COASTGUARDS = Helpers.ToArray(IniFile.GetValue("COAST GUARDS", "UNITS", "S_M_Y_SWAT_01, S_M_Y_SWAT_01"));

            //Vehicles.....
            VEHICLES = Helpers.ToArray(IniFile.GetValue("UNIVERSALLY USABLE", "VEHICLES", "POLICET, RIOT, INSURGENT2"));
            ARMOURED_VEHICLES = Helpers.ToArray(IniFile.GetValue("ARMOURED VEHICLES", "VEHICLES", "BARRAGE, RIOT2, INSURGENT"));
            HELICOPTERS = Helpers.ToArray(IniFile.GetValue("UNIVERSALLY USABLE HELI", "VEHICLES", "ANNIHILATOR, ANNIHILATOR2, BUZZARD2"));
            FIGHTER_PLANES = Helpers.ToArray(IniFile.GetValue("FIGHTERS", "VEHICLES", "LAZER, HYDRA"));
            ATTACK_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("ATTACK HELI", "VEHICLES", "HUNTER, SAVAGE, BUZZARD"));
            POLICE_HELICOPERS = Helpers.ToArray(IniFile.GetValue("POLICE HELI", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            POLICE_VEHICLES = Helpers.ToArray(IniFile.GetValue("POLICE CARS", "VEHICLES", "POLICE, POLICE2, POLICE3, POLICE4, POLICE5, POLICE6, POLICE7, POLPAT, NOOSE, POLSUV"));
            POLICE_VEHICLES_BCSO = Helpers.ToArray(IniFile.GetValue("BLAINE SHERIFF CARS", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            POLICE_VEHICLES_LSSD = Helpers.ToArray(IniFile.GetValue("SHERIFF CARS", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            POLICE_VEHICLES_SAPR = Helpers.ToArray(IniFile.GetValue("PARK RANGER CARS", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            POLICE_VEHICLES_SAHP = Helpers.ToArray(IniFile.GetValue("HIGHWAY PATROL CARS", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            NOOSE_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("NOOSE HELI", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            NOOSE_VEHICLES = Helpers.ToArray(IniFile.GetValue("NOOSE VEHICLES", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            FIB_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("FIB HELI", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            FIB_VEHICLES = Helpers.ToArray(IniFile.GetValue("FIB CARS", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            IAA_VEHICLES = Helpers.ToArray(IniFile.GetValue("IAA CARS", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            IAA_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("IAA HELI", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            MARINE_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("MARINE HELI", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            MARINE_VEHICLES = Helpers.ToArray(IniFile.GetValue("MARINE VEHICLES", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            MERRYW_HELICOPTERS = Helpers.ToArray(IniFile.GetValue("MERRYWEATHER HELI", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            MERRYW_VEHICLES = Helpers.ToArray(IniFile.GetValue("MERRYWEATHER VEHICLES", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            COASTGUARD_VEHICLES = Helpers.ToArray(IniFile.GetValue("COAST GUARD VEHICLES", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));
            LIFEGUARD_VEHICLES = Helpers.ToArray(IniFile.GetValue("LIFE GUARD VEHICLES", "VEHICLES", "POLMAV, ANNIHILATOR, FROGGER"));

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
    }
}