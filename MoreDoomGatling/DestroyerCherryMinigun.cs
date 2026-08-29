namespace MoreDoomGatling
{
    public class DestroyerCherryMinigun : MonoBehaviour
    {
        public static ID PLANT_ID=3153;
        public UltimateMinigun plant => gameObject.GetComponent<UltimateMinigun>();
        public int counter=0;
        public bool isPF=false;
        public void Start()
        {
            plant.shoot=gameObject.transform.FindChild("Shoot");
        }
        public BulletType GetBulletType()
        {
            if(counter>=96 || (counter>=48 && Lawnf.TravelAdvanced(AdvBuff.EnumValue3)) || Random.Range(0, 100)<5)
            {
                counter=0;
                return Bullet_fireDoom.BULLET_ID_CHERRY_BIG;
            }
            else
            {
                counter++;
                return Bullet_fireDoom.BULLET_ID_CHERRY;
            }
        }
        public virtual IEnumerator SuperShoot_Custom()
        {
            if (plant.shoot == null)
            {
                yield break;
            }
            isPF = true;
            plant.invincible = true;
            plant.uncrashable = true;
            plant.flashCountDown = 5f;
            plant.isFlashing = true;

            for (int i = 0; i < 500; i++)
            {
                Vector3 pos = plant.shoot.position;
                Bullet b=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, plant.thePlantRow,
                    GetBulletType(), BulletMoveWay.Right_free
                );
                switch (b.theBulletType)
                {
                    case (BulletType)3153: b.Damage=plant.attackDamage*6; b.theStatus = BulletStatus.Doom_big; break;
                    default: b.Damage=plant.attackDamage; break;
                }
                b.transform.Rotate(0,0,Random.Range(-15f,15f));
                Bullet b1=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y+0.3f, plant.thePlantRow,
                    GetBulletType(), BulletMoveWay.Right_free
                );
                switch (b1.theBulletType)
                {
                    case (BulletType)3153: b1.Damage=plant.attackDamage*6; b1.theStatus = BulletStatus.Doom_big; break;
                    default: b1.Damage=plant.attackDamage; break;
                }
                b1.transform.Rotate(0,0,Random.Range(-15f,15f));
                Bullet b2=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y-0.3f, plant.thePlantRow,
                    GetBulletType(), BulletMoveWay.Right_free
                );
                switch (b2.theBulletType)
                {
                    case (BulletType)3153: b2.Damage=plant.attackDamage*6; b2.theStatus = BulletStatus.Doom_big; break;
                    default: b2.Damage=plant.attackDamage; break;
                }
                b2.transform.Rotate(0,0,Random.Range(-15f,15f));

                if (Time.timeScale > 0f)
                {
                    yield return new WaitForFixedUpdate();
                }
                else
                    yield return null;
            }
            plant.invincible = false;
            plant.uncrashable = false;
            plant.isFlashing = false;
            isPF = false;
        }
        public IEnumerator Shooting()
        {
            Transform shoot = plant.shoot;
            if (shoot == null) yield break;

            Vector2 pos = shoot.position;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var cb = CreateBullet.Instance;
                    if (cb == null) yield break;

                    float x = pos.x + Random.Range(-0.1f, 0.1f);
                    float y = pos.y + Random.Range(-0.1f, 0.1f);

                    var b = cb.SetBullet(
                        x, y,
                        plant.thePlantRow,
                        GetBulletType(),
                        BulletMoveWay.Right_free,
                        false
                    );

                    switch (b.theBulletType)
                    {
                        case (BulletType)3153: b.Damage=plant.attackDamage*6; b.theStatus = BulletStatus.Doom_big; break;
                        default: b.Damage=plant.attackDamage; break;
                    }

                    if (b == null) yield break;

                    b.transform.Rotate(0f, 0f, Random.Range(-15f, 15f));
                    b.fromType = plant.thePlantType;
                    b.Damage = plant.attackDamage;
                    b.normalSpeed = Random.Range(12f, 14f);
                }

                yield return new WaitForFixedUpdate();
            }
        }
        public Bullet AnimShoot_Custom()
        {
            if (plant.starUp) plant.StartCoroutine(Shooting());
            int row = plant.thePlantRow;
            int dmg = plant.attackDamage;
            Vector2 pos = plant.shoot.position;

            Bullet b1 = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, row,
                GetBulletType(),
                BulletMoveWay.MoveRight,
                false
            );

            if (b1 != null)
            {
                b1.Damage = dmg;
                b1.from = plant;
                b1.fromType = plant.thePlantType;
                switch (b1.theBulletType)
                {
                    case (BulletType)3153: b1.Damage=plant.attackDamage*6; b1.theStatus = BulletStatus.Doom_big; break;
                    default: b1.Damage=plant.attackDamage; break;
                }
            }
            if (Lawnf.TravelAdvanced(AdvBuff.EnumValue3002))
            {
                FirePower_Func();
            }
            if(Random.Range(0,100)<5 && !isPF) plant.StartCoroutine(SuperShoot_Custom());
            return b1;
        }
        public void FirePower_Func()
        {
            Vector2 pos = plant.shoot.position;
            pos.x += Random.Range(-0.2f, 0.2f);
            pos.y += Random.Range(-0.2f, 0.2f);

            int row = plant.thePlantRow;
            int dmg = plant.attackDamage;

            Bullet b1 = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, row,
                GetBulletType(),
                BulletMoveWay.Sin,
                false
            );

            Bullet b2 = CreateBullet.Instance.SetBullet(
                pos.x, pos.y, row,
                GetBulletType(),
                BulletMoveWay.Sin,
                false
            );

            if (b1 != null)
            {
                b1.Damage = dmg;
                b1.from = plant;
                b1.fromType = plant.thePlantType;
                switch (b1.theBulletType)
                {
                    case (BulletType)3153: b1.Damage=plant.attackDamage*6; b1.theStatus = BulletStatus.Doom_big; break;
                    default: b1.Damage=plant.attackDamage; break;
                }
            }

            if (b2 != null)
            {
                b2.theExistTime = 0.5f;
                b2.Damage = dmg;
                b2.from = plant;
                b2.fromType = plant.thePlantType;
                switch (b2.theBulletType)
                {
                    case (BulletType)3153: b2.Damage=plant.attackDamage*6; b2.theStatus = BulletStatus.Doom_big; break;
                    default: b2.Damage=plant.attackDamage; break;
                }
            }
        }
    }
}