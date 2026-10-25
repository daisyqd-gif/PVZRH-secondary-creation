using CustomPlantClass.Runtime.Tasks;
using Cysharp.Threading.Tasks;

namespace MegaGatlingExpansion
{
    public readonly struct BulletProfile
    {
        public readonly int Value;

        public BulletProfile(int value)
        {
            Value = value;
        }

        public static implicit operator BulletProfile(int value)
            => new BulletProfile(value);

        public static implicit operator int(BulletProfile type)
            => type.Value;

        public static implicit operator BulletType(BulletProfile type)
            => (BulletType)type.Value;

        public static implicit operator BulletProfile(BulletType type)
            => new BulletProfile((int)type);

        public override int GetHashCode() => Value;

        public override bool Equals(object obj)
            => obj is BulletProfile other && other.Value == Value;

        public static readonly BulletProfile ExtremeFirePea = 3082;
        public static readonly BulletProfile PrimalPea = 3083;
        public static readonly BulletProfile GooPea = 3084;
        public static readonly BulletProfile ElectricPea = 3085;
        public static readonly BulletProfile HypnoMegaPea = 3086;
        public static readonly BulletProfile HypnoMegaPeaZ = 3087;
        public static readonly BulletProfile FlameGooPea = 3088;
    }

    public class ExtremeFirePea : BaseCustomBullet
    {
        public Bullet_firePea bullet => gameObject.GetComponent<Bullet_firePea>();

        public int Damage => bullet != null ? bullet.Damage : 0;
        public int Row => bullet != null ? bullet.theBulletRow : 0;

        public override bool HitZombie(Zombie zombie)
        {
            if (zombie == null || zombie.IsDestroyed())
                return true;
            if (zombie.board == null)
                return true;
            if (zombie.theZombieType == ZombieType.Nothing)
                return true;

            var b = bullet;
            if (b == null || b.board == null)
                return true;

            zombie.SetJalaed();
            zombie.JalaedExplode(true, Damage);

            Transform t = zombie.transform;
            if (t != null)
            {
                Vector3 pos = t.position;
                ParticleManager.Instance.SetParticle(ParticleType.Fire, pos, Row, true);
            }
            return false;
        }
        public override bool HitPlant(Plant plant)
        {
            if (plant != null)
            {
                int dmg = bullet.Damage;

                // If the plant is an ice‑type plant, deal double damage
                if (TypeMgr.IsIcePlant(plant.thePlantType))
                {
                    dmg *= 2;
                }
                else if (TypeMgr.IsFirePlant(plant.thePlantType))
                {
                    dmg = Mathf.RoundToInt(dmg / 2f);
                }

                plant.TakeDamage(dmg,bullet);
                plant.FlashOnce();
                bullet.Die();
            }
            return false;
        }
    }

    public class ElectricPea : BaseCustomBullet
    {
        public List<PlantType> IsElectricPlant;
        private const float AURA_RADIUS = 1.5f;
        private float AttrCountDown = 0f;
        private CancellationToken token;
        private static int counter = 0;
        public bool stopSpawn = false;
        public bool isPF = false;
        public void Awake()
        {
            IsElectricPlant = ElementUpgrade
                .GetPossibleUpgradeTypes(PlantType.ElectricOnion, true)
                .ToSystemList();
            token = new();
            counter++;
            stopSpawn = false;
            AttrCountDown = 0f;
        }
        public override void Start()
        {
            if (!isPF)
            {
                _bullet.col.enabled = false;
            }
            else
            {
                _bullet.col.enabled = true;
            }
            base.Start();
        }
        public void OnDestroy()
        {
            token.Cancel();
            counter--;
        }
        public override bool HitZombieCondition(Zombie zombie)
        {
            if (isPF)
            {
                return true;
            }
            return false;
        }
        public override bool HitZombie(Zombie zombie)
        {
            if (!isPF || stopSpawn)
            {
                return false;
            }
            for(int i = 0; i<360; i += 60)
            {
                Bullet b = CreateBullet.Instance.SetBullet(transform.position.x,transform.position.y,_bullet.theBulletRow,BulletProfile.ElectricPea,BulletMoveWay.Free);
                b.Damage = _bullet.Damage;
                b.transform.Rotate(0,0, i);
                b.fromType = _bullet.fromType;
                b.normalSpeed /= 2;
                b.GetComponent<ElectricPea>().isPF = false;
            }
            ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,_bullet.col.bounds.center).transform.localScale *= 4;
            stopSpawn=true;
            return true;
        }
        // -----------------------------
        // OPTIMIZED AURA DAMAGE
        // -----------------------------
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (GameAPP.theGameStatus != GameStatus.InGame || isPF)
                return;

