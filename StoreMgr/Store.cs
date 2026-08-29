namespace StoreMgr
{
    public class StoreItem : MonoBehaviour
        , IPointerEnterHandler
        , IPointerExitHandler
    {
        public int ItemID { private get; set; } = -1;
        public StoreItemEntry ItemEntry { private get; set; }
        public GameObject Glow => transform.FindChild("SelectGlow").gameObject;
        public Sprite Icon
        {
            get;
            set
            {
                field = value;
                transform.FindChild("Iconbank/icon").GetComponent<Image>().sprite = field;
            }
        }
        public CustomUIButton BuyButton => transform.FindChild("button").GetComponent<CustomUIButton>();
        public string Title
        {
            get;
            set
            {
                field = value;
                transform.FindChild("Title").GetComponent<TextMeshProUGUI>().text = field;
            }
        }
        public bool IsLocked
        {
            get;
            set
            {
                field = value;
                transform.FindChild("lock").gameObject.SetActive(value);
            }
        }
        public int Cost
        {
            get;
            set
            {
                field = value;
                BuyButton.transform.FindChild("Cost").GetComponent<TextMeshProUGUI>().text = field.ToString();
            }
        }
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            Glow.SetActive(true);
        }
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            Glow.SetActive(false);
        }
        public void Init(StoreItemEntry ItemEntry, StoreUI ui)
        {
            Icon=ItemEntry.Icon;
            Title=ItemEntry.Title;
            Cost=ItemEntry.Cost;
            BuyButton.Callback= ItemEntry.Use;
        }
    }
    public class StoreItemMenu : MonoBehaviour
    {
        public int ItemID { private get; set; } = -1;
        private Sprite _icon;
        public Sprite Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                transform.FindChild("Iconbank/icon").GetComponent<Image>().sprite = _icon;
            }
        }
        private string _title = "";
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                transform.FindChild("Name").GetComponent<TextMeshProUGUI>().text = _title;
            }
        }
        private string _desc = "";
        public string Description
        {
            get => _desc;
            set
            {
                _desc = value;
                transform.FindChild("Description").GetComponent<TextMeshProUGUI>().text = _desc;
            }
        }
        private int _cost = 0;
        public int Cost
        {
            get => _cost;
            set
            {
                _cost = value;
                transform.FindChild("Cost/GameObject").GetComponent<TextMeshProUGUI>().text = value.ToString();
            }
        }
        public CustomUIButton CancelButton => transform.FindChild("BackButton").GetComponent<CustomUIButton>();
        public CustomStoreButton BuyButton => transform.FindChild("BuyButton").GetComponent<CustomStoreButton>();
        public void Init(StoreItemEntry ItemEntry)
        {
            Icon=ItemEntry.Icon;
            Title=ItemEntry.Title;
            Description=ItemEntry.Description;
            Cost=ItemEntry.Cost;
            BuyButton.Callback= ItemEntry.Use;
            CancelButton.Callback = () => Destroy(gameObject);
        }
        public void OnDestroy()
        {
            
        }
    }
}