using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

public class DispatchVehicleInfo
{
    public string Name { get; set; } = string.Empty;
    public List<VehicleData> Vehicles { get; set; } = new List<VehicleData>();
    public List<string> Tasks { get; set; } = new List<string>();
    public List<string> Regions { get; set; } = new List<string>();
    public List<PedData> Pilots { get; set; } = new List<PedData>();
    public List<PedData> Soldiers { get; set; } = new List<PedData>();
    public List<WeaponData> PrimaryWeapons { get; set; } = new List<WeaponData>();
    public List<WeaponData> SecondaryWeapons { get; set; } = new List<WeaponData>();
}

public class WeaponData
{
    public string Name { get; set; } = string.Empty;
    public List<WeaponDetails> Weapons { get; set; } = new List<WeaponDetails>();
}

public class WeaponDetails
{
    public string Model { get; set; } = string.Empty;
    public int? Ammo { get; set; }
    public List<string> Attachments { get; set; } = new List<string>();
    public List<string> Flags { get; set; } = new List<string>(); // Optional
}

public class PedData
{
    public string Name { get; set; } = string.Empty;
    public List<PedModel> Peds { get; set; } = new List<PedModel>();
}

public class PedModel
{
    public string Model { get; set; } = string.Empty;
    public PedVariant Variants { get; set; } = new PedVariant();
}

public class PedVariant
{
    public PedAttributes? Attributes { get; set; }
    public List<ClothComponents> Clothing { get; set; } = new List<ClothComponents>();
}

public class PedAttributes
{
    public int? Health { get; set; }
    public int? Armour { get; set; }
    public string? Ability { get; set; }
    public string? Movement { get; set; }
    public string? FightRange { get; set; }
    public int? Accuracy { get; set; }
    public int? HearingRange { get; set; }
    public int? SeeingRange { get; set; }
    public bool Juggernaut { get; set; }
    public string? FiringPattern { get; set; }
    public bool SmokeProof { get; set; }
    public int? RateOfFire { get; set; }
}

public class ClothComponents
{
    public int? Type { get; set; }
    public int? TypeIndex { get; set; }
    public int? Index { get; set; }
    public int? PropType { get; set; }
    public int? PropTypeIndex { get; set; }
    public int? PropIndex { get; set; }
}

public class VehicleWeapon
{
    public string Name { get; set; } = string.Empty;
    public string Flag { get; set; } = string.Empty;
    public int? Ammo { get; set; }
}

public class VehicleMod
{
    public string Type { get; set; } = string.Empty;
    public int? Index { get; set; }
}

public class VehicleHealth
{
    public int? Engine { get; set; }
    public int? Body { get; set; }
    public int? Petrol { get; set; }
}

public class VehicleAttributes
{
    public List<VehicleCategoryType> VehicleCategory { get; set; }
    public List<VehicleMod> Mods { get; set; } = new List<VehicleMod>();
    public VehicleHealth? Health { get; set; }

    // Instead of two separate livery objects, just store both indexes
    public int? LiveryIndex { get; set; }      // index
    public int? LiveryIndex2 { get; set; }     // index2
}

public class VehicleInfo
{
    public string Model { get; set; } = string.Empty;
    public VehicleWeapon? VehicleWeapon { get; set; }
    public VehicleAttributes? Attributes { get; set; }
}

public class VehicleData
{
    public string Name { get; set; } = string.Empty;
    public List<VehicleInfo> Vehicles { get; set; } = new List<VehicleInfo>();
}


public class DispatchSetEntry
{
    public string Name { get; set; } = string.Empty;
}

public class DispatchTypeInfo
{
    public DispatchType Type { get; set; }
    public int MaxUnits { get; set; }

    // Final usable objects
    public List<DispatchVehicleInfo> DispatchSets { get; set; } = new();
}

public abstract class WantedStarBase
{
    public Dictionary<DispatchType, DispatchTypeInfo> Dispatches { get; set; } = new();

    public List<DispatchVehicleInfo> GetDispatchList(DispatchType type)
    {
        return Dispatches != null && Dispatches.TryGetValue(type, out var info)
            ? info.DispatchSets
            : new List<DispatchVehicleInfo>();
    }

    public int GetMaxUnits(DispatchType type)
    {
        return Dispatches != null && Dispatches.TryGetValue(type, out var info)
            ? info.MaxUnits
            : 0;
    }
}

