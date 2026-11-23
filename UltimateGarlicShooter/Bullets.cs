namespace UltimateGarlicShooter
{
    public class Bullet_garlicCherryPea : BaseCustomBullet
    {
        public virtual BulletType SplitType => Bullet2;
        public override bool HitZombie(Zombie zombie)
        {
			Bullet bullet = CreateBullet.Instance.SetBullet(_bullet.transform.position.x, _bullet.transform.position.y, zombie.theZombieRow, BulletType.Bullet_pea_bombCherry, BulletMoveWay.MoveRight, false);
			bullet.Damage = _bullet.Damage;
			bullet.fromType = _bullet.fromType;
			if (Lawnf.TravelUltimate(BulletBuff))
			{
				Bullet bullet2 = CreateBullet.Instance.SetBullet(_bullet.transform.position.x + 2f, _bullet.transform.position.y, zombie.theZombieRow, SplitType, BulletMoveWay.Free, false);
				bullet2.Damage = _bullet.Damage;
				bullet2.fromType = _bullet.fromType;
				bullet2.transform.Rotate(0f, 0f, 15f);
				Bullet bullet3 = CreateBullet.Instance.SetBullet(_bullet.transform.position.x + 2f, _bullet.transform.position.y, zombie.theZombieRow, SplitType, BulletMoveWay.Free, false);
				bullet3.Damage = _bullet.Damage;
				bullet3.fromType = _bullet.fromType;
				bullet3.transform.Rotate(0f, 0f, -15f);
			}
			Miasma m = Miasma.SetMiasma(zombie.ColliderPosition, zombie.theZombieRow, _bullet.board, true, false);
            if(m != null)
            {
                m.SetData("IsCherry",true);
            }
			int num = 0;
			foreach (Zombie zombie2 in Lawnf.GetAllZombies(false).OrderByDescending(z =>
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
				Vector3 position = zombie2.axis.position;
				Vector3 position2 = _bullet.transform.position;
				if (zombie2.theZombieRow == _bullet.theBulletRow || zombie2.theZombieRow == _bullet.theBulletRow + 1 || zombie2.theZombieRow == _bullet.theBulletRow - 1)
				{
					if (zombie2.theStatus != ZombieStatus.Dying && zombie2.theStatus != ZombieStatus.Flying && zombie2.theStatus != ZombieStatus.Miner_digging)
					{
                        if (!zombie2.isMindControlled && zombie2.col != null && zombie2.col.enabled && zombie2 != null && zombie2.axis != null && position.x > position2.x)
                        {
                            Bullet bullet4 = 
                            CreateBullet.Instance.SetBullet(position2.x, position2.y, zombie.theZombieRow, Bullet_throw2, BulletMoveWay.Throw, false);
                            bullet4.ThrowTo(zombie2,new(),new());
                            bullet4.fromType = _bullet.fromType;
                            bullet4.Damage = (int)(_bullet.Damage * 0.1f);
                            num++;
                            if (num >= 10)
                            {
                                break;
                            }
                        }
					}
				}
			}
			Board.Instance.boardAction.CreateCherryExplode(_bullet.transform.position, _bullet.theBulletRow, CherryBombType.Bullet, _bullet.Damage, PlantType.Nothing, null, true);
			_bullet.Die();
			return false;
        }
    }

    public class Bullet_garlicCherryPea_split : Bullet_garlicCherryPea
    {
        public override BulletType SplitType => Bullet3;
    }

    public class Bullet_garlicCherryPea_throw : BaseCustomBullet
    {
        public override bool HitZombie(Zombie zombie)
        {
            Bullet bullet = CreateBullet.Instance.SetBullet(_bullet.transform.position.x, _bullet.transform.position.y, zombie.theZombieRow, Bullet1, BulletMoveWay.MoveRight, false);
            bullet.Damage = _bullet.Damage;
            bullet.fromType = _bullet.fromType;
            _bullet.Die();
            return false;
        }
        public override bool HitLand()
        {
            Board.Instance.boardAction.CreateCherryExplode(_bullet.transform.position, _bullet.theBulletRow, CherryBombType.Bullet, _bullet.Damage, PlantType.Nothing, null, true);
			_bullet.Die();
            return false;
        }
    }

    public class Bullet_garlicCherryPea_throw2 : Bullet_garlicCherryPea_throw
    {
        public override bool HitZombie(Zombie zombie) => false;
    }

    public class Bullet_garlicCherryPea_split_2 : BaseCustomBullet
    {
        public override bool HitZombie(Zombie zombie)
        {
            HitLand();
            return false;
        }
        public override bool HitLand()
        {
            Board.Instance.boardAction.CreateCherryExplode(_bullet.transform.position, _bullet.theBulletRow, CherryBombType.Normal, _bullet.Damage, PlantType.Nothing, null, true);
			_bullet.Die();
            return false;
        }
    }
}