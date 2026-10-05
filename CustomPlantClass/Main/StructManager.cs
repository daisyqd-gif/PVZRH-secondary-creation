#nullable enable
namespace CustomPlantClass.Main
{
    /// <summary>
    /// Core metadata for defining a custom plant.
    /// </summary>
    public struct BaseCustomPlantData
    {
        /// <summary>
        /// The plant ID of the plant
        /// </summary>
        public ID PlantId;
        /// <summary>
        /// The plant's prefab
        /// </summary>
        public GameObject Prefab;
        /// <summary>
        /// The plant's preview image
        /// </summary>
        public GameObject Preview;

        /// <summary>
        /// How to fuse this plant (Can be an empty list for infusable plants, order sensitive)
        /// </summary>
        public List<(ID, ID)> Fusions;
        /// <summary>
        /// How fast should the plant attack (Set to 0 for non attackers)
        /// </summary>
        public float AttackInterval;
        /// <summary>
        /// How fast should the plant produce sun (Set to 0 for non producers)
        /// </summary>
        public float ProduceInterval;
        /// <summary>
        /// How much damage should the plant do when attacking (Set to 0 for non attackers)
        /// </summary>
        public int AttackDamage;
        /// <summary>
        /// How much health should this plant have (Non defensive plants usually have 300)
        /// </summary>
        public int MaxHealth;
        /// <summary>
        /// How long should the seed's cooldown be
        /// </summary>
        public float Cd;
        /// <summary>
        /// How much sun should this plant cost (for fusable plants, add up the full cost of all of its ancestors)
        /// </summary>
        public int Sun;
        /// <summary>
        /// The bullet type the plant should fire (Set to BulletType.Bullet_pea for non shooters)
        /// </summary>
        public BulletType DefaultBullet;
        /// <summary>
        /// Sets if a plant can have a super skill or not. The plant must have 1 component that implements ICustomPF
        /// </summary>
        public bool CanPF;
        /// <summary>
        /// Sets if a plant can star up or not
        /// </summary>
        public bool CanStarUp;
        /// <summary>
        /// The card level of the plant
        /// Normal plants: White
        /// Fused plants: Green
        /// Super plants: Blue
        /// Weak ultimates / normal ultimates: Purple
        /// Strong ultimates: Gold
        /// Treasure mode exclusive plants: Red (Can't be imitated)
        /// </summary>
        public CardLevel CardColor;
        /// <summary>
        /// Sets if the plant will appear in the rainbow card menu
        /// </summary>
        public bool IsRainbowCard;
        /// <summary>
        /// Sets if the plant is travel locked
        /// </summary>
        public bool IsUltimatePlant;
        /// <summary>
        /// Sets the amount of times the plant's seed packet can be selected if it is a rainbow card (Set to 0 if it is not a rainbow card)
        /// </summary>
        public int CardRepeatAmt;
        /// <summary>
        /// The name to be displayed in the almanac
        /// </summary>
        public string Name;
        /// <summary>
        /// The description of the plant shown in the almanac
        /// </summary>
        public string AlmanacEntry;

        /// <summary>
        /// Creates a default struct for the plant
        /// </summary>
        /// <param name="id">The plant ID of the plant</param>
        /// <param name="prefab">The plant's prefab</param>
        /// <param name="preview">The plant's preview</param>
        /// <returns>The default struct</returns>
        public static BaseCustomPlantData Create(
            ID id,
            GameObject prefab,
            GameObject preview
        ) => new BaseCustomPlantData
        {
            PlantId = id,
            Prefab = prefab,
            Preview = preview,

            // defaults
            Fusions = [],
            AttackInterval = 0f,
            ProduceInterval = 0f,
            AttackDamage = 0,
            MaxHealth = 300,
            Cd = 0f,
            Sun = 0,
            DefaultBullet = BulletType.Bullet_pea,
            CanPF = false,
            CanStarUp = false,
            CardColor = CardLevel.White,
            IsRainbowCard = false,
            IsUltimatePlant = false,
            CardRepeatAmt = 1,
            Name = "",
            AlmanacEntry = ""
        };
    }

    /// <summary>
    /// Metadata for defining a custom plant skin.
    /// </summary>
    public struct BasePlantSkinData
    {
        /// <summary>
        /// The base custom plant data
        /// </summary>
        public BaseCustomPlantData data;
        /// <summary>
        /// The skin prefab
        /// </summary>
        public GameObject SkinPrefab;
        /// <summary>
        /// The skin preview
        /// </summary>
        public GameObject SkinPreview;
        /// <summary>
        /// The bullet skin list(replaces bullets the plant fires)
        /// </summary>
        public List<(BulletType, List<GameObject?>)> BulletSkinList;
    }

