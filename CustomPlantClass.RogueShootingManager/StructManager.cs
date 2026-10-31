namespace CustomPlantClass.RogueShootingManager
{
    /// <summary>Defines the custom plant data used to create a Rogue Shooting config.</summary>
    public struct CustomRogueShootingConfig()
    {
        /// <summary>Gets or sets the plant represented by this config.</summary>
        public PlantType CustomPlantType { get; set; } = PlantType.Nothing;
        /// <summary>Gets or sets a factory for this plant's buffs.</summary>
        public Func<List<BaseBuff>> CustomBuffs { get; set; } = () => new();
        /// <summary>Gets or sets the callback that reinforces this plant.</summary>
        public Action<Plant> CustomReinforcePlant { get; set; } = null;
        /// <summary>Gets or sets the plant's displayed role.</summary>
        public string CustomRole { get; set; } = "";
    }
    /// <summary>Defines a custom buff shown for a plant in Rogue Shooting.</summary>
    public struct CustomRogueShootingBuff()
    {
        /// <summary>Gets or sets the plant associated with this buff.</summary>
        public PlantType CustomPlantType { get; set; } = PlantType.Nothing;
        /// <summary>Gets or sets the buff title.</summary>
        public string CustomTitle { get; set; } = "";
        /// <summary>Gets or sets the buff description.</summary>
        public string CustomDescription { get; set; } = "";
        /// <summary>Gets or sets the category that controls the buff's presentation.</summary>
        public ShootingBuffType CustomBuffType { get; set; } = ShootingBuffType.UniqueUpgrade;
        /// <summary>Gets or sets the callback invoked when the buff is acquired.</summary>
        public Action CustomOnGet { get; set; } = () => {};
    }
}