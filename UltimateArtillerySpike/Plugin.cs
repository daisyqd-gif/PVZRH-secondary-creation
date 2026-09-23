global using BepInEx;
global using CustomizeLib.BepInEx;
global using HarmonyLib;
global using System;
global using System.Reflection;
global using UnityEngine;
global using System.Collections.Generic;
global using CustomPlantClass;
global using CustomPlantClass.Main;
global using System.Linq;
global using Random = UnityEngine.Random;
global using CustomPlantClass.Runtime;
global using CustomPlantClass.RogueShootingManager;
global using GameLevel.RogueShooting;
global using CustomPlantClass.Runtime.Tasks;
namespace UltimateArtillerySpike
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        private AssetBundle assetBundle;
        public class DataContainer
        {
            public static ID PlantId_Spike=-1;
            public static ID PlantId_Blover=-1;
            public static ID BulletId=-1;
            public static ID ParticleId=-1;
            public static UltiBuff BuffID_Reduce=(UltiBuff)(-1);
            public static UltiBuff BuffID_Range=(UltiBuff)(-1);
            public static BuffID BuffID_Rogue=-1;
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "ultimateartilleryspike"
            );
            DataContainer.PlantId_Spike=DataMgr.AllocateID();
            DataContainer.PlantId_Blover=DataMgr.AllocateID();
            DataContainer.BulletId=DataMgr.AllocateID();
            DataContainer.ParticleId=DataMgr.AllocateID();
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = DataContainer.PlantId_Spike, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("CabbageCaltropPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                Preview = assetBundle.GetAsset<GameObject>("CabbageCaltropPreview"), // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */

                Fusions = ListHelper.MirrorTuple((PlantType.MelonCaltrop,PlantType.CabbageCaltrop)), // Optional fusion recipes

                AttackInterval = 1f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 1000,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 7.5f,               // Card cooldown
                Sun = 600,               // Sun cost

                DefaultBullet = DataContainer.BulletId, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = false,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
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

                Name = "究极重炮卷心瓜地刺",           // Plant name (shown in UI)
                AlmanacEntry = DataMgr.CreateAlmanacEntry("收集落地子弹，使其再次攻向僵尸。向上弹开接近的僵尸，击飞时令僵尸头晕目眩。",recipe:("西瓜地刺","卷心刺"),attackinterval:(1000,1f),specialeffects: ["本行有其他植物的投掷子弹落地时，每0.7秒限一次，攻击并将一发相同的子弹弹向本行最近的僵尸，无索敌目标时也会原路返还","攻击时，将僵尸原地向上击飞短暂时间，对巨型僵尸效果变为1/3，直接将小鬼僵尸击出场外。无法击飞领袖僵尸和载具"],usageconditions:"旅行模式购买配方")    // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };

            DataMgr.RegisterCustomPlant<MelonCaltrop, UltimateArtillerySpike>(Data);
            // Fill out the plant metadata
            BaseCustomPlantData Data2 = new BaseCustomPlantData()
            {
                PlantId = DataContainer.PlantId_Blover, // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("MelonBloverPrefab"),   // Main plant prefab, must copy an original prefab and delete the script component and then edit
                Preview = assetBundle.GetAsset<GameObject>("MelonBloverPreview"), // Card preview prefab, hirearchy must be this
                /*
                    root-> transform, spriterenderer
                    nothing else
                */

                Fusions = ListHelper.MirrorTuple((DataContainer.PlantId_Spike,PlantType.Blover)), // Optional fusion recipes

                AttackInterval = 1f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 1000,      // Damage per attack
                MaxHealth = 300,       // Plant HP
                Cd = 7.5f,               // Card cooldown
                Sun = 600,               // Sun cost

                DefaultBullet = DataContainer.BulletId, // Shooter bullet type, use GetBulletType to retrieve in the basecustomplant class

                CanPF = false,     // Enable PF ability if the plant has one: override the ienumerator for a pf with damage immunity or override StartPF for a instant pf
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

                Name = "究极卷心瓜浮艇",           // Plant name (shown in UI)
                AlmanacEntry = DataMgr.CreateAlmanacEntry("通过投下西瓜，提供近距离空中支援。","究极重炮卷心瓜地刺同人亚种",variantswitch:("铲除","三叶草"),attackinterval:(1000,1f),specialeffects: ["攻击时，向本格和左右两格各扔出一个爆炸卷心瓜"],usageconditions:"旅行模式购买配方")    // Almanac description, use DataMgr.CreateAlmanacEntry for automatic formatting
            };

            DataMgr.RegisterCustomPlant<CabbageBlover, UltimateMelonCabbageBlover>(Data2);

            DataMgr.RegisterCustomBullet<Bullet_cabbage, Bullet_ultimateMelonCabbage>(new()
            {
                BulletId = DataContainer.BulletId,
                Prefab = assetBundle.GetAsset<GameObject>("Bullet_cabbageMelon")
            });

            CustomCore.RegisterCustomCherry(DataContainer.ParticleId,assetBundle.GetAsset<GameObject>("BombCloud"));

            DataContainer.BuffID_Range = (UltiBuff)Compatibility.CustomCore_Old.RegisterCustomBuff("霰弹枪：究极重炮卷心瓜地刺将分3行而不是1行触发。",BuffType.UltimateBuff,()=> true, 5000,DataContainer.PlantId_Spike);
            DataContainer.BuffID_Reduce = (UltiBuff)Compatibility.CustomCore_Old.RegisterCustomBuff("快速攻击：究极重炮卷心瓜地刺超级技能冷却/2 (2级将/4) 。",BuffType.UltimateBuff,()=> true, 5000,DataContainer.PlantId_Spike,2,default);
            DataMgr.AddCustomStrongUltimatePlant
            (
                DataContainer.PlantId_Spike,
                DataMgr.FormatStrongUltimateUnlockBuff("究极重炮卷心瓜地刺","卷心菜地刺","西瓜地刺","究极卷心瓜浮艇","铲除","三叶草"),
                DataContainer.BuffID_Range,DataContainer.BuffID_Reduce, default, DataContainer.PlantId_Blover
            );

            CustomCore.TypeMgrExtra.IsSpickRock.Add(DataContainer.PlantId_Spike);
            CustomCore.TypeMgrExtra.FlyingPlants.Add(DataContainer.PlantId_Blover);
            
            ( DataContainer.BuffID_Rogue, BaseBuff buffCfg ) = RegistryHelper.RegisterCustomQualitativeChangeBuff("弹弹乐！","究极重炮卷心瓜地刺每击中半径为 3 范围内的僵尸，就会收集 1 发子弹。",DataContainer.PlantId_Spike);
            BaseBuff zidanBuff = RegistryHelper.MakeBuffType(new CustomRogueShootingBuff()
            {
                CustomPlantType = DataContainer.PlantId_Spike,
                CustomTitle = "强化：分裂",
                CustomDescription = "子弹数 + 1",
                CustomBuffType = ShootingBuffType.UniqueUpgrade,
                CustomOnGet = () =>
                {
                    if(ShootingManager.Instance.TryGetPlant(DataContainer.PlantId_Spike,out Plant plant))
                    {
                        plant.shootingLevel ++;
                    }
                }
            });
            BaseConfig cfg = RegistryHelper.MakeConfigType(new CustomRogueShootingConfig()
            {
                CustomPlantType = DataContainer.PlantId_Spike,
                CustomBuffs = () => new() { new DamageBuff(DataContainer.PlantId_Spike), new SpeedBuff(DataContainer.PlantId_Spike), buffCfg, zidanBuff },
                CustomReinforcePlant = (Plant p) => TravelMgr.Instance.GetUltiBuff(DataContainer.BuffID_Range),
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            BaseConfig cfg_inter = RegistryHelper.MakeConfigType(new CustomRogueShootingConfig()
            {
                CustomPlantType = PlantType.MelonCaltrop,
                CustomBuffs = () => new() { new UpgradeBuff(PlantType.MelonCaltrop, DataContainer.PlantId_Spike) },
                CustomReinforcePlant = (Plant p) => { },
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Attacker)
            });
            RegistryHelper.AddCustomRogueShootingPlant(DataContainer.PlantId_Spike,cfg);
            RegistryHelper.AddCustomRogueShootingPlant(PlantType.MelonCaltrop,cfg_inter);
            RegistryHelper.InjectUpgradeBuff(RSConfigType.Caltrop,PlantType.MelonCaltrop);
            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }
    public class Bullet_ultimateMelonCabbage : BaseCustomBullet
    {
        public override bool HitLand()
        {
            _bullet.board.boardAction.CreateCherryExplode(_bullet.col.bounds.center,_bullet.theBulletRow,Plugin.DataContainer.ParticleId,_bullet.Damage,_bullet.fromType);
            return true;
        }
        public override bool HitZombie(Zombie zombie)
        {
            _bullet.board.boardAction.CreateCherryExplode(_bullet.col.bounds.center,_bullet.theBulletRow,Plugin.DataContainer.ParticleId,_bullet.Damage,_bullet.fromType);
            return true;
        }
    }
    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class UltimateArtillerySpike : BaseCustomPlant
    {
        public MelonCaltrop plant => GetComponent<MelonCaltrop>();
        Queue<(BulletType theBulletType, int theDamage)> StoredBullets = new();
        int AccumulatedEnergy = 0;
        internal static int MaxEnergy()
        {
            if(Lawnf.TravelUltimateLevel(Plugin.DataContainer.BuffID_Reduce) > 1) return 12;
            return Lawnf.TravelUltimate(Plugin.DataContainer.BuffID_Reduce) ? 25 : 50;
        }
        bool isSuper = false;
        public override Transform FindShoot() => transform.FindChild("Shadow");
        public override void OnSpawn()
        {
            plant.isShort = true;
            UpdateLoop();
        }
        public async void UpdateLoop()
        {
            try
            {
                while (!token.IsCanceled)
                {
                    try
                    {
                        try
                        {
                            if( _plant == null ) return;
                        }
                        catch
                        {
                            return;
                        }
                        await DelayTask.DelayScaled(1f,()=>_plant.attributeSpeed,token);
                        for( int i = 0; i < _plant.shootingLevel; i++)
                        {
                            if( _plant == null ) return;
                            AccumulatedEnergy ++;
                            if(AccumulatedEnergy > MaxEnergy())
                            {
                                isSuper = true;
                                AccumulatedEnergy = 0;
                            }
                            StoredBullets.Enqueue((Plugin.DataContainer.BulletId,_plant.attackDamage));
                            _plant.anim.SetTriggerString("supply");
                        }
                    }
                    catch( Exception e )
                    {
                        try
                        {
                            if(e.Message.Contains(":line 235"))
                            {
                                return;
                            }
                            ModLogger.LogError(e.Message);
                        }
                        catch ( Exception )
                        {
                            
                        }
                    }
                }
            }
            catch( Exception e )
            {
                try
                {
                    ModLogger.LogError(e.Message);
                }
                catch ( Exception )
                {
                    
                }
            }
        }
        public void OnBulletHitLand(Bullet bullet)
        {
            if(_plant.theStatus != PlantStatus.Default || _plant.board.boardTag.rogueShooting) return;
            if(!(
                bullet.theBulletRow == _plant.thePlantRow
                || (bullet.theBulletRow == _plant.thePlantRow-1 || bullet.theBulletRow == _plant.thePlantRow+1)
                && Lawnf.TravelUltimate(Plugin.DataContainer.BuffID_Range)
            )) return;
            if( bullet.fromType == _plant.thePlantType ) return;
            AccumulatedEnergy ++;
            if(AccumulatedEnergy > MaxEnergy())
            {
                isSuper = true;
                AccumulatedEnergy = 0;
            }
            StoredBullets.Enqueue((bullet.theBulletType,bullet.Damage));
            _plant.anim.SetTriggerString("supply");
        }
        public void AnimSupply()
        {
            var pos = _plant.axis.position;
            if (isSuper)
            {
                isSuper = false;
                var zombies = Physics2D.OverlapCircleAll(_plant.shoot.position,CoreTools.ColumnX * (Lawnf.TravelUltimate(Plugin.DataContainer.BuffID_Range) ? 3 : 1),LayerMask.GetMask("Zombie"))
                .Where(col => col.IsObjExist() && col.TryGetComponent<Zombie>(out var zombie) && zombie.IsObjExist()). // 找到所有存在zombie组件的碰撞体
                    Select(zombie => zombie.GetComponent<Zombie>()).ToList();
                if (zombies.Count <= 0)
                    goto ShootBullets;
                foreach( var z in zombies)
                {
                    plant.OnAttack(z);
                }

                ShootBullets:
                StoredBullets.ActionPerItem((b)=>ShootOnce(b.theBulletType,b.theDamage,GetRandomPosition()));
                StoredBullets.Clear();
                return;
            }
            else
            {
                var a = (Zombie z) => Lawnf.InLandStatus(z.theStatus) && Math.Abs(z.theZombieRow - _plant.thePlantRow) <= (Lawnf.TravelUltimate(Plugin.DataContainer.BuffID_Range) ? 3 : 1) && !z.isMindControlled;
                if(!PlantMgr.IsNotNullMonoBehaviour(Lawnf.GetNearestZombie(_plant.board,plant.shoot.position, a), out var z)) return;
                if(!StoredBullets.TryDequeue(out var data))
                {
                    data=(Plugin.DataContainer.BulletId,_plant.attackDamage);
                }
                ShootOnce(data.theBulletType,data.theDamage,z);
            }
        }
        public void AnimAttack()
        {
            if (Lawnf.TravelUltimate(Plugin.DataContainer.BuffID_Range))
            {
                var zombies = Physics2D.OverlapCircleAll(_plant.shoot.position,CoreTools.ColumnX * (Lawnf.TravelUltimate(Plugin.DataContainer.BuffID_Range) ? 3 : 1),LayerMask.GetMask("Zombie"))
                .Where(col => col.IsObjExist() && col.TryGetComponent<Zombie>(out var zombie) && zombie.IsObjExist()). // 找到所有存在zombie组件的碰撞体
                    Select(zombie => zombie.GetComponent<Zombie>()).ToList();
                foreach( var z in zombies)
                {
                    AccumulatedEnergy ++;
                    if(AccumulatedEnergy > MaxEnergy())
                    {
                        isSuper = true;
                        AccumulatedEnergy = 0;
                    }
                    StoredBullets.Enqueue((Plugin.DataContainer.BulletId,_plant.attackDamage));
                    _plant.anim.SetTriggerString("supply");
                }
            }
        }
        Vector2 GetRandomPosition()
        {
            int row = _plant.thePlantRow;

            // Pick -1, 0, +1
            // IL2CPP RandomRangeInt uses max-exclusive → (-1, 2) gives -1, 0, 1
            int offset = Random.RandomRangeInt(-1, 2);

            // Clamp to board edges
            int targetRow = Mathf.Clamp(row + offset, 0, Board.Instance.rowNum - 1);

            // Horizontal jitter around the plant
            float x = _plant.axis.position.x + Random.Range(-5f, 5f);

            // Vertical lane center
            float y = Mouse.Instance.GetBoxYFromRow(targetRow);

            return new Vector2(x, y);
        }
        void ShootOnce(BulletType theBulletType, int damage, Zombie z)
        {
            var bullet = PlantMgr.SetBullet(_plant,z.theZombieRow,theBulletType,BulletMoveWay.Throw,damage);
            bullet.ThrowTo(z,new(),1.5f.GetNullable());
        }
        void ShootOnce(BulletType theBulletType, int damage, Vector2 pos)
        {
            var bullet = PlantMgr.SetBullet(_plant,Mouse.Instance.GetRowFromY(pos.x,pos.y),theBulletType,BulletMoveWay.Throw,damage);
            bullet.ThrowTo(pos,new(),1.5f.GetNullable());
        }
        public override string GetTextString()
        {
            return $"{AccumulatedEnergy} / {MaxEnergy()}";
        }
    }
    public class UltimateMelonCabbageBlover : MonoBehaviour, IRedirectAnimShoot, IPlantDieHandler
    {
        public CabbageBlover plant => GetComponent<CabbageBlover>();
        HashSet<Plant> shoots = new();
        [ResetOnBoardDestroy(0)]
        static int energy = 0;
        public void Awake()
        {
            plant.shoot = transform.FindChild("Body/Melonpult_body "); //there is a space in there in the editor too
            var action = OnThrow;
            EventManager.AddListener_obj(GameEvent.OnPlantShoot,action);
        }
        public void OnThrow(Il2CppSystem.Object obj)
        {
            if(obj.TryCast<Thrower>().IsNotNull())
            {
                energy++;
            }
        }
        void ShootOnce(Plant self, Zombie z)
        {
            var bullet = PlantMgr.SetBullet(self,z.theZombieRow,Plugin.DataContainer.BulletId,BulletMoveWay.Throw,plant.attackDamage);
            bullet.ThrowTo(z,new(),1.5f.GetNullable());
        }
        void ShootOnce(Plant self, Plant umbrella)
        {
            var bullet = PlantMgr.SetBullet(self,self.thePlantRow,Plugin.DataContainer.BulletId,BulletMoveWay.Throw,plant.attackDamage);
            bullet.ThrowTo(umbrella,new(),1.5f.GetNullable());
        }
        public void OnDie(Plant.DieReason reason)
        {
            if( reason == Plant.DieReason.ByShovel )
            {
                Lawnf.SetDroppedCard(plant.shoot.position,Plugin.DataContainer.PlantId_Spike);
            }
        }

        public Bullet Shoot1()
        {
            if(energy >= UltimateArtillerySpike.MaxEnergy())
            {
                foreach( var p in Lawnf.GetAllPlants() )
                {
                    if( p is Thrower t && t.shoot != null && !t.TryGetComponent<UltimateMelonCabbageBlover>(out _))
                    {
                        if( PlantMgr.IsNotNullMonoBehaviour(t.FindUmbrella(t.shoot.position), out var umbrella) )
                        {
                            ShootOnce(t, umbrella);
                            continue;
                        }
                        var z = t.SearchZombie();
                        if( z != null && z.TryGetComponent<Zombie>(out var zombie) )
                        {
                            ShootOnce(t, zombie);
                            continue;
                        }
                    }
                }
                energy=0;
            }
            // Get shoot transform
            Transform shoot = plant.shoot;
            if (shoot == null)
                return null;

            Vector3 pos = shoot.position;

            // Play sound
            int soundID = Random.Range(3, 5);
            GameAPP.PlaySound(soundID, 0.5f, 1f);

            float x = pos.x;
            float y = pos.y + 0.3f;
            int row = plant.thePlantRow;

            // Fire 3 bullets: vx = -2, 0, +2
            for (int vx = -2; vx <= 2; vx += 2)
            {
                Bullet b = CreateBullet.Instance.SetBullet(
                    x, y, row, Plugin.DataContainer.BulletId, BulletMoveWay.Throw, false
                );

                if (b == null)
                    break;

                b.Damage = plant.attackDamage;
                b.fromType = plant.thePlantType;

                // Set velocity and gravity (correct IL2CPP-safe struct assignment)
                b.velocity = new Vector2(vx, 4f);      // upward arc with horizontal spread
                b.acceleration = new Vector2(0f, -15f); // gravity pulling down
            }

            return null;
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateArtillerySpike.Bepinex";
        public const string PluginName = "UltimateArtillerySpike";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
    [HarmonyPatch(typeof(Caltrop))]
    public static class Caltrop_Patch
    {
        [HarmonyPatch(nameof(Caltrop.KillCar))]
        [HarmonyPrefix]
        public static bool KillCar_Prefix(Caltrop __instance) => !__instance.TryGetComponent<UltimateArtillerySpike>(out _);
    }
    [HarmonyPatch(typeof(EventManager))]
    public static class EventManager_Patch
    {
        [HarmonyPatch(nameof(EventManager.TriggerEvent),[typeof(GameEvent),typeof(Il2CppSystem.Object)])]
        [HarmonyPrefix]
        public static void TriggerEvent_Prefix(Il2CppSystem.Object data)
        {
            if(Board.Instance != null && data.TryCast<BulletMovement>() != null)
            {
                Lawnf.GetPlants(Plugin.DataContainer.PlantId_Spike,Board.Instance).ToSystemList().ActionPerItem((Plant p) =>
                {
                    if(p!= null && p.TryGetComponent<UltimateArtillerySpike>(out var spike))
                    {
                        spike.OnBulletHitLand(data.Cast<BulletMovement>().bullet);
                    }
                });
            }
        }
    }
}