    /// <summary>
    /// Metadata for defining a bullet.
    /// </summary>
    public struct BaseCustomBulletData
    {
        /// <summary>
        /// The bullet ID
        /// </summary>
        public ID BulletId;
        /// <summary>
        /// The bullet prefab
        /// </summary>
        public GameObject Prefab;

    }
    /// <summary>
    /// Metadata for defining a custom zombie
    /// </summary>
    public struct BaseCustomZombieData
    {
        /// <summary>
        /// The zombie's ID
        /// </summary>
        public ID theZombieType;
        /// <summary>
        /// The zombie's prefab
        /// </summary>
        public GameObject Prefab;
        /// <summary>
        /// The zombie's preview
        /// </summary>
        public Sprite Preview;
        /// <summary>
        /// How much damage the zombie will the zombie deal
        /// </summary>
        public int theAtackDamage;
        /// <summary>
        /// The max health of the zombie
        /// </summary>
        public int maxHealth;
        /// <summary>
        /// The zombie's first armor health
        /// </summary>
        public int theFirstArmorHealth;
        /// <summary>
        /// The zombie's first armor type
        /// </summary>
        public FirstArmorType theFirstArmorType;
        /// <summary>
        /// The zombie's first armor path from the root of the prefab
        /// </summary>
        public string theFirstArmorPath;
        /// <summary>
        /// The zombie's second armor health
        /// </summary>
        public int theSecondArmorHealth;
        /// <summary>
        /// The zombie's second armor type
        /// </summary>
        public SecondArmorType theSecondArmorType;
        /// <summary>
        /// The zombie's second armor path from the root of the prefab
        /// </summary>
        public string theSecondArmorPath;
        /// <summary>
        /// The zombie's spawn level
        /// </summary>
        public int SpawnLevel;
        /// <summary>
        /// The zombie's spawn weight
        /// </summary>
        public int SpawnWeight;
    }
    /// <summary>
    /// Metadata for defining a custom grid item
    /// </summary>
    public struct BaseCustomGridItemData
    {
        /// <summary>
        /// The type of the grid item
        /// </summary>
        public ID type;
        /// <summary>
        /// The prefab of the grid item
        /// </summary>
        public GameObject Prefab;
    }
    /// <summary>
    /// The metadata for defining a custom boss health slider
    /// </summary>
    public struct CustomBossHealthSliderData(Sprite icon)
    {
        /// <summary>
        /// The type of the zombie being displayed
        /// </summary>
        public ZombieType theZombieType = ZombieType.Nothing;
        /// <summary>
        /// The zombie's icon (usually a head)
        /// </summary>
        public Sprite? Icon = icon;
        /// <summary>
        /// The fill image (optional)
        /// </summary>
        public Sprite? FillIcon;
        /// <summary>
        /// The color of the fill (overriden if the fill icon is not null)
        /// </summary>
        public Color FillColor = Color.magenta;
    }
    /// <summary>
    /// A position on the board
    /// </summary>
    public struct BoardPosition : IEquatable<BoardPosition>
    {
        /// <summary>
        /// The row
        /// </summary>
        public int Row { get; }
        /// <summary>
        /// The column
        /// </summary>
        public int Column { get; }
        /// <summary>
        /// A position on the board
        /// </summary>
        public BoardPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// BoardPosition → world position
        /// </summary>
        /// <param name="pos">The board position</param>
        public static implicit operator Vector2(BoardPosition pos)
        {
            float x = pos.Column * 1.35f - 4.8f;

            Board board = Instance;
            bool roof = board.boardTag.isRoof;
            int rows = board.rowNum;

            float y;

            if (!roof)
            {
                if (rows == 6)
                    y = 2.3f - pos.Row * 1.45f;
                else
                    y = 2.3f - pos.Row * 1.67f;
            }
            else
            {
                // Roof math
                if (x <= 1.5f)
                {
                    float f = (pos.Row * 1.4f);
                    y = 1.6f - f + x * 0.22f + 0.5f;
                }
                else
                {
                    y = 4.0f - pos.Row * 1.45f;
                }
            }

            return new Vector2(x, y);
        }