public enum VehicleCategoryType
{
    Normal, //default 0
    Medium, //adds 500 health
    Heavy, //adds 1000 health, and enable heavy deform resistence
    ExplosiveProof, //boom proof
    BulletProof, //pew pew proof
    FireProof //burn proof
}

public class WantedLevelOne : WantedStarBase { }
public class WantedLevelTwo : WantedStarBase { }
public class WantedLevelThree : WantedStarBase { }
public class WantedLevelFour : WantedStarBase { }
public class WantedLevelFive : WantedStarBase { }

public class Regions
{
    public string Name { get; set; } = string.Empty;
    public List<string> ZoneName { get; set; } = new List<string>();
    public List<string> StreetNames { get; set; } = new List<string>();

    public Regions(string name, List<string> zonename, List<string> streetname)
    {
        Name = name;
        ZoneName = zonename;
        StreetNames = streetname;
    }
}


public class XMLReader
{
    private static string xmlPath = "./scripts/WOI/";
    private static string PedInfoPath = "PedInfo.xml";
    private static string VehicleInfoPath = "VehicleInfo.xml";
    private static string WeaponInfoPath = "WeaponInfo.xml";
    private static string RegionInfoPath = "RegionInfo.xml";

    public static List<Regions>? Regions { get; set; }
    public static Dictionary<string, PedData>? PedDatas { get; set; }
    public static Dictionary<string, VehicleData>? VehicleData { get; set; }
    public static Dictionary<string, WeaponData>? WeaponDatas { get; set; }
    public static Dictionary<string, WantedStarBase>? WantedLevels { get; set; }
    public static Dictionary<string, DispatchVehicleInfo>? DispatchVehicleSets { get; set; }

