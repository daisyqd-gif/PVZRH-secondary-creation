global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass.Main;
namespace MoreDolphinZombie
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ZombieType ZombieId_A=ZombieType.Nothing;
            public static ZombieType ZombieId_B=ZombieType.Nothing;
            public static ZombieType ZombieId_C=ZombieType.Nothing;
            public static ZombieType ZombieId_D=ZombieType.Nothing;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "moredolphinzombie"
            );
            DataContainer.ZombieId_A=DataMgr.AllocateID();
            DataContainer.ZombieId_B=DataMgr.AllocateID();
            DataContainer.ZombieId_C=DataMgr.AllocateID();
            DataContainer.ZombieId_D=DataMgr.AllocateID();
        }

        [System.Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override void InitializeZombies()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            CustomCore.RegisterCustomZombie<DolphinGatlingZombie,DolphinGatlingZombie_a>
            (
                DataContainer.ZombieId_A,assetBundle.GetAsset<GameObject>("DolphinGatling_a"),
                assetBundle.GetAsset<Sprite>("DolphinGatling_a_0"),50,
                DefaultZombieStats.UltiZombieA_Health_Light,0,0
            );
            CustomCore.TypeMgrExtra.WaterZombie.Add(DataContainer.ZombieId_A);
            DataMgr.AddCustomZombieSpawnRatio(DataContainer.ZombieId_A,4,1000);
            CustomCore.RegisterCustomZombie<DolphinGatlingZombie,DolphinGatlingZombie_a> //their mechanics are almost identical
            (
                DataContainer.ZombieId_B,assetBundle.GetAsset<GameObject>("DolphinGatling_b"),
                assetBundle.GetAsset<Sprite>("DolphinGatling_b_0"),100,
                DefaultZombieStats.UltiZombieB_Health_Light,0,0
            );
            CustomCore.TypeMgrExtra.WaterZombie.Add(DataContainer.ZombieId_B);
            DataMgr.AddCustomZombieSpawnRatio(DataContainer.ZombieId_B,5,600);
            CustomCore.RegisterCustomZombie<DolphinGatlingZombie,DolphinGatlingZombie_c> //their mechanics are almost identical
            (
                DataContainer.ZombieId_C,assetBundle.GetAsset<GameObject>("DolphinGatling_c"),
                assetBundle.GetAsset<Sprite>("DolphinGatling_c_0"),200,
                DefaultZombieStats.UltiZombieC_Health_Light,0,0
            );
            CustomCore.TypeMgrExtra.WaterZombie.Add(DataContainer.ZombieId_C);
            DataMgr.AddCustomZombieSpawnRatio(DataContainer.ZombieId_C,6,300);
            CustomCore.RegisterCustomZombie<DolphinGatlingZombie,DolphinGatlingZombie_d> //their mechanics are almost identical
            (
                DataContainer.ZombieId_D,assetBundle.GetAsset<GameObject>("DolphinGatling_d"),
                assetBundle.GetAsset<Sprite>("DolphinGatling_d_0"),400,
                DefaultZombieStats.UltiZombieD_Health_Light,0,0
            );
            CustomCore.TypeMgrExtra.WaterZombie.Add(DataContainer.ZombieId_D);
            DataMgr.AddCustomLevel4Zombie((ID)DataContainer.ZombieId_D,DataContainer.ZombieId_C);
            DataMgr.AddLevelZombie(DataContainer.ZombieId_A,DataContainer.ZombieId_B,DataContainer.ZombieId_C);
            ModRegistryManager.AddToRegistry("NeutralZombies",DataContainer.ZombieId_A);
            ModRegistryManager.AddToRegistry("NeutralZombies",DataContainer.ZombieId_B);
            ModRegistryManager.AddToRegistry("NeutralZombies",DataContainer.ZombieId_C);
            ModRegistryManager.AddToRegistry("NeutralZombies",DataContainer.ZombieId_D);

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
        public override void InitializeConditions()
        {
            TypeMgr.UltieZombie_level_water.Add(DataContainer.ZombieId_A);
            TypeMgr.UltieZombie_level_water.Add(DataContainer.ZombieId_B);
            TypeMgr.UltieZombie_level_water.Add(DataContainer.ZombieId_C);
            TypeMgr.UltieZombie_level_water.Add(DataContainer.ZombieId_D);
        }
    }
    public class DolphinGatlingZombie_a : MonoBehaviour
    {
        public DolphinGatlingZombie zombie => GetComponent<DolphinGatlingZombie>();
        public void Awake()
        {
            zombie.shoot=transform.FindChild("Shoot");
            zombie.head1=transform.FindChild("Zombie_dolphinrider_head/Pos").gameObject;
            zombie.head2=transform.FindChild("Zombie_dolphinrider_head/head2").gameObject;
            //IDK why the devs named it this way, I just used the prefab from assetripper with only minor changes
        }
    }
    public class DolphinGatlingZombie_c : DolphinGatlingZombie_a
    {
        private Transform shoot2;
        
        public void CreateWaterSplash()
        {
            zombie.anim.SetBoolString("isBack",true);
        }
        public void AnimShootGun()
        {
            // If zombie is dead, dying, changing row, or in a disabled status → do nothing
            if (zombie.theStatus == ZombieStatus.Dying ||
                zombie.beforeDying ||
                zombie.isChangingRow ||
                zombie.theStatus == ZombieStatus.Dolphinrider_jump)
                return;
            if(shoot2==null) shoot2=transform.FindChild("Zombie_dolphinrider_body1/gun/Shoot2");
            // Find the muzzle transform (child named "shoot")
            Transform muzzle = shoot2;
            if (muzzle == null)
                return;

            Vector3 pos = muzzle.position;

            // Determine bullet type
            BulletType bulletType = BulletType.Bullet_superCherry;

            // Determine moving direction (0 = right, 9 = left)
            BulletMoveWay movingWay = (zombie.towards == Towards.Left) ? BulletMoveWay.Left : BulletMoveWay.MoveRight;

            // Spawn bullet
            Bullet bullet = CreateBullet.Instance.SetBullet(
                pos.x,
                pos.y,
                zombie.theZombieRow,
                bulletType,
                movingWay,
                !zombie.isMindControlled
            );

            if (bullet == null)
                return;

            // Set bullet damage
            bullet.Damage=GameStrategy.ZombieSuperCherryDamage;

            // Play random shoot sound (3–5)
            int soundId = UnityEngine.Random.Range(3, 5);
            GameAPP.PlaySound(soundId, 0.5f, 1.0f);
        }
    }
    public class DolphinGatlingZombie_d : DolphinGatlingZombie_c
    {
        public void Start()
        {
            zombie.anim.SetBoolString("shooting",zombie.enabled && Board.Instance!=null && zombie.healthText!=null);
            zombie.AttributeEvent();
        }
        public void AnimShoot_Custom()
        {
            // If zombie is dead, dying, changing row, or in a disabled status → do nothing
            if (zombie.theStatus == ZombieStatus.Dying ||
                zombie.beforeDying ||
                zombie.isChangingRow ||
                zombie.theStatus == ZombieStatus.Dolphinrider_jump)
                return;
            
            // Find the muzzle transform (child named "shoot")
            Transform muzzle = zombie.shoot;
            if (muzzle == null)
                return;

            Vector3 pos = muzzle.position;

            // Determine bullet type
            BulletType bulletType = BulletType.Bullet_superCherry;

            // Determine moving direction (0 = right, 9 = left)
            BulletMoveWay movingWay = (zombie.towards == Towards.Left) ? BulletMoveWay.Left : BulletMoveWay.MoveRight;

            // Spawn bullet
            Bullet bullet = CreateBullet.Instance.SetBullet(
                pos.x,
                pos.y,
                zombie.theZombieRow,
                bulletType,
                movingWay,
                !zombie.isMindControlled
            );

            if (bullet == null)
                return;

            // Set bullet damage
            bullet.Damage=GameStrategy.ZombieSuperCherryDamage;

            // Play random shoot sound (3–5)
            int soundId = UnityEngine.Random.Range(3, 5);
            GameAPP.PlaySound(soundId, 0.5f, 1.0f);
        }
    }
    [HarmonyPatch(typeof(Lawnf))]
    public static class Lawnf_Patch
    {
        [HarmonyPatch(nameof(Lawnf.GetCertainZombies))]
        [HarmonyPostfix]
        public static void GetCertainZombies_PostFix(Il2CppSystem.Collections.Generic.List<Zombie> __result, Board board, ZombieType zombieType)
        {
            if(zombieType==ZombieType.UltimateDolphin)
                __result.Merge(Lawnf.GetCertainZombies(board,Plugin.DataContainer.ZombieId_D));
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "MoreDolphinZombie.Bepinex";
        public const string PluginName = "MoreDolphinZombie";
        public const string PluginVersion = "3.7";
    }
}
