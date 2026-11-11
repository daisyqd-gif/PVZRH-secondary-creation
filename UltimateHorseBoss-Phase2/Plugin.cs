using System.Reflection;

using BepInEx;
using CustomPlantClass.CustomizeLib;
using CustomPlantClass.Main;
using CustomPlantClass.Runtime.Core;
using UnityEngine;

namespace HorseBossPhase2
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ID ZombieID = -1;
            public static ID BombCloudID = -1;
            public static CustomItemType SickleID = -1;
            public static CustomItemType QiID = -1;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "ultimatehorsephase2"
            );
            DataContainer.ZombieID = DataMgr.AllocateID();
            DataContainer.BombCloudID = DataMgr.AllocateID();
            DataContainer.SickleID = GameObjectMgr.Register<Sickle>(assetBundle.GetAsset<GameObject>("Sickle"));
            DataContainer.QiID = GameObjectMgr.Register<SwordQi>(assetBundle.GetAsset<GameObject>("Qi"));
        }
        public override void InitializeZombies()
        {
            CustomCore.RegisterCustomZombie<Zombie, UltimateHorsePhase2>
            (
                DataContainer.ZombieID,
                assetBundle.GetAsset<GameObject>("UltimateHorse2_Boss"),
                assetBundle.GetAsset<Sprite>("GatlingBlackFootball2"),
                10000,
                100000,
                0,
                0
            );
            CustomCore.RegisterCustomParticle(DataContainer.BombCloudID,assetBundle.GetAsset<GameObject>("BombCloud"));
            CustomCore.AddZombieAlmanacStrings(DataContainer.ZombieID,new AlmanacData.ZombieInfo()
            {
                name = "<BOSS>机枪黑橄榄大帅",
                introduce = "<color=#3D1400>战争，是刻在大帅身体里的烙印，他是天生的战场主宰，无论什么都无法阻拦他的进攻。铜墙铁壁拦截，便永不疲倦地射击，紧随的部下溃败，便孤身一人继续作战，心爱的坐骑倒下，便亲自踏上地面为它报仇。手中紧握的镰刀是他的武器，也是为爱马预订的墓碑。</color>",
                info = 
                "<color=#00FF00>作者为这个模组开发了三项新技术，如今已精疲力竭。</color>\n"+
                "<color=#3D1400>韧性：</color><color=red>100000</color>\n"+
                "<color=#3D1400>特性：</color><color=red>领袖</color>\n"+
                "<color=#3D1400>子弹伤害：</color><color=red>镰刀伤害 / 100</color>\n"+
                "<color=#3D1400>剑气伤害：</color><color=red>镰刀伤害 / 10</color>\n"+
                "<color=#3D1400>镰刀伤害：</color><color=red>10000</color>\n"+
                "<color=#3D1400>特点：</color>\n"+
                "<color=#3D1400>①</color><color=red>秒杀接触的植物和魅惑僵尸</color>\n"+
                "<color=#3D1400>②</color><color=red>通常只在最右侧一列上下移动</color>\n"+
                "<color=#3D1400>③</color><color=red>在场时，每过一段时间释放一次技能</color>\n"+
                "<color=#3D1400>④</color><color=red>免疫魅惑、寒冷、黄油定身、红温、蒜毒</color>\n"+
                "<color=#3D1400>技能：</color>\n"+
                "<color=#3D1400>①</color><color=red>摇动镰刀，在每行随机召唤一波僵尸</color>\n"+
                "<color=#3D1400>②</color><color=red>向前冲刺到二格，随后跳回</color>\n"+
                "<color=#3D1400>③</color><color=red>进入飞行状态，每0.1秒发射15颗咒铁豌豆</color>\n"+
                "<color=#3D1400>④</color><color=red>向前方射一把镰刀，每0.1秒爆炸一次，对半径 1.5 范围内的植物造成 1000 点伤害</color>\n"+
                "<color=#3D1400>⑤</color><color=red>他跃起，猛击地面，向四面八方射出剑气</color>\n"+
                "<color=#3D1400>⑥</color><color=red>休息几秒</color>\n",
                theZombieType = DataContainer.ZombieID
            });
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "HorseBossPhase2.Bepinex";
        public const string PluginName = "HorseBossPhase2";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
