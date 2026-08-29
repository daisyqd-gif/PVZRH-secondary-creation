namespace CustomLevelLoader
{
    public struct BaseCustomLevelData
    {
        public BaseCustomLevelData(){}
        public List<AdvBuff> AdvBuffs { get; set; } = new List<AdvBuff>();
        public List<(UltiBuff,int)> UltiBuffs { get; set; } = new List<(UltiBuff,int)>();
        public List<InvestBuff> InvestBuffs { get; set; } = new List<InvestBuff>();
        public List<TravelDebuff> Debuffs { get; set; } = new List<TravelDebuff>();
        public MusicType BgmType { get; set; } = MusicType.Day;
        public Board.BoardTag BoardTag { get; set; } = default;
        public List<PlantType> ConveyBeltPlantTypes { get; set; } = new List<PlantType>();
        public LevelLogoType logoType = LevelLogoType.Default;
        public PlantType LogoPlantType=PlantType.Present;
        public ZombieType LogoZombieType=ZombieType.NormalZombie;
        public Sprite CustomLogo { get; set; } = new();
        public string Name { get; set; } = "";
        public bool NeedSelectCard { get; set; } = true;
        public Action<Board> PostBoard { get; set; } = (_) => { };
        public Action<InitBoard> PostInitBoard { get; set; } = (_) => { };
        public Action PreInitBoard { get; set; } = () => { };
        public List<(int, int, PlantType)> PrePlants { get; set; } = new List<(int, int, PlantType)>();
        public List<PlantType> PreSelectCards { get; set; } = new List<PlantType>();
        public int RowCount { get; set; } = 6;
        public CustomSceneType SceneType { get; set; } = CustomSceneType.Day;
        public List<PlantType> SeedRainPlantTypes { get; set; } = new List<PlantType>();
        public int Sun { get; set; } = 500;
        public int WaveCount { get; set; } = 10;
        public int ZombieHealthRate { get; set; } = 1;
        public List<ZombieType> ZombieList { get; set; } = new List<ZombieType>();
        public Dictionary<int, Action<Board>> WaveActions = new Dictionary<int, Action<Board>>(){};
    }
    public enum LevelLogoType
    {
        Default=0,
        Minigame=1,
        AdvChallenge=2,
        FlagChallenge=3,
        GardenChallenge=4,
        TravelMinigame=5,
        GiftBox=6,
        TravelExperience=7,
        TravelLevel=8,
        Wheat=9,
        WheatGlove=10,
        PlantType=11,
        ZombieType=12,
        Custom=13
    }
    public enum CustomSceneType
    {
        Day = 0,
        Night = 1,
        Pool = 2,
        NightPool = 3,
        Roof = 4,
        NightRoof = 5,
        Day_6 = 6,
        Night_6 = 7,
        SuperDay = 8,
        SuperPool = 9,
        Travel_roof = 10,
        Test_green = 11,
        Travel_roof_dusk = 12,
        Travel_roof_night = 13,
        MidDay = 14,
        BilliardBallDay = 15,
        BilliardBallMidDay = 16,
        PVPScaryPot = 17,
        Snow = 18,
        Chess = 19,
        Snow_6 = 20,
        ReversalPool = 21,
        BigPool = 22,
        Roof_Pool = 23,
        River = 24,
        IZDay = 25,
        SnowPool = 26,
        LongMap = 27,
        TreasureBeach = 28,
        MidMap = 29,
        LavaBeach = 30,
        NormalBeach = 31,
        SnowPool_night = 32,
        RoofPool_dusk = 33,
        RoofPool_night = 34,
        Day_bubble = 35,
        AutoChess = 36,
        LavaPool = 37,
        PVPRandom = 38,
        Custom = 39
    }
}