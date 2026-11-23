using CustomPlantClass.CustomizeLib;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine;

namespace CustomLevels.Bepinex
{
    public class CLevel_Load_Plugin
    {
        public static void Load()
        {}
        public static void NotRun(){
            Debug.Log("[CustomLevels] Loader initialized.");

            string folder = Path.Combine(Paths.PluginPath, "CustomLevels");
            Directory.CreateDirectory(folder);
            LoadLevel.SerializePlaceholder(folder);
            Debug.Log($"[CustomLevels] Wrote placeholder level: {Parse.CustomLevelJsonData.PlaceholderFileName}");

            foreach (string file in Directory.GetFiles(folder, "*.json"))
            {
                if (string.Equals(
                    Path.GetFileNameWithoutExtension(file),
                    Parse.CustomLevelJsonData.PlaceholderName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"[CustomLevels] Skipped placeholder file {Path.GetFileName(file)}");
                    continue;
                }

                try
                {
                    var data = LoadLevel.FromJson(file);
                    data.PostBoard=(Board b)=>{
                        try
                        {
                            foreach(int i in data.AdvBuffs())
                            {
                                TravelMgr.Instance.GetNormalBuff((AdvBuff) i);
                                Debug.Log($"Registered AdvBuff {i}");
                            }
                            foreach((int,int) i in data.UltiBuffs())
                            {
                                TravelMgr.Instance.GetUltiBuff((UltiBuff) i.Item1,false);
                                Debug.Log($"Registered Ultimate Buff {i}");
                                if (i.Item2 > 1)
                                {
                                    TravelMgr.Instance.GetUltiBuff((UltiBuff) i.Item1,true);
                                    Debug.Log($"Registered Ultimate Buff {i} level 2");
                                }
                            }
                            foreach(int i in data.Debuffs())
                            {
                                TravelMgr.Instance.GetDebuff((TravelDebuff) i);
                                Debug.Log($"Registered Debuff {i}");
                            }
                            var tag = b.boardTag;
                            var fields = typeof(Board.BoardTag).GetFields();

                            Debug.Log($"BoardTag:");
                            foreach (var f in fields)
                            {
                                if (f.FieldType == typeof(bool))
                                {
                                    bool v = (bool)f.GetValue(tag)!;
                                    if (v)
                                        Debug.Log($"    BoardTag.{f.Name} = true");
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.Log($"Custom level loaded with errors:\n{e.Message}\nPlease debug and try again.");
                        }
                    };
                    int id = CustomCore.RegisterCustomLevel(data);
                    Debug.Log($"[CustomLevels] Loaded level {data.Name()} (ID {id})");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[CustomLevels] Failed to load {file}: {ex}");
                }
            }
        }
    }
}
