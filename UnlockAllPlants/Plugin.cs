global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using Il2CppInterop.Runtime.Injection;
global using System;
global using System.Reflection;
global using UnityEngine;
global using System.Collections;
global using System.Collections.Generic;
global using CustomPlantClass;

namespace UnlockAllPlants
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : BasePlugin
    {

        public override void Load()
        {
            try{
                // Apply all Harmony patches in this assembly
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

                // Register the custom plant class with IL2CPP
                // (Required for all custom MonoBehaviours)
                ClassInjector.RegisterTypeInIl2Cpp<UnlockAllPlants>();
                AddComponent<UnlockAllPlants>();

                Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
                DataMgr.StartUpMessages.Add("Press page down on the keyboard to unlock all almanac entries");
            }
            catch (Exception e)
            {
                DataMgr.StartUpMessages.Add(MyPluginInfo.PluginName+" load failed.\n"+e.ToString());
            }
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class UnlockAllPlants : MonoBehaviour
    {
        public float LastKeyDownTime;
        public static bool completed = false;

        public void Update()
        {
            // Single press, no double-tap timing
            if (!Input.GetKeyDown(KeyCode.PageDown))
                return;

            if (completed)
                return;

            if (Board.Instance == null)
                return;

            completed = true;
            Board.Instance.StartCoroutine(
                PlantAllTypes(GameAPP.resourcesManager.allPlants.ToSystemList())
            );
        }

        public IEnumerator PlantAllTypes(List<PlantType> plants)
        {
            foreach (var type in plants)
            {
                if (Board.Instance == null)
                    break;

                // 3.6: if CreatePlant signature changed, swap to PlantMgr.CreatePlant
                var plant = CreatePlant.Instance.SetPlant(
                    0, 0, type,
                    isFreeSet: true,
                    withEffect: false
                );

                if (plant != null)
                {
                    // Let registration happen for a frame
                    yield return null;
                    plant.Die();
                }

                yield return null;
            }

            completed = false;
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "UnlockAllPlants.Bepinex";
        public const string PluginName = "UnlockAllPlants";
        public const string PluginVersion = "3.5";
    }
}
