using System;
using System.Reflection;
using BepInEx;
using CustomPlantClass.CustomizeLib;
using CustomPlantClass.Main;
using CustomPlantClass.Runtime.Core;
using UnityEngine;

namespace SniperBlover.BepInEx
{
	// Token: 0x02000003 RID: 3
	[BepInPlugin("SniperBlover.BepInEx", "SniperBlover", "4.0")]	public class Plugin : ModPlugin
	{
		// Token: 0x06000008 RID: 8 RVA: 0x000021A8 File Offset: 0x000003A8
		public Plugin()
		{
			AssetBundle = null;
			PlantID = new ID(0);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021C8 File Offset: 0x000003C8
		public override void InitializeMod()
		{
			AssetBundle = AssetMgr.LoadBundleFromResource(Assembly.GetExecutingAssembly(), "sniperblover", false);
			PlantID = DataMgr.AllocateID();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021EC File Offset: 0x000003EC
		public override void InitializePlants()
		{
            BaseCustomPlantData plantdata = new()
            {
                PlantId = PlantID,
                Prefab = AssetBundle.GetAsset<GameObject>("SniperBloverPrefab"),
                Preview = AssetBundle.GetAsset<GameObject>("SniperBloverPreview"),
                Fusions = ListHelper.MirrorTuple(new ValueTuple<ID, ID>(PlantType.SniperPea, PlantType.Blover)),
                AttackInterval = 3f,
                ProduceInterval = 0f,
                AttackDamage = 500,
                MaxHealth = 300,
                Cd = 3f,
                Sun = 700,
                DefaultBullet = BulletType.Bullet_pea,
                CanPF = false,
                CanStarUp = false,
                CardColor = CardLevel.Purple,
                IsRainbowCard = false,
                IsUltimatePlant = true,
                CardRepeatAmt = 1,
                Name = "浮空狙击射手",
                AlmanacEntry = DataMgr.CreateAlmanacEntry("定期狙击一只僵尸，造成高额伤害。", "removeifthisisdefaulted", new ValueTuple<string, string>("狙击射手", "三叶草"), new ValueTuple<int, float>(500, 3f), default(ValueTuple<int, float, string>), new string[] { "攻击方式：锁定最近的目标，直至其死亡。每第6次攻击爆头，造成100万伤害", "可以消耗机枪射手植物，生成狙击射手卡牌", "下方植物攻击间隔低于3秒时，自身攻速改为与其一致" }, new ValueTuple<string, string>("铲除", "三叶草"), "removeifthisisdefaulted", "removeifthisisdefaulted", "removeifthisisdefaulted", "removeifthisisdefaulted")
            };
            ID id = DataMgr.RegisterCustomPlant<SniperPea, SniperBlover>(plantdata);
			CustomCore.TypeMgrExtra.FlyingPlants.Add(PlantID);
			ModLogger.LogInfo("SniperBlover.BepInEx", "SniperBlover" + " " + "4.0" + " loaded successfully.");
		}
		internal AssetBundle AssetBundle;
		internal ID PlantID;
	}
}
