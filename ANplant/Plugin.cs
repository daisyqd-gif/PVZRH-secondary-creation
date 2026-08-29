using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;


namespace ANPlant{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "ANPlant.Bepinex";
        public const string PluginName = "ANPlant";
        public const string PluginVersion = "3.4.1";
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            ClassInjector.RegisterTypeInIl2Cpp<SkillCard>();
            ClassInjector.RegisterTypeInIl2Cpp<OperatorBase>();

            AssetBundle assetBundle = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "anplant");

            CustomCore.RegisterCustomPlant<Plant, SkillCard>(
                SkillCard.PLANT_ID1,
                assetBundle.GetAsset<GameObject>("SPrefab"),
                assetBundle.GetAsset<GameObject>("S1preview"),
                new List<(int, int)> {},
                0f,
                0f,
                0,
                300,
                0f,
                0
            );
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(SkillCard.PLANT_ID1);
            CustomCore.TypeMgrExtra.LevelPlants.Add (SkillCard.PLANT_ID1, CardLevel.White);
            CustomCore.AddPlantAlmanacStrings(
                SkillCard.PLANT_ID1,
                $"Skill Card 1 ({SkillCard.PLANT_ID1})",
                "Used to trigger skill 1 on select plants."
            );

            CustomCore.RegisterCustomPlant<Plant, SkillCard>(
                SkillCard.PLANT_ID2,
                assetBundle.GetAsset<GameObject>("SPrefab"),
                assetBundle.GetAsset<GameObject>("S2preview"),
                new List<(int, int)> {},
                0f,
                0f,
                0,
                300,
                0f,
                0
            );
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(SkillCard.PLANT_ID2);
            CustomCore.TypeMgrExtra.LevelPlants.Add (SkillCard.PLANT_ID2, CardLevel.White);
            CustomCore.AddPlantAlmanacStrings(
                SkillCard.PLANT_ID2,
                $"Skill Card 2 ({SkillCard.PLANT_ID2})",
                "Used to trigger skill 2 on select plants."
            );

            CustomCore.RegisterCustomPlant<Plant, SkillCard>(
                SkillCard.PLANT_ID3,
                assetBundle.GetAsset<GameObject>("SPrefab"),
                assetBundle.GetAsset<GameObject>("S1preview"),
                new List<(int, int)> {},
                0f,
                0f,
                0,
                300,
                0f,
                0
            );
            CustomCore.TypeMgrExtra.IsCustomPlant.Add(SkillCard.PLANT_ID3);
            CustomCore.TypeMgrExtra.LevelPlants.Add (SkillCard.PLANT_ID3, CardLevel.White);
            CustomCore.AddPlantAlmanacStrings(
                SkillCard.PLANT_ID3,
                $"Skill Card 3 ({SkillCard.PLANT_ID3})",
                "Used to trigger skill 3 on select plants."
            );

            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    public class SkillCard : MonoBehaviour
    {
        public static ID PLANT_ID1=3101;
        public static ID PLANT_ID2=3102;
        public static ID PLANT_ID3=3103;
        public Plant plant => gameObject.GetComponent<Plant>();
        public void Start()
        {
            if(GameAPP.theGameStatus!=GameStatus.InGame || plant.board==null) return;
            Lawnf.SetDroppedCard(new Vector2(plant.transform.position.x,plant.transform.position.y+1.5f),plant.thePlantType);
            plant.Die();
        }
    }
    public abstract class OperatorBase : MonoBehaviour
    {
        // Abstract cooldown definitions
        public abstract int SkillCooldown_1 { get; }
        public abstract int SkillCooldown_2 { get; }
        public abstract int SkillCooldown_3 { get; }

        // Runtime cooldown counters
        public int SkillCooldown1 = 0;
        public int SkillCooldown2 = 0;
        public int SkillCooldown3 = 0;

        //Util variables
        private float timer = 0f;
        public abstract int SkillCount { get; }

        //Skill card tracking
        public DroppedCard SkillCard1;
        public DroppedCard SkillCard2;
        public DroppedCard SkillCard3;

        //Active skill
        public bool Skill1Active = false;
        public bool Skill2Active = false;
        public bool Skill3Active = false;

        //Ready skill
        public bool Skill1Ready = false;
        public bool Skill2Ready = false;
        public bool Skill3Ready = false;

        public void Update()
        {
            timer += Time.deltaTime;

            // Tick cooldowns once per second
            if (timer >= 1f)
            {
                timer -= 1f;

                // Skill 1
                if (SkillCooldown1 > 0)
                {
                    SkillCooldown1--;
                    if (SkillCooldown1 == 0)
                    {
                        Skill1Ready = true;
                        DropSkillCard(1);
                    }
                }

                // Skill 2
                if (SkillCooldown2 > 0 && SkillCount >= 2)
                {
                    SkillCooldown2--;
                    if (SkillCooldown2 == 0)
                    {
                        Skill2Ready = true;
                        DropSkillCard(2);
                    }
                }

                // Skill 3
                if (SkillCooldown3 > 0 && SkillCount >= 3)
                {
                    SkillCooldown3--;
                    if (SkillCooldown3 == 0)
                    {
                        Skill3Ready = true;
                        DropSkillCard(3);
                    }
                }
            }

            // Auto-respawn only if skill is NOT active
            if (Skill1Ready && SkillCard1 == null && !Skill1Active)
                DropSkillCard(1);

            if (Skill2Ready && SkillCount >= 2 && SkillCard2 == null && !Skill2Active)
                DropSkillCard(2);

            if (Skill3Ready && SkillCount >= 3 && SkillCard3 == null && !Skill3Active)
                DropSkillCard(3);
        }

        public void DropSkillCard(int skillIndex)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y + 1.5f);

            switch (skillIndex)
            {
                case 1:
                    SkillCard1 = Lawnf.SetDroppedCard(pos, SkillCard.PLANT_ID1, 0);
                    break;

                case 2:
                    if (SkillCount >= 2)
                        SkillCard2 = Lawnf.SetDroppedCard(pos, SkillCard.PLANT_ID2, 0);
                    break;

                case 3:
                    if (SkillCount >= 3)
                        SkillCard3 = Lawnf.SetDroppedCard(pos, SkillCard.PLANT_ID3, 0);
                    break;
            }
        }

        public void TriggerSkill(int skillIndex)
        {
            switch (skillIndex)
            {
                case 1:
                    Skill1Ready = false;
                    Skill1Active = true;
                    OnSkillActivate1();
                    SkillCooldown1 = SkillCooldown_1;
                    SkillCard1 = null;
                    break;

                case 2:
                    if (SkillCount >= 2)
                    {
                        Skill2Ready = false;
                        Skill2Active = true;
                        OnSkillActivate2();
                        SkillCooldown2 = SkillCooldown_2;
                        SkillCard2 = null;
                    }
                    break;

                case 3:
                    if (SkillCount >= 3)
                    {
                        Skill3Ready = false;
                        Skill3Active = true;
                        OnSkillActivate3();
                        SkillCooldown3 = SkillCooldown_3;
                        SkillCard3 = null;
                    }
                    break;
            }
        }

        public abstract void OnSkillActivate1();
        public abstract void OnSkillActivate2();
        public abstract void OnSkillActivate3();
    }
}