    // Add this to your XMLReader.LoadAllInfos() method after loading other data
    public static void LoadAllInfos()
    {
        try
        {
            Logger.Log.Info("Loading XML configuration...");

            Regions = LoadRegions();
            Logger.Log.Info($"Regions loaded: {Regions?.Count ?? 0}");

            PedDatas = LoadAllPeds();
            Logger.Log.Info($"Ped groups loaded: {PedDatas?.Count ?? 0}");

            VehicleData = LoadAllVehicles();
            Logger.Log.Info($"Vehicle groups loaded: {VehicleData?.Count ?? 0}");

            WeaponDatas = LoadAllWeapons();
            Logger.Log.Info($"Weapon groups loaded: {WeaponDatas?.Count ?? 0}");

            DispatchVehicleSets = GetAllDispatchVehicleSets();
            Logger.Log.Info($"Dispatch vehicle sets loaded: {DispatchVehicleSets?.Count ?? 0}");

            WantedLevels = GetWantedLevelsFromXML();
            Logger.Log.Info($"Wanted levels loaded: {WantedLevels?.Count ?? 0}");

            // Load dispatch messages from the same file
            DispatchMessages.LoadFromDispatchDataXML();
            Logger.Log.Info($"Dispatch Messages: {DispatchMessages.GetLoadedStats()}");

            Logger.Log.Info("All XML data loaded successfully.");
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"LoadAllInfos error: {ex.Message}\n{ex.StackTrace}");
        }
    }


    public static List<Regions> LoadRegions()
    {
        try
        {
            string path = xmlPath + RegionInfoPath;
            XDocument doc = XDocument.Load(path);

            var regionList = doc.Root?
                .Elements("Region")
                .Select(region =>
                    new Regions(
                        region.Attribute("name")?.Value ?? "Unknown",
                        region.Elements("ZoneNames").Select(z => z.Value.Trim()).ToList(),
                        region.Elements("StreetNames").Select(s => s.Value.Trim()).ToList()
                    )
                ).ToList() ?? new List<Regions>();

            return regionList;
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"LoadRegions error: {ex.Message}\n{ex.StackTrace}");
            return new List<Regions>();
        }
    }



    public static Dictionary<string, VehicleData> LoadAllVehicles()
    {
        try
        {
            XDocument doc = XDocument.Load(xmlPath + VehicleInfoPath);
            var vehicleDataDict = new Dictionary<string, VehicleData>();

            foreach (var vehicleDataElem in doc.Descendants("VehicleData"))
            {
                string? groupName = vehicleDataElem.Attribute("name")?.Value;
                if (string.IsNullOrEmpty(groupName)) continue;

                var vehicleData = new VehicleData
                {
                    Name = groupName,
                    Vehicles = vehicleDataElem.Elements("Vehicle").Select(vehicleElem =>
                    {
                        return new VehicleInfo
                        {
                            Model = vehicleElem.Attribute("model")?.Value ?? string.Empty,
                            Attributes = ParseAttributes(vehicleElem.Element("Attributes"))
                        };
                    }).ToList()
                };

                vehicleDataDict[groupName] = vehicleData;
            }

            return vehicleDataDict;
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"LoadAllVehicles error: {ex.Message}\n{ex.StackTrace}");
            return new Dictionary<string, VehicleData>();
        }
    }

    private static VehicleAttributes? ParseAttributes(XElement? attributesElem)
    {
        if (attributesElem == null)
            return null;

        string typeAttr = attributesElem.Attribute("type")?.Value ?? string.Empty;
        var categories = typeAttr
            .Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(type =>
                Enum.TryParse(type, true, out VehicleCategoryType result) ? result : VehicleCategoryType.Normal)
            .Distinct()
            .ToList();


        var mods = attributesElem.Elements("Mods").Select(modElem => new VehicleMod
        {
            Type = modElem.Attribute("type")?.Value ?? string.Empty,
            Index = int.TryParse(modElem.Attribute("index")?.Value, out int idx) ? idx : null
        }).ToList();

        var healthElem = attributesElem.Element("Health");
        var health = healthElem != null
            ? new VehicleHealth
            {
                Engine = int.TryParse(healthElem.Attribute("engine")?.Value, out int engine) ? engine : null,
                Body = int.TryParse(healthElem.Attribute("body")?.Value, out int body) ? body : null,
                Petrol = int.TryParse(healthElem.Attribute("petrol")?.Value, out int petrol) ? petrol : null
            }
            : null;

        var liveryElem = attributesElem.Element("Livery");
        int? liveryIndex = null;
        int? liveryIndex2 = null;

        if (liveryElem != null)
        {
            if (int.TryParse(liveryElem.Attribute("index")?.Value, out int index))
                liveryIndex = index;

            if (int.TryParse(liveryElem.Attribute("index2")?.Value, out int index2))
                liveryIndex2 = index2;
        }

        return new VehicleAttributes
        {
            VehicleCategory = categories,
            Mods = mods,
            Health = health,
            LiveryIndex = liveryIndex,
            LiveryIndex2 = liveryIndex2
        };
    }

    public static Dictionary<string, PedData> LoadAllPeds()
    {
        try
        {
            XDocument doc = XDocument.Load(xmlPath + PedInfoPath);
            var result = new Dictionary<string, PedData>();

            foreach (var pedDataElem in doc.Descendants("PedData"))
            {
                string? groupName = pedDataElem.Attribute("name")?.Value;
                if (string.IsNullOrEmpty(groupName)) continue;

                PedData pedData = new PedData { Name = groupName };

                foreach (var pedElem in pedDataElem.Elements("Ped"))
                {
                    string? model = pedElem.Attribute("model")?.Value;
                    if (string.IsNullOrEmpty(model)) continue;

                    PedModel pedModel = new PedModel { Model = model };

                    // Load Attributes
                    var attrElem = pedElem.Element("Attributes");
                    if (attrElem != null)
                    {
                        pedModel.Variants.Attributes = new PedAttributes
                        {
                            Health = int.TryParse(attrElem.Attribute("health")?.Value, out int health) ? health : null,
                            Armour = int.TryParse(attrElem.Attribute("armour")?.Value, out int armour) ? armour : null,
                            Ability = attrElem.Attribute("ability")?.Value,
                            Movement = attrElem.Attribute("movement")?.Value,
                            FightRange = attrElem.Attribute("combatrange")?.Value,
                            Accuracy = int.TryParse(attrElem.Attribute("accuracy")?.Value, out int accuracy) ? accuracy : null,
                            HearingRange = int.TryParse(attrElem.Attribute("hearingrange")?.Value, out int hearingRange) ? hearingRange : null,
                            SeeingRange = int.TryParse(attrElem.Attribute("seeingrange")?.Value, out int seeingRange) ? seeingRange : null,
                            Juggernaut = bool.TryParse(attrElem.Attribute("juggernaut")?.Value, out var jug) && jug,
                            FiringPattern = attrElem.Attribute("firingpattern")?.Value,
                            SmokeProof = bool.TryParse(attrElem.Attribute("smokeproof")?.Value, out var smokeProof) && smokeProof,
                            RateOfFire = int.TryParse(attrElem.Attribute("rateoffire")?.Value, out int rateOfFire) ? rateOfFire : null
                        };
                    }

                    // Load ClothComponents
                    var clothComponentsElem = pedElem.Element("ClothComponents");
                    if (clothComponentsElem != null)
                    {
                        foreach (var clothElem in clothComponentsElem.Elements("ClothComponent"))
                        {
                            pedModel.Variants.Clothing.Add(new ClothComponents
                            {
                                Type = int.TryParse(clothElem.Attribute("type")?.Value, out var t) ? t : null,
                                TypeIndex = int.TryParse(clothElem.Attribute("typeindex")?.Value, out var ti) ? ti : null,
                                Index = int.TryParse(clothElem.Attribute("index")?.Value, out var i) ? i : null,
                                PropType = int.TryParse(clothElem.Attribute("proptype")?.Value, out var pt) ? pt : null,
                                PropTypeIndex = int.TryParse(clothElem.Attribute("proptypeindex")?.Value, out var pti) ? pti : null,
                                PropIndex = int.TryParse(clothElem.Attribute("propindex")?.Value, out var pi) ? pi : null,
                            });
                        }
                    }

                    pedData.Peds.Add(pedModel);
                }

                result[groupName] = pedData;
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"LoadAllPeds error: {ex.Message}\n{ex.StackTrace}");
            return new Dictionary<string, PedData>();
        }
    }

    public static Dictionary<string, WeaponData> LoadAllWeapons()
    {
        try
        {
            XDocument doc = XDocument.Load(xmlPath + WeaponInfoPath);
            var result = new Dictionary<string, WeaponData>();

            foreach (var dataElem in doc.Descendants("WeaponData"))
            {
                string? name = dataElem.Attribute("name")?.Value;
                if (string.IsNullOrEmpty(name)) continue;

                WeaponData weaponData = new WeaponData { Name = name };

                foreach (var weaponElem in dataElem.Elements("Weapon"))
                {
                    string model = weaponElem.Attribute("model")?.Value ?? "";
                    int? ammo = int.TryParse(weaponElem.Attribute("ammo")?.Value, out int ammoValue) ? ammoValue : null;

                    var attachments = (weaponElem.Attribute("attachments")?.Value ?? "")
                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .ToList();

                    var flags = (weaponElem.Attribute("flag")?.Value ?? "")
                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .ToList();

                    weaponData.Weapons.Add(new WeaponDetails
                    {
                        Model = model,
                        Ammo = ammo,
                        Attachments = attachments,
                        Flags = flags
                    });
                }

                result[name] = weaponData;
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"LoadAllWeapons error: {ex.Message}\n{ex.StackTrace}");
            return new Dictionary<string, WeaponData>();
        }
    }

    public static Dictionary<string, DispatchVehicleInfo> GetAllDispatchVehicleSets()
    {
        var result = new Dictionary<string, DispatchVehicleInfo>();
        try
        {
            var doc = XDocument.Load(xmlPath + "DispatchData.xml");
            var dispatchInfoRoot = doc.Root?.Element("DispatchVehicleInfo");
            if (dispatchInfoRoot == null)
            {
                Logger.Log.Fatal("[XML ERROR] <DispatchVehicleInfo> not found.");
                return result;
            }

            foreach (var dispatchElem in dispatchInfoRoot.Elements("Dispatch"))
            {
                string name = dispatchElem.Attribute("name")?.Value?.Trim() ?? "Unnamed";

                var tasksAttr = dispatchElem.Attribute("task")?.Value ?? "";
                var regionsAttr = dispatchElem.Attribute("region")?.Value ?? "";

                var info = new DispatchVehicleInfo
                {
                    Name = name,
                    Tasks = tasksAttr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                    Regions = regionsAttr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                    Vehicles = new List<VehicleData>(),
                    Pilots = ParsePilots(dispatchElem),
                    Soldiers = ParseSoldiers(dispatchElem),
                    PrimaryWeapons = new List<WeaponData>(),
                    SecondaryWeapons = new List<WeaponData>()
                };

                // Vehicles
                var vehicleModels = dispatchElem.Element("Vehicles")?.Elements("Vehicle")
                    .Select(v => v.Attribute("model")?.Value?.Trim())
                    .Where(model => !string.IsNullOrEmpty(model));

                if (vehicleModels != null && VehicleData != null)
                {
                    foreach (var model in vehicleModels)
                    {
                        if (VehicleData.TryGetValue(model, out var vData))
                            info.Vehicles.Add(vData);
                        else
                            Logger.Log.Fatal($"[XML WARNING] Vehicle group '{model}' not found in VehicleData.");
                    }
                }

                // Weapons
                var weaponsElem = dispatchElem.Element("Weapons");
                if (weaponsElem != null && WeaponDatas != null)
                {
                    var primary = weaponsElem.Element("PrimaryWeapons")?.Elements("Weapon")
                        .Select(w => w.Attribute("name")?.Value?.Trim())
                        .Where(name => !string.IsNullOrWhiteSpace(name));

                    if (primary != null)
                    {
                        foreach (var wpn in primary)
                        {
                            if (WeaponDatas.TryGetValue(wpn, out var wData))
                                info.PrimaryWeapons.Add(wData);
                            else
                                Logger.Log.Fatal($"[XML WARNING] PrimaryWeapon '{wpn}' not found in WeaponDatas.");
                        }
                    }

                    var secondary = weaponsElem.Element("SecondaryWeapons")?.Elements("Weapon")
                        .Select(w => w.Attribute("name")?.Value?.Trim())
                        .Where(name => !string.IsNullOrWhiteSpace(name));

                    if (secondary != null)
                    {
                        foreach (var wpn in secondary)
                        {
                            if (WeaponDatas.TryGetValue(wpn, out var wData))
                                info.SecondaryWeapons.Add(wData);
                            else
                                Logger.Log.Fatal($"[XML WARNING] SecondaryWeapon '{wpn}' not found in WeaponDatas.");
                        }
                    }
                }

                result[name] = info;
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"[XML ERROR] Exception in GetAllDispatchVehicleSets: {ex.Message}\n{ex.StackTrace}");
        }
        return result;
    }


    private static List<PedData> ParsePilots(XElement dispatch)
    {
        var pilots = new List<PedData>();
        var pilotsElement = dispatch.Element("Pilots");

        if (pilotsElement != null)
        {
            foreach (var pilot in pilotsElement.Elements("Pilot"))
            {
                var pilotName = pilot.Value;
                if (!string.IsNullOrEmpty(pilotName) && PedDatas != null && PedDatas.ContainsKey(pilotName))
                {
                    pilots.Add(PedDatas[pilotName]);
                }
                else if (!string.IsNullOrEmpty(pilotName))
                {
                    pilots.Add(new PedData { Name = pilotName });
                }
            }
        }

        return pilots;
    }

    private static List<PedData> ParseSoldiers(XElement dispatch)
    {
        var soldiers = new List<PedData>();
        var soldiersElement = dispatch.Element("Soldiers");

        if (soldiersElement != null)
        {
            foreach (var soldier in soldiersElement.Elements("Soldier"))
            {
                var soldierName = soldier.Value;
                if (!string.IsNullOrEmpty(soldierName) && PedDatas != null && PedDatas.ContainsKey(soldierName))
                {
                    soldiers.Add(PedDatas[soldierName]);
                }
                else if (!string.IsNullOrEmpty(soldierName))
                {
                    soldiers.Add(new PedData { Name = soldierName });
                }
            }
        }

        return soldiers;
    }

    public static Dictionary<string, WantedStarBase> GetWantedLevelsFromXML()
    {
        var result = new Dictionary<string, WantedStarBase>();
        try
        {
            var doc = XDocument.Load(xmlPath + "DispatchData.xml");

            foreach (var levelElem in doc.Descendants("WantedLevel"))
            {
                string? starRaw = levelElem.Attribute("star")?.Value;
                if (string.IsNullOrWhiteSpace(starRaw)) continue;

                string star = starRaw.Trim();
                Logger.Log.Info($"🟢 Parsing WantedLevel: {star}");

                var levelData = CreateStarLevel(star);
                if (levelData == null)
                {
                    Logger.Log.Fatal($"⚠️ Unrecognized wanted level key: {star}");
                    continue;
                }

                foreach (var dispatchTypeElem in levelElem.Elements("DispatchType"))
                {
                    string? typeStr = dispatchTypeElem.Attribute("type")?.Value?.Trim();
                    string maxUnitsStr = dispatchTypeElem.Attribute("maxunits")?.Value ?? "0";

                    if (string.IsNullOrEmpty(typeStr) || !Enum.TryParse(typeStr, out DispatchType dispatchType))
                    {
                        Logger.Log.Fatal($"⚠️ Invalid DispatchType: '{typeStr}' in level {star}");
                        continue;
                    }

                    var info = new DispatchTypeInfo
                    {
                        Type = dispatchType,
                        MaxUnits = int.TryParse(maxUnitsStr, out int max) ? max : 0,
                    };

                    foreach (var dispatchSetElem in dispatchTypeElem.Elements("DispatchSet"))
                    {
                        DispatchVehicleInfo dispatchSet = null;
                        string setName = dispatchSetElem.Value.Trim();
                        if (!DispatchVehicleSets?.TryGetValue(setName, out dispatchSet) ?? true)
                        {
                            Logger.Log.Fatal($"❌ DispatchSet '{setName}' in level '{star}' ({typeStr}) not found!");
                            continue;
                        }

                        info.DispatchSets.Add(dispatchSet);
                        Logger.Log.Info($"   ✅ Added Set '{setName}' to {dispatchType} of Level {star}");
                    }

                    levelData.Dispatches[dispatchType] = info;
                }

                result[star] = levelData;
            }

            Logger.Log.Info($"✅ Final WantedLevel Keys: {string.Join(", ", result.Keys)}");
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"❌ GetWantedLevelsFromXML error: {ex.Message}\n{ex.StackTrace}");
        }

        return result;
    }

    private static WantedStarBase CreateStarLevel(string star)
    {
        return star switch
        {
            "One" => new WantedLevelOne(),
            "Two" => new WantedLevelTwo(),
            "Three" => new WantedLevelThree(),
            "Four" => new WantedLevelFour(),
            "Five" => new WantedLevelFive(),
            _ => null
        };
    }
}

