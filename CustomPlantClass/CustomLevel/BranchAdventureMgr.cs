#nullable enable

namespace CustomPlantClass.Level
{
    /// <summary>
    /// Utility for creating branch adventure worlds
    /// </summary>
    //[Obsolete("TOP SECRET! Not done yet!",true)]
    public class BranchAdventureManager
    {
        internal static Dictionary<string, (Dictionary<int, (PlantType, Func<PlantType, bool>)>, Sprite?)> CustomLevelPlantTypes = new();
        internal static List<int> CustomAdventureLevels = new();
        /// <summary>
        /// Creates a new adventure branch
        /// </summary>
        public static void RegisterCustomBranchAdventure<T>(BaseCustomBranchAdventureData data) where T : MonoBehaviour
        {
            //throw new NotImplementedException("TOP SECRET! Not done yet!");
            int index = 0;
            HashSet<ZombieType> allZombies = new();
            HashSet<PlantType> allPlants = new();
            Dictionary<int, (PlantType, Func<PlantType, bool>)> unlocks = new();
            int levelID = -1;
            foreach (var a in data.PerLevelData)
            {
                index++;
                allZombies.UnionWith(a.Item2);
                allPlants.Add(a.Item1);
                unlocks.TryAdd(index, (a.Item1, (PlantType p) => levelID == -1 || LevelProgressionManager.IsCompleted(levelID)));
                var unlockCondition = () => levelID == -1 || LevelProgressionManager.IsCompleted(levelID);
                levelID = CustomLevelMgr.AllocateLevelID(data.nameEN + "_Adventure");
                var leveldata = new BaseCustomLevelData()
                {
                    LevelType = LevelType.NewAdvanture,
                    LevelID = levelID,
                    LevelName = $"{data.nameCN}：第{index}关",
                    LevelNameEn = $"{data.nameEN} {index}",
                    LevelSprite = data.AlmanacBGSprite,
                    SceneType = data.SceneType,
                    ScenePrefab = data.ScenePrefab,
                    SceneBackground = data.SceneBackground,
                    MusicType = data.MusicType,
                    MusicAudio = data.MusicAudio,
                    MaxWave = a.Item4,
                    ZombieTypes = [.. a.Item2],
                    MapRoadTypes = data.MapRoadTypes,
                    selection = CustomLevelSelection.Normal,
                    SelectTypes = [],
                    EnterAction = data.EnterAction,
                    EnterGameAction = data.EnterGameAction,
                    SunCounter = a.Item5,
                    BoardTag = data.BoardTag
                };
                CustomAdventureLevels.Add(levelID);
                CustomLevelMgr.RegisterCustomLevel<T>(leveldata, unlockCondition);
            }
            var unlockCondition2 = () => levelID == -1 || LevelProgressionManager.IsCompleted(levelID);
            levelID = CustomLevelMgr.AllocateLevelID(data.nameEN + "_Adventure");
            var leveldata2 = new BaseCustomLevelData()
            {
                LevelType = LevelType.NewAdvanture,
                LevelID = levelID,
                LevelName = $"{data.nameCN}：第{index + 1}关",
                LevelNameEn = $"{data.nameEN} {index + 1}",
                LevelSprite = data.AlmanacBGSprite,
                SceneType = data.SceneType,
                ScenePrefab = data.ScenePrefab,
                SceneBackground = data.SceneBackground,
                MusicType = data.MusicType,
                MusicAudio = data.MusicAudio,
                MaxWave = 4,
                ZombieTypes = [.. allZombies],
                MapRoadTypes = data.MapRoadTypes,
                selection = CustomLevelSelection.Convey,
                SelectTypes = [.. allPlants],
                EnterAction = data.EnterAction,
                EnterGameAction = data.EnterGameAction,
                SunCounter = 50000,
                BoardTag = data.BoardTag
            };
            CustomLevelMgr.RegisterCustomLevel<T>(leveldata2, unlockCondition2);
            CustomLevelPlantTypes.TryAdd(data.nameEN, (unlocks, data.CardSprite));
            CustomAdventureLevels.Add(levelID);
        }
        /// <summary>
        /// Creates a new adventure branch
        /// </summary>
        public static void RegisterCustomBranchAdventure(BaseCustomBranchAdventureData data)
        {
            //throw new NotImplementedException("TOP SECRET! Not done yet!");
            int index = 0;
            HashSet<ZombieType> allZombies = new();
            HashSet<PlantType> allPlants = new();
            Dictionary<int, (PlantType, Func<PlantType, bool>)> unlocks = new();
            int levelID = -1;
            foreach (var a in data.PerLevelData)
            {
                index++;
                allZombies.UnionWith(a.Item2);
                allPlants.Add(a.Item1);
                unlocks.TryAdd(index, (a.Item1, (PlantType p) => levelID == -1 || LevelProgressionManager.IsCompleted(levelID)));
                var unlockCondition = () => levelID == -1 || LevelProgressionManager.IsCompleted(levelID);
                levelID = CustomLevelMgr.AllocateLevelID(data.nameEN + "_Adventure");
                var leveldata = new BaseCustomLevelData()
                {
                    LevelType = LevelType.NewAdvanture,
                    LevelID = levelID,
                    LevelName = $"{data.nameCN}：第{index}关",
                    LevelNameEn = $"{data.nameEN} {index}",
                    LevelSprite = data.AlmanacBGSprite,
                    SceneType = data.SceneType,
                    ScenePrefab = data.ScenePrefab,
                    SceneBackground = data.SceneBackground,
                    MusicType = data.MusicType,
                    MusicAudio = data.MusicAudio,
                    MaxWave = a.Item4,
                    ZombieTypes = [.. a.Item2],
                    MapRoadTypes = data.MapRoadTypes,
                    selection = CustomLevelSelection.Normal,
                    SelectTypes = [],
                    EnterAction = data.EnterAction,
                    EnterGameAction = data.EnterGameAction,
                    SunCounter = a.Item5,
                    BoardTag = data.BoardTag
                };
                CustomLevelMgr.RegisterCustomLevel(leveldata, unlockCondition);
            }
            var unlockCondition2 = () => levelID == -1 || LevelProgressionManager.IsCompleted(levelID);
            levelID = CustomLevelMgr.AllocateLevelID(data.nameEN + "_Adventure");
            var leveldata2 = new BaseCustomLevelData()
            {
                LevelType = LevelType.NewAdvanture,
                LevelID = levelID,
                LevelName = $"{data.nameCN}：第{index + 1}关",
                LevelNameEn = $"{data.nameEN} {index + 1}",
                LevelSprite = data.AlmanacBGSprite,
                SceneType = data.SceneType,
                ScenePrefab = data.ScenePrefab,
                SceneBackground = data.SceneBackground,
                MusicType = data.MusicType,
                MusicAudio = data.MusicAudio,
                MaxWave = 4,
                ZombieTypes = [.. allZombies],
                MapRoadTypes = data.MapRoadTypes,
                selection = CustomLevelSelection.Convey,
                SelectTypes = [.. allPlants],
                EnterAction = data.EnterAction,
                EnterGameAction = data.EnterGameAction,
                SunCounter = 50000,
                BoardTag = data.BoardTag
            };
            CustomLevelMgr.RegisterCustomLevel(leveldata2, unlockCondition2);
            CustomLevelPlantTypes.TryAdd(data.nameEN, (unlocks, data.CardSprite));
        }
        /// <summary>
        /// Returns a hashset of zombie types for each level
        /// </summary>
        /// <param name="WeightedZombieTypes"> (Zombie type, the minimum level it can appear in)</param>
        /// <param name="theLevelNumber"> the ordinal level number for the current level (1 base)</param>
        public static HashSet<ZombieType> GetZombiePoolPerLevel(HashSet<(ZombieType, int)> WeightedZombieTypes, int theLevelNumber)
        {
            return [..WeightedZombieTypes
                    .Where(p => p.Item2 <= theLevelNumber)
                    .Select(p => p.Item1)];
        }
    }
}