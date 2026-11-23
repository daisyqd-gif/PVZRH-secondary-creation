namespace CustomPlantClass.Main
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class CreateMgr
    {
        public static Plant SetPlant(int row, int column, PlantType thePlantType, Plant targetPlant = null, Vector2 puffV = default, bool isFreeSet = false, bool withEffect = true, Plant hidplant = null)
        {
            return InstanceManager.CreatePlant.SetPlant(column, row, thePlantType, targetPlant, puffV, isFreeSet, withEffect, hidplant);
        }
        public static Zombie SetZombie(int row, ZombieType theZombieType, bool isMindControlled, float x = 9.9f, bool isIdleOrWithEffect = false)
        {
            if (isMindControlled) return InstanceManager.CreateZombie.SetZombieWithMindControl(row, theZombieType, x, isIdleOrWithEffect);
            return InstanceManager.CreateZombie.SetZombie(row, theZombieType, x, isIdleOrWithEffect);
        }
        public static BombCherry CreateCherryExplode(Vector2 v, int theRow, CherryBombType bombType = CherryBombType.Normal, int damage = 1800, PlantType fromType = PlantType.Nothing, Action<Zombie> action = null, bool immediately = true)
        {
            return InstanceManager.Board.boardAction.CreateCherryExplode(v, theRow, bombType, damage, fromType, action, immediately);
        }
        public static void SetCrater(int row, int column)
        {
            InstanceManager.Board.boardAction.SetPit(column, row);
        }
        public static Doom SetDoom(Board board, Vector2 position, DoomType doomType, Il2CppSystem.Action action = null, bool sprit = true)
        {
            return Doom.SetDoom(board, position, doomType, action, sprit);
        }
        public static void SetDoom(int theColumn, int theRow, bool setPit, bool iceDoom = false, Vector2 position = default, int damage = 1800, int effect = 0, Action<Zombie> action = null, bool existParticle = true, PlantType fromType = PlantType.Nothing)
        {
            InstanceManager.Board.boardAction.SetDoom(theColumn, theRow, setPit, iceDoom, position, damage, effect, action, existParticle, fromType);
        }
        public static void CreateFireLine(int theFireRow, int damage = 1800, bool fromZombie = false, bool fix = false, bool shake = true, Action<Zombie> action = null, PlantType fromType = PlantType.Nothing)
        {
            InstanceManager.Board.boardAction.CreateFireLine(theFireRow, damage, fromZombie, fix, shake, action, fromType);
        }
        public static void CreateMultipleChoiceMenu(List<MultipleChoiceMenuOptions> options, int refreshCount = 9999, int itemCount = 5, bool enableRefresh = true, bool enableTurnPage = false, bool enableCancel = false)
        {
            var menu = GameAPP.UIManager.Push(UIType.MultipleChoiceMenu2).GetComponent<MultipleChoiceMenu>();
            foreach(var option in options)
            {
                menu.RegisterOption(option.Name, option.Text, option.Callback, option.displayPlantType, option.displayZombieType, option.displayQuality, option.Interactable);
            }
            menu.RegisterWindow(itemCount.AtMost(5));
            menu.SetRefreshable(enableRefresh,refreshCount,false,true,enableTurnPage);
            menu.SetCancelable(enableCancel);
        }
    }
    /// <summary>
    /// Options for multiple choice menu items
    /// </summary>
    /// <param name="Name">Name</param>
    /// <param name="Text">Description</param>
    /// <param name="Callback">Called when the item is selected</param>
    /// <param name="displayPlantType">Displayed plant type, can be PlantType.Nothing if displaying a zombie</param>
    /// <param name="displayZombieType">Displayed zombie type, can be ZombieType.Nothing if displaying a zplant</param>
    /// <param name="displayQuality">The quality of the menu item</param>
    /// <param name="Interactable"></param>
    public struct MultipleChoiceMenuOptions(string Name, string Text, Action Callback, PlantType displayPlantType = PlantType.EndoFlame, ZombieType displayZombieType = ZombieType.Nothing, Quality displayQuality = Quality.Default, bool Interactable = true)
    {
        public string Name { get; init; } = Name;
        public string Text { get; init; } = Text;
        public Action Callback { get; init; } = Callback;
        public PlantType displayPlantType { get; init; } = displayPlantType;
        public ZombieType displayZombieType { get; init; } = displayZombieType;
        public Quality displayQuality { get; init; } = displayQuality;
        public bool Interactable { get; init; } = Interactable;
    }
}
