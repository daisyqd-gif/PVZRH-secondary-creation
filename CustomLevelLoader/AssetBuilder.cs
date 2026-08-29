using Unity.VisualScripting;
using UnityEngine.Rendering;

namespace CustomLevelLoader
{
    public class AssetBuilder : MonoBehaviour
    {
        public static AssetBundle GetAssetBundle(Assembly assembly, string name)
        {
            try
            {
                // Find the actual resource name
                string resourceName = assembly
                    .GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase));

                if (resourceName == null)
                {
                    Debug.LogError($"AssetBundle '{name}' not found in assembly resources.");
                    return null;
                }

                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                using MemoryStream ms = new();
                stream.CopyTo(ms);

                var ab = AssetBundle.LoadFromMemory(ms.ToArray());
                if (ab == null)
                {
                    Debug.LogError($"AssetBundle '{name}' failed to load.");
                    return null;
                }

                Debug.Log($"AssetBundle '{name}' loaded successfully!");
                return ab;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading AssetBundle '{name}':\n{e}");
                return null;
            }
        }
        public static void ApplySprite(Board board, Sprite sprite)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));
            if (sprite == null)
                throw new ArgumentNullException(nameof(sprite));

            Transform bg=board.background.transform.Find("bg");
            GameObject watermark = Instantiate(Core.assetBundle.GetAsset<GameObject>("Watermark"));
            watermark.transform.SetParent(bg);
            watermark.transform.localPosition=new Vector3(2.719f,-0.871f,0f);
            watermark.transform.localScale=new Vector3(0.75f,0.75f,1f);
            watermark.GetComponent<SortingGroup>().sortingOrder=-100;
            bg.Find("bg").GetComponent<SpriteRenderer>().sprite=sprite;
        }
        public static void ApplySprite<TBoard, TBG>(Board board, Sprite sprite) where TBoard : CustomBoardComponent where TBG : CustomBgComponent
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));
            if (sprite == null)
                throw new ArgumentNullException(nameof(sprite));

            Transform bg=board.background.transform.Find("bg");
            bg.parent.GetOrAddComponent<TBoard>();
            bg.GetOrAddComponent<TBG>();
            GameObject watermark = Instantiate(Core.assetBundle.GetAsset<GameObject>("Watermark"));
            watermark.transform.SetParent(bg);
            watermark.transform.localPosition=new Vector3(2.719f,-8.871f,0f);
            watermark.transform.localScale=new Vector3(0.75f,0.75f,1f);
            watermark.GetComponent<SortingGroup>().sortingOrder=-100;
            bg.Find("bg").GetComponent<SpriteRenderer>().sprite=sprite;
        }
    }
}