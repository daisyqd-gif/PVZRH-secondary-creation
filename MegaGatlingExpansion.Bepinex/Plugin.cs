#nullable enable

global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
global using CustomizeLib.BepInEx.ExtensionData.Basic;
global using HarmonyLib;
global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Reflection;
global using UnityEngine;
global using Unity.VisualScripting;
global using TMPro;
global using Random = UnityEngine.Random;
global using UnityEngine.Rendering;
global using CustomPlantClass;
global using System.Linq;
global using Core;
global using CustomPlantClass.Main;
global using CustomPlantClass.Level;
using Il2CppSystem.IO;

namespace MegaGatlingExpansion
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BasePlugin
    {
        public const string PluginGuid = "MegaGatlingExpansion.Bepinex";
        public const string PluginName = "MegaGatlingExpansion";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
        public const bool IsOverpowered = false;
        public static int Buff1 = -1;
        public static int Buff2 = -1;
        public static int Buff3 = -1;
        public static int LevelID=CustomLevelMgr.AllocateLevelID();
        public static List<ID> CustomSuperPlants=new();
        public static BaseCustomLevelData level;
        public static Material? FontOutlineMaterial;
        public class BoardEnablerEffect : CustomLevelComponent
        {
            
        }
        public override void Load()
        {
            try
            {
                
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            }
            catch(Exception e)
            {
                Log.LogError(e.ToString());
            }
            DataMgr.AutoRegisterTypes(Assembly.GetExecutingAssembly());

            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "megagatlingpea");
            PlantTypeExpand lanternID = DataMgr.AllocateID();
            PlantTypeExpand smallID = DataMgr.AllocateID();
            try{
                level=new()
                {
                    LevelID=LevelID,
                    BoardTag=new Board.BoardTag()
                    {
                        enableAllTravelPlant=true,
                        enableTravelPlant=true,
                        isConvey=true,
                        disableSelectCard=true
                    },
                    SceneType=SceneType.Day_6,
                    MaxWave=60,
                    SunCounter=500,
                    UltiBuffs=new(){UltiBuff.EnumValue50,UltiBuff.EnumValue51},
                    selection=CustomLevelSelection.Convey,
                    SelectTypes=new PlantType[]
                    {
                        PlantType.Peashooter,PlantTypeExpand.MegaGatlingPea,
                        PlantType.IceShroom,PlantType.Jalapeno,
                        PlantType.ThreePeater,PlantType.SmallPuff,
                        PlantType.MixBomb,PlantType.ElectricOnion,
                        PlantType.PortalPea,PlantType.IronPea,
                        PlantType.SunFlower,PlantType.DoomShroom,
                        PlantType.GatlingPea,PlantType.DoubleShooter,
                        PlantType.SplitPea,PlantType.CherryBomb,
                        PlantType.TorchWood,PlantType.HypnoShroom,
                        PlantType.StarFruit
                    },
                    MapRoadTypes = new BoxType_Short[,]
                    {
                        {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G },
                        {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G },
                        {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G },
                        {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G },
                        {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G },
                        {BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G,BoxType_Short.G }
                    },
                    LevelName="超级机枪射手挑战",
                    LevelNameEn="Super gatling challenge",
                    MusicType=MusicType.UltimateBattle,
                    LevelSprite=assetBundle.GetAsset<GameObject>("SuperGatlingPreview").GetComponent<SpriteRenderer>().sprite,
                    TravelUnlocks=new(){TravelUnlocks.UltimateGatling},
                    ZombieTypes=new(){ZombieType.PeaShooterZombie,ZombieType.NormalZombie,
                    ZombieType.GatlingPeaZombie,ZombieType.RandomGargantuar,
                    ZombieType.SnowGatlingPeaZombie,ZombieType.GatlingFootballZombie,
                    ZombieType.BlackFootball_a,ZombieType.BlackFootball_c2,
                    ZombieType.GatlingBlackFootball,ZombieType.CherryPaperZ95,
                    ZombieType.GatlingPaper_b,ZombieType.GatlingPaper_c,
                    ZombieType.CherryShooterZombie,ZombieType.CherryPaperZombie,
                    ZombieType.IronPeaZombie,ZombieType.IronPeaDoorZombie,
                    ZombieType.SuperCherryShooterZombie,(ZombieType)9000,
                    (ZombieType)9001,(ZombieType)9005,ZombieType.ProtalZombie,
                    ZombieType.DoomPaper,ZombieType.WhiteFootball}
                };
                CustomLevelMgr.RegisterCustomLevel<BoardEnablerEffect>(level);
            }catch(Exception e){
                Debug.LogError(e.ToString());
            }

            //Plant registration
            {
                // Base Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, MegaGatlingPea>(
                    PlantTypeExpand.MegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantType.GatlingPea, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.GatlingPea),
                        ((int)PlantType.SplitPea,   (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.SplitPea)
                    },
                    1.4f,
                    0f,
                    20,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, MegaGatlingPea>(
                    PlantTypeExpand.MegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantType.GatlingPea, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.GatlingPea),
                        ((int)PlantType.SplitPea,   (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.SplitPea)
                    },
                    1.4f,
                    0f,
                    20,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.MegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.MegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(
                    PlantTypeExpand.MegaGatlingPea,
                    "初版超级机枪射手",
                    "加特林豌豆的最初强化原型，能够根据融合材料进化为多种属性形态，是整个超级机枪体系的核心基础体。\n" +
                    "<color=#3D1400>融合配方：</color><color=red>加特林豌豆 + 豌豆射手 / 双发射手 / 三线豌豆 等</color>\n" +
                    "<color=#3D1400>伤害：</color><color=red>高速连射，子弹类型随形态变化，可触发爆炸、电弧、毒爆、击退、散射等效果</color>\n" +
                    "<color=#3D1400>特性：</color><color=red>多形态进化 / 属性子弹 / PF超高速扫射 / 充能强化(0-3级) / PF期间无敌</color>\n\n" +

                    "<color=#3D1400>核心机制：</color>\n" +
                    "<color=red>• 充能系统：每次PF结束后获得1层充能(最多3层)，提高PF扫射总弹数，并改变枪口外观。\n" +
                    "• PF技能：进入超级扫射状态，短时间内发射大量子弹，带有随机散射角度，期间免疫伤害与击飞。</color>\n\n" +

                    "<color=#3D1400>形态总览 - PVZ2C：</color>\n" +
                    "<color=red>• 火焰超级机枪豌豆：发射极火豌豆，命中后引爆小范围火焰爆炸；PF时火焰覆盖面更大。\n" +
                    "• 寒冰超级机枪豌豆：发射极寒豌豆，持续减速并熄灭火焰；PF时散射冰弹形成大范围冰冻带。\n" +
                    "• 毒液超级机枪豌豆：发射剧毒豌豆，叠加最多5层中毒；达到上限时触发毒爆并扩散毒素。\n" +
                    "• 电能超级机枪豌豆：电击豌豆命中后在3格范围内弹射电弧，并在1.5格范围内持续造成电伤害。\n" +
                    "• 三线超级机枪豌豆：可同时攻击三条线路，PF时三路全屏扫射，边路不足时自动补发额外子弹。\n" +
                    "• 原始超级机枪豌豆：发射高伤害原始豌豆，整列击退，靠近场边的僵尸会被直接击飞出场。</color>\n\n" +

                    "<color=#3D1400>形态总览 - PVZRH 扩展：</color>\n" +
                    "<color=red>• 樱桃超级机枪豌豆：发射超级樱桃弹，散射角度大；PF时进入樱桃风暴模式，疯狂倾泻樱桃弹幕。\n" +
                    "• 野性超级机枪豌豆：普通攻击一次发射多枚豌豆，PF时每帧喷出多发子弹，形成高密度弹幕。\n" +
                    "• 毁灭超级机枪豌豆：普通连射时有几率发射大毁灭菇弹，造成大范围爆炸伤害。\n" +
                    "• 终极毁灭超级机枪豌豆：可发射终极毁灭弹，威力极高，PF时几乎将整条线路变成禁区。\n" +
                    "• 电能、寒冰、火焰、毒液、原始等形态在RH中保留其PVZ2C效果，并与PF系统完美兼容。</color>\n\n" +

                    "<color=#3D1400>子弹特性细节：</color>\n" +
                    "<color=red>• 极火豌豆：命中后点燃僵尸并引发小范围爆炸。\n" +
                    "• 极寒豌豆：持续减速并熄灭火焰，5级可发射冰刺造成范围冰冻。\n" +
                    "• 剧毒豌豆：叠加中毒层数，达到阈值后触发毒爆并扩散毒素。\n" +
                    "• 电击豌豆：命中时弹射电弧，并在飞行过程中对附近僵尸造成持续电伤害。\n" +
                    "• 原始豌豆：整列击退，靠近场边的僵尸会被直接打出场外。\n" +
                    "• 樱桃弹：散射角度大，适合清理密集僵尸。\n" +
                    "• 毁灭菇弹：造成大范围爆炸伤害，终极毁灭弹威力更高。</color>\n\n" +

                    "<color=#3D1400>初版超级机枪射手说：</color><color=red>“别把我当一株植物，我是一整套武器系统。”</color>\n\n" +

                    "<color=#3D1400>English Description:</color>\n" +
                    "<color=red>The prototype of the Mega Gatling system. By fusing with different plants, it evolves into multiple elemental and special forms, each changing its bullet type, behavior, and battlefield role.</color>\n\n" +

                    "<color=#3D1400>Core Mechanics:</color>\n" +
                    "<color=red>• Charge system: Gains 1 charge after each PF (max 3), increasing PF bullet count and changing barrel visuals.\n" +
                    "• PF mode: Fires a massive rapid-fire storm with random spread while being completely invincible.</color>\n\n" +

                    "<color=#3D1400>Variant Overview - PVZ2C:</color>\n" +
                    "<color=red>• Fire Mega Gatling Pea: Enhanced fire peas that ignite zombies and cause small explosions.\n" +
                    "• Snow Mega Gatling Pea: Ice peas that slow zombies and extinguish fire, PF spreads wide ice coverage.\n" +
                    "• Goo Mega Gatling Pea: Poison stacks up to 5 layers, triggering poison explosions.\n" +
                    "• Electric Mega Gatling Pea: Electric peas arc to nearby zombies and deal periodic AoE damage.\n" +
                    "• Triple Mega Gatling Pea: Attacks three lanes at once, PF becomes a full three-lane barrage.\n" +
                    "• Primal Mega Gatling Pea: Heavy primal peas knock back entire columns and can eject zombies off the lawn.</color>\n\n" +

                    "<color=#3D1400>Variant Overview - PVZRH:</color>\n" +
                    "<color=red>• Cherry Mega Gatling Pea: Wide-spread cherry shots, PF becomes a cherry bullet storm.\n" +
                    "• Wild Mega Gatling Pea: Dense multi-shot patterns ideal for overwhelming lanes.\n" +
                    "• Doom Mega Gatling Pea: Chance to fire big doom shroom shots.\n" +
                    "• Ultimate Doom Mega Gatling Pea: Fires ultimate doom shots with massive power.\n" +
                    "• All elemental forms retain their PVZ2C effects while benefiting from the shared PF and charge system.</color>\n\n" +

                    "<color=#3D1400>Traits:</color><color=red> Multi-form evolution / Attribute bullets / PF rapid-fire storm / Charge levels / Temporary invincibility</color>\n" +
                    "<color=#3D1400>Mega Gatling Pea says:</color><color=red>“Prototype? No. I am the blueprint for every upgrade.”</color>"
                );

                // Fire Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, FireMegaGatlingPea>(
                    PlantTypeExpand.FireMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperFireGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperFireGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.Jalapeno),
                        ((int)PlantType.Jalapeno, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.JalaGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.JalaGatling),

                        ((int)PlantType.JalaSplit,   (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.JalaSplit),

                        ((int)PlantType.GatlingPea, (int)PlantType.JalaPeashooter),
                        ((int)PlantType.JalaPeashooter, (int)PlantType.GatlingPea),

                        ((int)PlantType.SplitPea,   (int)PlantType.JalaDoubleshooter),
                        ((int)PlantType.JalaDoubleshooter, (int)PlantType.SplitPea)
                    },
                    1.4f,
                    0f,
                    1000,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, FireMegaGatlingPea>(
                    PlantTypeExpand.FireMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperFireGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperFireGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.Jalapeno),
                        ((int)PlantType.Jalapeno, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.JalaGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.JalaGatling),

                        ((int)PlantType.JalaSplit,   (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.JalaSplit),

                        ((int)PlantType.GatlingPea, (int)PlantType.JalaPeashooter),
                        ((int)PlantType.JalaPeashooter, (int)PlantType.GatlingPea),

                        ((int)PlantType.SplitPea,   (int)PlantType.JalaDoubleshooter),
                        ((int)PlantType.JalaDoubleshooter, (int)PlantType.SplitPea)
                    },
                    1.4f,
                    0f,
                    1000,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.FireMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.FireMegaGatlingPea, CardLevel.Blue);
                CustomCore.TypeMgrExtra.IsFirePlant.Add(PlantTypeExpand.FireMegaGatlingPea);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.FireMegaGatlingPea, "初版火辣超级机枪射手", "");

                // Small Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, SmallMegaGatlingPea>(
                    smallID,
                    assetBundle.GetAsset<GameObject>("SuperSmallGatling"),
                    assetBundle.GetAsset<GameObject>("SuperSmallGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantType.SmallPuff, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.PeaPuff, (int)PlantType.GatlingPea),

                        ((int)PlantType.DoublePuff, (int)PlantType.SplitPea),

                        ((int)PlantType.SplitPuff, (int)PlantType.DoubleShooter),

                        ((int)PlantType.GatlingPuff, (int)PlantType.Peashooter),
                    },
                    1.4f,
                    0f,
                    20,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, SmallMegaGatlingPea>(
                    smallID,
                    assetBundle.GetAsset<GameObject>("SkinSuperSmallGatling"),
                    assetBundle.GetAsset<GameObject>("SuperSmallGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantType.SmallPuff, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.PeaPuff, (int)PlantType.GatlingPea),

                        ((int)PlantType.DoublePuff, (int)PlantType.SplitPea),

                        ((int)PlantType.SplitPuff, (int)PlantType.DoubleShooter),

                        ((int)PlantType.GatlingPuff, (int)PlantType.Peashooter),
                    },
                    1.4f,
                    0f,
                    20,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(smallID);
                CustomCore.TypeMgrExtra.LevelPlants.Add(smallID, CardLevel.Blue);
                CustomCore.TypeMgrExtra.IsPuff.Add(smallID);
                CustomCore.RegisterCustomMixBombFusion(smallID, smallID, smallID, PlantTypeExpand.MegaGatlingPea);
                CustomCore.AddPlantAlmanacStrings(smallID, "初版小喷超级机枪射手", "");

                // Ice Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, IceMegaGatlingPea>(
                    PlantTypeExpand.IceMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperSnowGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperSnowGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.IceShroom),
                        ((int)PlantType.IceShroom, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.SnowGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.SnowGatling),

                        ((int)PlantType.SnowSplit,   (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.SnowSplit),

                        ((int)PlantType.GatlingPea, (int)PlantType.SnowPeaShooter),
                        ((int)PlantType.SnowPeaShooter, (int)PlantType.GatlingPea),

                        ((int)PlantType.SplitPea,   (int)PlantType.DoubleSnow),
                        ((int)PlantType.DoubleSnow, (int)PlantType.SplitPea)
                    },
                    1.4f,
                    0f,
                    250,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, IceMegaGatlingPea>(
                    PlantTypeExpand.IceMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperSnowGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperSnowGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.IceShroom),
                        ((int)PlantType.IceShroom, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.SnowGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.SnowGatling),

                        ((int)PlantType.SnowSplit,   (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.SnowSplit),

                        ((int)PlantType.GatlingPea, (int)PlantType.SnowPeaShooter),
                        ((int)PlantType.SnowPeaShooter, (int)PlantType.GatlingPea),

                        ((int)PlantType.SplitPea,   (int)PlantType.DoubleSnow),
                        ((int)PlantType.DoubleSnow, (int)PlantType.SplitPea)
                    },
                    1.4f,
                    0f,
                    250,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.IceMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.IceMegaGatlingPea, CardLevel.Blue);
                CustomCore.TypeMgrExtra.IsIcePlant.Add(PlantTypeExpand.IceMegaGatlingPea);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.IceMegaGatlingPea, "初版寒冰超级机枪射手", "");

                // Sun Mega Gatling
                CustomCore.RegisterCustomPlant<PeaSunFlower, SunMegaGatlingPea>(
                    PlantTypeExpand.SunMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperSunGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperSunGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.SunFlower),
                        ((int)PlantType.SunFlower, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.PeaSunFlower, (int)PlantType.GatlingPea),
                        ((int)PlantType.GatlingPea, (int)PlantType.PeaSunFlower) //only pea sunflower for some reason
                    },
                    1.4f,
                    10f,
                    20,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<PeaSunFlower, SunMegaGatlingPea>(
                    PlantTypeExpand.SunMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperSunGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperSunGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.SunFlower),
                        ((int)PlantType.SunFlower, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.PeaSunFlower, (int)PlantType.GatlingPea),
                        ((int)PlantType.GatlingPea, (int)PlantType.PeaSunFlower) //only pea sunflower for some reason
                    },
                    1.4f,
                    10f,
                    20,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.SunMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.SunMegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.SunMegaGatlingPea, "初版阳光超级机枪射手", "");

                // Goo Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, GooMegaGatlingPea>(
                    PlantTypeExpand.GooMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperPoisonGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperPoisonGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.Garlic),
                        ((int)PlantType.Garlic, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.GarlicGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.GarlicGatling),

                        ((int)PlantType.GarlicSplit,   (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.GarlicSplit),

                        ((int)PlantType.GatlingPea, (int)PlantType.GarlicPea),
                        ((int)PlantType.GarlicPea, (int)PlantType.GatlingPea),

                        ((int)PlantType.SplitPea,   (int)PlantType.GarlicRepeater),
                        ((int)PlantType.GarlicRepeater, (int)PlantType.SplitPea)
                    },
                    1.4f,
                    0f,
                    500,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, GooMegaGatlingPea>(
                    PlantTypeExpand.GooMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperPoisonGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperPoisonGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.Garlic),
                        ((int)PlantType.Garlic, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.GarlicGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.GarlicGatling),

                        ((int)PlantType.GarlicSplit,   (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.GarlicSplit),

                        ((int)PlantType.GatlingPea, (int)PlantType.GarlicPea),
                        ((int)PlantType.GarlicPea, (int)PlantType.GatlingPea),

                        ((int)PlantType.SplitPea,   (int)PlantType.GarlicRepeater),
                        ((int)PlantType.GarlicRepeater, (int)PlantType.SplitPea)
                    },
                    1.4f,
                    0f,
                    500,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.GooMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.GooMegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.GooMegaGatlingPea, "初版毒液超级机枪射手", "");

                // Lantern Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, LightMegaGatlingPea>(
                    lanternID,
                    assetBundle.GetAsset<GameObject>("SuperLanternGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperLanternGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.Plantern),
                        ((int)PlantType.Plantern, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.LanternGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.LanternGatling),

                        ((int)PlantType.LanternSplit,   (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.LanternSplit),

                        ((int)PlantType.GatlingPea, (int)PlantType.LanternPea),
                        ((int)PlantType.LanternPea, (int)PlantType.GatlingPea),

                        ((int)PlantType.SplitPea,   (int)PlantType.LanternRepeater),
                        ((int)PlantType.LanternRepeater, (int)PlantType.SplitPea)
                    },
                    1.4f,
                    0f,
                    500,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, LightMegaGatlingPea>(
                    lanternID,
                    assetBundle.GetAsset<GameObject>("SkinSuperLanternGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperLanternGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.Plantern),
                        ((int)PlantType.Plantern, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.LanternGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.LanternGatling),

                        ((int)PlantType.LanternSplit,   (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.LanternSplit),

                        ((int)PlantType.GatlingPea, (int)PlantType.LanternPea),
                        ((int)PlantType.LanternPea, (int)PlantType.GatlingPea),

                        ((int)PlantType.SplitPea,   (int)PlantType.LanternRepeater),
                        ((int)PlantType.LanternRepeater, (int)PlantType.SplitPea)
                    },
                    1.4f,
                    0f,
                    500,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(lanternID);
                CustomCore.TypeMgrExtra.LevelPlants.Add(lanternID, CardLevel.Blue);
                CustomCore.TypeMgrExtra.IsSmallRangeLantern.Add(lanternID);
                CustomCore.AddPlantAlmanacStrings(lanternID, "初版流光超级机枪射手", "");
                /*CustomCore.RegisterCustomPlantClickEvent(lanternID, (Plant p) =>
                {
                    if (p.TryGetComponent<LightMegaGatlingPea>(out var a))
                    {
                        a.OnClicked();
                    }
                });//*/
                CustomCore.CustomPlantClicks.Add(lanternID, (Plant p) =>
                {
                    if (p.TryGetComponent<LightMegaGatlingPea>(out var a))
                    {
                        a.OnClicked();
                    }
                });

                // Electric Mega Gatling (fixed ID)
                CustomCore.RegisterCustomPlant<Shooter, ElectricMegaGatlingPea>(
                    PlantTypeExpand.ElectricMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperElectricGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperElectricGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.ElectricOnion),
                        ((int)PlantType.ElectricOnion, PlantTypeExpand.MegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    100,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, ElectricMegaGatlingPea>(
                    PlantTypeExpand.ElectricMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperElectricGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperElectricGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.ElectricOnion),
                        ((int)PlantType.ElectricOnion, PlantTypeExpand.MegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    100,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.ElectricMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.ElectricMegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.ElectricMegaGatlingPea, "初版电能超级机枪射手", "");

                // Primal Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, PrimalMegaGatlingPea>(
                    PlantTypeExpand.PrimalMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperPrimalGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperPrimalGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.IronPea),
                        ((int)PlantType.IronPea, PlantTypeExpand.MegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    100,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, PrimalMegaGatlingPea>(
                    PlantTypeExpand.PrimalMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperPrimalGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperPrimalGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.IronPea),
                        ((int)PlantType.IronPea, PlantTypeExpand.MegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    100,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.PrimalMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.PrimalMegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.PrimalMegaGatlingPea, "初版原始超级机枪射手", "");

                // Chrono Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, ChronoMegaGatlingPea>(
                    PlantTypeExpand.ChronoMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperChronoGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperChronoGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.PortalPea),
                        ((int)PlantType.PortalPea, PlantTypeExpand.MegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    20,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, ChronoMegaGatlingPea>(
                    PlantTypeExpand.ChronoMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperChronoGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperChronoGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.PortalPea),
                        ((int)PlantType.PortalPea, PlantTypeExpand.MegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    20,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.ChronoMegaGatlingPea);
                CustomCore.TypeMgrExtra.IsMagnetPlants.Add(PlantTypeExpand.ChronoMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.ChronoMegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.ChronoMegaGatlingPea, "初版时空超级机枪射手", "");

                // Doom Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, DoomMegaGatlingPea>(
                    PlantTypeExpand.DoomMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperDoomGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperDoomGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantType.DoomGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.DoomGatling),

                        ((int)PlantType.GatlingPea, (int)PlantType.DoomPeashooter),
                        ((int)PlantType.DoomPeashooter, (int)PlantType.GatlingPea),

                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.DoomShroom),
                        ((int)PlantType.DoomShroom, PlantTypeExpand.MegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    1800,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, DoomMegaGatlingPea>(
                    PlantTypeExpand.DoomMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperDoomGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperDoomGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantType.DoomGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.DoomGatling),

                        ((int)PlantType.GatlingPea, (int)PlantType.DoomPeashooter),
                        ((int)PlantType.DoomPeashooter, (int)PlantType.GatlingPea),

                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.DoomShroom),
                        ((int)PlantType.DoomShroom, PlantTypeExpand.MegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    1800,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.DoomMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.DoomMegaGatlingPea, CardLevel.Purple);
                CustomCore.AddUltimatePlant(PlantTypeExpand.DoomMegaGatlingPea);
                CustomCore.RegisterCustomBanMix(PlantTypeExpand.DoomMegaGatlingPea, () => (TravelMgr.Instance != null && TravelMgr.Instance.data.unlockedWeaks.Contains(PlantType.DoomGatling)) ||
                Board.Instance.boardTag.enableAllTravelPlant || Board.Instance.boardTag.isSuperRandom || Board.Instance.boardTag.isUltimateSuperRandom || GameAPP.developerMode,
                    null, () => InGameText.Instance.ShowText("该配方需要抽取", 3f));
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.DoomMegaGatlingPea, "初版毁灭超级机枪射手", "");

                // Ultimate Doom Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, UltimateDoomMegaGatlingPea>(
                    PlantTypeExpand.UltimateDoomMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("UltimateSuperDoomGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("UltimateSuperDoomGatlingPreview"),
                    new List<(int, int)> {
                        (PlantTypeExpand.DoomMegaGatlingPea, (int)PlantType.DoomShroom),
                        ((int)PlantType.DoomShroom, PlantTypeExpand.DoomMegaGatlingPea),

                        ((int)PlantType.DoomGatling, (int)PlantType.DoomPeashooter),
                        ((int)PlantType.DoomPeashooter, (int)PlantType.DoomGatling),

                        ((int)PlantType.UltimateDoomGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.UltimateDoomGatling)
                    },
                    1.4f,
                    0f,
                    1800,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, UltimateDoomMegaGatlingPea>(
                    PlantTypeExpand.UltimateDoomMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinUltimateSuperDoomGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("UltimateSuperDoomGatlingPreview"),
                    new List<(int, int)> {
                        (PlantTypeExpand.DoomMegaGatlingPea, (int)PlantType.DoomShroom),
                        ((int)PlantType.DoomShroom, PlantTypeExpand.DoomMegaGatlingPea),

                        ((int)PlantType.DoomGatling, (int)PlantType.DoomPeashooter),
                        ((int)PlantType.DoomPeashooter, (int)PlantType.DoomGatling),

                        ((int)PlantType.UltimateDoomGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.UltimateDoomGatling)
                    },
                    1.4f,
                    0f,
                    1800,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.UltimateDoomMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.UltimateDoomMegaGatlingPea, CardLevel.Gold);
                CustomCore.AddUltimatePlant(PlantTypeExpand.UltimateDoomMegaGatlingPea);
                CustomCore.RegisterCustomBanMix(PlantTypeExpand.UltimateDoomMegaGatlingPea, () => (Lawnf.TravelAdvanced(CoreTools.GetAdvBuffByString("枕戈待旦")) && Lawnf.TravelAdvanced(CoreTools.GetAdvBuffByString("核能威慑"))) ||
                Board.Instance.boardTag.enableAllTravelPlant || Board.Instance.boardTag.isSuperRandom || Board.Instance.boardTag.isUltimateSuperRandom || GameAPP.developerMode,
                    null, () => InGameText.Instance.ShowText("该配方需要抽取", 3f));
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.UltimateDoomMegaGatlingPea, "初版余烬毁灭超级机枪射手", "");

                // Regular Cherry Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, RegularCherryMegaGatlingPea>(
                    PlantTypeExpand.RegularCherryMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("RegularSuperCherryGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("RegularSuperCherryGatlingPreview"),
                    new List<(int, int)> {
                        (PlantTypeExpand.MegaGatlingPea, (int)PlantType.CherryBomb),
                        ((int)PlantType.CherryBomb, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.CherryGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.CherryGatling),

                        ((int)PlantType.CherrySplit, (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.CherrySplit),

                        ((int)PlantType.DoubleCherry, (int)PlantType.SplitPea),
                        ((int)PlantType.SplitPea, (int)PlantType.DoubleCherry),

                        ((int)PlantType.Cherryshooter, (int)PlantType.GatlingPea),
                        ((int)PlantType.GatlingPea, (int)PlantType.Cherryshooter)
                    },
                    1.4f,
                    0f,
                    40,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, RegularCherryMegaGatlingPea>(
                    PlantTypeExpand.RegularCherryMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinRegularSuperCherryGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("RegularSuperCherryGatlingPreview"),
                    new List<(int, int)> {
                        (PlantTypeExpand.MegaGatlingPea, (int)PlantType.CherryBomb),
                        ((int)PlantType.CherryBomb, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.CherryGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.CherryGatling),

                        ((int)PlantType.CherrySplit, (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.CherrySplit),

                        ((int)PlantType.DoubleCherry, (int)PlantType.SplitPea),
                        ((int)PlantType.SplitPea, (int)PlantType.DoubleCherry),

                        ((int)PlantType.Cherryshooter, (int)PlantType.GatlingPea),
                        ((int)PlantType.GatlingPea, (int)PlantType.Cherryshooter)
                    },
                    1.4f,
                    0f,
                    40,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.RegularCherryMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.RegularCherryMegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.RegularCherryMegaGatlingPea, "初版樱桃超级机枪射手", "");

                // Cherry Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, CherryMegaGatlingPea>(
                    PlantTypeExpand.CherryMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperCherryGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperCherryGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantType.UltimateGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.UltimateGatling),

                        (PlantTypeExpand.RegularCherryMegaGatlingPea, (int)PlantType.SuperCherryShooter),
                        ((int)PlantType.SuperCherryShooter, PlantTypeExpand.RegularCherryMegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    GameStrategy.SuperCherryDamage,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, CherryMegaGatlingPea>(
                    PlantTypeExpand.CherryMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperCherryGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperCherryGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantType.UltimateGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.UltimateGatling),

                        (PlantTypeExpand.RegularCherryMegaGatlingPea, (int)PlantType.SuperCherryShooter),
                        ((int)PlantType.SuperCherryShooter, PlantTypeExpand.RegularCherryMegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    GameStrategy.SuperCherryDamage,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.CherryMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.CherryMegaGatlingPea, CardLevel.Gold);
                CustomCore.AddUltimatePlant(PlantTypeExpand.CherryMegaGatlingPea);
                CustomCore.RegisterCustomBanMix(PlantTypeExpand.CherryMegaGatlingPea, () => (TravelMgr.Instance != null && TravelMgr.Instance.GetUnlocksPool().Contains(TravelUnlocks.UltimateGatling) && Lawnf.TravelUltimate(UltiBuff.EnumValue2) && Lawnf.TravelUltimate(UltiBuff.EnumValue3)) ||
                Board.Instance.boardTag.enableAllTravelPlant || GameAPP.developerMode,
                    null, () => InGameText.Instance.ShowText("该配方需要抽取", 3f));
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.CherryMegaGatlingPea, "终极樱桃机枪(SP究极樱桃射手)", "");

                // Hypno Cherry Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, HypnoCherryMegaGatlingPea>(
                    PlantTypeExpand.HypnoMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperHypnoGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperHypnoGatlingPreview"),
                    new List<(int, int)> {
                        (PlantTypeExpand.MegaGatlingPea, (int)PlantType.HypnoShroom),
                        ((int)PlantType.HypnoShroom, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.HypnoGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.HypnoGatling),

                        ((int)PlantType.HypnoSplit, (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.HypnoSplit),

                        ((int)PlantType.HypnoRepeater, (int)PlantType.SplitPea),
                        ((int)PlantType.SplitPea, (int)PlantType.HypnoRepeater),

                        ((int)PlantType.HypnoPeashooter, (int)PlantType.GatlingPea),
                        ((int)PlantType.GatlingPea, (int)PlantType.HypnoPeashooter)
                    },
                    1.4f,
                    0f,
                    40,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, HypnoCherryMegaGatlingPea>(
                    PlantTypeExpand.HypnoMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperHypnoGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperHypnoGatlingPreview"),
                    new List<(int, int)> {
                        (PlantTypeExpand.MegaGatlingPea, (int)PlantType.HypnoShroom),
                        ((int)PlantType.HypnoShroom, PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.HypnoGatling, (int)PlantType.Peashooter),
                        ((int)PlantType.Peashooter, (int)PlantType.HypnoGatling),

                        ((int)PlantType.HypnoSplit, (int)PlantType.DoubleShooter),
                        ((int)PlantType.DoubleShooter, (int)PlantType.HypnoSplit),

                        ((int)PlantType.HypnoRepeater, (int)PlantType.SplitPea),
                        ((int)PlantType.SplitPea, (int)PlantType.HypnoRepeater),

                        ((int)PlantType.HypnoPeashooter, (int)PlantType.GatlingPea),
                        ((int)PlantType.GatlingPea, (int)PlantType.HypnoPeashooter)
                    },
                    1.4f,
                    0f,
                    40,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.HypnoMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.HypnoMegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.HypnoMegaGatlingPea, "初版魅惑超级机枪射手", "");

                // Three-lane Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, ThreeMegaGatlingPea>(
                    PlantTypeExpand.ThreeMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperThreeGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperThreeGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.ThreePeater),
                        ((int)PlantType.ThreePeater, (int)PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.GatlingPea, (int)PlantType.AllPeater),
                        ((int)PlantType.AllPeater, (int)PlantType.GatlingPea)
                    },
                    1.4f,
                    0f,
                    210,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, ThreeMegaGatlingPea>(
                    PlantTypeExpand.ThreeMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperThreeGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperThreeGatlingPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.MegaGatlingPea, (int)PlantType.ThreePeater),
                        ((int)PlantType.ThreePeater, (int)PlantTypeExpand.MegaGatlingPea),

                        ((int)PlantType.GatlingPea, (int)PlantType.AllPeater),
                        ((int)PlantType.AllPeater, (int)PlantType.GatlingPea)
                    },
                    1.4f,
                    0f,
                    210,
                    300,
                    5f,
                    500,
                    new List<(BulletType, List<GameObject?>)>
                    {
                        (BulletType.Bullet_pea,new List<GameObject?> {assetBundle.GetAsset<GameObject>("Bullet_pea_super")})
                    }
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.ThreeMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.ThreeMegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.ThreeMegaGatlingPea, "初版三线超级机枪射手", "");

                // Wild Mega Gatling
                CustomCore.RegisterCustomPlant<Shooter, WildMegaGatlingPea>(
                    PlantTypeExpand.WildMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperWildGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperWildGatlingPreview"),
                    new List<(int, int)> {
                        ((int) PlantTypeExpand.MegaGatlingPea, (int) PlantType.GatlingPea),
                        ((int) PlantType.GatlingPea, (int) PlantTypeExpand.MegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    210,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Shooter, WildMegaGatlingPea>(
                    PlantTypeExpand.WildMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperWildGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperWildGatlingPreview"),
                    new List<(int, int)> {
                        ((int) PlantTypeExpand.MegaGatlingPea, (int) PlantType.GatlingPea),
                        ((int) PlantType.GatlingPea, (int) PlantTypeExpand.MegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    210,
                    300,
                    5f,
                    500,
                    new List<(BulletType, List<GameObject?>)>
                    {
                        (BulletType.Bullet_pea,new List<GameObject?> {assetBundle.GetAsset<GameObject>("Bullet_pea_super")})
                    }
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.WildMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.WildMegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.WildMegaGatlingPea, "初版狂野超级机枪射手", "");

                // Wild Mega Gatling
                CustomCore.RegisterCustomPlant<Plant, StarMegaGating>(
                    PlantTypeExpand.StarMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SuperStarGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperStarGatlingPreview"),
                    new List<(int, int)> {
                        ((int) PlantTypeExpand.MegaGatlingPea, (int) PlantType.StarFruit),
                        ((int) PlantType.StarFruit, (int) PlantTypeExpand.MegaGatlingPea),
                        ((int) PlantType.StarPea, (int) PlantType.GatlingPea),
                        ((int) PlantType.GatlingPea, (int) PlantType.StarPea)
                    },
                    1.4f,
                    0f,
                    20,
                    300,
                    5f,
                    500
                );
                CustomCore.RegisterCustomPlantSkin<Plant, StarMegaGating>(
                    PlantTypeExpand.StarMegaGatlingPea,
                    assetBundle.GetAsset<GameObject>("SkinSuperStarGatlingPrefab"),
                    assetBundle.GetAsset<GameObject>("SuperStarGatlingPreview"),
                    new List<(int, int)> {
                        ((int) PlantTypeExpand.MegaGatlingPea, (int) PlantType.StarFruit),
                        ((int) PlantType.StarFruit, (int) PlantTypeExpand.MegaGatlingPea),
                        ((int) PlantType.StarPea, (int) PlantType.GatlingPea),
                        ((int) PlantType.GatlingPea, (int) PlantType.StarPea)
                    },
                    1.4f,
                    0f,
                    20,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.StarMegaGatlingPea);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.StarMegaGatlingPea, CardLevel.Blue);
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.StarMegaGatlingPea, "初版杨桃超级机枪射手", "");


                // Cherry Mega Gatling Blover
                CustomCore.RegisterCustomPlant<UltimateGatlingBlover, UltimateExplodeGatlingBlover>(
                    PlantTypeExpand.ExplodeGatlingBlover,
                    assetBundle.GetAsset<GameObject>("UltimateGatlingBloverPrefab"),
                    assetBundle.GetAsset<GameObject>("UltimateGatlingBloverPreview"),
                    new List<(int, int)> {
                        ((int)PlantTypeExpand.CherryMegaGatlingPea, (int)PlantType.Blover),
                        ((int)PlantType.Blover, (int)PlantTypeExpand.CherryMegaGatlingPea)
                    },
                    1.4f,
                    0f,
                    GameStrategy.SuperCherryDamage,
                    300,
                    5f,
                    500
                );
                CustomCore.TypeMgrExtra.IsCustomPlant.Add(PlantTypeExpand.ExplodeGatlingBlover);
                CustomCore.TypeMgrExtra.FlyingPlants.Add(PlantTypeExpand.ExplodeGatlingBlover);
                CustomCore.TypeMgrExtra.LevelPlants.Add(PlantTypeExpand.ExplodeGatlingBlover, CardLevel.Gold);
                CustomCore.AddUltimatePlant(PlantTypeExpand.ExplodeGatlingBlover);
                CustomCore.RegisterCustomBanMix(PlantTypeExpand.ExplodeGatlingBlover, () => (TravelMgr.Instance != null && TravelMgr.Instance.GetUnlocksPool().Contains(TravelUnlocks.UltimateGatling) && Lawnf.TravelUltimate(UltiBuff.EnumValue2) && Lawnf.TravelUltimate(UltiBuff.EnumValue3)) ||
                Utils.EnableTravelPlant() || GameAPP.developerMode,
                    null, () => InGameText.Instance.ShowText("该配方需要抽取", 3f));
                CustomCore.AddPlantAlmanacStrings(PlantTypeExpand.ExplodeGatlingBlover, "终极浮空樱桃机枪(SP究极浮空樱桃射手)", "");
            }

            //Bullet registration
            {
                // Bullets
                CustomCore.RegisterCustomBullet<Bullet_pea, PrimalPea>(
                    BulletProfile.PrimalPea,
                    assetBundle.GetAsset<GameObject>("Bullet_primalPea")
                );
                CustomCore.RegisterCustomBullet<Bullet_pea, GooPea>(
                    BulletProfile.GooPea,
                    assetBundle.GetAsset<GameObject>("Bullet_poisonPea")
                );
                CustomCore.RegisterCustomBullet<Bullet_pea, ExtremeFirePea>(
                    BulletProfile.ExtremeFirePea,
                    assetBundle.GetAsset<GameObject>("Bullet_firePea_red")
                );
                CustomCore.RegisterCustomBullet<Bullet_pea, ElectricPea>(
                    BulletProfile.ElectricPea,
                    assetBundle.GetAsset<GameObject>("Bullet_electricPea")
                );
                CustomCore.RegisterCustomBullet<Bullet_pea, HypnoMegaPea>(
                    BulletProfile.HypnoMegaPea,
                    assetBundle.GetAsset<GameObject>("Bullet_hypnoMegaPea")
                );
                CustomCore.RegisterCustomBullet<Bullet_pea, HypnoMegaPea>(
                    BulletProfile.HypnoMegaPeaZ,
                    assetBundle.GetAsset<GameObject>("Bullet_hypnoMegaPeaZ")
                );
                CustomCore.RegisterCustomBullet<Bullet_pea, FlameGooPea>(
                    BulletProfile.FlameGooPea,
                    assetBundle.GetAsset<GameObject>("Bullet_poisonPea_fire")
                );
            }

            //Zombie registration
            // ===============================
            //  MEGA GATLING FAMILY ZOMBIES
            // ===============================
            {
                // --- Super-tier Mega Gatlings ---
                CustomCore.RegisterCustomZombie<PeaShooterZ, MegaGatlingPeaZombie>(
                    (ZombieType)9000,
                    assetBundle.GetAsset<GameObject>("SuperGatlingZombie"),
                    assetBundle.GetAsset<Sprite>("SuperGatlingZombie_0"), 50, 1500, 0, 0
                );
                DataMgr.AddCustomZombieSpawnRatio((ZombieType)9000,5,1000);

                CustomCore.RegisterCustomZombie<PeaShooterZ, HypnoMegaGatlingPeaZombie>(
                    (ZombieType)9001,
                    assetBundle.GetAsset<GameObject>("SuperHypnoGatlingZombie"),
                    assetBundle.GetAsset<Sprite>("SuperHypnoGatlingZombie_0"), 50, 1500, 0, 0
                );
                DataMgr.AddCustomZombieSpawnRatio((ZombieType)9001,5,1000);

                CustomCore.RegisterCustomZombie<PeaShooterZ, SnowMegaGatlingPeaZombie>(
                    (ZombieType)9005,
                    assetBundle.GetAsset<GameObject>("SuperSnowGatlingZombie"),
                    assetBundle.GetAsset<Sprite>("SuperSnowGatlingZombie_0"), 50, 1500, 0, 0
                );
                DataMgr.AddCustomZombieSpawnRatio((ZombieType)9005,5,1000);

                // --- Ultimate-tier Mega Gatlings ---
                CustomCore.RegisterCustomZombie<PeaShooterZ, ElectricMegaGatlingPeaZombie>(
                    (ZombieType)9002,
                    assetBundle.GetAsset<GameObject>("SuperElectricGatlingZombie"),
                    assetBundle.GetAsset<Sprite>("SuperElectricGatlingZombie_0"), 20, 15000, 15000, 15000
                );
                DataMgr.AddCustomZombieSpawnRatio((ZombieType)9002,5,0);

                CustomCore.RegisterCustomZombie<PeaShooterZ, DoomMegaGatlingPeaZombie>(
                    (ZombieType)9003,
                    assetBundle.GetAsset<GameObject>("SuperDoomGatlingZombie"),
                    assetBundle.GetAsset<Sprite>("SuperDoomGatlingZombie_0"), 500, 1500, 0, 0
                );
                DataMgr.AddCustomZombieSpawnRatio((ZombieType)9003,5,500);
            }

            // ===============================
            //  PAPER GATLING FAMILY ZOMBIES
            // ===============================
            {
                CustomCore.RegisterCustomZombie<GatlingPaperZombie_c, GatlingPaperZombie_d>(
                    (ZombieType)9004,
                    assetBundle.GetAsset<GameObject>("GatlingPaper_d"),
                    assetBundle.GetAsset<Sprite>("GatlingPaper_d_0"), 400, 50000, 0, 6000
                );
                DataMgr.AddCustomZombieSpawnRatio((ZombieType)9004,5,0);

                CustomCore.RegisterCustomZombie<GatlingPaperZombie_c, DoomPaperZombie_d>(
                    (ZombieType)9006,
                    assetBundle.GetAsset<GameObject>("DoomGatlingPaper_d"),
                    assetBundle.GetAsset<Sprite>("DoomGatlingPaper_d_0"), 400, 50000, 0, 6000
                );
                DataMgr.AddCustomZombieSpawnRatio((ZombieType)9006,5,0);
            }

            // ===============================
            //  TIER LISTS
            // ===============================
            {
                // Elite Zombies
                CustomCore.TypeMgrExtra.EliteZombie.Add((ZombieType)9002);
                CustomCore.TypeMgrExtra.EliteZombie.Add((ZombieType)9004);
                CustomCore.TypeMgrExtra.EliteZombie.Add((ZombieType)9006);

                // Ultimate Zombies
                CustomCore.TypeMgrExtra.UltimateZombie.Add((ZombieType)9001);
                CustomCore.TypeMgrExtra.UltimateZombie.Add((ZombieType)9002);
                CustomCore.TypeMgrExtra.UltimateZombie.Add((ZombieType)9003);
                CustomCore.TypeMgrExtra.UltimateZombie.Add((ZombieType)9004);
                CustomCore.TypeMgrExtra.UltimateZombie.Add((ZombieType)9005);
                CustomCore.TypeMgrExtra.UltimateZombie.Add((ZombieType)9006);

                // Hypno-useless (cannot be hypnotized)
                CustomCore.TypeMgrExtra.UselessHypnoZombie.Add((ZombieType)9002);
                CustomCore.TypeMgrExtra.UselessHypnoZombie.Add((ZombieType)9004);
                CustomCore.TypeMgrExtra.UselessHypnoZombie.Add((ZombieType)9006);

                // Not random-spawnable
                CustomCore.TypeMgrExtra.NotRandomZombie.Add((ZombieType)9002);
                CustomCore.TypeMgrExtra.NotRandomZombie.Add((ZombieType)9004);
                CustomCore.TypeMgrExtra.NotRandomZombie.Add((ZombieType)9006);
            }

            // ===============================
            //  ALMANAC STRINGS
            // ===============================
            {
                CustomCore.AddZombieAlmanacStrings(9000, "初版超级机枪僵尸", "注意：在图鉴中，血量和攻击伤害的显示存在BUG");
                CustomCore.AddZombieAlmanacStrings(9001, "初版魅惑超级机枪僵尸", "注意：在图鉴中，血量和攻击伤害的显示存在BUG");
                CustomCore.AddZombieAlmanacStrings(9002, "初版电能超级机枪僵尸", "吓哭了\n注意：在图鉴中，血量和攻击伤害的显示存在BUG");
                CustomCore.AddZombieAlmanacStrings(9003, "初版余烬毁灭超级机枪僵尸", "注意：在图鉴中，血量和攻击伤害的显示存在BUG");
                CustomCore.AddZombieAlmanacStrings(9004, "樱桃机枪教授僵尸", "注意：在图鉴中，血量和攻击伤害的显示存在BUG");
                CustomCore.AddZombieAlmanacStrings(9005, "初版寒冰超级机枪僵尸", "注意：在图鉴中，血量和攻击伤害的显示存在BUG");
                CustomCore.AddZombieAlmanacStrings(9006, "毁灭机枪教授僵尸", "注意：在图鉴中，血量和攻击伤害的显示存在BUG");
            }

            // ===============================
            //  BUFF REGISTRATION
            // ===============================
            {
                Buff1 = Compatibility.CustomCore_Old.RegisterCustomDebuff(
                    "超级电能机枪僵尸将与旗帜僵尸一起出现",
                    zombieType: (ZombieType)9002,
                    bg: BuffBgType.Night
                );

                Buff2 = Compatibility.CustomCore_Old.RegisterCustomDebuff(
                    "超级机枪僵尸现在会发射随机子弹",
                    zombieType: (ZombieType)9000,
                    bg: BuffBgType.Night
                );

                Buff3 = Compatibility.CustomCore_Old.RegisterCustomDebuff(
                    "超级机枪僵尸现在开大子弹增加30和超级机枪僵尸发射的子弹不再能被反射",
                    zombieType: (ZombieType)9000,
                    bg: BuffBgType.Night
                );
            }

            /*CustomCore.RegisterCustomPlantSkin<GatlingPea, RedGatlingPea>((int)PlantType.GatlingPea, assetBundle.GetAsset<GameObject>("SkinGatlingPeaPrefab"), assetBundle.GetAsset<GameObject>("GatlingPeaPreview"), (GatlingPea pea) => {},
                    new List<(BulletType, List<GameObject?>)>
                    {
                        (BulletType.Bullet_pea,new List<GameObject?> {assetBundle.GetAsset<GameObject>("Bullet_pea_super")})
                    });*/

            //Cross fusion
            {
                // Cross-fusions between core Mega variants (excluding Wild/Cherry/Doom)
                var coreMegas = new[] {
                    PlantTypeExpand.MegaGatlingPea,
                    PlantTypeExpand.IceMegaGatlingPea,
                    PlantTypeExpand.FireMegaGatlingPea,
                    PlantTypeExpand.PrimalMegaGatlingPea,
                    PlantTypeExpand.GooMegaGatlingPea,
                    PlantTypeExpand.ElectricMegaGatlingPea,
                    PlantTypeExpand.ThreeMegaGatlingPea,
                    PlantTypeExpand.RegularCherryMegaGatlingPea,
                    PlantTypeExpand.HypnoMegaGatlingPea,
                    PlantTypeExpand.ChronoMegaGatlingPea,
                    PlantTypeExpand.SunMegaGatlingPea,
                    PlantTypeExpand.WildMegaGatlingPea,
                    PlantTypeExpand.StarMegaGatlingPea,
                    lanternID
                };

                var catalysts = new Dictionary<PlantTypeExpand, PlantType> {
                    { PlantTypeExpand.MegaGatlingPea,        PlantType.Peashooter },
                    { PlantTypeExpand.IceMegaGatlingPea,      PlantType.IceShroom },
                    { PlantTypeExpand.FireMegaGatlingPea,     PlantType.Jalapeno },
                    { PlantTypeExpand.PrimalMegaGatlingPea,   PlantType.IronPea },
                    { PlantTypeExpand.GooMegaGatlingPea,      PlantType.Garlic },
                    { PlantTypeExpand.ElectricMegaGatlingPea, PlantType.ElectricOnion },
                    { PlantTypeExpand.ThreeMegaGatlingPea, PlantType.ThreePeater },
                    { PlantTypeExpand.RegularCherryMegaGatlingPea, PlantType.CherryBomb },
                    { PlantTypeExpand.HypnoMegaGatlingPea, PlantType.HypnoShroom },
                    { PlantTypeExpand.ChronoMegaGatlingPea, PlantType.PortalPea },
                    { PlantTypeExpand.SunMegaGatlingPea, PlantType.SunFlower },
                    { PlantTypeExpand.WildMegaGatlingPea, PlantType.GatlingPea },
                    { PlantTypeExpand.StarMegaGatlingPea, PlantType.StarFruit },
                    { lanternID, PlantType.Plantern }
                };

                foreach (var target in coreMegas)
                {
                    foreach (var source in coreMegas)
                    {
                        if (source == target)
                            continue;

                        int catalyst = (int)catalysts[target];
                        CustomCore.AddFusion((int)target, (int)source, catalyst);
                        CustomCore.AddFusion(smallID, (int)PlantType.SmallPuff, (int)source);
                    }
                    if (target != lanternID && target != PlantTypeExpand.StarMegaGatlingPea)
                        CustomCore.RegisterSuperSkill((int)target, p => 1000, (Plant p) =>
                        {
                            if (p.TryGetComponent<MegaGatlingPea>(out var component))
                            {
                                component.isPF = true;
                                component.StartPF();
                            }
                        });
                    if( target != PlantTypeExpand.PrimalMegaGatlingPea)
                    {
                        CustomCore.RegisterCustomUseItemOnPlantEvent(target,BucketType.Bucket,PlantTypeExpand.PrimalMegaGatlingPea);
                    }
                    if( target != PlantTypeExpand.ChronoMegaGatlingPea)
                    {
                        CustomCore.RegisterCustomUseItemOnPlantEvent(target,BucketType.PortalHeart,PlantTypeExpand.ChronoMegaGatlingPea);
                    }
                    if (!CustomCore.CustomBanMix.ContainsKey(target))
                    {
                        CustomCore.RegisterCustomBanMix(target,()=>GlobalTracker.IsCustomLevel && GlobalTracker.CustomLevelID==LevelID || LevelProgressionManager.IsCompleted(LevelID) || Board.Instance.TryGetComponent<BoardEnablerEffect>(out _) ,null,() => InGameText.Instance.ShowText("配方未解锁", 3f));
                    }
                    if (!CustomCore.CustomUltimatePlants.Contains(target))
                    {
                        CustomSuperPlants.Add(target);
                    }
                }
            }

            //Super skills
            {
                CustomCore.RegisterSuperSkill(lanternID, p => 1000, (Plant p) =>
                {
                    if (p.TryGetComponent<LightMegaGatlingPea>(out var component))
                    {
                        component.StartPF();
                    }
                });
                CustomCore.RegisterSuperSkill(PlantTypeExpand.StarMegaGatlingPea, p => 1000, (Plant p) =>
                {
                    if (p.TryGetComponent<BaseCustomPlant>(out var component))
                    {
                        component.StartPF();
                    }
                });
                DataMgr.RegisterCustomStarUp(PlantTypeExpand.StarMegaGatlingPea);
                CustomCore.RegisterSuperSkill(PlantTypeExpand.ExplodeGatlingBlover, p => 1000, (Plant p) =>
                {
                    if (p.TryGetComponent<UltimateExplodeGatlingBlover>(out var component))
                    {
                        component.plant.StartCoroutine(component.SuperShoot_Custom());
                    }
                });
                CustomCore.RegisterSuperSkill(PlantTypeExpand.DoomMegaGatlingPea, p => 1000, (Plant p) =>
                {
                    if (p.TryGetComponent<MegaGatlingPea>(out var component))
                    {
                        component.isPF = true;
                        component.StartPF();
                    }
                });
                CustomCore.RegisterSuperSkill(PlantTypeExpand.UltimateDoomMegaGatlingPea, p => 1000, (Plant p) =>
                {
                    if (p.TryGetComponent<MegaGatlingPea>(out var component))
                    {
                        component.isPF = true;
                        component.StartPF();
                    }
                });
                CustomCore.RegisterSuperSkill(PlantTypeExpand.CherryMegaGatlingPea, p => 1000, (Plant p) =>
                {
                    if (p.TryGetComponent<MegaGatlingPea>(out var component))
                    {
                        component.isPF = true;
                        component.StartPF();
                    }
                });
            }

            DataMgr.RegisterCustomBossHealthSlider((ZombieType)9002,assetBundle.GetAsset<GameObject>("ElectricSlider"));
            DataMgr.RegisterCustomBossHealthSlider((ZombieType)9004,assetBundle.GetAsset<GameObject>("CherryGSlider"));
            DataMgr.RegisterCustomBossHealthSlider((ZombieType)9006,assetBundle.GetAsset<GameObject>("DoomGSlider"));
            Log.LogInfo($"{PluginGuid} {PluginVersion} loaded.");
        }
    }
}
