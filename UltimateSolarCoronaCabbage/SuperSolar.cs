namespace UltimateSolarCoronaCabbage_Remade
{
    public class SuperSolar : MonoBehaviour
    {
        public static SuperSolar Instance;

        public Board board;
        public float getsuncd = 0.25f;
        public float deathTime = 31f;     // You keep your longer life
        public float timer = 0.5f;        // Attack pulse timer
        public float godTimer = 3f;       // Item spawn timer
        public bool arrived = false;
        public Vector3 targetPosition;

        public void Awake()
        {
            // Singleton logic
            if (Instance != null)
            {
                Destroy(gameObject);
                Instance.deathTime += 15f;
                return;
            }

            Instance = this;

            board = Board.Instance;
            if (board == null)
            {
                Destroy(gameObject);
                return;
            }

            // Match base-game Solar visual logic
            if (GameAPP.config.disableSolarStarEffect)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(true);
            }

            // Remove base-game Solar if still alive
            if (Solar.Instance != null)
                Destroy(Solar.Instance.gameObject);
        }

        public void Update()
        {
            // -----------------------------
            // 1. No smooth movement
            // -----------------------------
            transform.position = targetPosition;
            arrived = true;

            if (!arrived || GameAPP.theGameStatus != GameStatus.InGame)
                return;

            // -----------------------------
            // 2. Timers
            // -----------------------------
            timer -= Time.deltaTime;
            godTimer -= Time.deltaTime;
            deathTime -= Time.deltaTime;

            // -----------------------------
            // 3. Attack pulse (base game)
            // -----------------------------
            if (timer <= 0f && timer != 0f)
            {
                timer = 0.5f;

                if (board != null && board.zombieArray != null)
                {
                    int dmg = Mathf.Clamp(180+40*(Lawnf.GetPlantCount(PlantType.UltimateCabbage, board)+Lawnf.GetPlantCount(Plugin.DataContainer.thePlantType, board)), 0, 46339); // base Solar adds +60 to its damage
                    dmg*=dmg;
                    if (dmg < 238609294)
                    {
                        // UltiBuff 人造太阳 (ID 23)
                        if (Lawnf.TravelUltimate(UltiBuff.EnumValue23))
                            dmg *= 3;

                        // Sun > 15000 → triple damage + consume 200 sun
                        if (board.theSun > 15000)
                        {
                            board.UseSun(200f);
                            dmg *= 3;
                        }
                    }

                    // Apply AoE damage to all zombies
                    var zombies = board.zombieArray;
                    int count = zombies._size;

                    while (count-- > 0)
                    {
                        var obj = zombies[count];
                        if (obj != null && !obj.IsDestroyed() && !obj.isMindControlled && !obj.beforeDying)
                        {
                            // Base-game Solar damage call
                            obj.TakeDamage(
                                DmgType.Shieldless,
                                dmg,
                                Plugin.DataContainer.thePlantType    // plant ID 934 (0x3A6)
                            );
                        }
                    }
                }
            }

            // -----------------------------
            // 4. Death-time VFX logic
            // -----------------------------
            if (deathTime <= 0f)
            {
                Destroy(gameObject);
                Instance = null;
                return;
            }

            if (getsuncd <= 0f)
            {
                getsuncd=0.25f;
                board.SetSun(1000);
            }
            else
            {
                getsuncd-=Time.deltaTime;
            }

            // -----------------------------
            // 5. God-timer item spawn (base game)
            // -----------------------------
            if (godTimer <= 0f)
            {
                godTimer = 3f;

                if (board == null)
                    return;

                if (God())
                {
                    Instantiate(GameAPP.itemPrefab[47], board.transform);
                }
            }

            // -----------------------------
            // 6. Cleanup
            // -----------------------------
            if (Solar.Instance != null)
                Destroy(Solar.Instance.gameObject);

            if (Instance != this)
                Destroy(gameObject);
        }
        bool God()
        {
            // AdvBuff 3001 (0xBB9) – 星神合一
            if (!Lawnf.TravelAdvanced(AdvBuff.EnumValue3001))
                return false;

            int countA = Lawnf.GetPlantCount(PlantType.UltimateStar, board);
            int countB = Lawnf.GetPlantCount(PlantType.UltimateBlover, board);
            int countC = Lawnf.GetPlantCount(PlantType.UltimateCabbage, board);
            int countD = Lawnf.GetPlantCount(Plugin.DataContainer.thePlantType, board);

            if (countA + countB > 9)
                return countC+countD > 9;

            return false;
        }
    }
}
