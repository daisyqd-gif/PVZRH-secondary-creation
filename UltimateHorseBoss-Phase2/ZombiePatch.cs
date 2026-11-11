using System.Runtime.CompilerServices;
using GameLevel.RogueShooting;
using HarmonyLib;
using UnityEngine;

namespace HorseBossPhase2
{

    [HarmonyPatch(typeof(Zombie))]
    public static class Zombie_Patch
    {
        [HarmonyPatch(nameof(Zombie.SetMindControl))]
        [HarmonyPrefix]
        public static bool SetMindControl_Prefix(Zombie __instance)
        {
            if (__instance != null && __instance.TryGetComponent<UltimateHorsePhase2>(out _)) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.SetFreeze))]
        [HarmonyPrefix]
        public static bool SetFreeze_Prefix(Zombie __instance)
        {
            if (__instance != null && __instance.TryGetComponent<UltimateHorsePhase2>(out _)) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.SetCold))]
        [HarmonyPrefix]
        public static bool SetCold_Prefix(Zombie __instance)
        {
            if (__instance != null && __instance.TryGetComponent<UltimateHorsePhase2>(out _)) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.SetPoison))]
        [HarmonyPrefix]
        public static bool SetPoison_Prefix(Zombie __instance)
        {
            if (__instance != null && __instance.TryGetComponent<UltimateHorsePhase2>(out _)) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.SetJalaed))]
        [HarmonyPrefix]
        public static bool SetJalaed_Prefix(Zombie __instance)
        {
            if (__instance != null && __instance.TryGetComponent<UltimateHorsePhase2>(out _)) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.Garliced))]
        [HarmonyPrefix]
        public static bool Garliced_Prefix(Zombie __instance)
        {
            if (__instance != null && __instance.TryGetComponent<UltimateHorsePhase2>(out _)) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.BodyTakeDamage))]
        [HarmonyPrefix]
        public static bool BodyTakeDamage_Prefix(Zombie __instance, ref int theDamage)
        {
            if (__instance.theZombieType != Plugin.DataContainer.ZombieID)
                return true;
                
            __instance.theHealth -= theDamage;

            return false;
        }
    }

    [HarmonyPatch(typeof(HorseBoss))]
    public static class HorseBoss_Patch
    {
        [HarmonyPatch(nameof(HorseBoss.AnimRevive))]
        [HarmonyPrefix]
        public static bool AnimRevive_Prefix(HorseBoss __instance)
        {
            if (__instance != null)
            {
                Transform child = __instance.axis.GetChild(0);
                Vector3 position = child.position;
                ParticleManager.Instance.SetParticle(ParticleType.BombCloud_black, position, __instance.theZombieRow);
                ScreenShake.TriggerShake(0.15f);
                GameAPP.PlaySound(SoundType.CherryBomb);
                Transform child2 = __instance.axis.GetChild(0);
                Vector3 position2 = child2.position;
                float num2 = position2.x;
                float boxXFromColumn = Lawnf.GetBoxXFromColumn(2);
                if (boxXFromColumn > position2.x)
                {
                    float boxXFromColumn2 = Lawnf.GetBoxXFromColumn(2);
                    num2 = boxXFromColumn2;
                }
                if (!__instance.isMindControlled)
                {
                    Zombie zombie = CreateZombie.Instance.SetZombie(__instance.theZombieRow, Plugin.DataContainer.ZombieID, num2);
                    if (zombie != null)
                    {
                        float health = __instance.theMaxHealth/90000;
                        zombie.theMaxHealth = (long)(zombie.theMaxHealth * health);
                        zombie.theHealth = zombie.theMaxHealth;
                    }
                    if (ShootingManager.Instance != null)
                    {
                        ShootingManager.Instance.AsBoss(zombie);
                        zombie.onDeath = __instance.onDeath;
                        __instance.onDeath = null;
                    }
                }
                __instance.theStatus = (ZombieStatus)10086;
                __instance.Die(2);
            }
            return false;
        }
    }
    [HarmonyPatch]
    public static class HorseBoss_Die_Patch
    {
        [HarmonyPatch(typeof(Zombie),nameof(Zombie.Die))]
        [HarmonyReversePatch]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Die_ReversePatch(HorseBoss __instance, int reason = 0) { }
        [HarmonyPatch(typeof(HorseBoss),nameof(HorseBoss.Die))]
        [HarmonyPrefix]
        public static bool Die_Prefix(HorseBoss __instance, int reason)
        {
            if(__instance.theStatus != (ZombieStatus)10086) return true;
            Die_ReversePatch(__instance, reason);
            return false;
        }
    }
    [HarmonyPatch(typeof(TypeMgr))]
    public static class TypeMgr_Patch
    {
        [HarmonyPatch(nameof(TypeMgr.IsBossZombie))]
        [HarmonyPrefix]
        public static bool IsBossZombie_Prefix(ref ZombieType theZombieType, ref bool __result)
        {
            if (theZombieType==Plugin.DataContainer.ZombieID)
            {
                __result = true;
                return false;
            }

            return true;
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
            if
            (
                id == Plugin.DataContainer.ZombieID
            )
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Mower))]
    [HarmonyPriority(Priority.First)]
    public static class Mower_Patch
    {
        [HarmonyPatch(nameof(Mower.AttackZombie))]
        [HarmonyPrefix]
        static bool AttackZombie_Prefix(Zombie zombie)
        {
            if (zombie == null)
                return true;
            var id = zombie.theZombieType;
            if
            (
                id == Plugin.DataContainer.ZombieID
            )
            {
                return false;
            }
            return true;
        }
    }
}