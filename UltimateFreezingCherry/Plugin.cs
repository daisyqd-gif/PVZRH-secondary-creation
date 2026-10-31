using BepInEx;
using CustomizeLib.BepInEx;
using CustomizeLib.BepInEx.ExtensionData.Basic;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;
using Core;
using System.Collections.Generic;
using System;
using CustomPlantClass;
using CustomPlantClass.Main;

namespace FreezingCherry.BepInEx
{
    [BepInPlugin("salmon.freezingcherry", "FreezingCherry", "1.0")]
    public class Plugin : CorePlugin
    {
        public static ID Regular_PlantID = 1957;
        public override void OnStart()
        {
            Tools.InitMod();
            var ab = CustomCore.GetAssetBundle(Tools.GetAssembly(), "freezingcherry");
            {
                CustomCore.RegisterCustomPlant<IceCherry, FreezingCherry>(Regular_PlantID, ab.GetAsset<GameObject>("FreezingCherryPrefab"),
                    ab.GetAsset<GameObject>("FreezingCherryPreview"), new List<(int, int)>
                    {
                        ((int)PlantType.IceCherry, (int)PlantType.DoomShroom),
                        ((int)PlantType.DoomShroom, (int)PlantType.IceCherry),
                        ((int)PlantType.IceDoom, (int)PlantType.CherryBomb),
                        ((int)PlantType.CherryBomb, (int)PlantType.IceDoom),
                        ((int)PlantType.DoomCherry, (int)PlantType.IceShroom),
                        ((int)PlantType.IceShroom, (int)PlantType.DoomCherry)
                    }, 0f, 0f, 3600, 300, 7.5f, 350);
                CustomCore.AddPlantAlmanacStrings(Regular_PlantID, $"寒霜樱桃",
                    "特制的毁灭樱桃模具，吸收特定子弹来生产樱桃炸弹和毁灭菇。\n\n" +
                    "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                    "<color=#3D1400>融合配方：</color><color=red>樱桃炸弹+寒冰菇+毁灭菇</color>\n" +
                    "<color=#3D1400>伤害：</color><color=red>3600（灰烬）</color>\n" +
                    "<color=#3D1400>特点：</color><color=red>①不会自主爆炸\n" +
                    "②吸收特定子弹，回复能量\n" +
                    "每100点能量，产出一张樱桃炸弹卡片，每产生3张卡片，额外产出一张毁灭菇卡片\n" +
                    "*产出的卡片上限为8张，超出的部分会立即销毁</color>\n" +
                    "<color=#3D1400>蓄能能量：</color><color=red>①樱桃子弹：回复1点能量\n" +
                    "②火焰樱桃子弹，毁灭豌豆子弹，寄生樱桃子弹：回复2点能量\n" +
                    "③爆炸樱桃子弹，毁灭菇子弹及变种：回复10点能量\n" +
                    "④大毁灭菇子弹及变种：回复20点能量\n" +
                    "<color=#3D1400>词条1:</color><color=red>凝冰之力：寒霜樱桃（及其进阶）获得的能量x2</color>\n" +
                    "<color=#3D1400>词条2:</color><color=red>霜灭：寒霜樱桃（及其进阶）生产的卡片持续时间减半，卡片消失后，造成寒冰樱桃爆炸和寒冰毁灭菇爆炸</color>\n\n" +
                    "<color=#3D1400>作为一名战地记者，寒霜樱桃始终奔赴在前线，跟随大部队的步伐，穿梭于僵尸和植物之间，他们从不害怕，“越是危险的地方，越能看清事件的本质，我们想要知道僵尸需要脑子的根本原因，如这将是我们停止战争的关键！”</color>");
                CustomCore.TypeMgrExtra.IsIcePlant.Add(Regular_PlantID);
                FreezingCherry.buff1 = Compatibility.CustomCore_Old.RegisterCustomBuff("凝冰之力：寒霜樱桃获得的能量x2，进阶形态共享此效果", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<FreezingCherry>() || Board.Instance.ObjectExist<UltimateFreezingCherry>(),
                    5000, Regular_PlantID, bg: BuffBgType.Night);
                FreezingCherry.buff2 = Compatibility.CustomCore_Old.RegisterCustomBuff("霜灭：寒霜樱桃生产的卡片持续时间减半，卡片消失后，造成寒冰樱桃爆炸和寒冰毁灭菇爆炸，进阶形态共享此效果", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<FreezingCherry>() || Board.Instance.ObjectExist<UltimateFreezingCherry>(),
                    5000, Regular_PlantID, bg: BuffBgType.Night);
                CustomCore.RegisterCustomParticle(FreezingCherry.ParticleID, ab.GetAsset<GameObject>("IceDoomNoSprite").AddComponent<CustomParticle>().gameObject);
                CustomCore.AddUltimatePlant(Regular_PlantID);
            }
            {
                CustomCore.RegisterCustomPlant<IceCherry, UltimateFreezingCherry>(UltimateFreezingCherry.PlantID, ab.GetAsset<GameObject>("UltimateFreezingCherryPrefab"),
                    ab.GetAsset<GameObject>("UltimateFreezingCherryPreview"), new List<(int, int)>
                    {
                        (Regular_PlantID, (int)PlantType.IceShroom),
                        ((int)PlantType.IceShroom, Regular_PlantID),
                        ((int)PlantType.IceCherry, (int)PlantType.IceDoom),
                        ((int)PlantType.IceDoom, (int)PlantType.IceCherry),
                    }, 0f, 0f, 3600, 300, 7.5f, 425);

                CustomCore.AddPlantAlmanacStrings(UltimateFreezingCherry.PlantID, $"究极寒霜樱桃",
                    "从樱桃和毁灭子弹积蓄，生产樱桃炸弹和毁灭菇，并对僵尸投射爆炸樱桃副产物。\n" +
                    "<color=#0000FF>寒霜樱桃的进阶形态</color>\n\n" +
                    "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                    "<color=#3D1400>获取条件：</color><color=red>集齐基础形态双词条</color>\n" +
                    "<color=#3D1400>融合配方：</color><color=red>寒霜樱桃+寒冰菇</color>\n" +
                    "<color=#3D1400>伤害：</color><color=red>3600（灰烬）</color>\n" +
                    "<color=#3D1400>特点：</color><color=red>①不会自主爆炸\n" +
                    "②吸收特定子弹，回复能量\n" +
                    "每100点能量，产出一张樱桃炸弹卡片，每产生3张卡片，额外产出一张毁灭菇卡片\n" +
                    "*产出的卡片上限为8张，超出的部分会立即销毁\n" +
                    "④每次生产卡片时，投射3颗寒霜核能樱桃，全屏索敌，命中施加5秒冻结\n" +
                    "⑤使用樱桃炸弹或毁灭菇卡片可立即释放寒冰樱桃和寒冰毁灭菇效果</color>\n" +
                    "<color=#3D1400>蓄能能量：</color><color=red>①樱桃子弹：回复1点能量\n" +
                    "②火焰樱桃子弹，毁灭豌豆子弹，寄生樱桃子弹：回复2点能量\n" +
                    "③爆炸樱桃子弹，毁灭菇子弹及变种：回复10点能量\n" +
                    "④大毁灭菇子弹及变种：回复20点能量\n" +
                    "<color=#3D1400>连携词条:</color><color=red>霜陨子母弹：手套抓取核爆樱桃及亚种点击究极寒霜樱桃时，双方将进行短暂融合裂变，对全屏僵尸造成毁灭性打击：对全场造成150倍究极寒霜樱桃的伤害并冻结10秒，对周围连续产生大量樱桃爆炸，爆出72颗一圈的寒霜核能樱桃子弹，并留下10秒的寒霜辐射区域，冻结10秒关卡</color>\n\n" +
                    "<color=#3D1400>究极寒霜樱桃毅然决然的投入到与僵尸的斗争中，只是一昧的搜集研究是无法找到解决方法的，究极寒霜樱桃始终顶在最前边，“我的身后不只是我的队友，更是万家灯火！”</color>");
                CustomCore.TypeMgrExtra.IsIcePlant.Add(UltimateFreezingCherry.PlantID);
                ab.GetAsset<GameObject>("Doom_nuclear2").AddComponent<CustomParticle>();
                CustomCore.RegisterCustomParticle(UltimateFreezingCherry.ParticleID, ab.GetAsset<GameObject>("Doom_nuclear2"));
                ab.GetAsset<GameObject>("Doom_nuclear").AddComponent<CustomParticle>();
                CustomCore.RegisterCustomParticle(UltimateFreezingCherry.DoomParticle, ab.GetAsset<GameObject>("Doom_nuclear"));
                ab.GetAsset<GameObject>("Radiation").AddComponent<IceRadiation>();
                UltimateFreezingCherry.IceRadiation = ab.GetAsset<GameObject>("Radiation");
                ab.GetAsset<GameObject>("IceDoomOnlySprite").AddComponent<CustomParticle>();
                CustomCore.RegisterCustomParticle(UltimateFreezingCherry.IceDoomSprite, ab.GetAsset<GameObject>("IceDoomOnlySprite"));
                CustomCore.RegisterCustomCherry(UltimateFreezingCherry.CherryID, ab.GetAsset<GameObject>("BombCloud_vision_Super"));
                CustomCore.RegisterCustomCherry(UltimateFreezingCherry.CherrySmallID, ab.GetAsset<GameObject>("BombCloudSmall"));
                CustomCore.RegisterCustomClickCardOnPlantEvent(UltimateFreezingCherry.PlantID, PlantType.CherryBomb, (p) =>
                {
                    p.board.boardAction.SetDoom(0, 0, false, true, p.axis.transform.position, 3600, fromType: p.thePlantType);
                    Action<Zombie> action = (z) =>
                    {
                        z.SetFreeze(10f);
                        z.SetCold(15f);
                        z.TakeDamage(DmgType.NormalAll, p.attackDamage / 2, p.thePlantType);
                    };
                    p.board.boardAction.CreateCherryExplode(p.axis.transform.position, p.thePlantRow, CherryBombType.IceCharry, p.attackDamage, p.thePlantType, action);
                }, onPlant: new CustomClickCardOnPlant
                {
                    Trigger = CustomClickCardOnPlant.TriggerType.CardOnly
                });
                CustomCore.RegisterCustomClickCardOnPlantEvent(UltimateFreezingCherry.PlantID, PlantType.DoomShroom, (p) =>
                {
                    p.board.boardAction.SetDoom(0, 0, false, true, p.axis.transform.position, 3600, fromType: p.thePlantType);
                    Action<Zombie> action = (z) =>
                    {
                        z.SetFreeze(10f);
                        z.SetCold(15f);
                        z.TakeDamage(DmgType.NormalAll, p.attackDamage / 2, p.thePlantType);
                    };
                    p.board.boardAction.CreateCherryExplode(p.axis.transform.position, p.thePlantRow, CherryBombType.IceCharry, p.attackDamage, p.thePlantType, action);
                }, onPlant: new CustomClickCardOnPlant
                {
                    Trigger = CustomClickCardOnPlant.TriggerType.CardOnly
                });
                CustomCore.RegisterCustomClickCardOnPlantEvent(UltimateFreezingCherry.PlantID, PlantType.NuclearDoomCherry, (p) => p.anim.SetTrigger("super_cherry"),
                    (p) => p.anim.GetInteger("cureentStatus") == 0 && Lawnf.TravelAdvanced(UltimateFreezingCherry.buff));
                CustomCore.RegisterCustomClickCardOnPlantEvent(UltimateFreezingCherry.PlantID, PlantType.NuclearSquash, (p) => p.anim.SetTrigger("super_squash"),
                    (p) => p.anim.GetInteger("cureentStatus") == 0 && Lawnf.TravelAdvanced(UltimateFreezingCherry.buff));
                UltimateFreezingCherry.buff = Compatibility.CustomCore_Old.RegisterCustomBuff("霜陨子母弹：手套抓取核爆樱桃及亚种点击究极寒霜樱桃时，双方将进行短暂融合裂变，对全屏僵尸造成毁灭性打击",
                    BuffType.AdvancedBuff, () => TravelStore.Instance != null && Lawnf.TravelAdvanced((AdvBuff)30) && Lawnf.TravelAdvanced((AdvBuff)31) &&
                    Lawnf.TravelAdvanced(FreezingCherry.buff1) && Lawnf.TravelAdvanced(FreezingCherry.buff2), 15000, plantType: PlantType.EndoFlame);
                CustomCore.RegisterCustomBanMix(UltimateFreezingCherry.PlantID, () => (Lawnf.TravelAdvanced(FreezingCherry.buff1) && Lawnf.TravelAdvanced(FreezingCherry.buff2)) ||
                Board.Instance.boardTag.enableAllTravelPlant || Board.Instance.boardTag.isSuperRandom || Board.Instance.boardTag.isUltimateSuperRandom || GameAPP.developerMode,
                    null, () => InGameText.Instance.ShowText("该配方需要抽取", 3f));
                CustomCore.AddUltimatePlant(UltimateFreezingCherry.PlantID);
                CustomCore.RegisterCustomBullet<Bullet_nuclear>(UltimateFreezingCherry.BulletID, ab.GetAsset<GameObject>("Bullet_IceNuclear"));
            }
            new RSCore(this);
        }
    }

