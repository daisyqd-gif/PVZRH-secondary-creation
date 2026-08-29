using System;
using System.Collections.Generic;
using System.Linq;
using CustomizeLib.BepInEx;
using GameLevel.RogueShooting;
using HarmonyLib;
using Modified.Utils;
using TMPro;
using UnityEngine;
using ZenGarden;
using Random = UnityEngine.Random;

namespace Modified.Patches
{
    // Token: 0x0200001D RID: 29
    public class GamePatches
    {
        // Token: 0x0200001E RID: 30
        [HarmonyPatch(typeof(Mouse), "TryToSetPlantByCard")]
        public class MouseTryToSetPlantByCardPatch
        {
            [HarmonyPrefix]
            public static void Prefix(Mouse __instance)
            {
                if (!Plugin.rowPlant)
                    return;

                for (int row = 0; row < Board.Instance.rowNum; row++)
                {
                    if (row == __instance.theMouseRow)
                        continue;

                    CreatePlant.Instance.SetPlant(
                        __instance.theMouseColumn,
                        row,
                        __instance.thePlantTypeOnMouse,
                        null,
                        default,
                        false,
                        true,
                        null
                    );
                }
            }
        }

        // Token: 0x0200001F RID: 31
        [HarmonyPatch(typeof(SuperSnowGatling), "Update")]
        public class SuperSnowGatlingUpdatePatch
        {
            [HarmonyPrefix]
            public static void Prefix(SuperSnowGatling __instance)
            {
                if (Plugin.superShooter)
                    __instance.keepShooting = true;
            }
        }

        // Token: 0x02000020 RID: 32
        [HarmonyPatch(typeof(AlmanacPlantMenu), "Awake")]
        public class AlmanacPlantMenuAwakePatch
        {
            [HarmonyPostfix]
            public static void Postfix(AlmanacPlantMenu __instance)
            {
                Transform content = __instance.transform.Find("Scroll View/Viewport/Content");
                if (content == null)
                    return;

                if (content.transform.Find("LookNewPlant") != null)
                    return;

                Transform button = UnityEngine.Object.Instantiate(content.GetChild(1), content);
                button.name = "LookNewPlant";

                foreach (TextMeshProUGUI tmp in button.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    if (tmp != null)
                        tmp.text = "二创植物";
                }

                UIButton uiButton = button.GetComponent<UIButton>();
                if (uiButton != null)
                {
					var a=() =>
                    {
                        // Show only custom plants (not defined in base enum)
                        __instance.ShowPlants(GameAPP.resourcesManager.allPlants.ToSystemList().Where(plantType => !Enum.IsDefined(typeof(PlantType), plantType)).ToList().ToIl2CppList());
                    };
                    uiButton.clickEvent.AddListener(a);
                }
            }
        }

        // Token: 0x02000023 RID: 35
        [HarmonyPatch(typeof(AlmanacZombieMenu), "Start")]
        public class AlmanacZombieMenuStartPatch
        {
            [HarmonyPostfix]
            public static void Postfix(AlmanacZombieMenu __instance)
            {
                if (__instance.transform.Find("LoolAll_Other") != null)
                    return;

                Transform original = __instance.transform.Find("LookAll_1");
                if (original == null)
                    return;

                Transform button = UnityEngine.Object.Instantiate(original, __instance.transform);
                button.name = "LoolAll_Other";
                button.localPosition = new Vector2(440f, -499f);

                foreach (TextMeshProUGUI tmp in button.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    if (tmp != null)
                        tmp.text = "二创僵尸";
                }

                UIButton uiButton = button.GetComponent<UIButton>();
                if (uiButton != null)
                {
					var a=() =>
                    {
                        // Show only custom zombies (not defined in base enum)
						var b= (ZombieType zombieType) => !Enum.IsDefined(typeof(ZombieType), zombieType);
                        __instance.ShowZombieCards(b);
                    };
                    uiButton.clickEvent.AddListener(a);
                }
            }
        }

        // Token: 0x02000026 RID: 38
        [HarmonyPatch(typeof(GardenPlayer))]
        public class GardenPlayerPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void PostAwake(GardenPlayer __instance)
            {
                if (Plugin.bigGardenSkinType == Plugin.GardenSkinType.None)
                    return;

