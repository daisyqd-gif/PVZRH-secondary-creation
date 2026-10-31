global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System;
global using System.Reflection;
global using UnityEngine;
global using System.Collections.Generic;
global using CustomPlantClass;
global using CustomPlantClass.Main;
global using GameLevel.RogueShooting;
global using Random = UnityEngine.Random;
global using System.Threading.Tasks;
global using CustomPlantClass.Runtime.Tasks;
global using System.Runtime.CompilerServices;
global using CustomPlantClass.Runtime.Core;
using CustomParticle = CustomPlantClass.Main.CustomParticle;
namespace HorseBossPhase2
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ID ZombieID = -1;
            public static ID BombCloudID = -1;
            public static CustomItemType SickleID = -1;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "ultimatehorsephase2"
            );
            DataContainer.ZombieID = DataMgr.AllocateID();
            DataContainer.BombCloudID = DataMgr.AllocateID();
            DataContainer.SickleID = GameObjectMgr.Register<Sickle>(assetBundle.GetAsset<GameObject>("Sickle"));
        }
        public override void InitializeZombies()
        {
            CustomCore.RegisterCustomZombie<Zombie, UltimateHorsePhase2>
            (
                DataContainer.ZombieID,
                assetBundle.GetAsset<GameObject>("UltimateHorse2_Boss"),
                assetBundle.GetAsset<Sprite>("GatlingBlackFootball2"),
                10000,
                100000,
                0,
                0
            );
            CustomCore.RegisterCustomParticle(DataContainer.BombCloudID,assetBundle.GetAsset<GameObject>("BombCloud"));
        }
    }

    public class UltimateHorsePhase2 : ZombieComponent
    {
        Transform SicklePos;
        float cd = 0.1f;
        int superremain = 0;
        bool super = false;
        bool stopshoot = false;
        bool jumping = false;
        Vector2 savedPos;
        
        static ZombieType[] spawnZombieTypes =
        [
            ZombieType.BlackFootball_a,
            ZombieType.BlackFootball_b,
            ZombieType.BlackFootball_c,
            ZombieType.BlackFootball_c2,
            ZombieType.BlackFlagFootball,
            ZombieType.GatlingBlackFootball,
            ZombieType.BlackHorse,
            ZombieType.BlackJackboxZombie,
            ZombieType.SuperBlackFootball,
            ZombieType.SuperBlackHorse,
            ZombieType.BlackFootball,
            ZombieType.ElephantZombie_a,
            ZombieType.ElephantZombie_b,
            ZombieType.ElephantZombie_c
        ];
        Dictionary<Skill, Action<Zombie, UltimateHorsePhase2>> ActionOnSkill = new()
        {
            [Skill.Summon] = (zombie, h) =>
            {
                zombie.anim.SetTriggerString("summon");
            },
            [Skill.Sickle] = (zombie, h) =>
            {
                zombie.anim.SetTriggerString("attack");
            },
            [Skill.Jump] = (zombie, h) =>
            {
                h.savedPos = zombie.transform.position;
                if (h.jumping)
                {
                    return;
                }

                zombie.anim.SetTriggerString("walk");
                h.jumping = true;
            },
            [Skill.SuperShoot] = (zombie, h) =>
            {
                zombie.anim.SetBoolString("shooting", true);
                zombie.ChangeStatus(ZombieStatus.Raised);
                h.superremain = 360;
                h.super = true;
            }
        };
        Skill randomSkill => new[] { Skill.Summon, Skill.Sickle, Skill.SuperShoot, Skill.Jump }.GetRandomItem();
        public void Awake()
        {
            SicklePos = transform.FindChild("Sickle");
            zombie.shoot = transform.FindChild("Shoot");
        }
        public void Start()
        {
            if(zombie != null)
            {
                if(zombie.board!= null)
                {
                    if( zombie.board.boardTag.rogueShooting || zombie.board.boardTag.HorseBoss )
                    {
                        if (!GameAPP.soundManager.musics.ContainsKey((MusicType)2141984) || GameAPP.soundManager.musics[(MusicType)2141984] != null)
                        {
                            GameAPP.Instance.PlayMusic(MusicType.HorseBoss);
                        }
                        else
                        {
                            GameAPP.Instance.PlayMusic((MusicType)2141984);
                        }
                    }
                }
            }
        }
        public void Attack()
        {
            var sickle = GameObjectMgr.Instantiate(Plugin.DataContainer.SickleID, SicklePos.position, Quaternion.identity, zombie.board.transform).GetComponent<Sickle>();
            sickle.Damage = zombie.theAttackDamage;
            sickle.row = zombie.theZombieRow;
            sickle.plantLayer = zombie.plantLayer;
        }
        public void ChangePos()
        {
            transform.position = savedPos;
        }
        public void SwitchRow()
        {
            zombie.ChangeRow(zombie.GetAvaliableRow(zombie.theZombieRow));
        }
        public void AnimSummon()
        {
            IEnumerable<ZombieType> zombieTypes = ShootingManager.Instance != null ? ShootingManager.Instance.zombieTypes1.ToSystemList() : spawnZombieTypes;
            int num = 0;
            do
            {
                int num2 = 0;
                for (; ; )
                {
                    Board board = zombie.board;
                    if (num2 >= board.rowNum)
                    {
                        break;
                    }
                    Zombie zombie2 = CreateZombie.Instance.SetZombie(num2, zombieTypes.GetRandomItem(), 9.9f);
                    bool flag2 = zombie2 != null;
                    if (flag2)
                    {
                        bool flag3 = Lawnf.TravelDebuff((TravelDebuff)10033);
                        if (flag3)
                        {
                            Lawnf.SetZombieHealth(zombie, 1.6f);
                        }
                    }
                    num2++;
                }
                num++;
            }
            while (num < (ShootingManager.Instance != null ? 10 : 3));
        }
        public void TryStop()
        {
            if(zombie.Column <= (zombie.board.boardTag.isBigMap ? 24 : 10) && !jumping)
            {
                zombie.anim.SetTriggerString("stop");
            }
            else if(zombie.Column <= 2 && jumping)
            {
                zombie.anim.SetTriggerString("jump");
            }
        }
        public void FixedUpdate()
        {
            if(zombie == null) return;
            if (cd <= 0.0f && zombie.Alive && !stopshoot)
            {
                zombie.anim.SetTrigger("shoot");
                float num = zombie.theSpeed;
                if (0 <= (int)zombie.theSpeed)
                {
                    if (num > 1f)
                    {
                        num = 1f;
                    }
                }
                else
                {
                    num = 0f;
                }
                zombie.attributeCountDown = 0.1f / (num * 0.8f + 0.2f);
            }
            else if (zombie.Alive && !stopshoot)
            {
                cd -= Time.fixedDeltaTime;
            }
        }
        public void DoSkill()
        {
            if(zombie == null) return;
            ActionOnSkill[randomSkill](zombie, this);
        }
        public void ReShoot()
        {
            stopshoot = false;
        }
        public void AnimShoot()
        {
            if (zombie.shoot != null)
            {
                Vector3 position = zombie.shoot.position;
                if (super)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Bullet bullet = CreateBullet.Instance.SetBullet(
                            position.x + Random.Range(-0.1f, 0.1f),
                            position.y + Random.Range(-0.1f, 0.1f),
                            zombie.theZombieRow,
                            BulletType.Bullet_ironPea_curse,
                            BulletMoveWay.Free, !zombie.isMindControlled);
                        bullet.Damage = zombie.theAttackDamage / 100;
                        bullet.transform.Rotate(0, 0, 180 + Random.Range(-15f, 15f));
                        bullet.shootByZombie = true;
                        GameAPP.PlaySound(Random.Range(3, 5), 0.5f, 1f);
                        superremain--;
                    }
                    if (superremain <= 0)
                    {
                        zombie.anim.SetBoolString("shooting", false);
                        super = false;
                        zombie.ChangeStatus(ZombieStatus.Default);
                    }
                }
                else
                {
                    Bullet bullet = CreateBullet.Instance.SetBullet(
                        position.x, position.y, zombie.theZombieRow,
                        BulletType.Bullet_ironPea_curse,
                        BulletMoveWay.Left, !zombie.isMindControlled);
                    bullet.Damage = zombie.theAttackDamage / 100;
                    bullet.shootByZombie = true;
                    GameAPP.PlaySound(Random.Range(3, 5), 0.5f, 1f);
                }
            }
        }
        public void OnTriggerStay2D(Collider2D collision)
        {
            bool flag = collision.TryGetComponent<Plant>(out var plant);
            if (flag && plant.thePlantRow == zombie.theZombieRow && jumping)
            {
                plant.Crashed(0, 0, null);
                return;
            }
            bool flag4 = collision.TryGetComponent<Zombie>(out var zombie2);
            if (flag4 && zombie2.isMindControlled && zombie2.theZombieRow == zombie.theZombieRow && zombie2.theStatus != ZombieStatus.Flying)
            {
                zombie2.Die(0);
                return;
            }
        }
        public enum Skill
        {
            Summon,
            Sickle,
            Jump,
            SuperShoot
        }
    }

    public class Sickle : CustomParticle
    {
        public int Damage;
        public int row;
        public float cd = 0.1f;
        public LayerMask plantLayer;
        public Transform center;
        public void Awake()
        {
            center = transform.GetChild(0);
        }
        public void FixedUpdate()
        {
            cd -= Time.fixedDeltaTime;
            if(cd<= 0f)
            {
                CreateParticle.SetParticle(Plugin.DataContainer.BombCloudID, center.position, row);
                GameAPP.PlaySound(SoundType.CherryBomb);

                var hits = Physics2D.OverlapCircleAll(center.position, 1.5f, plantLayer);

                foreach (var hit in hits)
                {
                    if (hit != null && hit.TryGetComponent<Plant>(out var p))
                    {
                        p.TakeDamage(Damage, null, DamageType.Curse);
                    }
                }
                cd = 0.1f;
            }

        }
        public void Explode()
        {
        }
    }

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
    public class MyPluginInfo
    {
        public const string PluginGuid = "HorseBossPhase2.Bepinex";
        public const string PluginName = "HorseBossPhase2";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
