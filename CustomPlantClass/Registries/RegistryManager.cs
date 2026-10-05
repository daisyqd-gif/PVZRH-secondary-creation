using BepInEx.Configuration;
namespace CustomPlantClass.Registry
{
    /// <summary>
    /// A registry manager for plugins to register persistent memory in config files.
    /// </summary>
    public static class RegistryManager
    {
        internal static ConfigFile _cfg;
        [OnLoad]
        internal static void OnLoad()
        {
            _cfg = Plugin.Instance.Config;
        }
        /// <summary>
        /// Creates a config entry.
        /// </summary>
        /// <typeparam name="T">The type of the config entry(It is recommended that you use a basic type)</typeparam>
        /// <param name="modName">The name of the mod</param>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="description">The setting's description(can be a blank string if if is not needed)</param>
        /// <param name="callBack">This is called when the setting is changed.</param>
        /// <param name="defaultVal">The default value</param>
        public static void CreateConfigRegistry<T>(string modName, string settingName, string description, Action<ConfigEntry<T>> callBack, T defaultVal = default!) where T : IEquatable<T>
        {
            PluginBehaviour.QueueOrExecute(() => 
            {
                var entry = _cfg.Bind("Mods", $"{modName} : {settingName}", defaultVal, new ConfigDescription(description));
                entry.SettingChanged+= (_,_) => callBack(entry);
            });
        }
        /// <summary>
        /// Creates a dropdown config entry.
        /// </summary>
        /// <typeparam name="T">The type of the config entry(It is recommended that you use a basic type)</typeparam>
        /// <param name="modName">The name of the mod</param>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="description">The setting's description(can be a blank string if if is not needed)</param>
        /// <param name="callBack">This is called when the setting is changed.</param>
        /// <param name="defaultVal">The default value</param>
        /// <param name="acceptableValues">Values present in the dropdown</param>
        public static void CreateConfigRegistry<T>(string modName, string settingName, string description, Action<ConfigEntry<T>> callBack, T defaultVal = default!, params T[] acceptableValues) where T : IEquatable<T>
        {
            var accList = new AcceptableValueList<T>(acceptableValues);
            PluginBehaviour.QueueOrExecute(() => 
            {
                var entry = _cfg.Bind("Mods", $"{modName} : {settingName}", defaultVal, new ConfigDescription(description, accList));
                entry.SettingChanged+= (_,_) => callBack(entry);
            });
        }
        /// <summary>
        /// Creates a ranged config entry.
        /// </summary>
        /// <typeparam name="T">The type of the config entry(It is recommended that you use an icomparible)</typeparam>
        /// <param name="modName">The name of the mod</param>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="description">The setting's description(can be a blank string if if is not needed)</param>
        /// <param name="callBack">This is called when the setting is changed.</param>
        /// <param name="minVal">The minimum value of the range</param>
        /// <param name="maxVal">The maximum value of the range</param>
        /// <param name="defaultVal">The default value</param>
        public static void CreateConfigRegistry<T>(string modName, string settingName, string description, Action<ConfigEntry<T>> callBack, T minVal, T maxVal , T defaultVal = default!) where T : IComparable
        {
            var accList = new AcceptableValueRange<T>(minVal, maxVal);
            PluginBehaviour.QueueOrExecute(() => 
            {
                var entry = _cfg.Bind("Mods", $"{modName} : {settingName}", defaultVal, new ConfigDescription(description, accList));
                entry.SettingChanged+= (_,_) => callBack(entry);
            });
        }
        /// <summary>
        /// Creates a slider config entry.
        /// </summary>
        /// <param name="modName">The name of the mod</param>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="description">The setting's description(can be a blank string if if is not needed)</param>
        /// <param name="callBack">This is called when the setting is changed.</param>
        /// <param name="minVal">The minimum value of the slider</param>
        /// <param name="maxVal">The maximum value of the slider</param>
        /// <param name="defaultVal">The default value</param>
        public static void CreateConfigSlider(string modName, string settingName, string description, Action<ConfigEntry<int>> callBack, int minVal, int maxVal , int defaultVal = 0)
            => CreateConfigRegistry(modName,settingName,description,callBack,minVal,maxVal,defaultVal);

        /// <summary>
        /// Creates an advanced config entry.
        /// </summary>
        /// <typeparam name="T">The type of the config entry(It is recommended that you use a basic type)</typeparam>
        /// <param name="modName">The name of the mod</param>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="description">The setting's description(can be a blank string if if is not needed)</param>
        /// <param name="callBack">This is called when the setting is changed.</param>
        /// <param name="defaultVal">The default value</param>
        public static void CreateAdvancedConfigRegistry<T>(string modName, string settingName, string description, Action<ConfigEntry<T>> callBack, T defaultVal = default!) where T : IEquatable<T>
        {
            PluginBehaviour.QueueOrExecute(() => 
            {
                var entry = _cfg.Bind("Mods", $"{modName} : {settingName}", defaultVal, new ConfigDescription(description, tags: "Advanced"));
                entry.SettingChanged+= (_,_) => callBack(entry);
            });
        }
        /// <summary>
        /// Creates an advanced dropdown config entry.
        /// </summary>
        /// <typeparam name="T">The type of the config entry(It is recommended that you use a basic type)</typeparam>
        /// <param name="modName">The name of the mod</param>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="description">The setting's description(can be a blank string if if is not needed)</param>
        /// <param name="callBack">This is called when the setting is changed.</param>
        /// <param name="defaultVal">The default value</param>
        /// <param name="acceptableValues">Values present in the dropdown</param>
        public static void CreateAdvancedConfigRegistry<T>(string modName, string settingName, string description, Action<ConfigEntry<T>> callBack, T defaultVal = default!, params T[] acceptableValues) where T : IEquatable<T>
        {
            var accList = new AcceptableValueList<T>(acceptableValues);
            PluginBehaviour.QueueOrExecute(() => 
            {
                var entry = _cfg.Bind("Mods", $"{modName} : {settingName}", defaultVal, new ConfigDescription(description, accList, "Advanced"));
                entry.SettingChanged+= (_,_) => callBack(entry);
            });
        }
        /// <summary>
        /// Creates an advanced ranged config entry.
        /// </summary>
        /// <typeparam name="T">The type of the config entry(It is recommended that you use an icomparible)</typeparam>
        /// <param name="modName">The name of the mod</param>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="description">The setting's description(can be a blank string if if is not needed)</param>
        /// <param name="callBack">This is called when the setting is changed.</param>
        /// <param name="minVal">The minimum value of the range</param>
        /// <param name="maxVal">The maximum value of the range</param>
        /// <param name="defaultVal">The default value</param>
        public static void CreateAdvancedConfigRegistry<T>(string modName, string settingName, string description, Action<ConfigEntry<T>> callBack, T minVal, T maxVal , T defaultVal = default!) where T : IComparable
        {
            var accList = new AcceptableValueRange<T>(minVal, maxVal);
            PluginBehaviour.QueueOrExecute(() => 
            {
                var entry = _cfg.Bind("Mods", $"{modName} : {settingName}", defaultVal, new ConfigDescription(description, accList, "Advanced"));
                entry.SettingChanged+= (_,_) => callBack(entry);
            });
        }
    }
}
