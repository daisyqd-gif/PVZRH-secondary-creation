namespace UltimateSolarCoronaCabbage_Remade
{
    public class UltimateSolarCoronaCabbage_Remade : CustomThrower
    {
        private float stayTime;
        public void Enter()
        {
            _plant.anim.SetBoolString("enabled",true);
        }

        // ---------------------------------------------------------
        // 1. ONUPDATE (faithful ordering)
        // ---------------------------------------------------------
        public override void OnUpdate()
        {
            if (GameAPP.theGameStatus != GameStatus.InGame)
                return;

            stayTime += Time.deltaTime;

            // Every 30 seconds → spawn SuperSolar
            if (stayTime >= 30f)
            {
                StartPF();

                stayTime = 0f;
            }
        }

        // ---------------------------------------------------------
        // 3. SHOOT POINT (faithful)
        // ---------------------------------------------------------
        public override Transform FindShoot()
            => transform.Find("Shoot");

        // ---------------------------------------------------------
        // 4. BULLET TYPES (faithful)
        // ---------------------------------------------------------
        public override BulletType GetBulletType()
            => Plugin.DataContainer.theBulletType;

        public override BulletType GetBulletType2()
            => Plugin.DataContainer.theBulletType;

        // ---------------------------------------------------------
        // 5. UNIQUE EFFECT (faithful sun-scaling)
        // ---------------------------------------------------------
        protected override void UniqueEffect(Bullet b)
        {
            var board = _plant.board;

            if (board.theSun > 15000)
            {
                board.theSun -= 200;
                b.Damage *= 4;
            }
        }
        public override void PlantShootUpdate()
        {
            if(_plant.board==null) return;
            if(GameAPP.theGameStatus!=GameStatus.InGame) return;
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
        public override void StartPF()
        {
            _plant.anim.SetTriggerString("super");

            if (SuperSolar.Instance == null)
            {
                var prefab = Plugin.DataContainer.superSolar;
                if (prefab != null)
                {
                    var obj = Instantiate(
                        prefab,
                        new Vector2(-25f, 35f),
                        Quaternion.identity,
                        _plant.board.transform
                    );

                    var super = obj.GetComponent<SuperSolar>();
                    //-7.8836 3.2372 0
                    super.targetPosition = new Vector3(-7.88f, 3.24f);
                    if (super != null)
                    {
                        GameAPP.PlaySound(95, 0.5f, 1f);
                    }
                }
            }
            foreach(Zombie zombie in _plant.board.zombieArray)
            {
                if (zombie == null) continue;
                if (zombie.isMindControlled) continue;
                if (zombie.axis == null) continue;
                Vector2 targetCurrentPosition = zombie.ColliderPosition;

                SuperSolarEmit.SetSuperSolarEmit(targetCurrentPosition,zombie.theZombieRow,_plant.attackDamage);
            }
        }
    }
}
