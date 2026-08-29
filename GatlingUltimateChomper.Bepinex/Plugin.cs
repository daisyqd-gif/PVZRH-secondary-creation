using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using CustomizeLib.BepInEx;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace GatlingUltimateChomper
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "GatlingUltimateChomper.Bepinex";
        public const string PluginName = "GatlingUltimateChomper";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<GatlingUltimateChomper>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "gatlingchomper");
            CustomCore.RegisterCustomPlant<UltimateChomper, GatlingUltimateChomper>(
                GatlingUltimateChomper.PLANT_ID,
                assetBundle.GetAsset<GameObject>("UltimateChomperPrefab"),
                assetBundle.GetAsset<GameObject>("UltimateChomperPreview"),
                new List<(int, int)> { },
                1.5f,
                0f,
                1000,
                12000,
                0f,
                1500
            );

            CustomCore.AddUltimatePlant(GatlingUltimateChomper.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(GatlingUltimateChomper.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add(GatlingUltimateChomper.PLANT_ID, CardLevel.Red);
            CustomCore.TypeMgrExtra.IsNut.Add(GatlingUltimateChomper.PLANT_ID);
            CustomCore.TypeMgrExtra.IsTallNut.Add(GatlingUltimateChomper.PLANT_ID);
            CustomCore.TypeMgrExtra.UncrashablePlants.Add(GatlingUltimateChomper.PLANT_ID);
            CustomCore.AddPlantAlmanacStrings(
                GatlingUltimateChomper.PLANT_ID,
                $"Ultimate Gatling Chomper ({GatlingUltimateChomper.PLANT_ID})",
                "Eats zombies and spits out *a lot* of cherries. Immune to digging and breaks the pickaxe. Immune to the permanent trauma effect."
            );
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    public class GatlingUltimateChomper : MonoBehaviour
    {
        public static ID PLANT_ID = 3070;
        private int baseMaxHealth=12000;
        private int baseDamage=1500;
        public UltimateChomper plant => gameObject.GetComponent<UltimateChomper>();
        public void Start()
        {
            plant.shoot = plant.gameObject.transform.FindChild("Chomper_inside/Shoot");
        }
        public void Update()
        {
            if (plant == null) return;

            // Zombie search
            if (plant.targetZombie == null && GameAPP.theGameStatus == GameStatus.InGame)
                plant.ChomperSearchZombie();

            // Speed buff
            plant.thePlantSpeed = Lawnf.TravelUltimate(UltiBuff.EnumValue3) ? 3f : 1.5f;

            // Clamp current HP if needed
            if (plant.thePlantHealth > baseMaxHealth)
                plant.thePlantHealth = baseMaxHealth;

            // --- HARD LOCK MAX HEALTH ---
            if (plant.thePlantMaxHealth != baseMaxHealth)
                plant.thePlantMaxHealth = Math.Max(baseMaxHealth,12000);

            // Clamp attack damage if needed
            if (plant.attackDamage > baseDamage)
                plant.attackDamage = baseDamage;

            // --- HARD LOCK DAMAGE ---
            if (plant.attackDamage != baseDamage)
                plant.attackDamage = Math.Max(baseDamage,1000);

            plant.Recover(10f,default,false); //heal 1 health every frame
        }
        public void AnimShoot() //extends original game method but does not modify it
        {
            if (!Lawnf.TravelAdvanced (AdvBuff.EnumValue3002)||plant.starUp) {//starup already has enough bullets
				return;
			}
            Vector2 pos = plant.shoot.position;
            pos.x += UnityEngine.Random.Range(-0.2f, 0.2f);
            pos.y += UnityEngine.Random.Range(-0.2f, 0.2f);

            int row = plant.thePlantRow;
            int dmg = plant.attackDamage;

            Bullet b1 = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, row,
                BulletType.Bullet_superCherry,
                BulletMoveWay.Sin,
                false
            );

            Bullet b2 = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, row,
                BulletType.Bullet_superCherry,
                BulletMoveWay.Sin,
                false
            );

            if (b1 != null)
            {
                b1.Damage = dmg;
                b1.from = plant;
                b1.fromType = plant.thePlantType;
            }

            if (b2 != null)
            {
                b2.theExistTime = 0.5f;
                b2.Damage = dmg;
                b2.from = plant;
                b2.fromType = plant.thePlantType;
            }
        }
    }
    [HarmonyPatch(typeof(BombCherry), nameof(BombCherry.PlantTakeDamage))]
    public static class BombCherryTakeDamagePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Plant plant)
        {
            if (plant.thePlantType == GatlingUltimateChomper.PLANT_ID)
            {
                plant.Recover(200f);
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Lawnf), nameof(Lawnf.GetPlantCount), new Type[] { typeof(PlantType), typeof(Board) })]
    public static class LawnfGetCountPatch
    {
        [HarmonyPostfix]
        public static void Postfix(PlantType theSeedType, Board board, ref int __result)
        {
            if (theSeedType == PlantType.UltimateChomper)
                __result += Lawnf.GetPlantCount(GatlingUltimateChomper.PLANT_ID, board);
        }
    }
    /*[HarmonyPatch(typeof(UltimateChomper), nameof(UltimateChomper.Bite))]
    public static class Bite3TimesPatch
    {
        public static bool a;
        [HarmonyPostfix]
        public static void Bite (UltimateChomper __instance, Zombie zombie)
        {
            if (__instance.thePlantType==GatlingUltimateChomper.PLANT_ID && !a) {
                a = true;
                ((SuperChomper)__instance).Bite (zombie);
                ((SuperChomper)__instance).Bite (zombie);
                a = false;
            }
        }
    }*/
    [HarmonyPatch(typeof(Plant), nameof(Plant.Start))]
    [HarmonyPriority(Priority.First)]
    public static class Plant_Start_RareUpgrade
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance)
        {
            // --- SAFETY CHECKS ----------------------------------------------------

            // Null check (should never happen, but safe)
            if (__instance == null)
                return;

            // Only run in actual gameplay (prevents preview bugs)
            if (GameAPP.theGameStatus != GameStatus.InGame || __instance.board.boardTag.isIZ)
                return;

            // 5% chance total (you placed this BEFORE the type checks, which is correct)
            if (UnityEngine.Random.Range(0, 100) > 5)
                return;

            // --- FIRE SNIPER → ULTIMATE SNIPER -----------------------------------

            if (__instance.thePlantType == PlantType.UltimateChomper)
            {
                int col = __instance.thePlantColumn;
                int row = __instance.thePlantRow;

                PlantType firstParent = __instance.firstParent;
                PlantType secondParent = __instance.secondParent;

                // Remove original Fire Sniper
                __instance.Die();

                // Spawn Ultimate Sniper
                Plant newPlant = CreatePlant.Instance
                    .SetPlant(col, row, (PlantType)GatlingUltimateChomper.PLANT_ID, isFreeSet: true)
                    .GetComponent<Plant>();

                // Preserve fusion history (even if unused)
                newPlant.firstParent = firstParent;
                newPlant.secondParent = secondParent;

                return; // Important: prevents falling into the next block
            }
        }
    }
    [HarmonyPatch(typeof(Plant), nameof(Plant.TakeDamage))]
    public static class GatlingChomperTakeDamagePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Plant __instance, ref int damage)
        {
            if (__instance.thePlantType != GatlingUltimateChomper.PLANT_ID)
                return true;
            if(UnityEngine.Random.Range(0,100)<50&&__instance.starUp)
                return false;
            damage = Math.Min(Math.Max(1, damage / 5),250);
            return true;
        }
    }

    [HarmonyPatch(typeof(PickaxeZombie), "ZombieUpdate")]
    public static class GatlingChomperPickaxeZeroSpeed
    {
        [HarmonyPrefix]
        public static bool Prefix(PickaxeZombie __instance)
        {
            var target = __instance.theAttackTarget;
            if (target == null) return true;

            if (!target.IsPlant(out var plant))
                return true;

            if (plant.thePlantType != GatlingUltimateChomper.PLANT_ID)
                return true;

            // Freeze digging permanently
            __instance.digSpeed = 0f;
            __instance.progress = 0f;

            // Optionally: hide progress UI
            if (__instance.progressText != null)
            {
                var go = __instance.progressText.gameObject;
                if (go != null) go.SetActive(false);
            }

            // Skip original digging logic
            return false;
        }
    }
    [HarmonyPatch(typeof(Pickaxe_a), "ZombieUpdate")]
    public static class GatlingChomperPickaxeAZeroSpeed
    {
        [HarmonyPrefix]
        public static bool Prefix(Pickaxe_a __instance)
        {
            var target = __instance.theAttackTarget;
            if (target == null) return true;

            if (!target.IsPlant(out var plant))
                return true;

            if (plant.thePlantType != GatlingUltimateChomper.PLANT_ID)
                return true;

            // Freeze digging
            __instance.digSpeed = 0f;
            __instance.progress = 0f;

            // Hide progress UI
            if (__instance.progressText != null)
            {
                var go = __instance.progressText.gameObject;
                if (go != null) go.SetActive(false);
            }

            // Skip original digging logic
            return false;
        }
    }
    [HarmonyPatch(typeof(Plant), "Crashed")]
    public static class UltimateGatlingChomperCrashedPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Plant __instance)
        {
            if (__instance.thePlantType == GatlingUltimateChomper.PLANT_ID)
            {
                int col = __instance.thePlantColumn;
                int row = __instance.thePlantRow;
                bool star= __instance.starUp;
                Plant newPlant = CreatePlant.Instance.SetPlant(col, row, (PlantType)GatlingUltimateChomper.PLANT_ID, isFreeSet: true).GetComponent<Plant>();
                if(star){
                    newPlant.starUp=true;
                    newPlant.UpdateStarIcon();
                }
            }
            return true;
        }
    }
}
