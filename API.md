## API documentation
### CustomPlantClass

<details>
<summary>Click to show section</summary>
  
1. CustomBigStar -> Big star monobehaviour (undocumented)
2.  BaseCustomBullet -> Used for creating custom bullets, overridable methods are structured like prefixes, returning true will run the original method, returning false will skip the original method.(Currently supports bullet_pea and bullet_cabbage as its TBase) Example impl:
<details>
<summary>Click to show code</summary>

```csharp
public class Bullet_ultimateMelonCabbage : BaseCustomBullet
{
    public override bool HitLand()
    {
        _bullet.board.boardAction.CreateCherryExplode(
            _bullet.col.bounds.center,
            _bullet.theBulletRow,
            Plugin.DataContainer.ParticleId,
            _bullet.Damage,
            _bullet.fromType
        );
        return true;
    }

    public override bool HitZombie(Zombie zombie)
    {
        _bullet.board.boardAction.CreateCherryExplode(
            _bullet.col.bounds.center,
            _bullet.theBulletRow,
            Plugin.DataContainer.ParticleId,
            _bullet.Damage,
            _bullet.fromType
        );
        return true;
    }
}
```
</details>

3. CustomLevelComponent  ![Deprecated: Replaced](https://img.shields.io/badge/Deprecated-Replaced-red?style=for-the-badge)
4. CustomOnZombieComponent  ![Deprecated: Replaced](https://img.shields.io/badge/Deprecated-Replaced-red?style=for-the-badge)
5. CustomParticle -> Used to add a destroy animation event to particles with non-particlesystem components
6. BaseCustomPlant -> Central base class for most custom plants(see example code above)
7. CustomShooter -> An extension to BaseCustomPlant and handles shooting to allow custom plants to use "Plant" as its base class
8. CustomSolarBomb -> Untested
9. CustomThrower -> An extension to CustmShooter to handle catapult plants
10. BaseCustomZombie -> Used to add custom behaviour to zombies
11. CustomEffect -> Used to add effects to monobehaviours
12. InterfaceMgr -> Allows plants to implement base game behaviour without inhereting classes
<details>
<summary>Click to show code</summary>

```csharp
public interface IRedirectAnimShoot
{
    public Bullet Shoot1();
}
public interface IRedirectAnimShoot2
{
    public Bullet Shoot2();
}
public interface IOverrideDamagePipeline
{
    public int GetDamage(int damage, IDamageMaker damageFrom, DamageType theDamageType);
}
public interface ICustomClick
{
    public void OnClicked(Mouse mouse);
}
public interface ICustomPF
{
    public bool IsImmune { get; }
    public void StartPF();
    public void SuperEnd();
}
public interface IPlantDieRedirector
{
    public bool CanBeCrashed { get; }
    public bool CanDie { get; }
    public bool CanBeFrozen { get; }
}
public interface IPlantDieHandler
{
    public void OnDie(DieReason reason);
}
public interface IPlantTextHandler
{
    public void InitText();
}
public interface IPlantGetTextStringHandler
{
    public virtual Color SetTextColor() => Color.cyan;
    public virtual Vector2? GetTextSize() => null;
    public string GetTextString();
}
public interface IPlantCannonAimHandler
{
    string CannonName { get; }

    void OnCannonAimed(Vector2 pos);

    GameObject CannonObjReference =>
        Resources.Load<GameObject>("items/CobCannon_target.prefab");

    void CreateCannonAim(Plant p, Mouse mouse)
    {
        mouse.cannonPlant = p;

        mouse.theItemOnMouse = Object.Instantiate(
            CannonObjReference,
            mouse.MousePosition,
            Quaternion.identity,
            p.board.transform
        );

        mouse.theItemOnMouse.name = CannonName;
    }
}
```
</details>

13. PlantSkinComponent, BulletComponent, ZombieComponent -> Contains a getter property to get their respective property on its gameobject
14. ModLogger -> Used to lod information about the mod and also to log errors/warnings
</details>

### CustomPlantClass.Level
<details>
<summary>Click to show section</summary>
  
1. BranchAdventureManager -> Used to register custom branch adventures (Incomplete! do not use until verified!)
2. CustomLevelMgr -> Used to load custom levels into the game. API (full code in repo):
<details>
<summary>Click to show code</summary>

```csharp
public class CustomLevelMgr : MonoBehaviour
{
    public static Dictionary<int, Func<bool>> CanUnlockLevel = new();
    public static int RegisterCustomLevel<T>(BaseCustomLevelData data) where T : MonoBehaviour { }
    public static int RegisterCustomLevel(BaseCustomLevelData data) { }
    public static int RegisterCustomLevel<T>(BaseCustomLevelData data, Func<bool> canUnlock) where T : MonoBehaviour { }
    public static int RegisterCustomLevel(BaseCustomLevelData data, Func<bool> canUnlock) { }
    public static int AllocateLevelID() { }
    public static int AllocateLevelID(string name) { }
}
```
</details>

3. LevelProgressionManager -> Used to track what custom levels are completed. API  (full code in repo):
<details>
<summary>Click to show code</summary>

```csharp
public static class LevelProgressionManager
{
    public static void MarkCompleted(int levelID) { }
    public static void MarkNotCompleted(int levelID) { }
    public static bool IsCompleted(int levelID) { }
}
```
</details>
</details>

### CustomPlantClass.Examples (Provides inheretable classes for base game plants)
<details>
<summary>Click to show section</summary>
  
1. ArmedChomperBase -> Untested
2. DoomSniper_Example
3. SniperPea_Example
4. SuperHypnoGatling_Example
</details>

### CustomPlantClass.Main
<details>
<summary>Click to show section</summary>
  
1. AssetMgr -> Used to load/save assets from different sources. API  (full code in repo):
<details>
<summary>Click to show code</summary>

```csharp
public static class AssetMgr
{
    // -----------------------------
    //  Load AssetBundle from Base64
    // -----------------------------
    public static AssetBundle LoadBundleBase64(string base64) { }

    // -----------------------------
    //  Load AssetBundle from file
    // -----------------------------
    public static AssetBundle LoadBundleFromFile(string path, string name, bool deduplicate = true) { }


    // -----------------------------------------
    //  Load AssetBundle from embedded resources
    // -----------------------------------------
    public static AssetBundle LoadBundleFromResource(Assembly asm, string resourceName, bool deduplicate = true) { }

    public static async Task<string> DownloadAndConvertToBase64Async(string url) { }

    public static string GetBase64FromCache(string cacheFolder, string filename, string urlfallback) { }

    public static T CastAsset<T>(this Object obj) where T : Object => (T)obj;
}
public sealed class Asset<T> where T : Object
{
    public T Obj { get; }
    public string Name { get; }

    public Asset(T obj)
    {
        Obj = obj;
        Name = obj.name;
    }

    public static Asset<T> FromObject(Object obj)
        => new((T)obj);
}
public sealed class AssetDispatcher
{
    public void Add<T>(Predicate<T> match, Action<T> action) where T : Object { }
    public bool TryInvoke(Object obj) { }
    public void Switch(AssetBundle bundle) { }
}
```
</details>

2. DataMgr -> The core of the framework and contains most APIs. API  (full code in repo):
<details>
<summary>Click to show code</summary>

```csharp
/// <summary>
/// Central utility manager for all custom plant registration,
/// fusion helpers, skin helpers, ID allocation, and IL2CPP type registration.
/// This class is the backbone of the new custom plant framework.
/// </summary>
public sealed class DataMgr : MonoBehaviour
{
    public static ID AllocateID() { }

    #endregion

    #region Bullet Skin

    // ---------------------------------------------------------
    //  BULLET SKIN HELPERS
    // ---------------------------------------------------------

    /// <summary>
    /// Creates a bullet skin mapping list for plant skins.
    /// </summary>
    public static List<(BulletType, List<GameObject?>)> BulletSkin(params (BulletType, GameObject?[])[] entries) { }
    #endregion
    #region Boss Slider

    public static void RegisterCustomBossHealthSlider(ID zombieType, GameObject slider) { }

    public static void RegisterCustomBossHealthSlider(CustomBossHealthSliderData data) { }
    #endregion
    #region Data Builder

    // ---------------------------------------------------------
    //  DATA BUILDERS
    // ---------------------------------------------------------

    /// <summary>
    /// Creates a default-initialized plant data struct.
    /// </summary>
    public static BaseCustomPlantData CreatePlantData(ID id, GameObject prefab, GameObject preview) { }

    /// <summary>
    /// Creates a skin data struct for a plant.
    /// </summary>
    public static BasePlantSkinData CreateSkin(BaseCustomPlantData data, GameObject skinPrefab, GameObject skinPreview) { }
    #endregion
    #region Il2cpp Types

    // ---------------------------------------------------------
    //  IL2CPP TYPE REGISTRATION
    // ---------------------------------------------------------

    /// <summary>
    /// Registers all BaseCustomPlant-derived types in an assembly.
    /// </summary>
    public static void AutoRegisterTypes() => AutoRegisterTypes(Assembly.GetCallingAssembly());
    public static void AutoRegisterTypes(Assembly asm) { }

    /// <summary>
    /// Ensures a specific custom plant class is IL2CPP-registered.
    /// </summary>
    public static void EnsureTypeRegistered<TClass>() where TClass : MonoBehaviour { }
    #endregion
    #region Plants

    // ---------------------------------------------------------
    //  PLANT REGISTRATION
    // ---------------------------------------------------------
    public static ID RegisterCustomPlant<TBase, TClass>(BaseCustomPlantData data)
        where TBase : Plant
        where TClass : MonoBehaviour { }
    public static ID RegisterCustomPlant<TBase>(BaseCustomPlantData data)
        where TBase : Plant { }
    public static void AddCustomUltiPlant(PlantType thePlantType) => CustomUltiPlants.Add(thePlantType);
    /// <summary>
    /// Registers a custom plant and its skin in one call.
    /// Automatically registers TClass in IL2CPP.
    /// </summary>
    public static ID RegisterCustomPlant<TBase, TClass>(BasePlantSkinData skinData)
        where TBase : Plant
        where TClass : MonoBehaviour { }

    /// <summary>
    /// Registers a custom plant skin.
    /// Automatically registers TClass in IL2CPP.
    /// </summary>
    public static void RegisterCustomPlantSkin<TBase, TClass>(BasePlantSkinData skin)
        where TBase : Plant
        where TClass : MonoBehaviour
     { }
    #endregion
    #region Plant Helper
    public static void AddLevelPlant(ID type, CardLevel level)
    {
        int type_internal = type;
        if (CustomCardLevel.ContainsKey(type_internal)) ModLogger.LogError(MyPluginInfo.PluginName, "Duplicate ID type: " + type_internal);
        CustomCardLevel.Add(type_internal, level);
    }
    public static CardLevel GetCardLevel(PlantLevelData data) =>
        data switch
        {
            PlantLevelData.Basic => CardLevel.White,
            PlantLevelData.Secondary => CardLevel.Green,
            PlantLevelData.Super => CardLevel.Blue,
            PlantLevelData.WeakUltimate => CardLevel.Purple,
            PlantLevelData.StrongUltimate => CardLevel.Gold,
            PlantLevelData.FinalUltimate => CardLevel.Gold,
            PlantLevelData.TreasurePlant => CardLevel.Red,
            _ => CardLevel.White
        };
    public static string CreateAlmanacEntry(
        string introduction,
        string specialtext = "removeifthisisdefaulted",
        (string, string) recipe = default,
        (int damage, float interval) attackinterval = default,
        (int amount, float interval, string unit) produceinterval = default,
        string[]? specialeffects = null,
        (string, string) variantswitch = default,
        string feature = "removeifthisisdefaulted",
        string creator = "removeifthisisdefaulted",
        string usageconditions = "removeifthisisdefaulted",
        string flavor = "removeifthisisdefaulted")
     { }
    public static void AddCustomPlantUpgrade(ID fromType, ID toType, float percentChance)
     { }
    #endregion
    #region Misc Registration

    public static void RegisterCustomBigStar<TClass>(ref GameObject Star) where TClass : CustomBigStar
    { }

    public static ID RegisterCustomBullet<TBase, TClass>(BaseCustomBulletData data) where TBase : Bullet where TClass : MonoBehaviour
     { }

    /// <summary>
    /// Registers a plant as supporting Star-Up.
    /// </summary>
    public static void RegisterCustomStarUp(ID thePlantType)
        => CustomStarUps.Add(thePlantType);

    public static void AddCustomPlantUpgrade(ID fromType, ID toType, Func<Plant, bool> condition)
        => replaceList.TryAdd(fromType, (toType, condition));

    public static void AddCustomPlantUpgrade(ID fromType, ID toType, Func<bool> condition)
        => replaceList.TryAdd(fromType, (toType, (Plant p) => condition.Invoke()));
    public static ID RegisterCustomZombie<TBase, TClass>(BaseCustomZombieData data) where TBase : Zombie where TClass : MonoBehaviour
     { }

    public static void AddCustomOnZombieSpawnEvent(ID fromType, ID toType, Func<Zombie, bool> condition)
        => onZombieTypeSpawnActionList.TryAdd(fromType, (toType, condition));

    public static void AddGameStartAction(Action action)
     { }

    public static void AddGameAppInitAction(Action action)
     { }
    #endregion
    /// <summary>
    /// Registers a custom strong ultimate plant and sets it to ultimate if it is not. Return the BuffID of its unlock buff.
    /// </summary>
    public static void AddCustomStrongUltimatePlant(
        ID thePlantType,
        string BuffDescription,
        UltiBuff buff1,
        UltiBuff buff2,
        BuffBgType bg = default,
        PlantType? theVariantType = null)
     { }
    public static string FormatSPUpgradeBuff(
        string unlockedPlantName,
        string baseUltimateName,
        string parentPlant2Name,
        string unlockedVariantPlantName,
        string baseVariantUltimateName
    )
    { }
    public static string FormatStrongUltimateUnlockBuff
    (
        string unlockedPlantName,
        string basePlant1,
        string baseplant2,
        string unlockedVariantPlantName,
        string variantToBase,
        string baseToVariant
    )
     { }
    public static string FormatStrongUltimateUnlockBuff
    (
        string unlockedPlantName,
        string basePlant1,
        string baseplant2
    )
     { }
    /// <summary>
    /// Registers a custom level 4 zombie and sets its spawn level and weight
    /// </summary>
    public static ID AddCustomLevel4Zombie(ID theZombieType, ZombieType BaseLevel3)
    {
        Level4Zombies.TryAdd(BaseLevel3, theZombieType);
        AddCustomZombieSpawnRatio(theZombieType, 9, 0);
        return theZombieType;
    }
    public static Dictionary<ZombieType, ZombieType> Level4Zombies = new();

    /// <summary>
    /// Registers a custom weak ultimate plant and its variant and sets both to ultimate if it is not.
    /// </summary>
    public static void AddCustomWeakUltimatePlant(ID thePlantType, bool isVariant = true, PlantType theVariantType = PlantType.Nothing, Func<bool> canUnlock = null!)
     { }
    /// <summary>
    /// Registers a custom weak ultimate plant and sets it to ultimate if it is not.
    /// </summary>
    public static void AddCustomWeakUltimatePlant(ID thePlantType, bool isVariant = false)
     { }
    public static Dictionary<int, bool> CustomWeakUltiPlants = new();
    public static void AddZombieToLevel(LevelType theLevelType, int theLevelID, ZombieType theZombieType)
     { }
    #region Grid Items

    /// <summary>
    /// Registers a custom grid item.
    /// </summary>
    public static GridItemType RegisterCustomGridItem<TBase, TClass>(BaseCustomGridItemData data) where TBase : GridItem where TClass : MonoBehaviour
     { }

    /// <summary>
    /// Registers a custom grid item.
    /// </summary>
    public static GridItemType RegisterCustomGridItemWithType<TClass>(BaseCustomGridItemData data) where TClass : MonoBehaviour
     { }
    /// <summary>
    /// Registers a custom grid item.
    /// </summary>
    public static GridItemType RegisterCustomGridItem(BaseCustomGridItemData data)
     { }

    /// <summary>
    /// Registers a custom grid item.
    /// </summary>
    public static GridItemType RegisterCustomGridItem<TBase>(BaseCustomGridItemData data) where TBase : GridItem
    {
        EnsureGameNotStarted();
        GameObject gameObject = data.Prefab;
        if (!gameObject.TryGetComponent<GridItem>(out _)) gameObject.AddComponent<TBase>();
        GridItemType id = data.type.ToGridItemType();
        AddGameStartAction(() => GameAPP.resourcesManager.gridItemPrefabs[id] = gameObject);
        CustomGridItemTypes.Add(data.type.id);
        return id;
    }
    #endregion
    #region Levels
    #endregion
    /// <summary>
    /// Throws an InvalidOperationException if the game is started.
    /// </summary>
    public static void EnsureGameNotStarted()
     { }
    public static void AddLevelZombie(
        ZombieType A,
        string A_Desc,
        ZombieType B,
        string B_Desc,
        ZombieType C,
        string C_Desc,
        bool IsWater,
        ZombieType B2 = ZombieType.Nothing,
        string B2_Desc = "",
        ZombieType C2 = ZombieType.Nothing,
        string C2_Desc = ""
    )
     { }

    public static string RogueZombieTextFormatter(string name, int level, RogueZombieHealth healthDesc, RogueZombieAttack attackDesc, string special)
    => RogueZombieTextFormatter
    (
        name,
        level,
        healthDesc switch
        {
            RogueZombieHealth.VeryLow => "很低",
            RogueZombieHealth.Low => "低",
            RogueZombieHealth.Mid => "中",
            RogueZombieHealth.High => "高",
            RogueZombieHealth.VeryHigh => "很高",
            _ => "未知"
        },
        attackDesc switch
        {
            RogueZombieAttack.VeryLow => "很低",
            RogueZombieAttack.Low => "低",
            RogueZombieAttack.Mid => "中",
            RogueZombieAttack.High => "高",
            RogueZombieAttack.VeryHigh => "很高",
            RogueZombieAttack.Crashing => "碾压",
            _ => "未知"
        },
        special
    );

    public static string RogueZombieTextFormatter(string name, int level, string healthDesc, string attackDesc, string special)
    => $"{name?.Trim()}\n僵尸等级：{level}\n韧性：{healthDesc?.Trim()}\n攻击力：{attackDesc?.Trim()}\n特点：{special?.Trim()}\n";

    public static Dictionary<ZombieType, (int, int)> CustomZombieSpawns = new();
    public static void AddCustomZombieSpawnRatio(ID theZombieType, int level, int weight)
    {
        if (!CustomZombieSpawns.TryAdd(theZombieType, (level, weight)))
        {
            Debug.LogError("Duplicate zombie type in spawn ratio: " + (int)theZombieType);
        }
    }
}
public class DefaultPlantStats
{
    public const int BaseHealth = 300;
    public const int NutPlantHealth = 4000;
    public const int TallNutHealth = 8000;
    public const int UltimateNutHealth = 16000;
    public const int UltimateTallNutHealth = 32000;
    public const int UltimateObsidianJalaHealth = 64000;
}
public class DefaultZombieStats
{
    public const int NormalZombieDamage = 50;
    public const int NormalZombieHealth = 270;
    public const int ConeHealth = 370;
    public const int BucketHealth = 1100;
    public const int DoorHealth = 1100;
    public const int PaperHealth = 200;
    public const int HelmetHealth = 1400;
    public const int BrickHeadHealth = 2190; //2390 - 270 + 70 = this
    public const int GiantHealth = 3000;
    public const int UltiZombieA_Health_Light = 3000;
    public const int UltiZombieA_Health_Armored = 6000;
    public const int UltiZombieB_Health_Light = 6000;
    public const int UltiZombieB_Health_Armored = 12000;
    public const int UltiZombieC_Health_Light = 12000;
    public const int UltiZombieC_Health_Armored = 24000;
    public const int UltiZombieD_Health_Light = 24000;
    public const int UltiZombieD_Health_Armored = 48000;
}
public enum RogueZombieHealth
{
    VeryLow = -2,
    Low = -1,
    Mid = 0,
    High = 1,
    VeryHigh = 2
}
public enum RogueZombieAttack
{
    VeryLow = -2,
    Low = -1,
    Mid = 0,
    High = 1,
    VeryHigh = 2,
    Crashing = 100
}
```
</details>

3. ExtensionManager -> Contains extensions to various tools
4. GameObjectMgr -> A tool to store gameobjects, untested
5. GeneralTools -> unfinished
6. ListHelper -> An extension class
7. MathHelper -> Contains math tools
<details>
<summary>Click to show code</summary>

```csharp
public static class MathHelper
{
    // ============================================================
    //  ROTATION / QUATERNION HELPERS
    // ============================================================
    /// <summary>
    /// Converts a 2D direction vector into a Z‑axis rotation.
    /// This is a direct replacement for Core.Lawnf.GetRotateFromSpeed().
    ///
    /// Formula:
    ///     angle = atan2(direction.y, direction.x)
    ///     rotation = Quaternion.Euler(0, 0, angle_in_degrees)
    /// </summary>
    /// <param name="direction">A 2D direction vector.</param>
    /// <returns>A Quaternion facing the direction.</returns>
    public static Quaternion DirectionToRotation(Vector2 direction) { }

    /// <summary>
    /// Computes the facing angle (in degrees) of a 2D direction vector.
    /// This is the scalar form of Core.Lawnf.GetRotateFromSpeed().
    ///
    /// Formula:
    ///     angle = atan2(direction.y, direction.x)
    /// </summary>
    /// <param name="direction">A 2D direction vector.</param>
    /// <returns>The facing angle in degrees.</returns>
    public static float DirectionToDegrees(Vector2 direction) { }

    /// <summary>
    /// Converts a Z‑axis rotation into a normalized 2D direction vector.
    /// This is a direct replacement for Core.Lawnf.GetVectorFromQuaternion().
    ///
    /// Formula:
    ///     angle = rotation.eulerAngles.z * Deg2Rad
    ///     direction = (cos(angle), sin(angle)).normalized
    /// </summary>
    /// <param name="rotation">A Quaternion whose Z‑axis angle determines the direction.</param>
    /// <returns>A normalized Vector2 pointing in the facing direction.</returns>
    public static Vector2 RotationToDirection(Quaternion rotation) { }

    /// <summary>
    /// Converts a degree angle into a normalized 2D direction vector.
    /// This is the scalar form of Core.Lawnf.GetVectorFromQuaternion().
    ///
    /// Formula:
    ///     angle = rotationInDegrees * Deg2Rad
    ///     direction = (cos(angle), sin(angle)).normalized
    /// </summary>
    /// <param name="rotationInDegrees">A Z‑axis angle in degrees.</param>
    /// <returns>A normalized Vector2 pointing in the facing direction.</returns>
    public static Vector2 RotationToDirection(float rotationInDegrees) { }

    public static Quaternion LookAt2D(Vector2 from, Vector2 to) { }

    public static Quaternion RotateTowards2D(
        Quaternion current,
        float targetAngle,
        float maxDegreesPerSecond) { }

    public static Quaternion RandomRotation2D()
        => Quaternion.Euler(0, 0, Random.Range(0f, 360f));

    // ============================================================
    //  VECTOR HELPERS
    // ============================================================
    public static float DistanceSq(Vector2 a, Vector2 b) { }

    public static Vector2 RotateVector(Vector2 v, float degrees) { }

    public static Vector2 ClampMagnitude(Vector2 v, float max) { }

    // ============================================================
    //  RANDOM HELPERS
    // ============================================================
    public static T GetRandomValue<T>(List<T> values, Func<T, bool> selector = null) { }

    public static float GetRandomWithMean(float min, float max, float targetMean) { }

    public static float GetRandomLogSymmetric(float minRatio, float maxRatio) { }

    // ============================================================
    //  SCALAR HELPERS
    // ============================================================
    public static float Remap(float v, float a, float b, float c, float d)
        => c + (v - a) * (d - c) / (b - a);

    public static float RemapClamped(float v, float a, float b, float c, float d)
    {
        float t = Mathf.InverseLerp(a, b, v);
        return Mathf.Lerp(c, d, t);
    }

    public static bool ApproximatelyZero(float v, float eps = 0.0001f)
        => Mathf.Abs(v) < eps;

    // ============================================================
    //  BALLISTIC HELPERS
    // ============================================================
    public static Vector2 CalculateProjectileWithGravity(
        Vector2 projectilePos,
        Vector2 targetVelocity,
        Vector2 targetPos,
        float flightTime,
        float gravity) { }

    public static float[] CalculateProjectileWithSpeed(
        Vector2 projectilePos,
        Vector2 targetVelocity,
        Vector2 targetPos,
        float flightTime) { }

    public static float[] CalculateProjectileParameters(
        Vector2 startPos,
        float t1,
        Vector2 firstPlace,
        float t2,
        Vector2 secondPlace,
        float flightTime) { }

    // ============================================================
    //  COROUTINE HELPERS
    // ============================================================
    public static IEnumerator SmoothRotate(
        Transform t,
        Quaternion target,
        float smoothTime = 0.5f,
        float rotationSpeed = 0.6f) { }
    public static int NextEmptyIndex<T>(object source, Func<T, bool> isEmpty = null) { }
    public static List<T> GetEnumValues<T>() where T : Enum => [.. (T[])typeof(T).GetEnumValues()];
    public static string FormatToChineseUnits(this long num) { }
    public static string FormatToChineseUnits(this int num) { }
    public static string FormatToScientificNotation(this long num) { }
    public static string FormatToScientificNotation(this int num) { }
}
```
</details>

8. ModRegistryManager -> A way for mods to make registries for other mods to add stuff to
9. PlantMgr -> Tools

10. ModPlugin -> a plugin base
<details>
<summary>Click to show code</summary>

```csharp
    public class ModPlugin : BasePlugin
    {
        public ManualLogSource Logger;
        public virtual void InitializeMod() { }
        public virtual void OnStart() { }
        public virtual void InitializeBuffs() { }
        public virtual void InitializePlants() { }
        public virtual void InitializeZombies() { }
        public virtual void InitializeConditions() { }
        public virtual void OnGameStart() { }
        public virtual void OnGameInit() { }
        public virtual void OnDataMgrLoad() { }
    }
```
</details>

11. StructManager
<details>
<summary>Click to show code</summary>

```csharp
/// <summary>
/// Core metadata for defining a custom plant.
/// </summary>
public struct BaseCustomPlantData
{
    public ID PlantId;
    public GameObject Prefab;
    public GameObject Preview;

    public List<(ID, ID)> Fusions;
    public float AttackInterval;
    public float ProduceInterval;
    public int AttackDamage;
    public int MaxHealth;
    public float Cd;
    public int Sun;
    public BulletType DefaultBullet;
    public bool CanPF;
    public bool CanStarUp;
    public CardLevel CardColor;
    public bool IsRainbowCard;
    public bool IsUltimatePlant;
    public int CardRepeatAmt;
    public string Name;
    public string AlmanacEntry;

    public static BaseCustomPlantData Create(
        ID id,
        GameObject prefab,
        GameObject preview
    ) => new BaseCustomPlantData
    {
        PlantId = id,
        Prefab = prefab,
        Preview = preview,

        // defaults
        Fusions = [],
        AttackInterval = 0f,
        ProduceInterval = 0f,
        AttackDamage = 0,
        MaxHealth = 300,
        Cd = 0f,
        Sun = 0,
        DefaultBullet = BulletType.Bullet_pea,
        CanPF = false,
        CanStarUp = false,
        CardColor = CardLevel.White,
        IsRainbowCard = false,
        IsUltimatePlant = false,
        CardRepeatAmt = 1,
        Name = "",
        AlmanacEntry = ""
    };
}

/// <summary>
/// Metadata for defining a custom plant skin.
/// </summary>
public struct BasePlantSkinData
{
    public BaseCustomPlantData data;
    public GameObject SkinPrefab;
    public GameObject SkinPreview;
    public List<(BulletType, List<GameObject?>)> BulletSkinList;
}

/// <summary>
/// Metadata for defining a bullet.
/// </summary>
public struct BaseCustomBulletData
{
    public ID BulletId;
    public GameObject Prefab;

}
public struct BaseCustomZombieData
{
    public ID theZombieType;
    public GameObject Prefab;
    public Sprite Preview;
    public int theAtackDamage;
    public int maxHealth;
    public int theFirstArmorHealth;
    public FirstArmorType theFirstArmorType;
    public string theFirstArmorPath;
    public int theSecondArmorHealth;
    public SecondArmorType theSecondArmorType;
    public string theSecondArmorPath;
    public int SpawnLevel;
    public int SpawnWeight;
}
public struct BaseCustomGridItemData
{
    public ID type;
    public GameObject Prefab;
}
public struct CustomBossHealthSliderData
{
    public ZombieType theZombieType = ZombieType.Nothing;
    public Sprite? Icon;
    public Sprite? FillIcon;
    public Color FillColor = Color.magenta;

    public CustomBossHealthSliderData(Sprite icon)
    {
    }
}
public struct BoardPosition
{
    public int Row { get; }
    public int Column { get; }

    public BoardPosition(int row, int column)
    {
        Row = row;
        Column = column;
    }

    // BoardPosition → world position
    public static implicit operator Vector2(BoardPosition pos)
    {
        float x = pos.Column * 1.35f - 4.8f;

        Board board = Instance;
        bool roof = board.boardTag.isRoof;
        int rows = board.rowNum;

        float y;

        if (!roof)
        {
            if (rows == 6)
                y = 2.3f - pos.Row * 1.45f;
            else
                y = 2.3f - pos.Row * 1.67f;
        }
        else
        {
            // Roof math
            if (x <= 1.5f)
            {
                float f = (pos.Row * 1.4f);
                y = 1.6f - f + x * 0.22f + 0.5f;
            }
            else
            {
                y = 4.0f - pos.Row * 1.45f;
            }
        }

        return new Vector2(x, y);
    }

    // world position → BoardPosition
    public static implicit operator BoardPosition(Vector2 world)
    {
        float x = world.x;
        float y = world.y;

        Board board = Instance;
        bool roof = board.boardTag.isRoof;
        int rows = board.rowNum;

        // Column
        int col = Mathf.FloorToInt((x + 5.6f) / 1.35f);
        col = Mathf.Clamp(col, 0, board.columnNum - 1);

        // Row
        int row;

        if (!roof)
        {
            if (rows == 6)
                row = Mathf.FloorToInt((3.7f - y) / 1.45f);
            else
                row = Mathf.FloorToInt((3.7f - y) / 1.67f);
        }
        else
        {
            if (x <= 1.5f)
            {
                float f = (y - x * 0.22f) - 0.5f;
                row = Mathf.FloorToInt((1.6f - f) / 1.4f) + 1;
            }
            else
            {
                row = Mathf.FloorToInt((4.0f - y) / 1.45f);
            }
        }

        row = Mathf.Clamp(row, 0, rows - 1);

        return new BoardPosition(row, col);
    }

    public override string ToString() => $"({Row}, {Column})";
}
public struct Struct1_Plant
{
    public Type BaseType;
    public Type CustomType;
    public BaseCustomPlantData data;
}
public struct BaseCustomLevelData
{
    public BaseCustomLevelData()
    {
    }

    public LevelType LevelType { readonly get; set; } = LevelType.Nothing;
    public int LevelID { readonly get; set; } = -114514;
    public string LevelName { readonly get; set; } = "";
    public string LevelNameEn { readonly get; set; } = "";
    public Sprite? LevelSprite { readonly get; set; } = default;
    public SceneType SceneType { readonly get; set; } = SceneType.Day_6;
    public GameObject? ScenePrefab { readonly get; set; } = default;
    public Sprite? SceneBackground { readonly get; set; } = default;
    public MusicType MusicType { readonly get; set; } = (MusicType)(-1);
    public AudioClip? MusicAudio { readonly get; set; } = default;
    public int MaxWave { readonly get; set; } = 100;
    public List<ZombieType> ZombieTypes { readonly get; set; } = new() { ZombieType.RandomZombie, ZombieType.RandomPlusZombie, ZombieType.DiamondRandomZombie };
    public BoxType_Short[,] MapRoadTypes { readonly get; set; } = new BoxType_Short[,] { };
    public CustomLevelSelection selection { readonly get; set; } = default;
    public PlantType[] SelectTypes { readonly get; set; } = new PlantType[] { };
    public Action EnterAction { readonly get; set; } = () => { };
    public Action<Board> EnterGameAction { readonly get; set; } = (Board b) => { };
    public int SunCounter { readonly get; set; } = default;
    public List<AdvBuff> AdvBuffs { readonly get; set; } = new();
    public List<UltiBuff> UltiBuffs { readonly get; set; } = new();
    public List<TravelUnlocks> TravelUnlocks { readonly get; set; } = new();
    public List<TravelDebuff> TravelDebuffs { readonly get; set; } = new();
    public BoardTag BoardTag { readonly get; set; } = default;
}
public enum CustomLevelSelection
{
    Normal = 0,
    Convey = 1,
    PreSelected = 2
}
public enum BossSliderType
{
    UltimateSword = 0,
    ObsidianGarcantuar = 1,
    UltimateDrown = 2,
    UltimateFootball = 3,
    UltimateHorse = 4,
    UltimateImp = 5,
    UltimateJackbox = 6,
    UltimateJackson = 7,
    UltimateKirov = 8,
    UltimateLegion = 9,
    UltimateMachineNut = 10,
    UltimatePaper = 11,
    UltimateSnow = 12
}
public enum PlantLevelData
{
    Basic = 0,
    Secondary = 1,
    Super = 2,
    WeakUltimate = 3,
    StrongUltimate = 4,
    FinalUltimate = 5,
    TreasurePlant = 6
}
public enum BoxType_Short
{
    G = 0,         // 草地
    W = 1,         // 水域
    D = 2,         // 泥土
    R = 3,         // 屋顶
    S = 4,         // 石头
    River = 5,     // 河流
    Dirt_water = 6 // 泥水域
}
```
</details>

12. ZombieMgr -> Some zombie helper class

<details>
<summary>Click to show code</summary>

```csharp
public static class ZombieMgr
{
    public static void LosePaper(this PaperZombie self) { }
    public static void LosePaper(this GatlingPaperZombie_a self) { }
    public static void FlyAway(this Zombie self) { }
    public static void Crashed(this Zombie self) { }
}
```
</details>

</details>

### CustomPlantClass.Runtime

1. BoardBehaviour -> A class for storing behaviour that is called by board
<details>
<summary>Click to show code</summary>

```csharp
public class BoardBehaviour : MonoBehaviour
{
    public static void AddStartEvent(Action<Board> action) { }
    public static void AddUpdateEvent(Action<Board> action) { }
    public static void AddFixedUpdateEvent(Action<Board> action) { }
    public static void AddDestroyEvent(Action<Board> action) { }
}
[AttributeUsage(AttributeTargets.Field)]
public sealed class ResetOnBoardDestroyAttribute : Attribute
{
    public object DefaultValue { get; }

    public ResetOnBoardDestroyAttribute() { }

    public ResetOnBoardDestroyAttribute(object defaultValue)
    {
        DefaultValue = defaultValue;
    }
}
[AttributeUsage(AttributeTargets.Field)]
public sealed class ActionOnBoardDestroyAttribute : Attribute
{
    public Func<object> Action { get; }

    public ActionOnBoardDestroyAttribute(Func<object> action)
    {
        Action = action;
    }
}
```
</details>

### CustomPlantClass.Networking:

1. TCPManager -> used to send messages locally or on a localhost server
<details>
<summary>Click to show code</summary>

```csharp
public static class TCPManager
{
    public static void SendMessage(string message, string data) { } 
    public static void SendMessageLocal(string message, string data)
    {
        commandQueue.Enqueue((message, data));
    }
    public static void PingMod(string modName, Action<string> callBack = null, string data = "")
    {
        callBack = callBack ?? ((s) => { });
        commandQueue.Enqueue((modName+"Ping", data));
        callBacks.Add((modName+"Pong",callBack));
    }
    public static void StartClient(string ip, int port)
    {
        _running = true;
        Task.Run(() => RunClientAsync(ip, port));
    }
    public static void StartServer(int port)
    {
        _running = true;
        Task.Run(() => RunServerAsync(port));
    }
    public static void StartAuto(int port)
    {
        _running = true;

        Task.Run(async () =>
        {
            try
            {
                await RunServerAsync(port).ConfigureAwait(false);
            }
            catch
            {
                await RunClientAsync("127.0.0.1", port).ConfigureAwait(false);
            }
        });
    }
    public static bool IsConnected
    {
        get
        {
            WebSocket activeSocket = ActiveSocket;
            return activeSocket != null && activeSocket.State == WebSocketState.Open;
        }
    }
    private static WebSocket ActiveSocket => _clientSocket ?? _serverSocket;
    public static void RegisterCommandListener(ICommandListener listener)
    {
        commandListeners.Add(listener);
    }
}
public interface ICommandListener
{
    public string CommandName { get; }
    public void OnCommandReceived(string data);
}
public abstract class PingListener : ICommandListener
{
    public abstract string Name { get; }
    public string CommandName => Name + "Ping";
    public virtual string Data => "";

    public void OnCommandReceived(string data)
    {
        OnRecieved(data);
        TCPManager.SendMessageLocal(Name+"Pong",Data);
    }
    public virtual void OnRecieved(string data) { }
}
```
</details>

### CustomPlantClass.Runtime.CSharp:
1. This namespace is only a dll and is used to compile code on the go. An example mod:

<details>
<summary>Click to show code</summary>

```csharp
using HarmonyLib;
using GameLevel.RogueShooting;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Reflection;
using Unity.VisualScripting;
using CustomPlantClass.Runtime.CSharp;
public class UltimateFumeBuff_Main : ICompilableScript
{
    public string Name => "UltimateFumeBuff";

    public void Main()
    {
        if(!ClassInjector.IsTypeRegisteredInIl2Cpp<UltimateCactusBulletBuff>())
            ClassInjector.RegisterTypeInIl2Cpp<UltimateCactusBulletBuff>();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(),"UltimateFumeBuff_Harmony");
    }

    public void Dispose()
    {
        Harmony.UnpatchID("UltimateFumeBuff_Harmony");
    }
}

public class UltimateCactusBulletBuff : MonoBehaviour
{
    public Bullet __instance => GetComponent<Bullet>();
    public void Start()
    {
        if (__instance.fromType == PlantType.UltimateFume && ShootingManager.Instance != null && ShootingManager.Instance.TryGetPlant(PlantType.UltimateFume, out Plant plant) && plant != null && plant.starUp)
        {
            __instance.MoveWay = BulletMoveWay.Free;
            __instance.transform.Rotate(0f,0f,Random.Range(-15f,15f));
        }
        Destroy(this);
    }
}
[HarmonyPatch(typeof(Bullet_ultimateCactus), nameof(Bullet_ultimateCactus.SetPenetrationTime))]
public static class Bullet_ultimateCactus_SetPenetrationTime_Patch
{
    [HarmonyPostfix]
    public static void Postfix(Bullet_ultimateCactus __instance)
    {
        if( __instance is not Bullet_ultimateCactus ) return; // Il2Cpp compresses methods that are the same
        __instance.AddComponent<UltimateCactusBulletBuff>();
    }
}
```
</details>

### CustomPlantClass.RogueShootingManager:

<details>
<summary>Click to show code</summary>

```csharp
public class RegistryHelper : MonoBehaviour
{
    //planning: use the dictionary lookup approach but modify it to use a compile time set string inside the class
    public static BaseConfig MakeConfigType(CustomRogueShootingConfig spec) { }
    public static BaseBuff MakeBuffType(CustomRogueShootingBuff buff) { }
    public static void AddCustomExpertPlant(PlantType thePlantType, BaseConfig config) { }
    public static void AddCustomRogueShootingPlant(PlantType thePlantType, BaseConfig config) { }
    public static void AddCustomBaseRogueShootingPlant(PlantType thePlantType, BaseConfig config) { }
    public static void InjectUpgradeBuff(RSConfigType BasePlant, PlantType resultPlant) { }
    public const string QualitativeChangeText = "质变";
    public const string CurseText = "诅咒";
    public const string ReversedCurseText = "祝福";
    internal static 
    Dictionary<BuffID,(BuffID reversed,Func<Plant,bool> canreverse,Action oncurse,Action onreverse)> CurseBuffInfo = new();
    public static (BuffID curseBuff, BuffID reverseBuff, BaseBuff buffConfig) RegisterCustomCurseBuff(string name, string curseDesc, string reversedDesc, PlantType thePlantType, Func<Plant,bool> CanReverse, Action OnCurse = null, Action OnReverseEvent = null) { }
    public static string GetStringFromRole(Roles role)
    {
        switch (role)
        {
            case Roles.Attacker: return "输出";
            case Roles.Supporter: return "辅助";
            case Roles.Defense: return "防御";
            case Roles.Insta: return "灰烬";
            default:
            case Roles.Producer: return "未知";
        }
    }
    public static string FormatBuffTitle(ShootingBuffType buffType, string Title)
    {
        switch (buffType)
        {
            case ShootingBuffType.General:
            case ShootingBuffType.UniqueUpgrade:
            default:
            return $"强化：{Title}";
            case ShootingBuffType.QualitativeChange:
            return $"质变：{Title}";
            case ShootingBuffType.SuperUpgrade:
            return $"超进化：{Title}";
            case ShootingBuffType.CurseBuff:
            return $"诅咒：{Title}";
        }
    }
    public static (BuffID AdvBuff,BaseBuff buffConfig) RegisterCustomQualitativeChangeBuff(string name, string desc, PlantType thePlantType, Action OnGetBuff = null) { }
    private static bool IsManagedAssembly(string path) { }
}
public enum Roles
{
    Attacker = 0,
    Supporter = 1,
    Defense = 2,
    Insta = 3,
    Producer = 4
}
public enum RSConfigType
{
    Peashooter,
    CherryGatling,
    HelmetGatling,
    //terminal plant, it is not recommended to add entries here
    UltimateGatling,
    //terminal plant, it is not recommended to add entries here
    UltimateHelmetGatling,
    LanternSplit,
    //terminal plant, it is not recommended to add entries here
    UltimateLanternSplit,
    SniperPea,
    //terminal plant, it is not recommended to add entries here
    DoomSniper,
    //terminal plant, it is not recommended to add entries here
    FireSniper,
    SnowPeaShooter,
    //terminal plant, it is not recommended to add entries here
    MagicSnowPea2,
    WallNut,
    SuperChomper,
    //terminal plant, it is not recommended to add entries here
    UltimateChomper,
    TallNut,
    //terminal plant, it is not recommended to add entries here
    UltimateTallNut,
    CabbageNut,
    //terminal plant, it is not recommended to add entries here
    MelonNut,
    //terminal plant, it is not recommended to add entries here
    MagnetNut,
    PotatoMine,
    PeaMine,
    //terminal plant, it is not recommended to add entries here
    ThreeMine,
    Chomper,
    CherryChomper,
    //terminal plant, it is not recommended to add entries here
    DoomChomper,
    BigChomper,
    //terminal plant, it is not recommended to add entries here
    UltimateBigChomper,
    SmallPuff,
    IcePuff,
    //terminal plant, it is not recommended to add entries here
    SnowGatlingPuff,
    IronPuff,
    //terminal plant, it is not recommended to add entries here
    IFVIronPuff,
    FumeShroom,
    IceFumeShroom,
    //terminal plant, it is not recommended to add entries here
    UltimateFume,
    GarlicFume,
    //terminal plant, it is not recommended to add entries here
    UltimatePoisonFume,
    GloomShroom,
    //terminal plant, it is not recommended to add entries here
    UltimateGloom,
    HypnoShroom,
    HypnoNut,
    //terminal plant, it is not recommended to add entries here
    HypnoEmperor,
    ScaredyShroom,
    SuperHypno,
    //terminal plant, it is not recommended to add entries here
    UltimateHypno,
    ScaredyDoom,
    //terminal plant, it is not recommended to add entries here
    UltimateDoomScared,
    Squash,
    Squalour,
    //terminal plant, it is not recommended to add entries here
    CattailLour,
    CherrySquash,
    //terminal plant, it is not recommended to add entries here
    NuclearSquash,
    ThreePeater,
    ThreeSquash,
    //terminal plant, it is not recommended to add entries here
    SuperThreePeater,
    BigGatling,
    //terminal plant, it is not recommended to add entries here
    UltimateBigGatling,
    Caltrop,
    SpikeRock,
    //terminal plant, it is not recommended to add entries here
    ObsidianSpike,
    CaltropNut,
    //terminal plant, it is not recommended to add entries here
    ObsidianWallNut,
    Cactus,
    DoomCactus,
    //terminal plant, it is not recommended to add entries here
    UltimateCactus,
    StarFruit,
    SuperStar,
    //terminal plant, it is not recommended to add entries here
    UltimateStar,
    SwordStar,
    //terminal plant, it is not recommended to add entries here
    AbyssSwordStar,
    Cabbagepult,
    GoldCabbage,
    UltimateCabbage,
    CabbageCannon,
    //terminal plant, it is not recommended to add entries here
    UltimateCabbageCannon,
    Melonpult,
    SuperMelon,
    //terminal plant, it is not recommended to add entries here
    UltimateMelon,
    FireMelon,
    //terminal plant, it is not recommended to add entries here
    UltimateSpring,
    SilverMelon,
    GoldMelon,
    WinterMelon,
    //terminal plant, it is not recommended to add entries here
    UltimateWinterMelon,
    Cornpult,
    PortalCorn,
    //terminal plant, it is not recommended to add entries here
    UltimateCorn,
    Umbrellaleaf,
    LanternUmbrella,
    //terminal plant, it is not recommended to add entries here
    LaserUmbrella,
    Bamboo,
    LotusBamboo,
    //terminal plant, it is not recommended to add entries here
    UltimateBamboo,
    SpruceShooter,
    SuperSpruce,
    //terminal plant, it is not recommended to add entries here
    UltimateSpruce,
    //expert plant, it is not recommended to add entries here
    UltimateSniperGatling,
    //expert plant, it is not recommended to add entries here
    UltimateMinigun,
    //expert plant, it is not recommended to add entries here
    UltimateBlover,
    //expert plant, it is not recommended to add entries here
    EmeraleBlover,
    //expert plant, it is not recommended to add entries here
    UltimateStarTorch
}
public struct CustomRogueShootingConfig
{
    public CustomRogueShootingConfig()
    {
    }

    public PlantType CustomPlantType { get; set; } = PlantType.Nothing;
    public Func<List<BaseBuff>> CustomBuffs { get; set; } = () => new();
    public Action<Plant> CustomReinforcePlant { get; set; } = null;
    public string CustomRole { get; set; } = "";
}
public struct CustomRogueShootingBuff
{
    public CustomRogueShootingBuff()
    {
    }

    public PlantType CustomPlantType { get; set; } = PlantType.Nothing;
    public string CustomTitle { get; set; } = "";
    public string CustomDescription { get; set; } = "";
    public ShootingBuffType CustomBuffType { get; set; } = ShootingBuffType.UniqueUpgrade;
    public Action CustomOnGet { get; set; } = () => {};
}
```
</details>

### CustomPlantClass.Runtime.Tasks:

<details>
<summary>Click to show code</summary>

```csharp
public class Delay : IDelay
{
    private bool _isCompleted;
    private Action _continuation;
    private readonly CancellationToken _token;

    // Mode 1: with cancellation
    public Delay(CancellationToken token)
    {
        _token = token;
    }

    // Mode 2: without cancellation
    public Delay()
    {
        _token = null;
    }

    public bool IsCompleted => _isCompleted || (_token?.IsCanceled ?? false);

    public void OnCompleted(Action continuation)
    {
        _continuation = continuation;
    }

    public void Complete()
    {
        if (_token?.IsCanceled ?? false)
        {
            _isCompleted = true;
            return;
        }

        _isCompleted = true;
        _continuation?.Invoke();
    }

    public void GetResult() { }
}
public class DelayScaled : IDelay
{
    private bool _isCompleted;
    private Action _continuation;
    private readonly CancellationToken _token;
    public float remaining;
    public Func<float> speedMultiplier; // dynamic multiplier

    public DelayScaled(float seconds, Func<float> speed, CancellationToken token = null)
    {
        remaining = seconds;
        speedMultiplier = speed;
        _token = token;
    }

    public bool IsCompleted => _isCompleted || (_token?.IsCanceled ?? false);

    public void OnCompleted(Action continuation) => _continuation = continuation;

    public void Complete()
    {
        if (_isCompleted) return;
        _isCompleted = true;
        _continuation?.Invoke();
    }

    public void GetResult() { }
}
public class WaitUntil : IDelay
{
    private bool _isCompleted;
    private Action _continuation;
    private readonly CancellationToken _token;
    private readonly Func<bool> _predicate;

    // with cancellation
    public WaitUntil(Func<bool> predicate, CancellationToken token)
    {
        _predicate = predicate;
        _token = token;
    }

    // without cancellation
    public WaitUntil(Func<bool> predicate)
    {
        _predicate = predicate;
        _token = null;
    }

    public bool IsCompleted => _isCompleted || (_token?.IsCanceled ?? false);

    public void OnCompleted(Action continuation)
    {
        _continuation = continuation;
    }

    internal bool Check()
    {
        if (_isCompleted) return true;
        if (_token?.IsCanceled ?? false)
        {
            _isCompleted = true;
            return true;
        }

        if (_predicate != null && _predicate())
        {
            Complete();
            return true;
        }

        return false;
    }

    public void Complete()
    {
        if (_isCompleted) return;

        _isCompleted = true;
        _continuation?.Invoke();
    }

    public void GetResult() { }
}
public readonly struct WaitUntilTask
{
    private readonly WaitUntil _awaiter;

    public WaitUntilTask(WaitUntil awaiter)
    {
        _awaiter = awaiter;
    }

    public WaitUntil GetAwaiter() => _awaiter;

    // with cancellation
    public static WaitUntilTask WaitUntil(Func<bool> predicate, CancellationToken token)
    {
        var awaiter = new WaitUntil(predicate, token);
        WaitUntilScheduler.Schedule(awaiter);
        return new WaitUntilTask(awaiter);
    }

    // without cancellation
    public static WaitUntilTask WaitUntil(Func<bool> predicate)
    {
        var awaiter = new WaitUntil(predicate);
        WaitUntilScheduler.Schedule(awaiter);
        return new WaitUntilTask(awaiter);
    }
}
public struct DelayTask
{
    private readonly IDelay _awaiter;

    public DelayTask(IDelay awaiter)
    {
        _awaiter = awaiter;
    }

    public IDelay GetAwaiter() => _awaiter;

    // Mode 1: with cancellation
    public static DelayTask Delay(float seconds, CancellationToken token)
    {
        var awaiter = new Delay(token);
        DelayScheduler.Schedule(seconds, awaiter);
        return new DelayTask(awaiter);
    }

    // Mode 2: without cancellation
    public static DelayTask Delay(float seconds)
    {
        var awaiter = new Delay();
        DelayScheduler.Schedule(seconds, awaiter);
        return new DelayTask(awaiter);
    }

    // FixedUpdate (with cancellation)
    public static DelayTask WaitForFixedUpdate(CancellationToken token)
    {
        var awaiter = new Delay(token);
        DelayScheduler.ScheduleFixedUpate(awaiter);
        return new DelayTask(awaiter);
    }

    // FixedUpdate (without cancellation)
    public static DelayTask WaitForFixedUpdate()
    {
        var awaiter = new Delay();
        DelayScheduler.ScheduleFixedUpate(awaiter);
        return new DelayTask(awaiter);
    }

    // FixedUpdate steps (with cancellation)
    public static DelayTask WaitForFixedUpdate(int steps, CancellationToken token)
    {
        var awaiter = new Delay(token);
        for (int i = 0; i < steps; i++)
            DelayScheduler.ScheduleFixedUpate(awaiter);
        return new DelayTask(awaiter);
    }

    // FixedUpdate steps (without cancellation)
    public static DelayTask WaitForFixedUpdate(int steps)
    {
        var awaiter = new Delay();
        for (int i = 0; i < steps; i++)
            DelayScheduler.ScheduleFixedUpate(awaiter);
        return new DelayTask(awaiter);
    }
    public static DelayTask DelayScaled(float seconds, Func<float> speed, CancellationToken token = null)
    {
        var awaiter = new DelayScaled(seconds, speed, token);
        DelayScheduler.ScheduleScaled(awaiter);
        return new DelayTask(awaiter);
    }
}
public class DelayScheduler : MonoBehaviour
{
    private class Entry
    {
        public IDelay awaiter;
        public float remaining;
        public bool useFixedUpdate;
        public bool isScaled;
        public Action WhenDone = null;
    }

    private static readonly List<Entry> entries = new();

    public static void Schedule(float seconds, Delay awaiter)
    {
        entries.Add(new Entry
        {
            awaiter = awaiter,
            remaining = seconds,
            useFixedUpdate = false,
            isScaled = false
        });
    }

    public static void ScheduleScaled(DelayScaled awaiter)
    {
        entries.Add(new Entry
        {
            awaiter = awaiter,
            remaining = awaiter.remaining,
            useFixedUpdate = false,
            isScaled = true
        });
    }

    public static void ScheduleFixedUpate(Delay awaiter)
    {
        entries.Add(new Entry
        {
            awaiter = awaiter,
            remaining = 0f,
            useFixedUpdate = true,
            isScaled = false
        });
    }

    public static void Schedule(float seconds, Delay awaiter, Action whenDone)
    {
        entries.Add(new Entry
        {
            awaiter = awaiter,
            remaining = seconds,
            useFixedUpdate = false,
            isScaled = false,
            WhenDone = whenDone
        });
    }

    public static void ScheduleScaled(DelayScaled awaiter, Action whenDone)
    {
        entries.Add(new Entry
        {
            awaiter = awaiter,
            remaining = awaiter.remaining,
            useFixedUpdate = false,
            isScaled = true,
            WhenDone = whenDone
        });
    }

    public static void ScheduleFixedUpate(Delay awaiter, Action whenDone)
    {
        entries.Add(new Entry
        {
            awaiter = awaiter,
            remaining = 0f,
            useFixedUpdate = true,
            isScaled = false,
            WhenDone = whenDone
        });
    }

    public void FixedUpdate()
    {
        for (int i = entries.Count - 1; i >= 0; i--)
        {
            var e = entries[i];

            if (!e.useFixedUpdate)
                continue;

            if (e.awaiter.IsCompleted)
            {
                entries.RemoveAt(i);
                continue;
            }

            e.awaiter.Complete();
            if(e.WhenDone != null)
            {
                try
                {
                    e.WhenDone();
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
            entries.RemoveAt(i);
        }
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        for (int i = entries.Count - 1; i >= 0; i--)
        {
            var e = entries[i];

            if (e.useFixedUpdate)
                continue;

            if (e.awaiter.IsCompleted)
            {
                entries.RemoveAt(i);
                continue;
            }

            float mult = 1f;

            if (e.isScaled && e.awaiter is DelayScaled scaled)
                mult = scaled.speedMultiplier?.Invoke() ?? 1f;

            e.remaining -= dt * mult;

            if (e.remaining <= 0f)
            {
                e.awaiter.Complete();
                if(e.WhenDone != null)
                {
                    try
                    {
                        e.WhenDone();
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }
                entries.RemoveAt(i);
            }
        }
    }
}
public class WaitUntilScheduler : MonoBehaviour
{
    private class Entry
    {
        public WaitUntil awaiter;
        public Action action; // null if no action
    }

    private static readonly List<Entry> entries = new();

    public static void Schedule(WaitUntil awaiter)
    {
        entries.Add(new Entry { awaiter = awaiter, action = null });
    }

    public static void Schedule(WaitUntil awaiter, Action action)
    {
        entries.Add(new Entry { awaiter = awaiter, action = action });
    }

    public void Update()
    {
        for (int i = entries.Count - 1; i >= 0; i--)
        {
            var e = entries[i];
            var w = e.awaiter;

            if (w.IsCompleted || w.Check())
            {
                // run action if present
                if (e.action != null)
                {
                    try
                    {
                        e.action();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                }

                entries.RemoveAt(i);
            }
        }
    }
}
public class CancellationToken
{
    public bool IsCanceled { get; private set; }

    public void Cancel() => IsCanceled = true;

    public static CancellationToken CancelAfterFixedUpate()
    {
        var token = new CancellationToken();
        DelayScheduler.ScheduleFixedUpate(new Delay(), () => token.Cancel());
        return token;
    }
    public static CancellationToken CancelAfterFixedUpate(int steps)
    {
        var token = new CancellationToken();
        _ = WaitForFixedUpdates(steps,token);
        return token;
    }
    private async static Task WaitForFixedUpdates(int steps,CancellationToken token)
    {
        await DelayTask.WaitForFixedUpdate(steps);
        token.Cancel();
    }
    public static CancellationToken CancelAfterSeconds(float seconds)
    {
        var token = new CancellationToken();
        DelayScheduler.Schedule(seconds, new Delay(), () => token.Cancel());
        return token;
    }
    public static CancellationToken CancelWhen(Func<bool> predicate)
    {
        var token = new CancellationToken();
        WaitUntilScheduler.Schedule(new WaitUntil(predicate), () => token.Cancel());
        return token;
    }
}
public static class CancellationTokenExt
{
    public static CancellationToken CreateCancellationToken(this MonoBehaviour self)
    {
        var token = new CancellationToken();
        WaitUntilScheduler.Schedule(new WaitUntil(() => self.destroyCancellationToken.IsCancellationRequested), () => token.Cancel());
        return token;
    }
    public static CancellationToken CreateCancellationToken(this GameObject self) =>
        self.TryGetComponent<MonoBehaviour>(out var mono)
            ? CreateCancellationToken(mono)
            : self.GetOrAddComponent<MonobehaviourCancellationToken>().Token;
    public static CancellationToken CreateCancellationToken(this Component self) =>
        self.TryGetComponent<MonoBehaviour>(out var mono)
            ? CreateCancellationToken(mono)
            : self.GetOrAddComponent<MonobehaviourCancellationToken>().Token;
    public class MonobehaviourCancellationToken : MonoBehaviour
    {
        public CancellationToken Token { get; private set; }

        public void Awake()
        {
            Token = new CancellationToken();
        }

        public void OnDestroy()
        {
            Token.Cancel();
        }
    }
}
public interface IDelay : INotifyCompletion
{
    public bool IsCompleted { get; }
    public void Complete();
    public void GetResult();
}
```
</details>
