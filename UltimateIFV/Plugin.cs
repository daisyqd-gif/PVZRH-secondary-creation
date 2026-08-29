global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System;
global using System.Reflection;
global using UnityEngine;
global using System.Collections;
global using System.Collections.Generic;
global using CustomPlantClass;
global using CustomPlantClass.Main;
global using Random = UnityEngine.Random;
global using UnityEngine.Rendering;
using Core;
using System.Linq;
namespace UltimateIFV
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ID PlantId_ironPuff = -1;
            public static ID PlantId_blover = -1;
            public static CustomItemType wingmanID = -1;
            public static CustomItemType targetID = -1;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "ultimateifv"
            );
            DataContainer.PlantId_ironPuff = DataMgr.AllocateID();
            DataContainer.PlantId_blover = DataMgr.AllocateID();
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = DataContainer.PlantId_ironPuff, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("UltimateIFVIronPuffPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                Preview = assetBundle.GetAsset<GameObject>("UltimateIFVIronPuffPreview"), // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */

                Fusions = DataMgr.MirrorList(new List<(ID, ID)>(
                    [(PlantType.IFVIronPuff, PlantType.Peashooter), (DataContainer.PlantId_blover, PlantType.Magnetshroom)]
                )), // Optional fusion recipes

                AttackInterval = 1f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 80,      // Damage per attack
                MaxHealth = 16000,       // Plant HP
                Cd = 50f,               // Card cooldown
                Sun = 525,               // Sun cost

                DefaultBullet = BulletType.Bullet_puffIronPea, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = true,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
                CanStarUp = false, // Enable Star-Up ability if the plant has one, retrieve using _plant.starUp

                CardColor = CardLevel.Gold, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong / Final ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = true, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "究极铁豆小队",           // Plant name (shown in UI)
                AlmanacEntry = DataMgr.CreateAlmanacEntry
                (
                    "多功能铁豆小喷菇部队，狙击防空，各显神通。",
                    "毁铁豆小队进阶形态",
                    recipe: ("铁豆小喷菇×3（底座）", "磁力菇+豌豆射手"),
                    attackinterval: (80, 4),
                    specialeffects:
                    [
                        "机枪：发射小铁豆，造成击退并附加目标二类防具血量的伤害，每发子弹有2%概率触发大招：回复1倍韧性血量，5秒内免疫伤害和碾压，每0.02秒散射3发小铁豆",
                        "防空炮：本行索敌只对空，造成小幅击退和半径1格的爆炸伤害",
                        "狙击：索敌前方全场，可对空。每第6次攻击造成爆头（100万伤害）"
                    ],
                    usageconditions: "集齐基础形态双词条",
                    flavor: "背景故事铁豆小喷菇一直担心的事终于发生了：他们真的被磁力菇吸住了，而且没有人能把他们扒开。也许是因祸得福，磁力菇积攒的材料帮他们更新了装备，现在他们是真正的“钢铁之师”。"
                )    // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };

            DataMgr.RegisterCustomPlant<IFVIronPuff, UltimateIFVIronPuff>(Data);
            // Fill out the plant metadata
            BaseCustomPlantData Data2 = new BaseCustomPlantData()
            {
                PlantId = DataContainer.PlantId_blover, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("UltimateIFVBloverPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                Preview = assetBundle.GetAsset<GameObject>("UltimateIFVBloverPreview"), // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */

                Fusions = DataMgr.MirrorList(new List<(ID, ID)>(
                    [(PlantType.IFVBlover, PlantType.Peashooter), (DataContainer.PlantId_ironPuff, PlantType.Blover)]
                )), // Optional fusion recipes

                AttackInterval = 0.166f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 80,      // Damage per attack
                MaxHealth = 16000,       // Plant HP
                Cd = 50f,               // Card cooldown
                Sun = 575,               // Sun cost

                DefaultBullet = BulletType.Bullet_puffIronPea, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = true,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
                CanStarUp = false, // Enable Star-Up ability if the plant has one, retrieve using _plant.starUp

                CardColor = CardLevel.Gold, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong / Final ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = true, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "究极铁豆突击队",           // Plant name (shown in UI)
                AlmanacEntry = DataMgr.CreateAlmanacEntry
                (
                    "高机动铁豆小喷菇部队，听令而行，全场出击。",
                    "毁铁豆小队亚种进阶形态",
                    attackinterval: (80, 3),
                    specialeffects:
                    [
                        "手动点击本体后，可在全场任选一格，将僚机（铁豆突击队）部署在该格左侧。选定时如果按住ctrl键，则会同时部署场上所有僚机",
                        "所选格子或其左右两格有僵尸时，僚机每1/6秒发射一次小铁豆，造成击退并附加目标二类防具血量的伤害"
                    ],
                    variantswitch: ("三叶草", "磁力菇"),
                    usageconditions: "集齐基础形态双词条",
                    flavor: "在安装了最新的磁悬浮装置后，铁豆小队终于真正的变成了一支可以指哪打哪的特种小队了，但由于新技术的缺陷，它们无法再使用重火力武器，只能返璞归真的去使用机枪，但这也无法改变它们想要来去自如的打击僵尸的决心和信念。"

                )    // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };
            DataMgr.RegisterCustomPlant<IFVBlover, UltimateIFVBlover>(Data2);

            var wingman = assetBundle.GetAsset<GameObject>("UltimateIFVWingman");
            wingman.AddComponent<IFVWingman>();
            DataContainer.wingmanID = GameObjectMgr.Register<UltimateIFVWingman>(wingman);

            DataContainer.targetID = GameObjectMgr.Register(assetBundle.GetAsset<GameObject>("target"));

            foreach (var i in Enum.GetValues<BucketType>())
            {
                CustomCore.RegisterCustomUseItemOnPlantEvent(DataContainer.PlantId_blover, i, (Plant p) =>
                {
                    p.Recover(500);
                });
                CustomCore.RegisterCustomUseItemOnPlantEvent(DataContainer.PlantId_ironPuff, i, (Plant p) =>
                {
                    p.Recover(500);
                });
            }

            CustomCore.RegisterCustomBanMix(DataContainer.PlantId_blover, () => (Lawnf.TravelAdvanced(AdvBuff.EnumValue24) && Lawnf.TravelAdvanced(AdvBuff.EnumValue24)) ||
                Board.Instance.boardTag.enableAllTravelPlant || Board.Instance.boardTag.isSuperRandom || Board.Instance.boardTag.isUltimateSuperRandom || GameAPP.developerMode,
                    null, () => InGameText.Instance.ShowText("该配方需要抽取", 3f));
            CustomCore.RegisterCustomBanMix(DataContainer.PlantId_ironPuff, () => (Lawnf.TravelAdvanced(AdvBuff.EnumValue24) && Lawnf.TravelAdvanced(AdvBuff.EnumValue24)) ||
                Board.Instance.boardTag.enableAllTravelPlant || Board.Instance.boardTag.isSuperRandom || Board.Instance.boardTag.isUltimateSuperRandom || GameAPP.developerMode,
                    null, () => InGameText.Instance.ShowText("该配方需要抽取", 3f));

            CustomCore.CustomPlantClicks.Add(DataContainer.PlantId_blover, (Plant p) =>
            {
                if (p.TryGetComponent<UltimateIFVBlover>(out var ulti))
                {
                    ulti.OnClick();
                }
            });
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
        public override void OnGameStart()
        {
            GameAPP.Instance.StartCoroutine(latestart());
            IEnumerator latestart()
            {
                yield return new WaitForFixedUpdate();
                TypeData.WallNutPlants.Add(DataContainer.PlantId_blover);
                TypeData.WallNutPlants.Add(DataContainer.PlantId_ironPuff);
                TypeData.MagnetPlants.Add(DataContainer.PlantId_blover);
                TypeData.MagnetPlants.Add(DataContainer.PlantId_ironPuff);
                TypeMgr.UncrashablePlants.Add(DataContainer.PlantId_blover);
                TypeMgr.UncrashablePlants.Add(DataContainer.PlantId_ironPuff);
            }
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class UltimateIFVIronPuff : BaseCustomPlant
    {
        public IFVIronPuff plant => GetComponent<IFVIronPuff>();
        public float HealCD = 5f;
        public override void OnFixedUpdate()
        {
            if ( plant == null || plant.board == null ) return;
            HealCD -= Time.deltaTime;
            if (HealCD <= 0f)
            {
                HealCD = 5f;
                plant.Recover(500);
            }
        }
        public void CustomAwake()
        {
            if ( plant == null ) return;
            plant.shoot_sniper = transform.FindChild("Puff2/gun_lower /Shoot");
            plant.shoot_pult = transform.FindChild("Puff3/Pult/Shoot_1");
            plant.heartTransform = transform.FindChild("heart");
            plant.heart = plant.heartTransform.GetComponent<SortingGroup>();
            plant.pult = transform.FindChild("Puff3/Pult");
        }
        public override Transform FindShoot() => transform.FindChild("Puff1/outmouth");
        public override BulletType GetBulletType()
        {
            return BulletType.Bullet_puffIronPea;
        }
        public override int GetDamage()
        {
            if (Lawnf.TravelUltimate(UltiBuff.EnumValue51)) return _plant.attackDamage * 2;
            return _plant.attackDamage;
        }
        public override Bullet Shoot_Custom()
        {
            if (PlantMgr.GetPercent(2f) || (Lawnf.TravelUltimate(UltiBuff.EnumValue50) && PlantMgr.GetPercent(6f)))
            {
                StartPF();
            }
            return PlantMgr.SetBullet(_plant, GetBulletType(), BulletMoveWay.MoveRight);
        }
        protected override int DamageLimit => 500;
        protected override bool OverrideDamagePipeline => true;
        public override IEnumerator SuperShoot()
        {
            _plant.anim.SetBoolString("shooting", true);
            for (int i = 0; i < 250; i++)
            {
                for (int j = 0; j < 5; j++)
                    PlantMgr.SetBullet
                    (
                        _plant,
                        GetBulletType(),
                        GetBulletMoveWayPF_SuperGatling(),
                        GetDamage(),
                        new Vector2(0, Random.Range(-0.15f, 0.15f)), Random.Range(-15f, 15f)
                    ).normalSpeed = Random.Range(12f, 14f);
                _plant.thePlantAttackCountDown = 10f;
                yield return new WaitForFixedUpdate();
            }
            _plant.thePlantAttackCountDown = 0.05f;
            _plant.anim.SetBoolString("shooting", false);
        }
        public override void SuperEnd()
        {
            if (_plant.starUp) StartPF();
            else base.SuperEnd();
        }
        public override Bullet Shoot2_Custom()
        {
            return plant.Shoot2();
        }
        public void AnimShoot3_Custom()
        {
            plant.AnimShoot3();
        }
    }
    public class UltimateIFVWingman : MonoBehaviour
    {
        public IFVWingman plant => GetComponent<IFVWingman>();
        public void Awake_Prefix()
        {
            //plant.anim=GetComponent<Animator>();
            //plant.sortingGroup=GetComponent<SortingGroup>();
            plant.shoots = new();
            plant.shoots.Add(transform.GetChild(0).FindChild("Shoot"));
            plant.shoots.Add(transform.GetChild(1).FindChild("Shoot"));
            plant.shoots.Add(transform.GetChild(2).FindChild("Shoot"));
            plant.shadow = transform.FindChild("Shadow");
        }
        public void SetBullet(Vector2 pos)
        {
            var bullet = CreateBullet.Instance.SetBullet(
                pos.x, pos.y,
                plant.targetGrid.m_Y,
                BulletType.Bullet_ironPea_air,
                BulletMoveWay.Free);

            bullet.Damage = plant.from.attackDamage;
            bullet.from = plant.from;

            // 1. Get direction
            var dir = plant.shootRoad;

            // 2. Normalize
            var len = Math.Sqrt(dir.x * dir.x + dir.y * dir.y);

            if (len < 1e-5)
            {
                dir = Vector2.zero;
            }
            else
            {
                dir.x /= (float)len;
                dir.y /= (float)len;
            }

            // 3. Rotation from normalized direction
            var rot = MathHelper.DirectionToRotation(dir);
            bullet.transform.rotation = rot;

            // 4. Velocity
            var v=bullet.velocity;
            v.x = dir.x * 6f;
            v.y = dir.y * 6f;
            bullet.velocity = v;

            // 5. Buffs
            if (Lawnf.TravelAdvanced(AdvBuff.EnumValue24))
                bullet.Damage *= 3;

            if (Lawnf.TravelAdvanced(AdvBuff.EnumValue25))
                bullet.Damage *= 3;
        }
    }
    [HarmonyPatch(typeof(IFVWingman))]
    public static class IFVWingman_Patch
    {
        [HarmonyPatch(nameof(IFVWingman.Awake))]
        [HarmonyPrefix]
        public static void Awake_Prefix(IFVWingman __instance)
        {
            if (__instance.TryGetComponent<UltimateIFVWingman>(out var a))
            {
                a.Awake_Prefix();
            }
        }
        [HarmonyPatch(nameof(IFVWingman.AnimShoot))]
        [HarmonyPrefix]
        public static void AnimShoot_Prefix(IFVWingman __instance)
        {
            if (__instance.TryGetComponent<UltimateIFVWingman>(out var a))
            {
                foreach (var shoot in __instance.shoots)
                {
                    a.SetBullet(shoot.position);
                }
            }
        }
    }
    public class UltimateIFVBlover : BaseCustomPlant
    {
        public IFVBlover plant => GetComponent<IFVBlover>();
        public float HealCD = 5f;
        public override void OnFixedUpdate()
        {
            if ( plant == null || plant.board == null ) return;
            HealCD -= Time.deltaTime;
            if (HealCD <= 0f)
            {
                HealCD = 5f;
                plant.Recover(500);
            }
        }
        public void Awake()
        {
            if ( plant == null ) return;
            plant.shoot = transform.GetChild(2);
        }
        public void SetWingman_Custom()
        {
            // Destroy old wingman if it exists
            if (plant.wingman != null)
            {
                Destroy(plant.wingman.gameObject);
            }

            // Get shoot transform (same as original)
            var shoot = plant.shoot;
            if (shoot == null)
                return;

            // Position offset used by original IL2CPP code
            Vector3 pos = shoot.position;
            pos.y += 0.5f;

            // Parent is board.transform
            Transform parent = plant.board.transform;

            // Instantiate your registered prefab
            GameObject wingmanGO = GameObjectMgr.Instantiate(
                Plugin.DataContainer.wingmanID,
                pos,
                Quaternion.identity,
                parent
            );

            // Get managed IFVWingman (already added during registration)
            var wingman = wingmanGO.GetComponent<IFVWingman>();
            plant.wingman = wingman;

            // Wire fields exactly like IL2CPP SetWingman
            wingman.from = plant;
            wingman.board = plant.board;

            // Set target grid
            var target = new Vector2Int(
                plant.thePlantColumn,
                plant.thePlantRow
            );
            wingman.SetTarget(target);

            // fromType
            wingman.fromType = plant.thePlantType;

            // Play sound (same as IL2CPP)
            GameAPP.PlaySound(0x53, 0.5f, 1f);

            // Particle effect (same as IL2CPP)
            var pm = ParticleManager.Instance;
            if (pm != null)
            {
                Vector2 p = wingman.transform.position;
                pm.SetParticle((ParticleType)0x7e, p, target.y, true, 0f);
            }
        }
        protected override int DamageLimit => 500;
        protected override bool OverrideDamagePipeline => true;
        public static void MoveWingman(Mouse mouse, Plant selectedPlant, bool IsCtrl)
        {
            if (IsCtrl)
            {
                var plants = Lawnf.GetAllPlants().ToSystemList().Where((Plant p) => p != null && p.TryGetComponent<IFVBlover>(out var _));
                if (plants.Any())
                {
                    foreach (var p in plants)
                    {
                        if (p is IFVBlover)
                        {
                            var ifv = p as IFVBlover;
                            var wingman = ifv.wingman;
                            Vector2Int pos = new(mouse.theMouseColumn, mouse.theMouseRow);
                            wingman.SetTarget(pos);
                        }
                    }
                }
            }
            else
            {
                if (selectedPlant is IFVBlover)
                {
                    var ifv = selectedPlant as IFVBlover;
                    var wingman = ifv.wingman;
                    Vector2Int pos = new(mouse.theMouseColumn, mouse.theMouseRow);
                    wingman.SetTarget(pos);
                }
            }
        }
        public void OnClick()
        {
            Mouse mouse = Mouse.Instance;
            mouse.cannonPlant = plant;
            mouse.theItemOnMouse = GameObjectMgr.Instantiate(Plugin.DataContainer.targetID, mouse.MousePosition, Quaternion.identity, plant.board.transform);
            mouse.theItemOnMouse.name = "UltiIFVBlover_target";
        }
    }
    [HarmonyPatch(typeof(IFVBlover), nameof(IFVBlover.SetWingman))]
    public static class IFVBlover_SetWingman_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(IFVBlover __instance)
        {
            if (__instance.TryGetComponent<UltimateIFVBlover>(out var self))
            {
                self.SetWingman_Custom();
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(IFVIronPuff), nameof(IFVIronPuff.Awake))]
    public static class IFVIronPuff_Awake_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(IFVIronPuff __instance)
        {
            if (__instance.TryGetComponent<UltimateIFVIronPuff>(out var self))
            {
                self.CustomAwake();
            }
        }
    }

    [HarmonyPatch(typeof(Mouse))]
    public static class MousePatch
    {
        [HarmonyPatch(nameof(Mouse.LeftClickWithSomeThing))]
        [HarmonyPostfix]
        public static void PostLeftClickWithSomeThing(Mouse __instance)
        {
            if (__instance.theItemOnMouse != null && __instance.cannonPlant != null && __instance.cannonPlant.TryGetComponent<IFVBlover>(out var _) &&
            __instance.theItemOnMouse.name == "UltiIFVBlover_target")
            {
                UltimateIFVBlover.MoveWingman(__instance, __instance.cannonPlant, Input.GetKey(KeyCode.LeftControl));
                //__instance.cannonPlant.GetComponent<UltimatePortalSpring>().SetShootTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                __instance.ClearItemOnMouse(true);
            }
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateIFV.Bepinex";
        public const string PluginName = "UltimateIFV";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
