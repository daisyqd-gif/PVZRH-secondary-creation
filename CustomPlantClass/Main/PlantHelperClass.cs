using CustomPlantClass.Runtime.Tasks;

namespace CustomPlantClass.Main
{
    /// <summary>
    /// Various tools for custom behaviour
    /// </summary>
    public class PlantMgr : MonoBehaviour
    {
        //Position helpers
        /// <summary>Gets a random board row, or zero if no board is available.</summary>
        /// <returns>A random row</returns>
        public static int GetRandomBoardRow()
        {
            Board board = Instance;
            if (board == null)
            {
                return 0;
            }
            return Random.Range(0, board.rowNum);
        }
        /// <summary>Gets a random board column, or zero if no board is available.</summary>
        /// <returns>A random column</returns>
        public static int GetRandomBoardColumn()
        {
            Board board = Instance;
            if (board == null)
            {
                return 0;
            }
            return Random.Range(0, board.columnNum);
        }
        /// <summary>Gets the horizontal board position for a column.</summary>
        /// <param name="column">A column</param>
        /// <returns>The x value of the column</returns>
        public static float getX(int column)
        {
            return Mouse.Instance.GetBoxXFromColumn(column);
        }
        /// <summary>Gets the vertical board position for a row.</summary>
        /// <param name="row">A row</param>
        /// <returns>The y value of the row</returns>
        public static float getY(int row)
        {
            return Mouse.Instance.GetBoxYFromRow(row);
        }
        /// <summary>Finds the board column containing the specified horizontal position.</summary>
        /// <param name="x">The x value of the column</param>
        /// <returns>A column</returns>
        public static int getCol(float x)
        {
            return Mouse.Instance.GetColumnFromX(x);
        }
        /// <summary>Finds the board row containing the specified vertical position.</summary>
        /// <param name="y">The y value of the row</param>
        /// <returns>A row</returns>
        public static int getRow(float y)
        {
            return Lawnf.GetRowFromY(y);
        }
        /// <summary>Gets the board position at the specified row and column.</summary>
        /// <param name="row">The board row.</param>
        /// <param name="column">The board column.</param>
        /// <returns>The world position of the board cell.</returns>
        public static Vector2 GetPos(int row, int column)
        {
            Vector2 pos = new Vector2(getX(column), getY(row));
            return pos;
        }

        //Meteor creator
        /// <summary>Creates a meteor star using the supplied prefab.</summary>
        /// <param name="customMeteorPrefab">The prefab to use for the meteor.</param>
        /// <returns>The created meteor object.</returns>
        [Obsolete]
        public static GameObject MakeMeteor(GameObject customMeteorPrefab) => CustomBigStar.SetStar(customMeteorPrefab);

        //Central zombie spawn wrapper
        /// <summary>Spawns a zombie at a board cell, optionally under mind control.</summary>
        /// <param name="type">The type of zombie to spawn.</param>
        /// <param name="row">The board row in which to spawn the zombie.</param>
        /// <param name="column">The board column in which to spawn the zombie.</param>
        /// <param name="isHypno">Whether to spawn the zombie under mind control.</param>
        /// <returns>The spawned zombie, or <see langword="null"/> if the zombie spawner is unavailable.</returns>
        [Obsolete]
        public static Zombie SetZombie(ZombieType type, int row, int column, bool isHypno = true)
        {
            CreateZombie createZombie = CreateZombie.Instance;
            if (createZombie == null)
                return null;

            if (isHypno)
            {
                return createZombie.SetZombieWithMindControl(row, type, getX(column), false);
            }
            else
            {
                return createZombie.SetZombie(row, type, getX(column), false);
            }
        }

        //Percent getter
        /// <summary>Tests whether a random value from zero to one hundred is below the specified percentage.</summary>
        /// <param name="percent">The percentage threshold.</param>
        /// <returns><see langword="true"/> when the generated value is below the threshold; otherwise, <see langword="false"/>.</returns>
        public static bool GetPercent(float percent) => Random.Range(0f, 100f) < percent;

        //Plant text getter
        /// <summary>Formats the dictionary entries as concatenated key and value text.</summary>
        /// <param name="dic">The dictionary whose entries should be formatted.</param>
        /// <returns>The formatted dictionary entries.</returns>
        public static string GetTextString(Dictionary<string, string> dic)
        {
            string s = "";
            foreach (var i in dic.Keys)
            {
                s += $"{i} : {dic[i]}";
            }
            return s;
        }

