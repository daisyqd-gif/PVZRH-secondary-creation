# 🌱PVZ Fusion Custom Plant Class Framework
[![License](https://img.shields.io/badge/Apache_2.0-License-red.svg)](https://github.com/daisyqd-gif/PVZRH-secondary-creation/blob/main/LICENSE)
[![Deps](https://img.shields.io/badge/Customizelib-BepInEx-orange.svg)](https://github.com/SalmonCN-RH/CustomizeLib)
[![Deps](https://img.shields.io/badge/BepInEx-v6.0.0_pre-yellow.svg)](https://github.com/BepInEx/BepInEx/releases)
[![Deps](https://img.shields.io/badge/Roslyn-v5.9.0-green.svg)](https://github.com/dotnet/roslyn)
[![Deps](https://img.shields.io/badge/VSCode-blue.svg)](https://code.visualstudio.com/download)
[![Deps](https://img.shields.io/badge/C%23-v14-darkblue.svg)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Deps](https://img.shields.io/badge/.Net-v6.0-purple.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

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
## Quick access
[![API Documentation](https://img.shields.io/badge/Click_me_to_show_documentation-blue?style=for-the-badge)](https://github.com/daisyqd-gif/PVZRH-secondary-creation/blob/main/API.md)

[![Static Badge](https://img.shields.io/badge/Click%20me%20to%20open%20the%20latest%20stable%20release-blue?style=for-the-badge&logo=github)](https://github.com/daisyqd-gif/PVZRH-secondary-creation/releases)

[![Static Badge](https://img.shields.io/badge/Click%20me%20to%20open%20the%20latest%20bleeding%20edge%20release-red?style=for-the-badge&logo=github)](https://github.com/daisyqd-gif/PVZRH-secondary-creation/tree/main/Build/net6.0)

[![Static Badge](https://img.shields.io/badge/Click%20me%20to%20open%20the%20error%20fixes%20repo-green?style=for-the-badge)](https://github.com/daisyqd-gif/PVZRH-Modding-Patches)

[![Static Badge](https://img.shields.io/badge/Click%20to%20download%20the%20latest%20customizelib-blue?style=for-the-badge&logo=pan.quark.cn%2Ffavicon.ico)](https://pan.quark.cn/s/6461fdaccff5#/list/share/36e088bfe7cc4167ac502c32cad813b7)

[![Static Badge](https://img.shields.io/badge/Click%20to%20download%20the%20latest%20bleeding%20edge%20customizelib-orange?style=for-the-badge&logo=github)]([https://pan.quark.cn/s/6461fdaccff5#/list/share/36e088bfe7cc4167ac502c32cad813b7](https://github.com/SalmonCN-RH/CustomizeLib/tree/master/res/BepInEx))

## Features
- [x] Base classes for plants, zombies, levels, etc...
- [x] A central framework for registration
- [x] Convinence helpers for easier coding
- [x] A custom mod loader ( Incomplete )
- [x] Registries and an event manager for cross mod communication
- [x] Custom structs and data types for easier data storage
- [x] Limited and untested support for the easier .NET based F# language
- [x] A central command dispatcher for mods to communicate with each other and external apps via TCP
- [x] An async state machine runner that replaces UniTask and Task.Delay
- [x] Python like support for collections
- [x] A bridge for converting data from Il2cpp collections to managed collections
- [x] Runtime compilation of mods
- [x] An interrupt system
- [ ] A way to run mods in Lua
- [ ] A visual plant creator that allows modders or designers to create plants with minimal coding
- [ ] Full support for the easier .NET based F# language
- [ ] Sub frame timing in the async state machine runner
- [ ] A repl for compiling code at runtime
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

### 📚Guide:
1. Create your modding folder.
2. Install tools like dnspy(https://github.com/dnSpy/dnSpy/releases/tag/v6.1.8), Il2CppDumper(https://github.com/Perfare/Il2CppDumper/releases), Tuanjie editor 2022.3, Microsoft Visual Studio Code(https://apps.microsoft.com/detail/xp9khm4bk9fz7q), and idealy a dissasembler.
3. Put your tools into the buid folder in their own folders.
4. Create a folder and name it combinemod. This will be your build folder.
5. Download the template folder from here https://github.com/daisyqd-gif/PVZRH-modding-tools/releases and extract the sourcecode.zip into your combinemod folder
6. Create a folder called "lib" in the combinemod folder
7. Run Il2CppDumper on the game and copy the generated dummydll folder into the lib folder.
8. Locate the BepInEx folder and copy it into the lib folder.
9. Copy the template folder and name it your mod and follow the instructions there.
### 👾An example of a full mod
<details>
<summary>Click to show code</summary>

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
</details>

### ⬆️Updating your mod
1. Install the newest game version and repeat step 1, 2, 3, and 4  in the installation guide.
2. Repeat steps 5, 7, 8, 9 in the creating a mod guide.
3. Reopen all of your mods and rebuild all of them and fix all errors that resulted from the update.
## ❗Known Bugs
<details>
<summary>Click to show section</summary>
    
- Patching hot virtuals crashes the game.  ![Bug](https://img.shields.io/badge/Severity:-High-red)

- Exceptions thrown in async state machines will go uncaught and crashes the game.  ![Bug](https://img.shields.io/badge/Severity:-High-red)

- System.Collections.Immutable can't be resolved. ![Bug](https://img.shields.io/badge/Severity:-High-red)

- Base custom plant throws a NullReferenceException in Start_Async. ![Bug](https://img.shields.io/badge/Severity:-Medium-orange)

- Index out of range exception thrown in the curtom levels menu. ![Bug](https://img.shields.io/badge/Severity:-Medium-orange)

- Rogue almanac breaks sometimes.  ![Bug](https://img.shields.io/badge/Severity:-Unfixible._Please_remove_all_rogue_shooting_mods_that_don't_inject_into_the_almanac.-yellow)

- Some sniper plants shoot peas instead of sniping zombies.  ![Bug](https://img.shields.io/badge/Severity:-Low-green)

- Some mods are missing from the release.  ![Bug](https://img.shields.io/badge/Severity:-Low-green)

- Mods that use unitask will fail.  [![Bug](https://img.shields.io/badge/Severity:-Fixed-blue)](https://github.com/daisyqd-gif/PVZRH-Modding-Patches)

- Custom rogue shooting plants will never appear because of the unlock system. ![Bug](https://img.shields.io/badge/Severity:-Fixed-blue)

</details>

## 📁Mod Catalog
<details>
<summary>Click to show section</summary>
  
1. Charm Sniper ![Deprecated: Replaced](https://img.shields.io/badge/Deprecated-This_will_be_replaced_by_the_future_Sniper_DLC-red)
2. CustomPlant.dll : The central framework responsible for all mods
3. CustomPlant.CustomPlant.RogueShootingManager : Rogue shooting support for some mods
4. CustomPlantClass.Main.BulletBehaviour : Custom bullet moveway support
5. CustomPlantClass.Networking : TCP support for mods that need to communicate with other mods
6. CustomPlantClass.Runtime.Tasks : Async support for mods, required by all mods
7. FireSniperPuff (Requires CustomPlant.CustomPlant.RogueShootingManager)
8. MachineNutBuff
10. MegaGatlingPeaDLC (Requires CustomPlant.CustomPlant.RogueShootingManager)
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
27. UltimateDoomSniper_RogueShooting ![Deprecated: Replaced](https://img.shields.io/badge/Deprecated-Replaced_by_the_rogue_shooting_framework-red)
28. UltimateDoomSniper-2 (Requires CustomPlant.CustomPlant.RogueShootingManager)
29. UltimateGatlingBloverBuff.Bepinex
30. UltimateIFV
31. UltimatePlanternSkin
34. UltimateSniperAndUltimateMegaGatlingPea (Requires CustomPlant.CustomPlant.RogueShootingManager) : Contains ultimate flame gatling and ultimate flame sniper
35. UltimateSolarCoronaCabbage
36. Utilities : Debug tools
37. zombossleveladdon : Contains HeiTa and Gift box imitater
</details>

## Disclaimer
[![License](https://img.shields.io/badge/Dependency-Licenses-red.svg)](https://github.com/daisyqd-gif/PVZRH-secondary-creation/blob/main/Licenses)

This project does not include, distribute, or rely on any copyrighted Plants vs. Zombies assets. All game content referenced by this repository belongs to its respective copyright holders.

Please mod responsibly. Do not upload or share any proprietary PVZ files, including textures, models, audio, or other game data.
