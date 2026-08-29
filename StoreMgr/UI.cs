namespace StoreMgr
{
    public class TheCustomButton : MonoBehaviour
        , IPointerEnterHandler
        , IPointerExitHandler
        , IPointerClickHandler
    {
        protected bool _locked = true;
        public GameObject Press
        {
            get => transform.FindChild("Press").gameObject;
        }
        public virtual void OnClicked(){}
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!_locked)
            {
                GameAPP.PlaySound(SoundType.ButtonClick);
                CursorChange.SetClickCursor();
                Press.SetActive(true);
            }
        }
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!_locked)
            {
                CursorChange.SetDefaultCursor();
                Press.SetActive(false);
            }
        }
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!_locked)
            {
                GameAPP.PlaySound(SoundType.GraveButton);
                OnClicked();
            }
        }
    }
    public class CustomStoreButton : CustomUIButton
    {
        public GameObject Lock
        {
            get => transform.FindChild("Locked").gameObject;
        }
        public bool IsLocked
        {
            get => _locked;
            set
            {
                _locked = value;
                transform.FindChild("lock").gameObject.SetActive(_locked);
            }
        }
    }
    public class CustomUIButton : TheCustomButton
    {
        public Action Callback { private get; set; }
        public override void OnClicked()
        {
            Callback();
        }
    }
}
