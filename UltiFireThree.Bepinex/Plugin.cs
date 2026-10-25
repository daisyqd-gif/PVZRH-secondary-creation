using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using CustomPlantClass;
using CustomPlantClass.Main;
using System;
using Il2CppSystem.Linq;

namespace UltiFireThree{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "UltiFireThree.Bepinex";
        public const string PluginName = "UltiFireThree";
        public const string PluginVersion = "4.0";
        public static AdvBuff BuffID;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            BuffID= (AdvBuff)Compatibility.CustomCore_Old.RegisterCustomBuff(
                "冰火两冲天: The ice and fire effects can now coexist, ultimate winter melon will fire to all zombies in its lane at the same time, and ultimate fire allpeater will cause the portaled effect.",
                BuffType.AdvancedBuff, ()=>TravelStore.Instance != null && Board.Instance!=null && Lawnf.TravelUltimate(UltiBuff.EnumValue40) && Lawnf.TravelUltimate(UltiBuff.EnumValue41) && Lawnf.TravelAdvanced(AdvBuff.EnumValue9) && Lawnf.TravelAdvanced(AdvBuff.EnumValue8),
                25000,PlantType.EndoFlame,1,default
            );
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    public class UltiFireThree
    {
        public static bool isBuffActive() => Core.BuffID.IsActive();
    }
    [HarmonyPatch(typeof(Bullet_firePea_super), nameof(Bullet_firePea_super.HitZombie))]
    public class Bullet_firePea_super_HitPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Bullet_firePea_super __instance, Zombie zombie)
        {
            if (UltiFireThree.isBuffActive() && zombie!=null && zombie.theStatus!=ZombieStatus.Dying && __instance.fromType==PlantType.SuperThreePeater_sp)
            {
                zombie.SetPortaled(5f);
            }
        }
    }
    [HarmonyPatch(typeof(Zombie))]
    public static class Zombie_Patch
    {
        // Thanks to Mengxi. https://github.com/MKHkro1
		private static Dictionary<int, ValueTuple<bool, bool>> savedWarmStates_SetCold = new Dictionary<int, ValueTuple<bool, bool>>();
		private static Dictionary<int, ValueTuple<bool, bool>> savedWarmStates_SetFreeze = new Dictionary<int, ValueTuple<bool, bool>>();
        // Store fire state before cold/freeze overwrites it
        private static Dictionary<int, (bool isJalaed, bool isEmbered)> savedCold = new();
        private static Dictionary<int, (bool isJalaed, bool isEmbered)> savedFreeze = new();

        [HarmonyPatch(nameof(Zombie.Warm))]
		[HarmonyPrefix]
		public static bool Warm_Prefix(Zombie __instance)
		{
			if (!UltiFireThree.isBuffActive())
			{
				return true;
			}
			try
			{
				if (ZombieExtensions.GetAttrTimers(__instance).coldTimer > 0f || ZombieExtensions.GetAttrTimers(__instance).freezeTimer > 0f)
				{
					return false;
				}
			}
			catch
			{
			}
			return true;
		}

		[HarmonyPatch(nameof(Zombie.Unfreezing))]
		[HarmonyPrefix]
		public static bool Unfreezing_Prefix(Zombie __instance)
		{
			if (!UltiFireThree.isBuffActive())
			{
				return true;
			}
			try
			{
				if (ZombieExtensions.GetAttrTimers(__instance).freezeTimer > 0f)
				{
					return false;
				}
			}
			catch
			{
			}
			return true;
		}

		[HarmonyPatch(nameof(Zombie.SetCold))]
		[HarmonyPrefix]
		public static void SetCold_Prefix(Zombie __instance)
		{
			if (!UltiFireThree.isBuffActive())
			{
				return;
			}
			try
			{
				if (!(__instance == null))
				{
					int instanceID = __instance.GetInstanceID();
					savedWarmStates_SetCold[instanceID] = new ValueTuple<bool, bool>(ZombieExtensions.GetAttrTimers(__instance).isJalaed, ZombieExtensions.GetAttrTimers(__instance).isEmbered);
				}
			}
			catch
			{
			}
		}

		[HarmonyPatch(nameof(Zombie.SetCold))]
		[HarmonyPostfix]
		public static void SetCold_Postfix(Zombie __instance)
		{
			if (!UltiFireThree.isBuffActive())
			{
				return;
			}
			try
			{
				if (!(__instance == null))
				{
					int instanceID = __instance.GetInstanceID();
					ValueTuple<bool, bool> valueTuple;
					if (savedWarmStates_SetCold.TryGetValue(instanceID, out valueTuple))
					{
						savedWarmStates_SetCold.Remove(instanceID);
					}
				}
			}
			catch
			{
			}
		}

		[HarmonyPatch(nameof(Zombie.SetFreeze))]
		[HarmonyPrefix]
		public static void SetFreeze_Prefix(Zombie __instance)
		{
			if (!UltiFireThree.isBuffActive())
			{
				return;
			}
			try
			{
				if (!(__instance == null))
				{
					int instanceID = __instance.GetInstanceID();
					savedWarmStates_SetFreeze[instanceID] = new ValueTuple<bool, bool>(ZombieExtensions.GetAttrTimers(__instance).isJalaed, ZombieExtensions.GetAttrTimers(__instance).isEmbered);
				}
			}
			catch
			{
			}
		}

		[HarmonyPatch(nameof(Zombie.SetFreeze))]
		[HarmonyPostfix]
		public static void SetFreeze_Postfix(Zombie __instance)
		{
			if (!UltiFireThree.isBuffActive())
			{
				return;
			}
			try
			{
				if (!(__instance == null))
				{
					int instanceID = __instance.GetInstanceID();
					ValueTuple<bool, bool> valueTuple;
					if (savedWarmStates_SetFreeze.TryGetValue(instanceID, out valueTuple))
					{
						savedWarmStates_SetFreeze.Remove(instanceID);
					}
				}
			}
			catch
			{
			}
		}

		[HarmonyPatch(nameof(Zombie.SetJalaed))]
		[HarmonyPrefix]
		public static bool SetJalaed_Prefix(Zombie __instance)
		{
			if (!UltiFireThree.isBuffActive())
			{
				return true;
			}
			bool flag;
			try
			{
				flag = !(__instance == null) || true;
			}
			catch
			{
				flag = true;
			}
			return flag;
		}

		[HarmonyPatch(nameof(Zombie.SetEmbered))]
		[HarmonyPrefix]
		public static bool SetEmbered_Prefix(Zombie __instance, bool ulti = false)
		{
			if (!UltiFireThree.isBuffActive())
			{
				return true;
			}
			bool flag;
			try
			{
				flag = !(__instance == null) || true;
			}
			catch
			{
				flag = true;
			}
			return flag;
		}
    }

    [HarmonyPatch(typeof(UltimateWinterMelon), nameof(UltimateWinterMelon.Shoot1))]
    public static class UltimateWinterMelon_Shoot1_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(UltimateWinterMelon __instance, ref Bullet __result)
        {
            // Only modify behavior when the synergy buff is active
            if (!UltiFireThree.isBuffActive())
                return true;

            if (__instance == null || __instance.board == null)
                return true;

            try
            {
                Transform shootTf = __instance.shoot;
                if (shootTf == null)
                    shootTf = __instance.transform;

                Vector2 shootPos = shootTf.position;
                int row = __instance.thePlantRow;

                Bullet first = null;

                // Loop all zombies and fire at every valid one in this row
                foreach (var z in Lawnf.GetAllZombies())
                {
                    if (z == null) continue;
                    if (z.theZombieRow != row) continue;
                    if (z.beforeDying || z.theStatus == ZombieStatus.Dying) continue;

                    // Get target data
                    Vector2 targetPos = z.ColliderPosition;
                    Vector2 targetVel = z.Velocity;

                    // Create bullet with same type & move way as original
                    var inst = CreateBullet.Instance;
                    if (inst == null)
                        continue;

                    Bullet b = inst.SetBullet(
                        shootPos.x,
                        shootPos.y,
                        row,
                        BulletType.Bullet_winterMelon_ultimate,   // original uses 0xb0
                        BulletMoveWay.Throw,          // same as original Shoot1
                        false
                    );

                    var sel = (Plant p) => p!= null && p.thePlantColumn >= __instance.thePlantColumn && p.thePlantRow == __instance.thePlantRow;
                    if(PlantMgr.IsNotNull(Lawnf.GetAllPlants(sel).TryCast<Il2CppSystem.Collections.Generic.IEnumerable<Plant>>(), out Il2CppSystem.Collections.Generic.IEnumerable<Plant> IE) && PlantMgr.IsNotNull(Enumerable.FirstOrDefault(IE),out Plant umbrella))
                    {
                        Bullet b2 = inst.SetBullet(
                            shootPos.x,
                            shootPos.y,
                            row,
                            BulletType.Bullet_winterMelon_ultimate,
                            BulletMoveWay.Throw,
                            false
                        );
                        b.ThrowTo(umbrella);
                    }

                    if (b == null)
                        continue;

                    // Apply ballistic parameters
                    b.ThrowTo(z);

                    // Copy original metadata
                    b.Damage      = __instance.attackDamage;
                    b.from        = __instance;
                    b.melonSputter = __instance.melonSputter;
                    b.fromType    = __instance.thePlantType;

                    // Preserve the TravelUltimate(0x29) behavior
                    if (Lawnf.TravelUltimate((UltiBuff)0x29))
                        b.theStatus = BulletStatus.Water_cold;

                    if (first == null)
                        first = b;
                }

                __result = first;
                // We handled shooting ourselves; skip original Shoot1
                return false;
            }
            catch
            {
                // On error, fall back to original behavior
                return true;
            }
        }
    }
}