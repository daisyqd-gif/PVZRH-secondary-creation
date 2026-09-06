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
