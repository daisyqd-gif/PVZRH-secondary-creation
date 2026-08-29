using System.Threading.Tasks;
using CustomPlantClass.Runtime.Tasks;

namespace UltimateCherryFireShooter_Remade
{
    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class UltimateCherryFireShooter_Remade : BaseCustomPlant
    {
        public override void OnSpawn()
        {
            if (Utils.InGame() && _plant.board != null && _plant != null)
            {
                for (int i = 0; i < _plant.board.rowNum; i++)
                {
                    var a = (Zombie z) => { z.SetJalaed(); };
                    _plant.board.boardAction.CreateFireLine(i, 1800, false, false, true, a, _plant.thePlantType);
                }
            }
        }
        public async override void OnDie(Plant.DieReason reason)
        {
            OnSpawn();
        }
        public override Transform FindShoot() => transform.FindChild("Shoot");
        public IEnumerator Shooting()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var bullet = PlantMgr.SetBullet(
                        _plant,
                        Plugin.Bullet_FireCherry,
                        BulletMoveWay.SuperGatling,
                        new Vector2(Random.Range(-0.1f, 0.1f),Random.Range(-0.1f, 0.1f)),
                        Random.Range(-15f, 15f)
                    );
                    bullet.normalSpeed = Random.Range(12f, 14f);
                    var bullet1 = PlantMgr.SetBullet(
                        _plant,
                        Plugin.Bullet_FireCherry,
                        BulletMoveWay.SuperGatling,
                        new Vector2(Random.Range(-0.1f, 0.1f),Random.Range(-0.1f, 0.1f)+0.4f),
                        Random.Range(-15f, 15f)
                    );
                    bullet1.normalSpeed = Random.Range(12f, 14f);
                    var bullet2 = PlantMgr.SetBullet(
                        _plant,
                        Plugin.Bullet_FireCherry,
                        BulletMoveWay.SuperGatling,
                        new Vector2(Random.Range(-0.1f, 0.1f),Random.Range(-0.1f, 0.1f)-0.4f),
                        Random.Range(-15f, 15f)
                    );
                    bullet2.normalSpeed = Random.Range(12f, 14f);
                }