    [HarmonyPatch(typeof(IceCherry))]
    public static class IceCherryPatch
    {
        [HarmonyPatch(nameof(IceCherry.OnTriggerEnter2D))]
        [HarmonyPrefix]
        public static bool PreOnTriggerEnter2D(IceCherry __instance, ref Collider2D collision)
        {
            if (__instance != null && __instance.thePlantType == Plugin.Regular_PlantID)
            {
                if (__instance != null && __instance.TryGetComponent<FreezingCherry>(out var cherry) && cherry != null)
                    cherry.AddEnergy(collision);
                return false;
            }
            if (__instance != null && __instance.thePlantType == UltimateFreezingCherry.PlantID)
            {
                if (__instance != null && __instance.TryGetComponent<UltimateFreezingCherry>(out var cherry) && cherry != null)
                    cherry.AddEnergy(collision);
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(IceCherry.DieEvent))]
        [HarmonyPrefix]
        public static bool PreDieEvent(IceCherry __instance, ref Plant.DieReason reason)
        {
            if (__instance != null && (__instance.thePlantType == Plugin.Regular_PlantID || __instance.thePlantType == UltimateFreezingCherry.PlantID) && reason != Plant.DieReason.ByMix && reason != Plant.DieReason.BySelf)
            {
                Action<Zombie> action = (z) =>
                {
                    z.SetFreeze(10f);
                    z.SetCold(15f);
                    int damage = __instance.attackDamage;
                    if (z.GetAttrTimers().freezeTimer > 0f)
                        damage *= 4;
                    z.TakeDamage(DmgType.NormalAll, damage, __instance.thePlantType);
                };
                __instance.board.boardAction.CreateCherryExplode(__instance.axis.transform.position, __instance.thePlantRow, CherryBombType.IceCharry, __instance.attackDamage, __instance.thePlantType, action);
                __instance.board.boardAction.SetDoom(0, 0, false, true, __instance.axis.transform.position, 3600, fromType: __instance.thePlantType);
                if (__instance.TryGetComponent<UltimateFreezingCherry>(out var cherry) && cherry != null && !cherry.IsDestroyed())
                    __instance.StopAllCoroutines();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Plant))]
    public static class PlantPatch
    {
        [HarmonyPatch(nameof(Plant.Die))]
        [HarmonyPrefix]
        public static bool Prefix(Plant __instance, ref Plant.DieReason reason)
        {
            if (__instance.thePlantType == UltimateFreezingCherry.PlantID && __instance.anim.GetInteger("cureentStatus") != 0 && reason != Plant.DieReason.BySelf)
            {
                __instance.thePlantHealth = __instance.thePlantMaxHealth;
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(Plant.Crashed))]
        [HarmonyPrefix]
        public static bool PreCrashed(Plant __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType == UltimateFreezingCherry.PlantID && __instance.anim.GetInteger("cureentStatus") != 0)
            {
                __instance.thePlantHealth = __instance.thePlantMaxHealth;
                if (zombie != null)
                    zombie.Die();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(DroppedCard))]
    public static class DroppedCardPatch
    {
        [HarmonyPatch(nameof(DroppedCard.Update))]
        [HarmonyPrefix]
        public static void PreUpdate(DroppedCard __instance)
        {
            if (__instance.GetData<bool>("FreezingCherry_CreateByCherry") && Lawnf.TravelAdvanced(FreezingCherry.buff2))
            {
                if (__instance.existTime > 5f && __instance.existTime < 7.5f)
                    __instance.flashing = true;
                if (__instance.existTime + Time.deltaTime >= 7.5f && GameAPP.theGameStatus == GameStatus.InGame && Time.timeScale > 0f)
                {
                    Action<Zombie> action = (z) =>
                    {
                        z.SetFreeze(10f);
                        z.SetCold(15f);
                        int damage = 3600;
                        if (z.GetAttrTimers().freezeTimer > 0f)
                            damage *= 4;
                        z.TakeDamage(DmgType.NormalAll, damage, __instance.GetData<PlantType>("FreezingCherry_FromType"));
                    };
                    var bomb = CoreTools.CreateCherryExplode(__instance.transform.position, Mouse.Instance.GetRowFromY(__instance.transform.position.x, __instance.transform.position.y), CherryBombType.IceCharry,
                        3600, __instance.GetData<PlantType>("FreezingCherry_FromType"), action).Item2;
                    if (GameAPP.config.distablexplodeFlash)
                    {
                        bomb.GetComponent<ParticleSystem>().Simulate(0f, true);
                        bomb.GetComponent<ParticleSystemRenderer>().enabled = false;
                    }
                    __instance.board.boardAction.SetDoom(0, 0, false, true, __instance.transform.position - new Vector3(0f, 0.75f, 0f), 3600, existParticle: false, fromType: __instance.GetData<PlantType>("FreezingCherry_FromType"));
                    if (!GameAPP.config.distablexplodeFlash)
                        CreateParticle.SetParticle(FreezingCherry.ParticleID, __instance.transform.position, 11, true);
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Bullet_nuclear))]
    public static class Bullet_nuclearPatch
    {
        [HarmonyPatch(nameof(Bullet_nuclear.HitZombie))]
        [HarmonyPrefix]
        public static bool PreHitZombie(Bullet_nuclear __instance, ref Zombie zombie)
        {
            if (__instance.theBulletType == UltimateFreezingCherry.BulletID)
            {
                if (zombie != null)
                {
                    Action<Zombie> action = (z) =>
                    {
                        z.SetCold(10f);
                        z.AddfreezeLevel(25);
                        if (z.GetAttrTimers().freezeTimer > 0f)
                            z.TakeDamage(DmgType.NormalAll, __instance.Damage * 4, __instance.fromType);
                        else
                            z.TakeDamage(DmgType.NormalAll, __instance.Damage, __instance.fromType);
                    };
                    var cherry = CoreTools.CreateCherryExplode(__instance.transform.position, __instance.theBulletRow, UltimateFreezingCherry.CherrySmallID, __instance.Damage, action: action, volume: 0.2f).Item1;
                    cherry.bombRow = __instance.theBulletRow;
                    cherry.range = 1f;
                    cherry.fromType = __instance.fromType;
                    cherry.maxRow = 1;
                    __instance.board.boardAction.SetDoom(0, 0, false, true, __instance.transform.position, __instance.Damage, existParticle: false, fromType: __instance.fromType);
                    var position = __instance.transform.position;
                    position.y -= 0.75f;
                    if (!GameAPP.config.distablexplodeFlash)
                        CreateParticle.SetParticle(UltimateFreezingCherry.ParticleID, position, 11, true);
                    __instance.Die();
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(Bullet_nuclear.HitLand))]
        [HarmonyPrefix]
        public static bool PreLand(Bullet_nuclear __instance)
        {
            if (__instance.theBulletType == UltimateFreezingCherry.BulletID)
            {
                Action<Zombie> action = (z) =>
                {
                    z.SetCold(10f);
                    z.AddfreezeLevel(25);
                    if (z.GetAttrTimers().freezeTimer > 0f)
                        z.TakeDamage(DmgType.NormalAll, __instance.Damage * 4, __instance.fromType);
                    else
                        z.TakeDamage(DmgType.NormalAll, __instance.Damage, __instance.fromType);
                };
                var cherry = CoreTools.CreateCherryExplode(__instance.transform.position, __instance.theBulletRow, UltimateFreezingCherry.CherrySmallID, __instance.Damage, action: action, volume: 0.2f).Item1;
                cherry.bombRow = __instance.theBulletRow;
                cherry.range = 1f;
                cherry.fromType = __instance.fromType;
                cherry.maxRow = 1;
                cherry.zombieAction = action;
                __instance.board.boardAction.SetDoom(0, 0, false, true, __instance.transform.position, __instance.Damage, existParticle: false, fromType: __instance.fromType);
                var position = __instance.transform.position;
                position.y -= 0.75f;
                if (!GameAPP.config.distablexplodeFlash)
                    CreateParticle.SetParticle(UltimateFreezingCherry.ParticleID, position, 11, true);
                __instance.Die();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Mouse), nameof(Mouse.GetPlantsOnMouse))]
    public static class MousePatch
    {
        [HarmonyPostfix]
        public static void Postfix(Il2CppSystem.Collections.Generic.List<Plant> __result)
        {
            for (int i = __result.Count - 1; i >= 0; i--)
            {
                var plant = __result[i];
                if (plant == null) return;
                if (plant.thePlantType == UltimateFreezingCherry.PlantID && plant.anim.GetInteger("cureentStatus") != 0)
                    __result.Remove(plant);
            }
        }
    }
}