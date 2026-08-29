using BepInEx;
using CustomizeLib.BepInEx;
using HarmonyLib;
using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

namespace GargantuarHypnoEmperor
{
    [BepInPlugin("GargantuarHypnoEmperor.Bepinex", "GargantuarHypnoEmperor", CustomPlantClass.MyPluginInfo.TargetVersion)]
    public class Core : CorePlugin
    {
        public override void OnStart()
        {
            Harmony.CreateAndPatchAll(Tools.GetAssembly());

            var ab = CustomCore.GetAssetBundle(Tools.GetAssembly(), "gargantuarhypnoemperor");;
            CustomCore.RegisterCustomPlant<Plant, GargantuarHypnoEmperor>(
                GargantuarHypnoEmperor.PlantID,
                ab.GetAsset<GameObject>("HypnoShroomPrefab"),
                ab.GetAsset<GameObject>("HypnoEmperorPreview"),
                new List<(int, int)>
                {
                    ((int)PlantType.HypnoDoom, (int)PlantType.HypnoDoom),
                    ((int)PlantType.SuperHypnoDoom, (int)PlantType.HypnoShroom),
                    ((int)PlantType.HypnoShroom, (int)PlantType.SuperHypnoDoom)
                },
                1f, 0f, 300, 300, 7.5f, 600
            );

            CustomCore.RegisterCustomZombie<Gargantuar, BlackFootballGargantuar>(
                BlackFootballGargantuar.PlantID,
                ab.GetAsset<GameObject>("FootballGargantuar"),
                new Sprite(), 3600, 18000, 0, 0
            );
            CustomCore.AddPlantAlmanacStrings(
                GargantuarHypnoEmperor.PlantID,
                "鬼影魅惑菇",
                AlmanacText.HypnoEmperor
            );
            CustomCore.AddZombieAlmanacStrings(
                BlackFootballGargantuar.PlantID,
                "冲锋黑橄榄红眼巨人僵尸",
                AlmanacText.BlackFootballGarg
            );

            CustomCore.AddUltimatePlant(GargantuarHypnoEmperor.PlantID);
            CustomCore.TypeMgrExtra.IsNut.Add(BlackFootballGargantuar.PlantID);
            CustomCore.TypeMgrExtra.BigZombie.Add(BlackFootballGargantuar.PlantID);
            CustomCore.TypeMgrExtra.UltimateZombie.Add(BlackFootballGargantuar.PlantID);
        }
        public static class AlmanacText
        {
            public static readonly string HypnoEmperor =
                "Summons a gargantuar and heals it.\n" +
                "召唤为你而战的巨人大军！\n" +
                "\n" +
                "关于鬼影魅惑菇，大家曾经只知道她是游乐场鬼屋的老板…" +
                "直到她带着自己的巨人打手走上草坪、与僵尸从容对峙。\n" +
                "\n" +
                "“什么黑帮？我只是缺乏安全感所以招了几个保镖。”" +
                "鬼影魅惑菇慵懒的声音从面罩下传出，" +
                "但这次没人觉得她只有表面上那么简单了。";

            public static readonly string BlackFootballGarg =
                "Moves fast and has high health.\n" +
                "冲锋伽刚特尔眼红了，现在黑色是他的最爱。\n" +
                "\n" +
                "Health : 180000\n" +
                "Speed : Fast\n" +
                "\n" +
                "“黑色巨人球队”是僵尸界享有盛名的全明星球队。" +
                "有导演准备为他们拍摄一部宣传片，" +
                "小道消息流出的片名为《黑橄榄：巨人》。";
        }
    }

    public class GargantuarHypnoEmperor : MonoBehaviour
    {
        public static ID PlantID = 3095;
        public Zombie zombie = null;
        public GameObject radiationPrefab;
        public Radiation radiation;

