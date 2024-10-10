using ItemManager;
using RustyHalloweenPack.Managers;
using UnityEngine;
using CraftingTable = ItemManager.CraftingTable;

namespace RustyHalloweenPack.Solution;

public static class Load
{
    public static void Setup()
    {
        LoadPieces();
        LoadItems();
        LoadFlora();
    }

    private static void LoadPieces()
    {
        LoadBenches();
        LoadCoffins();
        LoadCrates();
        LoadBuckets();
        LoadPumpkins();
        LoadTombstones();
        LoadFoliage();
        LoadHands();
        LoadSets();
        LoadCandles();
    }

    private static void LoadFlora()
    {
        FloraManager.Flora pickablePumpkin = new(RustyHalloweenPackPlugin.m_assets, "RS_Pickable_Pumpkin", "Pickable Pumpkin");
        pickablePumpkin.SetBiome(Heightmap.Biome.Meadows | Heightmap.Biome.Swamp | Heightmap.Biome.BlackForest);
        pickablePumpkin.SetMinMaxSpawn(1, 3);
        pickablePumpkin.ScaleMin = 0.6f;
        pickablePumpkin.ScaleMax = 1.2f;
        pickablePumpkin.ChanceToTilt = 0f;
        pickablePumpkin.MinGroupSize = 2;
        pickablePumpkin.MaxGroupSize = 3;
        pickablePumpkin.GroupRadius = 10f;
        pickablePumpkin.MinAltitude = 0;
        pickablePumpkin.ForcePlacement = true;
    }

    private static void LoadCandles()
    {
        for (int index = 1; index < 2; ++index)
        {
            BuildPiece piece = new(RustyHalloweenPackPlugin.m_assets, $"RS_HalloCandle_{index}");
            piece.Name.English($"Halloween Candle {index}");
            piece.Description.English("");
            piece.Crafting.Set(Managers.CraftingTable.Workbench);
            piece.Category.Set("Rusty Halloween");
            piece.RequiredItems.Add("Wood", 1, true);
            piece.RequiredItems.Add("Resin", 1, true);
            piece.PlaceEffects.Add("vfx_Place_wood_floor");
            piece.PlaceEffects.Add("sfx_build_hammer_stone");
            piece.DestroyedEffects.Add("sfx_wood_destroyed");
            piece.DestroyedEffects.Add("vfx_SawDust");
            piece.HitEffects.Add("vfx_SawDust");
            MaterialReplacer.RegisterGameObjectForShaderSwap(piece.Prefab, MaterialReplacer.ShaderType.PieceShader);
        }
    }

    private static void LoadSets()
    {
        for (int index = 1; index < 11; ++index)
        {
            BuildPiece piece = new(RustyHalloweenPackPlugin.m_assets, $"RS_HalloweenSet_{index}");
            piece.Name.English($"Halloween set {index}");
            piece.Description.English("");
            piece.Crafting.Set(Managers.CraftingTable.Workbench);
            piece.Category.Set("Rusty Halloween");
            piece.RequiredItems.Add("Wood", 50, true);
            piece.PlaceEffects.Add("vfx_Place_wood_floor");
            piece.PlaceEffects.Add("sfx_build_hammer_stone");
            piece.DestroyedEffects.Add("sfx_wood_destroyed");
            piece.DestroyedEffects.Add("vfx_SawDust");
            piece.HitEffects.Add("vfx_SawDust");
            MaterialReplacer.RegisterGameObjectForShaderSwap(piece.Prefab, MaterialReplacer.ShaderType.PieceShader);
        }
    }

    private static void LoadFoliage()
    {
        for (int index = 1; index < 8; ++index)
        {
            BuildPiece piece = new(RustyHalloweenPackPlugin.m_assets, $"RS_Foliage_{index}");
            piece.Name.English($"Foliage {index}");
            piece.Description.English("");
            piece.Crafting.Set(Managers.CraftingTable.Workbench);
            piece.Category.Set("Rusty Halloween");
            piece.RequiredItems.Add("Stone", 1, true);
            piece.PlaceEffects.Add("vfx_Place_wood_floor");
            piece.PlaceEffects.Add("sfx_build_hammer_wood");
            MaterialReplacer.RegisterGameObjectForShaderSwap(piece.Prefab, MaterialReplacer.ShaderType.PieceShader);
        }
    }

