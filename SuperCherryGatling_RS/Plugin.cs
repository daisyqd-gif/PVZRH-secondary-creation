using BepInEx;
using GameLevel.RogueShooting;
using HarmonyLib;
using System.Collections.Generic;
using CustomPlantClass;
using CustomPlantClass.Main;
using CustomPlantClass.RogueShootingManager;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
namespace SuperCherryGatling_RS
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        public override void InitializeMod()
        {
            var ubuff1 = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = PlantType.SuperCherryGatling,
                CustomTitle = "进化：散射",
                CustomDescription = "爆炸樱桃概率 + 5%\n额外散射3发子弹概率 + 10%",
                CustomBuffType = ShootingBuffType.UniqueUpgrade,
                CustomOnGet = delegate
                {
                    if(ShootingManager.Instance.TryGetPlant(PlantType.SuperCherryGatling,out Plant p))
                    {
                        p.shootingLevel ++;
                    }
                }
            });
            var qbuff = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = PlantType.SuperCherryGatling,
                CustomTitle = "质变：极速爆发",
                CustomDescription = "各种超级机枪射手和究极机枪射手释放大招的概率x3",
                CustomBuffType = ShootingBuffType.QualitativeChange,
                CustomOnGet = delegate
                {
                    TravelMgr.Instance.GetUltiBuff((UltiBuff)50);
                }
            });
            RegistryHelper.RegisterBuffsForPlant(PlantType.SuperCherryGatling,true,true,false);
            var cfg = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = PlantType.SuperCherryGatling,
                CustomBuffs = () => new List<BaseBuff>(){new DamageBuff(PlantType.SuperCherryGatling), new SpeedBuff(PlantType.SuperCherryGatling), ubuff1, qbuff},
                CustomReinforcePlant = (p) =>
                {
                    if (p.damageAdder == null)
                    {
                        p.damageAdder = new();
                    }

                    if (p.damageAdder.ContainsKey(PlantDamageAdder.Shooting))
                    {
                        p.damageAdder[PlantDamageAdder.Shooting] += 6f;
                    }
                    else
                    {
                        p.damageAdder[PlantDamageAdder.Shooting] = 6f;
                    }

                    var shootingBonus = p.damageAdder.TryGetValue(PlantDamageAdder.Shooting, out var bonus) ? bonus : 0f;
                    p.attackDamage = Mathf.Max(1, Mathf.RoundToInt(p.attackDamage + shootingBonus));
                },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            RegistryHelper.InjectUpgradeBuff(RSConfigType.CherryGatling,PlantType.SuperCherryGatling);
            RegistryHelper.AddCustomRogueShootingPlant(PlantType.SuperCherryGatling,cfg);
            RegistryHelper.AddCustomEvolutionPathway(PlantType.Peashooter,PlantType.Peashooter,PlantType.CherryGatling,PlantType.SuperCherryGatling);
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    [HarmonyPatch(typeof(SuperSnowGatling))]
    public static class SuperSnowGatling_Patch
    {
        [HarmonyPatch(nameof(SuperSnowGatling.AttributeEvent))]
        [HarmonyPrefix]
        public static bool AttributeEvent_Prefix(SuperSnowGatling __instance)
        {
            if(__instance != null && __instance.board.boardTag.rogueShooting)
            {
                Vector3 position = __instance.shoot.position;
                for(int i = 0 ; i < 4 ; i++)
                {
                    __instance.SuperShoot
                    (
                        Random.Range(-15f, 15f), 
                        Random.Range(12f, 14f), 
                        position.x + Random.Range(-0.1f, 0.1f), 
                        position.y + Random.Range(-0.1f, 0.1f), 
                        BulletMoveWay.SuperGatling, 
                        __instance.attackDamage
                    );
                    if(__instance.shootingLevel * 10f > Random.Range(0f, 100f))
                    {
                        __instance.SuperShoot
                        (
                            Random.Range(-15f, 15f), 
                            Random.Range(12f, 14f), 
                            position.x + Random.Range(-0.1f, 0.1f), 
                            position.y + Random.Range(-0.1f, 0.1f), 
                            BulletMoveWay.SuperGatling, 
                            __instance.attackDamage
                        );
                        __instance.SuperShoot
                        (
                            Random.Range(-15f, 15f), 
                            Random.Range(12f, 14f), 
                            position.x + Random.Range(-0.1f, 0.1f), 
                            position.y + Random.Range(-0.1f, 0.1f), 
                            BulletMoveWay.SuperGatling, 
                            __instance.attackDamage
                        );
                        __instance.SuperShoot
                        (
                            Random.Range(-15f, 15f), 
                            Random.Range(12f, 14f), 
                            position.x + Random.Range(-0.1f, 0.1f), 
                            position.y + Random.Range(-0.1f, 0.1f), 
                            BulletMoveWay.SuperGatling, 
                            __instance.attackDamage
                        );
                    }
                }
                GameAPP.PlaySound(Random.Range(3, 5), 0.5f, 1f);
                if (__instance.timer <= 0f && !__instance.keepShooting)
                {
                    __instance.AttributeCountdown = 0f;
                    __instance.anim.SetBool("shooting", false);
                }
                else
                {
                    __instance.AttributeCountdown = 0.02f;
                }
                return false;
            }
            return true;
        }
        [HarmonyPatch(nameof(SuperSnowGatling.Shoot1))]
        [HarmonyPrefix]
        public static bool Shoot1_Prefix(SuperSnowGatling __instance, ref Bullet __result)
        {
            if(__instance != null && __instance.board.boardTag.rogueShooting)
            {
                Bullet bullet = null;
                if (__instance.attributeCount > 100 - __instance.attributeFloat || __instance.keepShooting || (Lawnf.TravelUltimate((UltiBuff)50)?6 : 2) >= Random.Range(0, 100))
                {
                    __instance.SuperShoot(5f);
                }
                else
                {
                    int num4 = __instance.attributeCount + 1;
                    __instance.attributeCount = num4;
                    bool isScatter = __instance.shootingLevel * 10f > Random.Range(0f, 100f);
                    Vector3 position = __instance.shoot.position;
                    BulletType bulletType = __instance.GetBulletType();
                    BulletMoveWay bulletMoveWay = isScatter ? BulletMoveWay.SuperGatling : BulletMoveWay.MoveRight;
                    for(int i = 0; i < (isScatter ? 4 : 1); i++)
                    {
                        bullet = CreateBullet.Instance.SetBullet(position.x, position.y, __instance.thePlantRow, bulletType, bulletMoveWay);
                        if (bullet != null)
                        {
                            bullet.Damage = __instance.attackDamage;
                            bullet.fromType = __instance.thePlantType;
                            if(isScatter)
                            {
                                bullet.transform.Rotate(0f, 0f, Random.Range(-15f, 15f));
                            }
                            GameAPP.PlaySound(Random.Range(3, 5), 0.5f, 1f);
                        }
                    }
                    __result = bullet;
                }
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(CherrySuperGatling))]
    public static class CherrySuperGatling_Patch
    {
        [HarmonyPatch(nameof(CherrySuperGatling.GetBulletType))]
        [HarmonyPrefix]
        public static bool AttributeEvent_Prefix(CherrySuperGatling __instance, ref BulletType __result)
        {
            if(__instance != null && __instance.shootingLevel * 5f > Random.Range(0f,100f) && __instance.shootingLevel != 0)
            {
                __result = BulletType.Bullet_superCherry;
                return false;
            }
            return true;
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "SuperCherryGatling_RS.Bepinex";
        public const string PluginName = "SuperCherryGatling_RS";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
