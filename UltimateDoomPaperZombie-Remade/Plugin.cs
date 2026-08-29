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

namespace Template
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : ModPlugin
    {
        private AssetBundle assetBundle;
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "abname"
            );
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = DataMgr.AllocateID(), // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("xxxPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("xxxPreview"), // Card preview prefab

                Fusions = new List<(ID, ID)>(), // Optional fusion recipes

                AttackInterval = 0f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 0,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 0f,               // Card cooldown
                Sun = 0,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = false,     // Enable PF ability if the plant has one
                CanStarUp = false, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.White, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = false, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "xxx",           // Plant name (shown in UI)
                AlmanacEntry = "xxx"    // Almanac description (CN + EN recommended)
            };

            // Register the plant and retrieve its ID
            ID plantID = DataMgr.RegisterCustomPlant<Plant, Template>(Data);

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
        public const string PluginVersion = "3.6.1";
    }
}
