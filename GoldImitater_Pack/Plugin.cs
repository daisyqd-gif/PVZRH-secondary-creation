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
global using Core;
global using System.Linq;
global using Unity.VisualScripting;


namespace GoldImitater
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle ab1;
        private AssetBundle ab2;
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            ab1 = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "goldimitater"
            );
            ab2 = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "ultimategoldimitater"
            );
        }
        public override void InitializePlants()
        {
            CustomCore.RegisterCustomPlant<Imitater, GoldImitater>(GoldImitater.PlantID, ab1.GetAsset<GameObject>("GoldImitaterPrefab"),
                ab1.GetAsset<GameObject>("GoldImitaterPreview"), new(), 0f, 0f, 0, 300, 15, 50);
            CustomCore.RegisterCustomPlantSkin<Imitater, GoldImitater>(GoldImitater.PlantID, ab1.GetAsset<GameObject>("GoldImitaterPrefabNewYear"),
                ab1.GetAsset<GameObject>("GoldImitaterPreviewNewYear"), new(), 0f, 0f, 0, 300, 15, 50);
            CustomCore.AddPlantAlmanacStrings(GoldImitater.PlantID, $"黄金模仿者({GoldImitater.PlantID})",
                "或许是宝藏呢？\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>特点：</color><color=red>出场1.5秒后，生成随机植物，僵尸和特殊产出</color>\n" +
                "<color=#3D1400>概率明细：</color>\n" +
                "<color=red>各类普通植物（40%）；究极植物（20%），各类普通僵尸（20%）；究极僵尸（10%）；领袖及Boss僵尸（5%）；其他事件（5%）。生成的僵尸随机具有0.8/1/1.2倍韧性</color>\n" +
                "<color=#3D1400>事件明细：</color>\n" +
                "<color=red>①生成10张卡片：黄金模仿者（50%）；钻石模仿者（50%）\n" +
                "②获得一个词条：植物词条（90%）；僵尸词条（10%）\n" +
                "③获得1000阳光</color>\n" +
                "<color=#3D1400>特殊强化：</color><color=red>①<Boss>僵王博士：血量x100\n" +
                "②<Boss>黄金僵王博士：血量x100\n" +
                "③<Boss>黑橄榄大帅：血量x15</color>\n" +
                "<color=#3D1400>词条1：</color><color=red>孤注一掷：黄金模仿者出现究极植物与究极僵尸的概率大幅提高\n" +
                "*概率明细：\n" +
                "各类普通植物（10%）；究极植物（35%），各类普通僵尸（5%）；究极僵尸（30%）；领袖及Boss僵尸（15%）；其他事件（5%）。</color>\n\n" +
                "<color=#3D1400>“茄本无相，吾有万象。”黄金模仿者侃侃而谈：“就像你看到的，我可以是任何植物，任何僵尸，在后院奋战的豌豆是我，高举旗帜冲锋的僵尸也是我，沉入土壤的尸体是我，哇哇啼哭的孩童也是我。但是'我'与'我'之间的性格是不同的，记忆是不同的，童年成长都是不同的，他们各司其职，他们努力生活。“那拥有不同记忆不同性格的你还是你么”黄金模仿者似乎陷入了沉思……不再说话。</color>\n\n" +
                "<color=#955300>价格：</color><color=red>50</color>\n" +
                "<color=#955300>冷却：</color><color=red>15秒</color>");
            CustomCore.RegisterCustomCardToColorfulCards((PlantType)GoldImitater.PlantID, 14);
            GoldImitater.buff = CustomCore.RegisterCustomBuff("孤注一掷：黄金模仿者出现究极植物与究极僵尸的概率大幅提高", BuffType.AdvancedBuff, () => true, 5000, plantType: (PlantType)GoldImitater.PlantID);
            
            
            CustomCore.RegisterCustomPlant<Imitater, UltimateGoldImitater>(UltimateGoldImitater.PlantID, ab2.GetAsset<GameObject>("UltimateGoldImitaterPrefab"),
                ab2.GetAsset<GameObject>("UltimateGoldImitaterPreview"), new(), 0f, 0f, 0, 300, 15f, 50);
            CustomCore.AddUltimatePlant(UltimateGoldImitater.PlantID);
            CustomCore.AddPlantAlmanacStrings(UltimateGoldImitater.PlantID, $"究极黄金模仿者",
                $"孤注一掷，遍历死地而后生！\n" +
                $"<color=blue>黄金模仿者的限定形态</color>\n\n" +
                $"<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                $"<color=#3D1400>特点：</color><color=red>①不可被铲除\n" +
                $"②不会被僵尸索敌\n" +
                $"③出场1.5秒后，生成随机植物，僵尸和各种产出，概率均为随机值\n" +
                $"④使用钻石模仿者，黄金模仿者或究极黄金模仿者时，重置此次各类事件的概率\n" +
                $"⑤使用僵尸模仿者时，消耗700阳光，重置此次各类事件的概率，并使随机两个事件的概率大幅提高\n" +
                $"⑥使用各类模仿者重置五次时，使随机一个事件的概率提升至100%，此时不能再通过使用模仿者使其重置概率</color>\n" +
                $"<color=#3D1400>概率明细：</color>\n" +
                $"<color=red>各类普通植物（？%）\n" +
                $"究极植物（？%）\n" +
                $"各类普通僵尸（？%）\n" +
                $"究极僵尸（？%）\n" +
                $"领袖和Boss僵尸（？%）\n" +
                $"其他事件（？%）</color>\n" +
                $"<color=#3D1400>事件明细：</color><color=red>①生成十张卡片：究极黄金模仿者（？%）；黄金模仿者（？%）；钻石模仿者（？%）\n" +
                $"②获得一个词条：植物词条（？%）；僵尸词条（？%）\n" +
                $"③获得？阳光</color>\n" +
                $"<color=#3D1400>特殊强化：</color><color=red>①<Boss>僵王博士：血量x？，免疫寒冷，免疫冻结\n" +
                $"②<Boss>黄金僵王博士：血量x？，免疫寒冷，免疫冻结\n" +
                $"③<Boss>黑橄榄大帅：血量x？</color>\n" +
                $"<color=#3D1400>词条1:</color><color=red>孤注一掷：黄金模仿者和究极黄金模仿者随机究极的概率大幅提升</color>\n\n" +
                $"<color=#3D1400>“欲戴其冠，必承其重”\n" +
                $"那枚头冠，从出生起，就戴在他的头上，人们都说他是天选，这是他的宿命。在他很小的时候，他的父母带他到那尊巨大面前，幼小的他看着雕像上巨大的头冠，在对比自己的，自己的头冠更像是一枚精巧的戒指，落在他的小脑袋上，他不懂那意味着什么，只是指着头冠“像～”又指了指雕像。\n" +
                $"那尊巨大的雕像，曾是带来希望和财富的象征，再有象征性的事物，在经过历史的长河时，总会丢失些什么，而这座雕像丢失的，这枚头冠丢失的，正是希望。人们指望这个带着头冠小孩儿为他们创造财富，日子一天一天过去，孩子一天一天长大，人们从期待逐渐变得怀疑，直到最后变得愤怒！“那个孩子，他不能带给我们财富，那就是祸害！我们会变成这样，我们变得平庸，我们没有富贵，都是因为他！抓住他！把他烧了！”门口的人越聚越多，就像是挤在蜂巢的蜜蜂……\n" +
                $"“后来呢。”\n" +
                $"“后来，我逃出来了，我一个人逃出来了。”</color>\n\n" +
                $"<color=#955300>花费：</color><color=red>50</color>\n" +
                $"<color=#955300>冷却：</color><color=red>15秒</color>");
            CustomCore.RegisterCustomClickCardOnPlantEvent(UltimateGoldImitater.PlantID, PlantType.DiamondImitater,
                (p) => p.GetComponent<UltimateGoldImitater>().FeedPlant(PlantType.DiamondImitater), (p) => !p.GetComponent<UltimateGoldImitater>().isLock,
                new CustomClickCardOnPlant()
                {
                    Trigger = CustomClickCardOnPlant.TriggerType.All
                });
            CustomCore.RegisterCustomClickCardOnPlantEvent(UltimateGoldImitater.PlantID, (PlantType)1931,
                (p) => p.GetComponent<UltimateGoldImitater>().FeedPlant((PlantType)1931), (p) => !p.GetComponent<UltimateGoldImitater>().isLock,
                new CustomClickCardOnPlant()
                {
                    Trigger = CustomClickCardOnPlant.TriggerType.All
                });
            CustomCore.RegisterCustomClickCardOnPlantEvent(UltimateGoldImitater.PlantID, UltimateGoldImitater.PlantID,
                (p) => p.GetComponent<UltimateGoldImitater>().FeedPlant(UltimateGoldImitater.PlantID), (p) => !p.GetComponent<UltimateGoldImitater>().isLock,
                new CustomClickCardOnPlant()
                {
                    Trigger = CustomClickCardOnPlant.TriggerType.All
                });
            CustomCore.RegisterCustomClickCardOnPlantEvent(UltimateGoldImitater.PlantID, (PlantType)1960,
                (p) =>
                {
                    p.GetComponent<UltimateGoldImitater>().FeedPlant((PlantType)1960);
                    Board.Instance.UseSun(700f);
                }, (p) =>
                {
                    if (Board.Instance.theSun < 700)
                        InGameText.Instance.ShowText("需要消耗700阳光", 3f);
                    return !p.GetComponent<UltimateGoldImitater>().isLock && Board.Instance.theSun >= 700;
                },
                new CustomClickCardOnPlant()
                {
                    Trigger = CustomClickCardOnPlant.TriggerType.All
                });
            CustomCore.RegisterCustomCardToColorfulCards(UltimateGoldImitater.PlantID, 14);
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "GoldImitater.Bepinex";
        public const string PluginName = "GoldImitater";
        public const string PluginVersion = "3.7";
    }
}