                if (Plugin.bigGardenSkinId == -1)
                    return;

                GameObject go = null;

                switch (Plugin.bigGardenSkinType)
                {
                    case Plugin.GardenSkinType.Plant:
                        go = CreatePlant.SetPlantInAlmamac(__instance.transform.position, Plugin.bigGardenSkinId);
                        if (go != null)
                            go.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
                        break;

                    case Plugin.GardenSkinType.Zombie:
                        go = CreateZombie.SetZombieInAlmanac(__instance.transform.position, (ZombieType)Plugin.bigGardenSkinId);
                        break;
                }

                if (go == null)
                    return;

                __instance.transform.GetChild(0).gameObject.SetActive(false);
                go.transform.SetParent(__instance.transform);
            }
        }

        // Token: 0x02000027 RID: 39
        [HarmonyPatch(typeof(AlmanacCardUI), "OnPointerDown")]
        public class AlmanacCardUIPatch
        {
            [HarmonyPostfix]
            public static void Postfix(AlmanacCardUI __instance)
            {
                if (__instance.menu.name == "AlmanacPlantMenu(Clone)")
                    Plugin.almanacMgrPlantId = (int)__instance.PlantType;

                if (__instance.menu.name == "AlmanacZombieMenu(Clone)")
                    Plugin.almanacMgrZombieId = (int)__instance.ZombieType;
            }
        }

        // Token: 0x02000028 RID: 40
        [HarmonyPatch(typeof(Glove), "OnUpdate")]
        public class GloveMgrUpdatePatch
        {
            [HarmonyPostfix]
            public static void Postfix(Glove __instance)
            {
                if (Plugin.isGloveNoCD)
                    __instance.CD = __instance.fullCD;
            }
        }

        // Token: 0x02000029 RID: 41
        /*[HarmonyPatch(typeof(GridManager), "OnDrawGizmos")]
        public class GridManagerPatch
        {
            [HarmonyPostfix]
            public static void Postfix(GridManager __instance)
            {
                __instance.maxY = (GameAPP.resourcesManager.allPlants.Count / 9f) * 1.5f;
            }
        }*/