        //Type getters
        /// <summary>Gets plant types accepted by the supplied selector.</summary>
        /// <param name="selector">The filter to apply, or <see langword="null"/> to exclude the vector plant.</param>
        /// <returns>The matching plant types.</returns>
        public static List<PlantType> GetAllPlantTypes(Func<PlantType, bool> selector = null)
        {
            if (selector == null) selector = (PlantType pt) => pt != PlantType.VectorPlant;
            return (List<PlantType>)GameAPP.resourcesManager.allPlants.ToSystemList().Where(selector);
        }
        /// <summary>Gets non-vector plant types with the specified card level.</summary>
        /// <param name="cardLevel">The card level to match.</param>
        /// <returns>The matching plant types.</returns>
        public static List<PlantType> GetAllPlantTypes(CardLevel cardLevel)
        {
            var selector = (PlantType pt) => TreasureData.GetCardLevel(pt) == cardLevel && pt != PlantType.VectorPlant;
            return (List<PlantType>)GameAPP.resourcesManager.allPlants.ToSystemList().Where(selector);
        }
        /// <summary>Gets all non-vector plant types that are neither ultimate nor super plants.</summary>
        /// <returns>The matching normal plant types.</returns>
        public static List<PlantType> GetAllNormalPlantTypes()
        {
            var selector = (PlantType pt) => !Lawnf.IsUltiPlant(pt) && !Lawnf.IsSuperPlant(pt) && pt != PlantType.VectorPlant;
            return (List<PlantType>)GameAPP.resourcesManager.allPlants.ToSystemList().Where(selector);
        }
        /// <summary>Gets all non-vector ultimate plant types.</summary>
        /// <returns>The matching ultimate plant types.</returns>
        public static List<PlantType> GetAllUltimatePlantTypes()
        {
            var selector = (PlantType pt) => Lawnf.IsUltiPlant(pt) && pt != PlantType.VectorPlant;
            return (List<PlantType>)GameAPP.resourcesManager.allPlants.ToSystemList().Where(selector);
        }
        /// <summary>Gets zombie types accepted by the supplied selector.</summary>
        /// <param name="selector">The filter to apply, or <see langword="null"/> to exclude the training dummy.</param>
        /// <returns>The matching zombie types.</returns>
        public static List<ZombieType> GetAllZombieTypes(Func<ZombieType, bool> selector = null)
        {
            if (selector == null) selector = (ZombieType zt) => zt != ZombieType.TrainingDummy;
            return (List<ZombieType>)GameAPP.resourcesManager.allZombieTypes.ToSystemList().Where(selector);
        }

