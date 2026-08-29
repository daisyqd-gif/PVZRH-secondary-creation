global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using HarmonyLib;
global using Il2CppInterop.Runtime.Injection;
global using System.Reflection;
global using UnityEngine;
global using System.Collections.Generic;
global using System.Linq;
global using GameLevel.RogueShooting;
using System;
namespace RogueShootingRandomFormation
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public static List<ZombieType> RandomList = new();
        public static List<ZombieType> UltimateRandomList = new();
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
        public static void LoadMod()
        {
            RandomList= new List<ZombieType>([..GameAPP.resourcesManager.allZombieTypes])
            .Where((ZombieType type) => 
            !TypeMgr.WaterZombie(type) && 
            !TypeMgr.IsBossZombie(type) && 
            !TypeMgr.UltimateZombie(type) && 
            !TypeMgr.IsAirZombie(type) &&
            !TypeMgr.BannedInRandomZombies(type) &&
            type != ZombieType.BedRockSnowZombie &&
            type != ZombieType.SnowDrownZombie &&
            type != ZombieType.RandomZombie &&
            type != ZombieType.RandomPlusZombie &&
            type != ZombieType.DiamondRandomZombie &&
            type != ZombieType.TrainingDummy &&
            type != ZombieType.EndoFlameZombie).ToList();
            UltimateRandomList= new List<ZombieType>([.. GameAPP.resourcesManager.allZombieTypes])
            .Where((ZombieType type) =>
            !TypeMgr.WaterZombie(type) &&
            !TypeMgr.IsBossZombie(type) &&
            TypeMgr.UltimateZombie(type) &&
            !TypeMgr.IsAirZombie(type) &&
            !TypeMgr.BannedInRandomZombies(type) &&
            type != ZombieType.BedRockSnowZombie &&
            type != ZombieType.SnowDrownZombie &&
            type != ZombieType.RandomZombie &&
            type != ZombieType.RandomPlusZombie &&
            type != ZombieType.DiamondRandomZombie &&
            type != ZombieType.TrainingDummy &&
            type != ZombieType.EndoFlameZombie).ToList();
        }
    }
    [HarmonyPatch(typeof(GameAPP))]
    public static class GameAPP_Patch
    {
        [HarmonyPatch(nameof(GameAPP.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix()
        {
            Plugin.LoadMod();
        }
    }
    [HarmonyPatch(typeof(ShootingManager))]
    public static class ShootingManager_Patch
    {
        [HarmonyPatch(nameof(ShootingManager.GetZombieType))]
        [HarmonyPrefix]
        public static bool GetZombieType_Prefix(ShootingManager __instance, ref ZombieType __result, int wave, int waveToAdd = 5)
        {
            if(ShootingManager.randomType == RandomZombieType.Random)
            {
                if(wave < 5)
                {
                    __result = ZombieType.NormalZombie;
                    goto done;
                }
                if(wave < 15)
                {
                    __result = new List<ZombieType>() { ZombieType.NormalZombie, ZombieType.RandomZombie }.OrderBy(_ => UnityEngine.Random.value).First();
                    goto done;
                }
                if(wave < 30)
                {
                    __result = new List<ZombieType>() { ZombieType.NormalZombie, ZombieType.RandomZombie, ZombieType.RandomPlusZombie }.OrderBy(_ => UnityEngine.Random.value).First();
                    goto done;
                }
                else
                {
                    __result = new List<ZombieType>() { ZombieType.NormalZombie, ZombieType.RandomZombie, ZombieType.RandomPlusZombie, ZombieType.DiamondRandomZombie }.OrderBy(_ => UnityEngine.Random.value).First();
                    goto done;
                }
                done:
                return false;
            }
            return true;
        }
        [HarmonyPatch(nameof(ShootingManager.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(ShootingManager __instance)
        {
            if((int)ShootingManager.randomType == 1000)
            {
                Board.Instance.config.applyRandomData = true;

                Board.Instance.config.zombieScaleAvg = 1;
                Board.Instance.config.zombieScaleMax = 1;
                Board.Instance.config.zombieScaleMin = 1;

                Board.Instance.config.zombieSpeedAvg = 0.5f;
                Board.Instance.config.zombieSpeedMax = 1;
                Board.Instance.config.zombieSpeedMin = 2f;

                Board.Instance.config.zombieModifyAvg = 0.5f;
                Board.Instance.config.zombieModifyMax = 1;
                Board.Instance.config.zombieModifyMin = 2f;
            }
        }
        [HarmonyPatch(nameof(ShootingManager.RandomSettings))]
        [HarmonyPostfix]
        public static void RandomSettings_Postfix()
        {
            // Get the enum list again (managed)
            var list = Enum.GetValues<RandomZombieType>().ToList();

            // OPTIONAL: re-randomize using the corrected list
            var chosen = list[UnityEngine.Random.Range(0, list.Count)];

            ShootingManager.randomType = chosen;
        }
    }
    [HarmonyPatch(typeof(RandomZombie))]
    public static class RandomZombie_Patch
    {
        [HarmonyPatch(nameof(RandomZombie.SetRandomZombie))]
        [HarmonyPrefix]
        public static bool SetRandomZombie_Prefix(RandomZombie __instance, ref Zombie __result, Vector3 pos)
        {
            if(!__instance.board.boardTag.rogueShooting) return true;
            // If your custom list is empty, let the original run
            if (Plugin.RandomList.Count == 0)
                return true;

            var cz = CreateZombie.Instance;
            if (cz == null)
                return true;

            int row = __instance.theZombieRow;

            // Pick random from your list
            ZombieType chosen = Plugin.RandomList[
                UnityEngine.Random.Range(0, Plugin.RandomList.Count)
            ];

            // Spawn
            if (!__instance.isMindControlled)
                __result = cz.SetZombie(row, chosen, pos.x, false);
            else
                __result = cz.SetZombieWithMindControl(row, chosen, pos.x, false);

            return false; // skip original SetRandomZombie
        }
    }
    [HarmonyPatch(typeof(DiamondRandomZombie))]
    public static class DiamondRandomZombie_Patch
    {
        [HarmonyPatch(typeof(DiamondRandomZombie), nameof(DiamondRandomZombie.SetRandomZombie))]
        [HarmonyPrefix]
        public static bool DiamondRandomZombie_SetRandomZombie_Prefix(
            DiamondRandomZombie __instance,
            ref Zombie __result,
            Vector3 pos)
        {
            if(!__instance.board.boardTag.rogueShooting) return true;
            if (Plugin.UltimateRandomList.Count == 0)
                return true;

            var cz = CreateZombie.Instance;
            if (cz == null)
                return true;

            int row = __instance.theZombieRow;

            ZombieType chosen = Plugin.UltimateRandomList[
                UnityEngine.Random.Range(0, Plugin.UltimateRandomList.Count)
            ];

            if (!__instance.isMindControlled)
                __result = cz.SetZombie(row, chosen, pos.x, false);
            else
                __result = cz.SetZombieWithMindControl(row, chosen, pos.x, false);

            return false;
        }
    }
    [HarmonyPatch(typeof(FreezedPlant))]
    public class FreezedPlant_Patch
    {
        [HarmonyPatch(nameof(FreezedPlant.CanFreeze))]
        [HarmonyPrefix]
        public static bool CanFreeze_Prefix(
            Plant plant, ref bool __result)
        {
            if(!Board.Instance.boardTag.rogueShooting) return true;
            __result = false;
            return false;
        }
        [HarmonyPatch(nameof(FreezedPlant.CanFreezeFire))]
        [HarmonyPrefix]
        public static bool CanFreezeFire_Prefix(
            Plant plant, ref bool __result)
        {
            if(!Board.Instance.boardTag.rogueShooting) return true;
            __result = false;
            return false;
        }
    }
    [HarmonyPatch(typeof(Fertilize))]
    public static class Fertilize_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Fertilize.Use), [typeof(int),typeof(int)])]
        public static bool Use_Prefix(Fertilize __instance, int theColumn, int theRow)
        {
            // If this is zombie fertilizer AND rogue shooting is active → skip everything
            if (__instance.zombie && Board.Instance.boardTag.rogueShooting)
            {
                __instance.Die();
                return false; // do nothing
            }

            return true; // run original
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "RogueShootingRandomFormation.Bepinex";
        public const string PluginName = "RogueShootingRandomFormation";
        public const string PluginVersion = "3.9";
    }
}
