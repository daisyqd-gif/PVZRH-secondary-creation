using BepInEx;
using BepInEx.Unity.IL2CPP;
using GameLevel.RogueShooting;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Il2CppInterop.Runtime;
using System.Runtime.CompilerServices;
namespace RogueShootingRandomFormation
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public static IReadOnlyList<ZombieType> RandomList = 
        [
            ZombieType.NormalZombie,
            ZombieType.ConeZombie,
            ZombieType.PaperZombie,
            ZombieType.BucketZombie,
            ZombieType.DoorZombie,
            ZombieType.PolevaulterZombie,
            ZombieType.BucketPaper,
            ZombieType.DancePolZombie,
            ZombieType.CoachPaper,
            ZombieType.JacksonZombie,
            ZombieType.DriverZombie,
            ZombieType.FootballZombie,
            ZombieType.Gargantuar,
            ZombieType.FlagFootball,
            ZombieType.DollDiamond,
            ZombieType.PogoZombie,
            ZombieType.ElitePaperZombie,
            ZombieType.MachineNutZombie,
            ZombieType.TallNutFootballZombie,
            ZombieType.RedGargantuar,
            ZombieType.SuperPogoZombie,
            ZombieType.SuperJackboxZombie,
            ZombieType.LadderZombie,
            ZombieType.SuperLadderZombie,
            ZombieType.IronGargantuar,
            ZombieType.NewYearZombie,
            ZombieType.HorseZombie,
            ZombieType.PenguinZombie,
            ZombieType.SuperPenguinZombie,
            ZombieType.ElephantZombie,
            ZombieType.SuperPolevaulter,
            ZombieType.PeaShooterZombie,
            ZombieType.CherryPaperZombie,
            ZombieType.CherryShooterZombie,
            ZombieType.GatlingPeaZombie,
            ZombieType.CatapultZombie,
            ZombieType.GatlingFootballZombie,
            ZombieType.CherryCatapultZombie,
            ZombieType.DrownZombie,
            ZombieType.SnowNormalZombie,
            ZombieType.SnowConeZombie,
            ZombieType.SnowBucketZombie,
            ZombieType.SnowShieldZombie,
            ZombieType.MiniSnowMonster,
            ZombieType.SnowMonsterZombie,
            ZombieType.SuperSnowMonsterZombie,
            ZombieType.SnowDrownZombie,
            ZombieType.SnowGatlingPeaZombie,
            ZombieType.LevatationZombie,
            ZombieType.BalloonZombie,
            ZombieType.IronBalloonZombie
        ];
        public static IReadOnlyList<ZombieType> UltimateRandomList = 
        [
            ZombieType.Jackson_a,
            ZombieType.Jackson_b,
            ZombieType.Jackson_c,
            ZombieType.BlackFootball_a,
            ZombieType.BlackFootball_b,
            ZombieType.BlackFootball_c,
            ZombieType.BlackFootball_c2,
            ZombieType.BlackFlagFootball,
            ZombieType.Jackbox_a,
            ZombieType.Jackbox_b,
            ZombieType.GatlingPaper_b,
            ZombieType.GatlingPaper_b,
            ZombieType.GatlingPaper_c,
            ZombieType.ArmedGargantuar,
            ZombieType.SuperGargantuar,
            ZombieType.BlackHorse,
            ZombieType.BlackTrainZombie,
            ZombieType.ElephantZombie_a,
            ZombieType.ElephantZombie_b,
            ZombieType.ElephantZombie_c,
            ZombieType.BlackJackboxZombie,
            ZombieType.LegionZombie,
            ZombieType.ObsidianTallNutZombie,
            ZombieType.GatlingBlackFootball,
            ZombieType.Drown_a,
            ZombieType.Drown_b,
            ZombieType.Drown_c,
            ZombieType.Driver_a,
            ZombieType.Driver_b,
            ZombieType.Driver_c,
            ZombieType.Drownpult_a,
            ZombieType.Drownpult_b,
            ZombieType.Drownpult_c,
            ZombieType.UltimateGoldGargantuar,
            ZombieType.Kirov_a,
            ZombieType.Kirov_b,
            ZombieType.Kirov_c,
            ZombieType.MachineLevatation,
            ZombieType.SuperLevatation
        ];
        public static IReadOnlyList<ZombieType> MixedRandomList = 
        [
            ZombieType.NormalZombie,
            ZombieType.ConeZombie,
            ZombieType.PaperZombie,
            ZombieType.BucketZombie,
            ZombieType.DoorZombie,
            ZombieType.PolevaulterZombie,
            ZombieType.BucketPaper,
            ZombieType.DancePolZombie,
            ZombieType.CoachPaper,
            ZombieType.JacksonZombie,
            ZombieType.DriverZombie,
            ZombieType.FootballZombie,
            ZombieType.Gargantuar,
            ZombieType.FlagFootball,
            ZombieType.DollDiamond,
            ZombieType.PogoZombie,
            ZombieType.ElitePaperZombie,
            ZombieType.MachineNutZombie,
            ZombieType.TallNutFootballZombie,
            ZombieType.RedGargantuar,
            ZombieType.SuperPogoZombie,
            ZombieType.SuperJackboxZombie,
            ZombieType.LadderZombie,
            ZombieType.SuperLadderZombie,
            ZombieType.IronGargantuar,
            ZombieType.NewYearZombie,
            ZombieType.HorseZombie,
            ZombieType.PenguinZombie,
            ZombieType.SuperPenguinZombie,
            ZombieType.ElephantZombie,
            ZombieType.SuperPolevaulter,
            ZombieType.PeaShooterZombie,
            ZombieType.CherryPaperZombie,
            ZombieType.CherryShooterZombie,
            ZombieType.GatlingPeaZombie,
            ZombieType.CatapultZombie,
            ZombieType.GatlingFootballZombie,
            ZombieType.CherryCatapultZombie,
            ZombieType.DrownZombie,
            ZombieType.SnowNormalZombie,
            ZombieType.SnowConeZombie,
            ZombieType.SnowBucketZombie,
            ZombieType.SnowShieldZombie,
            ZombieType.MiniSnowMonster,
            ZombieType.SnowMonsterZombie,
            ZombieType.SuperSnowMonsterZombie,
            ZombieType.SnowDrownZombie,
            ZombieType.SnowGatlingPeaZombie,
            ZombieType.LevatationZombie,
            ZombieType.BalloonZombie,
            ZombieType.IronBalloonZombie,
            ZombieType.Jackson_a,
            ZombieType.Jackson_b,
            ZombieType.Jackson_c,
            ZombieType.BlackFootball_a,
            ZombieType.BlackFootball_b,
            ZombieType.BlackFootball_c,
            ZombieType.BlackFootball_c2,
            ZombieType.BlackFlagFootball,
            ZombieType.Jackbox_a,
            ZombieType.Jackbox_b,
            ZombieType.GatlingPaper_b,
            ZombieType.GatlingPaper_b,
            ZombieType.GatlingPaper_c,
            ZombieType.ArmedGargantuar,
            ZombieType.SuperGargantuar,
            ZombieType.BlackHorse,
            ZombieType.BlackTrainZombie,
            ZombieType.ElephantZombie_a,
            ZombieType.ElephantZombie_b,
            ZombieType.ElephantZombie_c,
            ZombieType.BlackJackboxZombie,
            ZombieType.LegionZombie,
            ZombieType.ObsidianTallNutZombie,
            ZombieType.GatlingBlackFootball,
            ZombieType.Drown_a,
            ZombieType.Drown_b,
            ZombieType.Drown_c,
            ZombieType.Driver_a,
            ZombieType.Driver_b,
            ZombieType.Driver_c,
            ZombieType.Drownpult_a,
            ZombieType.Drownpult_b,
            ZombieType.Drownpult_c,
            ZombieType.UltimateGoldGargantuar,
            ZombieType.Kirov_a,
            ZombieType.Kirov_b,
            ZombieType.Kirov_c,
            ZombieType.MachineLevatation,
            ZombieType.SuperLevatation
        ];
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
	    [MethodImpl(MethodImplOptions.NoInlining)]
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
