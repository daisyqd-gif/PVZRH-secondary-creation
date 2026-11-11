using System;
using System.Collections.Generic;
using System.Linq;
using CustomPlantClass.CustomizeLib;
using CustomPlantClass;
using CustomPlantClass.Main;
using CustomPlantClass.Runtime.Core;
using GameLevel.RogueShooting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HorseBossPhase2
{
    public class UltimateHorsePhase2 : ZombieComponent
    {
        Transform SicklePos;
        Transform Shoot2Pos;
        Transform QiPos;
        float cd = 0.1f;
        int superremain = 0;
        bool super = false;
        bool stopshoot = false;
        bool jumping = false;
        Vector2 savedPos;
        int cdcount = 0;
        Skill prev_skill = Skill.Nothing;
        
        static ZombieType[] spawnZombieTypes =>
            Board.Instance != null && Board.Instance.boardTag.isSuperRandom ?
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
            ] : [
                ZombieType.BlackFootball_a,
                ZombieType.RandomZombie,
                ZombieType.RandomPlusZombie,
                ZombieType.RandomGargantuar,
                ZombieType.ChickenImp,
                ZombieType.HorseZombie,
                ZombieType.YellowFootball,
                ZombieType.FlagFootball,
                ZombieType.WhiteFootball,
                ZombieType.GatlingFootballZombie,
                ZombieType.TallNutFootballZombie,
                ZombieType.ElephantZombie,
                ZombieType.CoachPaper,
                ZombieType.ElitePaperZombie
            ];
        Dictionary<Skill, Action<Zombie, UltimateHorsePhase2>> ActionOnSkill = new()
        {
            [Skill.Summon] = (zombie, h) =>
            {
                zombie.anim.SetTriggerString("summon");
                h.cdcount = 3;
                h.prev_skill = Skill.Nothing;
            },
            [Skill.Nothing] = (zombie, h) =>
            {
                h.prev_skill = Skill.Nothing;
            },
            [Skill.Sickle] = (zombie, h) =>
            {
                zombie.anim.SetTriggerString("attack");
                h.cdcount = 3;
                h.prev_skill = Skill.Nothing;
            },
            [Skill.Smash] = (zombie, h) =>
            {
                zombie.anim.SetTriggerString("smash");
                h.prev_skill = Skill.Smash;
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
                h.cdcount = 0;
                h.prev_skill = Skill.Jump;
            },
            [Skill.SuperShoot] = (zombie, h) =>
            {
                zombie.anim.SetBoolString("shooting", true);
                zombie.ChangeStatus(ZombieStatus.Raised);
                h.superremain = 180;
                h.super = true;
                h.cdcount = 0;
                h.prev_skill = Skill.SuperShoot;
            }
        };
        Skill randomSkill => new[] { Skill.Summon, Skill.Summon, Skill.Sickle, Skill.SuperShoot, Skill.Jump, Skill.Smash, Skill.Nothing, Skill.Nothing }.Where((s) => s != prev_skill).GetRandomItem();
        public void Awake()
        {
            SicklePos = transform.FindChild("Sickle");
            zombie.shoot = transform.FindChild("Shoot");
            Shoot2Pos = transform.FindChild("Shoot2");
            QiPos = transform.FindChild("Qi");
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
        public void Qi()
        {
            for( float i = 0f ; i <= 360f ; i += 15f )
            {
                var qi = GameObjectMgr.Instantiate(Plugin.DataContainer.QiID, QiPos.position, Quaternion.identity, zombie.board.transform).GetComponent<SwordQi>();
                qi.dmg = zombie.theAttackDamage / 10;
                qi.free=true;
                qi.transform.Rotate(0,0,i);
            }
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
                zombie.ChangeStatus(ZombieStatus.Raised);
            }
        }
        public void FixedUpdate()
        {
            if(zombie == null) return;
            if (cd <= 0.0f && zombie.Alive && !stopshoot)
            {
                zombie.anim.SetTrigger("shoot");
                float num = zombie.theSpeed;
                if (0 <= num)
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
        public void IntoRaised()
        {
            zombie.ChangeStatus(ZombieStatus.Raised);
        }
        public void DoSkill()
        {
            if(zombie == null) return;
            if (cdcount > 0)
            {
                cdcount --;
                return;
            }
            ActionOnSkill[randomSkill](zombie, this);
        }
        public void ReShoot()
        {
            stopshoot = false;
            zombie.ChangeStatus(ZombieStatus.Default);
        }
        public void AnimShoot()
        {
            if (zombie.shoot != null)
            {
                if (super)
                {
                    Vector3 position1 = Shoot2Pos.position;
                    for (int i = 0; i < 5; i++)
                    {
                        Bullet bullet1 = CreateBullet.Instance.SetBullet(
                            position1.x + Random.Range(-0.1f, 0.1f),
                            position1.y + Random.Range(-0.1f, 0.1f),
                            zombie.theZombieRow,
                            BulletType.Bullet_ironPea_curse,
                            BulletMoveWay.SuperGatling,
                            !zombie.isMindControlled);
                        bullet1.Damage = zombie.theAttackDamage / 100;
                        bullet1.transform.Rotate(0, 0, zombie.isMindControlled ? 0 : 180 + Random.Range(-15f, 15f));
                        bullet1.shootByZombie = true;
                        Bullet bullet2 = CreateBullet.Instance.SetBullet(
                            position1.x + 0.5f + Random.Range(-0.1f, 0.1f),
                            position1.y + 0.5f + Random.Range(-0.1f, 0.1f),
                            zombie.theZombieRow,
                            BulletType.Bullet_ironPea_curse,
                            BulletMoveWay.SuperGatling,
                            !zombie.isMindControlled);
                        bullet2.Damage = zombie.theAttackDamage / 100;
                        bullet2.transform.Rotate(0, 0, zombie.isMindControlled ? 0 : 180 + Random.Range(-15f, 15f));
                        bullet2.shootByZombie = true;
                        Bullet bullet3 = CreateBullet.Instance.SetBullet(
                            position1.x + 1f + Random.Range(-0.1f, 0.1f),
                            position1.y + 1f + Random.Range(-0.1f, 0.1f),
                            zombie.theZombieRow,
                            BulletType.Bullet_ironPea_curse,
                            BulletMoveWay.SuperGatling,
                            !zombie.isMindControlled);
                        bullet3.Damage = zombie.theAttackDamage / 100;
                        bullet3.transform.Rotate(0, 0, zombie.isMindControlled ? 0 : 180 + Random.Range(-15f, 15f));
                        bullet3.shootByZombie = true;
                        GameAPP.PlaySound(Random.Range(3, 5), 0.5f, 1f);
                        superremain--;
                    }
                    if (superremain <= 5)
                    {
                        SwitchRow();
                    }
                    if (superremain <= 0)
                    {
                        zombie.anim.SetBoolString("shooting", false);
                        super = false;
                        zombie.ChangeStatus(ZombieStatus.Default);
                    }
                }
                Vector3 position = zombie.shoot.position;
                Bullet bullet = CreateBullet.Instance.SetBullet(
                    position.x, position.y, zombie.theZombieRow,
                    BulletType.Bullet_ironPea_curse,
                    BulletMoveWay.Left, !zombie.isMindControlled);
                bullet.Damage = zombie.theAttackDamage / 100;
                bullet.shootByZombie = true;
                GameAPP.PlaySound(Random.Range(3, 5), 0.5f, 1f);
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
        enum Skill
        {
            Summon,
            Sickle,
            Jump,
            SuperShoot,
            Smash,
            Nothing
        }
    }
}