public static class DispatchMessages
{
    // Default hardcoded messages as fallback
    private static readonly Dictionary<string, string[]> _defaultGround = new()
    {
        { "LSPD", new[] { "LSPD ground unit dispatched.", "Patrolling officers en route." } },
        { "LSSD", new[] { "Sheriff ground team rolling out.", "LSSD dispatches a ground patrol." } },
        { "NOOSE", new[] {
            "NOOSE tactical team is after you.",
            "You've triggered a NOOSE manhunt.",
            "They've sent the NOOSE ground division. Run." } },
        { "FIB", new[] {
            "FIB has boots on the ground.",
            "Federal agents incoming.",
            "You're now under FIB surveillance. Good luck." } },
        { "IAA", new[] {
            "IAA field operatives closing in.",
            "You made the IAA's list. Bad move.",
            "The IAA isn't bluffing. They're coming." } },
        { "SANG", new[] {
            "Military ground convoy dispatched.",
            "SANG armored units en route.",
            "You're facing military response now." } },
        { "SAPR", new[] { "Park Rangers are rolling in.", "SAPR team deployed." } }
    };

    private static readonly Dictionary<string, string[]> _defaultAir = new()
    {
        { "LSPD", new[] { "Air unit dispatched.", "LSPD heli eyes in the sky." } },
        { "NOOSE", new[] {
            "NOOSE air unit inbound.",
            "Hostile chopper approaching.",
            "Prepare for airborne engagement." } },
        { "FIB", new[] {
            "FIB has deployed a chopper.",
            "Federal surveillance is live from the sky." } },
        { "IAA", new[] {
            "High-clearance IAA chopper dispatched.",
            "They're watching from above." } },
        { "SANG", new[] {
            "SANG gunship inbound.",
            "Military helicopter approaching." } },
        { "SAPR", new[] { "Park Ranger chopper on patrol." } }
    };

