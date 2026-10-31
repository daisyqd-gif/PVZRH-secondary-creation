using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using CustomPlantClass;
using UnityEngine.Rendering;
using CustomPlantClass.Main;
using CustomPlantClass.Runtime.Tasks;

namespace UltimatePlanternSkin
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, CustomPlantClass.MyPluginInfo.TargetVersion)]
    public class Plugin : BasePlugin
    {
        public static ID particletype;
        public override void Load()
        {
            try
            {
                // Apply all Harmony patches in this assembly
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
                particletype = DataMgr.AllocateID();

                // Load the AssetBundle containing your plant prefab(s)
                // Replace "abname" with your actual bundle name
                AssetBundle assetBundle = CustomCore.GetAssetBundle(
                    Assembly.GetExecutingAssembly(),
                    "skin_949_0"
                );

                CustomCore.RegisterCustomPlantSkin
                (
                    (int)PlantType.UltimatePlantern, 
                    assetBundle.GetAsset<GameObject>("UltimatePlanternPrefab"), 
                    assetBundle.GetAsset<GameObject>("UltimatePlanternPreview"),
                    (UltimatePlantern p) =>
                    {
                        SpriteRenderer sr = p.transform.FindChild("UltimateLight").GetComponent<SpriteRenderer>();
                        sr.material = Resources.Load<Material>("shaders/Brightness.mat");
                        p.lanternLight = p.transform.FindChild("Light").gameObject;
                        p.shoot = p.transform.FindChild("Shoot");
                        p.shoot2 = p.transform.FindChild("Shoot2");
                        p.laserPrefab = Resources.Load<GameObject>("plants/peashooter/lanternpea/Laser.prefab");
                        p.ultimateLight = sr;
                    }
                );

                GameObject obj=assetBundle.GetAsset<GameObject>("AuroraVision");
                CustomCore.RegisterCustomParticle(particletype, obj);
                SortingGroup group=obj.GetComponent<SortingGroup>();
                group.sortingLayerName="up2";

                Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
            }
            catch (Exception e)
            {
                DataMgr.StartUpMessages.Add(MyPluginInfo.PluginName + " load failed.\n" + e.ToString());
            }
        }
    }
    [HarmonyPatch(typeof(UltimatePlantern), nameof(UltimatePlantern.Shrink))]
    public static class Shrink_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(UltimatePlantern __instance)
        {
            Aurora();
        }
        public static async void Aurora()
        {
            try
            {
                for( int i = 0 ;i < 50; i ++ )
                {
                    // Spawn 1 aurora
                    int row = PlantMgr.GetRandomBoardRow();
                    int col = PlantMgr.GetRandomBoardColumn();

                    if (row >= 0 && col >= 0 && ParticleManager.Instance != null)
                    {
                        var pos = PlantMgr.GetPos(row, col);
                        CreateParticle.SetParticle(Plugin.particletype, pos, row);
                    }

                    await DelayTask.Delay(0.5f);
                }
            }
            catch(Exception e)
            {
                global::CustomPlantClass.Runtime.Core.ModLogger.LogError(e.ToString());
            }
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimatePlanternSkin.Bepinex";
        public const string PluginName = "UltimatePlanternSkin";
        public const string PluginVersion = "4.0";
    }
}
