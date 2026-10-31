using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomizeLib.BepInEx;
using CustomizeLib.BepInEx.ExtensionData.Basic;
using CustomPlantClass.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace FreezingCherry.BepInEx
{
    public class FreezingCherry : MonoBehaviour
    {
        //public static ID PlantID = 1957;
        public static ID ParticleID = 1957;
        public static BuffID buff1 = -1; // 凝冰之力
        public static BuffID buff2 = -1; // 霜灭

        public static GameObject? IceDoomPrefab = null;
        public static int maxCard = 8;

        public IceCherry plant => gameObject.GetComponent<IceCherry>();
        public int spawnTime = 0;
        public List<DroppedCard> cards = new();

        public virtual bool AddEnergy(Collider2D collision)
        {
            if (plant == null) return false;
            if (collision == null || !collision.TryGetComponent<Bullet>(out var bullet) || bullet == null)
                return false;
            if (bullet.Team == Team.AI)
                return false;
            int energy = 0;

            switch (bullet.theBulletType)
            {
                case BulletType.Bullet_cherry:
                    energy = 1;
                    break;
                case BulletType.Bullet_fireCherry:
                case BulletType.Bullet_pea_doom:
                case BulletType.Bullet_pea_doom_fire:
                case BulletType.Bullet_pea_threeCherry:
                case BulletType.Bullet_pea_bombCherry:
                    energy = 2;
                    break;
                case BulletType.Bullet_superCherry:
                case BulletType.Bullet_cherrySquash:
                    energy = 10;
                    break;
                case BulletType.Bullet_doom:
                case BulletType.Bullet_doom_ulti:
                    if (bullet.theStatus == BulletStatus.Doom_big)
                        energy = 20;
                    else
                        energy = 10;
                    break;
                case BulletType.Bullet_doom_big:
                case BulletType.Bullet_doom_big_ulti:
                    energy = 20;
                    break;
                case BulletType.Bullet_doom_fire:
                    if (bullet.theStatus == BulletStatus.Doom_big)
                        energy = 20;
                    else
                        energy = 10;
                    break;
                default:
                    return false;
            }
            CreateParticle.SetParticle((int)ParticleType.SnowPeaSplat, new Vector2(bullet.transform.position.x, bullet.transform.position.y), bullet.theBulletRow);
            if (Lawnf.TravelAdvanced(buff1))
                energy *= 2;
            plant.attributeCount += energy;
            GameAPP.PlaySound(68, 0.5f, 1.0f);
            if (plant.attributeCount >= 100)
            {
                if (plant.board.boardTag.rogueShooting)
                {
                    bullet.Die();
                    plant.attributeCount -= 100;
                    return true;
                }
                cards = cards.Where(card => card != null).OrderByDescending(card => card.existTime).ToList();
                var cherry = Lawnf.SetDroppedCard(plant.axis.transform.position + new Vector3(0f, 1f, 0f), PlantType.CherryBomb);
                cherry.SetData("FreezingCherry_CreateByCherry", true);
                cherry.SetData("FreezingCherry_FromType", plant.thePlantType);
                if (cards.Count >= maxCard)
                    cherry.existTime = Lawnf.TravelAdvanced(buff2) ? 7.5f : 15f;
                else
                    cards.Add(cherry);
                spawnTime++;
                if (spawnTime >= 3)
                {
                    var doom = Lawnf.SetDroppedCard(plant.axis.transform.position + new Vector3(0f, 1f, 0f), PlantType.DoomShroom);
                    doom.SetData("FreezingCherry_CreateByCherry", true);
                    doom.SetData("FreezingCherry_FromType", plant.thePlantType);
                    if (cards.Count(card => card.thePlantType == PlantType.DoomShroom) < 2)
                    {
                        var list = cards.OrderByDescending(card => card.existTime).ToList();
                        if (list.Count > 0)
                        {
                            var cherryCard = list.First(card => card.thePlantType == PlantType.CherryBomb);
                            if (cherryCard != null)
                            {
                                cards[cards.IndexOf(cherryCard)] = doom;
                                cards.Add(cherryCard);
                            }
                            else
                                cards.Add(doom);
                        }
                        else
                            cards.Add(doom);
                    }
                    else
                        doom.existTime = Lawnf.TravelAdvanced(buff2) ? 7.5f : 15f;
                    spawnTime = 0;
                }
                if (cards.Count > maxCard)
                {
                    for (int i = maxCard; i < cards.Count; i++)
                    {
                        cards[i].existTime = Lawnf.TravelAdvanced(buff2) ? 7.5f : 15f;
                        cards.RemoveAt(i);
                    }
                }
                bullet.Die();
                plant.attributeCount -= 100;
                return true;
            }
            bullet.Die();
            plant.UpdateText();
            return false;
        }
    }

    public class UltimateFreezingCherry : FreezingCherry
    {
        public static ID PlantID = 1958;
        public static ID BulletID = 1958;
        public static new ID ParticleID = 1958;
        public static ID DoomParticle = 1959;
        public static ID IceDoomSprite = 1960;
        public static BuffID buff = -1;
        public static GameObject? IceRadiation = null;
        public static ID CherryID = 1958;
        public static ID CherrySmallID = 1959;

        public void Awake()
        {
            plant.shoot = transform.FindChild("Body/Shoot");
        }

        public void Start()
        {
            if (plant == null) return;
            plant.gameObject.layer = LayerMask.NameToLayer("TorchWood");
        }

        public void Update()
        {
            if (plant == null) return;
            plant.attributeFloat += Time.deltaTime;
        }

        public void SuperStart()
        {
            plant.anim.SetInteger("cureentStatus", 1);
            plant.invincible = true;
            plant.GetComponent<SortingGroup>().sortingLayerName = "plant11";
        }

        public void ChangeState()
        {
            plant.anim.SetInteger("cureentStatus", 2);
            plant.board.boardAction.SetDoom(0, 0, false, true, plant.axis.transform.position, 18000, existParticle: false, fromType: plant.thePlantType);
            CreateParticle.SetParticle(IceDoomSprite, plant.axis.transform.position, 11);
            plant.StartCoroutine(CreateBomb());
        }

        public void SuperEnd()
        {
            CreateParticle.SetParticle(DoomParticle, plant.axis.transform.position, 11);
            foreach (var zombie in Lawnf.GetAllZombies())
            {
                if (zombie == null) continue;
                if (zombie.isMindControlled) continue;
                zombie.SetFreeze(10f);
                zombie.SetCold(15f);
                zombie.TakeDamage(DmgType.IceAll, plant.attackDamage * 150, plant.thePlantType);
            }
            Vector3 position = plant.axis.position;
            for (int angle = 0; angle < 360; angle += 5)
            {
                // 创建子弹
                Bullet bullet = CreateBullet.Instance.SetBullet(position.x, position.y, plant.thePlantRow, BulletID, BulletMoveWay.Free, false);
                if (bullet != null)
                {
                    bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
                    bullet.Damage = plant.attackDamage * 150;
                    bullet.fromType = UltimateFreezingCherry.PlantID;
                }
            }
            Instantiate(IceRadiation, plant.axis.transform.position, Quaternion.identity, plant.board.transform);
            plant.board.iceDoomFreezeTime += 10f;
            plant.board.StartCoroutine(CreateBomb());
            plant.Die(Plant.DieReason.BySelf);
        }

        public override bool AddEnergy(Collider2D collision)
        {
            if (base.AddEnergy(collision))
                StartShoot();
            return true;
        }

        public void Shoot()
        {
            if (plant == null) return;
            if (plant.board == null) return;
            Vector2 shoot = plant.shoot.position;
            var zombies = Lawnf.GetAllZombies().ToSystemList().Where(z => z != null && !z.isMindControlled).ToList();
            float[] projectileParams = new float[4];
            Zombie? zombie = null;
            Vector2 position = default;
            if (zombies.Count > 0)
            {
                zombie = zombies[UnityEngine.Random.Range(0, zombies.Count)];
                projectileParams = Core.Lawnf.CalculateProjectileWithSpeed(shoot, zombie.Velocity, zombie.ColliderPosition, 1.5f);
            }
            else
            {
                return;
            }
            if (projectileParams == null || projectileParams.Length <= 1)
                return;
            Bullet? bullet = null;
            if (zombie != null)
            {
                bullet = CreateBullet.Instance.SetBullet(shoot.x, shoot.y, zombie.theZombieRow, BulletID, BulletMoveWay.Throw);
                bullet.ThrowTo(zombie, new(), 1.5f.GetNullable());
            }
            else
            {
                bullet = CreateBullet.Instance.SetBullet(shoot.x, shoot.y, Mouse.Instance.GetRowFromY(position.x, position.y), BulletID, BulletMoveWay.Throw);
                bullet.ThrowTo(position, new(), 1.5f.GetNullable());
            }
            bullet.Damage = plant.attackDamage;
            bullet.fromType = plant.thePlantType;
        }

        public async void StartShoot()
        {
            try
            {
                for (int i = 1; i <= 3+plant.shootingLevel; i++)
                {
                    Shoot();
                    await DelayTask.Delay(0.08f);
                }
            }
            catch
            {
                return;
            }
        }
        public IEnumerator CreateBomb()
        {
            if (Board.Instance == null)
                yield break;

            int bombCount = 0;
            int maxBombs = 30 * 3; // 总共90个炸弹

            while (bombCount < maxBombs)
            {
                for (int j = 0; j < 3; j++)
                {
                    try
                    {
                        SetBomb();
                        bombCount++;
                    }
                    catch (Exception)
                    {
                        yield break;
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void SetBomb()
        {
            try
            {
                Action<Zombie> action = (z) =>
                {
                    z.SetFreeze(10f);
                    z.SetCold(15f);
                    int damage = 7200;
                    if (z.GetAttrTimers().freezeTimer > 0f)
                        damage *= 4;
                    z.TakeDamage(DmgType.NormalAll, damage, PlantID);
                };
                int column = UnityEngine.Random.Range(0, Board.Instance.columnNum);
                int row = UnityEngine.Random.Range(0, Board.Instance.rowNum);
                var position = new Vector2(Mouse.Instance.GetBoxXFromColumn(column), Mouse.Instance.GetBoxYFromRow(row));
                Board.Instance.boardAction.CreateCherryExplode(position, row, CherryID, 7200, PlantID, action);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}