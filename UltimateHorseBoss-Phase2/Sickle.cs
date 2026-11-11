using CustomPlantClass.Main;
using UnityEngine;

namespace HorseBossPhase2
{
    public class Sickle : CustomParticle
    {
        public int Damage;
        public int row;
        public float cd = 0.1f;
        public LayerMask plantLayer;
        public Transform center;
        public void Awake()
        {
            center = transform.GetChild(0);
        }
        public void FixedUpdate()
        {
            cd -= Time.fixedDeltaTime;
            if(cd<= 0f)
            {
                CreateParticle.SetParticle(Plugin.DataContainer.BombCloudID, center.position, row);
                GameAPP.PlaySound(SoundType.CherryBomb);

                var hits = Physics2D.OverlapCircleAll(center.position, 1.5f, plantLayer);

                foreach (var hit in hits)
                {
                    if (hit != null && hit.TryGetComponent<Plant>(out var p))
                    {
                        p.TakeDamage(Damage, null, DamageType.Curse);
                    }
                }
                cd = 0.1f;
            }

        }
        public void Explode()
        {
        }
    }
}