    private static readonly Dictionary<string, string[]> _defaultSea = new()
    {
        { "LSPD", new[] { "Marine patrol dispatched." } },
        { "NOOSE", new[] {
            "NOOSE boat team closing in.",
            "Fast-response marine team on water." } },
        { "FIB", new[] { "FIB has aquatic eyes on you." } },
        { "SANG", new[] { "Military sea patrol deployed." } },
        { "IAA", new[] { "An unmarked vessel is watching..." } },
        { "SAPR", new[] { "Rangers are watching from the water." } }
    };

    // Dynamic dictionaries loaded from XML
    public static Dictionary<string, string[]> Ground { get; private set; }
    public static Dictionary<string, string[]> Air { get; private set; }
    public static Dictionary<string, string[]> Sea { get; private set; }

    // Flag to track if XML has been loaded
    private static bool _xmlLoaded = false;

    static DispatchMessages()
    {
        // Initialize with defaults
        Ground = new Dictionary<string, string[]>(_defaultGround);
        Air = new Dictionary<string, string[]>(_defaultAir);
        Sea = new Dictionary<string, string[]>(_defaultSea);
    }

    /// <summary>
    /// Loads dispatch messages from the DispatchData.xml file
    /// </summary>
    public static void LoadFromDispatchDataXML()
    {
        string xmlPath = "./scripts/WOI/DispatchData.xml";
        LoadFromXML(xmlPath);
    }