        //Get plants in area
        /// <summary>Checks whether any plant in the 3-by-3 area around a cell satisfies the selector.</summary>
        /// <param name="theColumn">The center cell's column.</param>
        /// <param name="theRow">The center cell's row.</param>
        /// <param name="selector">The condition to test for each plant.</param>
        /// <returns><see langword="true"/> if a matching plant is in the area.</returns>
        public static bool IsTypeIn3x3(int theColumn, int theRow, Func<Plant, bool> selector)
        {
            return Lawnf.Get3x3Plants(theColumn, theRow).ToSystemList().Any(selector);
        }
        /// <summary>Checks whether a plant of the specified type is in the 3-by-3 area around a cell.</summary>
        /// <param name="theColumn">The center cell's column.</param>
        /// <param name="theRow">The center cell's row.</param>
        /// <param name="thePlantType">The plant type to search for.</param>
        /// <returns><see langword="true"/> if a plant of that type is in the area.</returns>
        public static bool IsTypeIn3x3(int theColumn, int theRow, PlantType thePlantType)
        {
            return IsTypeIn3x3(theColumn, theRow, (Plant p) => p.thePlantType == thePlantType);
        }
        /// <summary>Gets the first plant in the 3-by-3 area around a cell that satisfies the selector.</summary>
        /// <param name="theColumn">The center cell's column.</param>
        /// <param name="theRow">The center cell's row.</param>
        /// <param name="selector">The condition to test for each plant.</param>
        /// <returns>The first matching plant, or <see langword="null"/> if none is found.</returns>
        public static Plant GetPlantIn3x3(int theColumn, int theRow, Func<Plant, bool> selector)
        {
            return Lawnf.Get3x3Plants(theColumn, theRow).ToSystemList().FirstOrDefault(selector);
        }
        /// <summary>Gets the first plant of the specified type in the 3-by-3 area around a cell.</summary>
        /// <param name="theColumn">The center cell's column.</param>
        /// <param name="theRow">The center cell's row.</param>
        /// <param name="thePlantType">The plant type to search for.</param>
        /// <returns>The first matching plant, or <see langword="null"/> if none is found.</returns>
        public static Plant GetPlantIn3x3(int theColumn, int theRow, PlantType thePlantType)
        {
            return GetPlantIn3x3(theColumn, theRow, (Plant p) => p.thePlantType == thePlantType);
        }
        /// <summary>Checks whether any plant in the specified cell satisfies the selector.</summary>
        /// <param name="theColumn">The cell's column.</param>
        /// <param name="theRow">The cell's row.</param>
        /// <param name="selector">The condition to test for each plant.</param>
        /// <returns><see langword="true"/> if a matching plant is in the cell.</returns>
        public static bool IsTypeIn1x1(int theColumn, int theRow, Func<Plant, bool> selector)
        {
            return Lawnf.Get1x1Plants(theColumn, theRow).ToSystemList().Any(selector);
        }
        /// <summary>Checks whether a plant of the specified type is in the given cell.</summary>
        /// <param name="theColumn">The cell's column.</param>
        /// <param name="theRow">The cell's row.</param>
        /// <param name="thePlantType">The plant type to search for.</param>
        /// <returns><see langword="true"/> if a plant of that type is in the cell.</returns>
        public static bool IsTypeIn1x1(int theColumn, int theRow, PlantType thePlantType)
        {
            return IsTypeIn1x1(theColumn, theRow, (Plant p) => p.thePlantType == thePlantType);
        }
        /// <summary>Gets the first plant in the specified cell that satisfies the selector.</summary>
        /// <param name="theColumn">The cell's column.</param>
        /// <param name="theRow">The cell's row.</param>
        /// <param name="selector">The condition to test for each plant.</param>
        /// <returns>The first matching plant, or <see langword="null"/> if none is found.</returns>
        public static Plant GetPlantIn1x1(int theColumn, int theRow, Func<Plant, bool> selector)
        {
            return Lawnf.Get1x1Plants(theColumn, theRow).ToSystemList().FirstOrDefault(selector);
        }
        /// <summary>Gets the first plant of the specified type in the given cell.</summary>
        /// <param name="theColumn">The cell's column.</param>
        /// <param name="theRow">The cell's row.</param>
        /// <param name="thePlantType">The plant type to search for.</param>
        /// <returns>The first matching plant, or <see langword="null"/> if none is found.</returns>
        public static Plant GetPlantIn1x1(int theColumn, int theRow, PlantType thePlantType)
        {
            return GetPlantIn1x1(theColumn, theRow, (Plant p) => p.thePlantType == thePlantType);
        }
        /// <summary>Gets a random plant type accepted by the selector.</summary>
        /// <param name="selector">The filter to apply, or <see langword="null"/> to use the default plant-type filter.</param>
        /// <returns>A randomly selected matching plant type.</returns>
        public static PlantType GetRandomPlantType(Func<PlantType, bool> selector = null)
        {
            return GetAllPlantTypes(selector).GetRandomItem();
        }

        //Null checkers
        /// <summary>Copies a reference to the output and reports whether it is non-null.</summary>
        /// <typeparam name="T">The reference type being checked.</typeparam>
        /// <param name="input">The value to check.</param>
        /// <param name="output">Receives the input value.</param>
        /// <returns><see langword="true"/> if the input is non-null.</returns>
        public static bool IsNotNull<T>(T input, out T output)
        {
            output = input;
            if (input == null) return false;
            else return true;
        }
        /// <summary>Copies a Unity behaviour to the output and checks that it is not null or destroyed.</summary>
        /// <typeparam name="T">The Unity behaviour type being checked.</typeparam>
        /// <param name="input">The behaviour to check.</param>
        /// <param name="output">Receives the input behaviour.</param>
        /// <returns><see langword="true"/> if the behaviour exists and has not been destroyed.</returns>
        public static bool IsNotNullMonoBehaviour<T>(T input, out T output) where T : MonoBehaviour
        {
            output = input;
            if (input == null || !input || input.IsDestroyed()) return false;
            else return true;
        }

