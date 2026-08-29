using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using CustomPlantClass;
using Unity.VisualScripting;

namespace UltiFireThree{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "UltiFireThree.Bepinex";
        public const string PluginName = "UltiFireThree";
        public const string PluginVersion = "3.5";
        public static int BuffID=-1;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            ClassInjector.RegisterTypeInIl2Cpp<UltiFireThree>();
            ClassInjector.RegisterTypeInIl2Cpp<UltiFireThreeBlover>();
            ID peaID=PlantType.Peashooter;
            ID superThreeID=PlantType.SuperThreePeater;
            UltiFireThree.PLANT_ID=DataMgr.AllocateID();

            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ultifirethree");
            CustomCore.RegisterCustomPlant<AllPeater, UltiFireThree>(
                UltiFireThree.PLANT_ID,
                assetBundle.GetAsset<GameObject>("UltiFireThreePrefab"),
                assetBundle.GetAsset<GameObject>("UltiFireThreePreview"),
                new List<(int, int)> { (superThreeID, peaID), (peaID, superThreeID) },
                1.5f,
                0f,
                80,
                300,
                0f,
                1000
            );

            CustomCore.AddUltimatePlant (UltiFireThree.PLANT_ID);
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(UltiFireThree.PLANT_ID);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(UltiFireThree.PLANT_ID);
            CustomCore.TypeMgrExtra.LevelPlants.Add (UltiFireThree.PLANT_ID, (CardLevel)4);
            CustomCore.AddPlantAlmanacStrings(
                UltiFireThree.PLANT_ID,
                "Ultimate Fire Allpeater",
                "<color=#FF0000>!!!这个模组目前处于实验阶段，在某些情况下可能会导致游戏崩溃。请谨慎使用。!!!\n!!!This mod is experimental and may crash the game under certain conditions. If you choose to use it, please do so with caution!!!!\n</color> 融合配方：超级三线豌豆 + 豌豆射手\n全身被烈焰包裹的三线豌豆，能够发射超级火豌豆，三线火力进一步强化。\n\nFusion recipe: Super Threepeater + Peashooter\nA blazing version of the Threepeater that fires super fire peas with enhanced triple‑lane coverage."
            );
            CustomCore.RegisterCustomBanMix(
                UltiFireThree.PLANT_ID,
                () =>
                    (Lawnf.TravelAdvanced(AdvBuff.EnumValue9) && Lawnf.TravelAdvanced(AdvBuff.EnumValue8)) ||
                    Board.Instance.boardTag.enableAllTravelPlant ||
                    Board.Instance.boardTag.isSuperRandom ||
                    Board.Instance.boardTag.isUltimateSuperRandom ||
                    GameAPP.developerMode,
                null,
                () => InGameText.Instance.ShowText("该配方需要抽取", 3f)
            );

            BuffID=CustomCore.RegisterCustomBuff(
                "冰火两冲天: The ice and fire effects can now coexist, ultimate winter melon will fire to all zombies in its lane at the same time, and ultimate fire allpeater will cause the portaled effect.",
                BuffType.AdvancedBuff, ()=>TravelStore.Instance != null && Board.Instance!=null && Lawnf.TravelUltimate(UltiBuff.EnumValue40) && Lawnf.TravelUltimate(UltiBuff.EnumValue41) && Lawnf.TravelAdvanced(AdvBuff.EnumValue9) && Lawnf.TravelAdvanced(AdvBuff.EnumValue8),
                25000,PlantType.EndoFlame,1,default
            );
            CustomCore.RegisterCustomPlantClickEvent(UltiFireThree.PLANT_ID,(Plant p) =>
            {
                UltiFireThree plant=p.GetComponent<UltiFireThree>();
                plant.plant.ChangeType();
            });
            CustomCore.RegisterSuperSkill(UltiFireThree.PLANT_ID,p => 1000,(Plant p) =>
            {
                UltiFireThree plant=p.GetComponent<UltiFireThree>();
                plant.StartPF();
            },1000);
            DataMgr.StartUpMessages.Add("注意！Ulti Fire Three 是实验性模组，可能会导致游戏崩溃，请谨慎使用！\nHeads up! Ulti Fire Three is experimental and may crash the game. Please use it with caution!");

            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    public class UltiFireThreeBlover : MonoBehaviour
    {
        public static ID PLANT_ID = 100962;
        public FlyingThreePeater plant => gameObject.GetComponent<FlyingThreePeater>();
    }
    public class UltiFireThree : MonoBehaviour
    {
        public bool isPF = false;
        public static ID PLANT_ID = 100919;
        public AllPeater plant => gameObject.GetComponent<AllPeater>();
        public void Start()
        {
            plant.shoot=gameObject.transform.FindChild("ThreePeater_head_mid/Shoot");
        }
        public void FixedUpdate()
        {
            if(isPF) plant.thePlantHealth = plant.thePlantMaxHealth;
        }
        public static bool isBuffActive() => Core.BuffID!=-1 && Lawnf.TravelAdvanced((AdvBuff)Core.BuffID) && Lawnf.GetPlantCount(PLANT_ID,Board.Instance)>=1 && Lawnf.GetPlantCount(PlantType.UltimateWinterMelon,Board.Instance)>=1;
        public virtual void StartPF()
        {
            plant.StartCoroutine(SuperShoot());
        }