    /// <summary>
    /// Loads dispatch messages from XML file. Call this during initialization.
    /// </summary>
    /// <param name="xmlPath">Path to the XML file containing dispatch messages</param>
    public static void LoadFromXML(string xmlPath)
    {
        try
        {
            if (!System.IO.File.Exists(xmlPath))
            {
                Logger.Log.Info($"[DispatchMessages] XML file not found at {xmlPath}, using defaults");
                return;
            }

            var doc = System.Xml.Linq.XDocument.Load(xmlPath);
            var dispatchMessagesRoot = doc.Root?.Element("DispatchMessages");

            if (dispatchMessagesRoot == null)
            {
                Logger.Log.Info("[DispatchMessages] DispatchMessages section not found in XML, using defaults");
                return;
            }

            // Clear existing dictionaries and reload from XML
            var tempGround = new Dictionary<string, string[]>();
            var tempAir = new Dictionary<string, string[]>();
            var tempSea = new Dictionary<string, string[]>();

            // Load each dispatch type
            foreach (var dispatchType in dispatchMessagesRoot.Elements("DispatchType"))
            {
                string typeName = dispatchType.Attribute("type")?.Value?.Trim();

                if (string.IsNullOrEmpty(typeName))
                {
                    Logger.Log.Fatal("[DispatchMessages] DispatchType missing 'type' attribute");
                    continue;
                }

                Dictionary<string, string[]> targetDictionary = GetTempDictionaryForType(typeName, tempGround, tempAir, tempSea);
                if (targetDictionary == null)
                {
                    Logger.Log.Fatal($"[DispatchMessages] Unknown dispatch type: {typeName}");
                    continue;
                }

                LoadDepartmentsForType(dispatchType, targetDictionary, typeName);
            }

            // Only update the main dictionaries if we successfully loaded something
            if (tempGround.Count > 0 || tempAir.Count > 0 || tempSea.Count > 0)
            {
                // Update dictionaries with loaded data, keep defaults for missing types
                if (tempGround.Count > 0) Ground = tempGround;
                if (tempAir.Count > 0) Air = tempAir;
                if (tempSea.Count > 0) Sea = tempSea;

                _xmlLoaded = true;
                Logger.Log.Info($"[DispatchMessages] Successfully loaded from XML: Ground={Ground.Count}, Air={Air.Count}, Sea={Sea.Count} departments");
            }
            else
            {
                Logger.Log.Info("[DispatchMessages] No valid dispatch messages found in XML, keeping defaults");
            }

            // Ensure we always have minimum messages
            EnsureMinimumMessages();
        }
        catch (Exception ex)
        {
            Logger.Log.Fatal($"[DispatchMessages] Error loading XML: {ex.Message}\n{ex.StackTrace}");

            // Keep existing dictionaries on error (don't reset to defaults)
            EnsureMinimumMessages();
        }
    }