                yield return new WaitForFixedUpdate();
            }
        }
        
        public override Bullet Shoot_Custom()
        {
            if (_plant.starUp)
            {
                _plant.StartCoroutine(Shooting());
            }
            else
            {
                PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherry,BulletMoveWay.MoveRight,new Vector2(0,0.4f));
                PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherry,BulletMoveWay.MoveRight,new Vector2(0,-0.4f));
                if (Lawnf.TravelAdvanced(AdvBuff.EnumValue3002))
                {
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherry,BulletMoveWay.Sin);
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherry,BulletMoveWay.Sin,new Vector2(0,0.4f));
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherry,BulletMoveWay.Sin,new Vector2(0,-0.4f));
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherry,BulletMoveWay.Sin).theExistTime+=0.5f;
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherry,BulletMoveWay.Sin,new Vector2(0,0.4f)).theExistTime+=0.5f;
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherry,BulletMoveWay.Sin,new Vector2(0,-0.4f)).theExistTime+=0.5f;
                }
            }
            Bullet b=PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherry,BulletMoveWay.MoveRight);
            // 3% chance to trigger fire line
            if (PlantMgr.GetPercent(3f))
            {
                var a = (Zombie z) => { z.SetJalaed(); };
                _plant.board.boardAction.CreateFireLine(
                    _plant.thePlantRow,
                    action:a,
                    fromType:_plant.thePlantType
                );
            }
            return b;
        }
    }
    public class FinalCherryFireShooter_Remade : BaseCustomPlant
    {
        public override Transform FindShoot() => transform.FindChild("Shoot");
        public async override void OnSpawn()
        {
            if (Utils.InGame() && _plant.board != null && _plant != null)
            {
                float x = _plant.axis.position.x;
                float y = _plant.axis.position.y;
                int damage = AttackDamage;
                var plantType = _plant.thePlantType;
                var board = _plant.board;
                for(int i = 0; i < 6; i++)
                {
                    for (int row = 0; row < board.rowNum; row++)
                    {
                        var a = (Zombie z) => z.SetJalaed();
                        board.boardAction.CreateFireLine(row, 10 * damage, false, false, true, a, plantType);
                        Bullet bullet = CreateBullet.Instance.SetBullet(x, y + 0.7f, row, Plugin.Bullet_FireCherryFinalFire, BulletMoveWay.MoveRight_threePeater);
                        bullet.Damage = damage;
                        bullet.fromType = plantType;
                    }
                    await DelayTask.Delay(0.167f);
                }
            }
        }
        public IEnumerator Shooting()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    var bullet = PlantMgr.SetBullet(
                        _plant,
                        Plugin.Bullet_FireCherryFinal,
                        GetBulletMoveWay(),
                        new Vector2(Random.Range(-0.1f, 0.1f),Random.Range(-0.1f, 0.1f)),
                        Random.Range(-15f, 15f)
                    );
                    bullet.normalSpeed = Random.Range(12f, 14f);
                }

                yield return new WaitForFixedUpdate();
            }
        }
        public override BulletMoveWay GetBulletMoveWay()
        {
            if(Lawnf.TravelUltimate(UltiBuff.EnumValue51) && isPF) return BulletMoveWay.MoveRight;
            return BulletMoveWay.SuperGatling;
        }
        public override int AttackDamage => 
        Lawnf.TravelUltimate(UltiBuff.EnumValue51) && isPF ? _plant.attackDamage*2 : _plant.attackDamage;
        protected override bool IsAsyncPF => true;
        protected override async Task SuperShoot_Async()
        {
            _plant.anim.SetBoolString("shooting",true);
            // Runs once per physics tick while skillTime > 0
            for(int i=0;i<500;i++)
            {
                // Play animation
                _plant.anim.SetTrigger("shoot");
                // Base 3 bullets
                Bullet b0 = PlantMgr.SetBullet(
                    _plant,
                    Plugin.Bullet_FireCherryFinal,
                    GetBulletMoveWay(),
                    AttackDamage,
                    Vector2.zero,
                    Random.Range(-15, 15)
                );

                Bullet b1 = PlantMgr.SetBullet(
                    _plant,
                    Plugin.Bullet_FireCherryFinal,
                    GetBulletMoveWay(),
                    AttackDamage,
                    new Vector2(0,0.3f),
                    Random.Range(-15, 15)
                );

                Bullet b2 = PlantMgr.SetBullet(
                    _plant,
                    Plugin.Bullet_FireCherryFinal,
                    GetBulletMoveWay(),
                    AttackDamage,
                    new Vector2(0,-0.3f),
                    Random.Range(-15, 15)
                );

                b0.normalSpeed = 10f;
                b1.normalSpeed = 10f;
                b2.normalSpeed = 10f;

                // Buff 51 extra bullets
                if (Lawnf.TravelAdvanced(AdvBuff.EnumValue3002))
                {
                    // Group 1
                    Bullet g1a = PlantMgr.SetBullet(_plant, Plugin.Bullet_FireCherryFinal, BulletMoveWay.Sin, AttackDamage, Vector2.zero,15);
                    Bullet g1b = PlantMgr.SetBullet(_plant, Plugin.Bullet_FireCherryFinal, BulletMoveWay.Sin, AttackDamage, Vector2.zero,-15);
                    g1b.theExistTime = 0.5f;
                    g1a.normalSpeed = 10f;
                    g1b.normalSpeed = 10f;

                    // Group 2
                    Bullet g2a = PlantMgr.SetBullet(_plant, Plugin.Bullet_FireCherryFinal, BulletMoveWay.Sin, AttackDamage, Vector2.zero,15);
                    Bullet g2b = PlantMgr.SetBullet(_plant, Plugin.Bullet_FireCherryFinal, BulletMoveWay.Sin, AttackDamage, Vector2.zero,-15);
                    g2b.theExistTime = 0.5f;
                    g2a.normalSpeed = 10f;
                    g2b.normalSpeed = 10f;

                    // Group 3
                    Bullet g3a = PlantMgr.SetBullet(_plant, Plugin.Bullet_FireCherryFinal, BulletMoveWay.Sin, AttackDamage, Vector2.zero,15);
                    Bullet g3b = PlantMgr.SetBullet(_plant, Plugin.Bullet_FireCherryFinal, BulletMoveWay.Sin, AttackDamage, Vector2.zero,-15);
                    g3b.theExistTime = 0.5f;
                    g3a.normalSpeed = 10f;
                    g3b.normalSpeed = 10f;
                }

                // Wait for next physics tick
                await DelayTask.DelayScaled(0.02f,()=>_plant.attributeSpeed,token);
            }
            _plant.anim.SetBoolString("shooting",false);
        }
        public override Bullet Shoot_Custom()
        {
            if (isPF)
            {
                //tecnically impossible because the pf effect anim does not have events
            }
            else if (_plant.starUp)
            {
                _plant.StartCoroutine(Shooting());
            }
            else
            {
                PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherryFinal,BulletMoveWay.SuperGatling,Vector2.zero,15);
                PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherryFinal,BulletMoveWay.SuperGatling,Vector2.zero,-15);
                if (Lawnf.TravelAdvanced(AdvBuff.EnumValue3002))
                {
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherryFinal,BulletMoveWay.Sin);
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherryFinal,BulletMoveWay.Sin,new Vector2(0,0.4f));
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherryFinal,BulletMoveWay.Sin,new Vector2(0,-0.4f));
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherryFinal,BulletMoveWay.Sin).theExistTime+=0.5f;
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherryFinal,BulletMoveWay.Sin,new Vector2(0,0.4f)).theExistTime+=0.5f;
                    PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherryFinal,BulletMoveWay.Sin,new Vector2(0,-0.4f)).theExistTime+=0.5f;
                }
            }
            if(PlantMgr.GetPercent(0.3f)) StartPF();
            Bullet b=PlantMgr.SetBullet(_plant,Plugin.Bullet_FireCherryFinal,BulletMoveWay.SuperGatling);
            // 3% chance to trigger fire line
            if (PlantMgr.GetPercent(3f))
            {
                var a = (Zombie z) => { z.SetJalaed(); };
                _plant.board.boardAction.CreateFireLine(
                    _plant.thePlantRow,
                    action:a,
                    fromType:_plant.thePlantType
                );
            }
            return b;
        }
        public async override void OnDie(Plant.DieReason reason)
        {
            float x = _plant.axis.position.x;
            float y = _plant.axis.position.y;
            int damage = AttackDamage;
            var plantType = _plant.thePlantType;
            var board = _plant.board;
            for(int i = 0; i < 6; i++)
            {
                for (int row = 0; row < board.rowNum; row++)
                {
                    var a = (Zombie z) => z.SetJalaed();
                    board.boardAction.CreateFireLine(row, 10 * damage, false, false, true, a, plantType);
                    Bullet bullet = CreateBullet.Instance.SetBullet(x, y + 0.7f, row, Plugin.Bullet_FireCherryFinalFire, BulletMoveWay.MoveRight_threePeater);
                    bullet.Damage = damage;
                    bullet.fromType = plantType;
                }
                await DelayTask.Delay(0.167f);
            }
        }
        //public async Task DieEvent()
        //{
        //}
    }
}
