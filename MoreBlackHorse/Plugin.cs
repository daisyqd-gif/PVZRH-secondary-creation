global using BepInEx;
global using CustomizeLib.BepInEx;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass;
global using CustomPlantClass.Main;
namespace MoreBlackHorse
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ID ZombieId_A = -1;
            public static ID ZombieId_B = -1;
            public static ID ZombieId_C = -1;
            public static ID ZombieId_C2 = -1;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = AssetMgr.LoadBundleFromResource(
                Assembly.GetExecutingAssembly(),
                "blackhorseexpansion"
            );
            DataContainer.ZombieId_A = DataMgr.AllocateID();
            DataContainer.ZombieId_B = DataMgr.AllocateID();
            DataContainer.ZombieId_C = DataMgr.AllocateID();
            DataContainer.ZombieId_C2 = DataMgr.AllocateID();
        }
        public override void InitializeZombies()
        {
            CustomCore.RegisterCustomZombie<BlackHorse, BlackHorse_A>(
                DataContainer.ZombieId_A,
                assetBundle.GetAsset<GameObject>("BlackHorse_a"),
                assetBundle.GetAsset<Sprite>("BlackHorse_a_0"),
                200, //matches csv file
                DefaultZombieStats.UltiZombieA_Health_Armored,
                0, DefaultZombieStats.UltiZombieA_Health_Armored
            );
            DataMgr.AddCustomZombieSpawnRatio(DataContainer.ZombieId_A,5,1000);
            CustomCore.AddZombieAlmanacStrings(DataContainer.ZombieId_A,"黑橄榄骑兵僵尸",
            DataMgr.CreateAlmanacEntry("驾驭着诅咒铁骑冲击植物。",specialeffects:[
                "1级僵尸",
                "乘骑会碾压植物的战马。冲锋出场，撞到植物时施加该植物[5%×(植物血量+韧性)/2]的诅咒层数。"+
                "随后稳步前进，碾压时施加[1%×(植物血量+韧性)/2]的诅咒层数。战马或本体被击杀时下马，回满并保留本体血量，变为黑橄榄僵尸"])+"\n韧性：6000+6000");
            CustomCore.RegisterCustomZombie<BlackHorse, BlackHorse_B>(
                DataContainer.ZombieId_B,
                assetBundle.GetAsset<GameObject>("BlackHorse_b"),
                assetBundle.GetAsset<Sprite>("BlackHorse_b_0"),
                200, //matches csv file
                DefaultZombieStats.UltiZombieB_Health_Armored,
                0, DefaultZombieStats.UltiZombieB_Health_Armored
            );
            DataMgr.AddCustomZombieSpawnRatio(DataContainer.ZombieId_B,6,600);
            CustomCore.AddZombieAlmanacStrings(DataContainer.ZombieId_B,"黑橄榄骑兵队长",
            DataMgr.CreateAlmanacEntry("驾驭着诅咒铁骑冲击植物。",specialeffects:[
                "2级僵尸",
                "乘骑会碾压植物的战马。冲锋出场，撞到植物时施加该植物[5%×(植物血量+韧性)/2]的诅咒层数。"+
                "随后稳步前进，碾压时施加[1%×(植物血量+韧性)/2]的诅咒层数。战马或本体被击杀时下马，回满并保留本体血量，变为黑橄榄队长"])+"\n韧性：12000+12000");
            CustomCore.RegisterCustomZombie<BlackHorse, BlackHorse_C>(
                DataContainer.ZombieId_C,
                assetBundle.GetAsset<GameObject>("BlackHorse_c"),
                assetBundle.GetAsset<Sprite>("BlackHorse_c_0"),
                200, //matches csv file
                DefaultZombieStats.UltiZombieC_Health_Armored,
                0, DefaultZombieStats.UltiZombieC_Health_Armored
            );
            DataMgr.AddCustomZombieSpawnRatio(DataContainer.ZombieId_C,7,300);
            CustomCore.AddZombieAlmanacStrings(DataContainer.ZombieId_C,"黑橄榄骑兵副将",
            DataMgr.CreateAlmanacEntry("驾驭着诅咒铁骑冲击植物。",specialeffects:[
                "3级僵尸",
                "乘骑会碾压植物的战马。冲锋出场，撞到植物时施加该植物[5%×(植物血量+韧性)/2]的诅咒层数。"+
                "随后稳步前进，碾压时施加[1%×(植物血量+韧性)/2]的诅咒层数。战马或本体被击杀时下马，回满并保留本体血量，变为黑橄榄副将"])+"\n韧性：24000+24000");
            CustomCore.RegisterCustomZombie<SuperBlackHorse, BlackHorse_C2>(
                DataContainer.ZombieId_C2,
                assetBundle.GetAsset<GameObject>("BlackHorse_c2"),
                assetBundle.GetAsset<Sprite>("BlackHorse_c2_0"),
                200, //matches csv file
                DefaultZombieStats.UltiZombieC_Health_Armored,
                0, DefaultZombieStats.UltiZombieC_Health_Armored
            );
            DataMgr.AddCustomZombieSpawnRatio(DataContainer.ZombieId_C2,7,300);
            CustomCore.AddZombieAlmanacStrings(DataContainer.ZombieId_C2,"黑橄榄机枪骑兵副将", //that is a long name
            DataMgr.CreateAlmanacEntry("驾驭着诅咒铁骑稳步推进，情势不利时召集部下进攻。",specialeffects:[
                "3级僵尸",
                "免疫击退，持续发射铁豆子弹攻击植物，撞击植物会为其增加诅咒效果，血量过半将摇旗在每一行召唤一个黑橄榄骑兵副将僵尸，"+
                "黑橄榄战马死亡后，自身下马转变为黑橄榄机枪副将"])
                +"\n韧性：24000+24000");
            DataMgr.AddLevelZombie(DataContainer.ZombieId_A,
            "黑橄榄骑兵僵尸\n僵尸等级:1\n韧性:高\n攻击力:高\n"+
            "特点: 乘骑会碾压植物的战马。冲锋出场，撞到植物时施加该植物[5%×(植物血量+韧性)/2]的诅咒层数。"+
            "随后稳步前进，碾压时施加[1%×(植物血量+韧性)/2]的诅咒层数。战马或本体被击杀时下马，回满并保留本体血量，变为黑橄榄僵尸\n"+
            "最早出现:第4轮",
            DataContainer.ZombieId_B,
            "黑橄榄骑兵队长\n僵尸等级:2\n韧性:高\n攻击力:高\n"+
            "特点: 乘骑会碾压植物的战马。冲锋出场，撞到植物时施加该植物[5%×(植物血量+韧性)/2]的诅咒层数。"+
            "随后稳步前进，碾压时施加[1%×(植物血量+韧性)/2]的诅咒层数。战马或本体被击杀时下马，回满并保留本体血量，变为黑橄榄队长\n"+
            "最早出现:第4轮",
            DataContainer.ZombieId_C,
            "黑橄榄骑兵副将\n僵尸等级:3\n韧性:高\n攻击力:压\n"+
            "特点: 乘骑会碾压植物的战马。冲锋出场，撞到植物时施加该植物[5%×(植物血量+韧性)/2]的诅咒层数。"+
            "随后稳步前进，碾压时施加[1%×(植物血量+韧性)/2]的诅咒层数。战马或本体被击杀时下马，回满并保留本体血量，变为黑橄榄队长\n"+
            "最早出现:第4轮",false,ZombieType.Nothing,"",
            DataContainer.ZombieId_C2,
            "黑橄榄机枪骑兵副将\n僵尸等级:3\n韧性:高\n攻击力:压\n"+
            "特点: 驾驭着诅咒铁骑稳步推进，情势不利时召集部下进攻。免疫击退，持续发射铁豆子弹攻击植物，撞击植物会为其增加诅咒效果，血量过半将摇旗在每一行召唤一个黑橄榄骑兵副将僵尸，随后稳步前进，碾压时施加[1%×(植物血量+韧性)/2]的诅咒层数。战马或本体被击杀时下马，回满并保留本体血量，变为黑橄榄副将"+
            "最早出现:第4轮");
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    public class BlackHorse_A : ZombieComponent
    {
        public virtual int GetHorseHealth() => DefaultZombieStats.UltiZombieA_Health_Armored;
        public virtual void Awake()
        {
            zombie.theSecondArmor = transform.FindChild("Horse/head/Armor").gameObject;
            zombie.theSecondArmorHealth = GetHorseHealth();
            zombie.theSecondArmorMaxHealth = GetHorseHealth();
            zombie.theSecondArmorType = Zombie.SecondArmorType.Paper; //Special second armor since the armor is a literal horse
            zombie.UpdateHealthText();
        }
        public virtual ZombieType GetZombieType() => ZombieType.BlackFootball_a;

        public void AnimRevive_Custom()
        {
            if (zombie == null || zombie.axis == null)
                return;

            // 1. Get child position safely
            Transform axis = zombie.axis;
            if (axis.childCount == 0)
                return;

            Vector3 pos = axis.GetChild(0).position;

            // 2. VFX + SFX
            InstanceManager.ParticleManager.SetParticle(
                ParticleType.BombCloud_black,
                new Vector2(pos.x, pos.y + 0.5f)
            );

            GameAPP.PlaySound(SoundType.Explosion);

            // 3. Cache revive data BEFORE destroying
            int row = zombie.theZombieRow;
            int maxHP = (int)zombie.theMaxHealth;
            bool hypno = zombie.isMindControlled;

            // 4. Destroy original zombie
            Destroy(zombie.gameObject);

            // 5. Spawn revived zombie
            Zombie revived = hypno
                ? InstanceManager.CreateZombie.SetZombieWithMindControl(row, GetZombieType(), pos.x)
                : InstanceManager.CreateZombie.SetZombie(row, GetZombieType(), pos.x);

            if (revived == null)
                return;

            // 6. Copy HP
            revived.theHealth = maxHP;
            revived.theMaxHealth = maxHP;
            revived.UpdateHealthText();
        }
    }
    public class BlackHorse_B : BlackHorse_A
    {
        public override int GetHorseHealth() => DefaultZombieStats.UltiZombieB_Health_Armored;
        public override ZombieType GetZombieType() => ZombieType.BlackFootball_b;
    }
    public class BlackHorse_C : BlackHorse_A
    {
        public override int GetHorseHealth() => DefaultZombieStats.UltiZombieC_Health_Armored;
        public override ZombieType GetZombieType() => ZombieType.BlackFootball_c;
    }
    public class BlackHorse_C2 : BlackHorse_A
    {
        public override void Awake()
        {
            base.Awake();
            zombie.shoot = transform.FindChild("Zombie/GatlingPea_head/Shoot");
        }
        public override int GetHorseHealth() => DefaultZombieStats.UltiZombieC_Health_Armored;
        public override ZombieType GetZombieType() => ZombieType.BlackFootball_c2;
        public void AnimFlagUp_Custom()
        {
            Board board = zombie.board;
            if (board == null)
                return;
            for (int row = 0; row < board.rowNum; row++)
            {
                var roadType = board.roadType;
                if (roadType == null)
                    return;

                if (roadType[row] == BoxType.Water)
                    continue;

                float x = 9.9f;

                if (zombie.isMindControlled)
                {
                    x = PlantMgr.getX(-2);
                }

                InstanceManager.CreateZombie.SetZombie(
                    row,
                    Plugin.DataContainer.ZombieId_C,
                    x,
                    zombie.isMindControlled
                );
            }
        }
    }
    public class MyPluginInfo
    {
        public const string PluginGuid = "MoreBlackHorse.Bepinex";
        public const string PluginName = "MoreBlackHorse";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
