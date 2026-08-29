global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using CustomizeLib.BepInEx;
global using CustomPlantClass;
global using HarmonyLib;
global using Il2CppInterop.Runtime.Injection;
global using System;
global using System.Collections.Generic;
global using System.Reflection;
global using UnityEngine;
global using CustomPlantClass.Main;

namespace CharmSniper
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "CharmSniper.Bepinex";
        public const string PluginName = "CharmSniper";
        public const string PluginVersion = MyPluginInfo.TargetVersion;

        public override void Load()
        {
            try
            {
                // Apply all Harmony patches in this assembly
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

                // Register the custom plant class with IL2CPP
                // (Required for all custom MonoBehaviours)
                ClassInjector.RegisterTypeInIl2Cpp<CharmSniper>();

                // Load the AssetBundle containing your plant prefab(s)
                // Replace "abname" with your actual bundle name
                AssetBundle assetBundle = CustomCore.GetAssetBundle(
                    Assembly.GetExecutingAssembly(),
                    "charmsniper"
                );

                // Fill out the plant metadata
                BaseCustomPlantData Data = new BaseCustomPlantData()
                {
                    PlantId = DataMgr.AllocateID(), // Automatically assigns a unique ID

                    Prefab = assetBundle.GetAsset<GameObject>("CharmSniperPrefab"),   // Main plant prefab
                    Preview = assetBundle.GetAsset<GameObject>("CharmSniperPreview"), // Card preview prefab

                    Fusions = DataMgr.MirrorList(new List<(ID, ID)>() { (PlantType.SniperPea, PlantType.HypnoShroom), (PlantType.SuperHypnoGatling, PlantType.Peashooter) }), // Optional fusion recipes

                    AttackInterval = 1.5f,   // Time between attacks (shooters only)
                    ProduceInterval = 0f,  // Time between sun/production cycles
                    AttackDamage = 300,      // Damage per attack
                    MaxHealth = 300,       // Plant HP
                    Cd = 0f,               // Card cooldown
                    Sun = 725,               // Sun cost : 600(Sniper pea) + 125(Hypno Shroom)

                    DefaultBullet = BulletType.Bullet_pea, // Shooter bullet type

                    CanPF = false,     // Enable PF ability if the plant has one
                    CanStarUp = false, // Enable Star-Up ability if the plant has one

                    CardColor = CardLevel.Purple, // Determines card rarity and UI color
                    /*
                        White  = Normal plants
                        Green  = Fusion plants
                        Blue   = Super plants
                        Purple = Weak ultimate plants
                        Gold   = Strong ultimate plants
                        Red    = Special/Treasure mode plants
                    */

                    IsRainbowCard = false,  // Appears in the Rainbow Card menu
                    IsUltimatePlant = true, // Travel-locked ultimate plant
                    CardRepeatAmt = 1,       // How many copies appear in Rainbow Card menu

                    Name = "魅惑狙击豌豆  Charm Sniper",           // Plant name (shown in UI)
                    AlmanacEntry = "远程魅惑并狙击一只僵尸，造成高额伤害；若目标具备魅惑免疫，则改为施加魅惑冲击并重置目标\n\n" +
                        "<color=#3D1400>贴图作者：</color><color=red>@你的名字</color>\n" +
                        "<color=#3D1400>伤害：</color><color=red>300/1.5秒</color>\n" +
                        "<color=#3D1400>特点：</color><color=red>融合狙击豌豆与魅惑菇的能力，远程魅惑僵尸；每6次狙击造成毁灭性伤害；对魅惑免疫僵尸施加魅惑冲击并重置仇恨</color>\n" +
                        "<color=#3D1400>融合配方：</color><color=red>狙击射手 + 魅惑菇</color>\n" +
                        "<color=#3D1400>转换配方：</color><color=red>超级魅惑机枪射手 ←→ 豌豆射手</color>\n\n" +
                        "<color=#3D1400>“魅惑回声研究所”前首席情绪干扰师。它声称魅惑不是魔法，而是把僵尸的脑回路轻轻拧了一下，让它们突然觉得‘同类真可爱’。第六发‘心灵穿刺’？嘘——别问，那是它在僵尸心房里按下的重置键。警告：请勿在情绪不稳定的僵尸、哲学系僵尸或正在思考“我是谁”的僵尸面前种植… 毕竟，它射出的不是豌豆，是恋爱脑的开关。</color>"+
                        "\n\n【English Translation】\n Snipes a zombie every 1.5 seconds, hypnotizing it. If that fails, it will deal a high amount of damage." +
                        "Former chief ‘Emotional Interference Engineer’ of the Enchanting Echo Institute. Charm Sniper insists that mind control isn’t magic — it’s just a gentle twist of a zombie’s neural dial, making them suddenly think their fellow undead look adorable. And that sixth shot, the so‑called ‘Heart‑Piercing Reset’? Shh… don’t ask. That’s the moment it taps the reset switch inside a zombie’s chest cavity.\n\n"    // Almanac description (CN + EN recommended)
                };

                // Register the plant and retrieve its ID
                ID plantID = DataMgr.RegisterCustomPlant<SniperPea, CharmSniper>(Data);

                CustomCore.AddFusion((int)PlantType.SuperHypnoGatling, plantID, (int)PlantType.Peashooter);
                CustomCore.AddFusion((int)PlantType.SuperHypnoGatling, (int)PlantType.Peashooter, plantID);
                CustomCore.RegisterCustomClickCardOnPlantEvent(plantID,PlantType.GatlingPea,(Plant p) => {Lawnf.SetDroppedCard(p.axis.position,PlantType.SniperPea);});
            }
            catch (Exception e)
            {
                DataMgr.StartUpMessages.Add($"Charm sniper mod load failed.\n{e.ToString}");
            }



            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }

    // Your custom plant class. Put this into its own file if it gets too big
    // You can leave it empty or override BaseCustomPlant methods for custom behavior.
    public class CharmSniper : BaseCustomPlant
    {
        public SniperPea plant => gameObject.GetComponent<SniperPea>();
        public override Bullet Shoot_Custom()
        {
            try
            {
                Zombie target = plant.targetZombie;
                if (!plant.SearchUniqueZombie(target))
                {
                    target = plant.SearchZombie().GetComponent<Zombie>();
                }
                plant.attackCount++;
                if (AttackEffect(target).isMindControlled == false) //some zombies have hypno immunity
                {
                    GameAPP.PlaySound(0x28, 0.2f, 1f);
                    int damage = plant.attackDamage;
                    DamageType damageType = DamageType.MaxDamage;
                    if (plant.attackCount % 6 == 0)
                    {
                        damage = 1000000000; //added a few more 0's
                        damageType = DamageType.MaxDamage;
                    }
                    plant.AttackZombie(target, damage, damageType);
                }
                else
                {
                    GameAPP.PlaySound(0x3e, 0.2f, 1f);
                    GameAPP.PlaySound(0x3f, 0.2f, 1f);
                    Vector2 pos = target.axis.position;
                    CreateParticle.SetParticle(0x14, new Vector2(pos.x, pos.y + 1.5f), target.theZombieRow, false);
                    plant.targetZombie = null;
                }
                if (target.theStatus == ZombieStatus.Dying || target.beforeDying)
                {
                    plant.targetZombie = null;
                }
            }
            catch(Exception){}
            return null;
        }
        public virtual Zombie AttackEffect(Zombie zombie)
        {
            zombie.SetMindControl();
            return zombie;
        }
        public override string GetTextString() => $"充能 : {plant.attackCount % 6}";
    }
}
