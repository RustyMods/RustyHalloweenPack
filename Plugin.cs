using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using RustyHalloweenPack.Solution;
using ServerSync;
using UnityEngine;

namespace RustyHalloweenPack
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class RustyHalloweenPackPlugin : BaseUnityPlugin
    {
        internal const string ModName = "RustyHalloweenPack";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "RustyMods";
        private const string ModGUID = Author + "." + ModName;
        private static readonly string ConfigFileName = ModGUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);
        public static readonly ManualLogSource RustyHalloweenPackLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);
        private static readonly ConfigSync ConfigSync = new(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };
        public enum Toggle { On = 1, Off = 0 }
        public static readonly AssetBundle m_assets = GetAssetBundle("halloweenbundle");
        public static readonly GameObject m_broomObject = m_assets.LoadAsset<GameObject>("WizardBroom");
        public static RustyHalloweenPackPlugin _Plugin = null!;

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;
        public static ConfigEntry<int> _broomSpeed = null!;
        public static ConfigEntry<float> _broomAcceleration = null!;
        public static ConfigEntry<Toggle> _enableBroom = null!;
        public static ConfigEntry<float> _staminaConsumption = null!;
        public static ConfigEntry<float> _eitrConsumption = null!;
        public static ConfigEntry<float> _levelConsumptionReduction = null!;
        public static ConfigEntry<Toggle> _enableParticles = null!;
        // public static ConfigEntry<Toggle> _enablePumpkinHeads = null!;

        private void InitConfigs()
        {
            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On, "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);
            
            _broomSpeed = config("2 - Settings", "Broom speed", 10, "Set flight speed");
            _broomAcceleration = config("2 - Settings", "Broom Acceleration", 0.01f, new ConfigDescription("Set acceleration", new AcceptableValueRange<float>(0f, 1f)));
            _enableBroom = config("2 - Settings", "_Enable Broom Flight", Toggle.On, "If on, players can fly with the broom");
            _staminaConsumption = config("2 - Settings", "Broom Stamina Cost", 1f,
                new ConfigDescription("Set stamina consumption per second", new AcceptableValueRange<float>(0f, 5f)));
            _eitrConsumption = config("2 - Settings", "Broom Eitr Cost", 0f,
                new ConfigDescription("Set eitr consumption per second", new AcceptableValueRange<float>(0f, 5f)));
            _levelConsumptionReduction = config("2 - Settings", "Item Level Consumption Reduction", 0.5f,
                new ConfigDescription("Set the multiplier for item quality consumption reduction",
                    new AcceptableValueRange<float>(0f, 1f)));
            _enableParticles = config("2 - Settings", "Broom Particles", Toggle.On,
                "If on, broom trails particles while flying");
            _enableParticles.SettingChanged += (sender, args) =>
            {
                foreach (var instance in BroomFly.m_instances)
                {
                    instance.SetParticles(_enableParticles.Value is Toggle.On);
                }
            };
            // _enablePumpkinHeads = config("3 - Pumpkin Heads", "_Enable", Toggle.On, "If on, pumpkin heads appear on creatures");
            // _enablePumpkinHeads.SettingChanged += (sender, args) =>
            // {
            //     foreach (var instance in PumpkinHead.m_instances)
            //     {
            //         instance.SetPumpkin(_enablePumpkinHeads.Value is Toggle.On);
            //     }
            // };
        }
        
            
        private static AssetBundle GetAssetBundle(string fileName)
        {
            Assembly execAssembly = Assembly.GetExecutingAssembly();
            string resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
            using Stream? stream = execAssembly.GetManifestResourceStream(resourceName);
            return AssetBundle.LoadFromStream(stream);
        }

        public void Awake()
        {
            //Localizer.Load(); 
            _Plugin = this;
            InitConfigs();
            Load.Setup();
            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void OnDestroy() => Config.Save();
        
        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                RustyHalloweenPackLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                RustyHalloweenPackLogger.LogError($"There was an issue loading your {ConfigFileName}");
                RustyHalloweenPackLogger.LogError("Please check your config entries for spelling and format!");
            }
        }

        public ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        public ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            [UsedImplicitly] public int? Order;
            [UsedImplicitly] public bool? Browsable;
            [UsedImplicitly] public string? Category;
            [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer;
        }
    }
}