    private static void LoadHands()
    {
        for (int index = 1; index < 4; ++index)
        {
            BuildPiece piece = new(RustyHalloweenPackPlugin.m_assets, $"RS_Hand_{index}");
            piece.Name.English($"Hand {index}");
            piece.Description.English("");
            piece.Crafting.Set(Managers.CraftingTable.Workbench);
            piece.Category.Set("Rusty Halloween");
            piece.RequiredItems.Add("Wood", 1, true);
            piece.PlaceEffects.Add("vfx_Place_wood_floor");
            piece.PlaceEffects.Add("sfx_build_hammer_stone");
            piece.DestroyedEffects.Add("sfx_wood_destroyed");
            piece.DestroyedEffects.Add("vfx_SawDust");
            piece.HitEffects.Add("vfx_SawDust");
            MaterialReplacer.RegisterGameObjectForShaderSwap(piece.Prefab, MaterialReplacer.ShaderType.PieceShader);
        }
    }
    
    private static void LoadTombstones()
    {
        for (int index = 1; index < 3; ++index)
        {
            BuildPiece piece = new(RustyHalloweenPackPlugin.m_assets, $"RS_Tombstone_{index}");
            piece.Name.English($"Tombstone {index}");
            piece.Description.English("");
            piece.Crafting.Set(Managers.CraftingTable.Workbench);
            piece.Category.Set("Rusty Halloween");
            piece.RequiredItems.Add("Stone", 10, true);
            piece.PlaceEffects.Add("vfx_Place_wood_floor");
            piece.PlaceEffects.Add("sfx_build_hammer_stone");
            piece.DestroyedEffects.Add("sfx_wood_destroyed");
            piece.DestroyedEffects.Add("vfx_SawDust");
            piece.HitEffects.Add("vfx_SawDust");
            MaterialReplacer.RegisterGameObjectForShaderSwap(piece.Prefab, MaterialReplacer.ShaderType.PieceShader);
        }
    }

    private static void LoadPumpkins()
    {
        for (int index = 1; index < 4; ++index)
        {
            BuildPiece piece = new(RustyHalloweenPackPlugin.m_assets, $"RS_Pumpkin_{index}");
            piece.Name.English($"Pumpkin {index}");
            piece.Description.English("");
            piece.Crafting.Set(Managers.CraftingTable.Workbench);
            piece.Category.Set("Rusty Halloween");
            piece.RequiredItems.Add("RS_PumpkinItem_1", 1, true);
            piece.PlaceEffects.Add("vfx_Place_wood_floor");
            piece.PlaceEffects.Add("sfx_build_hammer_wood");
            piece.DestroyedEffects.Add("sfx_wood_destroyed");
            piece.DestroyedEffects.Add("vfx_SawDust");
            piece.HitEffects.Add("vfx_SawDust");
            MaterialReplacer.RegisterGameObjectForShaderSwap(piece.Prefab, MaterialReplacer.ShaderType.PieceShader);
        }
    }

    private static void LoadBuckets()
    {
        for (int index = 1; index < 3; ++index)
        {
            BuildPiece piece = new(RustyHalloweenPackPlugin.m_assets, $"RS_HalloBucket_{index}");
            piece.Name.English($"Bucket {index}");
            piece.Description.English("");
            piece.Crafting.Set(Managers.CraftingTable.Workbench);
            piece.Category.Set("Rusty Halloween");
            piece.RequiredItems.Add("Wood", 1, true);
            piece.RequiredItems.Add("Tin", 1, true);
            piece.PlaceEffects.Add("vfx_Place_wood_floor");
            piece.PlaceEffects.Add("sfx_build_hammer_metal");
            piece.DestroyedEffects.Add("sfx_wood_destroyed");
            piece.DestroyedEffects.Add("vfx_SawDust");
            piece.HitEffects.Add("vfx_SawDust");
            MaterialReplacer.RegisterGameObjectForShaderSwap(piece.Prefab, MaterialReplacer.ShaderType.PieceShader);
        }
    }

