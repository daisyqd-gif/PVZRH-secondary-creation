global using BepInEx;
global using System;
global using UnityEngine;
global using System.Collections.Generic;
global using CustomPlantClass;
global using CustomPlantClass.Main;
global using System.Text.Json;
using HarmonyLib;
namespace PlantMixTreeCreator
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        public static void OnInit()
        {
            try
            {
                // 2. Build JSON list
                var jsonList = new List<PlantDump>();

                foreach (var kv in PlantMixTreeManager.PlantMixTrees)
                {
                    var node = kv.Value;
                    var id = node.PlantType;
                    var name = Lawnf.GetName(id);

                    var children = node.DirectChildren;
                    int[] childIds = new int[children.Count];
                    for (int i = 0; i < children.Count; i++)
                        childIds[i] = (int)children[i].PlantType;

                    jsonList.Add(new PlantDump
                    {
                        Id = (int)id,
                        Name = name,
                        Children = childIds,
                        IsBase = node.IsBasicPlant
                    });
                }

                // ⭐ Sort by ID
                jsonList.Sort((a, b) => a.Id.CompareTo(b.Id));

                // 3. Serialize
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(new { Plants = jsonList }, options);

                string path = System.IO.Path.Combine(Application.persistentDataPath, "fusion_dump.json");
                System.IO.File.WriteAllText(path, json);

                ModLogger.LogInfo($"Fusion tree dumped to: {path}");
            }
            catch (Exception ex)
            {
                ModLogger.LogError($"Fusion dump failed: {ex}");
            }
        }
        private static int[] ConvertChildren(List<PlantType> list)
        {
            int count = list.Count;
            int[] arr = new int[count];
            for (int i = 0; i < count; i++)
                arr[i] = (int)list[i];
            return arr;
        }
    }
    // Helper class for System.Text.Json
    [Serializable]
    public class PlantDump
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] Children { get; set; }
        public bool IsBase { get; set; }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "PlantMixTreeCreator.Bepinex";
        public const string PluginName = "PlantMixTreeCreator";
        public const string PluginVersion = "3.7";
    }
    [HarmonyPatch(typeof(PlantMixTreeManager), nameof(PlantMixTreeManager.Init))]
    public static class PlantMixTreeManager_Init_Patch
    {
        [HarmonyPostfix]
        public static void AfterInit()
        {
            Plugin.OnInit();
        }
    }
}
