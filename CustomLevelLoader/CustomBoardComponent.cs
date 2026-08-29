namespace CustomLevelLoader
{
    public class CustomBgComponent : MonoBehaviour
    {
        public Board board => gameObject.transform.parent?.GetComponentInParent<Board>();
        public void AnimGive()
        {
            
        }
    }
    public class CustomBoardComponent : MonoBehaviour
    {
        public Board board => gameObject.GetComponent<Board>();
    }
    public class CustomGameLoseComponent : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D collision) {}
    }
}
