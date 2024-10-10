using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace RustyHalloweenPack.Managers;

public class FloraManager
{
    private static readonly List<Flora> m_flora = new();
    
    static FloraManager()
    {
        Harmony harmony = new("org.bepinex.helpers.FloraManager");
        harmony.Patch(AccessTools.DeclaredMethod(typeof(ZNetScene), nameof(ZNetScene.Awake)),
            postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(FloraManager),
                nameof(Patch_ZNetScene_Awake))));
        harmony.Patch(AccessTools.DeclaredMethod(typeof(ZoneSystem), nameof(ZoneSystem.SetupLocations)),
            postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(FloraManager),
                nameof(Patch_ZoneSystem_SetupLocations_Patch))));
    }

    internal static void Patch_ZNetScene_Awake(ZNetScene __instance)
    {
        GameObject pickable_thistle = __instance.GetPrefab("Pickable_Thistle");
        EffectList effectList = pickable_thistle.GetComponent<Pickable>().m_pickEffector;
        foreach (var flora in m_flora)
        {
            if (flora.Prefab == null) continue;
            flora.Prefab.GetComponent<Pickable>().m_pickEffector = effectList;
            RegisterToZNetScene(__instance, flora.Prefab);
        }
    }

    internal static void Patch_ZoneSystem_SetupLocations_Patch(ZoneSystem __instance)
    {
        __instance.m_vegetation.AddRange(m_flora.Select(flora => flora.GetZoneVegetation()).ToList());
    }

    private static void RegisterToZNetScene(ZNetScene __instance, GameObject prefab)
    {
        if (!__instance.m_prefabs.Contains(prefab)) __instance.m_prefabs.Add(prefab);
        if (!__instance.m_namedPrefabs.ContainsKey(prefab.name.GetStableHashCode()))
        {
            __instance.m_namedPrefabs[prefab.name.GetStableHashCode()] = prefab;
        }
    }

    public class Flora
    {
        public readonly GameObject? Prefab;
        
        // Data
        public bool Enabled = true;
        public float MinSpawn = 1f;
        public float MaxSpawn = 1f;
        public bool ForcePlacement = false;
        public float ScaleMin = 1f;
        public float ScaleMax = 1f;
        public float RandomTilt = 0f;
        public float ChanceToTilt = 0f;
        public Heightmap.Biome Biome = Heightmap.Biome.None;
        public Heightmap.BiomeArea BiomeArea = Heightmap.BiomeArea.Everything;
        public bool BlockCheck = false;
        public bool SnapToStaticSolid = false;
        public float MinAltitude = 0f;
        public float MaxAltitude = 1000f;
        public float MinVegetation = 0f;
        public float MaxVegetation = 100f;
        public bool SurroundCheckVegetation = false;
        public float SurroundCheckDistance = 20f;
        public int SurroundCheckLayers = 2;
        public float SurroundBetterThanAverage = 0f;
        public float MinOceanDepth = 0f;
        public float MaxOceanDepth = 0f;
        public float MinTilt = 0f;
        public float MaxTilt = 50f;
        public float TerrainDeltaRadius = 4f;
        public float MaxTerrainDelta = 75f;
        public bool SnapToWater = false;
        public float GroundOffset = 0f;
        public int MinGroupSize = 1;
        public int MaxGroupSize = 1;
        public float GroupRadius = 0f;
        public bool InForest = false;
        public float MinForestThreshold = 0f;
        public float MaxForestThreshold = 0f;
        public bool Foldout = false;

        public string ConfigGroup = null!;

        private static ConfigEntry<Heightmap.Biome>? m_biomeConfig;
        private static ConfigEntry<float>? m_minSpawnConfig;
        private static ConfigEntry<float>? m_maxSpawnConfig;


        public Flora(AssetBundle assetBundle, string prefabName, string configGroup)
        {
            ConfigGroup = configGroup;
            Prefab = assetBundle.LoadAsset<GameObject>(prefabName);
            if (Prefab == null)
            {
                Debug.LogWarning($"Failed to load asset {prefabName}");
                return;
            }
            m_flora.Add(this);

        }

        public void SetBiome(Heightmap.Biome biome)
        {
            Biome = biome;
            m_biomeConfig = RustyHalloweenPackPlugin._Plugin.config(ConfigGroup, "Biome", biome, "Set biomes");
        }

        public void SetMinMaxSpawn(float min, float max)
        {
            MinSpawn = min;
            MaxSpawn = max;
            m_minSpawnConfig = RustyHalloweenPackPlugin._Plugin.config(ConfigGroup, "Min Spawn", MinSpawn, "Set minimum spawn amount");
            m_maxSpawnConfig = RustyHalloweenPackPlugin._Plugin.config(ConfigGroup, "Max Spawn", MaxSpawn, "Set maximum spawn amount");
        }

        public ZoneSystem.ZoneVegetation? GetZoneVegetation()
        {
            if (Prefab == null) return null;
            return new()
            {
                m_name = Prefab.name,
                m_prefab = Prefab,
                m_enable = Enabled,
                m_min = m_minSpawnConfig?.Value ?? MinSpawn,
                m_max = m_maxSpawnConfig?.Value ?? MaxSpawn,
                m_forcePlacement = ForcePlacement,
                m_scaleMin = ScaleMin,
                m_scaleMax = ScaleMax,
                m_randTilt = RandomTilt,
                m_chanceToUseGroundTilt = ChanceToTilt,
                m_biome = m_biomeConfig?.Value ?? Biome,
                m_biomeArea = BiomeArea,
                m_blockCheck = BlockCheck,
                m_snapToStaticSolid = SnapToStaticSolid,
                m_minAltitude = MinAltitude,
                m_maxAltitude = MaxAltitude,
                m_minVegetation = MinVegetation,
                m_maxVegetation = MaxVegetation,
                m_surroundCheckVegetation = SurroundCheckVegetation,
                m_surroundCheckDistance = SurroundCheckDistance,
                m_surroundCheckLayers = SurroundCheckLayers,
                m_surroundBetterThanAverage = SurroundBetterThanAverage,
                m_minOceanDepth = MinOceanDepth,
                m_maxOceanDepth = MaxOceanDepth,
                m_minTilt = MinTilt,
                m_maxTilt = MaxTilt,
                m_terrainDeltaRadius = TerrainDeltaRadius,
                m_maxTerrainDelta = MaxTerrainDelta,
                m_snapToWater = SnapToWater,
                m_groundOffset = GroundOffset,
                m_groupSizeMin = MinGroupSize,
                m_groupSizeMax = MaxGroupSize,
                m_groupRadius = GroupRadius,
                m_inForest = InForest,
                m_forestTresholdMin = MinForestThreshold,
                m_forestTresholdMax = MaxForestThreshold,
                m_foldout = Foldout
            };
        }
    }
}

