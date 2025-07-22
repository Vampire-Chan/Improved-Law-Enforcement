using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static VehicleExtensions;

public class ModCycler : Script
{
    private Vehicle currentVehicle;
    private List<ModType> modTypes;
    private int currentModIndex = 0;
    private int currentModValue = 0;

    public ModCycler()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
        Interval = 0;

        modTypes = new List<ModType>();

        // Load all mod types except toggles
        foreach (ModType mod in Enum.GetValues(typeof(ModType)))
        {
            if (!IsToggleMod(mod)) modTypes.Add(mod);
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        if (!Game.Player.Character.IsInVehicle()) return;

        var vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle != currentVehicle)
        {
            currentVehicle = vehicle;
            currentModIndex = 0;
            currentModValue = 0;
            HelperClass.Subtitle($"[ModCycler] Vehicle locked. {modTypes.Count} mods available.");
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (currentVehicle == null || !currentVehicle.Exists()) return;

        if (e.KeyCode == Keys.Left)
        {
            CycleToNextModType();
        }
        else if (e.KeyCode == Keys.Right)
        {
            CycleToNextModValue();
        }
    }

    private void CycleToNextModType()
    {
        currentModIndex = (currentModIndex + 1) % modTypes.Count;
        currentModValue = 0;

        var mod = modTypes[currentModIndex];
        ApplyMod(currentVehicle, mod, currentModValue);

        HelperClass.Subtitle($"[ModCycler] Mod: {mod.ToStringValue()} | Set to 0");
    }

    private void CycleToNextModValue()
    {
        var mod = modTypes[currentModIndex];
        if (!TryConvertToVehicleModType(mod, out ModType gtaModType))
        {
            HelperClass.Subtitle($"[ModCycler] Skipped invalid mod: {mod}");
            return;
        }

        int max = currentVehicle.GetModCount(gtaModType);
        if (max <= 0)
        {
            HelperClass.Subtitle($"[ModCycler] {mod.ToStringValue()} has 0 options.");
            return;
        }

        currentModValue = (currentModValue + 1) % max;
        currentVehicle.SetVehicleMod(gtaModType, currentModValue, false);

        HelperClass.Subtitle($"[ModCycler] {mod.ToStringValue()} → {currentModValue}/{max - 1}");
    }

    private void ApplyMod(Vehicle vehicle, ModType mod, int index)
    {
        if (!TryConvertToVehicleModType(mod, out ModType gtaModType)) return;

        int count = vehicle.GetModCount(gtaModType);
        if (count <= 0) return;

        vehicle.SetVehicleMod(gtaModType, index % count, false);
    }

    private bool TryConvertToVehicleModType(ModType modType, out ModType gtaModType)
    {
        return Enum.TryParse(modType.ToString(), out gtaModType);
    }

    private bool IsToggleMod(ModType mod)
    {
        return mod.ToString().StartsWith("Toggle");
    }
}
