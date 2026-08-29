namespace CustomLevels.Bepinex
{
    public static class LoadLevel
    {
        /*private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };*/

        public static CustomLevelData FromJson(string path)
        {
            Debug.Log($"[CustomLevels] Parsing {Path.GetFileName(path)}");

            string json = File.ReadAllText(path);
            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            CustomLevelData data = new();

            // Name
            if (root.TryGetProperty("Name", out var nameElem))
                data.Name = () => nameElem.GetString();
            else
                data.Name = () => Path.GetFileNameWithoutExtension(path);

            // SceneType
            Parse.GetSceneType(root, ref data);

            // RowCount
            if (root.TryGetProperty("RowCount", out var rowElem))
                data.RowCount = rowElem.GetInt32();
            else
                //data.RowCount = 6;
                data.RowCount = Parse.AutoDetectLaneCount(data.SceneType);

            // Sun
            if (root.TryGetProperty("Sun", out var sunElem))
                data.Sun = () => sunElem.GetInt32();
            else
                data.Sun = () => 1000;

            // WaveCount
            if (root.TryGetProperty("WaveCount", out var waveElem))
                data.WaveCount = () => waveElem.GetInt32();
            else
                data.WaveCount = () => 10;

            // ZombieHealthRate
            if (root.TryGetProperty("ZombieHealthRate", out var zhElem))
                data.ZombieHealthRate = () => zhElem.GetInt32();
            else
                data.ZombieHealthRate = () => 1;

            // NeedSelectCard
            if (root.TryGetProperty("NeedSelectCard", out var nscElem))
                data.NeedSelectCard = nscElem.GetBoolean();

            // MusicType
            Parse.Music(root, ref data);

            // BoardTag
            Parse.BoardTag(root, ref data);

            // Buffs
            Parse.AdvBuffs(root, ref data);
            Parse.Debuffs(root, ref data);
            Parse.UltiBuffs(root, ref data);

            // Plants
            Parse.Conveyor(root, ref data);
            Parse.SeedRain(root, ref data);
            Parse.PreSelectCards(root, ref data);
            Parse.PrePlants(root, ref data);

            // Zombies
            Parse.ZombieList(root, ref data);

            return data;
        }
    }
}
