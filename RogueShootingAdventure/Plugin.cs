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
using GameLevel.RogueShooting;
namespace RogueShootingAdventure
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        public class DayData
        {
            public static int LevelID_1=CustomLevelMgr.AllocateLevelID("RogueShooting_Day");
            public static BaseCustomLevelData Level_1 = new()
            {
                LevelType = LevelType.CustomLevel,
                LevelID = LevelID_1,
                LevelName = "诸神：冒险 1-1",
                LevelNameEn = "RogueAdventure1-1",
                SceneType = SceneType.ShootingDay,
                MusicType = MusicType.Day,
                MaxWave = 50,
                ZombieTypes = [ZombieType.NormalZombie, ZombieType.ConeZombie, ZombieType.BucketZombie],
                selection = CustomLevelSelection.PreSelected,
                SelectTypes = [PlantType.Peashooter],
                SunCounter = 0,
                MapRoadTypes = new BoxType_Short[,]
                {
                    { BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G },
                    { BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G },
                    { BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G },
                    { BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G },
                    { BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G, BoxType_Short.G }
                },
                BoardTag = new()
                {
                    isShooting = true,
                    rogueShooting = true,
                    disableSelectCard = true
                },
                EnterAction = ( ) => 
                {
                    var board = Board.Instance;
                    board.GetOrAddComponent<ShootingManager>();
                    board.cardBank = false;
                    board.timeUntilNextWave = 3f;
                    var travel = TravelMgr.Instance;
                    travel.GetNormalBuff(AdvBuff.EnumValue1000);
                    travel.GetNormalBuff(AdvBuff.EnumValue1018);
                    travel.GetNormalBuff(AdvBuff.EnumValue18);
                },
                EnterGameAction = (board) =>
                {
                    ShootingManager.Instance.GetNewPlant(PlantType.Peashooter);
                    InGameUI.Instance.SeedBank.SetActive(false);
                    ShootingManager.Instance.refreshCount += 4;
                    ShootingManager.Instance.BaseHealthMulitpier = 2f;
                    ShootingManager.Instance.difficulty = 2;
                    ShootingManager.Instance.BaseSpeedMultiplier = 1.5f;
                },
                AdvBuffs = new(){ }
            };
            public static int LevelID_2=CustomLevelMgr.AllocateLevelID("RogueShooting_Day");
            public static int LevelID_3=CustomLevelMgr.AllocateLevelID("RogueShooting_Day");
            public static int LevelID_Mini=CustomLevelMgr.AllocateLevelID("RogueShooting_Day");
            public static int LevelID_4=CustomLevelMgr.AllocateLevelID("RogueShooting_Day");
            public static int LevelID_5=CustomLevelMgr.AllocateLevelID("RogueShooting_Day");
            public static int LevelID_6=CustomLevelMgr.AllocateLevelID("RogueShooting_Day");
            public static int LevelID_Super=CustomLevelMgr.AllocateLevelID("RogueShooting_Day");
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            CustomLevelMgr.RegisterCustomLevel(DayData.Level_1);
        }
        public override void InitializePlants()
        {


            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "RogueShootingAdventure.Bepinex";
        public const string PluginName = "RogueShootingAdventure";
        public const string PluginVersion = "3.9";
    }
}
