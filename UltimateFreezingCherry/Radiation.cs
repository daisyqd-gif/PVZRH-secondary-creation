using UnityEngine;

namespace FreezingCherry.BepInEx
{
    public class IceRadiation : MonoBehaviour
    {
        public static IceRadiation? Instance = null;

        public LayerMask zombieLayer = default;
        public float timer = 0.5f;
        public float lifeTimer = 10f;
        public int damage = 10800;
        public PlantType fromType = UltimateFreezingCherry.PlantID;

        public void Awake()
        {
            zombieLayer = LayerMask.GetMask("Zombie");

            if (Instance != null)
                Instance.Die();

            Instance = this;
        }

        public void Die()
        {
            Destroy(gameObject);
            Instance = null;
        }

        public void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;

            if (timer <= 0.0f)
            {
                timer = 0.5f;

                Vector3 position = transform.position;
                float radius = transform.localScale.x * 2.5f / 3.7f;

                Collider2D[] colliders = Physics2D.OverlapCircleAll(
                    new Vector2(position.x, position.y),
                    radius,
                    zombieLayer.value
                );

                foreach (Collider2D collider in colliders)
                {
                    if (collider.TryGetComponent<Zombie>(out var zombie) && zombie != null)
                    {
                        float damage = this.damage * lifeTimer;

                        if (zombie.theFirstArmor == null)
                        {
                            damage *= 3.0f;
                        }
                        zombie.AddfreezeLevel(100);
                        zombie.TakeDamage((int)damage, null, DamageType.NormalAll, fromType);
                    }
                }
            }
        }

        public void Update()
        {
            lifeTimer -= Time.deltaTime;

            if (lifeTimer <= 0.0f)
            {
                Die();
            }
        }
    }
}