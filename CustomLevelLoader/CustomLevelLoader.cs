namespace CustomLevelLoader
{
    public class CustomLevelLoader : MonoBehaviour
    {
        
    }
    public class DefaultBoardData: MonoBehaviour
    {
        public static int GetRowCount(SceneType theSceneType)
        {
            switch (theSceneType)
            {
                case SceneType.Day:
                case SceneType.Day_bubble:
                case SceneType.Night:
                case SceneType.Roof:
                case SceneType.IZDay:
                case SceneType.LongMap:
                case SceneType.River:
                case SceneType.Snow:
                case SceneType.LavaBeach:
                    return 5;
                case SceneType.SuperDay:
                case SceneType.SuperPool:
                case SceneType.Chess:
                    return 12;
                default:
                    return 6;
            }
        }

        private static readonly BoxType[] _defaultLandArray =
        {
            BoxType.Grass, BoxType.Grass, BoxType.Grass, BoxType.Grass, BoxType.Grass,
            BoxType.Grass, BoxType.Grass, BoxType.Grass, BoxType.Grass, BoxType.Grass
        };

        public static IReadOnlyList<BoxType> DefaultLandArray => _defaultLandArray;

        private static readonly BoxType[] _defaultWaterArray =
        {
            BoxType.Water, BoxType.Water, BoxType.Water, BoxType.Water, BoxType.Water,
            BoxType.Water, BoxType.Water, BoxType.Water, BoxType.Water, BoxType.Water
        };

        public static IReadOnlyList<BoxType> DefaultWaterArray => _defaultWaterArray;
    }
}
