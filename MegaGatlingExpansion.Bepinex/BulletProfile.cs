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

    public class ExtremeFirePea : MonoBehaviour
    {
        public Bullet_firePea bullet => gameObject.GetComponent<Bullet_firePea>();

        public int Damage => bullet != null ? bullet.Damage : 0;
        public int Row => bullet != null ? bullet.theBulletRow : 0;

        public void HitZombie(Zombie zombie)
        {
            if (zombie == null || zombie.IsDestroyed())
                return;
            if (zombie.board == null)
                return;
            if (zombie.theZombieType == ZombieType.Nothing)
                return;

            var b = bullet;
            if (b == null || b.board == null)
                return;

            zombie.SetJalaed();
            zombie.JalaedExplode(true, Damage);

            Transform t = zombie.transform;
            if (t != null)
            {
                Vector3 pos = t.position;
                ParticleManager.Instance.SetParticle(ParticleType.Fire, pos, Row, true);
            }
        }
        public void HitPlant(Plant plant)
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
        }
    }

    public class ElectricPea : BaseCustomBullet
    {
        public List<PlantType> IsElectricPlant;
        private const float AURA_RADIUS = 1.5f;
        private float AttrCountDown = 0f;
        private CancellationToken token;
        public void Awake()
        {
            IsElectricPlant = ElementUpgrade
                .GetPossibleUpgradeTypes(PlantType.ElectricOnion, true)
                .ToSystemList();
            token = new();
        }
        public void OnDestroy()
        {
            token.Cancel();
        }
        public override bool HitZombieCondition(Zombie zombie)
        {
            return false;
        }
        // -----------------------------
        // OPTIMIZED AURA DAMAGE
        // -----------------------------
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (GameAPP.theGameStatus != GameStatus.InGame)
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
                        if(p.axis != null) ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,p.axis.position,lim:false);
                        p.TakeDamage(_bullet.Damage * 3, _bullet.Cast<IDamageMaker>());
                        PlantMgr.CreateLine(_bullet.transform,_bullet.transform.position,p.axis.position,Color.cyan,Color.white,_bullet.theBulletRow,token);
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
                        if(z.col != null) ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,z.col.bounds.center,lim:false);
                        z.TakeDamage(_bullet.Damage * 3, _bullet.Cast<IDamageMaker>(), DamageType.Shieldless);
                        PlantMgr.CreateLine(_bullet.transform,_bullet.transform.position,z.col.bounds.center,Color.cyan,Color.white,_bullet.theBulletRow,token);
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
                        if(z.col != null) ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,z.col.bounds.center,lim:false);
                        z.TakeDamage(_bullet.Damage * 3, _bullet.Cast<IDamageMaker>(), DamageType.Shieldless, _bullet.fromType);
                        if (PlantMgr.GetPercent(5f))
                        {
                            z.Buttered(0.5f,false);
                        }
                        PlantMgr.CreateLine(_bullet.transform,_bullet.transform.position,z.col.bounds.center,Color.cyan,Color.white,_bullet.theBulletRow,token);
                    }
                    if(col.TryGetComponent<ZombieBall>(out var ball) && !ball.plant)
                    {
                        foundTarget=true;
                        ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,ball.transform.position,lim:false);
                        ball.bulletTime ++;
                        if(ball.bulletTime >= 31) ball.Die();
                        PlantMgr.CreateLine(_bullet.transform,_bullet.transform.position,ball.transform.position,Color.cyan,Color.white,_bullet.theBulletRow,token);
                    }
                    if(col.TryGetComponent<FreezedPlant>(out var freezed))
                    {
                        foundTarget=true;
                        ParticleManager.Instance.SetParticle(ParticleType.ElectricSplat,freezed.transform.position,lim:false);
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
                        PlantMgr.CreateLine(_bullet.transform,_bullet.transform.position,freezed.transform.position,Color.cyan,Color.white,_bullet.theBulletRow,token);
                    }
                }
            }
            //Too loud
            if(foundTarget)
            {
                //GameAPP.PlaySound(SoundType.Laser);
            }
        }
    }

    public class FlameGooPea : GooPea
    {
        public override void HitZombie(Zombie zombie)
        {
            if (zombie == null || zombie.IsDestroyed())
                return;
            if (zombie.board == null)
                return;
            if (zombie.theZombieType == ZombieType.Nothing)
                return;
            Miasma.SetMiasma(bullet.transform.position, bullet.theBulletRow, zombie.board, false, true);
            base.HitZombie(zombie);
        }
    }

    public class GooPea : MonoBehaviour
    {
        public Bullet bullet => gameObject.GetComponent<Bullet>();
        public virtual void HitZombie(Zombie zombie)
        {
            if (zombie == null || zombie.IsDestroyed())
                return;
            if (zombie.board == null)
                return;
            if (zombie.theZombieType == ZombieType.Nothing)
                return;

            var b = bullet;
            if (b == null || b.board == null || b.board.boardAction == null)
                return;

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
        }
        public void HitPlant(Plant plant)
        {
            if (plant != null)
            {
                plant.TakeDamage(bullet.Damage,bullet);
                plant.FlashOnce();
                GameAPP.PlaySound((SoundType)Random.RandomRangeInt(0, 3));
                bullet.Die();
            }
        }
    }

    public class PrimalPea : MonoBehaviour
    {
        public Bullet bullet => gameObject.GetComponent<Bullet>();

        public int Damage => bullet != null ? bullet.Damage : 0;
        private bool CanAffect(Zombie z) =>
        !TypeMgr.IsBossZombie(z.theZombieType) && !TypeMgr.BigZombie(z.theZombieType) || Plugin.IsOverpowered;

        public void HitZombie(Zombie zombie)
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

                if (col >= 9 && func(z))
                {
                    z.FlyAway();
                }
            }
        }
        public void HitPlant(Plant plant)
        {
            if (plant != null)
            {
                plant.TakeDamage(bullet.Damage,bullet,DamageType.Normal);
                plant.FlashOnce();
                ParticleManager.Instance.SetParticle(0, bullet.transform.position, plant.thePlantRow);
                GameAPP.PlaySound((SoundType)Random.RandomRangeInt(0, 3));
                bullet.Die();
            }
        }
    }

    public class HypnoMegaPea : MonoBehaviour
    {
        public Bullet bullet => gameObject.GetComponent<Bullet>();
        public bool fromHypnoZombie = false;
        public void HitPlant(Plant plant)
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
        }
        public class RemovedEffect : CustomEffect
        {
            public override CustomEffectUsage Usage => CustomEffectUsage.Plant;
        }
    }
}
