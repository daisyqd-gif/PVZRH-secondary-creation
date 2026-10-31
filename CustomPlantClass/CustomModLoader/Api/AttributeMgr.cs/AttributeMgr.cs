using CustomPlantClass;

namespace FrameWorkLoader.API
{
    /// <summary>Associates mod metadata with a mod class.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModAttribute : Attribute
    {
        /// <summary>Gets the metadata supplied for the mod.</summary>
        public ModInfo Info { get; }

        /// <summary>Creates an attribute containing the specified mod metadata.</summary>
        /// <param name="info">Metadata to associate with the mod.</param>
        public ModAttribute(ModInfo info)
        {
            Info = info;
        }
    }
    /// <summary>Describes a mod's identity, compatibility, load order, and dependencies.</summary>
    [Serializable]
    public struct ModInfo
    {
        /// <summary>The display name of the mod.</summary>
        public string Name;
        /// <summary>The mod's version.</summary>
        public string Version;
        /// <summary>The game version targeted by the mod.</summary>
        public string TargetGameVersion;
        /// <summary>The mod's relative load priority.</summary>
        public int Priority;
        /// <summary>The category assigned to the mod.</summary>
        public int Category;
        /// <summary>The mod's runtime risk classification.</summary>
        public int Danger;
        /// <summary>Whether the loader should automatically load this mod's attributes.</summary>
        public bool AutoLoadAttributes;
        /// <summary>The names of mods that must load before this mod.</summary>
        public string[] Dependencies = Array.Empty<string>();
        /// <summary>Creates metadata using the current mod version as both version values.</summary>
        /// <param name="Name">The mod's display name.</param>
        /// <param name="Priority">The mod's relative load priority.</param>
        /// <param name="Category">The mod's category.</param>
        /// <param name="Danger">The mod's runtime risk classification.</param>
        /// <param name="AutoLoadAttributes">Whether to load the mod's attributes automatically.</param>
        public ModInfo(string Name, int Priority = 0, ModCategory Category = ModCategory.SecondaryCreation, ModDanger Danger = ModDanger.Normal, bool AutoLoadAttributes = false)
        {
            var Version = MyPluginInfo.TargetVersion;
            this.Name = Name;
            this.Version = Version;
            this.Priority = Priority;
            this.Danger = (int)Danger;
            TargetGameVersion = Version;
            this.Category = (int)Category;
            this.AutoLoadAttributes = AutoLoadAttributes;
        }
        /// <summary>Creates metadata with an explicitly specified mod version.</summary>
        /// <param name="Name">The mod's display name.</param>
        /// <param name="Priority">The mod's relative load priority.</param>
        /// <param name="Category">The mod's category.</param>
        /// <param name="Danger">The mod's runtime risk classification.</param>
        /// <param name="Version">The mod's version string.</param>
        /// <param name="AutoLoadAttributes">Whether to load the mod's attributes automatically.</param>
        public ModInfo(string Name, int Priority = 0, ModCategory Category = ModCategory.SecondaryCreation, ModDanger Danger = ModDanger.Normal, string Version = "1.0.0", bool AutoLoadAttributes = false)
        {
            TargetGameVersion = MyPluginInfo.TargetVersion;
            this.Name = Name;
            this.Version = Version;
            this.Priority = Priority;
            this.Danger = (int)Danger;
            this.Category = (int)Category;
            this.AutoLoadAttributes = AutoLoadAttributes;
        }
        /// <summary>Sets the mod assemblies that this mod depends on.</summary>
        /// <param name="Dependencies">Names of the required mods.</param>
        public void DependsOn(string[] Dependencies)
        {
            this.Dependencies = Dependencies;
        }
    }
    /// <summary>
    /// Categorizes mods based on their purpose and scale.
    /// </summary>
    public enum ModCategory
    {
        /// <summary>
        /// Mods that only fix bugs or improve the base game without adding new content.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Mods that add custom plants and/or custom zombies.
        /// Typically single-content or small-scale creations.
        /// </summary>
        SecondaryCreation = 1,

        /// <summary>
        /// Mods that add multiple custom plants and/or zombies.
        /// Essentially content packs or mod bundles.
        /// </summary>
        ModGroup = 2,

        /// <summary>
        /// Large-scale mods that extend the game's systems and add a significant amount of content.
        /// Comparable to DLC-level expansions.
        /// </summary>
        DLC = 3,

        /// <summary>
        /// Mods that provide systems, APIs, or frameworks that other mods depend on.
        /// </summary>
        Framework = 4
    }
    /// <summary>
    /// Indicates how dangerous a mod is to the runtime based on its patching behavior.
    /// </summary>
    public enum ModDanger
    {
        /// <summary>
        /// Mods that do not patch anything.
        /// Pure data/content mods; extremely safe.
        /// </summary>
        VeryLow = 0,

        /// <summary>
        /// Mods that patch small, isolated methods with low risk of conflict.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Mods that patch frequently overridden virtual methods.
        /// These can conflict with other mods and affect core gameplay behavior.
        /// </summary>
        Normal = 2,

        /// <summary>
        /// Mods that use IL2CPP subclassing or other unsafe techniques.
        /// Highest risk; may cause instability or memory corruption.
        /// </summary>
        High = 3
    }
}