    private static void LoadCrates()
    {
        BuildPiece piece = new(RustyHalloweenPackPlugin.m_assets, "RS_Crate");
        piece.Name.English("Crate");
        piece.Description.English("");
        piece.Crafting.Set(Managers.CraftingTable.Workbench);
        piece.Category.Set("Rusty Halloween");
        piece.RequiredItems.Add("Wood", 10, true);
        piece.PlaceEffects.Add("vfx_Place_wood_floor");
        piece.PlaceEffects.Add("sfx_build_hammer_wood");
        piece.DestroyedEffects.Add("sfx_wood_destroyed");
        piece.DestroyedEffects.Add("vfx_SawDust");
        piece.HitEffects.Add("vfx_SawDust");
        MaterialReplacer.RegisterGameObjectForShaderSwap(piece.Prefab, MaterialReplacer.ShaderType.PieceShader);
    }

    private static void LoadBenches()
    {
        for (int index = 1; index < 3; ++index)
        {
            var prefabName = $"RS_HalloBench_{index}";
            var name = $"Bench {index}";
            BuildPiece piece = new(RustyHalloweenPackPlugin.m_assets, prefabName);
            piece.Name.English(name);
            piece.Description.English("");
            piece.Crafting.Set(Managers.CraftingTable.Workbench);
            piece.Category.Set("Rusty Halloween");
            piece.RequiredItems.Add("Wood", 10, true);
            piece.RequiredItems.Add("Iron", 1, true);
            piece.PlaceEffects.Add("vfx_Place_wood_floor");
            piece.PlaceEffects.Add("sfx_build_hammer_wood");
            piece.DestroyedEffects.Add("sfx_wood_destroyed");
            piece.DestroyedEffects.Add("vfx_SawDust");
            piece.HitEffects.Add("vfx_SawDust");
            MaterialReplacer.RegisterGameObjectForShaderSwap(piece.Prefab, MaterialReplacer.ShaderType.PieceShader);
        }
    }

    private static void LoadCoffins()
    {
        BuildPiece piece = new(RustyHalloweenPackPlugin.m_assets, "RS_Coffin");
        piece.Name.English("Coffin");
        piece.Description.English("");
        piece.Crafting.Set(Managers.CraftingTable.Workbench);
        piece.Category.Set("Rusty Halloween");
        piece.RequiredItems.Add("Wood", 10, true);
        piece.RequiredItems.Add("IronNails", 10, true);
        piece.PlaceEffects.Add("vfx_Place_wood_floor");
        piece.PlaceEffects.Add("sfx_build_hammer_wood");
        piece.DestroyedEffects.Add("sfx_wood_destroyed");
        piece.DestroyedEffects.Add("vfx_SawDust");
        piece.HitEffects.Add("vfx_SawDust");
        MaterialReplacer.RegisterGameObjectForShaderSwap(piece.Prefab, MaterialReplacer.ShaderType.PieceShader);
    }

