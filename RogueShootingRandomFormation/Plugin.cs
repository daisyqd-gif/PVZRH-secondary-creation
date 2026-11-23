using BepInEx;
using BepInEx.Unity.IL2CPP;
using GameLevel.RogueShooting;
using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace RogueShootingRandomFormation
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public partial class Plugin : BasePlugin
    {
        public static RandomZombieType mixed;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }
    
    [HarmonyPatch(typeof(GameAPP))]
    public static class GameAPP_Patch
    {
        [HarmonyPatch(nameof(GameAPP.Start))]
        [HarmonyPostfix]
        public static void GetZombieType_Prefix()
        {
            Plugin.mixed=(RandomZombieType)1000;
        }
    }
    [HarmonyPatch(typeof(ShootingManager))]
    public static class ShootingManager_Patch
    {
        public static T GetRandom<T>(this IEnumerable<T> self)
            => self.ElementAt(UnityEngine.Random.Range(0,self.Count()));
        
        [HarmonyPatch(nameof(ShootingManager.GetZombieType))]
        [HarmonyPrefix]
        public static bool GetZombieType_Prefix(ShootingManager __instance, ref ZombieType __result, int wave, int waveToAdd = 5)
        {
            if(ShootingManager.randomType == RandomZombieType.Random)
            {
                if(wave < 5)
                    __result = ZombieType.NormalZombie;
                else if(wave < 30)
                    __result = new List<ZombieType>() { ZombieType.NormalZombie, ZombieType.RandomZombie }.GetRandom();
                else if(wave < 60)
                    __result = new List<ZombieType>() { ZombieType.NormalZombie, ZombieType.RandomZombie, ZombieType.RandomPlusZombie }.GetRandom();
                else
                    __result = new List<ZombieType>() { ZombieType.NormalZombie, ZombieType.RandomZombie, ZombieType.RandomPlusZombie, ZombieType.DiamondRandomZombie }.GetRandom();
                return false;
            }
            if(ShootingManager.randomType == Plugin.mixed)
            {
                if(wave < 5)
                    __result = ZombieType.NormalZombie;
                else if(wave < 30)
                    __result = Plugin.RandomList.GetRandom();
                else
                    __result = Plugin.MixedRandomList.GetRandom();
                return false;
            }
            return true;
        }
        [HarmonyPatch(nameof(ShootingManager.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(ShootingManager __instance)
        {
            if(ShootingManager.randomType == RandomZombieType.Random)
            {
                __instance.board.config.applyRandomData = true;

                __instance.board.config.zombieScaleAvg = 1;
                __instance.board.config.zombieScaleMax = 1;
                __instance.board.config.zombieScaleMin = 1;

                __instance.board.config.zombieSpeedAvg = 0.5f;
                __instance.board.config.zombieSpeedMax = 1f;
                __instance.board.config.zombieSpeedMin = 1.5f;

                __instance.board.config.zombieModifyAvg = 0.5f;
                __instance.board.config.zombieModifyMin = 1f;
                __instance.board.config.zombieModifyMax = 1.5f;

                __instance.board.config.plantSpeedMax = 1f;
                __instance.board.config.plantSpeedMin = 1f;
                __instance.board.config.plantSpeedAvg = 1f;

                __instance.board.config.plantModifyMin = 1f;
                __instance.board.config.plantModifyMax = 1f;
            }
            if(ShootingManager.randomType == Plugin.mixed)
            {
                ShootingManager.randomType = RandomZombieType.Random;
                __instance.shieldHealth += 15000;
            }
        }
        [HarmonyPatch(nameof(ShootingManager.RandomSettings))]
        [HarmonyPostfix]
        public static void RandomSettings_Postfix()
            => ShootingManager.randomType = Enum.GetValues<RandomZombieType>().Union([Plugin.mixed]).GetRandom();
    }
    [HarmonyPatch(typeof(RandomZombie))]
    public static class RandomZombie_Patch
    {
        [HarmonyPatch(nameof(RandomZombie.SetRandomZombie))]
        [HarmonyPrefix]
        public static bool SetRandomZombie_Prefix(RandomZombie __instance, ref Zombie __result, Vector3 pos)
        {
            if(!__instance.board.boardTag.rogueShooting) return true;
            if(__instance is RandomPlusZombie)
            {
                __result = CreateZombie.Instance.SetZombie(__instance.theZombieRow, Plugin.MixedRandomList.GetRandom(), pos.x, __instance.isMindControlled);

                return false;
            }

            __result = CreateZombie.Instance.SetZombie(__instance.theZombieRow, Plugin.RandomList.GetRandom(), pos.x, __instance.isMindControlled);

            return false;
        }
        [HarmonyPatch(nameof(RandomZombie.RandomEvent))]
        [HarmonyPrefix]
        public static bool RandomEvent_Prefix(RandomZombie __instance) => !__instance.board.boardTag.rogueShooting;
    }
    [HarmonyPatch(typeof(DiamondRandomZombie))]
    public static class DiamondRandomZombie_Patch
    {
        [HarmonyPatch(nameof(DiamondRandomZombie.SetRandomZombie))]
        [HarmonyPrefix]
        public static bool SetRandomZombie_Prefix(
            DiamondRandomZombie __instance,
            ref Zombie __result,
            Vector3 pos)
        {
            if(!__instance.board.boardTag.rogueShooting) return true;

            __result = CreateZombie.Instance.SetZombie(__instance.theZombieRow, Plugin.UltimateRandomList.GetRandom(), pos.x, __instance.isMindControlled);

            return false;
        }
    }
    public static class MyPluginInfo
    {
        public const string PluginGuid = "RogueShootingRandomFormation.Bepinex";
        public const string PluginName = "RogueShootingRandomFormation";
        public const string PluginVersion = "4.0";
    }
}
