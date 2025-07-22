using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Screen = GTA.UI.Screen;

namespace DispatchSystem
{
    public class MainClass : Script
    {
        private DispatchManager _dispatchManager;
        private static bool hasLoaded = false;
        public static List<Model> BlacklistedPeds { get; set; }
        public static bool log = false;

        public MainClass()
        {
            try
            {
                _dispatchManager = new DispatchManager();
                Tick += OnTick;
                Aborted += OnAborted;

                LoadSettings(); // Preload settings
                hasLoaded = true;

                Logger.Log.Info("Dispatch System Initialized");
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal($"Error initializing Dispatch System: {ex.Message}");
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                _dispatchManager?.Update();

                DisarmNearbyPeds(); // Optional, for constant monitoring
            }

            
            catch (Exception ex)
            {
                Logger.Log.Fatal($"Tick error: {ex.Message}");
            }
        }

        private void OnAborted(object sender, EventArgs e)
        {
            try
            {
                _dispatchManager?.Cleanup();
                Logger.Log.Info("Dispatch System Cleaned Up");
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal($"Cleanup error: {ex.Message}");
            }
        }

        private void DisarmNearbyPeds()
        {
            try
            {
                if (!hasLoaded)
                {
                    LoadSettings();
                    hasLoaded = true;
                }

                Ped[] nearbyPeds = World.GetNearbyPeds(Game.Player.Character.Position, 500f);
                foreach (Ped ped in nearbyPeds)
                {
                    DisarmPed(ped);
                }
                DisarmPed(Game.Player.Character);

            }
            catch (Exception ex)
            {
                Logger.Log.Fatal($"DisarmNearbyPeds Error: {ex.Message}");
            }
        }

        private void LoadSettings()
        {
            string iniPath = "./scripts/WOI/Disarm.ini";

            try
            {
                // Auto-generate if not found
                if (!File.Exists(iniPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(iniPath));

                    ScriptSettings set = ScriptSettings.Load(iniPath);
                    set.SetValue<bool>("Settings", "Logging", false);
                    set.SetValue<string>("Blacklisted Peds", "Ped Models", "s_m_y_juggernaut_01, testname" );
                    set.Save();
                    Logger.Log.Info("Disarm.ini not found. Default created.");
                }
                else
                {
                    Logger.Log.Info("Disarm.ini Found. Setting Up Values.");
                }

                ScriptSettings settings = ScriptSettings.Load(iniPath);

                log = settings.GetValue("Settings", "Logging", false);
                Logger.Log.Info(log ? "Logging is Enabled." : "Logging is Disabled.");

                string[] models = ReadModels(settings.GetValue("Blacklisted Peds", "Ped Models", ""));
                BlacklistedPeds = models.Select(m => new Model(m)).ToList();

                Logger.Log.Info($"Blacklisted Models: {string.Join(", ", models)}");
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal($"LoadSettings Error: {ex.Message}");
            }
        }


        private string[] ReadModels(string input)
        {
            return input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .ToArray();
        }