        //SetBullet wrappers
        /// <summary>Fires a bullet from a plant's shoot position using the plant's attack damage and row.</summary>
        /// <param name="fromPlant">The plant firing the bullet.</param>
        /// <param name="theBulletType">The type of bullet to create.</param>
        /// <param name="theMovingWay">The bullet's movement pattern.</param>
        /// <param name="offset">An offset applied to the bullet's spawn position.</param>
        /// <param name="rotation">The bullet's rotation in degrees.</param>
        /// <param name="fromEnemy">Whether the bullet is considered enemy-fired.</param>
        /// <returns>The created bullet, or <see langword="null"/> if creation fails.</returns>
        public static Bullet SetBullet(Plant fromPlant, BulletType theBulletType, BulletMoveWay theMovingWay, Vector2 offset = new Vector2(), float rotation = 0f, bool fromEnemy = false)
        {
            return SetBullet(fromPlant, fromPlant.shoot.position, fromPlant.thePlantRow, theBulletType, theMovingWay, fromPlant.attackDamage, offset, rotation, fromEnemy);
        }
        /// <summary>Fires a bullet from a plant's shoot position using the specified damage and the plant's row.</summary>
        /// <param name="fromPlant">The plant firing the bullet.</param>
        /// <param name="theBulletType">The type of bullet to create.</param>
        /// <param name="theMovingWay">The bullet's movement pattern.</param>
        /// <param name="damage">The damage dealt by the bullet.</param>
        /// <param name="offset">An offset applied to the bullet's spawn position.</param>
        /// <param name="rotation">The bullet's rotation in degrees.</param>
        /// <param name="fromEnemy">Whether the bullet is considered enemy-fired.</param>
        /// <returns>The created bullet, or <see langword="null"/> if creation fails.</returns>
        public static Bullet SetBullet(Plant fromPlant, BulletType theBulletType, BulletMoveWay theMovingWay, int damage, Vector2 offset = new Vector2(), float rotation = 0f, bool fromEnemy = false)
        {
            return SetBullet(fromPlant, fromPlant.shoot.position, fromPlant.thePlantRow, theBulletType, theMovingWay, damage, offset, rotation, fromEnemy);
        }
        /// <summary>Fires a bullet from a plant's shoot position in the specified row using the plant's attack damage.</summary>
        /// <param name="fromPlant">The plant firing the bullet.</param>
        /// <param name="theRow">The row in which the bullet is created.</param>
        /// <param name="theBulletType">The type of bullet to create.</param>
        /// <param name="theMovingWay">The bullet's movement pattern.</param>
        /// <param name="offset">An offset applied to the bullet's spawn position.</param>
        /// <param name="rotation">The bullet's rotation in degrees.</param>
        /// <param name="fromEnemy">Whether the bullet is considered enemy-fired.</param>
        /// <returns>The created bullet, or <see langword="null"/> if creation fails.</returns>
        public static Bullet SetBullet(Plant fromPlant, int theRow, BulletType theBulletType, BulletMoveWay theMovingWay, Vector2 offset = new Vector2(), float rotation = 0f, bool fromEnemy = false)
        {
            return SetBullet(fromPlant, fromPlant.shoot.position, theRow, theBulletType, theMovingWay, fromPlant.attackDamage, offset, rotation, fromEnemy);
        }
        /// <summary>Fires a bullet from a plant's shoot position in the specified row using the specified damage.</summary>
        /// <param name="fromPlant">The plant firing the bullet.</param>
        /// <param name="theRow">The row in which the bullet is created.</param>
        /// <param name="theBulletType">The type of bullet to create.</param>
        /// <param name="theMovingWay">The bullet's movement pattern.</param>
        /// <param name="damage">The damage dealt by the bullet.</param>
        /// <param name="offset">An offset applied to the bullet's spawn position.</param>
        /// <param name="rotation">The bullet's rotation in degrees.</param>
        /// <param name="fromEnemy">Whether the bullet is considered enemy-fired.</param>
        /// <returns>The created bullet, or <see langword="null"/> if creation fails.</returns>
        public static Bullet SetBullet(Plant fromPlant, int theRow, BulletType theBulletType, BulletMoveWay theMovingWay, int damage, Vector2 offset = new Vector2(), float rotation = 0f, bool fromEnemy = false)
        {
            return SetBullet(fromPlant, fromPlant.shoot.position, theRow, theBulletType, theMovingWay, damage, offset, rotation, fromEnemy);
        }
        /// <summary>Fires a bullet from the specified position in the plant's row using the plant's attack damage.</summary>
        /// <param name="fromPlant">The plant firing the bullet.</param>
        /// <param name="pos">The bullet's base spawn position.</param>
        /// <param name="theBulletType">The type of bullet to create.</param>
        /// <param name="theMovingWay">The bullet's movement pattern.</param>
        /// <param name="offset">An offset applied to the bullet's spawn position.</param>
        /// <param name="rotation">The bullet's rotation in degrees.</param>
        /// <param name="fromEnemy">Whether the bullet is considered enemy-fired.</param>
        /// <returns>The created bullet, or <see langword="null"/> if creation fails.</returns>
        public static Bullet SetBullet(Plant fromPlant, Vector2 pos, BulletType theBulletType, BulletMoveWay theMovingWay, Vector2 offset = new Vector2(), float rotation = 0f, bool fromEnemy = false)
        {
            return SetBullet(fromPlant, pos, fromPlant.thePlantRow, theBulletType, theMovingWay, fromPlant.attackDamage, offset, rotation, fromEnemy);
        }
        /// <summary>Fires a bullet from the specified position in the plant's row using the specified damage.</summary>
        /// <param name="fromPlant">The plant firing the bullet.</param>
        /// <param name="pos">The bullet's base spawn position.</param>
        /// <param name="theBulletType">The type of bullet to create.</param>
        /// <param name="theMovingWay">The bullet's movement pattern.</param>
        /// <param name="damage">The damage dealt by the bullet.</param>
        /// <param name="offset">An offset applied to the bullet's spawn position.</param>
        /// <param name="rotation">The bullet's rotation in degrees.</param>
        /// <param name="fromEnemy">Whether the bullet is considered enemy-fired.</param>
        /// <returns>The created bullet, or <see langword="null"/> if creation fails.</returns>
        public static Bullet SetBullet(Plant fromPlant, Vector2 pos, BulletType theBulletType, BulletMoveWay theMovingWay, int damage, Vector2 offset = new Vector2(), float rotation = 0f, bool fromEnemy = false)
        {
            return SetBullet(fromPlant, pos, fromPlant.thePlantRow, theBulletType, theMovingWay, damage, offset, rotation, fromEnemy);
        }
        /// <summary>Fires a bullet from the specified position and row using the plant's attack damage.</summary>
        /// <param name="fromPlant">The plant firing the bullet.</param>
        /// <param name="pos">The bullet's base spawn position.</param>
        /// <param name="theRow">The row in which the bullet is created.</param>
        /// <param name="theBulletType">The type of bullet to create.</param>
        /// <param name="theMovingWay">The bullet's movement pattern.</param>
        /// <param name="offset">An offset applied to the bullet's spawn position.</param>
        /// <param name="rotation">The bullet's rotation in degrees.</param>
        /// <param name="fromEnemy">Whether the bullet is considered enemy-fired.</param>
        /// <returns>The created bullet, or <see langword="null"/> if creation fails.</returns>
        public static Bullet SetBullet(Plant fromPlant, Vector2 pos, int theRow, BulletType theBulletType, BulletMoveWay theMovingWay, Vector2 offset = new Vector2(), float rotation = 0f, bool fromEnemy = false)
        {
            return SetBullet(fromPlant, pos, theRow, theBulletType, theMovingWay, fromPlant.attackDamage, offset, rotation, fromEnemy);
        }
        /// <summary>Creates a bullet at the specified position and row with the requested damage.</summary>
        /// <param name="fromPlant">The plant firing the bullet.</param>
        /// <param name="pos">The bullet's base spawn position.</param>
        /// <param name="theRow">The row in which the bullet is created.</param>
        /// <param name="theBulletType">The type of bullet to create.</param>
        /// <param name="theMovingWay">The bullet's movement pattern.</param>
        /// <param name="damage">The damage dealt by the bullet.</param>
        /// <param name="offset">An offset applied to the bullet's spawn position.</param>
        /// <param name="rotation">The bullet's rotation in degrees.</param>
        /// <param name="fromEnemy">Whether the bullet is considered enemy-fired.</param>
        /// <returns>The created bullet, or <see langword="null"/> if creation fails.</returns>
        public static Bullet SetBullet(Plant fromPlant, Vector2 pos, int theRow, BulletType theBulletType, BulletMoveWay theMovingWay, int damage, Vector2 offset = new Vector2(), float rotation = 0f, bool fromEnemy = false)
        {
            Bullet b = InstanceManager.CreateBullet.SetBullet(pos.x + offset.x, pos.y + offset.y, theRow, theBulletType, theMovingWay, fromEnemy);
            if (b == null) return null;
            b.Damage = damage;
            b.fromType = fromPlant.thePlantType;
            b.transform.Rotate(0, 0, rotation);
            b.from = fromPlant;
            return b;
        }

