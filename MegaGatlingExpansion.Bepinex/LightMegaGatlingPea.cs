namespace MegaGatlingExpansion
{
    public class LightMegaGatlingPea : BaseCustomPlant
    {
        public class Target
        {
            // Fields
            public LineRenderer laser; // 0x10
            public Zombie zombie; // 0x18

            // Methods

            // RVA: 0x4BF8A0 Offset: 0x4BE8A0 VA: 0x1804BF8A0
            public Target(LightMegaGatlingPea plant)
            {
                var prefab = plant.laserPrefab;
                var parent = plant.transform;
                laser = Instantiate(prefab, parent);
            }
        }
        // Fields
        public LineRenderer laserPrefab; // 0x228
        private readonly List<Target> targets = new(); // 0x230
        public int attr = 0;
        public float attrcd = 0.5f;
        public int pf_count = 0;
        public override Transform FindShoot() => transform.FindChild("Head/GatlingPea_mouth 拷贝");
        public BulletType GetBulletType_Custom() => BulletType.Bullet_lanternCactus_glow;
        public override string GetTextString() => "充能:" + attr + "\n光能:" + _plant.attributeCount;
        public override Bullet Shoot_Custom()
        {
            if (!isPF && ((Lawnf.TravelUltimate(UltiBuff.EnumValue50) && Random.Range(0, 100) <= 3 + _plant.currentLightLevel * 3) || Random.Range(0, 100) <= 1 + _plant.currentLightLevel))
            {
                return Super();
            }
            if (isPF)
            {
                if (pf_count == 0)
                {
                    isPF = false;
                    OnClicked();
                }
                return null;
            }
            Vector3 pos = _plant.shoot.position;
            Bullet b_curr = null;
            for (int j = 0; j < attr + 1; j++)
            {
                Bullet b = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, _plant.thePlantRow,
                    GetBulletType_Custom(),
                    BulletMoveWay.MoveRight
                );
                b.Damage = _plant.attackDamage + _plant.currentLightLevel * 10;
                
                b.fromType = _plant.thePlantType;
                b_curr = b;
            }
            return b_curr;
        }
        public Bullet Super()
        {
            isPF = true;
            pf_count = 100;
            OnClicked();
            _plant.FlashOnce();
            Vector3 pos = _plant.shoot.position;
            Bullet b_curr = null;
            for (int j = -10; j < 10; j++)
            {
                Bullet b = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, _plant.thePlantRow,
                    GetBulletType_Custom(),
                    BulletMoveWay.Free
                );
                b.Damage = _plant.attackDamage + _plant.currentLightLevel * 10;
                
                b.fromType = _plant.thePlantType;
                b.transform.Rotate(0, 0, j * 3);
                b_curr = b;
                if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) b.Damage *= 2;
            }
            attr = Math.Min(attr + 1, 3);
            return b_curr;
        }
        public bool Shootable()
        {
            if (isPF) return true;
            // 1. Existing targetPlant still valid?
            if (_plant.targetPlant != null && CheckPlant(_plant.targetPlant))
                return true;

            // 2. Scan forward for a chargeable plant
            var board = _plant.board;
            int col = _plant.thePlantColumn + 2;

            Func<Plant, bool> sysPredicate = p =>
            {
                if (p == null)
                    return false;

                try
                {
                    return LanternPea.__c.__9._Shootable_b__20_0(p);
                }
                catch (Exception e)
                {
                    Debug.LogError("Using fallback\n" + e.ToString());
                    return p.thePlantType == PlantType.LanternSplit
                        || p.thePlantType == PlantType.UltimateLanternSplit;
                }
            };

            while (board != null && col < board.columnNum)
            {
                var plants = Lawnf.Get1x1Plants(col, _plant.thePlantRow).ToSystemList();

                var p = plants.FirstOrDefault(sysPredicate);

                _plant.targetPlant = p;

                if (_plant.targetPlant != null)
                    return true;

                col++;
            }

            if (SearchZombie() != null) return true;
            if (_plant.SearchBoss() != null) return true;
            if (_plant.SearchGoldMagnet()) return true;

            // 3. Fallback: normal shooting
            return false;
        }
        public override void Awake()
        {
            base.Awake();
            laserPrefab = transform.FindChild("Laser").GetComponent<LineRenderer>();
            // Attribute mode starts with 0.5 seconds
            _plant.attributeCountdown = 0.5f;

            // Create N targets, where N = TargetCount
            for (int i = 0; i < attr + 4; i++)
            {
                var t = new Target(this);

                // Add to targets list
                targets.Add(t);
            }
        }
        public override void OnUpdate()
        {
            if (_plant != null && _plant.Active && _plant.board != null && !_plant.board.boardTag.isScaredyDream) //this can't be 9
            {
                _plant.UpdateAttackCountDown();
                if (_plant.thePlantAttackCountDown < 0f)
                {
                    _plant.thePlantAttackCountDown = _plant.thePlantAttackInterval * Random.Range(0.95f, 1.05f);
                    if (!_plant.anim.GetBoolString("shooting"))
                    {
                        _plant.anim.SetBoolString("shooting", Shootable());
                    }
                }
            }
            foreach (var t in targets)
            {
                if (t == null) continue;
                if (t.laser == null) continue;

                // If the beam is active, update its start position
                if (t.laser.gameObject.activeSelf)
                {
                    var pos = _plant.shoot.position;
                    t.laser.SetPosition(0, pos);
                }
            }
        }
        public override void OnFixedUpdate()
        {
            attrcd -= Time.deltaTime;
            if (attrcd <= 0)
            {
                attrcd = 0.5f;
                _plant.attributeCount += _plant.currentLightLevel;
                int max = 1000;
                int level = Lawnf.TravelUltimateLevel(UltiBuff.EnumValue48);
                if (level == 1) max = 2000;
                else if (level == 2) max = 10000;
                _plant.attributeCount = Mathf.Clamp(_plant.attributeCount, 0, max);
            }
        }
        public override void StartPF()
        {
            _ = Super();
        }
        void ActionOnZombie(Zombie zombie, int damage)
        {
            if (zombie != null)
            {
                zombie.TakeDamage(
                    DmgType.NormalAll,                     // DmgType.Normal
                    damage,
                    _plant.thePlantType,   // used for damage attribution
                    false
                );
            }
        }
        IEnumerator AttackZombie(Target target)
        {
            // initial setup
            target.zombie = GetNearestAvailableZombie(target);
            var zombie = target.zombie;
            var laser = target.laser;
            float attackTimer = 0.1f;

            while (true)
            {
                // if zombie is valid and still acceptable
                if (zombie != null && CheckZombie(zombie))
                {
                    // 1. Move beam end toward zombie collider center
                    var endPos = laser.GetPosition(1); 
                    var bounds = zombie.col.bounds;
                    var center = bounds.center;
                    var dt = Time.deltaTime;
                    var newEnd = Vector3.MoveTowards(endPos, center, dt * 24f); // FUN_1803d0510

                    laser.SetPosition(1, newEnd);

                    // 2. Move child effect to previous end
                    var child = laser.transform.FindChild("AttackNode");
                    child.position = endPos;

                    // 3. Sorting layer by zombie row
                    var sg = laser.GetComponent<SortingGroup>();
                    sg.sortingLayerName = $"Row{zombie.theZombieRow}";

                    // 4. Tick attack timer with attackSpeedAdder
                    attackTimer -= Time.deltaTime * (_plant.attackSpeedAdder + 1f);

                    // 5. When timer expires → deal damage
                    if (attackTimer < 0f)
                    {
                        if (_plant.attributeCount > 1)
                        {
                            int step = _plant.attributeCount / 10 + 1;
                            _plant.attributeCount -= step;
                            _plant.UpdateText();
                        }

                        ActionOnZombie(zombie, _plant.attackDamage);
                        int soundId = Random.Range(0, 3);
                        GameAPP.PlaySound(soundId, 0.5f, 1f);

                        attackTimer = 0.2f;
                    }

                    yield return null;
                    continue;
                }

                // zombie invalid → try retarget
                target.zombie = GetNearestAvailableZombie(target);
                zombie = target.zombie;

                if (zombie == null)
                {
                    // disable this beam
                    if (target.laser != null)
                        target.laser.gameObject.SetActive(false);

                    // if no beams left → EndShoot, else just stop this coroutine
                    if (!HasAnyTarget())
                        EndShoot();
                    else
                        yield return null; // state 3

                    yield break;
                }
                else
                {
                    // got a new zombie → re‑arm beam
                    zombie = target.zombie;
                    var go = target.laser.gameObject;
                    go.SetActive(true);

                    var shootPos = _plant.shoot.position;
                    laser.SetPosition(0, shootPos);

                    yield return null; // state 2
                }
            }
        }
        IEnumerator ChargePlant(Plant plant)
        {
            // setup
            var laser = targets[0].laser;
            float attackTimer = 0.1f;

            int max = 1000;
            int level = Lawnf.TravelUltimateLevel(UltiBuff.EnumValue48);
            if (level == 1) max = 2000;
            else if (level == 2) max = 10000;

            bool chao = Lawnf.TravelAdvanced(AdvBuff.EnumValue3007);

            while (true)
            {
                // 1. If plant no longer valid → stop
                if (!CheckPlant(plant))
                {
                    if (laser != null)
                        laser.gameObject.SetActive(false);

                    EndShoot();
                    yield break;
                }

                // 2. Move beam end toward plant.shoot2
                attackTimer -= Time.deltaTime;

                var endPos = laser.GetPosition(1);
                var shoot2 = plant.shoot2.position;
                var dt = Time.deltaTime;
                var newEnd = Vector3.MoveTowards(endPos, shoot2, dt * 24f); // same helper

                laser.SetPosition(1, newEnd);

                // move child effect to previous end
                var child = laser.transform.FindChild("AttackNode");
                child.position = endPos;

                // sorting layer by plant row
                var sg = laser.GetComponent<SortingGroup>();
                sg.sortingLayerName = $"Row{plant.thePlantRow}";

                // 3. Tick attack timer with attackSpeedAdder
                attackTimer -= Time.deltaTime * (_plant.attackSpeedAdder + 1f);

                if (attackTimer < 0f)
                {
                    // base charge amount = _plant.attackDamage
                    float charge = _plant.attackDamage;

                    // consume LanternPea.attributeCount in chunks, boosting charge
                    if (_plant.attributeCount > 1)
                    {
                        int step = _plant.attributeCount / 10 + 1;
                        _plant.attributeCount -= step;
                        _plant.UpdateText();
                        charge += step * _plant.attackDamage;
                    }

                    // TravelUltimate(0x31) bonus
                    if (Lawnf.TravelUltimate(UltiBuff.EnumValue49))
                    {
                        if (plant.attributeCount < 999)
                            charge += charge * 3f;
                        else
                            charge += charge * 0.5f;
                    }

                    // add to plant.attributeCount scaled by target count
                    int targetCount = attr + 4;
                    plant.attributeCount += (int)(targetCount * charge * 0.1f);

                    // cap or trigger special
                    if (!chao)
                    {
                        if (plant.attributeCount >= max)
                            plant.attributeCount = max;
                    }
                    else
                    {
                        // special UltimatePlantern super charge
                        if (plant is UltimatePlantern up) //<-added mod compatibility
                            up.OnSuperCharge();
                    }

                    plant.UpdateText();
                    attackTimer = 0.2f;
                }

                yield return null;
            }
        }
        public void StartShoot()
        {
            // A: Charging another plant
            if (_plant.targetPlant != null)
            {
                var t = targets[0];

                t.laser.gameObject.SetActive(true);

                var pos = _plant.shoot.position;
                t.laser.SetPosition(0, pos);
                t.laser.SetPosition(1, pos);

                _plant.StartCoroutine(ChargePlant(_plant.targetPlant));
                return;
            }

            // B: Normal multi-beam attack
            foreach (var t in targets)
                _plant.StartCoroutine(AttackZombie(t));

            foreach (var t in targets)
            {
                t.laser.gameObject.SetActive(true);

                var pos = _plant.shoot.position;
                t.laser.SetPosition(0, pos);
                t.laser.SetPosition(1, pos);
            }
        }
        public GameObject SearchZombie()
        {
            var board = _plant.board;
            if (board == null || board.zombieArray == null)
                return null;

            foreach (var zombie in board.zombieArray)
            {
                if (zombie == null)
                    continue;

                if (!CheckZombie(zombie))
                    continue;

                // first valid zombie → return its GameObject
                return zombie.gameObject;
            }

            return null;
        }
        public bool HasAnyTarget()
        {
            if (targets == null || targets.Count == 0)
                return false;

            foreach (var t in targets)
            {
                if (t == null)
                    continue;

                // closest to IL2CPP: “do I still have a live visual?”
                if (t.laser != null && PlantMgr.IsNotNull(t.laser.gameObject, out var a) && a.activeSelf)
                    return true;
            }

            return false;
        }
        public Zombie GetNearestAvailableZombie(Target currentTarget)
        {
            Zombie best = null;
            float bestDist = float.MaxValue;

            var shootPos = _plant.shoot.position;

            foreach (var z in _plant.board.zombieArray)
            {
                // 1. Basic validity + Lantern-style filter
                if (!PlantMgr.IsNotNull(z, out _))
                    continue;

                if (!CheckZombie(z))
                    continue;

                // 2. Skip zombies already targeted by other beams
                bool alreadyUsed = false;
                foreach (var t in targets)
                {
                    if (t == null || t == currentTarget)
                        continue;

                    if (t.zombie == z)
                    {
                        alreadyUsed = true;
                        break;
                    }
                }

                if (alreadyUsed)
                    continue;

                // 3. Distance from shoot point
                float dist = Vector3.Distance(shootPos, z.axis.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = z;
                }
            }

            return best;
        }
        public void OnClicked()
        {
            if (isPF) return;
            EndShoot();                     // stop beams + disable visuals
            StopAllCoroutines();            // stop beam coroutines
            _plant.attributeCountdown = Mathf.Min(_plant.attributeCountdown, 0.1f);
            return;
        }

        public void EndShoot()
        {
            if (_plant.anim != null)
                _plant.anim.SetBool("shooting", false);

            if (_plant.attributeCountdown > 0.15f)
                _plant.attributeCountdown = 0.15f;

            if (targets == null)
                return;

            foreach (var t in targets)
            {
                if (t.laser != null)
                {
                    t.laser.gameObject.SetActive(false);
                }
            }
        }
        public bool CheckPlant(Plant plant)
        {
            if (!PlantMgr.IsNotNull(plant, out var _)) return false;
            if (isPF) return true;

            int front1 = _plant.thePlantColumn + 1;
            if (plant.thePlantColumn <= front1) return false;
            if (Lawnf.TravelAdvanced(AdvBuff.EnumValue3007) && plant.thePlantType == PlantType.UltimatePlantern) return true;
            return plant.thePlantRow == _plant.thePlantRow;
        }
        public bool CheckZombie(Zombie zombie)
        {
            // 0. Null / destroyed
            if (!PlantMgr.IsNotNull(zombie, out _))
                return false;

            // 1. Must not be hypno
            if (zombie.isMindControlled)
                return false;

            // 2. PF override (your plant-specific rule)
            if (isPF)
                return true;

            // 3. Must be in a hittable status (LanternPea uses this)
            if (!Lawnf.InLandStatus(zombie.theStatus))
                return false;

            // 4. Must be same row
            if (zombie.theZombieRow != _plant.thePlantRow)
                return false;

            // 5. World positions
            float zX = zombie.axis.position.x;
            float visionX = _plant.vision;               // LanternPea uses this
            float shootX = _plant.shoot.position.x;     // world X of shoot transform

            // 6. Must be inside [shootX, visionX)
            if (visionX <= zX)
                return false;

            return shootX <= zX && zX != shootX;
        }
    }
}
