namespace StoreMgr
{
    public class BagItem
    {
        public Sprite Icon { get; set; } = new();
        public int SellPrice { get; set; } = 0;
        public string Title = "";
        public string Description = "";
        public Func<bool> CanUse { get; set; } = () => true;
        public Action<Board> UseInLevel { get; set; } = null;
        public BagItem()
        {
        }
        public BagItem(StoreItemEntry item)
        {
            Icon = item.Icon;
            SellPrice = item.Cost >= 2000 ? item.Cost - 2000 : 0;
            Title = item.Title;
            Description = item.Description;
            CanUse = item.CanUse;
            UseInLevel = item.UseInLevel;
        }
    }
    public class StoreItemEntry
    {
        public Sprite Icon { get; set; } = new();
        public int Cost { get; set; } = 0;
        public string Title = "";
        public string Description = "";
        public Func<bool> CanBuy { get; set; } = () => true;
        public Func<bool> CanUse { get; set; } = () => true;
        public Action UseOutLevel { get; set; } = () => { };
        public Action<Board> UseInLevel { get; set; } = null;
        public bool IsInGameItem = false;
        public void Use()
        {
            if (Board.Instance != null && (GameAPP.theGameStatus == GameStatus.InGame || GameAPP.theGameStatus == GameStatus.Pause) && UseInLevel != null && IsInGameItem)
            {
                UseInLevel(Board.Instance);
            }
            else if (IsInGameItem)
            {
                BagItem stored = new BagItem(this);
            }
            else
            {
                UseOutLevel();
            }
        }
    }
    public class StoreSaveMgr
    {

    }
}