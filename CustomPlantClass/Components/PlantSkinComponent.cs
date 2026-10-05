namespace CustomPlantClass
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class PlantSkinComponent : MonoBehaviour
    {
        public Plant plant => GetComponent<Plant>();
    }
    public class BulletComponent : MonoBehaviour
    {
        public Bullet bullet => GetComponent<Bullet>();
    }
    public class ZombieComponent : MonoBehaviour
    {
        public Zombie zombie => GetComponent<Zombie>();
    }
}