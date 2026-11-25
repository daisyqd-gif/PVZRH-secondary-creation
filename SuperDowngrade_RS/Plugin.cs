using BepInEx;
using CustomPlantClass.CustomizeLib;
using CustomPlantClass.RogueShootingManager;
using HarmonyLib;
using System;
using CustomPlantClass.Main;
using Unity.VisualScripting;
using Il2CppInterop.Runtime;
using System.Runtime.InteropServices;
using BepInEx.Unity.IL2CPP.Hook;
using Il2CppInterop.Runtime.Runtime;
using GameLevel.RogueShooting;
using UI;
using Core;
using System.Threading.Tasks;
using UnityEngine;
using CustomPlantClass.Runtime.Tasks;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using System.Collections.Generic;
namespace SuperDowngrade_RS
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        internal static Lazy<(TravelDebuff low, TravelDebuff high, TravelDebuff diamond, TravelDebuff silver)> Buffs = new(() =>
        {
            var result = ((TravelDebuff)10036, (TravelDebuff)10037, (TravelDebuff)10038, (TravelDebuff)10040);
            foreach (var val in Il2CppSystem.Enum.GetValues(Il2CppType.From(typeof(TravelDebuff))))
            {
                if (val == null) continue;
                if (val.ToString() == "Shooting_廉价审美") result.Item1 = val.Unbox<TravelDebuff>();
                else if (val.ToString() == "Shooting_高贵审美") result.Item2 = val.Unbox<TravelDebuff>();
                else if (val.ToString() == "Shooting_点钻成金") result.Item3 = val.Unbox<TravelDebuff>();
                else if (val.ToString() == "Shooting_白银时代") result.Item4 = val.Unbox<TravelDebuff>();
            }
            return result;
        });

        internal static Lazy<AdvBuff> polarization = new(() =>
        {
            var result = (AdvBuff)13101;
            foreach (var val in Il2CppSystem.Enum.GetValues(Il2CppType.Of<TravelDebuff>()))
            {
                if (val == null) continue;
                if (val.ToString() == "Shooting_两极分化") result = val.Unbox<AdvBuff>();
            }
            return result;
        });

        internal static bool IsSuperQualitative => ShootingManager.Instance != null && ShootingManager.Instance.superUpgrade;
        public override void InitializeMod()
        {
            RegistryHelper.AddCustomCurseMissionBuff("步步塌方","所有词条一定是随机品质，且随机范围扩大，受两极分化等词条影响",mgr =>
            {
                mgr.GetOrAddComponent<SuperDowngradeShooting>().downGrade = true;
            });

            DamageBuffQualityHook.ApplyHook();
            SpeedBuffQualityHook.ApplyHook();
        }

        public static class DamageBuffQualityHook
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void DamageBuffConstruct(IntPtr @this, int type, IntPtr method);
            private static DamageBuffConstruct Origin = null!;
            public static INativeDetour Detour = null!;

            public static unsafe void ApplyHook()
            {
                var cls = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "GameLevel.RogueShooting", "DamageBuff");
                IL2CPP.il2cpp_init(cls);
                var method = IL2CPP.GetIl2CppMethod(cls, false, ".ctor", "System.Void", IL2CPP.RenderTypeName<PlantType>());
                var strc = UnityVersionHandler.Wrap((Il2CppMethodInfo*)method);
                Detour = INativeDetour.CreateAndApply(strc.MethodPointer, OnDamageBuffConstructHook, out Origin);
            }

            public static void OnDamageBuffConstructHook(IntPtr instance, int type, IntPtr method)
            {
                Origin.Invoke(instance, type, method);
                var @this = new DamageBuff(instance);
                if (ShootingManager.Instance != null && ShootingManager.Instance.GetOrAddComponent<SuperDowngradeShooting>().downGrade)
                    @this.randomQuality = Quality.random;
            }
        }

        public static class SpeedBuffQualityHook
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void SpeedBuffConstruct(IntPtr @this, int type, IntPtr method);
            private static SpeedBuffConstruct Origin = null!;
            public static INativeDetour Detour = null!;

            public static unsafe void ApplyHook()
            {
                var cls = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "GameLevel.RogueShooting", "SpeedBuff");
                IL2CPP.il2cpp_init(cls);
                var method = IL2CPP.GetIl2CppMethod(cls, false, ".ctor", "System.Void", IL2CPP.RenderTypeName<PlantType>());
                var strc = UnityVersionHandler.Wrap((Il2CppMethodInfo*)method);
                Detour = INativeDetour.CreateAndApply(strc.MethodPointer, OnSpeedBuffConstructHook, out Origin);
            }

            public static void OnSpeedBuffConstructHook(IntPtr instance, int type, IntPtr method)
            {
                Origin.Invoke(instance, type, method);
                var @this = new SpeedBuff(instance);
                if (ShootingManager.Instance != null && ShootingManager.Instance.GetOrAddComponent<SuperDowngradeShooting>().downGrade)
                    @this.randomQuality = Quality.random;
            }
        }
    }
    
    public class SuperDowngradeShooting : MonoBehaviour
    {
        public bool downGrade
        {
            get
            {
                if(Plugin.IsSuperQualitative) return false;
                return field;
            }
            set => field = value;
        }
        public int combo = 0;
        public bool first = false;

        public static SuperDowngradeShooting GetInstance()
        {
            return ShootingManager.Instance.GetOrAddComponent<SuperDowngradeShooting>();
        }

        public void Awake()
        {
            first = true;
        }

        public static async Task RecreateMenu(string newText)
        {
            await DelayTask.WaitForFixedUpdate();
            var comp = ShootingManager.Instance.GetOrAddComponent<SuperDowngradeShooting>();
            if (comp.combo < 3 && comp.downGrade)
            {
                if (Random.value <= 0.5f)
                {
                    var menu = GameAPP.UIManager.Push(UIType.MultipleChoiceMenu2).GetComponent<MultipleChoiceMenu>();
                    ShootingManager.Instance.RegisterCoreBuff(menu);
                    Il2CppSystem.Collections.Generic.List<MultipleChoiceMenu.OptionData> options = new();
                    foreach (var option in menu.optionDatas)
                    {
                        if(
                            option.title.Contains("强化：速度") || 
                            option.title.Contains("强化：力量") || 
                            option.title.Contains("幸运") || 
                            option.title.Contains("合理投资") || 
                            option.title == "保护" || 
                            option.title.Contains("伤害增幅") || 
                            option.title.Contains("复活")
                        )
                            options.Add(option);
                    }
                    menu.SetOrdered(true);
                    menu.ClearOptions();
                    foreach (var option in options)
                        menu.RegisterOption(option.title, option.text, option.call, option.thePlantType, option.theZombieType, Quality.random, option.interactable);
                    menu.RegisterWindow(options.Count.AtMost(5));
                    menu.SetCancelable(false);
                    menu.SetRefreshable(false, enableTurnPage: true);
                    menu.Start();
                    menu.KeySelect = true;
                    comp.combo++;
                    InGameText.Instance.ShowText(newText, 3f, true);
                }
            }
            else
                comp.combo = 0;
        }
    }
    [HarmonyPatch(typeof(DamageBuff))]
    public static class DamageBuffPatch
    {
        [HarmonyPatch(nameof(DamageBuff.OnGet))]
        [HarmonyPrefix]
        public static bool PreOnGet(DamageBuff __instance)
        {
            if (ShootingManager.Instance != null && ShootingManager.Instance.GetOrAddComponent<SuperDowngradeShooting>().downGrade)// &&
            //    __instance.randomQuality == Quality.random)
            {
                float min = -1f / 2f;
                float max = 2.3f / 2f;

                if (Lawnf.TravelDebuff(Plugin.Buffs.Value.low))
                {
                    min *= 0.7f;
                    max *= 0.7f;
                }
                if (Lawnf.TravelDebuff(Plugin.Buffs.Value.high))
                {
                    min *= 1.1f;
                    max *= 1.1f;
                }

                // 点钻成金: 上限-0.3
                if (Lawnf.TravelDebuff(Plugin.Buffs.Value.diamond))
                    max -= 0.3f / 2f;

                if (ShootingManager.Instance.superUpgrade) max = 7.5f;

                min *= 2f;
                max *= 2f;
                var val = Random.Range(min, max);
                var ori = val;
                // 两极分化: 增加0.1~0.5或-0.1~-0.5
                if (Lawnf.TravelAdvanced(Plugin.polarization.Value))
                {
                    var offset = Random.Range(0.1f, 0.5f);
                    val += Random.Range(0, 2) == 0 ? offset : -offset;
                }

                // 白银时代: 加成x0.9
                if (Lawnf.TravelDebuff(Plugin.Buffs.Value.silver))
                    val *= 0.9f;

                // 第一抽给必定>=0保底
                if (SuperDowngradeShooting.GetInstance().first)
                {
                    if (val < 0)
                    {
                        val = -val;
                        ori = -ori;
                    }
                    SuperDowngradeShooting.GetInstance().first = false;
                }

                var show = $"获得了{(val * 100f):F0}%力量增幅 (词条影响：{(val - ori) * 100:F0}%)";
                _ = SuperDowngradeShooting.RecreateMenu(show);
                InGameText.Instance.ShowText(show, 3f, true);

                TravelMgr.Instance.data.AddDamage(__instance.plantType, val);

                return false;
            }
            return true;
        }
    }



    [HarmonyPatch(typeof(SpeedBuff))]
    public static class SpeedBuffPatch
    {
        [HarmonyPatch(nameof(SpeedBuff.OnGet))]
        [HarmonyPrefix]
        public static bool PreOnGet(SpeedBuff __instance)
        {
            if (ShootingManager.Instance != null && ShootingManager.Instance.GetOrAddComponent<SuperDowngradeShooting>().downGrade)// &&
            //    __instance.randomQuality == Quality.random)
            {
                float min = -0.8f / 2f;
                float max = 1.7f / 2f;

                if (Lawnf.TravelDebuff(Plugin.Buffs.Value.low))
                {
                    min *= 0.7f;
                    max *= 0.7f;
                }
                if (Lawnf.TravelDebuff(Plugin.Buffs.Value.high))
                {
                    min *= 1.1f;
                    max *= 1.1f;
                }

                // 点钻成金: 上限-0.2
                if (Lawnf.TravelDebuff(Plugin.Buffs.Value.diamond))
                    max -= 0.2f / 2f;

                if (ShootingManager.Instance.superUpgrade) max = 5f;

                min *= 2f;
                max *= 2f;
                var val = Random.Range(min, max);
                var ori = val;
                // 两极分化: 增加0.08~0.4或-0.08~-0.4
                if (Lawnf.TravelAdvanced(Plugin.polarization.Value))
                {
                    var offset = Random.Range(0.08f, 0.4f);
                    val += Random.Range(0, 2) == 0 ? offset : -offset;
                }
                
                // 白银时代: 加成x0.9
                if (Lawnf.TravelDebuff(Plugin.Buffs.Value.silver))
                    val *= 0.9f;

                // 第一抽给必定>=0保底
                if (SuperDowngradeShooting.GetInstance().first)
                {
                    if (val < 0)
                    {
                        val = -val;
                        ori = -ori;
                    }
                    SuperDowngradeShooting.GetInstance().first = false;
                }

                var show = $"获得了{val * 100f:F0}%速度增幅 (词条影响：{(val - ori) * 100:F0}%)";
                _ = SuperDowngradeShooting.RecreateMenu(show);
                InGameText.Instance.ShowText(show, 3f, true);

                TravelMgr.Instance.data.AddSpeed(__instance.plantType, val);
                if (ShootingManager.Instance.TryGetPlant(__instance.plantType, out var plant))
                    plant.AddSpeed(val);

                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(MultipleChoiceMenu))]
    public static class MultipleChoiceMenu_Patch
    {
        [HarmonyPatch(nameof(MultipleChoiceMenu.RegisterOption))]
        [HarmonyPrefix]
        public static void RegisterOption_Prefix(MultipleChoiceMenu __instance, string title, ref string text, ref UnityAction call, ref Quality frameType)
        {
            if (ShootingManager.Instance != null && ShootingManager.Instance.GetOrAddComponent<SuperDowngradeShooting>().downGrade)// &&
            //    __instance.randomQuality == Quality.random)
            {
                if(title == "幸运" || title == "合理投资" || title == "保护" || title == "伤害增幅" || title == "复活")
                {
                    frameType = Quality.random;
                    float minimum = 0;
                    float maximum = 0;
                    float val = 0;
                    var show = "";
                    Action a = () => {};
                    switch (title)
                    {
                        case "幸运": 
                            minimum = -25;
                            maximum = Plugin.polarization.Value.IsActive() ? 250 : 125;
                            val = Random.Range(minimum,maximum);
                            a = () =>
                            {
                                ShootingManager.Instance._lucky += val / 100f;
                                show = $"幸运提高{val:F0}，幸运可以提高好词条出现概率";
                                _ = SuperDowngradeShooting.RecreateMenu(show);
                                InGameText.Instance.ShowText(show,3f,true);
                            };
                            text = "幸运提高随机数，幸运可以提高好词条出现概率";
                            break;
                        case "合理投资": 
                            minimum = 0;
                            maximum = Plugin.polarization.Value.IsActive() ? 10 : 5;
                            val = Random.Range(minimum,maximum);
                            a = () =>
                            {
                                ShootingManager.Instance.refreshCount += (int)val;
                                show = $"获得{(int)val}次词条刷新的机会";
                                _ = SuperDowngradeShooting.RecreateMenu(show);
                                InGameText.Instance.ShowText(show,3f,true);
                            };
                            text = "获得随机次词条刷新的机会";
                            break;
                        case "保护": 
                            minimum = 0;
                            maximum = Plugin.polarization.Value.IsActive() ? 50000 : 25000;
                            val = Random.Range(minimum,maximum);
                            a = () =>
                            {
				                var allPlants = Lawnf.GetAllPlants();
                                Action<Plant> a = p => p.theShieldHealth += (int)val;
                                allPlants.ForEach(a);
                                ShootingManager.Instance.shieldHealth += val;
                                show = $"全体植物获得{(int)val}护盾，复活后依然生效";
                                _ = SuperDowngradeShooting.RecreateMenu(show);
                                InGameText.Instance.ShowText(show,3f,true);
                            };
                            text = "全体植物获得随机护盾，复活后依然生效";
                            break;
                        case "伤害增幅": 
                            minimum = -10;
                            maximum = Plugin.polarization.Value.IsActive() ? 100 : 50;
                            val = Random.Range(minimum,maximum);
                            a = () =>
                            {
				                TravelMgr.Instance.damageAmplification += val;
                                show = $"全体植物获得{val:F0}%伤害增幅";
                                _ = SuperDowngradeShooting.RecreateMenu(show);
                                InGameText.Instance.ShowText(show,3f,true);
                            };
                            text = "全体植物获得随机伤害增幅";
                            break;
                        case "复活": 
                            minimum = 0;
                            maximum = Plugin.polarization.Value.IsActive() ? 99 : 50;
                            val = Random.Range(minimum,maximum);
                            a = () =>
                            {
				                ShootingManager.Instance.reviveTimer *= 1-val;
                                show = $"复活时间降低{val:F0}%";
                                _ = SuperDowngradeShooting.RecreateMenu(show);
                                InGameText.Instance.ShowText(show,3f,true);
                            };
                            text = "复活时间降低随机";
                            break;
                    }
                    call = a;
                }
                /*
                if(title.Contains("幸运") || title.Contains("合理投资") || title.Contains("保护") || title.Contains("伤害增幅") || title.Contains("复活"))
                {
                    int minimum = 0;
                    int maximum = 0;
                    int val = 0;
                    var show = "";
                    Action a = () => {};
                    if (title.Contains("幸运"))
                    {
                        minimum = -25;
                        maximum = Plugin.polarization.Value.IsActive() ? 250 : 125;
                        val = Random.Range(minimum,maximum);
                        a = () =>
                        {
                            ShootingManager.Instance._lucky += val / 100;
                            show = $"幸运提高{0:F0}，幸运可以提高好词条出现概率";
                            _ = SuperDowngradeShooting.RecreateMenu(show);
                            InGameText.Instance.ShowText(show,3f,true);
                        };
                        text = "幸运提高随机数，幸运可以提高好词条出现概率";
                    }
                    else if (title.Contains("合理投资"))
                    {
                        minimum = 0;
                        maximum = Plugin.polarization.Value.IsActive() ? 10 : 5;
                        val = Random.Range(minimum,maximum);
                        a = () =>
                        {
                            ShootingManager.Instance.refreshCount += val;
                            show = $"获得{val:F0}次词条刷新的机会";
                            _ = SuperDowngradeShooting.RecreateMenu(show);
                            InGameText.Instance.ShowText(show,3f,true);
                        };
                        text = "获得随机次词条刷新的机会";
                    }
                    else if (title.Contains("保护"))
                    {
                        minimum = 0;
                        maximum = Plugin.polarization.Value.IsActive() ? 50000 : 25000;
                        val = Random.Range(minimum,maximum);
                        a = () =>
                        {
                            var allPlants = Lawnf.GetAllPlants();
                            Action<Plant> a = p => p.theShieldHealth += val;
                            allPlants.ForEach(a);
                            ShootingManager.Instance.shieldHealth += val;
                            show = $"全体植物获得{val:F0}护盾，复活后依然生效";
                            _ = SuperDowngradeShooting.RecreateMenu(show);
                            InGameText.Instance.ShowText(show,3f,true);
                        };
                        text = "全体植物获得随机护盾，复活后依然生效";
                    }
                    else if (title.Contains("伤害增幅"))
                    {
                        minimum = -10;
                        maximum = Plugin.polarization.Value.IsActive() ? 100 : 50;
                        val = Random.Range(minimum,maximum);
                        a = () =>
                        {
                            TravelMgr.Instance.damageAmplification += val;
                            show = $"全体植物获得{val:F0}%伤害增幅";
                            _ = SuperDowngradeShooting.RecreateMenu(show);
                            InGameText.Instance.ShowText(show,3f,true);
                        };
                        text = "全体植物获得随机伤害增幅";
                    }
                    else if (title.Contains("复活"))
                    {
                        minimum = 0;
                        maximum = Plugin.polarization.Value.IsActive() ? 99 : 50;
                        val = Random.Range(minimum,maximum);
                        a = () =>
                        {
                            ShootingManager.Instance.reviveTimer *= 1-val;
                            show = $"复活时间降低{val:F0}%";
                            _ = SuperDowngradeShooting.RecreateMenu(show);
                            InGameText.Instance.ShowText(show,3f,true);
                        };
                        text = "复活时间降低随机";
                    }
                    call = a;
                }
                */
            }
        }
    }
    
    public class MyPluginInfo
    {
        public const string PluginGuid = "SuperDowngrade_RS.Bepinex";
        public const string PluginName = "SuperDowngrade_RS";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
