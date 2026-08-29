global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using HarmonyLib;
global using System.Reflection;
using CustomizeLib.BepInEx;
using UnityEngine;

namespace SuperHammer
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        internal static BuffID id=-1;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            id=CustomCore.RegisterCustomBuff("锤子大大的威武",BuffType.AdvancedBuff,() => true,5000,PlantType.EndoFlame);
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    // Example: global flag you toggle from your buff system
    public static class SuperHammerBuff
    {
        public static bool IsActive { get => Lawnf.TravelAdvanced(Plugin.id); } // set this true when buff is on
    }

    [HarmonyPatch(typeof(Hammer), nameof(Hammer.AnimCrush))]
    public static class HammerAnimCrushPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Hammer __instance)
        {
            // === 1. Board check ===
            var board = Board.Instance;
            if (board == null)
                return false; // skip original

            // === 2. Whack-a-Zombie mode uses vanilla logic ===
            if (board.boardTag.isHammerZombie || !SuperHammerBuff.IsActive)
                return true;

            // === 3. Cooldown check ===
            if (__instance.CD != __instance.fullCD)
            {
                __instance.PutDown();
                return false;
            }

            // === 4. Get hammer world position ===
            var t = __instance.transform;
            if (t == null)
                return false;

            Vector2 point = t.position;

            // === 5. Put hammer down (same as IL2CPP) ===
            __instance.PutDown();

            // === 6. OverlapCircleAll on "Zombie" layer ===
            int mask = LayerMask.GetMask("Zombie");
            var hits = Physics2D.OverlapCircleAll(point, 0.5f, mask);

            bool hitAny = false;

            foreach (var col in hits)
            {
                if (col == null) continue;
                if (!col.TryGetComponent<Zombie>(out var z)) continue;
                if(z.theZombieType ==ZombieType.ZombieBoss || z.theZombieType ==ZombieType.ZombieBoss2 || z.theZombieType ==ZombieType.HorseBoss ||
                z.theZombieType ==ZombieType.UltimateSnowZombie && z.TryGetComponent<UltimateSnowZombie>(out var s) && s.boss) continue;

                // IL2CPP check: skip if zombie klass index == 1
                // (you can add your own filter here if needed)

                // === 7. Real hammer damage ===
                // IL2CPP calls vtable.TakeDamage(z, 1000000, hammer, 0xe, -1, 0, ...)
                var maker = __instance.Cast<IDamageMaker>();
                z.theHealth=0;
                z.theFirstArmorHealth=0;
                z.theSecondArmorHealth=0;
                z.TakeDamage(
                    1000000,        // huge damage
                    maker,            // hammer as IDamageMaker
                    DamageType.Hammer // 0xe
                );
                z.Die(0);

                hitAny = true;
            }

            // === 8. If hit any zombie, reset cooldown + play sound ===
            if (hitAny)
            {
                __instance.CD = 0f;
                GameAPP.PlaySound(SoundType.Bonk, 0.5f, 1f);
            }

            // === 9. Skip original AnimCrush ===
            return false;
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "SuperHammer.Bepinex";
        public const string PluginName = "SuperHammer";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