    private static void LoadItems()
    {
        SE_Stats SEWizard = ScriptableObject.CreateInstance<SE_Stats>();
        SEWizard.name = "SE_Wizard";
        SEWizard.m_name = "Enchanted";
        SEWizard.m_tooltip = "";
        SEWizard.m_jumpModifier = new Vector3(0f, 0.05f, 0f);

        Item wizardHat = new(RustyHalloweenPackPlugin.m_assets, "RS_HelmetWizardHat");
        wizardHat.Name.English("Wizard Hat");
        wizardHat.Description.English("A rugged, rune-etched wizard hat with a fur brim, blending mystic power and Norse warrior style.");
        wizardHat.Crafting.Add(CraftingTable.Workbench, 1);
        wizardHat.RequiredItems.Add("SwordCheat", 1);
        wizardHat.AddSetStatusEffect(SEWizard);
        SEWizard.m_icon = wizardHat.Prefab.GetComponent<ItemDrop>().m_itemData.GetIcon();

        Item wizardBroom = new(RustyHalloweenPackPlugin.m_assets, "RS_WizardBroom");
        wizardBroom.Name.English("Wizard Broom");
        wizardBroom.Description.English("A sturdy, rune-carved broom, adorned with leather straps, granting flight when paired with the matching hat.");
        wizardBroom.Crafting.Add(CraftingTable.Workbench, 1);
        wizardBroom.RequiredItems.Add("SwordCheat", 1);
        wizardBroom.RequiredUpgradeItems.Add("RS_WizardPotion", 10);
        wizardBroom.AddSetStatusEffect(SEWizard);
        wizardBroom.CloneEffectsFrom = "Club";

        Item wizardPotion = new(RustyHalloweenPackPlugin.m_assets, "RS_WizardPotion");
        wizardPotion.Name.English("Newt Elixir");
        wizardPotion.Description.English("A bubbling, dark green elixir with bits of scales and herbs floating inside, consume at your own risk.");
        wizardPotion.Crafting.Add(CraftingTable.Workbench, 1);
        wizardPotion.RequiredItems.Add("RS_PumpkinItem_1", 10);
        wizardPotion.RequiredItems.Add("NeckTail", 1);
        wizardPotion.RequiredItems.Add("Thistle", 2);

        SE_Stats SEPumpkin = ScriptableObject.CreateInstance<SE_Stats>();
        SEPumpkin.name = "SE_Pumpkin";
        SEPumpkin.m_name = "Pumpkin";
        SEPumpkin.m_windMovementModifier = 0.05f;
        SEPumpkin.m_tooltip = "";

        Item pumpkinHat = new(RustyHalloweenPackPlugin.m_assets, "RS_HelmetPumpkin");
        pumpkinHat.Name.English("Jack'o Hat");
        pumpkinHat.Description.English("A robust, carved pumpkin helmet with glowing eye slits, reinforced with iron bands, offering both protection and a touch of eerie magic.");
        pumpkinHat.Crafting.Add(CraftingTable.Workbench, 1);
        pumpkinHat.RequiredItems.Add("RS_PumpkinItem_1", 100);
        pumpkinHat.RequiredItems.Add("Iron", 5);
        pumpkinHat.AddSetStatusEffect(SEPumpkin);
        SEPumpkin.m_icon = pumpkinHat.Prefab.GetComponent<ItemDrop>().m_itemData.GetIcon();

        Item lantern = new(RustyHalloweenPackPlugin.m_assets, "RS_PumpkinLantern");
        lantern.Name.English("Jack'O Lantern");
        lantern.Description.English("A glowing pumpkin lantern with intricate carvings, enchanted to emit a flickering, mystical light that wards off dark spirits and guides travelers through foggy nights.");
        lantern.Crafting.Add(CraftingTable.Workbench, 1);
        lantern.RequiredItems.Add("RS_PumpkinItem_1", 1);
        lantern.RequiredItems.Add("Bronze", 1);
        lantern.RequiredItems.Add("Resin", 10);
        lantern.AddSetStatusEffect(SEPumpkin);
        
        Item pumpkin1 = new(RustyHalloweenPackPlugin.m_assets, "RS_PumpkinItem_1");
        pumpkin1.Name.English("Pumpkin");
        pumpkin1.Description.English("A plump, golden-orange pumpkin fruit, sweet and juicy with a hint of spice, often used in potions or feasts for its rich flavor and subtle magical properties.");

        Item dollShield = new(RustyHalloweenPackPlugin.m_assets, "RS_ShieldDoll");
        dollShield.Name.English("Stuffed Doll");
        dollShield.Description.English("A whimsical shield made of stitched fabric and filled with soft stuffing, adorned with colorful patches and a friendly face, offering both protection and a comforting presence in battle.");
        dollShield.Crafting.Add(CraftingTable.Workbench, 1);
        dollShield.RequiredItems.Add("RS_WizardPotion", 1);
        dollShield.RequiredItems.Add("Barley", 10);
        dollShield.RequiredItems.Add("Flax", 10);
        dollShield.RequiredItems.Add("Iron", 2);
        dollShield.CloneEffectsFrom = "ShieldWood";

        Item ladle = new(RustyHalloweenPackPlugin.m_assets, "RS_Ladle");
        ladle.Name.English("Ladle");
        ladle.Description.English("A sturdy, bronze ladle with intricate carvings, perfect for stirring potions or serving hearty stews, embodying both utility and charm in the kitchen.");
        ladle.Crafting.Add(CraftingTable.Forge, 1);
        ladle.RequiredItems.Add("Bronze", 2);
        ladle.RequiredItems.Add("RS_WizardPotion", 1);
        ladle.CloneEffectsFrom = "SwordBronze";
    }
}