    private static Dictionary<string, string[]> GetTempDictionaryForType(string typeName,
        Dictionary<string, string[]> tempGround,
        Dictionary<string, string[]> tempAir,
        Dictionary<string, string[]> tempSea)
    {
        return typeName.ToLowerInvariant() switch
        {
            "ground" => tempGround,
            "air" => tempAir,
            "sea" => tempSea,
            _ => null
        };
    }

    private static void LoadDepartmentsForType(System.Xml.Linq.XElement dispatchType, Dictionary<string, string[]> targetDictionary, string typeName)
    {
        foreach (var department in dispatchType.Elements("Department"))
        {
            string departmentName = department.Attribute("name")?.Value?.Trim();

            if (string.IsNullOrEmpty(departmentName))
            {
                Logger.Log.Fatal($"[DispatchMessages] Department missing 'name' attribute in {typeName}");
                continue;
            }

            var messages = department.Elements("DispatchMessage")
                .Select(msg => msg.Value?.Trim())
                .Where(msg => !string.IsNullOrEmpty(msg))
                .ToArray();

            if (messages.Length == 0)
            {
                Logger.Log.Info($"[DispatchMessages] No valid messages found for {departmentName} in {typeName}, skipping");
                continue;
            }

            targetDictionary[departmentName] = messages;
            Logger.Log.Info($"[DispatchMessages] Loaded {messages.Length} messages for {departmentName} ({typeName})");
        }
    }

