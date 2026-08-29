using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CustomPlantClass;
using CustomPlantClass.Main;

namespace Fireworks{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "Fireworks.Bepinex";
        public const string PluginName = "Fireworks";
        public const string PluginVersion = "3.7.0";
        public static AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "firework");
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            ClassInjector.RegisterTypeInIl2Cpp<Fireworks>();
            DataMgr.RegisterCustomPlant<CherryBomb, Fireworks>(
                Fireworks.Data
            );

            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    public class Fireworks : BaseCustomPlant
    {
        public static readonly BaseCustomPlantData Data=new BaseCustomPlantData()
        {
            PlantId=DataMgr.AllocateID(),
            Prefab=Core.assetBundle.GetAsset<GameObject>("CherryBombPrefab"),
            Preview=Core.assetBundle.GetAsset<GameObject>("CherryBombPreview"),
            Fusions=default,
            AttackInterval=0f,
            ProduceInterval=0f,
            AttackDamage=0,
            MaxHealth=300,
            Cd=60f,
            Sun=5000,
            DefaultBullet=BulletType.Bullet_pea,
            CanPF=false,
            CardColor=CardLevel.Blue,
            IsRainbowCard=true,
            CardRepeatAmt=255,
            Name="Firework",
            AlmanacEntry="Happy Lunar New Year 2026!"
        };
        public void Bomb_Custom()
        {
            Board b = Board.Instance;
            if (b == null)
            {
                Debug.Log("Null Detected!");
                return;
            }
            var zlist = Lawnf.GetAllZombies(false);
            List<PlantType> seedlist = new List<PlantType>();

            foreach (var type in GameAPP.resourcesManager.allPlants)
            {
                if (type == PlantType.Nothing ||
                    type == PlantType.MagnetBox ||
                    type == PlantType.MagnetInterface ||
                    type == PlantType.Pit ||
                    type == PlantType.Refrash ||
                    type == PlantType.Extract_single ||
                    type == PlantType.Extract_ten)
                    continue;

                if (!Lawnf.IsUltiPlant(type))
                    continue;

                seedlist.Add(type);
            }

            foreach (Zombie z in zlist)
            {
                Vector2 pos = z.transform.position;
                pos.y+=2f;
                Lawnf.SetDroppedCard(pos, seedlist[Random.Range(0, seedlist.Count)], 0);
                z.TakeDamage(DmgType.Carred,3600,_plant.thePlantType,false);
            }
        }

        public void Die_Custom()
        {
            Board b = Board.Instance;
            if (b == null)
            {
                Debug.Log("Null Detected!");
                return;
            }
            _plant.Die(Plant.DieReason.BySelf);
            for(int i = 0; i <= b.rowNum; i++)
            {
                var createplants = CreatePlant.Instance;
                if (createplants == null)
                    return;
                createplants.SetPlant(0, i, PlantType.CherryJalapeno, null, Vector2.zero, true, true, null);
            }
        }
    }
}