        // Token: 0x0200002A RID: 42
        [HarmonyPatch(typeof(Gargantuar), "AnimCrash")]
        public class GargantuarAnimCrashPatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                return !Plugin.isPlantInvincible && !Plugin.isPlantAntiRolling;
            }
        }

        // Token: 0x0200002B RID: 43
        [HarmonyPatch(typeof(Zombie), "Update")]
        public class ZombieUpdatePatch
        {
            [HarmonyPrefix]
            public static void Prefix(Zombie __instance)
            {
                if (!Plugin.isSameSpeed)
                    return;

                __instance.theSpeed = 1f;
                __instance.theOriginSpeed = 1f;
                __instance.anim.SetFloat("speed", 1f);
            }
        }

        // Token: 0x0200002C RID: 44
        [HarmonyPatch(typeof(DrawCardManager), "PerformSinglePull")]
        public class DrawCardManagerPerformSinglePullPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(DrawCardManager __instance, ref PlantType __result)
            {
                __result = (PlantType)951;
                return false;
            }
        }

        // Token: 0x0200002D RID: 45
        [HarmonyPatch(typeof(Zombie), "Die")]
        public class ZombieDiePatch
        {
            [HarmonyPrefix]
            public static void Prefix(ref int reason)
            {
                if (Plugin.isMomentZombieDie)
                    reason = 2;
            }
        }

        // Token: 0x0200002E RID: 46
        [HarmonyPatch(typeof(Bullet), "Update")]
        public class BulletUpdatePatch
        {
            [HarmonyPostfix]
            public static void Postfix(Bullet __instance)
            {
                if (Plugin.isReversalPlant)
                    __instance.transform.Translate(6f * Time.deltaTime * Vector3.left);

                if (Plugin.bulletAtk != -1)
                    __instance.Damage = Plugin.bulletAtk;

                if (Plugin.isManyHitTimes)
                    __instance.hitCount = 0;
            }
        }

        // Token: 0x0200002F RID: 47
        [HarmonyPatch(typeof(CobCannon), "AnimShoot")]
        public class CobCannonAnimShootPatch
        {
            [HarmonyPostfix]
            public static void Postfix(CobCannon __instance)
            {
                if (Plugin.isCobCannonShoop)
                    __instance.anim.SetTrigger("charge");
            }
        }

        // Token: 0x02000030 RID: 48
        [HarmonyPatch(typeof(Plant), "LimHealth")]
        public class PlantLimHealthPatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                return !Plugin.isPlantLimHp;
            }
        }

        // Token: 0x02000031 RID: 49
        [HarmonyPatch(typeof(Zombie), "OnTriggerEnter2D")]
        public class DriverZombieOnTriggerStay2DPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(Collider2D collision, Zombie __instance)
            {
                if (__instance == null || collision == null)
                    return true;

                Plant plant = collision.GetComponent<Plant>();
                if (plant == null)
                    return true;

                if (plant.thePlantRow == __instance.theZombieRow &&
                    Plugin.isPlantAntiRolling &&
                    (TypeMgr.IsDriverZombie(__instance.theZombieType) || TypeMgr.IsGargantuar(__instance.theZombieType)))
                {
                    plant.TakeDamage(100, __instance.TryCast<IDamageMaker>());
                    __instance.gameObject.transform.Translate(1f, 0f, 0f);
                    GameAPP.PlaySound(Random.Range(72, 75), 0.5f, 1f);
                    return false;
                }

                return true;
            }
        }

        // Token: 0x02000032 RID: 50
        [HarmonyPatch(typeof(Plant), "Crashed")]
        public class PlantCrashedPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(Plant __instance, ref Zombie zombie)
            {
                if (!Plugin.isPlantAntiRolling)
                    return true;

                __instance.TakeDamage(zombie.theAttackDamage, __instance.TryCast<IDamageMaker>());
                zombie.transform.Translate(1f, 0f, 0f);
                return false;
            }
        }

        // Token: 0x02000033 RID: 51
        [HarmonyPatch(typeof(Plant), "TakeDamage")]
        public class PlantTakeDamagePatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref int damage)
            {
                if (Plugin.isZombieSeckill)
                    damage = 99999;

                if (Plugin.isPlantInvincible)
                {
                    damage = 0;
                    return false;
                }

                return true;
            }
        }

        // Token: 0x02000034 RID: 52
        [HarmonyPatch(typeof(Zombie), "TakeDamage")]
        public class ZombieTakeDamagePatch
        {
            [HarmonyPrefix]
            public static void Prefix(ref int theDamage)
            {
                if (Plugin.isPlantSeckill)
                    theDamage = 99999;

                if (Plugin.isZombieInvincible)
                    theDamage = 0;
            }
        }

        // Token: 0x02000036 RID: 54
        [HarmonyPatch(typeof(CardUI))]
        public class CardUIPatch
        {
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            public static void PostStart(CardUI __instance)
            {
                if (__instance.transform.Find("CDText") != null)
                    return;

                GameObject go = UnityEngine.Object.Instantiate(__instance.transform.GetChild(1).gameObject, __instance.transform);
                go.name = "CDText";

                TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
                tmp.font = GameAPP.font;
                tmp.color = Color.yellow;
                tmp.fontStyle = FontStyles.Bold;
                tmp.fontSize = 19f;

                Vector2 pos = go.transform.position;
                pos.y += 0.4f;
                pos.x += 0.05f;
                go.transform.position = pos;

                float cd = __instance.fullCD - __instance.CD;
                tmp.text = $"冷却:{cd:F2}";
                tmp.enabled = cd != 0f;
            }

            [HarmonyPatch("Update")]
            [HarmonyPrefix]
            public static void PreUpdate(CardUI __instance)
            {
                if (!__instance.onCardBank)
                    return;

                if (__instance.GetComponent<SpecialCard>() != null)
                    return;

                Transform child = __instance.transform.childCount > 4 ? __instance.transform.GetChild(4) : null;
                TextMeshProUGUI tmp = child != null ? child.GetComponent<TextMeshProUGUI>() : null;
                if (tmp == null)
                    return;

                float cd = __instance.fullCD - __instance.CD;
                tmp.text = $"冷却:{cd:F2}";
                tmp.enabled = cd != 0f && Plugin.isCDText;
            }

            [HarmonyPatch("CDUpdate")]
            [HarmonyPrefix]
            public static void PreCDUpdate(CardUI __instance)
            {
                if (Plugin.isCardNoCD)
                    __instance.CD = __instance.fullCD;
            }
        }

        // Token: 0x02000037 RID: 55
        [HarmonyPatch(typeof(Hammer), "OnUpdate")]
        public class HammerMgrUpdatePatch
        {
            [HarmonyPrefix]
            public static void Prefix(Hammer __instance)
            {
                if (Plugin.isHammerNoCD)
                    __instance.CD = __instance.fullCD;
            }
        }

        // Token: 0x02000038 RID: 56
        [HarmonyPatch(typeof(InGameUI), "Update")]
        public class InGameUIMgrUpdatePatch
        {
            [HarmonyPostfix]
            public static void Postfix(InGameUI __instance)
            {
                if (Plugin.lockSun)
                    __instance.sun.text = "∞";
            }
        }

        // Token: 0x02000039 RID: 57
        [HarmonyPatch(typeof(GameLose), "OnTriggerEnter2D")]
        public class GameLoseOnTriggerEnter2DPatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                return !Plugin.daveInvincible;
            }
        }

        // Token: 0x0200003A RID: 58
        [HarmonyPatch(typeof(CreateBullet))]
        public class CreateBulletPatch
        {
            [HarmonyPatch("SetBullet", new[]
            {
                typeof(float),
                typeof(float),
                typeof(int),
                typeof(BulletType),
                typeof(BulletMoveWay),
                typeof(bool)
            })]
            [HarmonyPrefix]
            public static void PrefixSetBullet2(ref BulletType theBulletType)
            {
                if (Plugin.isRandomBullet)
                    theBulletType = (BulletType)Random.Range(0, 120);

                if (Plugin.appointBullet != -1)
                    theBulletType = (BulletType)Plugin.appointBullet;
            }
        }

        // Token: 0x0200003B RID: 59
        [HarmonyPatch(typeof(Zombie), "InitHealth")]
        public class ZombieAwakePatch
        {
            [HarmonyPostfix]
            public static void Postfix(Zombie __instance)
            {
                if (__instance == null)
                    return;

                __instance.theMaxHealth *= Plugin.zombieHpMu;
                __instance.theHealth *= Plugin.zombieHpMu;
            }
        }

        // Token: 0x0200003C RID: 60
        [HarmonyPatch(typeof(CreatePlant), "LimTravel")]
        public class CreatePlantLimTravelPatch
        {
            [HarmonyPostfix]
            public static bool Prefix(ref bool __result)
            {
                if (!Plugin.isTravelPlant)
                    return true;

                __result = false;
                return false;
            }
        }

        // Token: 0x0200003D RID: 61
        [HarmonyPatch(typeof(CreatePlant), "CheckBox")]
        public class CreatePlantCheckBoxPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result)
            {
                if (Plugin.anyPlant)
                    __result = true;
            }
        }

        // Token: 0x0200003E RID: 62
        [HarmonyPatch(typeof(Plant), "Update")]
        public class PlantUpdatePatch
        {
            [HarmonyPostfix]
            public static void Prefix(Plant __instance)
            {
                if (Plugin.isACD && __instance.attributeCountdown > 1f)
                    __instance.attributeCountdown -= 1f;

                if (Input.GetKeyDown(KeyCode.Alpha7))
                    __instance.Die(0);
            }
        }

        // Token: 0x0200003F RID: 63
        [HarmonyPatch(typeof(Shooter), "Update")]
        public class ShooterUpdatePatch
        {
            [HarmonyPostfix]
            public static void Prefix(Shooter __instance)
            {
                if (Plugin.isPlantAbnormalShoot)
                    __instance.AnimShoot();
            }
        }

        // Token: 0x02000041 RID: 65
        [HarmonyPatch(typeof(Thrower), "PlantShootUpdate")]
        public class ThrowerPlantShootUpdatePatch
        {
            [HarmonyPostfix]
            public static void Prefix(Thrower __instance)
            {
                if (Input.GetKeyDown(KeyCode.M))
                    __instance.AnimSuperShoot();

                if (Plugin.isThrowerType)
                    __instance.anim.SetTrigger("shoot2");
            }
        }

        // Token: 0x02000042 RID: 66
        [HarmonyPatch(typeof(Thrower), "Shoot1")]
        public class ThrowerShoot1Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Thrower __instance)
            {
                if (!Plugin.isThrowerType)
                    return true;

                __instance.Shoot2();
                return false;
            }
        }

        // Token: 0x02000043 RID: 67
        [HarmonyPatch(typeof(ShootingManager), "Update")]
        public class ShootingManagerUpdatePatch
        {
            [HarmonyPostfix]
            public static void Prefix(ShootingManager __instance)
            {
                if (Plugin.isTravelRefresh)
                    __instance.refreshCount = 9999999;
            }
        }

        // Token: 0x02000044 RID: 68
        [HarmonyPatch(typeof(TravelRefresh))]
        public class TravelRefreshOnMouseUpAsButtonPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPrefix]
            public static void PrefixAwake(TravelRefresh __instance)
            {
                if (!Plugin.isTravelRefresh)
                    return;

                __instance.refreshTimes = 9999999;
                __instance.text.text = "刷新(∞)";
                __instance.text_shadow.text = "刷新(∞)";
            }

            [HarmonyPatch("OnMouseUpAsButton")]
            [HarmonyPrefix]
            public static void Prefix(TravelRefresh __instance)
            {
                if (Plugin.isTravelRefresh)
                    __instance.refreshTimes = 9999999;
            }

            [HarmonyPatch("OnMouseUpAsButton")]
            [HarmonyPostfix]
            public static void Postfix(TravelRefresh __instance)
            {
                if (!Plugin.isTravelRefresh)
                    return;

                __instance.refreshTimes = 9999999;
                __instance.text.text = "刷新(∞)";
                __instance.text_shadow.text = "刷新(∞)";
            }
        }

        // Token: 0x02000045 RID: 69
        [HarmonyPatch(typeof(TravelStore), "Update")]
        public class TravelStoreUpdatePatch
        {
            [HarmonyPostfix]
            public static void Postfix(TravelStore __instance)
            {
                if (Plugin.isTravelRefresh)
                    __instance.refreshCount = 0;
            }
        }

        // Token: 0x02000046 RID: 70
        [HarmonyPatch(typeof(Money), "Awake")]
        public class MoneyAwakePatch
        {
            [HarmonyPostfix]
            public static void Postfix(Money __instance)
            {
                Vector2 pos = __instance.transform.position;
                pos.x += 1f;
                __instance.transform.position = pos;
            }
        }
        // Token: 0x0200004A RID: 74
        [HarmonyPatch(typeof(Lawnf))]
        public class LawnfPatch
        {
            [HarmonyPatch("GetSuperPlantCount", new[] { typeof(Board) })]
            [HarmonyPostfix]
            public static void PostGetSuperPlantCount(ref int __result)
            {
                if (Plugin.isAbyssMaxPlantCount)
                    __result = -1;
            }

            [HarmonyPatch("GetPlantCount", new[] { typeof(Board) })]
            [HarmonyPostfix]
            public static void PostGetPlantCount(ref int __result)
            {
                if (Plugin.isAbyssMaxPlantCount)
                    __result = -1;
            }

            [HarmonyPatch("GetUltiPlantCount", new[] { typeof(Board) })]
            [HarmonyPostfix]
            public static void PostGetUltiPlantCount(ref int __result)
            {
                if (Plugin.isAbyssMaxPlantCount)
                    __result = -1;
            }

            [HarmonyPatch("BannedInAbyss", new[] { typeof(PlantType) })]
            [HarmonyPostfix]
            public static void PostBannedInAbyss(ref bool __result)
            {
                if (Plugin.isBannedInAbyss)
                    __result = false;
            }

            [HarmonyPatch("CheckIfPlantUnlock")]
            [HarmonyPostfix]
            public static void PostCheckIfPlantUnlock(ref PlantType thePlantType, ref UnlockType __result)
            {
                if (Plugin.isAUnlock)
                    __result = 0;
            }
        }

        // (SolarSunflowerPatch and further patches would follow the same cleaned pattern)
    }
}