    private static void EnsureMinimumMessages()
    {
        if (Ground.Count == 0)
        {
            Logger.Log.Info("[DispatchMessages] No Ground messages available, using defaults");
            Ground = new Dictionary<string, string[]>(_defaultGround);
        }

        if (Air.Count == 0)
        {
            Logger.Log.Info("[DispatchMessages] No Air messages available, using defaults");
            Air = new Dictionary<string, string[]>(_defaultAir);
        }

        if (Sea.Count == 0)
        {
            Logger.Log.Info("[DispatchMessages] No Sea messages available, using defaults");
            Sea = new Dictionary<string, string[]>(_defaultSea);
        }
    }

    /// <summary>
    /// Gets a random dispatch message for the specified department from the given source
    /// </summary>
    /// <param name="source">The dictionary to search (Ground, Air, or Sea)</param>
    /// <param name="name">The vehicle/department name to match</param>
    /// <returns>Random message or null if no match found</returns>
    public static string GetRandomMessage(Dictionary<string, string[]> source, string name)
    {
        if (source == null || string.IsNullOrEmpty(name))
            return null;

        // Try exact match first
        if (source.TryGetValue(name, out var exactMessages))
        {
            return exactMessages[HelperClass.SharedRandom.Next(exactMessages.Length)];
        }

        // Try partial match (original behavior for backward compatibility)
        foreach (var key in source.Keys)
        {
            if (name.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var options = source[key];
                return options[HelperClass.SharedRandom.Next(options.Length)];
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a random ground dispatch message for the specified department
    /// </summary>
    public static string GetGroundMessage(string departmentName)
    {
        return GetRandomMessage(Ground, departmentName);
    }

    /// <summary>
    /// Gets a random air dispatch message for the specified department
    /// </summary>
    public static string GetAirMessage(string departmentName)
    {
        return GetRandomMessage(Air, departmentName);
    }

    /// <summary>
    /// Gets a random sea dispatch message for the specified department
    /// </summary>
    public static string GetSeaMessage(string departmentName)
    {
        return GetRandomMessage(Sea, departmentName);
    }

    /// <summary>
    /// Adds or updates messages for a department dynamically
    /// </summary>
    public static void AddOrUpdateDepartment(string dispatchType, string departmentName, string[] messages)
    {
        if (string.IsNullOrEmpty(departmentName) || messages == null || messages.Length == 0)
            return;

        var targetDictionary = GetDictionaryForType(dispatchType);
        if (targetDictionary != null)
        {
            targetDictionary[departmentName] = messages;
            Logger.Log.Info($"[DispatchMessages] Updated {departmentName} in {dispatchType} with {messages.Length} messages");
        }
    }

    private static Dictionary<string, string[]> GetDictionaryForType(string typeName)
    {
        return typeName.ToLowerInvariant() switch
        {
            "ground" => Ground,
            "air" => Air,
            "sea" => Sea,
            _ => null
        };
    }

    /// <summary>
    /// Gets all available departments for a specific dispatch type
    /// </summary>
    public static string[] GetAvailableDepartments(string dispatchType)
    {
        var dictionary = GetDictionaryForType(dispatchType);
        return dictionary?.Keys.ToArray() ?? new string[0];
    }

    /// <summary>
    /// Checks if XML messages have been loaded
    /// </summary>
    public static bool IsXMLLoaded => _xmlLoaded;

    /// <summary>
    /// Reloads dispatch messages from the default DispatchData.xml
    /// </summary>
    public static void ReloadXML()
    {
        _xmlLoaded = false;
        LoadFromDispatchDataXML();
    }

    /// <summary>
    /// Gets statistics about loaded messages
    /// </summary>
    public static string GetLoadedStats()
    {
        int totalGround = Ground.Values.Sum(arr => arr.Length);
        int totalAir = Air.Values.Sum(arr => arr.Length);
        int totalSea = Sea.Values.Sum(arr => arr.Length);

        return $"Loaded: {Ground.Count} Ground depts ({totalGround} msgs), " +
               $"{Air.Count} Air depts ({totalAir} msgs), " +
               $"{Sea.Count} Sea depts ({totalSea} msgs)";
    }
}

