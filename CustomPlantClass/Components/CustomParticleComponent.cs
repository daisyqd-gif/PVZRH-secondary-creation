namespace CustomPlantClass
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CustomParticle : MonoBehaviour
    {
        public void Die() => Destroy(gameObject);
    }
}