        //Buff helpers
        /// <summary>Checks whether the travel store is currently available.</summary>
        /// <returns><see langword="true"/> if the travel store exists.</returns>
        public static bool IsTravelStore()
        {
            return InstanceManager.TravelStore != null;
        }
        /// <summary>Applies the travel effect or unlock represented by the specified string.</summary>
        /// <param name="str">The string identifying the travel effect or unlock.</param>
        /// <returns><see langword="true"/> if one of the supported travel effects was applied.</returns>
        public static bool GetBuffByString(string str)
        {
            bool result = CoreTools.TravelAdvanced(str);
            if (result) return true;
            result = CoreTools.TravelUltimate(str);
            if (result) return true;
            result = Lawnf.TravelUnlock(CoreTools.GetTravelUnlocksByString(str));
            if (result) return true;
            return Lawnf.TravelDebuff(CoreTools.GetTravelDebuffByString(str));
        }
        /// <summary>Checks whether a board exists and the game is currently in progress.</summary>
        /// <returns><see langword="true"/> while the game is in progress.</returns>
        public static bool IsInGame() => InstanceManager.Board != null && GameAPP.theGameStatus is GameStatus.InGame;
        /// <summary>Plays the specified sound clip.</summary>
        /// <param name="soundClip">The audio clip to play.</param>
        /// <returns>The result reported by the sound manager.</returns>
        public static bool PlaySound(AudioClip soundClip)
        {
            return SoundManager.PlaySound(soundClip).GetAwaiter().GetResult();
        }
        /// <summary>Runs an action after the specified delay.</summary>
        /// <param name="a">The action to run.</param>
        /// <param name="delaySeconds">The delay before running the action, in seconds.</param>
        public static async void WaitAndExecute(Action a, float delaySeconds)
        {
            await DelayTask.Delay(delaySeconds);
            a();
        }
        /// <summary>Runs an action with a parameter after the specified delay.</summary>
        /// <typeparam name="T">The type of the action parameter.</typeparam>
        /// <param name="a">The action to run.</param>
        /// <param name="parameter">The value passed to the action.</param>
        /// <param name="delaySeconds">The delay before running the action, in seconds.</param>
        public static async void WaitAndExecute<T>(Action<T> a, T parameter, float delaySeconds)
        {
            await DelayTask.Delay(delaySeconds);
            a(parameter);
        }
        /// <summary>Runs a function with a parameter after the specified delay and returns its result.</summary>
        /// <typeparam name="T">The type of the function parameter.</typeparam>
        /// <typeparam name="Tout">The type of the function result.</typeparam>
        /// <param name="a">The function to run.</param>
        /// <param name="parameter">The value passed to the function.</param>
        /// <param name="delaySeconds">The delay before running the function, in seconds.</param>
        /// <returns>A task that completes with the function's result.</returns>
        public static async Task<Tout> WaitAndExecute<T, Tout>(Func<T, Tout> a, T parameter, float delaySeconds)
        {
            await DelayTask.Delay(delaySeconds);
            return a(parameter);
        }
        /// <summary>Creates a two-point line renderer that fades after remaining visible for the specified time.</summary>
        /// <param name="parent">The transform that will parent the line object.</param>
        /// <param name="from">The line's starting position.</param>
        /// <param name="to">The line's ending position.</param>
        /// <param name="fromColor">The line's starting color.</param>
        /// <param name="toColor">The line's ending color.</param>
        /// <param name="row">The board row used to select the sorting layer.</param>
        /// <param name="token">An optional token that can cancel the fade loop.</param>
        /// <param name="stayTime">The initial opacity duration before fading.</param>
        /// <param name="startWidth">The line width at its start.</param>
        /// <param name="endWidth">The line width at its end.</param>
        /// <param name="mat">The line material, or <see langword="null"/> to use the default.</param>
        public static async void CreateLine(
            Transform parent,
            Vector2 from, Vector2 to,
            Color fromColor, Color toColor, int row,
            CancellationToken token = null, float stayTime = 1f,
            float startWidth = 0.5f, float endWidth = 0.5f,
            Material mat = null
        )
        {
            var obj = new GameObject("Line", typeof(LineRenderer).ToIl2CppType());
            obj.transform.SetParent(parent);
            var line = obj.GetComponent<LineRenderer>();

            line.positionCount = 2;
            line.SetPosition(0, from);
            line.SetPosition(1, to);
            line.startWidth = startWidth;
            line.endWidth = endWidth;
            line.startColor = fromColor;
            line.endColor = toColor;
            line.sharedMaterial = mat ?? Resources.Load<Material>("Plants/ElectricOnion/electirc");
            line.sortingLayerName = string.Format("plant{0}", row);

            float a = stayTime;

            if (token == null) token = new();

            do
            {
                a -= Time.deltaTime * 5;
                var color1 = line.startColor;
                var color2 = line.endColor;
                color1.a = a;
                color2.a = a;
                line.startColor = color1;
                line.endColor = color2;
                await DelayTask.WaitForFixedUpdate(token);
            }
            while (a > 0f && !token.IsCanceled);
        }
        /// <summary>Creates a line renderer through the supplied points and fades it after the specified time.</summary>
        /// <param name="parent">The transform that will parent the line object.</param>
        /// <param name="fromColor">The line's starting color.</param>
        /// <param name="toColor">The line's ending color.</param>
        /// <param name="row">The board row used to select the sorting layer.</param>
        /// <param name="token">An optional token that can cancel the fade loop.</param>
        /// <param name="stayTime">The initial opacity duration before fading.</param>
        /// <param name="startWidth">The line width at its start.</param>
        /// <param name="endWidth">The line width at its end.</param>
        /// <param name="mat">The line material, or <see langword="null"/> to use the default.</param>
        /// <param name="pts">The points connected by the line.</param>
        public static async void CreateLine(
            Transform parent,
            Color fromColor, Color toColor, int row,
            CancellationToken token = null, float stayTime = 1f,
            float startWidth = 0.5f, float endWidth = 0.5f,
            Material mat = null, params Vector2[] pts
        )
        {
            if (pts.Length < 1)
            {
                Debug.LogWarning("Can't make a line with 1 point!");
                return;
            }
            var obj = new GameObject("Line", typeof(LineRenderer).ToIl2CppType());
            obj.transform.SetParent(parent);
            var line = obj.GetComponent<LineRenderer>();

            line.positionCount = pts.Length;
            for (int i = 0; i < pts.Length; i++)
            {
                line.SetPosition(i, pts[i]);
            }
            line.startWidth = startWidth;
            line.endWidth = endWidth;
            line.startColor = fromColor;
            line.endColor = toColor;
            line.sharedMaterial = mat ?? Resources.Load<Material>("Plants/ElectricOnion/electirc");
            line.sortingLayerName = string.Format("plant{0}", row);

            float a = stayTime;

            if (token == null) token = new();

            do
            {
                a -= Time.deltaTime * 5;
                var color1 = line.startColor;
                var color2 = line.endColor;
                color1.a = a;
                color2.a = a;
                line.startColor = color1;
                line.endColor = color2;
                await DelayTask.WaitForFixedUpdate(token);
            }
            while (a > 0f && !token.IsCanceled);
        }
        /// <summary>Creates and fades a two-point line renderer using an IL2CPP cancellation token.</summary>
        /// <param name="parent">The transform that will parent the line object.</param>
        /// <param name="from">The line's starting position.</param>
        /// <param name="to">The line's ending position.</param>
        /// <param name="fromColor">The line's starting color.</param>
        /// <param name="toColor">The line's ending color.</param>
        /// <param name="row">The board row used to select the sorting layer.</param>
        /// <param name="token">An optional IL2CPP token that can cancel the fade loop.</param>
        /// <param name="stayTime">The initial opacity duration before fading.</param>
        /// <param name="startWidth">The line width at its start.</param>
        /// <param name="endWidth">The line width at its end.</param>
        /// <param name="mat">The line material, or <see langword="null"/> to use the default.</param>
        public static async void CreateLine(
            Transform parent,
            Vector2 from, Vector2 to,
            Color fromColor, Color toColor, int row,
            Il2CppSystem.Threading.CancellationToken token = null, float stayTime = 1f,
            float startWidth = 0.5f, float endWidth = 0.5f,
            Material mat = null
        )
        {
            var obj = new GameObject("Line", typeof(LineRenderer).ToIl2CppType());
            obj.transform.SetParent(parent);
            var line = obj.GetComponent<LineRenderer>();

            line.positionCount = 2;
            line.SetPosition(0, from);
            line.SetPosition(1, to);
            line.startWidth = startWidth;
            line.endWidth = endWidth;
            line.startColor = fromColor;
            line.endColor = toColor;
            line.sharedMaterial = mat ?? Resources.Load<Material>("Plants/ElectricOnion/electirc");
            line.sortingLayerName = string.Format("plant{0}", row);

            float a = stayTime;

            if (token == null) token = new();

            do
            {
                a -= Time.deltaTime * 5;
                var color1 = line.startColor;
                var color2 = line.endColor;
                color1.a = a;
                color2.a = a;
                line.startColor = color1;
                line.endColor = color2;
                await DelayTask.WaitForFixedUpdate();
            }
            while (a > 0f && !token.IsCancellationRequested);
        }
        /// <summary>Creates and fades a line renderer through the supplied points using an IL2CPP cancellation token.</summary>
        /// <param name="parent">The transform that will parent the line object.</param>
        /// <param name="fromColor">The line's starting color.</param>
        /// <param name="toColor">The line's ending color.</param>
        /// <param name="row">The board row used to select the sorting layer.</param>
        /// <param name="token">An optional IL2CPP token that can cancel the fade loop.</param>
        /// <param name="stayTime">The initial opacity duration before fading.</param>
        /// <param name="startWidth">The line width at its start.</param>
        /// <param name="endWidth">The line width at its end.</param>
        /// <param name="mat">The line material, or <see langword="null"/> to use the default.</param>
        /// <param name="pts">The points connected by the line.</param>
        public static async void CreateLine(
            Transform parent,
            Color fromColor, Color toColor, int row,
            Il2CppSystem.Threading.CancellationToken token = null, float stayTime = 1f,
            float startWidth = 0.5f, float endWidth = 0.5f,
            Material mat = null, params Vector2[] pts
        )
        {
            if (pts.Length < 1)
            {
                Debug.LogWarning("Can't make a line with 1 point!");
                return;
            }
            var obj = new GameObject("Line", typeof(LineRenderer).ToIl2CppType());
            obj.transform.SetParent(parent);
            var line = obj.GetComponent<LineRenderer>();

            line.positionCount = pts.Length;
            for (int i = 0; i < pts.Length; i++)
            {
                line.SetPosition(i, pts[i]);
            }
            line.startWidth = startWidth;
            line.endWidth = endWidth;
            line.startColor = fromColor;
            line.endColor = toColor;
            line.sharedMaterial = mat ?? Resources.Load<Material>("Plants/ElectricOnion/electirc");
            line.sortingLayerName = string.Format("plant{0}", row);

            float a = stayTime;

            if (token == null) token = new();

            do
            {
                a -= Time.deltaTime * 5;
                var color1 = line.startColor;
                var color2 = line.endColor;
                color1.a = a;
                color2.a = a;
                line.startColor = color1;
                line.endColor = color2;
                await DelayTask.WaitForFixedUpdate();
            }
            while (a > 0f && !token.IsCancellationRequested);
        }
    }
}