        private void DisarmPed(Ped ped)
        {
            try
            {
                if (ped == null || !ped.Exists() || ped.IsDead) return;

                if (BlacklistedPeds.Any(model => model.Hash == ped.Model.Hash))
                {
                    return;
                }

                int boneId = ped.GetLastDamageBone();

                if (IsHitOnArm(boneId))
                {
                    ped.PlayAmbientSpeech("GENERIC_CURSE_MED", false);
                    ped.ClearLastDamageBone();
                    ped.ClearLastWeaponDamage();

                    if (ped == Game.Player.Character)
                    {
                        // Player: Pain reaction first, then drop weapon
                        ped.CanSufferCriticalHits = true;
                        ped.DiesOnLowHealth = false;
                        ped.CanWrithe = false;
                        ped.CanRagdoll = true;

                        // Clear ragdoll blocking for this specific hit
                        ped.ClearRagdollBlockingFlags(
                            RagdollBlockingFlags.BulletImpact |
                            RagdollBlockingFlags.Electrocution |
                            RagdollBlockingFlags.Melee
                        );

                        // Brief stumble/pain reaction
                        ped.Ragdoll(300, RagdollType.Balance);

                        // Drop weapon after reaction
                       
                            if (ped.Exists()) ped.Weapons.Drop();
                       
                    }
                    else
                    {
                        // NPCs: Standard behavior - drop weapon immediately
                        ped.Weapons.Drop();
                        ped.Ragdoll(200, RagdollType.Balance);
                    }

                    //if(ped.HasBeenDamagedByWeapon(WeaponHash))

                    if (log)
                        Logger.Log.Info($"Disarmed {(ped.IsPlayer ? "Player" : "Ped")} {ped.Handle} (Bone: {boneId}) at {ped.Position}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Warning($"DisarmPed Error: {ex.Message}");
            }
        }


        private bool IsHitOnArm(int boneId)
        {
            int[] armBones = { 18905, 57005, 28252, 61163, 40269, 45509 };
            return armBones.Contains(boneId);
        }
    }
}


//public class CargobobTroopSpawner : Script
//{
//    private Vehicle cargobob;
//    private Model pedModel = new Model(PedHash.Marine01SMY);
//    private List<Ped> spawnedPeds = new();
//    private Keys spawnKey = Keys.G;
//    private Keys doorKey = Keys.J;

//    public CargobobTroopSpawner()
//    {
//        Tick += OnTick;
//        KeyDown += OnKeyDown;
//        Aborted += OnAbort;
//        HelperClass.Notification("~g~Cargobob Spawner Ready! ~w~G: Spawn Heli/Peds, J: Open Door");
//    }

//    private void OnTick(object sender, EventArgs e)
//    {
//        if (cargobob != null && cargobob.Exists())
//        {
//            string status = Function.Call<bool>(Hash.IS_VEHICLE_DOOR_FULLY_OPEN, cargobob, 2) ? "OPEN" : "CLOSED";
//            Screen.ShowSubtitle("~y~Ramp Door Status: ~w~" + status, 1);
//        }
//    }

//    private void OnKeyDown(object sender, KeyEventArgs e)
//    {
//        if (e.KeyCode == spawnKey)
//        {
//            if (cargobob == null || !cargobob.Exists())
//                SpawnCargobob();
//            else
//                SpawnPedInsideBay();
//        }

//        if (e.KeyCode == doorKey && cargobob != null && cargobob.Exists())
//        {
//            Function.Call(Hash.SET_VEHICLE_DOOR_OPEN, cargobob, 2, false, false); // Open REAR LEFT
//            HelperClass.Notification("~b~Back door opened.");
//        }
//    }

//    private void SpawnCargobob()
//    {
//        Vector3 spawnPos = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 15f;
//        cargobob = World.CreateVehicle(new Model("cargobob"), spawnPos, Game.Player.Character.Heading);
//        cargobob.PlaceOnGround();
//        cargobob.PlaceOnNextStreet();
//       // cargobob.MarkAsNoLongerNeeded();
//        //cargobob.IsPositionFrozen = true;
//        HelperClass.Notification("~g~Cargobob spawned.");
//    }

//    private void SpawnPedInsideBay()
//    {
//        if (cargobob == null || !cargobob.Exists()) return;

//        if (!pedModel.IsLoaded) pedModel.Request(500);

//        // We'll space them out slightly per spawn
//        int index = spawnedPeds.Count;
//        Vector3 offset = new Vector3(0f, -2.0f +0.6f, -0.5f); // inside cargo bay
//        Vector3 spawnPos = cargobob.GetOffsetPosition(offset);

//        Ped ped = World.CreatePed(pedModel, spawnPos);
//        ped.Task.StandStill(10000);
//        ped.BlockPermanentEvents = true;
//        //ped.MarkAsNoLongerNeeded();
//        spawnedPeds.Add(ped);

//        HelperClass.Notification($"~w~Spawned ~y~Trooper #{index + 1} ~w~in cargobob.");
//    }

//    private void OnAbort(object sender, EventArgs e)
//    {
//        foreach (var ped in spawnedPeds)
//        {
//            if (ped != null && ped.Exists()) ped.MarkAsNoLongerNeeded();
//        }

//        if (cargobob != null && cargobob.Exists())
//            cargobob.MarkAsNoLongerNeeded();

//        HelperClass.Notification("~r~Script aborted. All cleaned up.");
//    }
//}
