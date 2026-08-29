using System;

namespace UltimateSniper
{
    public class FireMeteor : MonoBehaviour
    {
        public float g = -9.8f;
        public float minY;
        public float speedX = 7f;
        public float speedY = 0f;
        public bool zombie = false;
        public int fromType = 0;
        public int _damage = 0;
        public static bool exists = false;

        private Board board;

        public void Start()
        {
            board = Board.Instance;
            if (board == null)
            {
                Destroy(gameObject);
                return;
            }

            // Zombie variant (IL accurate)
            if (zombie)
            {
                speedX = -speedX;
                transform.position = new Vector3(15f, 9f, 0f);
            }

            // Play spawn sound (0x5F)
            GameAPP.PlaySound(95, 1f, 1f);

            // IL-accurate minY calculation
            if (board.rowNum % 2 == 0)
            {
                float y1 = Mouse.Instance.GetBoxYFromRow(board.rowNum / 2 - 1);
                float y2 = Mouse.Instance.GetBoxYFromRow(board.rowNum / 2);
                minY = (y1 + y2 + 1f) * 0.5f;
            }
            else
            {
                float yMid = Mouse.Instance.GetBoxYFromRow(board.rowNum / 2);
                minY = yMid + 0.5f;
            }
            exists = true;
        }

        public void Update()
        {
            if (board == null) Destroy(gameObject);
            // Gravity
            speedY += g * Time.deltaTime;

            // Movement
            transform.position += new Vector3(speedX, speedY, 0f) * Time.deltaTime;

            // IL-accurate rotation (360°/sec)
            transform.Rotate(0f, 0f, 180f * Time.deltaTime);

            // Landing detection (IL accurate)
            if (transform.position.y <= minY)
            {
                Crash();
            }
        }

        public static GameObject SetStar()
        {
            if (Board.Instance == null) return null;

            var go = Instantiate(Plugin.FireMeteor2, Board.Instance.transform);

            // IL-accurate spawn: above board, same X as Big Star
            go.transform.position = new Vector3(-9f, 13f, 0f);
            return go;
        }

        public void Crash()
        {
            // Destroy meteor object (IL accurate)
            exists = false;
            try
            {
                CreateParticle.SetParticle(Plugin.Doom_Big_Fire_ID, transform.position, 2);
                // Custom damage formula
                int damage = Mathf.CeilToInt(3600f * Mathf.Max(1f,
                    Mathf.Pow(2f, _damage / 50f)) * UltimateMegaGatlingPea.Multiplier);
                UltimateMegaGatlingPea.Multiplier = 1;
                // Damage ALL enemy zombies
                var zombies = Board.Instance.zombieArray;
                if (zombies != null)
                {
                    for (int i = zombies.Count - 1; i >= 0; i--)
                    {
                        
                        var z = zombies[i];
                        if (z == null || z.theStatus == ZombieStatus.Dying) continue;
                        if (z.isMindControlled) continue;
                        if (z.theMaxHealth <= 1000) { z.Die(); continue; }
                        int dmg=damage;
                        z.SetPortaled(30f); //bosses are immune
                        // PortalEffect bonus
                        if (z.TryGetEffect<PortalEffect>(EffectType.Portal, out var portal) && portal.duration > 0)
                            dmg += 1500;
                        // Boss multiplier
                        if (TypeMgr.IsBossZombie(z.theZombieType))
                            dmg *= 3;
                        // Ulti buff multiplier
                        if (Lawnf.TravelAdvanced(Plugin.Buff1))
                            dmg *= 3;
                        z.SetJalaed(); //Custom logic
                        z.SetPortaled(3f);
                        z.TakeDamage(DmgType.Carred, dmg);
                        Burn.TryAddZombieBurn(z, true);
                    }
                }
                ScreenShake.TriggerShake(0.15f);
            }
            catch (Exception) { }
            CreateCustomStars();
            Destroy(gameObject);
        }

        public void CreateCustomStars()
        {
            bool hasUltiBuff = Lawnf.TravelAdvanced(Plugin.Buff1);

            int bulletCount = hasUltiBuff ? 90 : 30;
            int angleStep = hasUltiBuff ? 4 : 12;
            Vector3 center = transform.position;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = i * angleStep;

                Bullet bullet = CreateBullet.Instance.SetBullet(
                    center.x,
                    center.y,
                    2,
                    UltimateExplosivePea.BULLET_ID,
                    BulletMoveWay.Free,
                    false
                );
                bullet.normalSpeed *= 3;

                if (bullet != null)
                {
                    bullet.transform.Rotate(0, 0, angle);
                }
            }
        }
    }
}
