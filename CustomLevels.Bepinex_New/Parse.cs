namespace CustomLevels.Bepinex
{
    public static class Parse
    {
        // SceneType
        public static void GetSceneType(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("SceneType", out var elem))
                return;
            Debug.Log($"SceneType: {elem}");
            if (elem.ValueKind == JsonValueKind.Number)
                data.SceneType = (SceneType)elem.GetInt32();
            else if (elem.ValueKind == JsonValueKind.String &&
                Enum.TryParse(elem.GetString(), true, out SceneType st))
                data.SceneType = st;
        }

        // MusicType
        public static void Music(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("MusicType", out var elem))
                return;
            Debug.Log($"MusicType: {elem}");
            if (elem.ValueKind == JsonValueKind.Number)
                data.BgmType = (MusicType)elem.GetInt32();
            else if (elem.ValueKind == JsonValueKind.String &&
                     Enum.TryParse(elem.GetString(), true, out MusicType mt))
                data.BgmType = mt;
        }

        // BoardTag
        public static void BoardTag(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("BoardTag", out var tagElem))
                return;
            Debug.Log("Found BoardTag");
            Board.BoardTag tag = new();

            foreach (var prop in tagElem.EnumerateObject())
            {
                var field = typeof(Board.BoardTag).GetField(prop.Name);
                if (field != null && field.FieldType == typeof(bool)){
                    Debug.Log($"    -{tag} : {prop.Value.GetBoolean()}");
                    field.SetValueDirect(__makeref(tag), prop.Value.GetBoolean());
                }
            }

            data.BoardTag = tag;

            if (tag.isConvey)
                data.NeedSelectCard = false;
        }

        // Buffs
        public static void AdvBuffs(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("AdvBuffs", out var arr) ||
                arr.ValueKind != JsonValueKind.Array)
                return;
            
            List<int> list = new();

            foreach (var item in arr.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number)
                    list.Add(item.GetInt32());
                else if (item.ValueKind == JsonValueKind.String &&
                         int.TryParse(item.GetString(), out int id))
                    list.Add(id);
            }
            Debug.Log($"AdvBuffs: {string.Join(", ", list)}");
            data.AdvBuffs = () => list;
        }

        public static void Debuffs(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("Debuffs", out var arr) ||
                arr.ValueKind != JsonValueKind.Array)
                return;
                
            List<int> list = new();

            foreach (var item in arr.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number)
                    list.Add(item.GetInt32());
                else if (item.ValueKind == JsonValueKind.String &&
                         int.TryParse(item.GetString(), out int id))
                    list.Add(id);
            }
            Debug.Log($"Debuffs: {string.Join(", ", list)}");
            data.Debuffs = () => list;
        }

        public static void UltiBuffs(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("UltiBuffs", out var arr) ||
                arr.ValueKind != JsonValueKind.Array)
                return;

            List<(int, int)> list = new();
            Debug.Log("Ulti Buffs:");
            foreach (var item in arr.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Array &&
                    item.GetArrayLength() >= 2)
                {
                    int id = item[0].GetInt32();
                    int level = item[1].GetInt32();
                    list.Add((id, level));
                    Debug.Log($"    -({id}, {level})");
                }
            }
            
            data.UltiBuffs = () => list;
        }

        // Plants
        public static void Conveyor(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("ConveyBeltPlantTypes", out var arr))
                return;
            Debug.Log($"Plants on convey belt:");
            data.ConveyBeltPlantTypes = () => ParsePlantList(arr);
        }

        public static void SeedRain(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("SeedRainPlantTypes", out var arr))
                return;
            Debug.Log($"Plants on seed rain:");
            data.SeedRainPlantTypes = () => ParsePlantList(arr);
        }

        public static void PreSelectCards(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("PreSelectCards", out var arr))
                return;
            Debug.Log($"Plants pre selected:");
            data.PreSelectCards = () => ParsePlantList(arr);
        }

        public static void PrePlants(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("PrePlants", out var arr) ||
                arr.ValueKind != JsonValueKind.Array)
                return;

            List<(int, int, PlantType)> list = new();

            foreach (var item in arr.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Array || item.GetArrayLength() < 3)
                    continue;

                int row = item[0].GetInt32();
                int col = item[1].GetInt32();

                PlantType pt;
                if (item[2].ValueKind == JsonValueKind.Number)
                    pt = (PlantType)item[2].GetInt32();
                else
                    Enum.TryParse(item[2].GetString(), true, out pt);
                Debug.Log($"Placed {item} at ({row}, {col})");
                list.Add((row, col, pt));
            }

            data.PrePlants = () => list;
        }

        private static List<PlantType> ParsePlantList(JsonElement arr)
        {
            List<PlantType> list = new();

            foreach (var item in arr.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number){
                    Debug.Log($"    -{item}");
                    list.Add((PlantType)item.GetInt32());
                }else if (item.ValueKind == JsonValueKind.String &&
                         Enum.TryParse(item.GetString(), true, out PlantType pt)){
                    Debug.Log($"    -{item}");
                    list.Add(pt);
                }else
                    Debug.LogWarning($"[CustomLevels] Unknown plant '{item}'");
            }

            return list;
        }

        // Zombies
        public static void ZombieList(JsonElement root, ref CustomLevelData data)
        {
            if (!root.TryGetProperty("ZombieList", out var arr) ||
                arr.ValueKind != JsonValueKind.Array)
                return;

            List<ZombieType> list = new();

            foreach (var item in arr.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number)
                {
                    list.Add((ZombieType)item.GetInt32());
                    Debug.Log($"    -{item}");
                }
                else if (item.ValueKind == JsonValueKind.String &&
                         Enum.TryParse(item.GetString(), true, out ZombieType zt))
                {
                    list.Add(zt);
                    Debug.Log($"    -{item}");
                }
            }
        
            data.ZombieList = () => list;
        }

        // Lane auto-detection
        public static int AutoDetectLaneCount(SceneType scene)
        {
            switch (scene)
            {
                // 6‑lane maps
                case SceneType.Pool:
                case SceneType.NightPool:
                case SceneType.SnowPool:
                case SceneType.SnowPool_night:
                case SceneType.Day_6:
                case SceneType.Night_6:
                case SceneType.Snow_6:
                case SceneType.ReversalPool:
                case SceneType.BigPool:
                case SceneType.River:
                case SceneType.Roof_Pool:
                case SceneType.RoofPool_dusk:
                case SceneType.RoofPool_night:
                case SceneType.SuperPool:
                case SceneType.SuperDay:
                    return 6;

                // Travel roof maps are 6 lanes
                case SceneType.Travel_roof:
                case SceneType.Travel_roof_dusk:
                case SceneType.Travel_roof_night:
                    return 6;

                // 5‑lane maps (including Beach variants)
                case SceneType.Day:
                case SceneType.Night:
                case SceneType.Roof:
                case SceneType.NightRoof:
                case SceneType.Snow:
                case SceneType.MidDay:
                case SceneType.MidMap:
                case SceneType.Day_bubble:
                case SceneType.Test_green:
                case SceneType.IZDay:
                case SceneType.BilliardBallDay:
                case SceneType.BilliardBallMidDay:
                case SceneType.PVPScaryPot:
                case SceneType.TreasureBeach:
                case SceneType.LavaBeach:
                case SceneType.NormalBeach:
                    return 5;

                // Special / unsupported maps → safest fallback
                case SceneType.Chess:
                case SceneType.AutoChess:
                case SceneType.LongMap:
                    return 5;

                default:
                    return 5;
            }
        }
    }
}
