global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using Il2CppInterop.Runtime.Injection;
global using System;
global using System.Reflection;
global using UnityEngine;
global using TMPro;
global using System.Collections;
global using System.Collections.Generic;
global using CustomPlantClass;
global using CustomPlantClass.Main;
global using CustomPlantClass.Level;
global using Unity.VisualScripting;
//global using CustomPlantClass.RogueShootingMgr;
/*
Recommendations:
1.Use a copied original plant prefab with similar stats for avoiding manually setting up the prefab, *Make sure to delete the script component before editing!*
2.For ultimate plant mods, it is recommended to add 2 buffs associated with it and a variant that uses those 2 buffs
3.Turn on loop time on animations for looping animations
4.Don't patch frequently overrided virtual methods to avoid game crashes
5.Don't add too many buffs for plants
6.When making previews, make sure it has the same size as the main prefab
7.Stick to the design philosiphy:
    a.Stay unique, don't be too similar to other plants unlss it is a variant, SP upgrade, or evolution
    b.Stay balanced, make sure your plant has a weakness that disables it from becoming counterless
    c.Stay true, don't change other plants' effects unless the mod involves making a variant, SP upgrade, or evolution
8.Optimize your code to remove lag and improve player experience
9.Organize your code into blocks for ease of reading
10.Don't use confuserex or similar tools because they can always be removed and it also corrupts game data
11.Open source your code with an open source lisence in github/gitee to allow others to contribute
12.Always add preview guards/null checks to every custom component
13.Never use ==null on monobehaviours but rather comp==null and comp.IsDestroyed()
14.For overriding il2cpp classes, always remember to add the 2 required constructor overloads and never override il2cpp monobehaviours:
        public insertClassNameHere(IntPtr ptr) : base(ptr) { }
        public insertClassNameHere() : base(ClassInjector.DerivedConstructorPointer<insertClassNameHere>()) => ClassInjector.DerivedConstructorBody(this);
15.Never stack 2 sprite renderers on the same order in layer on top of each other
16.Always make plants automatic in rogue shooting
17.Decompiling other mods is acceptable but never copy logic exactly unless it has an open source lisence
18.Decompiling the game is acceptable but never leak its internals
19.Don't use pirated tools, it is very illegal and also very unsafe
20.Creating your own tools is highly recommended
21.Collaborating with others is recommended for complex mods
22.Use Assetripper to extract specific sprites
23.Plan your mod before building
24.Il2cppDumper enums may not match actual enums and never use reflection on enums with characters not in the alphabet and _
25.The compiler parser renames all unacceptable characters to _
26.Keep pdb files with your mod, the deps.json and runtimeconfig.json can be deleted
27.Build mods in a folder outside of your user account
28.Use Unity versions before the game version and always use the same year/subversion:
    example: if the game is 2022.3.62f1c1, using 2022.3.20f1c1 is acceptable

*/
namespace Template
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ID PlantId=-1;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "abname"
            );
            DataContainer.PlantId=DataMgr.AllocateID();
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = DataContainer.PlantId, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("xxxPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                Preview = assetBundle.GetAsset<GameObject>("xxxPreview"), // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */

                Fusions = new List<(ID, ID)>(), // Optional fusion recipes

                AttackInterval = 0f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 0,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 0f,               // Card cooldown
                Sun = 0,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = false,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
                CanStarUp = false, // Enable Star-Up ability if the plant has one, retrieve using _plant.starUp

                CardColor = CardLevel.White, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong / Final ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = false, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "xxx",           // Plant name (shown in UI)
                AlmanacEntry = "xxx"    // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };

            DataMgr.RegisterCustomPlant<Plant, Template>(Data);

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class Template : BaseCustomPlant
    {
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "Template.Bepinex";
        public const string PluginName = "Template";
        public const string PluginVersion = "3.7";
    }
}
