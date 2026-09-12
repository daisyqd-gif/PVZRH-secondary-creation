# PVZ Fusion Custom Plant Class Framework

A modular, extensible gameplay framework for Plants vs. Zombies Fusion that allows modders to create custom plants, zombies, effects, projectiles, and gameplay systems using clean C# APIs.
## Overview
This framework provides a unified API for extending PVZ Fusion without modifying the game’s internal code. It is designed for modders who want to build:
- new plants
- new zombies
- new projectiles
- new effects
- new mechanics
- new levels
- etc...
  
All through a stable C# interface.
It is built on top of:
- CustomizeLib (Apache‑2.0)
- BepInEx (LGPL‑2.1)
- PVZ Fusion’s internal gameplay architecture
- [![Roslyn](https://img.shields.io/nuget/v/Microsoft.CodeAnalysis.CSharp?label=Roslyn&color=blue)](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp/)
- ![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
- -![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
## Features
1. Base classes for plants, zombies, levels, etc...
2. A central framework for registration
3. Convinence helpers for easier coding
4. A custom mod loader ( Incomplete )
5. Registries and an event manager for cross mod communication
6. Custom structs and data types for easier data storage
7. Limited and untested support for the easier .NET based F# language
8. A central command dispatcher for mods to communicate with each other and external apps via TCP
9. An async state machine runner that replaces UniTask and Task.Delay
10. Python like support for collections
11. A bridge for converting data from Il2cpp collections to managed collections
12. Runtime compilation of mods
13. An interrupt system
### Future
1. A way to run mods in Lua
2. A visual plant creator that allows modders or designers to create plants with minimal coding
3. Full support for the easier .NET based F# language
4. Sub frame timing in the async state machine runner
5. A repl for compiling code at runtime
## Installation
1. Install this custom fork of BepInEx 6.0.0 and CustomizeLib from https://pan.quark.cn/s/6461fdaccff5#/list/share -> 融合版/融合Mod/鲑鱼MOD整理/BepinEX版本/BepInEx前置框架 and download all files in the folder and place them into the game's directory (where PlantsVsZombiesRH.exe is located) and also locate customizelib in any of salmon's mods and put it in. TEMPORARY WARNING: Do not use the August 22 version, and if you insist, use dnspy to remove the native hooks in Customizelib on Plant.Update and Plant.FixedUpdate
2. Locate the release for PVZ Fusion Custom Plant Class Framework in the releases tab in this repo and download it into: gamedirectory/BepInEx/plugins
3. Run the game. You should see a black window open up and its name should be: BepInEx 6.0.0 dev - PlantsVsZombiesRH.exe). When the game is fully booted up and it is in the main menu, wait 5 seconds and close the game.
4. You should see a folder called interop in the BepInEx folder
5. Done!
## Creating a mod

### Prerequisites:
- Medium to advanced understanding in the C# coding language
- Medium understanding in making AssetBundles in Unity
- A 64 bit computer running Windows 10 or later
- Basic drawing and animating capibilities in Unity

### Guide:
1. Create your modding folder.
2. Install tools like dnspy(https://github.com/dnSpy/dnSpy/releases/tag/v6.1.8), Il2CppDumper(https://github.com/Perfare/Il2CppDumper/releases), Tuanjie editor 2022.3, Microsoft Visual Studio Code(https://apps.microsoft.com/detail/xp9khm4bk9fz7q), and idealy a dissasembler.
3. Put your tools into the buid folder in their own folders.
4. Create a folder and name it combinemod. This will be your build folder.
5. Download the template folder from here https://github.com/daisyqd-gif/PVZRH-modding-tools/releases and extract the sourcecode.zip into your combinemod folder
6. Create a folder called "lib" in the combinemod folder
7. Run Il2CppDumper on the game and copy the generated dummydll folder into the lib folder.
8. Locate the BepInEx folder and copy it into the lib folder.
9. Copy the template folder and name it your mod and follow the instructions there.
### An example of a full mod
```csharp
global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass;
global using CustomPlantClass.Main;
global using Random = UnityEngine.Random
using System.Threading.Tasks;
using CustomPlantClass.Runtime.Tasks;

namespace GatlingPea
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : ModPlugin
    {
        private AssetBundle assetBundle;
        private ID plantType = DataMgr.AllocateID();
        public override void InitializeMod()
        {
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "gatlingpea"
            );
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = plantType, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("GatlingPeaPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("GatlingPeaPreview"), // Card preview prefab

                Fusions = new List<(ID,ID)>(), // Optional fusion recipes

                AttackInterval = 0.75f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 80,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 400,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = false, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.Green, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = true,  // Appears in the Rainbow Card menu
                IsUltimatePlant = false, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "80!射手",           // Plant name (shown in UI)
                AlmanacEntry = "一次发射四颗豌豆。\n\n"+    // Almanac description (CN + EN recommended)
                "<color=#3D1400>伤害：</color><color=red>20×4/1.5秒</color>\n" +
                "<color=#3D1400>融合配方：</color><color=red>豌豆射手×4</color>\n"+
                "<color=#3D1400>80! 80! 80!</color>\n"
            };

            // Register the plant and retrieve its ID
            DataMgr.RegisterCustomPlant<GatlingPea80, Shooter>(Data);

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class GatlingPea80 : BaseCustomPlant
    {
        public override Transform FindShoot() => _plant.transform.FindChild("GatlingPea_head/Shoot");
        public override Bullet Shoot_Custom()
        {
            Vector2 pos=_plant.shoot.position;
            if (Random.Range(0,100)<25)
            {
                Bullet b1=CreateBullet.Instance.SetBullet(pos.x,pos.y,_plant.thePlantRow,BulletType.Bullet_pea,BulletMoveWay.MoveRight);
                b1.Damage=_plant.attackDamage*2;
                b1.fromType=_plant.thePlantType;
                Bullet b4 = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, _plant.thePlantRow,
                    BulletType.Bullet_pea,
                    BulletMoveWay.Free
                );
                b4.Damage = _plant.attackDamage*2;
                b4.fromType = _plant.thePlantType;
                b4.transform.Rotate(0, 0, 45);
                Bullet b5 = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, _plant.thePlantRow,
                    BulletType.Bullet_pea,
                    BulletMoveWay.Free
                );
                b5.Damage = _plant.attackDamage*2;
                b5.fromType = _plant.thePlantType;
                b5.transform.Rotate(0, 0, 30);
                Bullet b2 = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, _plant.thePlantRow,
                    BulletType.Bullet_pea,
                    BulletMoveWay.Free
                );
                b2.Damage = _plant.attackDamage*2;
                b2.fromType = _plant.thePlantType;
                b2.transform.Rotate(0, 0, -30);
                Bullet b3 = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, _plant.thePlantRow,
                    BulletType.Bullet_pea,
                    BulletMoveWay.Free
                );
                b3.Damage = _plant.attackDamage*2;
                b3.fromType = _plant.thePlantType;
                b3.transform.Rotate(0, 0, -45);
                _plant.attributeCount=-1;
                return b1;
            }
            Bullet b=CreateBullet.Instance.SetBullet(pos.x,pos.y,_plant.thePlantRow,BulletType.Bullet_pea,BulletMoveWay.MoveRight);
            b.Damage=_plant.attackDamage;
            b.fromType=_plant.thePlantType;
            return b;
        }
        protected override bool IsAsyncPF => true;
        protected override async Task SuperShoot_Async()
        {
            isPF = true;
            plant.invincible = true;
            plant.uncrashable = true;
            plant.anim.SetBool("shooting", true);
            plant.flashCountDown = 5f;
            plant.isFlashing = true;

            int total = 90;

            for (int i = 0; i < total; i++)
            {
                if (plant == null || plant.IsDestroyed()) return;
                Vector3 pos = plant.shoot.position;
                Bullet b = CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, plant.thePlantRow,
                    BulletType.Bullet_pea,
                    BulletMoveWay.MoveRight
                );

                b.Damage = plant.attackDamage;
                b.fromType = plant.thePlantType;
                
                if(plant.thePlantRow < 0)
                {
                    Bullet b1 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow,
                        BulletType.Bullet_pea,
                        BulletMoveWay.MoveRight
                    );

                    b1.Damage = plant.attackDamage;
                    b1.fromType = plant.thePlantType;
                }
                else
                {
                    Bullet b1 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow-1,
                        BulletType.Bullet_pea,
                        BulletMoveWay.MoveRight_threePeater
                    );

                    b1.Damage = plant.attackDamage;
                    b1.fromType = plant.thePlantType;
                }
                
                if(plant.thePlantRow > plant.board.rowNum - 1)
                {
                    Bullet b1 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow,
                        BulletType.Bullet_pea,
                        BulletMoveWay.MoveRight
                    );

                    b1.Damage = plant.attackDamage;
                    b1.fromType = plant.thePlantType;
                }
                else
                {
                    Bullet b1 = CreateBullet.Instance.SetBullet(
                        pos.x, pos.y, plant.thePlantRow+1,
                        BulletType.Bullet_pea,
                        BulletMoveWay.MoveRight_threePeater
                    );

                    b1.Damage = plant.attackDamage;
                    b1.fromType = plant.thePlantType;
                }

                await DelayTask.DelayScaled(0.1f,() => _plant.attributeSpeed,token);
            }
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "GatlingPea.Bepinex";
        public const string PluginName = "GatlingPea";
        public const string PluginVersion = "1.0.0";
    }
}
```
### Updating your mod
1. Install the newest game version and repeat step 1, 2, 3, and 4  in the installation guide.
2. Repeat steps 5, 7, 8, 9 in the creating a mod guide.
3. Reopen all of your mods and rebuild all of them and fix all errors that resulted from the update.

## Mod Catalog
1. Charm Sniper ![Deprecated: Replaced](https://img.shields.io/badge/Deprecated-Replaced-red?style=for-the-badge) : This will be replaced in a future remake of SniperDLC
2. CustomPlant.dll : The central framework responsible for all mods
3. CustomPlant.CustomPlant.RogueShootingManager : Rogue shooting support for some mods
4. CustomPlantClass.Main.BulletBehaviour : Custom bullet moveway support
5. CustomPlantClass.Networking : TCP support for mods that need to communicate with other mods
6. CustomPlantClass.Runtime.Tasks : Async support for mods, required by all mods
7. FireSniperPuff (Requires CustomPlant.CustomPlant.RogueShootingManager)
8. MachineNutBuff
9. MegaGatlingExpansionRShooting ![Deprecated: Replaced](https://img.shields.io/badge/Deprecated-Replaced-red?style=for-the-badge) : Replaced by the rogue shooting framework
10. MegaGatlingPeaDLC (Requires CustomPlant.CustomPlant.RogueShootingManager)
11. Modified-Plus-Lite ![Deprecated: Replaced](https://img.shields.io/badge/Deprecated-Replaced-red?style=for-the-badge)
12. MoreBlackHorse
13. MoreBossSlider
14. MoreDolphinZombie
15. MoreMinigun (Requires CustomPlant.CustomPlant.RogueShootingManager)
16. MowerFix
17. PortalSuperGatling
18. RemoveCraters
19. RemoveHypnoMiner
20. RogueShootingRandomFormation (Requires CustomPlant.CustomPlant.RogueShootingManager)
21. StarPeashooter
22. StarUpManager
23. SuperCherryThreeGatling
24. SuperHammer
25. UltimateArtillerySpike (Requires CustomPlant.CustomPlant.RogueShootingManager)
26. UltimateCherryFireShooter.Bepinex-Deconfused
27. UltimateDoomSniper_RogueShooting ![Deprecated: Replaced](https://img.shields.io/badge/Deprecated-Replaced-red?style=for-the-badge) : Replaced by the rogue shooting framework
28. UltimateDoomSniper-2 (Requires CustomPlant.CustomPlant.RogueShootingManager)
29. UltimateGatlingBloverBuff.Bepinex
30. UltimateIFV
31. UltimatePlanternSkin
32. UltimateRedLunar_RogueShooting
33. UltimateSniper_FlameGatlingRShooting ![Deprecated: Replaced](https://img.shields.io/badge/Deprecated-Replaced-red?style=for-the-badge) : Replaced by the rogue shooting framework
34. UltimateSniperAndUltimateMegaGatlingPea (Requires CustomPlant.CustomPlant.RogueShootingManager) : Contains ultimate flame gatling and ultimate flame sniper
35. UltimateSolarCoronaCabbage
36. Utilities : Debug tools
37. zombossleveladdon : Contains HeiTa and Gift box imitater

## API documentation
### namespace CustomPlantClass
1. CustomBigStar -> Big star monobehaviour (undocumented)
2.  BaseCustomBullet -> Used for creating custom bullets, overridable methods are structured like prefixes, returning true will run the original method, returning false will skip the original method.(Currently supports bullet_pea and bullet_cabbage as its TBase) Example impl:
```csharp
public class Bullet_ultimateMelonCabbage : BaseCustomBullet
{
    public override bool HitLand()
    {
        _bullet.board.boardAction.CreateCherryExplode(_bullet.col.bounds.center,_bullet.theBulletRow,Plugin.DataContainer.ParticleId,_bullet.Damage,_bullet.fromType);
        return true;
    }
    public override bool HitZombie(Zombie zombie)
    {
        _bullet.board.boardAction.CreateCherryExplode(_bullet.col.bounds.center,_bullet.theBulletRow,Plugin.DataContainer.ParticleId,_bullet.Damage,_bullet.fromType);
        return true;
    }
}
```
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
13. PlantSkinComponent, BulletComponent, ZombieComponent -> Contains a getter property to get their respective property on its gameobject
14. ModLogger -> Used to lod information about the mod and also to log errors/warnings
### CustomPlantClass.Level
1. BranchAdventureManager -> Used to register custom branch adventures (Incomplete! do not use until verified!)
2. CustomLevelMgr -> Used to load custom levels into the game. API (full code in repo):
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
3. LevelProgressionManager -> Used to track what custom levels are completed. API  (full code in repo):
```csharp
public static class LevelProgressionManager
{
    public static void MarkCompleted(int levelID) { }
    public static void MarkNotCompleted(int levelID) { }
    public static bool IsCompleted(int levelID) { }
}
```
### CustomPlantClass.Examples (Provides inheretable classes for base game plants)
1. ArmedChomperBase -> Untested
2. DoomSniper_Example
3. SniperPea_Example
4. SuperHypnoGatling_Example
### CustomPlantClass.Main
1. AssetMgr -> Used to load/save assets from different sources. API  (full code in repo):
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
2. DataMgr -> The core of the framework and contains most APIs. API  (full code in repo):
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
