global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using HarmonyLib;
global using System.Reflection;
global using UnityEngine;
global using System.Collections.Generic;
global using CustomPlantClass;
using Unity.VisualScripting;
using CustomPlantClass.Main;


namespace MowerFix{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "MowerFix.Bepinex";
        public const string PluginName = "MowerFix";
        public const string PluginVersion = MyPluginInfo.TargetVersion;
        public static readonly HashSet<ZombieType> List = new()
        {
            ZombieType.MinerZombie,
            ZombieType.BungiZombie,
            ZombieType.ZombieBoss,
            ZombieType.ZombieBoss2,
            ZombieType.SnowMonsterZombie,
            ZombieType.SuperSnowMonsterZombie,
            ZombieType.MiniSnowMonster,
            ZombieType.GoldBungiZombie,
            ZombieType.MiniSandMonster,
            ZombieType.HypnoJalapenoPickaxeZombie,
            ZombieType.SnowDolphinrider,
            ZombieType.FootballDolphin,
            ZombieType.YellowFootball,
            ZombieType.SnowZombie,
            ZombieType.DolphinPeaZombie,
            ZombieType.DolphinGatlingZombie,
            ZombieType.ImpKing,
            ZombieType.HorseBoss,
            ZombieType.SummonedHorse
        };
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            ModRegistryManager.CreateRegistry<ZombieType>("NeutralZombies");
            foreach(var i in List)
            {
                ModRegistryManager.AddToRegistry("NeutralZombies",i);
            }
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    [HarmonyPatch(typeof(GameLose))]
    [HarmonyPriority(Priority.First)]
    public static class GameLose_Patch
    {
        [HarmonyPatch(nameof(GameLose.OnTriggerEnter2D))]
        [HarmonyPrefix]
        static bool OnTriggerEnter2D_Prefix(Collider2D collision)
        {
            var zombie = collision.GetComponent<Zombie>();
            if (zombie == null)
                return true;
            var id = zombie.theZombieType;
            if(id==ZombieType.ZombieBoss || id==ZombieType.ZombieBoss2 || id==ZombieType.HorseBoss || id==ZombieType.UltimateSnowZombie && zombie.TryGetComponent<UltimateSnowZombie>(out var z) && z.boss)
            {
                return false;
            }

            //Dead zombie fix
            if (zombie.theStatus == ZombieStatus.Dying || zombie.FindZombieHead()==null || zombie.beforeDying || zombie.IsDestroyed() || zombie.isMindControlled)
            {
                return false;
            }


            // Neutral IDs
            if (ModRegistryManager.TryGetRegistry("NeutralZombies", out List<ZombieType> r) && r.Contains(id))
            {
                zombie.Die(1);   // remove safely
                return false;   // skip lose logic entirely
            }

            var mowers=zombie.board.mowerArray;
            foreach(Mower i in mowers)
            {
                if (zombie.theZombieRow == i.theMowerRow)
                {
                    zombie.Die(1);
                    if (!i.started)
                    {
                        i.StartMove();
                    }
                    return false;
                }
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Mower), nameof(Mower.OnTriggerStay2D))]
    [HarmonyPriority(Priority.First)]
    public static class Mower_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(Mower __instance, Collider2D collision)
        {
            var zombie = collision.GetComponent<Zombie>();
            if (zombie == null)
                return true;

            if (zombie.isMindControlled)
            {
                return false;
            }

            var id = zombie.theZombieType;
            if(id==ZombieType.ZombieBoss || id==ZombieType.ZombieBoss2 || id==ZombieType.HorseBoss || id==ZombieType.UltimateSnowZombie && zombie.TryGetComponent<UltimateSnowZombie>(out var z) && z.boss)
            {
                return false;
            }

            // Neutral IDs
            if (ModRegistryManager.TryGetRegistry("NeutralZombies", out List<ZombieType> r) && r.Contains(id))
            {
                zombie.Die(1);   // remove safely
                return false;   // skip lose logic entirely
            }
            //Dead zombie fix
            if (zombie.theStatus == ZombieStatus.Dying || zombie.beforeDying || zombie.theZombieRow!=__instance.theMowerRow)
            {
                return false;
            }
            if (!__instance.started)
            {
                __instance.StartMove();
                __instance.board.boardStatistics.mowerUsedCount++;
            }
            __instance.AttackZombie(zombie);

            return true;
        }
    }
}
