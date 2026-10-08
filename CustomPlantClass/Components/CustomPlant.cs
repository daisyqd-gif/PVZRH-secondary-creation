using CustomPlantClass.Runtime.Tasks;

namespace CustomPlantClass
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class BaseCustomPlant : MonoBehaviour, IRedirectAnimShoot, IRedirectAnimShoot2,
        IOverrideDamagePipeline, ICustomClick, ICustomPF, IPlantDieRedirector, IPlantDieHandler,
        IPlantTextHandler
    {
        public bool IsImmune => isPF;
        public bool isPF = false;
        public virtual float maxSliderCount { get; protected set; }
        public float sliderCount = 0f;
        public Plant _plant => GetComponent<Plant>();
        public TextMeshPro extraText;
        public TextMeshPro extraTextShadow;
        private int _pfLockedMaxHealth;
        protected CancellationToken token;
        public virtual void Start()
        {
            if(_plant == null)
            {
                Destroy(this);
                return;
            }
            _plant.shoot = GetShoot();
            OnSpawn();
            Start_Async();
        }
        private async void Start_Async()
        {
            try
            {
                if(_plant == null)
                {
                    return;
                }
                await DelayTask.WaitForFixedUpdate(token);
                if(_plant == null)
                {
                    return;
                }
                _pfLockedMaxHealth = _plant.thePlantMaxHealth;
            }
            catch( Exception e )
            {
                ModLogger.LogError(e.ToString());
            }
        }
        public virtual void Awake()
        {
            token = new();
            if(_plant == null)
            {
                Destroy(this);
                return;
            }
        }
        public virtual void OnDestroy()
        {
            token.Cancel();
        }
        /// <summary>
        /// Called one frame after the object is created, original method is empty to allow for skipping the base call
        /// </summary>
        public virtual void OnSpawn() { }
        /// <summary>
        /// Called when killed, original method is empty to allow for skipping the base call
        /// </summary>
        public virtual void OnDie(DieReason reason) { }
        public virtual void Update()
        {
            OnUpdate();
        }
        /// <summary>
        /// Called every physics frame, original method is empty to allow for skipping the base call
        /// </summary>
        public virtual void OnUpdate() { }
        /// <summary>
        /// Sets the plant's custom text style
        /// </summary>
        public virtual void SetTextStyle(TextMeshPro text)
        {
            text.fontSize = 2.1f;
        }
        /// <summary>
        /// Sets the plant's custom text color
        /// </summary>
        public virtual Color SetTextColor() => Color.cyan;
        /// <summary>
        /// Sets the plant's custom text size
        /// </summary>
        public virtual Vector2? GetTextSize() => null;
        /// <summary>
        /// Sets the plant's custom text, called every frame to update the text
        /// </summary>
        public virtual string GetTextString() => "";
        /// <summary>
        /// Shows up in the plant data menu
        /// </summary>
        public virtual List<KeyValuePair<string, string>> GetLiveInfo()
        {
            return new List<KeyValuePair<string, string>>();
        }
        public virtual void InitText()
        {
            Color color = SetTextColor();
            _plant.RegisterText(color, GetTextString, GetTextSize());
        }
        /// <summary>
        /// Locates the plant's shoot transform (can be empty for some plants that don't need it)
        /// </summary>
        public virtual Transform FindShoot()
            => _plant.transform.FindChild(GetShootPath());

        /// <summary>
        /// Gets the plant's shoot path from root
        /// </summary>
        public virtual string GetShootPath() => "Shoot";

        public Transform GetShoot()
        {
            var s = FindShoot();
            return s != null ? s : _plant.transform;
        }

        /// <summary>
        /// Gets the plant's bullet type (defaulted for plants registered through datamgr)
        /// </summary>
        public virtual BulletType GetBulletType()
        {
            if (DataMgr.plantBulletTypes.TryGetValue(_plant.thePlantType, out var a))
            {
                return a;
            }
            else
            {
                return BulletType.Bullet_pea;
            }
        }
        /// <summary>
        /// Gets the plant's alternate bullet type (calls GetBulletType() if not overriden)
        /// </summary>
        public virtual BulletType GetBulletType2() => GetBulletType();

        /// <summary>
        /// Gets the plant's bullet move way
        /// </summary>
        public virtual BulletMoveWay GetBulletMoveWay() => BulletMoveWay.MoveRight;
        /// <summary>
        /// Gets the plant's alternate move way (calls GetBulletMoveWay() if not overriden)
        /// </summary>
        public virtual BulletMoveWay GetBulletMoveWay2() => GetBulletMoveWay();
        public virtual BulletMoveWay GetBulletMoveWayPF_SuperGatling()
        {
            if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) return BulletMoveWay.MoveRight;
            return BulletMoveWay.SuperGatling;
        }
        public int GetDamage(int damage, IDamageMaker damageFrom, DamageType damageType)
        {
            if (isPF) return 0;
            var dmg = OnTakeDamage(damage, damageFrom, damageType);
            if (DamageLimit != -1) dmg = Mathf.Clamp(dmg, 0, DamageLimit);
            dmg = Mathf.RoundToInt(dmg * (1f - DamageReductionPercent / 100f));
            return OnTakeDamage(dmg, damageFrom, damageType);
        }
        public virtual int AttackDamage => _plant.attackDamage;
        public virtual int AttackDamage2 => AttackDamage;

        public Bullet Shoot1()
        {
            Bullet b = Shoot_Custom();
            EventManager.TriggerEvent(GameEvent.OnPlantShoot, _plant);
            return b;
        }
        public Bullet Shoot2() => Shoot2_Custom();

        protected virtual float DamageReductionPercent { get; } = 0f;
        protected virtual int DamageLimit { get; } = -1;
        protected virtual bool OverrideDamagePipeline { get; } = false;
        internal bool ovr_dmg => OverrideDamagePipeline;
        public virtual int OnTakeDamage(int damage, IDamageMaker damageFrom, DamageType damageType)
        {
            return damage;
        }
        public virtual bool CanBeCrashed { get => true; }
        public virtual bool CanDie { get => true; }
        public virtual bool CanBeFrozen { get => true; }

        public virtual Bullet Shoot_Custom()
        {
            Bullet b = PlantMgr.SetBullet(_plant, GetBulletType(), GetBulletMoveWay(), AttackDamage); //applies fields automatically
            int soundId = Random.Range(3, 5);
            GameAPP.PlaySound(soundId, 0.5f, 1.0f);
            return b;
        }

        public virtual Bullet Shoot2_Custom()
        {
            Bullet b = PlantMgr.SetBullet(_plant, GetBulletType2(), GetBulletMoveWay2(), AttackDamage2); //applies fields automatically
            int soundId = Random.Range(3, 5);
            GameAPP.PlaySound(soundId, 0.5f, 1.0f);
            return b;
        }

        public virtual void AnimShoot_Custom() => Shoot1();
        public virtual void AnimShoot2_Custom() => Shoot2();
        public virtual void FixedUpdate()
        {
            if (GameAPP.theGameStatus == GameStatus.InGame && Board.Instance != null && _plant != null && _plant.healthSlider != null)
            {
                _plant.healthSlider.ProgressFill = sliderCount;
                _plant.healthSlider.progressMaxValue = maxSliderCount;
                if (isPF)
                {
                    if (_plant.thePlantMaxHealth <= _pfLockedMaxHealth) _plant.thePlantMaxHealth = _pfLockedMaxHealth;
                    else if (_plant.thePlantMaxHealth >= _pfLockedMaxHealth) _pfLockedMaxHealth = _plant.thePlantMaxHealth;
                    _plant.thePlantHealth = _plant.thePlantMaxHealth;
                    _plant.flashCountDown = 10f;
                    _plant.UpdateText();
                    _plant.isCrashed = false;
                    if (_plant.TryGetEffect<PlantCurseEffect>(EffectType.Curse, out var _))
                    {
                        _plant.RemoveBuff(EffectType.Curse);
                    }
                    if (_plant.TryGetEffect<PlantFragileEffect>(EffectType.Fragile_plant, out var _))
                    {
                        _plant.RemoveBuff(EffectType.Fragile_plant);
                    }
                    if (_plant.TryGetEffect<PlantFragileEffect>(EffectType.PortalBalloon, out var _))
                    {
                        _plant.RemoveBuff(EffectType.PortalBalloon);
                    }
                    _plant.disableCount = 0;
                }
            }
            OnFixedUpdate();
        }
        public virtual void OnFixedUpdate() { }
        protected virtual bool IsAsyncPF => false;
        protected bool _prevUncrashable=false;
        public virtual void StartPF()
        {
            _plant.invincible = true;
            _prevUncrashable = _plant.uncrashable;
            _plant.uncrashable = true;
            isPF = true;
            _plant.isFlashing = true;

            if (IsAsyncPF) _ = PFWrapper_Async();
            // Wrap the coroutine so we can detect when it finishes
            else _plant.StartCoroutine(PFWrapper());
        }

        protected IEnumerator PFWrapper()
        {
            // Run the user‑overridable supershoot
            yield return SuperShoot();

            // When it finishes, call the overridable end hook
            SuperEnd();
        }

        // Overridable supershoot logic
        public virtual IEnumerator SuperShoot()
        {
            yield return null;
        }

        // Overridable end hook
        public virtual void SuperEnd()
        {
            _plant.invincible = false;
            _plant.uncrashable = _prevUncrashable;
            isPF = false;
            _plant.flashCountDown = 0f;
            _plant.isFlashing = false;
        }
        protected async Task PFWrapper_Async()
        {
            try
            {
                await SuperShoot_Async();
                SuperEnd();
            }
            catch( Exception e )
            {
                ModLogger.LogError(e.ToString());
            }
        }
        protected virtual async Task SuperShoot_Async()
        {
            await Task.Delay(1);
        }
        public virtual bool CanClick => false;
        public virtual void OnClicked(Mouse mouse)
        {

        }
    }
}