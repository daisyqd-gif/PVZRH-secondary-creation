namespace MoreDoomGatling
{
    public class UltimateFireDoomGatling : MonoBehaviour
    {
        public static ID PLANT_ID=3150;
        public UltimateDoomGatling plant => gameObject.GetComponent<UltimateDoomGatling>();
        public int counter=0;
        public bool isPF=false;
        public void Start()
        {
            plant.shoot=gameObject.transform.FindChild("GatlingPea_head/Shoot");
            if(plant.board!=null && GameAPP.theGameStatus == GameStatus.InGame)
            {
                for(int i=0; i<plant.board.rowNum; i++)
                {
                    plant.board.boardAction.CreateFireLine(i);
                }
            }
        }
        public BulletType GetBulletType()
        {
            if(counter>=96 || (counter>=48 && Lawnf.TravelAdvanced(AdvBuff.EnumValue3)) || Random.Range(0, 100)<5)
            {
                if (!Lawnf.TravelAdvanced(AdvBuff.EnumValue2))
                {
                    plant.thePlantAttackCountDown = 3f;
                }
                counter=0;
                return Bullet_fireDoom.BULLET_ID_BIG;
            }
            else
            {
                counter++;
                return Bullet_fireDoom.BULLET_ID;
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
            plant.anim.SetBoolString("shooting",true);

            for (int i = 0; i < 500; i++)
            {
                Vector3 pos = plant.shoot.position;
                Bullet b=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, plant.thePlantRow,
                    GetBulletType(), BulletMoveWay.Right_free
                );
                switch (b.theBulletType)
                {
                    case (BulletType)3151: b.Damage=plant.attackDamage*6; b.theStatus = BulletStatus.Doom_big; break;
                    default: b.Damage=plant.attackDamage; break;
                }
                b.transform.Rotate(0,0,Random.Range(-15f,15f));
                Bullet b1=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y+0.3f, plant.thePlantRow,
                    GetBulletType(), BulletMoveWay.Right_free
                );
                switch (b1.theBulletType)
                {
                    case (BulletType)3151: b1.Damage=plant.attackDamage*6; b1.theStatus = BulletStatus.Doom_big; break;
                    default: b1.Damage=plant.attackDamage; break;
                }
                b1.transform.Rotate(0,0,Random.Range(-15f,15f));
                Bullet b2=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y-0.3f, plant.thePlantRow,
                    GetBulletType(), BulletMoveWay.Right_free
                );
                switch (b2.theBulletType)
                {
                    case (BulletType)3151: b2.Damage=plant.attackDamage*6; b2.theStatus = BulletStatus.Doom_big; break;
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
            plant.anim.SetBoolString("shooting",false);
        }
        public Bullet AnimShoot_Custom()
        {
            Vector2 pos=plant.shoot.position;
            Bullet b=CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType(), BulletMoveWay.MoveRight
            );
            switch (b.theBulletType)
            {
                case (BulletType)3151: b.Damage=plant.attackDamage*6; b.theStatus = BulletStatus.Doom_big; break;
                default: b.Damage=plant.attackDamage; break;
            }
            Bullet b1=CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType(), BulletMoveWay.Right_free
            );
            switch (b1.theBulletType)
            {
                case (BulletType)3151: b1.Damage=plant.attackDamage*6; b1.theStatus = BulletStatus.Doom_big; break;
                default: b1.Damage=plant.attackDamage; break;
            }
            b1.transform.Rotate(0,0,15f);
            Bullet b2=CreateBullet.Instance.SetBullet(
                pos.x, pos.y, plant.thePlantRow,
                GetBulletType(), BulletMoveWay.Right_free
            );
            switch (b2.theBulletType)
            {
                case (BulletType)3151: b2.Damage=plant.attackDamage*6; b2.theStatus = BulletStatus.Doom_big; break;
                default: b2.Damage=plant.attackDamage; break;
            }
            b2.transform.Rotate(0,0,-15f);
            int soundID = Random.Range(3, 5);
            GameAPP.PlaySound(soundID, 0.5f, 1f);
            if(Random.Range(0,100)<5 && !isPF) plant.StartCoroutine(SuperShoot_Custom());
            return b;
        }
        public void OnDestroy()
        {
            if(plant.board!=null && GameAPP.theGameStatus == GameStatus.InGame)
            {
                for(int i=0; i<plant.board.rowNum; i++)
                {
                    plant.board.boardAction.CreateFireLine(i);
                }
            }
        }
    }
    
}