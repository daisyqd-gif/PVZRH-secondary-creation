using System.Text.Json;
using System.Text.Json.Serialization;
using CustomPlantClass.CustomizeLib;
using UnityEngine;

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

        [Serializable]
        public sealed class CustomLevelJsonData
        {
            [NonSerialized]
            public const string PlaceholderName = "__CUSTOM_LEVEL_PLACEHOLDER_DO_NOT_PARSE__";
            [NonSerialized]
            public const string PlaceholderFileName = PlaceholderName + ".json";

            public string? Name { get; set; }
            public SceneType SceneType { get; set; } = SceneType.Day;
            public int? RowCount { get; set; }
            public int? Sun { get; set; }
            public int? WaveCount { get; set; }
            public int? ZombieHealthRate { get; set; }
            public bool? NeedSelectCard { get; set; }
            public MusicType MusicType { get; set; } = MusicType.Day;
            public Dictionary<string, bool>? BoardTag { get; set; }
            public List<int>? AdvBuffs { get; set; }
            public List<int>? Debuffs { get; set; }
            public List<UltiBuffJsonData>? UltiBuffs { get; set; }
            public List<PlantType>? ConveyBeltPlantTypes { get; set; }
            public List<PlantType>? SeedRainPlantTypes { get; set; }
            public List<PlantType>? PreSelectCards { get; set; }
            public List<PrePlantJsonData>? PrePlants { get; set; }
            public List<ZombieType>? ZombieList { get; set; }
        }

        [Serializable]
        [JsonConverter(typeof(UltiBuffJsonConverter))]
        public sealed class UltiBuffJsonData
        {
            public int Id { get; set; }
            public int Level { get; set; }
        }

        [Serializable]
        [JsonConverter(typeof(PrePlantJsonConverter))]
        public sealed class PrePlantJsonData
        {
            public int Row { get; set; }
            public int Column { get; set; }
            public PlantType PlantType { get; set; }
        }

        public sealed class FlexibleEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
        {
            public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number)
                    return (TEnum)Enum.ToObject(typeof(TEnum), reader.GetInt32());

                if (reader.TokenType == JsonTokenType.String &&
                    Enum.TryParse(reader.GetString(), true, out TEnum value))
                    return value;

                throw new JsonException($"Invalid {typeof(TEnum).Name} value.");
            }

            public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options) =>
                writer.WriteStringValue(value.ToString());
        }

        public sealed class FlexibleIntListConverter : JsonConverter<List<int>>
        {
            public override List<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException("Expected an integer array.");

                var values = new List<int>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        return values;
                    if (reader.TokenType == JsonTokenType.Number)
                        values.Add(reader.GetInt32());
                    else if (reader.TokenType == JsonTokenType.String &&
                             int.TryParse(reader.GetString(), out int value))
                        values.Add(value);
                }

                throw new JsonException("Unexpected end of integer array.");
            }

            public override void Write(Utf8JsonWriter writer, List<int> value, JsonSerializerOptions options) =>
                JsonSerializer.Serialize(writer, value, options);
        }

        public sealed class UltiBuffJsonConverter : JsonConverter<UltiBuffJsonData>
        {
            public override UltiBuffJsonData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException("Expected a two-item ultimate buff array.");

                reader.Read();
                int id = ReadInt(ref reader);
                reader.Read();
                int level = ReadInt(ref reader);
                reader.Read();
                if (reader.TokenType != JsonTokenType.EndArray)
                    throw new JsonException("Expected a two-item ultimate buff array.");

                return new UltiBuffJsonData { Id = id, Level = level };
            }

            private static int ReadInt(ref Utf8JsonReader reader) =>
                reader.TokenType == JsonTokenType.Number
                    ? reader.GetInt32()
                    : int.TryParse(reader.GetString(), out int value)
                        ? value
                        : throw new JsonException("Ultimate buff values must be integers.");

            public override void Write(Utf8JsonWriter writer, UltiBuffJsonData value, JsonSerializerOptions options)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(value.Id);
                writer.WriteNumberValue(value.Level);
                writer.WriteEndArray();
            }
        }

        public sealed class PrePlantJsonConverter : JsonConverter<PrePlantJsonData>
        {
            public override PrePlantJsonData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException("Expected a three-item pre-plant array.");

                reader.Read();
                int row = ReadInt(ref reader);
                reader.Read();
                int column = ReadInt(ref reader);
                reader.Read();
                PlantType plantType = ReadPlantType(ref reader);
                reader.Read();
                if (reader.TokenType != JsonTokenType.EndArray)
                    throw new JsonException("Expected a three-item pre-plant array.");

                return new PrePlantJsonData { Row = row, Column = column, PlantType = plantType };
            }

            private static int ReadInt(ref Utf8JsonReader reader) =>
                reader.TokenType == JsonTokenType.Number
                    ? reader.GetInt32()
                    : int.TryParse(reader.GetString(), out int value)
                        ? value
                        : throw new JsonException("Pre-plant row and column must be integers.");

            private static PlantType ReadPlantType(ref Utf8JsonReader reader)
            {
                if (reader.TokenType == JsonTokenType.Number)
                    return (PlantType)reader.GetInt32();
                if (reader.TokenType == JsonTokenType.String &&
                    Enum.TryParse(reader.GetString(), true, out PlantType value))
                    return value;
                throw new JsonException("Invalid plant type.");
            }

            public override void Write(Utf8JsonWriter writer, PrePlantJsonData value, JsonSerializerOptions options)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(value.Row);
                writer.WriteNumberValue(value.Column);
                writer.WriteStringValue(value.PlantType.ToString());
                writer.WriteEndArray();
            }
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
                case SceneType.ShootingDay:
                case SceneType.Swamp:
                case SceneType.NightWinter:
                //    return 5;

                // Special / unsupported maps → safest fallback
                case SceneType.Chess:
                case SceneType.AutoChess:
                case SceneType.LongMap:
                //    return 5;

                default:
                    return 5;
            }
        }
    }
    public static class LoadLevel
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        static LoadLevel()
        {
            Options.Converters.Add(new Parse.FlexibleEnumConverter<SceneType>());
            Options.Converters.Add(new Parse.FlexibleEnumConverter<MusicType>());
            Options.Converters.Add(new Parse.FlexibleEnumConverter<PlantType>());
            Options.Converters.Add(new Parse.FlexibleEnumConverter<ZombieType>());
            Options.Converters.Add(new Parse.FlexibleIntListConverter());
        }

        public static void SerializePlaceholder(string folder)
        {
            string path = Path.Combine(folder, Parse.CustomLevelJsonData.PlaceholderFileName);
            if (File.Exists(path))
                return;

            Parse.CustomLevelJsonData config = new()
            {
                Name = Parse.CustomLevelJsonData.PlaceholderName,
                SceneType = SceneType.Day,
                RowCount = 5,
                Sun = 1000,
                WaveCount = 10,
                ZombieHealthRate = 1,
                NeedSelectCard = true,
                MusicType = MusicType.Day,
                BoardTag = [],
                AdvBuffs = [],
                Debuffs = [],
                UltiBuffs = [],
                ConveyBeltPlantTypes = [],
                SeedRainPlantTypes = [],
                PreSelectCards = [],
                PrePlants = [],
                ZombieList = []
            };

            var writeOptions = new JsonSerializerOptions(Options)
            {
                WriteIndented = true
            };
            File.WriteAllText(path, JsonSerializer.Serialize(config, writeOptions));
        }

        public static CustomLevelData FromJson(string path)
        {
            Debug.Log($"[CustomLevels] Parsing {Path.GetFileName(path)}");

            string json = File.ReadAllText(path);
            Parse.CustomLevelJsonData? config = JsonSerializer.Deserialize<Parse.CustomLevelJsonData>(json, Options)
                ?? throw new JsonException("Custom level JSON must contain an object.");
            if (string.Equals(config.Name, Parse.CustomLevelJsonData.PlaceholderName, StringComparison.Ordinal))
                throw new JsonException($"The reserved placeholder name '{Parse.CustomLevelJsonData.PlaceholderName}' cannot be loaded as a custom level.");
            CustomLevelData data = new()
            {
                Name = () => string.IsNullOrWhiteSpace(config.Name)
                    ? Path.GetFileNameWithoutExtension(path)
                    : config.Name,
                SceneType = config.SceneType,
                RowCount = config.RowCount ?? Parse.AutoDetectLaneCount(config.SceneType),
                Sun = () => config.Sun ?? 1000,
                WaveCount = () => config.WaveCount ?? 10,
                ZombieHealthRate = () => config.ZombieHealthRate ?? 1,
                NeedSelectCard = config.NeedSelectCard ?? true,
                BgmType = config.MusicType
            };

            if (config.BoardTag != null)
            {
                Board.BoardTag tag = new();
                foreach (var field in typeof(Board.BoardTag).GetFields())
                {
                    if (field.FieldType != typeof(bool))
                        continue;
                    if (config.BoardTag.TryGetValue(field.Name, out bool value))
                        field.SetValueDirect(__makeref(tag), value);
                }
                data.BoardTag = tag;
                if (tag.isConvey)
                    data.NeedSelectCard = false;
            }

            if (config.AdvBuffs != null)
                data.AdvBuffs = () => config.AdvBuffs;
            if (config.Debuffs != null)
                data.Debuffs = () => config.Debuffs;
            if (config.UltiBuffs != null)
                data.UltiBuffs = () => config.UltiBuffs
                    .Select(buff => (buff.Id, buff.Level))
                    .ToList();
            if (config.ConveyBeltPlantTypes != null)
                data.ConveyBeltPlantTypes = () => config.ConveyBeltPlantTypes;
            if (config.SeedRainPlantTypes != null)
                data.SeedRainPlantTypes = () => config.SeedRainPlantTypes;
            if (config.PreSelectCards != null)
                data.PreSelectCards = () => config.PreSelectCards;
            if (config.PrePlants != null)
                data.PrePlants = () => config.PrePlants
                    .Select(plant => (plant.Row, plant.Column, plant.PlantType))
                    .ToList();
            if (config.ZombieList != null)
                data.ZombieList = () => config.ZombieList;

            return data;
        }
    }
}
