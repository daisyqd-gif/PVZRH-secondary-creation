namespace CustomPlantClass.RogueShootingManager
{
    public struct CustomRogueShootingConfig
    {
        public CustomRogueShootingConfig()
        {
        }

        public PlantType CustomPlantType { get; set; } = PlantType.Nothing;
        public Func<List<BaseBuff>> CustomBuffs { get; set; } = () => new();
        public Action<Plant> CustomReinforcePlant { get; set; } = null;
        public string CustomRole { get; set; } = "";
    }
    public struct CustomRogueShootingBuff
    {
        public CustomRogueShootingBuff()
        {
        }

        public PlantType CustomPlantType { get; set; } = PlantType.Nothing;
        public string CustomTitle { get; set; } = "";
        public string CustomDescription { get; set; } = "";
        public ShootingBuffType CustomBuffType { get; set; } = ShootingBuffType.UniqueUpgrade;
        public Action CustomOnGet { get; set; } = () => {};
    }
}