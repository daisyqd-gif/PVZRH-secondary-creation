using CustomPlantClass.Runtime.Tasks;

namespace CustomPlantClass.Main
{
    /// <summary>
    /// Various tools for custom behaviour involving zombies
    /// </summary>
    public static class ZombieMgr
    {
        private static Vector3 cornerpos = new();
        /// <summary>
        /// Makes a newspaper zombie lose its newspaper.
        /// </summary>
        /// <param name="self">The affected zombie</param>
        public static void LosePaper(this PaperZombie self)
        {
            self.theSecondArmorHealth = 0;
            self.TakeDamage(1, self, DamageType.Normal); //It wants an Idamagemaker so I put itself in
            self.SecondArmorFall();
        }
        /// <summary>
        /// Makes a newspaper zombie lose its newspaper.
        /// </summary>
        /// <param name="self">The affected zombie</param>
        public static void LosePaper(this GatlingPaperZombie_a self)
        {
            self.theSecondArmorHealth = 0;
            self.TakeDamage(1, self, DamageType.Normal); //It wants an Idamagemaker so I put itself in
            self.SecondArmorFall();
        }
        /// <summary>
        /// Instantly kills a zombie and makes it fly away
        /// </summary>
        /// <param name="self">The zombie to fly away</param>
        public static void FlyAway(this Zombie self)
        {
            if (self == null) return;

            GameObject preview = CreateZombie.CreateZombiePreview(
                self.theZombieType,
                Color.white,
                self.board.transform,
                self.axis.position
            );

            self.Die(2);
            Vector3 target;
            try
            {
                // get bg SpriteRenderer
                SpriteRenderer sr = self.board.background.transform.Find("bg").Find("bg").GetComponent<SpriteRenderer>();

                // compute top-right corner
                Vector2 size = sr.sprite.bounds.size;
                Vector3 topRightLocal = new Vector3(size.x / 2f, size.y / 2f, 0f);
                Vector3 topRightWorld = sr.transform.TransformPoint(topRightLocal);
                target = topRightWorld + new Vector3(5f, 5f, 0f);
                cornerpos = target;
            }
            catch (NullReferenceException)
            {
                target = cornerpos;
            }
            // start coroutine
            preview.AddComponent<CustomParticle>().StartCoroutine(FlyAwayRoutine(preview, target));
            static IEnumerator FlyAwayRoutine(GameObject obj, Vector3 target)
            {
                float duration = 1.5f;       // total travel time
                float elapsed = 0f;
                float maxSpeed = 20f;        // speed at the end
                float spinSpeed = 1440f;      // degrees per second

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);

                    // slow → fast acceleration curve
                    float speed = maxSpeed * t;

                    // movement
                    obj.transform.position = Vector3.MoveTowards(
                        obj.transform.position,
                        target,
                        speed * Time.deltaTime
                    );

                    // spin (constant or accelerating)
                    obj.transform.RotateAround(obj.transform.TransformPoint(obj.GetCenterLocalSprite()), new Vector3(0f, 0f, 1f), spinSpeed * Time.deltaTime);

                    yield return null;
                }

                obj.transform.position = target;
                Object.Destroy(obj);
            }
        }
        /// <summary>
        /// Instantly kills a zombie and crushes it
        /// </summary>
        /// <param name="self">The zombie to be crushed</param>
        public static void Crashed(this Zombie self)
        {
            if (self == null) return;

            GameObject preview = CreateZombie.CreateZombiePreview(
                self.theZombieType,
                Color.white,
                self.board.transform,
                self.axis.position
            );

            self.Die(2);

            preview.transform.localScale = Vector3.Scale(
                preview.transform.localScale,
                new Vector3(1f, 0.1f, 1f)
            );
            preview.transform.localPosition -= new Vector3(0f, 0.5f, 0f);
            preview.AddComponent<CustomParticle>().StartCoroutine(DieRoutine(preview));
            static IEnumerator DieRoutine(GameObject obj)
            {
                float duration = 1.5f;       // total travel time
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                Object.Destroy(obj);
            }
        }
        /// <summary>
        /// Stuns a zombie
        /// </summary>
        public static void StunZombie(this Zombie self, float timeInSeconds = 4, Action<Zombie> onStun = null, Action<Zombie> done = null, Vector2 offset = default)
        {
            onStun = onStun ?? ((z) =>
            {
            self.ChangeStatus(ZombieStatus.Default);
            GameAPP.PlaySound(SoundType.Bonk);
                z.anim.CrossFade("idle", 0.2f);
                z.theFirstArmor?.SetActive(false);
                z.theSecondArmor?.SetActive(false);
                CreateParticle.SetParticle(107, self.transform.position + Vector3.up, 0, setLayer: false);
            });
            done = done ??((z) => z.anim.CrossFade("walk", 0.2f));
            if(offset == default)
            {
                offset = new(0,1.5f);
            }
            StunZombie_async(self,timeInSeconds,onStun,done,offset);
        }
        private static async void StunZombie_async(Zombie self, float time, Action<Zombie> onStun, Action<Zombie> done, Vector2 offset)
        {
            if (self == null)
            {
                return;
            }

            var cancellationToken = self.CreateCancellationToken();
            onStun(self);

            await DelayTask.WaitForFixedUpdate();

            Vector2 dizzinessPosition = self.transform.position;
            dizzinessPosition += offset;
            ParticleManager.Instance.SetParticle(ParticleType.Dizziness, dizzinessPosition, 0, false, 1f);

            await DelayTask.DelayScaled(
                time,
                () => Time.timeScale,
                cancellationToken
            );

            done(self);
        }
        internal static void TrySetDTierZombies(Board board, int theWave, int theRound, List<ZombieType> zombieTypes)
        {
            if (theRound >= 13 && board.boardTag.isRogue && board.boardTag.isTravel || theWave > 80 && board.boardTag.rogueShooting)
            {
                foreach (var i in DataMgr.Level4Zombies)
                {
                    if (zombieTypes.Contains(i.Key))
                    {
                        InitZombieList.AddZombieToList(i.Value, theWave);
                    }
                }
            }
        }
        /// <summary>
        /// Gets the closest zombies closest to a position
        /// </summary>
        public static IEnumerable<Zombie> GetClosestZombies(int amount, Vector2 origin, bool containsMindControlled = false, Func<Zombie,bool> selector = default)
        {
            List<(float,Zombie)> zombies = new();
            Func<Zombie,bool> select = null;
            if(selector == null)  select = (z) => containsMindControlled || !z.isMindControlled;
            else select = (z) => (containsMindControlled || !z.isMindControlled) && selector(z);
            foreach( var i in Lawnf.GetAllZombies(select))
            {
                float dist = Vector2.Distance(i.axis.position,origin);
                zombies.Add((dist,i));
            }

            if (zombies.Count <= amount)
                return zombies.Select(t => t.Item2);

            return zombies
                .OrderBy(t => t.Item1)
                .Take(amount)
                .Select(t => t.Item2);
        }
    }
}