        public Plant plant => gameObject.GetComponent<Plant>();
        public void FixedUpdate()
        {
            if(plant.board==null || GameAPP.theGameStatus!=GameStatus.InGame || Board.Instance==null) return;
            if (plant.starUp)
            {
                if (radiation == null)
                {
                    InstantiateRadiation();
                }
            }
        }
        public void TrySummon()
        {
            if(plant.board==null || GameAPP.theGameStatus!=GameStatus.InGame || Board.Instance==null) return;
            if (zombie == null)
            {
                plant.anim.SetTriggerString("summon");
            }
            else if(!zombie.TryGetComponent<Zombie>(out var z))
            {
               plant.anim.SetTriggerString("summon");
            }
            else if(!z.isMindControlled || z.IsDestroyed() || z.theStatus==ZombieStatus.Dying)
            {
                plant.anim.SetTriggerString("summon");
            }
            else
            {
                plant.anim.SetTriggerString("healZ");
            }
        }
        public void AnimSummon_Custom()
        {
            if(plant.board==null || GameAPP.theGameStatus!=GameStatus.InGame || Board.Instance==null) return;
            if(Random.Range(0,100)<5) SuperSkill_Custom();
            zombie=CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow,BlackFootballGargantuar.PlantID,Mouse.Instance.GetBoxXFromColumn(plant.thePlantColumn),false);
            if (plant.starUp)
            {
                Action<Zombie> a = (Zombie z)=>{z.SetMindControl(1);};
                plant.board.boardAction.SetDoom(plant.thePlantColumn,plant.thePlantRow,false,false,default,3600,0,a,true,plant.thePlantType);
            }
        }
        public void AnimHeal_Custom_Zombie()
        {
            if(plant.board==null || GameAPP.theGameStatus!=GameStatus.InGame || Board.Instance==null) return;
            if(zombie==null){}
            else if(!zombie.isMindControlled || zombie.IsDestroyed() || zombie.theStatus==ZombieStatus.Dying){}
            else
            {
                int oldMax = zombie.theMaxHealth;
                zombie.theMaxHealth = (int)(oldMax * 1.05);
                zombie.theHealth += zombie.theMaxHealth - oldMax;
                zombie.theOriginSpeed= (int)Math.Round(zombie.theOriginSpeed*1.1);
                zombie.UpdateHealthText();
                CreateParticle.SetParticle(16,zombie.transform.position,zombie.theZombieRow);
            }
        }
        public void InstantiateRadiation()
        {
            if(plant.board==null || GameAPP.theGameStatus!=GameStatus.InGame || Board.Instance==null) return;
            var origin = Radiation.radiation;
            Radiation.radiation = null;
            radiationPrefab=Resources.Load<GameObject>("plants/cherrybomb/nucleardoomcherry/Radiation");

            radiation = Instantiate(
                radiationPrefab,
                plant.axis.transform.position,
                Quaternion.identity,
                plant.transform
            ).GetComponent<Radiation>();

            Radiation.radiation = origin;

            radiation.transform.localScale = new Vector3(4f, 4f, 4f);
            radiation.GetComponent<ParticleSystem>().Simulate(0.02f, true);
            ORadiation r=radiation.GetOrAddComponent<ORadiation>();
            r.SetR(radiation);
            r.p=plant;
            Renderer renderer=radiation.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                if (renderer.material.HasProperty("_TintColor"))
                    renderer.material.SetColor("_TintColor", new Color(0f, 0f, 0f, 1f));
                if (renderer.material.HasProperty("_Color"))
                    renderer.material.SetColor("_Color", new Color(0f, 0f, 0f, 1f));
            }
        }
        public void SuperSkill_Custom()
        {
            if(plant.board==null || GameAPP.theGameStatus!=GameStatus.InGame || Board.Instance==null) return;
            for(int i = 0; i < plant.board.rowNum; i++)
            {
                CreateZombie.Instance.SetZombieWithMindControl(i,BlackFootballGargantuar.PlantID,Mouse.Instance.GetBoxXFromColumn(0),false);
            }
            if (plant.starUp)
            {
                var list = Lawnf.GetAllZombies();
                int count = list.Count;

                var zombies = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Zombie>(count);
                list.CopyTo(zombies, 0);

                foreach (Zombie z in zombies)
                {
                    if (z == null) continue;
                    if (z.isMindControlled) continue;
                    if (TypeMgr.IsBossZombie(z.theZombieType)) continue;
                    if (TypeMgr.BigZombie(z.theZombieType)) continue;

                    // Spawn your custom mind‑controlled gargantuar
                    CreateZombie.Instance.SetZombieWithMindControl(
                        z.theZombieRow,
                        BlackFootballGargantuar.PlantID,
                        z.transform.position.x,
                        false
                    );

                    // Visual-only doom effect
                    z.board.boardAction.SetDoom(z.Column, z.theZombieRow, false, damage: 0, fromType: PlantID);

                    // Kill original zombie
                    z.Die(1);
                }
            }
        }
    }
    public class ORadiation : MonoBehaviour
    {
        public GameObject z;
        public Radiation r;
        public Plant p;
        public int timer=0;
        public void FixedUpdate()
        {
            if(z!=null)timer++;
            Disintegrate();
            if (r != null)
            {
                r.damage=400;
                r.lifeTimer=3;
            }
        }
        public void SetR(Radiation rad)
        {
            r=rad;
        }
        public void SetZ(GameObject zombie)
        {
            z=zombie;
        }
        public void Disintegrate()
        {
            if(z==null) return;
            Zombie zombie=z.GetComponent<Zombie>();
            if (timer >= 1000)
            {
                timer=0;
                Action<Zombie> action= (Zombie z1) =>
                {
                    ORadiation oRadiation=z1.gameObject.GetOrAddComponent<ORadiation>();
                    oRadiation.SetZ(z1.gameObject);
                    oRadiation.p.theShieldHealth+=1000;
                    oRadiation.p.UpdateText();
                };
                zombie.board.boardAction.CreateCherryExplode(z.transform.position,zombie.theZombieRow,CherryBombType.Normal,0,GargantuarHypnoEmperor.PlantID,action);
                CreateZombie.Instance.SetZombieWithMindControl(zombie.theZombieRow,BlackFootballGargantuar.PlantID,zombie.transform.position.x,true);
                zombie.Die(1);
            }
            p.theShieldHealth+=1000;
            p.UpdateText();
        }
        public void Die()
        {
            if(r!=null)r.Die();
            Disintegrate();
        }
    }
    public class BlackFootballGargantuar : MonoBehaviour
    {
        public static ID PlantID = 3095;
        public Zombie plant => gameObject.GetComponent<Zombie>(); //supposed to only be summoned
        public bool isSuper=false;
        public void Start()
        {
            if (plant != null)
            {
                plant.theOriginSpeed*=2;
            }
        }
        public void Update()
        {
            if (plant != null && plant.isMindControlled && plant.theHealth <= plant.theMaxHealth * 0.25f && !isSuper)
            {
                isSuper = true; // <-- prevents infinite stacking

                plant.UpdateColor(Zombie.ZombieColor.Lunar);

                plant.theMaxHealth *= 3;
                plant.theHealth = plant.theMaxHealth;

                plant.theOriginSpeed *= 3;
                plant.theAttackDamage *= 3;
            }
        }
    }
    // -----------------------------
    //  ZOMBIE: BodyTakeDamage patch
    // -----------------------------
    [HarmonyPatch(typeof(Gargantuar), nameof(Gargantuar.BodyTakeDamage))]
    public static class Patch_GargantuarBodyTakeDamage
    {
        static bool Prefix(Gargantuar __instance, int theDamage)
        {
            SafeBodyTakeDamage(__instance, theDamage);
            return false; // Skip original
        }

        private static void SafeBodyTakeDamage(Gargantuar g, int dmg)
        {
            // Apply damage
            g.theHealth -= dmg;

            int max = g.theMaxHealth;
            int hp  = g.theHealth;

            // -----------------------------
            //  Smash animation trigger
            //  (original logic preserved)
            // -----------------------------
            if (hp < max * 0.5f && g.theStatus == ZombieStatus.Gargantuar_withImp)
            {
                var axis = g.axis;
                if (axis != null)
                {
                    Vector3 pos = axis.position;

                    // Original condition: x > 3 and not mind‑controlled
                    if (pos.x > 3f && !g.isMindControlled)
                    {
                        var board = g.board;
                        if (board != null && !board.boardTag.isTowerDefence)
                        {
                            g.theStatus = 0;
                            if (g.anim != null)
                                g.anim.SetTrigger("Smash"); // StringLiteral_876
                        }
                    }
                }
            }

            // -----------------------------
            //  Low‑health threshold
            //  (original logic: return if hp >= max/3)
            // -----------------------------
            if (hp >= max / 3f)
                return;

            // -----------------------------
            //  No visual swaps here
            //  (original code toggled children 0–3)
            // -----------------------------
            // You can optionally trigger a generic animation:
            // g.AnimLoseActive();
        }
    }

    // -----------------------------
    //  GARGANTUAR: SetWeapon patch
    // -----------------------------
    [HarmonyPatch(typeof(Gargantuar), nameof(Gargantuar.SetWeapon))]
    public static class Patch_GargantuarSetWeapon
    {
        [HarmonyPrefix]
        static bool Prefix(Gargantuar __instance)
        {
            if(__instance.theZombieType==BlackFootballGargantuar.PlantID) return false;
            return true;
        }
    }
    [HarmonyPatch(typeof(Plant), nameof(Plant.StarUp))]
    public static class StarUp_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance)
        {
            if(__instance==null || __instance.board==null || GameAPP.theGameStatus!=GameStatus.InGame || __instance.thePlantType==PlantType.Nothing)return;

            // Only touch your plants; do NOT access board or other refs
            var type = __instance.thePlantType;
            if (type == GargantuarHypnoEmperor.PlantID)
            {
                __instance.thePlantMaxHealth=8000;
                __instance.thePlantHealth=8000;
                __instance.theShieldHealth=8000;
                __instance.starUp=true;
                __instance.UpdateStarIcon();
            }
        }
    }
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.PlayEatSound))]
    public static class Zombie_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Zombie __instance)
        {
            if (__instance != null && __instance.theAttackTarget != null && __instance.theAttackTarget.IsPlant(out var plant) &&
                plant != null && plant.thePlantType == GargantuarHypnoEmperor.PlantID)
            {
                __instance.SetMindControl();
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UltimateFootballZombie), nameof(UltimateFootballZombie.AttackEffect))]
    public static class UltimateFootballZombie_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Plant plant)
        {
            if (plant != null && plant.thePlantType == GargantuarHypnoEmperor.PlantID)
            {
                return false;
            }
            return true;
        }
    }
}