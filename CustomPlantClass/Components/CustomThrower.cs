using CustomPlantClass.Runtime.Core;

namespace CustomPlantClass
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CustomThrower : CustomShooter
    {
        protected Vector2 firstPostion;
        protected float firstTime;
        protected virtual float flightTime { get; set; } = 1.5f;
        protected virtual bool CheckRange(Zombie zombie)
        {
            if (zombie != null)
            {
                Vector3 position = zombie.axis.position;
                Vector3 position2 = _plant.axis.position;
                return !MathHelper.ApproximatelyZero(position.x - position2.x) && !(position.x < position2.x);
            }
            return false;
        }
        protected virtual Zombie ThrowerSearchZombie()
        {
            var board = _plant.board;
            if (board == null || board.zombieArray == null)
                return null;

            Zombie best = null;
            float bestDist = float.MaxValue;

            foreach (var z in board.zombieArray)
            {
                if (z == null) continue;
                if (z.theZombieRow != _plant.thePlantRow) continue;
                if (z.axis == null) continue;

                var zPos = z.axis.position;
                float vision = _plant.vision;

                // must be within vision range
                if (!(zPos.x < vision)) continue;

                // your virtual filter
                if (!CheckRange(z)) continue;

                // extra static filter (umbrella / special cases)
                if (!Thrower.ThrowSearchZombie(z)) continue;

                // pick closest in front
                var plantPos = _plant.axis.position;
                float dist = Mathf.Abs(zPos.x - plantPos.x);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = z;
                }
            }

            _plant.targetZombie = best;

            // fallback: boss
            if (best == null)
            {
                var bossObj = _plant.SearchBoss(); // virtual on plant
                if (bossObj != null)
                {
                    best = bossObj.GetComponent<Zombie>();
                    _plant.targetZombie = best;
                }
            }

            return best;
        }
        public override void PlantShootUpdate()
        {
            _plant.UpdateAttackCountDown();
            if (_plant.thePlantAttackCountDown > 0) return;
            _plant.thePlantAttackCountDown = _plant.thePlantAttackInterval * Random.Range(0.95f, 1.05f);
            Zombie zombie = ThrowerSearchZombie();
            if (zombie == null || zombie.col == null)
                return;
            Vector2 targetPos = GetZombiePosition(zombie);
            firstPostion = targetPos;
            _plant.anim.SetTriggerString("shoot");
            firstTime = Time.time;
        }
        protected virtual void ShootZombie(Zombie zombie)
        {
            // must have a shoot point
            if (_plant.shoot == null || zombie == null)
                return;

            // spawn bullet at shoot point
            Bullet b = PlantMgr.SetBullet(
                _plant,
                GetBulletType(),
                BulletMoveWay.Throw,
                _plant.attackDamage * 2
            );

            if (b == null)
                return;

            b.ThrowTo(zombie);
            b.melonSputter = _plant.melonSputter;

            // hook for subclasses (Winter, Lava, Gold, etc.)
            UniqueEffect(b);
        }
        protected virtual bool IsUmbrellaValid(Plant p)
        {
            return IsUmbrella(p) && p.axis.position.x < firstPostion.x;
        }
        // v = projectile start position (shoot point)
        // returns (arcParams, umbrellaPlant)
        protected virtual Plant FindUmbrella(Vector2 v)
        {
            var board = _plant.board;
            if (board == null)
                return null;

            var gridSystem = board.gridSystem;
            if (gridSystem == null)
                return null;

            int col = _plant.thePlantColumn;
            int row = _plant.thePlantRow;

            Plant bestUmbrella = null;

            // scan columns from this plant to the right
            while (col < board.columnNum)
            {
                var grid = gridSystem.GetGrid(col, row);
                if (grid == null || grid.plants == null)
                    break;
                foreach (var p in grid.plants.ToList())
                {
                    if (p == null)
                        continue;

                    if (!IsUmbrellaValid(p))
                        continue;

                    bestUmbrella = p;
                    break;
                }

                col++;
            }

            return bestUmbrella;
        }
        // Called by animation event
        public override Bullet Shoot_Custom()
        {
            if (_plant.shoot == null)
                return null;

            Vector2 startPos = _plant.shoot.position;

            var creator = CreateBullet.Instance;
            if (creator == null)
                return null;

            int row = _plant.thePlantRow;
            var bulletType = GetBulletType();

            Bullet b = creator.SetBullet(
                startPos.x,
                startPos.y,
                row,
                bulletType,
                BulletMoveWay.Throw,
                false
            );

            if (b == null)
                return null;

            // 1) Umbrella branch
            Plant umbrella = FindUmbrella(startPos);
            if (umbrella != null)
            {
                b.ThrowTo(umbrella, new(startPos), new(flightTime));
                goto ApplyCommon;
            }

            // 2) Zombie branch
            Zombie z = _plant.targetZombie;
            if (z != null && z.col != null)
            {
                b.ThrowTo(z, new(startPos), new(flightTime));
                goto ApplyCommon;
            }

            // 3) Fallback: grid-based speed
            Board board = _plant.board;
            if (board != null && board.gridSystem != null)
            {
                var grid = board.gridSystem.GetGrid(board.columnNum - 1, _plant.thePlantRow);
                if (grid != null)
                {
                    b.ThrowTo(grid.Position, new(startPos), new(flightTime));
                    goto ApplyCommon;
                }
            }

            return null;

        ApplyCommon:
            b.Damage = _plant.attackDamage;
            b.fromType = _plant.thePlantType;
            b.melonSputter = _plant.melonSputter;

            UniqueEffect(b);

            _plant.targetZombie = null;

            int soundId = Random.RandomRangeInt(3, 5);
            GameAPP.PlaySound(soundId, 0.5f, 1f);

            return b;
        }
        // used in MelonShoot: .OrderBy(z => MelonOrderKey(z))
        protected virtual float MelonOrderKey(Zombie z)
        {
            if (z == null || z.axis == null || _plant.axis == null)
                return float.PositiveInfinity;

            Vector3 zPos = z.axis.position;
            Vector3 plantPos = _plant.axis.position;

            float dx = zPos.x - plantPos.x;
            float dy = zPos.y - plantPos.y;

            // Euclidean distance
            return Mathf.Sqrt(dx * dx + dy * dy);
        }
        protected virtual bool IsUmbrella(Plant p) =>
            p.thePlantType == PlantType.CabbageUmbrella
                || p.thePlantType == PlantType.EmeraldUmbrella;
        // Called by animation event
        public override Bullet Shoot2_Custom()
        {
            if (_plant.shoot == null)
                return null;

            Vector2 startPos = _plant.shoot.position;

            var creator = CreateBullet.Instance;
            if (creator == null)
                return null;

            int row = _plant.thePlantRow;
            var bulletType = GetBulletType();

            Bullet b = creator.SetBullet(
                startPos.x,
                startPos.y,
                row,
                bulletType,
                BulletMoveWay.Throw,
                false
            );

            if (b == null)
                return null;

            // 1) Umbrella branch
            Plant umbrella = FindUmbrella(startPos);
            if (umbrella != null)
            {
                b.ThrowTo(umbrella, new(startPos), new(flightTime));
                goto ApplyCommon;
            }

            // 2) Zombie branch
            Zombie z = _plant.targetZombie;
            if (z != null && z.col != null)
            {
                b.ThrowTo(z, new(startPos), new(flightTime));
                goto ApplyCommon;
            }

            // 3) Fallback: grid-based speed
            Board board = _plant.board;
            if (board != null && board.gridSystem != null)
            {
                var grid = board.gridSystem.GetGrid(board.columnNum - 1, _plant.thePlantRow);
                if (grid != null)
                {
                    b.ThrowTo(grid.Position, new(startPos), new(flightTime));
                    goto ApplyCommon;
                }
            }

            return null;

        ApplyCommon:
            b.Damage = _plant.attackDamage;
            b.fromType = _plant.thePlantType;
            b.melonSputter = _plant.melonSputter;

            UniqueEffect(b);

            _plant.targetZombie = null;

            int soundId = Random.RandomRangeInt(3, 5);
            GameAPP.PlaySound(soundId, 0.5f, 1f);

            return b;
        }
        public override BulletType GetBulletType() => BulletType.Bullet_cabbage;
        public override BulletType GetBulletType2() => GetBulletType();
        protected virtual Vector2 GetZombiePosition(Zombie zombie)
        {
            if (zombie == null || zombie.col == null)
                return default;

            var bounds = zombie.col.bounds;

            // NORMAL ZOMBIES
            if (zombie.theZombieType != ZombieType.ZombieBoss && zombie.theZombieType != ZombieType.ZombieBoss2) //the 2 zombie types
            {
                // aim at the TOP of the collider
                return new Vector2(
                    bounds.center.x,
                    bounds.center.y + bounds.extents.y
                );
            }

            // SPECIAL ZOMBIES (0x2C, 0x2E) — use land height instead
            var mouse = Mouse.Instance;
            if (mouse != null)
            {
                float landY = mouse.GetLandY(8.5f, _plant.thePlantRow);
                return new Vector2(8.5f, landY + 0.3f);
            }

            return default;
        }
    }
}