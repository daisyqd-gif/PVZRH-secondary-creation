namespace MegaGatlingExpansion
{
    public class StarMegaGating : CustomShooter
    {
        private static int PFChance = 10;
        public int AttributeCount_Custom = 0;
        public override Transform FindShoot() => transform.FindChild("GatlingPea_head/Shoot");
        public override void OnSpawn()
        {
            if (GameAPP.theGameStatus == GameStatus.InGame && CreateBullet.Instance != null && _plant.board != null && _plant.anim != null && CreateItem.Instance != null && Board.Instance != null)
            {
                for (int i = 0; i < 5; i++)
                    CreateItem.Instance.SetCoin(_plant.thePlantColumn, _plant.thePlantRow, 0, 0);
                StartPF();
            }
        }
        public override BulletType GetBulletType()
        {
            return BulletType.Bullet_pea_star;
        }
        public IEnumerator Shooting_Custom()
        {
            for (int i = 0; i < 4 + AttributeCount_Custom; i++)
            {
                if (_plant == null || _plant.IsDestroyed()) yield break;
                PlantMgr.SetBullet(_plant,GetBulletType(),BulletMoveWay.MoveRight);
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        }
        public override void StartPF()
        {
            AttributeCount_Custom = Mathf.Min(3, AttributeCount_Custom + 1);
            _plant.invincible = true;
            _plant.uncrashable = true;
            isPF = true;
            _plant.isFlashing = true;
            _plant.anim.SetTriggerString("create");
        }
        public void Summon()
        {
            _plant.board.CreateActiveMateorite();
        }
        public override Bullet Shoot_Custom()
        {
            if ((Lawnf.TravelUltimate(UltiBuff.EnumValue50) && Random.Range(0, 100) <= PFChance*3) || Random.Range(0, 100) <= PFChance || _plant.starUp)
            {
                if(isPF)
                {
                    _plant.StartCoroutine(Shooting_Custom());
                    return null;
                }
                StartPF();
                return null;
            }

            _plant.StartCoroutine(Shooting_Custom());
            return null;
        }
    }
}
