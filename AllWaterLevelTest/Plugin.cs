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
global using CustomPlantClass.Level;
global using CustomPlantClass.Main;

namespace AllWaterLevel_Fog
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : ModPlugin
    {
        public override void InitializeMod()
        {
            var level = new BaseCustomLevelData
            {
                LevelType = LevelType.CustomLevel,
                LevelID = CustomLevelMgr.AllocateLevelID(),
                LevelName = "纯水图(夜晚)",
                LevelNameEn = "AllWaterLevel_Fog",
                LevelSprite = null,
                SceneType = SceneType.NightPool,
                ScenePrefab = null,
                SceneBackground = BytesToSprite(Convert.FromBase64String(AssetMgr.GetBase64FromCache("AllWaterLevel","all_water_night_land.png","https://yun.urldwz.com/f/V68nTG/all_water_night_land.png"))),
                MusicType = MusicType.Fog,
                MusicAudio = null,
                MaxWave = 40,
                ZombieTypes = new List<ZombieType>()
                {
                    ZombieType.NormalZombie,
                    ZombieType.FlagZombie,
                    ZombieType.ConeZombie,
                    ZombieType.BucketZombie,
                    ZombieType.SnorkleZombie,
                    ZombieType.Dolphinrider,
                    ZombieType.KirovZombie,
                    ZombieType.BalloonZombie,
                    ZombieType.SnowDolphinrider,
                    ZombieType.IronBalloonZombie,
                    ZombieType.LevatationZombie,
                    ZombieType.SuperSubmarine,
                    ZombieType.IronBalloonZombie2
                },
                MapRoadTypes = new BoxType_Short[,]
                {
                    {BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W },
                    {BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W },
                    {BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W },
                    {BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W },
                    {BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W },
                    {BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W,BoxType_Short.W }
                },
                selection = CustomLevelSelection.Normal,
                SelectTypes = Array.Empty<PlantType>(),
                EnterAction = new Action(() =>
                {
                    Board.Instance.background.transform.Find("bg").Find("bg").Find("water").GetComponent<SpriteRenderer>().sprite = BytesToSprite(Convert.FromBase64String(AssetMgr.GetBase64FromCache("AllWaterLevel","all_water_night_water.png","https://yun.urldwz.com/f/V68nTG/all_water_night_water.png")));
                }),
                SunCounter = 1000,
                AdvBuffs = new List<AdvBuff>(),
                UltiBuffs = new List<UltiBuff>(),
                TravelUnlocks = new List<TravelUnlocks>(),
                TravelDebuffs = new List<TravelDebuff>(),
                BoardTag = new Board.BoardTag
                {
                    isUltimateSuperRandom=true,
                    isSuperRandom=true,
                    enableAllTravelPlant=true,
                    enableTravelPlant=true
                }
            };
            CustomLevelMgr.RegisterCustomLevel(level);
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
        public static Sprite BytesToSprite(byte[] bytes)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);

            ImageConversion.LoadImage(tex, bytes); // IL2CPP‑safe

            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }

    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "AllWaterLevel_Fog.Bepinex";
        public const string PluginName = "AllWaterLevel_Fog";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