        // Overridable supershoot logic
        public virtual IEnumerator SuperShoot()
        {
            plant.invincible = true;
            plant.uncrashable = true;
            isPF = true;
            plant.flashCountDown = 5f;
            plant.isFlashing = true;

            // Each phase is 15 peas
            int phaseSize = 15;

            for (int i = 0; i < 60; i++)
            {
                Vector2 pos;
                try
                {
                    pos = plant.shoot.position;
                }
                catch
                {
                    pos = plant.transform.position;
                    pos=new Vector2(pos.x,pos.y+1.5f);
                }

                // Determine which phase we are in
                int phase = i / phaseSize;
                int idx = i % phaseSize;

                float mainAngle;

                switch (phase)
                {
                    case 0: // 0 → 90
                        mainAngle = 90f / (phaseSize - 1) * idx;
                        break;

                    case 1: // 90 → 0
                        mainAngle = 90f - 90f / (phaseSize - 1) * idx;
                        break;

                    case 2: // repeat 0 → 90 (AllPeater only)
                        mainAngle = 90f / (phaseSize - 1) * idx;
                        break;

                    case 3: // repeat 90 → 0 (AllPeater only)
                        mainAngle = 90f - 90f / (phaseSize - 1) * idx;
                        break;

                    default:
                        mainAngle = 0f;
                        break;
                }

                // Three angles: up, straight, down
                float[] angles = { mainAngle, 0f, -mainAngle };

                foreach (float angle in angles)
                {
                    float y=pos.y;
                    BulletMoveWay move =
                        angle == 0f ? BulletMoveWay.MoveRight : BulletMoveWay.Free;

                    Bullet b = CreateBullet.Instance.SetBullet(
                        pos.x, y,
                        plant.thePlantRow,
                        BulletType.Bullet_firePea_super,
                        move,
                        false
                    );

                    if (b == null)
                        continue;

                    b.Damage = plant.attackDamage;
                    b.fromType = plant.thePlantType;
                    b.normalSpeed = 5f;

                    // Free bullets use rotation as movement direction
                    b.transform.Rotate(0f, 0f, angle);
                }

                // Your triple WaitForFixedUpdate preserved
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }

            plant.invincible = false;
            plant.uncrashable = false;
            isPF = false;
            plant.flashCountDown = 5f;
            plant.isFlashing = false;
        }
    }
    [HarmonyPatch(typeof(Shooter), nameof(Shooter.GetBulletType))]
    public class Shooter_GetBulletType_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Shooter __instance, ref BulletType __result)
        {
            if (__instance == null || __instance.IsDestroyed() || Board.Instance==null) return;

            // Now it's safe to read thePlantType
            if (__instance.thePlantType == UltiFireThree.PLANT_ID)
            {
                __result = BulletType.Bullet_firePea_super;
            }
        }
    }
    [HarmonyPatch(typeof(Bullet_firePea_super), nameof(Bullet_firePea_super.HitZombie))]
    public class Bullet_firePea_super_HitZombie_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Bullet_firePea_super __instance, Zombie zombie)
        {
            if (UltiFireThree.isBuffActive() && zombie!=null && zombie.theStatus!=ZombieStatus.Dying && __instance.fromType==UltiFireThree.PLANT_ID)
            {
                zombie.SetPortaled(5f);
            }
        }
    }
    [HarmonyPatch(typeof(Zombie))]
    public static class Zombie_Patch
    {
        // Store fire state before cold/freeze overwrites it
        private static Dictionary<int, (bool isJalaed, bool isEmbered)> savedCold = new();
        private static Dictionary<int, (bool isJalaed, bool isEmbered)> savedFreeze = new();

        // -----------------------------
        // 1. Prevent Warm() from clearing freeze
        // -----------------------------
        [HarmonyPatch("Warm")]
        [HarmonyPrefix]
        public static bool Zombie_Warm_Prefix(Zombie __instance)
        {
            if (!UltiFireThree.isBuffActive()) return true;

            // If zombie is frozen or cold, block warm
            if (__instance.freezeTimer > 0f || __instance.coldTimer > 0f)
                return false;

            return true;
        }

        // -----------------------------
        // 2. Prevent Unfreezing() from clearing freeze
        // -----------------------------
        [HarmonyPatch("Unfreezing")]
        [HarmonyPrefix]
        public static bool Zombie_Unfreezing_Prefix(Zombie __instance)
        {
            if (!UltiFireThree.isBuffActive()) return true;

            if (__instance.freezeTimer > 0f)
                return false;

            return true;
        }

        // -----------------------------
        // 3. Preserve fire when SetCold() is applied
        // -----------------------------
        [HarmonyPatch("SetCold")]
        [HarmonyPrefix]
        public static void Zombie_SetCold_Prefix(Zombie __instance)
        {
            if (!UltiFireThree.isBuffActive()) return;

            int id = __instance.GetInstanceID();
            savedCold[id] = (__instance.isJalaed, __instance.isEmbered);
        }

        [HarmonyPatch("SetCold")]
        [HarmonyPostfix]
        public static void Zombie_SetCold_Postfix(Zombie __instance)
        {
            if (!UltiFireThree.isBuffActive()) return;

            int id = __instance.GetInstanceID();
            if (savedCold.TryGetValue(id, out var tuple))
            {
                __instance.isJalaed = tuple.isJalaed;
                __instance.isEmbered = tuple.isEmbered;
                savedCold.Remove(id);
            }
        }

        // -----------------------------
        // 4. Preserve fire when SetFreeze() is applied
        // -----------------------------
        [HarmonyPatch("SetFreeze")]
        [HarmonyPrefix]
        public static void Zombie_SetFreeze_Prefix(Zombie __instance)
        {
            if (!UltiFireThree.isBuffActive()) return;

            int id = __instance.GetInstanceID();
            savedFreeze[id] = (__instance.isJalaed, __instance.isEmbered);
        }

        [HarmonyPatch("SetFreeze")]
        [HarmonyPostfix]
        public static void Zombie_SetFreeze_Postfix(Zombie __instance)
        {
            if (!UltiFireThree.isBuffActive()) return;

            int id = __instance.GetInstanceID();
            if (savedFreeze.TryGetValue(id, out var tuple))
            {
                __instance.isJalaed = tuple.isJalaed;
                __instance.isEmbered = tuple.isEmbered;
                savedFreeze.Remove(id);
            }
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

                    // Same helper as original: ballistic solution with speed
                    var param = Lawnf.CalculateProjectileWithSpeed(
                        shootPos,
                        targetVel,
                        targetPos,
                        1.0f
                    );

                    if (param == null || param.Length < 4)
                        continue;

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

                    if (b == null)
                        continue;

                    // Apply ballistic parameters
                    b.Vx     = param[1];
                    b.Vy     = param[2];
                    b.detaVy = -param[3];

                    // Copy original metadata
                    b.Damage      = __instance.attackDamage;
                    b.from        = __instance;
                    b.melonSputter = __instance.melonSputter;
                    b.fromType    = __instance.thePlantType;

                    // Preserve the TravelUltimate(0x29) behavior
                    if (Lawnf.TravelUltimate((UltiBuff)0x29))
                        b.theStatus = (BulletStatus)9;

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