        /// <summary>
        /// world position → BoardPosition
        /// </summary>
        /// <param name="world">The world position</param>
        public static implicit operator BoardPosition(Vector2 world)
        {
            float x = world.x;
            float y = world.y;

            Board board = Instance;
            bool roof = board.boardTag.isRoof;
            int rows = board.rowNum;

            // Column
            int col = Mathf.FloorToInt((x + 5.6f) / 1.35f);
            col = Mathf.Clamp(col, 0, board.columnNum - 1);

            // Row
            int row;

            if (!roof)
            {
                if (rows == 6)
                    row = Mathf.FloorToInt((3.7f - y) / 1.45f);
                else
                    row = Mathf.FloorToInt((3.7f - y) / 1.67f);
            }
            else
            {
                if (x <= 1.5f)
                {
                    float f = (y - x * 0.22f) - 0.5f;
                    row = Mathf.FloorToInt((1.6f - f) / 1.4f) + 1;
                }
                else
                {
                    row = Mathf.FloorToInt((4.0f - y) / 1.45f);
                }
            }

            row = Mathf.Clamp(row, 0, rows - 1);

            return new BoardPosition(row, col);
        }
        /// <inheritdoc/>
        public override string ToString() => $"({Row}, {Column})";
        /// <inheritdoc/>
        public bool Equals(BoardPosition other)
        {
            return other.Row == Row && other.Column == Column;
        }
    }
    internal struct Struct1_Plant
    {
        public Type BaseType;
        public Type CustomType;
        public BaseCustomPlantData data;
    }
    /// <summary>
    /// Metadata for defining a custom level
    /// </summary>
    public struct BaseCustomLevelData()
    {
        /// <summary>
        /// The type of the level
        /// </summary>
        public LevelType LevelType { readonly get; set; } = LevelType.Nothing;
        /// <summary>
        /// The ID of the level
        /// </summary>
        public int LevelID { readonly get; set; } = -114514;
        /// <summary>
        /// The name of the level (Display)
        /// </summary>
        public string LevelName { readonly get; set; } = "";
        /// <summary>
        /// The name of the level (Internal)
        /// </summary>
        public string LevelNameEn { readonly get; set; } = "";
        /// <summary>
        /// The sprite of the level
        /// </summary>
        public Sprite? LevelSprite { readonly get; set; } = default;
        /// <summary>
        /// The scenetype of the level
        /// </summary>
        public SceneType SceneType { readonly get; set; } = SceneType.Day_6;
        /// <summary>
        /// The level's scene prefab (optional, overrides scenetype and background)
        /// </summary>
        public GameObject? ScenePrefab { readonly get; set; } = default;
        /// <summary>
        /// The background of the scene (leave null in not needed)
        /// </summary>
        public Sprite? SceneBackground { readonly get; set; } = default;
        /// <summary>
        /// The type of the music played
        /// </summary>
        public MusicType MusicType { readonly get; set; } = (MusicType)(-1);
        /// <summary>
        /// The audio of the music played (must use a new musictype)
        /// </summary>
        public AudioClip? MusicAudio { readonly get; set; } = default;
        /// <summary>
        /// How many waves are present (one huge wave is 10 waves)
        /// </summary>
        public int MaxWave { readonly get; set; } = 100;
        /// <summary>
        /// The zombie types that appear in the level
        /// </summary>
        public List<ZombieType> ZombieTypes { readonly get; set; } = new() { ZombieType.RandomZombie, ZombieType.RandomPlusZombie, ZombieType.DiamondRandomZombie };
        /// <summary>
        /// The grid types of the level (must be present)
        /// </summary>
        public BoxType_Short[,] MapRoadTypes { readonly get; set; } = new BoxType_Short[,] { };
        /// <summary>
        /// How plants should be selected
        /// </summary>
        public CustomLevelSelection selection { readonly get; set; } = CustomLevelSelection.Normal;
        /// <summary>
        /// What plants are selected
        /// </summary>
        public PlantType[] SelectTypes { readonly get; set; } = [];
        /// <summary>
        /// The action to run when the level is opened
        /// </summary>
        public Action EnterAction { readonly get; set; } = () => { };
        /// <summary>
        /// The action to run when the game starts
        /// </summary>
        public Action<Board> EnterGameAction { readonly get; set; } = (Board b) => { };
        /// <summary>
        /// How much sun should the player get when the level starts
        /// </summary>
        public int SunCounter { readonly get; set; } = 500;
        /// <summary>
        /// Unlocked buffs for the level
        /// </summary>
        public List<AdvBuff> AdvBuffs { readonly get; set; } = new();
        /// <summary>
        /// Unlocked ultimate buffs for the level
        /// </summary>
        public List<UltiBuff> UltiBuffs { readonly get; set; } = new();
        /// <summary>
        /// Unlocked unlock buffs for the level
        /// </summary>
        public List<TravelUnlocks> TravelUnlocks { readonly get; set; } = new();
        /// <summary>
        /// Unlocked debuffs for the level
        /// </summary>
        public List<TravelDebuff> TravelDebuffs { readonly get; set; } = new();
        /// <summary>
        /// The level's boardtag
        /// </summary>
        public BoardTag BoardTag { readonly get; set; } = default;
        /// <summary>
        /// A quick tool for generating full screen layouts
        /// </summary>
        /// <param name="tile">The type of the tile</param>
        /// <param name="lanes">How many lanes are there</param>
        /// <param name="cols">How manu columns are there</param>
        /// <returns>The layout</returns>
        public static BoxType_Short[,] CreateFullScreenLayout(BoxType_Short tile, int lanes = 5, int cols = 10)
        {
            BoxType_Short[,] board = new BoxType_Short[lanes, cols];
            for (int lane = 0; lane < lanes; lane++)
            {
                for (int col = 0; col < cols; col++)
                {
                    board[lane, col] = tile;
                }
            }
            return board;
        }
    }
    /// <summary>
    /// Metadata for branch adventures
    /// </summary>
    public struct BaseCustomBranchAdventureData()
    {
        /// <summary>
        /// (The unlocked plant, the zombie pool, the level sprite, wave count, sun count)
        /// </summary>
        public List<(PlantType, HashSet<ZombieType>, Sprite?, int, int)> PerLevelData { readonly get; set; } = new();
        /// <summary>
        /// The name of the level (Display)
        /// </summary>
        public string nameCN { readonly get; set; } = "";
        /// <summary>
        /// The name of the level (Internal)
        /// </summary>
        public string nameEN { readonly get; set; } = "";
        /// <summary>
        /// The background of the almanac plant
        /// </summary>
        public Sprite? AlmanacBGSprite { readonly get; set; } = default;
        /// <summary>
        /// The background of the seed packed
        /// </summary>
        public Sprite? CardSprite { readonly get; set; } = default;
        /// <summary>
        /// The scene type (Should not be defined in the enum)
        /// </summary>
        public SceneType SceneType { readonly get; set; } = SceneType.Day;
        /// <summary>
        /// The level's scene prefab (optional, overrides scenetype and background)
        /// </summary>
        public GameObject? ScenePrefab { readonly get; set; } = default;
        /// <summary>
        /// The background of the scene (leave null in not needed)
        /// </summary>
        public Sprite? SceneBackground { readonly get; set; } = default;
        /// <summary>
        /// The type of the music played
        /// </summary>
        public MusicType MusicType { readonly get; set; } = (MusicType)(-1);
        /// <summary>
        /// The audio of the music played (must use a new musictype)
        /// </summary>
        public AudioClip? MusicAudio { readonly get; set; } = default;
        /// <summary>
        /// The grid types of the level (must be present)
        /// </summary>
        public BoxType_Short[,] MapRoadTypes { readonly get; set; } = new BoxType_Short[,]
        {
            {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G },
            {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G },
            {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G },
            {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G },
            {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G },
            {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G }
        };
        /// <summary>
        /// The action to run when the level is opened
        /// </summary>
        public Action EnterAction { readonly get; set; } = () => { };
        /// <summary>
        /// The action to run when the game starts
        /// </summary>
        public Action<Board> EnterGameAction { readonly get; set; } = (Board b) => { };
        /// <summary>
        /// The level's boardtag
        /// </summary>
        public BoardTag BoardTag { readonly get; set; } = default;
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum CustomLevelSelection
    {
        Normal = 0,
        Convey = 1,
        PreSelected = 2
    }
    public enum BossSliderType
    {
        UltimateSword = 0,
        ObsidianGarcantuar = 1,
        UltimateDrown = 2,
        UltimateFootball = 3,
        UltimateHorse = 4,
        UltimateImp = 5,
        UltimateJackbox = 6,
        UltimateJackson = 7,
        UltimateKirov = 8,
        UltimateLegion = 9,
        UltimateMachineNut = 10,
        UltimatePaper = 11,
        UltimateSnow = 12
    }
    public enum PlantLevelData
    {
        Basic = 0,
        Secondary = 1,
        Super = 2,
        WeakUltimate = 3,
        StrongUltimate = 4,
        FinalUltimate = 5,
        TreasurePlant = 6
    }
    public enum BoxType_Short
    {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        /// <summary>
        /// Grass
        /// </summary>
        G = 0,         // 草地
        /// <summary>
        /// Water
        /// </summary>
        W = 1,         // 水域
        /// <summary>
        /// Dirt
        /// </summary>
        D = 2,         // 泥土
        /// <summary>
        /// Roof
        /// </summary>
        R = 3,         // 屋顶
        /// <summary>
        /// Stone
        /// </summary>
        S = 4,         // 石头
        /// <summary>
        /// River
        /// </summary>
        River = 5,     // 河流
        /// <summary>
        /// Mud
        /// </summary>
        Dirt_water = 6 // 泥水域
    }
}