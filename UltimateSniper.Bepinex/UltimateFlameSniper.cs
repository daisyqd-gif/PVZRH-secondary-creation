namespace UltimateSniper
{
    public class UltimateFlameSniper : MonoBehaviour, IPlantTextHandler
    {
        public int hitCount = 0;
        public FireSniper plant => gameObject.GetComponent<FireSniper>();
        public void Awake() => plant.shoot=transform.FindChild("PeaShooter_Head/gun_lower/Shoot");
        public void Start()
        {
            // Heart is now child index 0
            Transform heart = plant.transform.GetChild(0);

            plant.ac = heart.gameObject;
            plant.ac.SetActive(false);

            // SortingGroup
            var sg = plant.ac.GetComponent<SortingGroup>();
            if (sg == null)
                sg = plant.ac.AddComponent<SortingGroup>();

            sg.sortAtRoot = true;
            plant.r = sg;
        }
        public void AttackZombie(Zombie zombie,int damage)
        {
            if (hitCount >= (Lawnf.TravelUltimate(Plugin.Buff2) ? 125 : 250) || plant.starUp)
            {
                if (Lawnf.TravelUltimate(UltiBuff.EnumValue50) || plant.starUp) plant.anim.SetTriggerString("shoot");
                damage*=6;
                for(int i = 0 ; i < 360; i += 10)
                {
                    GameObject particle_2 = CreateParticle.SetParticle(Plugin.ParticleID, plant.shoot.position, 11);
                    particle_2.transform.Rotate(0,0,i);
                }
                Zombie[] z_list=[..Lawnf.GetAllZombies()];
                foreach (var z in z_list)
                {
                    int dmg = damage;
                    z.SetPortaled();
                    z.AddBurn();
                    z.TakeDamage(DamageType.NormalAll, dmg, plant.thePlantType);
                }
                hitCount = 0;
            }
            Vector2 dir = zombie.col.bounds.center - plant.shoot.position;
            GameObject particle = CreateParticle.SetParticle(Plugin.ParticleID, plant.shoot.position, 11);
            particle.transform.rotation = MathHelper.DirectionToRotation(dir);

            RaycastHit2D[] hits = Physics2D.RaycastAll(plant.shoot.position, dir, float.PositiveInfinity, LayerMask.GetMask("Zombie"));

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.TryGetComponent<Zombie>(out var z))
                {
                    int dmg = damage;
                    z.SetPortaled();
                    z.AddBurn();
                    z.TakeDamage(DamageType.NormalAll, dmg, plant.thePlantType);
                    hitCount++;
                }
            }
            //zombie.TakeDamage(damage,plant,DamageType.NormalAll,plant.thePlantType);
            
        }
        public void InitText()
        {
            Color color = Color.cyan;
            plant.RegisterText(color, GetTextString, null);
        }
        public string GetTextString() => "充能:" + hitCount;
    }
}