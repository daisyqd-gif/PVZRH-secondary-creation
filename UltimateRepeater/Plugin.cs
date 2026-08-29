global using BepInEx;
global using CustomizeLib.BepInEx;
global using System.Reflection;
global using UnityEngine;
global using System.Collections;
global using System.Collections.Generic;
global using CustomPlantClass;
global using HarmonyLib;
global using Random=UnityEngine.Random;
global using CustomPlantClass.Main;

namespace UltimateRepeater
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Core : ModPlugin
    {
        private AssetBundle assetBundle;
        public static class DataContainer
        {
            public static ID RepeaterID;
            public static ID Bullet_ID_Pea = DataMgr.AllocateID();
            public static ID Bullet_ID_Snow = DataMgr.AllocateID();
            public static ID Bullet_ID_ExtremeSnow = DataMgr.AllocateID();
            public static ID Bullet_ID_Hypno = DataMgr.AllocateID();
            public static ID Bullet_ID_Jala = DataMgr.AllocateID();
            public static ID Bullet_ID_Cherry = DataMgr.AllocateID();
            public static ID Bullet_ID_Fire_Yellow = DataMgr.AllocateID();
            public static ID Bullet_ID_Fire_Orange = DataMgr.AllocateID();
            public static ID Bullet_ID_Fire_Red = DataMgr.AllocateID();
            public static ID Bullet_ID_Fire_Hypno = DataMgr.AllocateID();
            public static ID Bullet_ID_Fire_Ember = DataMgr.AllocateID();
            public static ID Bullet_ID_Fire_Super = DataMgr.AllocateID();
            public static ID Bullet_ID_Fire_Ulti = DataMgr.AllocateID();
            public static ID Bullet_ID_Fire_Cherry = DataMgr.AllocateID();
            public static ID Bullet_ID_Fire_Red_Big = DataMgr.AllocateID();
        }
        public override void InitializeMod()
        {
            // Load the AssetBundle containing your plant prefab(s)
            // Replace "abname" with your actual bundle name
            assetBundle = CustomCore.GetAssetBundle(
                Assembly.GetExecutingAssembly(),
                "ultimaterepeater"
            );
        }
        public override void InitializePlants()
        {
            // Fill out the plant metadata
            BaseCustomPlantData Data = new BaseCustomPlantData()
            {
                PlantId = DataMgr.AllocateID(), // Automatically assigns a unique ID

                Prefab = assetBundle.GetAsset<GameObject>("DoubleShooterPrefab"),   // Main plant prefab
                Preview = assetBundle.GetAsset<GameObject>("DoubleShooterPreview"), // Card preview prefab

                Fusions = DataMgr.MirrorList(new List<(ID, ID)>(){(PlantType.DoubleSnow,PlantType.Jalapeno),(PlantType.JalaDoubleshooter,PlantType.IceShroom),(PlantType.DoubleShooter,PlantType.ObsidianJalapeno),(PlantType.SnowPeaShooter,PlantType.JalaPeashooter)}), // Optional fusion recipes

                AttackInterval = 1.5f,   // Time between attacks (shooters only)
                ProduceInterval = 0f,  // Time between sun/production cycles
                AttackDamage = 60,      // Damage per attack
                MaxHealth = 900,       // Plant HP
                Cd = 1.5f,               // Card cooldown
                Sun = 300,               // Sun cost

                DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type, this is never used for now so just leave it as is.

                CanPF = true,     // Enable PF ability if the plant has one
                CanStarUp = false, // Enable Star-Up ability if the plant has one

                CardColor = CardLevel.Blue, // Determines card rarity and UI color
                /*
                    White  = Normal plants
                    Green  = Fusion plants
                    Blue   = Super plants
                    Purple = Weak ultimate plants
                    Gold   = Strong ultimate plants
                    Red    = Special/Treasure mode plants
                */

                IsRainbowCard = false,  // Appears in the Rainbow Card menu
                IsUltimatePlant = false, // Travel-locked ultimate plant
                CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                Name = "超级双发射手",           // Plant name (shown in UI)
                AlmanacEntry = DataMgr.CreateAlmanacEntry(
                    "一次发射两颗豌豆。",
                    recipe:("豌豆射手×2","寒冰菇+火爆辣椒"),
                    attackinterval:(60,1.5f),
                    specialeffects:new string[]{"普攻升级为巨型母弹，命中敌人后会向斜向分裂攻击。并且继承火焰、冰冻元素效果。"},
                    flavor:"如果你对双重射手好，他会加倍报答。如果你惹了他，那你可能会得到加倍的反击。当然他的人际关系相当好，大家最愿意做的就是借钱给他。"
                )    // Almanac description (CN + EN recommended)
            };
            BasePlantSkinData Data2 = new()
            {
                data=Data,
                SkinPrefab=assetBundle.GetAsset<GameObject>("SkinDoubleShooterPrefab"),
                SkinPreview=assetBundle.GetAsset<GameObject>("DoubleShooterPreview"),
                BulletSkinList=new()
            };

            // Register the plant and retrieve its ID
            DataContainer.RepeaterID = DataMgr.RegisterCustomPlant<Shooter, UltimateRepeater>(Data2);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(DataContainer.RepeaterID);
            CustomCore.TypeMgrExtra.IsIcePlant.Add(DataContainer.RepeaterID);
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Pea,
                assetBundle.GetAsset<GameObject>("Bullet_pea")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Snow,
                assetBundle.GetAsset<GameObject>("Bullet_snowpea")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_ExtremeSnow,
                assetBundle.GetAsset<GameObject>("Bullet_extremesnow")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Hypno,
                assetBundle.GetAsset<GameObject>("Bullet_hypnoPea")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Jala,
                assetBundle.GetAsset<GameObject>("Bullet_flamePea")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Cherry,
                assetBundle.GetAsset<GameObject>("Bullet_cherry")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Fire_Cherry,
                assetBundle.GetAsset<GameObject>("Bullet_fireCherry")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Fire_Yellow,
                assetBundle.GetAsset<GameObject>("Bullet_firePea_yellow")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Fire_Orange,
                assetBundle.GetAsset<GameObject>("Bullet_firePea_orange")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Fire_Red,
                assetBundle.GetAsset<GameObject>("Bullet_firePea_red")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Fire_Red_Big,
                assetBundle.GetAsset<GameObject>("Bullet_firePea_red_big")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Fire_Super,
                assetBundle.GetAsset<GameObject>("Bullet_firePea_super")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Fire_Ulti,
                assetBundle.GetAsset<GameObject>("Bullet_firePea_ultimate")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Fire_Hypno,
                assetBundle.GetAsset<GameObject>("Bullet_hypnoPea_fire")
            );
            CustomCore.RegisterCustomBullet<Bullet_pea, SplitBullet>(
                DataContainer.Bullet_ID_Fire_Ember,
                assetBundle.GetAsset<GameObject>("Bullet_firePea_purple")
            );

            Log.LogInfo($"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded.");
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class UltimateRepeater : BaseCustomPlant
    {
        public bool entered=false;
        public override Transform FindShoot() => transform.FindChild("Shoot");
        public void Enter()
        {
            entered=true;
            if(Utils.IsGameRunning()) StartPF();
            _plant.anim.SetBoolString("active",true);
        }
        public override void StartPF()
        {
            _plant.invincible = true;
            _plant.uncrashable = true;
            isPF = true;
            _plant.isFlashing = true;
            _plant.anim.SetBoolString("shooting",true);
            transform.FindChild("1000_1").gameObject.SetActive(true);
            transform.FindChild("1000_0").gameObject.SetActive(true);
            transform.FindChild("PeaShooter_Head/GatlingPea_helmet").gameObject.SetActive(true);
            transform.FindChild("PeaShooter_Head/PeaShooter_Head (1)").gameObject.SetActive(true);
        }
        public void StartShooting()
        {
            // Wrap the coroutine so we can detect when it finishes
            _plant.StartCoroutine(SuperShoot());
        }
        public override BulletType GetBulletType2()
        {
            int i=Random.Range(0,11);
            switch (i)
            {
                case 1: return Core.DataContainer.Bullet_ID_Snow;
                case 2: return Core.DataContainer.Bullet_ID_Hypno;
                case 3: return Core.DataContainer.Bullet_ID_Jala;
                case 4: return Core.DataContainer.Bullet_ID_Cherry;
                case 5: return Core.DataContainer.Bullet_ID_Fire_Yellow;
                case 6: return Core.DataContainer.Bullet_ID_Fire_Orange;
                case 7: return Core.DataContainer.Bullet_ID_Fire_Hypno;
                case 8: return Core.DataContainer.Bullet_ID_Fire_Ember;
                case 9: return Core.DataContainer.Bullet_ID_Fire_Cherry;
                case 10: return Core.DataContainer.Bullet_ID_Fire_Red;
                default: return Core.DataContainer.Bullet_ID_Pea;
            }
        }
        public override BulletType GetBulletType()
        {
            int i=Random.Range(0,7);
            switch (i)
            {
                case 1: return Core.DataContainer.Bullet_ID_Snow;
                case 2: return Core.DataContainer.Bullet_ID_Hypno;
                case 3: return Core.DataContainer.Bullet_ID_Jala;
                case 4: return Core.DataContainer.Bullet_ID_Cherry;
                case 5: return Core.DataContainer.Bullet_ID_Fire_Yellow;
                case 6: return Core.DataContainer.Bullet_ID_Fire_Orange;
                default: return Core.DataContainer.Bullet_ID_Pea;
            }
        }
        public static int GetBulletDamageMultiplier(BulletType bulletType)
        {
            if(
                bulletType==Core.DataContainer.Bullet_ID_Pea ||
                bulletType==Core.DataContainer.Bullet_ID_Hypno ||
                bulletType == Core.DataContainer.Bullet_ID_Snow
            )
            {
                return 1;
            }
            else if (
                bulletType==Core.DataContainer.Bullet_ID_Jala ||
                bulletType==Core.DataContainer.Bullet_ID_Cherry ||
                bulletType==Core.DataContainer.Bullet_ID_Fire_Yellow ||
                bulletType==Core.DataContainer.Bullet_ID_Fire_Ember ||
                bulletType == Core.DataContainer.Bullet_ID_Fire_Hypno
            )
            {
                return 2;
            }
            else if (
                bulletType==Core.DataContainer.Bullet_ID_Fire_Orange
            )
            {
                return 3;
            }
            else if(
                bulletType==Core.DataContainer.Bullet_ID_Fire_Red ||
                bulletType==Core.DataContainer.Bullet_ID_Fire_Cherry
            )
            {
                return 4;
            }
            else if(
                bulletType==Core.DataContainer.Bullet_ID_Fire_Red_Big
            )
            {
                return 40; //yes, this is the big pea at the end, the damage is intended
            }
            else return 1;
        }
        public override void SuperEnd()
        {
            base.SuperEnd();
            _plant.anim.SetBoolString("shooting",false);
            transform.FindChild("1000_1").gameObject.SetActive(false);
            transform.FindChild("1000_0").gameObject.SetActive(false);
            transform.FindChild("PeaShooter_Head/GatlingPea_helmet").gameObject.SetActive(false);
            transform.FindChild("PeaShooter_Head/PeaShooter_Head (1)").gameObject.SetActive(false);
        }
        public override IEnumerator SuperShoot()
        {
            if(_plant.shoot==null)yield break;
            for(int i = 0; i < 60; i++)
            {
                BulletType btype=GetBulletType2();
                var b=PlantMgr.SetBullet(_plant,btype,BulletMoveWay.MoveRight,new Vector2(Random.Range(-0.3f,0.3f),Random.Range(-0.3f,0.3f)));
                if(b!=null)
                {
                    b.Damage*=GetBulletDamageMultiplier(b.theBulletType);
                    b.GetComponent<SplitBullet>().splittimes=2;
                }
                else Debug.LogError("Null bullet type: "+(int)btype);
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
            _plant.anim.SetTriggerString("stopshooting");
        }
        public override Bullet Shoot2_Custom()
        {
            var b=PlantMgr.SetBullet(_plant,Core.DataContainer.Bullet_ID_Fire_Red_Big,BulletMoveWay.MoveRight);
            b.Damage*=GetBulletDamageMultiplier(b.theBulletType);
            b.GetComponent<SplitBullet>().splittimes=2;
            return b;
        }
        public override Bullet Shoot_Custom()
        {
            if(!entered) return null;
            var b=PlantMgr.SetBullet(_plant,GetBulletType(),BulletMoveWay.MoveRight);
            b.Damage*=GetBulletDamageMultiplier(b.theBulletType);
            b.GetComponent<SplitBullet>().splittimes=2;
            return b;
        }
    }

    public class SplitBullet : BaseCustomBullet
    {
        public int splittimes=2;
        public virtual BulletType GetBulletType(BulletType inType)
        {
            if (inType == Core.DataContainer.Bullet_ID_Pea)
                return BulletType.Bullet_pea;

            if (inType == Core.DataContainer.Bullet_ID_Snow)
                return BulletType.Bullet_snowPea;

            if (inType == Core.DataContainer.Bullet_ID_ExtremeSnow)
                return BulletType.Bullet_extremeSnowPea;

            if (inType == Core.DataContainer.Bullet_ID_Hypno)
                return BulletType.Bullet_hypnoPea;

            if (inType == Core.DataContainer.Bullet_ID_Jala)
                return BulletType.Bullet_pea_jala;

            if (inType == Core.DataContainer.Bullet_ID_Cherry)
                return BulletType.Bullet_cherry;

            if (inType == Core.DataContainer.Bullet_ID_Fire_Yellow)
                return BulletType.Bullet_firePea_yellow;

            if (inType == Core.DataContainer.Bullet_ID_Fire_Orange)
                return BulletType.Bullet_firePea_orange;

            if (inType == Core.DataContainer.Bullet_ID_Fire_Red)
                return BulletType.Bullet_firePea_red;

            if (inType == Core.DataContainer.Bullet_ID_Fire_Hypno)
                return BulletType.Bullet_hypnoPea_fire;

            if (inType == Core.DataContainer.Bullet_ID_Fire_Ember)
                return BulletType.Bullet_fireStar_magic;

            if (inType == Core.DataContainer.Bullet_ID_Fire_Super)
                return BulletType.Bullet_firePea_super;

            if (inType == Core.DataContainer.Bullet_ID_Fire_Ulti)
                return BulletType.Bullet_firePea_ultimate;

            if (inType == Core.DataContainer.Bullet_ID_Fire_Cherry)
                return BulletType.Bullet_fireCherry;

            if (inType == Core.DataContainer.Bullet_ID_Fire_Red_Big)
                return Core.DataContainer.Bullet_ID_Fire_Red; // damage overridden elsewhere

            // fallback
            return BulletType.Bullet_pea;
        }
        public override bool HitZombie(Zombie zombie)
        {
            if (splittimes > 1)
            {
                Bullet b1;
                if(_bullet.theBulletRow==0) b1=InstanceManager.CreateBullet.SetBullet(_bullet.transform.position.x,_bullet.transform.position.y,_bullet.theBulletRow,_bullet.theBulletType,BulletMoveWay.MoveRight);
                else b1=InstanceManager.CreateBullet.SetBullet(_bullet.transform.position.x,_bullet.transform.position.y,_bullet.theBulletRow-1,_bullet.theBulletType,BulletMoveWay.Three_up);
                b1.Damage=_bullet.Damage/2;
                b1.fromType=_bullet.fromType;
                b1.GetComponent<SplitBullet>().splittimes=splittimes-1;
                Bullet b2;
                if(_bullet.theBulletRow==_bullet.board.rowNum) b2=InstanceManager.CreateBullet.SetBullet(_bullet.transform.position.x,_bullet.transform.position.y,_bullet.theBulletRow,_bullet.theBulletType,BulletMoveWay.MoveRight);
                else b2=InstanceManager.CreateBullet.SetBullet(_bullet.transform.position.x,_bullet.transform.position.y,_bullet.theBulletRow+1,_bullet.theBulletType,BulletMoveWay.Three_down);
                b2.Damage=_bullet.Damage/2;
                b2.fromType=_bullet.fromType;
                b2.GetComponent<SplitBullet>().splittimes=splittimes-1;
            }
            else
            {
                Bullet b1;
                if(_bullet.theBulletRow==0) b1=InstanceManager.CreateBullet.SetBullet(_bullet.transform.position.x,_bullet.transform.position.y,_bullet.theBulletRow,GetBulletType(_bullet.theBulletType),BulletMoveWay.MoveRight);
                else b1=InstanceManager.CreateBullet.SetBullet(_bullet.transform.position.x,_bullet.transform.position.y,_bullet.theBulletRow-1,GetBulletType(_bullet.theBulletType),BulletMoveWay.Three_up);
                b1.Damage=_bullet.Damage/2;
                b1.fromType=_bullet.fromType;
                Bullet b2;
                if(_bullet.theBulletRow==_bullet.board.rowNum) b2=InstanceManager.CreateBullet.SetBullet(_bullet.transform.position.x,_bullet.transform.position.y,_bullet.theBulletRow,GetBulletType(_bullet.theBulletType),BulletMoveWay.MoveRight);
                else b2=InstanceManager.CreateBullet.SetBullet(_bullet.transform.position.x,_bullet.transform.position.y,_bullet.theBulletRow+1,GetBulletType(_bullet.theBulletType),BulletMoveWay.Three_down);
                b2.Damage=_bullet.Damage/2;
                b2.fromType=_bullet.fromType;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(UltimateTorch),nameof(UltimateTorch.OnTriggerEnter2D))]
    public class UltimateTorch_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(UltimateTorch __instance, Collider2D collision)
        {
            if (!collision.TryGetComponent(out Bullet bullet))
                return true;

            // Only process bullets from same row and not from plants
            if (bullet.theBulletRow != __instance.thePlantRow || bullet.from != null)
                return true;

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Pea || bullet.theBulletType == Core.DataContainer.Bullet_ID_Fire_Yellow || bullet.theBulletType == Core.DataContainer.Bullet_ID_Fire_Orange)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Red).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Hypno)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Hypno).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Jala)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Super).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Fire_Super)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Ulti).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Cherry)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Cherry).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(UltimateStarTorch),nameof(UltimateStarTorch.OnTriggerEnter2D))]
    public class UltimateStarTorch_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(UltimateStarTorch __instance, Collider2D collision)
        {
            if (!collision.TryGetComponent(out Bullet bullet))
                return true;

            // Only process bullets from same row and not from plants
            if (bullet.theBulletRow != __instance.thePlantRow || bullet.from != null)
                return true;

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Pea || bullet.theBulletType == Core.DataContainer.Bullet_ID_Fire_Yellow || bullet.theBulletType == Core.DataContainer.Bullet_ID_Fire_Orange)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Red).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Hypno)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Hypno).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Jala)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Super).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Fire_Super)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Ulti).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Cherry)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Cherry).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(SuperTorch),nameof(SuperTorch.OnTriggerEnter2D))]
    public class SuperTorch_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SuperTorch __instance, Collider2D collision)
        {
            if (!collision.TryGetComponent(out Bullet bullet))
                return true;

            // Only process bullets from same row and not from plants
            if (bullet.theBulletRow != __instance.thePlantRow || bullet.from != null)
                return true;

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Pea || bullet.theBulletType == Core.DataContainer.Bullet_ID_Fire_Yellow || bullet.theBulletType == Core.DataContainer.Bullet_ID_Fire_Orange)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Red).GetComponent<SplitBullet>().splittimes=splittimes;
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Hypno)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Hypno).GetComponent<SplitBullet>().splittimes=splittimes;
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Jala)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Super).GetComponent<SplitBullet>().splittimes=splittimes;
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Fire_Super)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Ulti).GetComponent<SplitBullet>().splittimes=splittimes;
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Cherry)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,4,Core.DataContainer.Bullet_ID_Fire_Cherry).GetComponent<SplitBullet>().splittimes=splittimes;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(JalaTorch),nameof(JalaTorch.OnTriggerEnter2D))]
    public class JalaTorch_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(JalaTorch __instance, Collider2D collision)
        {
            if (!collision.TryGetComponent(out Bullet bullet))
                return true;

            // Only process bullets from same row and not from plants
            if (bullet.theBulletRow != __instance.thePlantRow || bullet.from != null)
                return true;

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Pea || bullet.theBulletType == Core.DataContainer.Bullet_ID_Fire_Yellow)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,3,Core.DataContainer.Bullet_ID_Fire_Orange).GetComponent<SplitBullet>().splittimes=splittimes;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(TorchWood),nameof(TorchWood.OnTriggerEnter2D))]
    public class TorchWood_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(TorchWood __instance, Collider2D collision)
        {
            if (!collision.TryGetComponent(out Bullet bullet))
                return true;

            // Only process bullets from same row and not from plants
            if (bullet.theBulletRow != __instance.thePlantRow || bullet.from != null)
                return true;

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Pea)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,3,Core.DataContainer.Bullet_ID_Fire_Yellow).GetComponent<SplitBullet>().splittimes=splittimes;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(SquashTorch),nameof(SquashTorch.OnTriggerEnter2D))]
    public class SquashTorch_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SquashTorch __instance, Collider2D collision)
        {
            if (!collision.TryGetComponent(out Bullet bullet))
                return true;

            // Only process bullets from same row and not from plants
            if (bullet.theBulletRow != __instance.thePlantRow || bullet.from != null)
                return true;

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Pea)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,3,Core.DataContainer.Bullet_ID_Fire_Yellow).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(300);
                }
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(TorchPumpkin),nameof(TorchPumpkin.OnTriggerEnter2D))]
    public class TorchPumpkin_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(TorchPumpkin __instance, Collider2D collision)
        {
            if (!collision.TryGetComponent(out Bullet bullet))
                return true;

            // Only process bullets from same row and not from plants
            if (bullet.theBulletRow != __instance.thePlantRow || bullet.from != null)
                return true;

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Pea)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,3,Core.DataContainer.Bullet_ID_Fire_Yellow).GetComponent<SplitBullet>().splittimes=splittimes;
                __instance.fireTimes++;
                if (__instance.fireTimes > 6)
                {
                    __instance.fireTimes = 0;
                    __instance.SummonPlant(4000);
                }
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(DoomTorch),nameof(DoomTorch.OnTriggerEnter2D))]
    public class DoomTorch_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(DoomTorch __instance, Collider2D collision)
        {
            if (!collision.TryGetComponent(out Bullet bullet))
                return true;

            // Only process bullets from same row and not from plants
            if (bullet.theBulletRow != __instance.thePlantRow || bullet.from != null)
                return true;

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Pea)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,2,Core.DataContainer.Bullet_ID_Fire_Ember).GetComponent<SplitBullet>().splittimes=splittimes;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(IceTorch),nameof(IceTorch.OnTriggerEnter2D))]
    public class IceTorch_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(IceTorch __instance, Collider2D collision)
        {
            if (!collision.TryGetComponent(out Bullet bullet))
                return true;

            // Only process bullets from same row and not from plants
            if (bullet.theBulletRow != __instance.thePlantRow || bullet.from != null)
                return true;

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Pea)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,1,Core.DataContainer.Bullet_ID_Snow).GetComponent<SplitBullet>().splittimes=splittimes;
                return false;
            }

            if (bullet.theBulletType == Core.DataContainer.Bullet_ID_Snow)
            {
                int splittimes=bullet.GetComponent<SplitBullet>().splittimes;
                bullet.board.boardAction.FirePeas(bullet,__instance,2,Core.DataContainer.Bullet_ID_ExtremeSnow).GetComponent<SplitBullet>().splittimes=splittimes;
                return false;
            }
            return true;
        }
    }

    public class MyPluginInfo
    {
        public const string PluginGuid = "UltimateRepeater.Bepinex";
        public const string PluginName = "UltimateRepeater";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
