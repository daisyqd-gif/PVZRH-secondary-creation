using BepInEx;
using HarmonyLib;
using UnityEngine;
using CustomPlantClass;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using CustomPlantClass.Main;
using Unity.VisualScripting;
using GameLevel.RogueShooting;
using CustomPlantClass.Runtime;
using System.Collections.Generic;
using CustomPlantClass.RogueShootingManager;
namespace UltimateCannon_RS
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : BasePlugin
    {
        public Harmony patch;
        public override void Load()
        {
            DataMgr.AutoRegisterTypes(Assembly.GetExecutingAssembly());
            patch=Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(),MyPluginInfo.PluginGuid);
            BaseBuff buff1 = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = PlantType.UltimateCannon,
                CustomTitle = "进化：装填",
                CustomDescription = "装填时间-3秒",
                CustomBuffType = ShootingBuffType.UniqueUpgrade,
                CustomOnGet = () =>
                {
                    if(ShootingManager.Instance != null && ShootingManager.Instance.TryGetPlant(PlantType.UltimateCannon, out Plant plant))
                    {
                        plant.thePlantAttackInterval -= 3;
                    }
                }
            });
            BaseBuff buff2 = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = PlantType.UltimateCannon,
                CustomTitle = "进化：副炮",
                CustomDescription = "副炮炮弹+1\n副炮炮弹范围+0.5",
                CustomBuffType = ShootingBuffType.UniqueUpgrade,
                CustomOnGet = () =>
                {
                    CobCannon_RogueShootingComponent.level++;
                }
            });
            BaseBuff buff3 = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = PlantType.UltimateCannon,
                CustomTitle = "质变：兵贵神速",
                CustomDescription = "究极冷寂加农炮的装填时间/3",
                CustomBuffType = ShootingBuffType.QualitativeChange,
                CustomOnGet = () =>
                {
                    TravelMgr.Instance.GetUltiBuff((UltiBuff)15);
                }
            });
            var cobCannon = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = PlantType.CobCannon,
                CustomBuffs = () => new List<BaseBuff>(){ new UpgradeBuff(PlantType.CobCannon, PlantType.IceCannon) },
                CustomReinforcePlant = (p) =>
                {
                    p.AddComponent<CobCannon_RogueShootingComponent>();
                    p.thePlantAttackInterval = 2f;
                },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            var iceCannon = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = PlantType.IceCannon,
                CustomBuffs = () => new List<BaseBuff>(){ new UpgradeBuff(PlantType.IceCannon, PlantType.UltimateCannon) },
                CustomReinforcePlant = (p) =>
                {
                    p.AddComponent<CobCannon_RogueShootingComponent>();
                    p.thePlantAttackInterval = 2f;
                },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            var ultiCannon = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = PlantType.UltimateCannon,
                CustomBuffs = () => new List<BaseBuff>(){ new DamageBuff(PlantType.UltimateCannon), new SpeedBuff(PlantType.UltimateCannon), buff1, buff2, buff3},
                CustomReinforcePlant = (p) =>
                {
                    p.AddComponent<CobCannon_RogueShootingComponent>();
                },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            RegistryHelper.AddCustomBaseRogueShootingPlant(PlantType.CobCannon,cobCannon);
            RegistryHelper.AddCustomRogueShootingPlant(PlantType.IceCannon,iceCannon);
            RegistryHelper.AddCustomRogueShootingPlant(PlantType.UltimateCannon,ultiCannon);
            RegistryHelper.AddCustomEvolutionPathway(PlantType.CobCannon,PlantType.CobCannon,PlantType.IceCannon,PlantType.UltimateCannon);
        }
    }

    public class CobCannon_RogueShootingComponent : MonoBehaviour
    {
        CobCannon cannon => GetComponent<CobCannon>();
        [ResetOnBoardDestroy(0)]
        public static int level = 0;
        public void FixedUpdate()
        {
            if (cannon.avaliable)
            {
                var func = (Zombie z) => !z.isMindControlled && z.Alive;
                Zombie z = Lawnf.GetNearestZombie(cannon.board,transform.position,func);
                if( z != null )
                {
                    cannon.cannonTarget = z.axis.position;
                }
                else
                {
                    return;
                }
                cannon.StartShoot();
            }
        }
        public void AnimSubShoot()
        {
            GameAPP.PlaySound(Random.Range(3, 5), 0.5f, 1f);
            Transform child = transform.GetChild(1);
            Vector3 position = child.position;
            for( int i = 0; i < level+1; i++ )
            {
                Bullet bullet = CreateBullet.Instance.SetBullet(position.x, position.y, cannon.thePlantRow, BulletType.Bullet_ultimateKernal, BulletMoveWay.Track);
                bullet.Damage = cannon.attackDamage;
                bullet.shootingLevel = level;
                bullet.fromType = cannon.thePlantType;
            }
        }
        public void OnDestroy()
        {
            level=0;
        }
    }

    [HarmonyPatch(typeof(Bullet_cannon), nameof(Bullet_cannon.HitLand))]
    public class Bullet_cannon_HitLand_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Bullet_cannon __instance)
        {
            if (__instance.theBulletType == BulletType.Bullet_ultimateCannon && __instance.board.boardTag.rogueShooting)
            {
                Vector2 vector = __instance.transform.GetChild(0).GetChild(0).position;
                List<Zombie> list = new();
                foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(vector, 1.5f, __instance.zombieLayer))
                {
                    if (collider2D != null)
                    {
                        Zombie component = collider2D.gameObject.GetComponent<Zombie>();
                        if (component != null && !component.isMindControlled)
                        {
                            list.Add(component);
                        }
                    }
                }
                foreach( var zombie in list )
                {
                    if(zombie != null)
                    {
                        zombie.TakeDamage(__instance.Damage*3,__instance.Cast<IDamageMaker>(),DamageType.Carred,__instance.fromType);
                        zombie.SetFreeze(10f,1000);
                    }
                }
                Doom.SetDoom(Board.Instance, vector, DoomType.IceDoom_big, null, !GameAPP.config.distablexplodeFlash);
                foreach( var zombie in new List<Zombie>([..Lawnf.GetAllZombies()]) )
                {
                    if(zombie != null)
                    {
                        zombie.TakeDamage(__instance.Damage,__instance.Cast<IDamageMaker>(),DamageType.Carred,__instance.fromType);
                        zombie.SetFreeze(10f,1000);
                    }
                }
                if(ShootingManager.Instance != null && ShootingManager.Instance.TryGetPlant(PlantType.UltimateCannon, out Plant cannon))
                {
                    for( float i = 0f; i< 360f; i += 10f )
                    {
                        Bullet bullet = CreateBullet.Instance.SetBullet(vector.x, vector.y, Mouse.Instance.GetRowFromY(vector.x, vector.y), BulletType.Bullet_ultimateKernal, BulletMoveWay.Free);
                        bullet.Damage = cannon.attackDamage;
                        bullet.shootingLevel = CobCannon_RogueShootingComponent.level;
                        bullet.fromType = cannon.thePlantType;
                        bullet.transform.Rotate(0,0,i);
                        bullet.col.enabled = false;
                        bullet.AddComponent<DelayedTracking_UKernel>();
                    }
                }
                __instance.Die();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UltimateCannon))]
    public class UltimateCannon_Patch
    {
        [HarmonyPatch(nameof(UltimateCannon.AnimSubShoot))]
        [HarmonyPrefix]
        public static bool AnimSubShoot_Prefix(UltimateCannon __instance)
            => __instance != null && !__instance.TryGetComponent<CobCannon_RogueShootingComponent>(out _);
        [HarmonyPatch(nameof(UltimateCannon.Update))]
        [HarmonyPrefix]
        public static bool Update_Prefix(UltimateCannon __instance)
        {
            if(__instance != null || !__instance.board.boardTag.rogueShooting) return true;
            if (GameAPP.theGameStatus == GameStatus.InGame)
            {
                __instance.PlantUpdate();
            }
            __instance.UpdateAttackCountDown();
            if (0 >= __instance.thePlantAttackCountDown)
            {
                float num = Random.Range(0.95f, 1.05f);
                float num2 = num * 1.5f;
                __instance.thePlantAttackCountDown = num2;
                var a = (Zombie z) => z != null && Lawnf.InLandUpStatus(z.theStatus) && z.Alive && !z.isMindControlled;
                if (Lawnf.GetNearestZombie(__instance.board,__instance.transform.position,a) != null)
                {
                    __instance.anim.SetTrigger("subshoot");
                }
                else if (__instance.SearchBoss() != null)
                {
                    __instance.anim.SetTrigger("subshoot");
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Bullet_ultimateKernal), nameof(Bullet_ultimateKernal.HitZombie))]
    public class Bullet_ultimateKernal_HitZombie_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Bullet_ultimateKernal __instance, Zombie zombie)
        {
            if(!__instance.board.boardTag.rogueShooting) return true;
            var hits = Physics2D.OverlapCircleAll(__instance.col.bounds.center,0.5f*(1+__instance.shootingLevel),__instance.zombieLayer);
            ParticleManager.Instance.SetParticle(ParticleType.BombCloud_blue,__instance.col.bounds.center);
            foreach( var i in hits)
            {
                if(i != null && i.TryGetComponent(out Zombie z))
                {
                    z.TakeDamage(__instance._damage, __instance.Cast<IDamageMaker>(), DamageType.IceShieldless, __instance.fromType);
                    z.AddfreezeLevel(10, 0);
                }
            }
            GameAPP.PlaySound(SoundType.CherryBomb);
            __instance.Die();
            return false;
        }
    }
    public class DelayedTracking_UKernel : BulletComponent
    {
        public float delay = 0.15f;

        public void FixedUpdate()
        {
            delay -= Time.fixedDeltaTime;
            if (delay > 0f)
                return;
            if (bullet != null)
            {
                bullet.col.enabled = true;
                bullet.MoveWay = BulletMoveWay.Track;
            }

            Destroy(this);
        }
    }

    public static class MyPluginInfo
    {
        public const string PluginGuid = "UltimateCannon_RS.Bepinex";
        public const string PluginName = "UltimateCannon_RS";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
