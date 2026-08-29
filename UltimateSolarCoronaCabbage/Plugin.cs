global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System;
global using System.Reflection;
global using UnityEngine;
global using System.Collections.Generic;
global using CustomPlantClass;
global using Unity.VisualScripting;
global using Random = UnityEngine.Random;
global using Object = UnityEngine.Object;
global using CustomPlantClass.Main;

namespace UltimateSolarCoronaCabbage_Remade
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, CustomPlantClass.MyPluginInfo.TargetVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ID thePlantType;
            public static ID theBulletType;
            public static GameObject superSolar;
            public static GameObject superSolarEmit;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = AssetMgr.LoadBundleFromResource(
                Assembly.GetExecutingAssembly(),
                "ultimatesolarcoronacabbage",
                false
            );
        }
        public override void InitializePlants()
        {
            DataContainer.theBulletType=DataMgr.AllocateID();
            CustomCore.RegisterCustomBullet<Bullet_sunCabbage>(DataContainer.theBulletType,assetBundle.GetAsset<GameObject>("Bullet_DoubleSuncabbage"));
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = DataMgr.AllocateID(), // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("UltimateSolarCoronaCabbagePrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("UltimateSolarCoronaCabbagePreview"), // Card preview prefab

                Fusions = DataMgr.MirrorList(new(){(PlantType.UltimateCabbage,PlantType.UltimateCabbage)}), // Optional fusion recipes

                AttackInterval = 1f,   // Time between attacks (shooters only)
                ProduceInterval = 2f,  // Time between sun/production cycles
                AttackDamage = 1000,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 1000,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = true, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.Gold, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = true, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "终极圣辉日冕太阳神 • 重制版",           // Plant name (shown in UI)
                AlmanacEntry = "倍增太阳神——究极圣辉日冕太阳神\n"+
                "<color=#3D1400>融合配方：</color><color=red>究极太阳神卷心菜+究极太阳神卷心菜</color>\n"+
                "<color=#3D1400>画师：</color><color=red>@HaDemo-Doom-</color>\n"+ //The attribution part of the MIT lisence
                "<color=#3D1400>原作者：</color><color=red>高数带我飞</color>\n"+ //The attribution part of the MIT lisence
                "<color=#3D1400>伤害：</color><color=red>1000×5×3/3秒</color>\n"+
                "<color=#3D1400>特点：</color><color=red>特点1:拥有太阳神卷心菜的所有特点,出场时阳光上限翻倍，在场时所有阳光提供的阳光量*2\n"+
                "特点2:子弹将会散射成三个砸在最前方的三个僵尸上，每30秒一次，召唤太阳，持续30秒，每轮存在时间内，召唤日冕太阳会增加30秒的持续时间，太阳存在时，每0.5秒造成（180+40×太阳神数量+80×倍阳神数量）的伤害，所有僵尸死亡会产生一次光爆,造成阳光炸弹同等效果,伤害为当前阳光数量\n"+
                "特点3:阳光高于15000时，每次攻击会消耗200阳光使本次伤害*2，若消耗后依然高于15000则再消耗一次，最多计算三次\n"+
                "大招：消耗1000金钱，立刻召唤太阳并且向全场所有僵尸散射子弹</color>\n"+
                "<color=#3D1400>词条1</color><color=red>（金光闪闪）：倍阳神的子弹会消耗超过15000阳光部分0.5 % 阳光，使子弹增加（50×消耗阳光）的伤害</color>\n"+
                "<color=#3D1400>词条2</color><color=red>（人造太阳）：太阳伤害×3</color>\n"+
                "<color=#3D1400>金光是她的象征，烈日是她的使徒，这就是究极圣辉日冕太阳神。不只是植物，就连僵尸界也有她的狂信徒。不管是在遥远的埃及，还是在失落的古城，信徒们都相信，只要她在，哪怕是暗淡无光的深渊也会成为一片富饶之地。虽然明面上是这么一位高高在上的神明，但究极圣辉日冕太阳神其实十分友善。这么一位完美的神明，相信她吧，相信她会为我们带来更好的未来，相信她会把圣光洒满大地，给予她忠实的信徒们来自圣灵的祝福</color>"    // Almanac description (CN + EN recommended)
            };

            // Register the plant and retrieve its ID
            DataContainer.thePlantType = DataMgr.RegisterCustomPlant<Plant, UltimateSolarCoronaCabbage_Remade>(Data);

            DataContainer.superSolar=assetBundle.GetAsset<GameObject>("Solar");
            DataContainer.superSolar.AddComponent<SuperSolar>();

            DataContainer.superSolarEmit=assetBundle.GetAsset<GameObject>("PrismLine");
            DataContainer.superSolarEmit.AddComponent<SuperSolarEmit>();

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateSolarCoronaCabbage_Remade.Bepinex";
        public const string PluginName = "UltimateSolarCoronaCabbage_Remade";
        public const string PluginVersion = "3.7";
    }
}
