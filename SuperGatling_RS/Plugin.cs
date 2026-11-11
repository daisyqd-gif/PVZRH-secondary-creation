using BepInEx;
using GameLevel.RogueShooting;
using HarmonyLib;
using System.Collections.Generic;
using CustomPlantClass.Main;
using CustomPlantClass.RogueShootingManager;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using CustomPlantClass.CustomizeLib;
namespace SuperCherryGatling_RS
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        public static AdvBuff QBuff_2;
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
            var ubuff2 = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = PlantType.SuperSnowGatling,
                CustomTitle = "进化：极冰",
                CustomDescription = "植物施加的冻结值+100%。寒冰豌豆每增加1点冻结值，子弹伤害+1%。额外散射3发子弹概率 + 10%",
                CustomBuffType = ShootingBuffType.UniqueUpgrade,
                CustomOnGet = delegate
                {
                    if(ShootingManager.Instance.TryGetPlant(PlantType.SuperSnowGatling,out Plant p))
                    {
                        p.GetOrAddComponent<SuperSnowGatling_Shooting>();
                        p.shootingLevel ++;
                    }
                    SuperSnowGatling_Shooting.BuffCount++;
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
            (QBuff_2,var qbuff_2) = RegistryHelper.RegisterCustomQualitativeChangeBuff(
                "凝冰之心","寒冰豌豆升级为极冰豆；命中目标时，会额外造成一次相当于原始伤害100%的范围伤害。\n其子弹赋予的冻结值翻倍",
                PlantType.SuperSnowGatling,delegate
                {
                    if(ShootingManager.Instance.TryGetPlant(PlantType.SuperSnowGatling,out Plant p))
                    {
                        p.GetOrAddComponent<SuperSnowGatling_Shooting>().IsZhiBian = true;
                    }
					TravelMgr.Instance.GetNormalBuff((AdvBuff)1011);
                }
                );
            RegistryHelper.RegisterBuffsForPlant(PlantType.SuperCherryGatling,true,true,false);
            RegistryHelper.RegisterBuffsForPlant(PlantType.SuperSnowGatling,true,true,false);
            var cfg = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = PlantType.SuperCherryGatling,
                CustomBuffs = () => new List<BaseBuff>(){new DamageBuff(PlantType.SuperCherryGatling), new SpeedBuff(PlantType.SuperCherryGatling), ubuff1, qbuff},
                CustomReinforcePlant = (p) =>
                {
                    p.attackDamage *= 19;
                },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            var cfg_ice = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = PlantType.SuperSnowGatling,
                CustomBuffs = () => new List<BaseBuff>(){new DamageBuff(PlantType.SuperSnowGatling), new SpeedBuff(PlantType.SuperSnowGatling), ubuff2, qbuff_2},
                CustomReinforcePlant = (p) =>
                {
                    p.attackDamage *= 12;
                },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            var cfg_base = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = PlantType.SuperGatling,
                CustomBuffs = () => new List<BaseBuff>()
                {
                    new UpgradeBuff(PlantType.SuperGatling, PlantType.SuperCherryGatling),
                    new UpgradeBuff(PlantType.SuperGatling, PlantType.SuperSnowGatling)
                },
                CustomReinforcePlant = (p) =>
                {
                    p.ModifyDamage(PlantDamageAdder.Shooting,14f,false,new Il2CppSystem.Nullable<float>(float.MaxValue));
                },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            RegistryHelper.InjectUpgradeBuff(RSConfigType.Peashooter,PlantType.SuperGatling);
            RegistryHelper.AddCustomRogueShootingPlant(PlantType.SuperCherryGatling,cfg);
            RegistryHelper.AddCustomRogueShootingPlant(PlantType.SuperSnowGatling,cfg_ice);
            RegistryHelper.AddCustomRogueShootingPlant(PlantType.SuperGatling,cfg_base);
            RegistryHelper.AddCustomEvolutionPathway(PlantType.Peashooter,PlantType.Peashooter,PlantType.SuperGatling,PlantType.SuperCherryGatling);
            RegistryHelper.AddCustomEvolutionPathway(PlantType.Peashooter,PlantType.Peashooter,PlantType.SuperGatling,PlantType.SuperSnowGatling);
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    public class SuperSnowGatling_Shooting : MonoBehaviour
    {
        public bool IsZhiBian = false;
        public static int BuffCount = 0;
        public void Awake()
        {
            BuffCount = 0;
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
                var hasComp = false;
                if(__instance.TryGetComponent<SuperSnowGatling_Shooting>(out var s))
                {
                    hasComp = true;
                }
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
                    BulletType bulletType = __instance.GetBulletType() ;
                    BulletMoveWay bulletMoveWay = isScatter ? BulletMoveWay.SuperGatling : BulletMoveWay.MoveRight;
                    for(int i = 0; i < (isScatter ? 4 : 1); i++)
                    {
                        bullet = CreateBullet.Instance.SetBullet(position.x, position.y, __instance.thePlantRow, bulletType, bulletMoveWay);
                        if (bullet != null)
                        {
                            bullet.Damage = __instance.attackDamage;
                            bullet.fromType = __instance.thePlantType;
                            bullet.shootingLevel = __instance.shootingLevel;
                            if(hasComp)
                            {
                                bullet.GetOrAddComponent<SuperSnowGatling_Shooting>().IsZhiBian=s.IsZhiBian;
                            }
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
        [HarmonyPatch(nameof(SuperSnowGatling.GetBulletType))]
        [HarmonyPrefix]
        public static bool GetBulletType_Prefix(SuperSnowGatling __instance, ref BulletType __result)
        {
            if(__instance != null && __instance.TryGetComponent<SuperSnowGatling_Shooting>(out var s) && s.IsZhiBian)
            {
                __result = BulletType.Bullet_extremeSnowPea;
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
        public static bool GetBulletType_Prefix(CherrySuperGatling __instance, ref BulletType __result)
        {
            if(__instance != null && __instance.shootingLevel * 5f > Random.Range(0f,100f) && __instance.shootingLevel != 0)
            {
                __result = BulletType.Bullet_superCherry;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Zombie))]
	public static class ZombiePatch
	{
		[HarmonyPatch(nameof(Zombie.AddfreezeLevel))]
		[HarmonyPrefix]
		public static void AddfreezeLevel_Prefix(ref int level)
		{
			level *= 1 + SuperSnowGatling_Shooting.BuffCount;
		}
	}
    [HarmonyPatch(typeof(Bullet_extremeSnowPea))]
	public static class Bullet_extremeSnowPeaPatch
	{
		[HarmonyPatch(nameof(Bullet_extremeSnowPea.HitZombie))]
		[HarmonyPrefix]
		public static void HitZombie_Prefix(Bullet_extremeSnowPea __instance)
		{
            __instance.Damage += (int)(__instance.Damage * (20 * __instance.shootingLevel * 2 * 0.01f * __instance.shootingLevel));
            if (Plugin.QBuff_2.IsActive())
            {
                foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(__instance.transform.position, 1.5f, LayerMask.GetMask(new string[] { "Zombie" })))
                {
                    bool flag3 = collider2D == null || UnityObjectUtility.IsDestroyed(collider2D) || collider2D.gameObject == null || collider2D.gameObject.IsDestroyed();
                    if (!flag3)
                    {
                        bool flag4 = !collider2D.gameObject.TryGetComponent<Zombie>(out var zombie) || zombie == null || zombie.IsDestroyed();
                        if (!flag4)
                        {
                            if (Lawnf.InLandStatus(zombie.theStatus) || zombie.theStatus == ZombieStatus.Flying)
                            {
                                zombie.TakeDamage(__instance.Damage / 2, __instance.Cast<IDamageMaker>(), DamageType.IceAll, __instance.fromType, false);
                                zombie.AddfreezeLevel(5, 0);
                                zombie.SetCold(10f, 0, false);
                            }
                        }
                    }
                }
                CreateParticle.SetParticle(97, __instance.transform.position, __instance.theBulletRow, true);
            }
		}
	}
    [HarmonyPatch(typeof(Bullet_snowPea))]
	public static class Bullet_snowPeaPatch
	{
		[HarmonyPatch(nameof(Bullet_snowPea.HitZombie))]
		[HarmonyPrefix]
		public static void HitZombie_Prefix(Bullet_snowPea __instance)
		{
            __instance.Damage += (int)(__instance.Damage * (20 * __instance.shootingLevel * 2 * 0.01f * __instance.shootingLevel));
		}
	}
    public class MyPluginInfo
    {
        public const string PluginGuid = "SuperCherryGatling_RS.Bepinex";
        public const string PluginName = "SuperCherryGatling_RS";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