            // Aura tick timer
            AttrCountDown -= Time.deltaTime;
            if (AttrCountDown > 0f && !Plugin.IsOverpowered)
                return;

            AttrCountDown = Random.Range(0.25f, 0.5f);

            Vector3 pos = transform.position;

            // -----------------------------
            // OverlapCircleAll (fast + safe)
            // -----------------------------
            var hits = Physics2D.OverlapCircleAll(
                new Vector2(pos.x, pos.y),
                AURA_RADIUS
            );
            bool foundTarget = false;
            foreach (var col in hits)
            {
                if (col == null || col.IsDestroyed())
                    continue;

                // -----------------------------
                // AI bullet → damage plants + Hypno zombies
                // -----------------------------
                if (_bullet.Team==Team.AI)
                {
                    // Plants
                    if (col.TryGetComponent<Plant>(out var p) &&
                        p != null &&
                        !p.IsDestroyed() &&
                        p.board != null &&
                        p.thePlantType != PlantType.Nothing &&
                        !IsElectricPlant.Contains(p.thePlantType))
                    {
                        foundTarget=true;
                        if(p.axis != null && counter < 50) ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,p.axis.position,lim:false);
                        p.TakeDamage(_bullet.Damage * 3, _bullet.Cast<IDamageMaker>());
                        if(counter < 50) PlantMgr.CreateLine(_bullet.transform,_bullet.transform.position,p.axis.position,Color.cyan,Color.white,_bullet.theBulletRow,token);
                        continue;
                    }

                    // Mind‑controlled zombies
                    if (col.TryGetComponent<Zombie>(out var z) &&
                        z != null &&
                        !z.IsDestroyed() &&
                        z.board != null &&
                        z.theZombieType != ZombieType.Nothing &&
                        z.theZombieType != (ZombieType)9002 &&
                        z.isMindControlled)
                    {
                        foundTarget=true;
                        if(z.col != null && counter < 50) ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,z.col.bounds.center,lim:false);
                        z.TakeDamage(_bullet.Damage * 3, _bullet.Cast<IDamageMaker>(), DamageType.Shieldless);
                        if(counter < 50) PlantMgr.CreateLine(_bullet.transform,_bullet.transform.position,z.col.bounds.center,Color.cyan,Color.white,_bullet.theBulletRow,token);
                        continue;
                    }
                }
                // -----------------------------
                // Player bullet → damage normal zombies
                // -----------------------------
                else
                {
                    if (col.TryGetComponent<Zombie>(out var z) &&
                        z != null &&
                        !z.IsDestroyed() &&
                        z.board != null &&
                        z.theZombieType != ZombieType.Nothing &&
                        z.theZombieType != (ZombieType)9002 &&
                        !z.isMindControlled)
                    {
                        foundTarget=true;
                        if(z.col != null && counter < 50) ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,z.col.bounds.center,lim:false);
                        z.TakeDamage(_bullet.Damage * 3, _bullet.Cast<IDamageMaker>(), DamageType.Shieldless, _bullet.fromType);
                        if (PlantMgr.GetPercent(5f))
                        {
                            z.Buttered(0.5f,false);
                        }
                        if(counter < 50) PlantMgr.CreateLine(_bullet.transform,_bullet.transform.position,z.col.bounds.center,Color.cyan,Color.white,_bullet.theBulletRow,token);
                    }
                    if(col.TryGetComponent<ZombieBall>(out var ball) && !ball.plant)
                    {
                        foundTarget=true;
                        if(counter < 50) ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,ball.transform.position,lim:false);
                        ball.bulletTime ++;
                        if(ball.bulletTime >= 31) ball.Die();
                        if(counter < 50) PlantMgr.CreateLine(_bullet.transform,_bullet.transform.position,ball.transform.position,Color.cyan,Color.white,_bullet.theBulletRow,token);
                    }
                    if(col.TryGetComponent<FreezedPlant>(out var freezed))
                    {
                        foundTarget=true;
                        if(counter < 50) ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,freezed.transform.position,lim:false);
                        if (freezed.fired)
                        {
                            freezed.lightCountDown = 0.2f;
                            freezed.fireCount --;
                            if(freezed.fireCount <1) freezed.Die();
                        }
                        else
                        {
                            freezed.TakeDamage(_bullet.Damage * 3, _bullet.Cast<IDamageMaker>(), DamageType.Shieldless, _bullet.fromType);
                        }
                        if(counter < 50) PlantMgr.CreateLine(_bullet.transform,_bullet.transform.position,freezed.transform.position,Color.cyan,Color.white,_bullet.theBulletRow,token);
                    }
                }
            }
            if(foundTarget && counter < 50)
            {
                GameAPP.PlaySound(SoundType.ThunderLightning);
            }
        }
    }

    public class FlameGooPea : GooPea
    {
        public override bool HitZombie(Zombie zombie)
        {
            if (zombie == null || zombie.IsDestroyed())
                return true;
            if (zombie.board == null)
                return true;
            if (zombie.theZombieType == ZombieType.Nothing)
                return true;
            Miasma.SetMiasma(bullet.transform.position, bullet.theBulletRow, zombie.board, Plugin.IsOverpowered, true);
            return base.HitZombie(zombie);
        }
    }

    public class GooPea : BaseCustomBullet
    {
        public Bullet bullet => gameObject.GetComponent<Bullet>();
        public override bool HitZombie(Zombie zombie)
        {
            if (zombie == null || zombie.IsDestroyed())
                return true;
            if (zombie.board == null)
                return true;
            if (zombie.theZombieType == ZombieType.Nothing)
                return true;

            var b = bullet;
            if (b == null || b.board == null || b.board.boardAction == null)
                return true;

            zombie.SetPoison(1);

            if (zombie.poisonLevel >= 5)
            {
                Action<Zombie> action = z =>
                {
                    if (z == null || z.IsDestroyed()) return;
                    if (z.board == null) return;
                    if (z.theZombieType == ZombieType.Nothing) return;

                    z.SetPoison(1);
                };

                b.board.boardAction.CreateCherryExplode(
                    b.transform.position,
                    b.theBulletRow,
                    CherryBombType.Bullet,
                    b.Damage * 10 + zombie.poisonLevel * 10,
                    b.fromType,
                    action,
                    true
                );
                if(!Plugin.IsOverpowered)zombie.poisonLevel=0;
            }
            return false;
        }
        public override bool HitPlant(Plant plant)
        {
            if (plant != null)
            {
                plant.TakeDamage(bullet.Damage,bullet);
                plant.FlashOnce();
                GameAPP.PlaySound((SoundType)Random.RandomRangeInt(0, 3));
                bullet.Die();
            }
            return false;
        }
    }

    public class PrimalPea : BaseCustomBullet
    {
        public Bullet bullet => gameObject.GetComponent<Bullet>();

        public int Damage => bullet != null ? bullet.Damage : 0;
        private bool CanAffect(Zombie z) =>
        !TypeMgr.IsBossZombie(z.theZombieType) && !TypeMgr.BigZombie(z.theZombieType) || Plugin.IsOverpowered;

        public override bool HitZombie(Zombie zombie)
        {
            var zlist = Lawnf.GetAllZombies();
            int targetCol = Mouse.Instance.GetColumnFromX(bullet.transform.position.x);
            int row = bullet.theBulletRow;
            foreach (Zombie z in zlist)
            {
                if (z == null || z.IsDestroyed()) continue;
                if (z.board == null) continue;
                if (z.theZombieType == ZombieType.Nothing) continue;

                if (z.theZombieRow != row) continue;

                Func<Zombie,bool> func = CanAffect;

                int col = Mouse.Instance.GetColumnFromX(z.transform.position.x);
                if (col != targetCol) continue;

                if (Random.Range(0, 100) > 50 && !func(z)) z.KnockBack(2f, Zombie.KnockBackReason.Normal);
                else z.Buttered(2, false); //2 seconds without sprite

                if (func(z))
                {
                    z.FlyAway();
                }
            }
            return false;
        }
        public override bool HitPlant(Plant plant)
        {
            if (plant != null)
            {
                plant.TakeDamage(bullet.Damage,bullet,DamageType.Normal);
                plant.FlashOnce();
                ParticleManager.Instance.SetParticle(0, bullet.transform.position, plant.thePlantRow);
                GameAPP.PlaySound((SoundType)Random.RandomRangeInt(0, 3));
                bullet.Die();
            }
            return false;
        }
    }

    public class HypnoMegaPea : BaseCustomBullet
    {
        public Bullet bullet => gameObject.GetComponent<Bullet>();
        public bool fromHypnoZombie = false;
        public override bool HitPlant(Plant plant)
        {
            if (plant != null)
            {
                plant.TakeDamage(bullet.Damage,bullet);
                if (plant.thePlantHealth <= 0 && !plant.TryGetCustomEffect<RemovedEffect>(out _))
                {
                    CreateZombie.Instance.SetZombie(plant.thePlantRow, (ZombieType)9000, Mouse.Instance.GetBoxXFromColumn(plant.thePlantColumn));
                    plant.AddEffect<RemovedEffect>();
                    ParticleManager.Instance.SetParticle(ParticleType.HypnoEmperorSkinCloud, bullet.transform.position, plant.thePlantRow);
                }
                else
                {
                    if(plant.TryGetCustomEffect<RemovedEffect>(out var effect))
                    {
                        Destroy(effect);
                    }
                }
                plant.FlashOnce();
                ParticleManager.Instance.SetParticle(ParticleType.RandomCloud, bullet.transform.position, plant.thePlantRow);
                GameAPP.PlaySound((SoundType)Random.RandomRangeInt(0, 3));
                bullet.Die();
            }
            return false;
        }
        public override bool HitZombie(Zombie zombie)
        {
            if (zombie != null)
            {
                zombie.TakeDamage(bullet.Damage,bullet,DamageType.Normal);
                if (zombie.theHealth <= 0 && !zombie.TryGetCustomEffect<RemovedZEffect>(out _))
                {
                    CreateZombie.Instance.SetZombieWithMindControl(zombie.theZombieRow, (ZombieType)9000, zombie.Column);
                    zombie.AddEffect<RemovedZEffect>();
                    ParticleManager.Instance.SetParticle(ParticleType.HypnoEmperorSkinCloud, bullet.transform.position, zombie.theZombieRow);
                }
                else
                {
                    if(zombie.TryGetCustomEffect<RemovedZEffect>(out var effect))
                    {
                        Destroy(effect);
                    }
                }
                ParticleManager.Instance.SetParticle(ParticleType.RandomCloud, bullet.transform.position, zombie.theZombieRow);
                GameAPP.PlaySound(Random.RandomRangeInt(0, 3));
                bullet.Die();
            }
            return false;
        }
        public class RemovedEffect : CustomEffect
        {
            public override CustomEffectUsage Usage => CustomEffectUsage.Plant;
        }
        public class RemovedZEffect : CustomEffect
        {
            public override CustomEffectUsage Usage => CustomEffectUsage.Zombie;
        }
    }
}
