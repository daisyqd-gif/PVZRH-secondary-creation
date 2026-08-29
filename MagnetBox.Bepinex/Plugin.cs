global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
//global using HarmonyLib;
global using System;
global using System.Collections.Generic;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass.Main;

namespace MagnetBox_MagnetInterface
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "MagnetBox_MagnetInterface.Bepinex";
        public const string PluginName = "MagnetBox_MagnetInterface";
        public const string PluginVersion = "3.5";

        public override void Load()
        {
            try
            {
                // Apply all Harmony patches in this assembly
                //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

                // Load the AssetBundle containing your plant prefab(s)
                // Replace "abname" with your actual bundle name
                AssetBundle assetBundle = CustomCore.GetAssetBundle(
                    Assembly.GetExecutingAssembly(),
                    "magnetbox"
                );
                ID boxID = DataMgr.AllocateID();
                CustomCore.RegisterCustomPlant<MagnetBox>(boxID, assetBundle.GetAsset<GameObject>("MagnetBoxPrefab"), assetBundle.GetAsset<GameObject>("MagnetBoxPreview"), new List<(int, int)>(), 0f, 0f, 0, 300, 7.5f, 100);
                CustomCore.TypeMgrExtra.LevelPlants.Add(boxID, CardLevel.White);
                CustomCore.RegisterCustomCardToColorfulCards(boxID);
                CustomCore.TypeMgrExtra.IsMagnetPlants.Add(boxID);

                ID interfaceID = DataMgr.AllocateID();
                CustomCore.RegisterCustomPlant<MagnetInterface>(interfaceID, assetBundle.GetAsset<GameObject>("MagnetInterfacePrefab"), assetBundle.GetAsset<GameObject>("MagnetInterfacePreview"), new List<(int, int)>() { ((int)PlantType.Magnetshroom, boxID), (boxID, (int)PlantType.Magnetshroom) }, 0f, 0f, 0, 300, 7.5f, 225);
                CustomCore.TypeMgrExtra.LevelPlants.Add(interfaceID, CardLevel.White);
                CustomCore.TypeMgrExtra.IsMagnetPlants.Add(interfaceID);
            }
            catch (Exception e)
            {
                DataMgr.StartUpMessages.Add($"Magnet box mod load failed.\n{e.ToString}");
            }
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
}
