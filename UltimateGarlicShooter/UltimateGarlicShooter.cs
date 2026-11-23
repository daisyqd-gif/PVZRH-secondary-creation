namespace UltimateGarlicShooter
{
    public class UltimateGarlicShooter : BaseCustomPlant
    {
        public int level = 1;
        public override Transform FindShoot() => transform.Find("Shoot");
        public bool IsRogueShooting => ShootingManager.Instance != null;
        public override void OnSpawn()
        {
            if(IsRogueShooting)
            {
                _plant.transform.Find("PeaShooter_Head").transform.Find("Level3").gameObject.SetActive(true);
            }
        }
        public override string GetTextString() => IsRogueShooting ? $"等级：{_plant.shootingLevel}" : "";
        public override Bullet Shoot_Custom()
        {
            Shoot_Async();
            return null;
        }
        public async void Shoot_Async()
        {
            try
            {
                try
                {
                    if (PlantMgr.GetPercent(2f))
                    {
                        StartPF();
                    }
                    Vector2 vector = _plant.shoot.position;
                    int num = (_plant.thePlantRow - 1 < 0) ? _plant.thePlantRow : (_plant.thePlantRow - 1);
                    int num2 = (_plant.thePlantRow + 1 >= Board.Instance.rowNum) ? _plant.thePlantRow : (_plant.thePlantRow + 1);
                    //int num3 = _plant.thePlantRow + 1;
                    //int rowNum = Board.Instance.rowNum;
                    //int num4 = _plant.thePlantRow - 1;
                    var f = (Zombie z) => z != null && z.theZombieRow == _plant.thePlantRow && !z.isMindControlled && z.Alive && Lawnf.InLandStatus(z.theStatus);
                    for(int i = 0; i < (IsRogueShooting ? 3 : level); i++)
                    {
                        SetBullets(vector,num,num2,f);

                        int counter = 0;
                        foreach(Zombie zombie in Lawnf.GetAllZombies(false).OrderBy(z =>
                        {
                            if(z == null || !z.Alive)
                            {
                                return float.MaxValue;
                            }
                            else
                            {
                                return MathHelper.DistanceSq(_plant.shoot.position,z.axis.position);
                            }
                        }))
                        {
                            if(counter >= _plant.shootingLevel) return;
                            SetBulletsAtZombie(zombie,f);
                            counter++;
                            Vector2 pos = zombie.col != null ? zombie.col.bounds.center : zombie.axis.position;
                            PlantMgr.CreateLine(transform,_plant.shoot.position,pos,new Color(238, 210, 204),new Color(238, 210, 204),_plant.thePlantRow,token,1,0.5f,1,null);
                        }
                        await DelayTask.WaitForFixedUpdate();
                    }
                    SuperEnd();
                }
                catch(Exception e)
                {
                    SuperEnd();
                    ModLogger.LogError(e);
                }
            }
            catch(Exception e)
            {
                ModLogger.LogError(e);
            }
        }
        public void SetBullets(Vector2 vector, int num, int num2, Il2CppSystem.Func<Zombie,bool> f)
        {
            if (PlantMgr.IsNotNullMonoBehaviour(Lawnf.GetNearestZombie(_plant.board, vector, f),out var z))
            {
                Bullet bullet = CreateBullet.Instance.SetBullet(vector.x, vector.y, z.theZombieRow, Bullet_throw1, BulletMoveWay.Throw, false);
                bullet.ThrowTo(z,new(),new());
                bullet.fromType = _plant.thePlantType;
                bullet.Damage = _plant.attackDamage;
            }
            Bullet bullet2 = CreateBullet.Instance.SetBullet(vector.x, vector.y, _plant.thePlantRow, Bullet1, BulletMoveWay.MoveRight, false);
            Bullet bullet3 = CreateBullet.Instance.SetBullet(vector.x + 0.2f, vector.y, _plant.thePlantRow, Bullet1, BulletMoveWay.MoveRight, false);
            Bullet bullet4 = CreateBullet.Instance.SetBullet(vector.x, vector.y, num, Bullet1, BulletMoveWay.MoveRight_threePeater, false);
            Bullet bullet5 = CreateBullet.Instance.SetBullet(vector.x, vector.y, num2, Bullet1, BulletMoveWay.MoveRight_threePeater, false);
            bullet2.Damage = _plant.attackDamage;
            bullet4.Damage = _plant.attackDamage;
            bullet5.Damage = _plant.attackDamage;
            bullet3.Damage = _plant.attackDamage;
            bullet2.fromType = _plant.thePlantType;
            bullet4.fromType = _plant.thePlantType;
            bullet5.fromType = _plant.thePlantType;
            bullet3.fromType = _plant.thePlantType;
        }
        public void SetBulletsAtZombie(Zombie z, Il2CppSystem.Func<Zombie,bool> f)
        {
            if(z == null || z.col == null) return;
            Vector2 vector = z.col.bounds.center + Vector3.right * 2;
            ParticleManager.Instance.SetParticle(ParticleType.BombCloud_white_poison,vector);
            GameAPP.PlaySound(SoundType.CherryBomb);
            if (PlantMgr.IsNotNullMonoBehaviour(Lawnf.GetNearestZombie(_plant.board, vector, f),out var zombie))
            {
                Bullet bullet = CreateBullet.Instance.SetBullet(vector.x, vector.y, zombie.theZombieRow, Bullet_throw1, BulletMoveWay.Throw, false);
                bullet.ThrowTo(zombie,new(),new());
                bullet.fromType = _plant.thePlantType;
                bullet.Damage = _plant.attackDamage;
            }
        }


        public override Bullet Shoot2_Custom()
        {
            return null;
        }

        public override void StartPF()
        {
            _plant.invincible = true;
            _prevUncrashable = _plant.uncrashable;
            _plant.uncrashable = true;
            isPF = true;
            _plant.isFlashing = true;
            Vector2 vector = _plant.transform.Find("Shoot").transform.position;
            Board.Instance.boardAction.CreateCherryExplode(vector, _plant.thePlantRow, CherryBombType.Normal, 1800, PlantType.Nothing, null, true);
            int counter = 0;
            foreach (Zombie zombie in Lawnf.GetAllZombies(false).OrderByDescending(z =>
            {
                if(z == null || !z.Alive)
                {
                    return 0;
                }
                else
                {
                    return z.theHealth;
                }
            }))
            {
                if (zombie != null && zombie.theStatus != ZombieStatus.Dying && zombie.theStatus != ZombieStatus.Flying && zombie.theStatus != ZombieStatus.Miner_digging && !zombie.isMindControlled)
                {
                    Bullet bullet = CreateBullet.Instance.SetBullet(vector.x, vector.y + 0.1f, zombie.theZombieRow, Bullet_throw1, BulletMoveWay.Throw, false);
                    bullet.ThrowTo(zombie,new(),new());
                    bullet.fromType = _plant.thePlantType;
                    bullet.Damage = _plant.attackDamage * 3;
                    counter++;
                }
                if (counter >= 20)
                {
                    break;
                }
            }
            if(!UnlockUpgradesBuff.IsActiveUlti()) return;
            int num = 2;
            _plant.thePlantHealth *= num;
            _plant.attackDamage *= num;
            _plant.thePlantHealth = _plant.thePlantHealth;
            _plant.UpdateText();
            if (!IsRogueShooting)
            {
                if (level == 1)
                {
                    level = 2;
                    _plant.transform.Find("PeaShooter_Head").transform.Find("Level2").gameObject.SetActive(true);
                    return;
                }
                if (level == 2)
                {
                    level = 3;
                    _plant.transform.Find("PeaShooter_Head").transform.Find("Level3").gameObject.SetActive(true);
                }
            }
        }
    }
}