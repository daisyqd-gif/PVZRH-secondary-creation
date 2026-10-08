![🌱PVZ Fusion Custom Plant Class Framework](https://forthebadge.com/api/badges/generate?panels=3&primaryLabel=pvzrh&secondaryLabel=Custom+plant+class+framework&primaryBGColor=%2331C4F3&primaryTextColor=%23FFFFFF&secondaryBGColor=%23389AD5&secondaryTextColor=%23FFFFFF&primaryFontSize=12&primaryFontWeight=900&primaryLetterSpacing=2&primaryFontFamily=Roboto&primaryTextTransform=uppercase&secondaryFontSize=12&secondaryFontWeight=600&secondaryLetterSpacing=2&secondaryFontFamily=Montserrat&secondaryTextTransform=uppercase&tertiaryLabel=1.0.5&tertiaryBGColor=%232674A4&tertiaryTextColor=%23FFFFFF&tertiaryFontSize=12&tertiaryFontWeight=500&tertiaryLetterSpacing=2&tertiaryFontFamily=Roboto&tertiaryTextTransform=uppercase&scale=2&secondaryTextShadowColor=%23000000&secondaryTextShadowOffsetX=1.5&secondaryTextShadowOffsetY=1.5&secondaryTextShadowBlur=2)

---

![C#](https://forthebadge.com/api/badges/generate?panels=2&primaryLabel=Made+with&secondaryLabel=C%23%2014&primaryBGColor=%239179e4&primaryTextColor=%23FFFFFF&secondaryBGColor=%23389AD5&secondaryTextColor=%23FFFFFF&primaryFontSize=12&primaryFontWeight=600&primaryLetterSpacing=2&primaryFontFamily=Roboto&primaryTextTransform=uppercase&secondaryFontSize=12&secondaryFontWeight=900&secondaryLetterSpacing=2&secondaryFontFamily=Montserrat&secondaryTextTransform=uppercase)

A modular, extensible gameplay framework for Plants vs. Zombies Fusion that allows modders to create custom plants, zombies, effects, projectiles, and gameplay systems using clean C# APIs.
## Table of contents
- [PVZ Fusion Custom Plant Class Framework](#pvz-fusion-custom-plant-class-framework)
  - [Overview](#overview)
  - [Quick access](#quick-access)
  - [Features](#features)
  - [Installation](#installation)
  - [Creating a mod](#creating-a-mod)
    - [Prerequisites:](#prerequisites)
    - [Guide](#guide)
    - [An example of a full mod](#an-example-of-a-full-mod)
    - [️Updating your mod](#updating-your-mod)
  - [Known Bugs](#known-bugs)
  - [Mod Catalog](#mod-catalog)
  - [Disclaimer](#disclaimer)
## Overview
This framework provides a unified API for extending PVZ Fusion without modifying the game’s internal code. It is designed for modders who want to build:
- new plants
- new zombies
- new projectiles
- new effects
- new mechanics
- new levels
- etc...

[⬆️ Back to table of contents](#table-of-contents)

## Quick access
[![API Documentation](https://gist.githubusercontent.com/cxmeel/0dbc95191f239b631c3874f4ccf114e2/raw/documentation_learn.svg)](https://github.com/daisyqd-gif/PVZRH-secondary-creation/blob/main/API.md)

[![Static Badge](https://gist.githubusercontent.com/cxmeel/0dbc95191f239b631c3874f4ccf114e2/raw/download.svg)](https://github.com/daisyqd-gif/PVZRH-secondary-creation/releases)

[![Static Badge](https://forthebadge.com/api/badges/generate?panels=3&primaryLabel=CustomizeLib&secondaryLabel=4.0&primaryBGColor=%2331C4F3&primaryTextColor=%23FFFFFF&secondaryBGColor=%23389AD5&secondaryTextColor=%23FFFFFF&primaryFontSize=12&primaryFontWeight=600&primaryLetterSpacing=2&primaryFontFamily=Montserrat&primaryTextTransform=uppercase&secondaryFontSize=12&secondaryFontWeight=600&secondaryLetterSpacing=2&secondaryFontFamily=Montserrat&secondaryTextTransform=uppercase&tertiaryLabel=By+SalmonCN-RH&tertiaryBGColor=%232674A4&tertiaryTextColor=%23FFFFFF&tertiaryFontSize=12&tertiaryFontWeight=500&tertiaryLetterSpacing=2&tertiaryFontFamily=Roboto&tertiaryTextTransform=uppercase&primaryIcon=github&primaryIconColor=%23ffffff&primaryIconSize=16&primaryIconPosition=left)](https://pan.quark.cn/s/6461fdaccff5#/list/share/36e088bfe7cc4167ac502c32cad813b7)

[⬆️ Back to table of contents](#table-of-contents)

## Features
- [x] Base classes for plants, zombies, levels, etc...
- [x] A central framework for registration
- [x] Convinence helpers for easier coding
- [x] A custom mod loader ( Incomplete )
- [x] Registries and an event manager for cross mod communication
- [x] Custom structs and data types for easier data storage
- [x] A central command dispatcher for mods to communicate with each other and external apps via TCP
- [x] An async state machine runner that replaces UniTask and Task.Delay
- [x] Python like support for collections
- [x] A bridge for converting data from Il2cpp collections to managed collections
- [x] Runtime compilation of mods
- [x] An interrupt system
- [x] Full support for the easier .NET based F# language
- [ ] A way to run mods in Lua
- [ ] A visual plant creator that allows modders or designers to create plants with minimal coding
- [ ] Sub frame timing in the async state machine runner
- [ ] A repl for compiling code at runtime

[⬆️ Back to table of contents](#table-of-contents)

## Installation
1. Install this custom fork of BepInEx 6.0.0 and [CustomizeLib](https://pan.quark.cn/s/6461fdaccff5#/list/share) -> 融合版/融合Mod/鲑鱼MOD整理/BepinEX版本/BepInEx前置框架 and download all files in the folder and place them into the game's directory (where PlantsVsZombiesRH.exe is located) and also locate customizelib in any of salmon's mods and put it in. TEMPORARY WARNING: Do not use the August 22 version, and if you insist, use dnspy to remove the native hooks in Customizelib on Plant.Update and Plant.FixedUpdate
2. Locate the release for PVZ Fusion Custom Plant Class Framework in the releases tab in this repo and download it into: gamedirectory/BepInEx/plugins
3. Run the game. You should see a black window open up and its name should be: BepInEx 6.0.0 dev - PlantsVsZombiesRH.exe. When the game is fully booted up and it is in the main menu, wait 5 seconds and close the game.
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
2. Install tools like [DnSpy 6.1.8 .Net Framework](https://github.com/dnSpy/dnSpy/releases/tag/v6.1.8), [Il2CppDumper](https://github.com/Perfare/Il2CppDumper/releases), Tuanjie editor 2022.3, [Microsoft Visual Studio Code](https://apps.microsoft.com/detail/xp9khm4bk9fz7q), and idealy a dissasembler.
3. Put your tools into the buid folder in their own folders.
4. Create a folder and name it combinemod. This will be your build folder.
5. Download the template folder from [here](https://github.com/daisyqd-gif/PVZRH-modding-tools/releases) and extract the sourcecode.zip into your combinemod folder
6. Create a folder called "lib" in the combinemod folder
7. Run Il2CppDumper on the game and copy the generated dummydll folder into the lib folder.
8. Locate the BepInEx folder and copy it into the lib folder.
9. Copy the template folder, name it your mod and follow the instructions there.

[⬆️ Back to table of contents](#table-of-contents)


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

[⬆️ Back to table of contents](#table-of-contents)

## ❗Known Bugs

<details>
<summary>Click to show section</summary>

| Bug | Severity | Fix |
|:-|:-:|-:
| Patching hot virtuals crashes the game. | ![Bug](https://img.shields.io/badge/High-red?style=flat-square) | Don't |
| Exceptions thrown in async state machines will go uncaught and crashes the game. | ![Bug](https://img.shields.io/badge/High-red?style=flat-square) | Wrap all async methods with try/catch blocks |
| System.Collections.Immutable can't be resolved. | ![Bug](https://img.shields.io/badge/High-red?style=flat-square) |  |
| Index out of range exception thrown in the curtom levels menu. | ![Bug](https://img.shields.io/badge/Medium-orange?style=flat-square) | Ignore this bug, it doesn't break anyting. |
| Rogue almanac breaks sometimes. | ![Bug](https://img.shields.io/badge/Medium-orange?style=flat-square) | Delete all rogue shooting mods that don't inject into the almanac. Replace those mods with hengming's rogue shooting catalog. |
| Some sniper plants shoot peas instead of sniping zombies. | ![Bug](https://img.shields.io/badge/Low-green?style=flat-square) | Change the overriden shoot method from Animshoot_Custom to Shoot_Custom |
| Some mods are missing from the release. | ![Bug](https://img.shields.io/badge/Low-green?style=flat-square) | Those mods are not ready for release. |
| Mods that use unitask will fail. | ![Bug](https://img.shields.io/badge/Low-green?style=flat-square) | Use salmon's or gaoshu's modified il2cppinterop |
| Custom rogue shooting plants will never appear because of the unlock system. | ![Bug](https://img.shields.io/badge/Low-green?style=flat-square) | Delete all rogue shooting mods that don't inject into the almanac. A permanent fix is coming. |

[![Static Badge](https://img.shields.io/badge/Click%20me%20to%20open%20the%20error%20fixes%20repo-green?style=for-the-badge)](https://github.com/daisyqd-gif/PVZRH-Modding-Patches)

</details>

[⬆️ Back to table of contents](#table-of-contents)

## 📁Mod Catalog
<details>
<summary>Click to show section</summary>

| Mod | Description | Status | Dependencies | Preview |
|:-|:-:|:-:|:-:|-:|
| CharmSniper | Deprecated | ![Status](https://img.shields.io/badge/Deprecated-red?style=flat-square) | CustomPlant | ![Preview](https://github.com/daisyqd-gif/pvzrh-mod-resources/blob/main/Screenshot%202026-09-13%20152726.png?raw=true) |
| CustomPlant | Central framework responsible for all mods | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlantClass.Runtime.Tasks |  |
| CustomPlant.RogueShootingManager | Rogue shooting support for some mods | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| CustomPlantClass.Main.BulletBehaviour | Custom bullet moveway support | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| CustomPlantClass.Networking | TCP support for mods that need to communicate | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | None |  |
| CustomPlantClass.Runtime.Tasks | Async support for mods, required by all mods | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | None |  |
| FireSniperPuff | Check almanac | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant.RogueShootingManager, CustomPlant | ![Preview](https://github.com/daisyqd-gif/pvzrh-mod-resources/blob/main/Screenshot%202026-09-13%20152732.png?raw=true) |
| MachineNutBuff | Machine nut effect buff | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) |  |  |
| MegaGatlingPeaDLC | 请输入文字 | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant.RogueShootingManager, CustomPlant | ![Preview](https://github.com/daisyqd-gif/pvzrh-mod-resources/blob/main/Screenshot%202026-09-13%20152546.png?raw=true) |
| MoreBlackHorse | Black football horse evolution states | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| MoreBossSlider | Extra boss slider content | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| MoreDolphinZombie | Gatling dolphin evolution states | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| MoreMinigun | More minigun plants | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant.RogueShootingManager, CustomPlant | ![Preview](https://github.com/daisyqd-gif/pvzrh-mod-resources/blob/main/Screenshot%202026-09-13%20152719.png?raw=true) |
| MowerFix | Mower behavior fix | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| PortalSuperGatling | Check almanac | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant | ![Preview](https://github.com/daisyqd-gif/pvzrh-mod-resources/blob/main/Screenshot%202026-09-13%20152655.png?raw=true) |
| RemoveCraters | Removes crater effects | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| RemoveHypnoMiner | Removes hypno miner and other things | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| RogueShootingRandomFormation | Randomized rogue shooting formation support | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant.RogueShootingManager, CustomPlant |  |
| StarPeashooter | Star peashooter | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant | ![Preview](https://github.com/daisyqd-gif/pvzrh-mod-resources/blob/main/Screenshot%202026-09-13%20152709.png?raw=true) |
| StarUpManager | Increase star up chance and also add hotkey | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) |  |  |
| SuperCherryThreeGatling | Check almanac | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant | ![Preview](https://github.com/daisyqd-gif/pvzrh-mod-resources/blob/main/Screenshot%202026-09-13%20152702.png?raw=true) |
| SuperHammer | Buffs hammer when buff is selected | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| UltimateArtillerySpike | Check almanac | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant.RogueShootingManager, CustomPlant | ![Preview](https://github.com/daisyqd-gif/pvzrh-mod-resources/blob/main/Screenshot%202026-09-13%20152637.png?raw=true) |
| UltimateCherryFireShooter.Bepinex-Deconfused | Check almanac | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| UltimateDoomSniper-2 | Doom sniper's SP form and variant | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant.RogueShootingManager, CustomPlant | ![Preview](https://github.com/daisyqd-gif/pvzrh-mod-resources/blob/main/Screenshot%202026-09-13%20152627.png?raw=true) |
| UltimateGatlingBloverBuff.Bepinex | Gatling blover enhancement | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| UltimateIFV | SP form for ifv iron puff | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant | ![Preview](https://github.com/daisyqd-gif/pvzrh-mod-resources/blob/main/Screenshot%202026-09-13%20152740.png?raw=true) |
| UltimatePlanternSkin | Fix for ultimate plantern's skin and also enhances the effect | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| UltimateSniperAndUltimateMegaGatlingPea | Check almanac | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant.RogueShootingManager, CustomPlant | ![Preview](https://raw.githubusercontent.com/daisyqd-gif/pvzrh-mod-resources/refs/heads/main/Screenshot%202026-09-13%20152618.png) |
| UltimateSolarCoronaCabbage | Check almanac | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| Utilities | Debug tools | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |
| zombossleveladdon | Contains HeiTa and Gift box imitater | ![Status](https://img.shields.io/badge/Active-green?style=flat-square) | CustomPlant |  |

</details>

[⬆️ Back to table of contents](#table-of-contents)

## Disclaimer
[![License](https://img.shields.io/badge/Dependency-Licenses-red.svg)](https://github.com/daisyqd-gif/PVZRH-secondary-creation/blob/main/Licenses)

This project does not include, distribute, or rely on any copyrighted Plants vs. Zombies assets. All game content referenced by this repository belongs to its respective copyright holders.

Please mod responsibly. Do not upload or share any proprietary PVZ files, including textures, models, audio, or other game data.

CustomPlantClass is not sponsored by, affiliated with or endorsed by Unity Technologies or its affiliates.
"Unity" is a trademark or a registered trademark of Unity Technologies or its affiliates in the U.S. and elsewhere.

CustomPlantClass is not sponsored by, affiliated with or endorsed by Electronic Arts or its affiliates.
"Plants vs Zombies" is a trademark or a registered trademark of Electronic Arts or its affiliates in the U.S. and elsewhere.

[⬆️ Back to table of contents](#table-of-contents)
