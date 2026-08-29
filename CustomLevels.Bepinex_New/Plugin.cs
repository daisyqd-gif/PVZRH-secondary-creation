// Global imports for the entire loader
global using CustomizeLib.BepInEx;
global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using UnityEngine;
global using System;
global using System.IO;
global using System.Text.Json;
global using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Threading.Tasks.Dataflow;
using System.Collections;
using System.Security.Cryptography;

namespace CustomLevels.Bepinex
{
    [BepInPlugin("customlevels.bepinex", "Custom JSON Levels", "3.0.0")]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            Debug.Log("[CustomLevels] Loader initialized.");

            string folder = Path.Combine(Paths.PluginPath, "CustomLevels");
            Directory.CreateDirectory(folder);

            foreach (string file in Directory.GetFiles(folder, "*.json"))
            {
                try
                {
                    var data = LoadLevel.FromJson(file);
                    data.PostBoard=(Board b)=>{
                        try{
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
                                bool v = (bool)f.GetValue(tag);
                                if (v)
                                    Debug.Log($"    BoardTag.{f.Name} = true");
                            }
                        }
                        Debug.Log(" ");
                        Debug.Log("Custom level loaded successfully! Have fun!");
                        }
                        catch(Exception e)
                        {
                            Debug.Log($"Custom level loaded with errors:\n{e.Message}\nPlease debug and try again.");
                        }
                    };
                    int id = CustomCore.RegisterCustomLevel(data);
                    Debug.Log($"[CustomLevels] Loaded level {data.Name()} (ID {id})");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[CustomLevels] Failed to load {file}: {ex}");
                }
            }
        }
    }
}
