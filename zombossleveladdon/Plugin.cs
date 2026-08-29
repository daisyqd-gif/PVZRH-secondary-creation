using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Reflection;
using CustomPlantClass;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using CustomizeLib.BepInEx;
using CustomPlantClass.Main;
using Random = UnityEngine.Random;
using Core;

namespace ZombossAddon
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BasePlugin
    {
        public const string PluginGuid = "ZombossAddon.Bepinex";
        public const string PluginName = "ZombossAddon";
        public const string PluginVersion = MyPluginInfo.TargetVersion;
        public static AssetBundle PresentImitaterAssets;
        public static AssetBundle HeiTaAssets;
        public static ID id_PresentImitater;
        public static ID id_HeiTa;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            DataMgr.AutoRegisterTypes();
            PresentImitaterAssets = AssetMgr.LoadBundleFromResource(Tools.GetAssembly(), "PresentImitater.BepInEx.presentimitater", false);
            HeiTaAssets = AssetMgr.LoadBundleFromResource(Tools.GetAssembly(), "heita.unity3d", false);
            id_PresentImitater = DataMgr.AllocateID();
            id_HeiTa = DataMgr.AllocateID();
            CustomCore.RegisterCustomPlant<Imitater>
            (
                id_PresentImitater,
                PresentImitaterAssets.GetAsset<GameObject>("PresentImitaterPrefab"),
                PresentImitaterAssets.GetAsset<GameObject>("PresentImitaterPreview"),
                new([(245, 256), (256, 245)]),
                0f, 0f, 0, 300, 60f, 300
            );
            CustomCore.AddPlantAlmanacStrings
            (
                id_PresentImitater,
                $"礼盒模仿者({id_PresentImitater})",
                "礼盒模仿者\n\n" +
                "<color=#3D1400>作者：梧萱梦汐X、橙源是咸鱼</color>\n\n" +
                "<color=#3D1400>特点：</color>\n<color=red>①模仿植物后消失，掉落融合过程中带有该植物的上位植物的卡片</color>\n" +
                "<color=red>②若该植物没有上位植物，则掉落该植物自身，并使下一次种植该植物返还75阳光</color>\n\n" +
                "花费：<color=red>125</color>\n韧性：<color=red>300</color>\n冷却：<color=red>60秒</color>\n" +
                "融合配方：惊喜礼盒+模仿者（底座）"
            );
            if(HeiTaAssets.GetAsset<GameObject>("HeiTaPrefab").TryGetComponent<DiamondImitater>(out var a))
            {
                UnityEngine.Object.Destroy(a);
            }
            CustomCore.RegisterCustomPlant<Imitater,SizeChanger_HeiTa>
            (
                id_HeiTa,
                HeiTaAssets.GetAsset<GameObject>("HeiTaPrefab"),
                HeiTaAssets.GetAsset<GameObject>("HeiTaPreview"),
                new(),
                0f, 0f, 0, 300, 45f, 300
            );
            CustomCore.RegisterCustomCardToColorfulCards(id_HeiTa, 99);//can select 99 copies of the card, the max card count is 14
            CustomCore.AddPlantAlmanacStrings
            (
                id_HeiTa,
                $"礼盒模仿者({id_HeiTa})",
                "<color=#3D1400>作者：梧萱梦汐X</color>\n<color=#955300>韧性：</color><color=red>300</color>\n" +
                "<color=#955300>花费：</color><color=red>50</color>\n" +
                "<color=#955300>冷却：</color><color=red>45s</color>则随机抽取词条"
            );
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
        public static Dictionary<PlantType,HashSet<PlantType>> MixDatas = new();
        public static void OnInit()
        {
            try
            {
                /*
                foreach (var kv in PlantMixTreeManager.PlantMixTrees)
                {
                    var node = kv.Value;
                    var id = node.PlantType;
                    var name = Lawnf.GetName(id);

                    var children = node.AllDescendants;
                }
                */
                foreach( var pt in GameAPP.resourcesManager.allPlants )
                {
                    MixDatas[pt]=[..PlantMixTreeManager.GetAllMixablePlants(pt)];
                    if (MixDatas[pt].Count == 0)
                    {
                        MixDatas[pt] = new()
                        {
                            pt
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.LogError($"Fusion dump failed: {ex}");
            }
        }
    }
    public class SizeChanger_HeiTa : MonoBehaviour
    {
        public void Awake()
        {
            if(TryGetComponent<DiamondImitater>(out var a))
            {
                Destroy(a);
            }
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            Destroy(this);
        }
    }
    [HarmonyPatch(typeof(Board), nameof(Board.Start))]
    public static class Board_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Board __instance)
        {
            if (__instance == null) return; //let the game handle it
            bool isMyLevel = false;

            // 1. Check row count
            if (__instance.rowNum == 6)
            {
                // 2. Check wave count
                if (__instance.theMaxWave == 100)
                {
                    // 3. Check BoardTag flags
                    var tag = __instance.boardTag;

                    if (tag.isSuperRandom &&
                        tag.enableAllTravelPlant &&
                        tag.disableSelectCard)
                    {
                        isMyLevel = true;
                    }
                }
            }
            if (isMyLevel)
            {
                TravelMgr.Instance.GetNormalBuff(AdvBuff.EnumValue10006); //prevents zomboss self killing bug
            }
        }
    }
    [HarmonyPatch(typeof(Imitater), nameof(Imitater.AnimExplode))]
    [HarmonyPriority(Priority.Last)]
    public static class Imitater_AnimExplode_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Imitater __instance)
        {
            try
            {
                Transform axis = __instance.axis;
                Vector2 particlepos = Vector2.zero;
                Vector2 pos = axis.position;
                particlepos.y = pos.y + 0.5f;
                particlepos.x = pos.x;
                Vector2 dropPos2D = new Vector2(pos.x, pos.y + 0.5f);
                if (__instance.thePlantType == Plugin.id_HeiTa)
                {
                    var mgr = TravelMgr.Instance;
                    var data = TravelMgr.Instance.data;
                    switch (Random.Range(1, __instance.starUp ? 3 : 4))
                    {
                        default:
                        case 1:
                            HashSet<AdvBuff> buffs = [..Enum.GetValues<AdvBuff>()];
                            _ = CustomCore.CustomAdvancedBuffs.Keys.Where((int i) =>
                            {
                                buffs.Add((AdvBuff) i);
                                return true;
                            });
                            if (!buffs.Any())
                            {
                                var droppedCard = Lawnf.SetDroppedCard(dropPos2D, Plugin.id_HeiTa, 0);
                                if (__instance.starUp && droppedCard != null)
                                {
                                    var a = (Plant p) => { p.StarUp(); };
                                    droppedCard.plantAction = a;
                                }
                            }
                            else
                            {
                                var buff=buffs.ToList().GetRandomItem();
                                var istextavailable = TravelDictionary.advancedBuffsText.TryGetValue(buff, out var text);
                                InGameText.Instance.ShowText($"黑塔：抽到一个旅行词条\n{(istextavailable || string.IsNullOrEmpty(text) ? "抽到词条：" + text : "")}", 4f, false);
                                mgr.GetNormalBuff(buff);
                            }
                            CreateParticle.SetParticle(0xb, particlepos, __instance.thePlantRow, true);
                            break;
                        case 2:
                            HashSet<UltiBuff> buffs2 = [..Enum.GetValues<UltiBuff>()];
                            _ = CustomCore.CustomUltimateBuffs.Keys.Where((int i) =>
                            {
                                buffs2.Add((UltiBuff) i);
                                return true;
                            });
                            if (!buffs2.Any())
                            {
                                var droppedCard = Lawnf.SetDroppedCard(dropPos2D, Plugin.id_HeiTa, 0);
                                if (__instance.starUp && droppedCard != null)
                                {
                                    var a = (Plant p) => { p.StarUp(); };
                                    droppedCard.plantAction = a;
                                }
                            }
                            else
                            {
                                var buff2=buffs2.ToList().GetRandomItem();
                                var istextavailable = TravelDictionary.ultimateBuffsText.TryGetValue(buff2, out var text);
                                InGameText.Instance.ShowText($"黑塔：抽到一个旅行词条\n{(istextavailable || string.IsNullOrEmpty(text) ? "抽到词条：" + text : "")}", 4f, false);
                                mgr.GetUltiBuff(buff2);
                            }
                            CreateParticle.SetParticle(0xb, particlepos, __instance.thePlantRow, true);
                            break;
                        case 3:
                            HashSet<TravelDebuff> buffs3 = [..Enum.GetValues<TravelDebuff>()];
                            _ = CustomCore.CustomDebuffs.Keys.Count((int i) =>
                            {
                                buffs3.Add((TravelDebuff) i);
                                return true;
                            });
                            if (!buffs3.Any())
                            {
                                var droppedCard = Lawnf.SetDroppedCard(dropPos2D, Plugin.id_HeiTa, 0);
                                if (__instance.starUp && droppedCard != null)
                                {
                                    var a = (Plant p) => { p.StarUp(); };
                                    droppedCard.plantAction = a;
                                }
                            }
                            else
                            {
                                var buff3=buffs3.ToList().GetRandomItem();
                                var str=TravelDictionary.debuffData[buff3].Item1;
                                InGameText.Instance.ShowText($"黑塔：抽到一个旅行词条\n{(string.IsNullOrEmpty(str) ? "抽到词条：" + str : "")}", 4f, false);
                                mgr.GetDebuff(buff3);
                            }
                            CreateParticle.SetParticle(0xb, particlepos, __instance.thePlantRow, true);
                            break;
                    }
                    __instance.Die();
                    return false;
                }
                CreateParticle.SetParticle(0xb, particlepos, __instance.thePlantRow, true);
                //__instance.Die(Plant.DieReason.ByMix);
                PlantType thePlantType = PlantType.Imitater;
                Board board = __instance.board;

                var plants1x1IL2cpp = Lawnf.Get1x1Plants(__instance.thePlantColumn, __instance.thePlantRow);
                var plants1x1 = plants1x1IL2cpp?.ToSystemList() ?? new List<Plant>();

                // Pre-filter once
                var validPlants = plants1x1
                    .Where(p =>
                        p.thePlantType != PlantType.Imitater &&
                        !(p.thePlantType == PlantType.SuperMachineNut && board.boardTag.isRogue) &&
                        !(TypeMgr.RedPlant.Contains(p.thePlantType) && !(board.boardTag.isUltimateSuperRandom || board.boardTag.isSuperRandom || board.boardTag.isTreasure || board.boardTag.isIZ || GameAPP.developerMode || thePlantType == Plugin.id_PresentImitater)))
                    .ToList();

                // Category filters (materialized once)
                var landPlants = validPlants
                    .Where(p => !p.plantTag.flyingPlant && !p.plantTag.pumpkinPlant && !p.plantTag.potPlant)
                    .ToList();

                var flyingPlants = validPlants
                    .Where(p => !p.plantTag.pumpkinPlant && !p.plantTag.potPlant)
                    .ToList();

                var pumpkinPlants = validPlants
                    .Where(p => !p.plantTag.potPlant)
                    .ToList();

                // Priority selection
                if (landPlants.Count > 0)
                    thePlantType = landPlants[0].thePlantType;
                else if (flyingPlants.Count > 0)
                    thePlantType = flyingPlants[0].thePlantType;
                else if (pumpkinPlants.Count > 0)
                    thePlantType = pumpkinPlants[0].thePlantType;
                else if (validPlants.Count > 0)
                    thePlantType = validPlants[0].thePlantType;
                // 5. Drop the card at imitater's position
                if (__instance.thePlantType == Plugin.id_PresentImitater)
                {
                    thePlantType=Plugin.MixDatas[thePlantType].ToList().GetRandomItem();
                }
                DroppedCard dropped = Lawnf.SetDroppedCard(dropPos2D, thePlantType, 0);

                // 6. If star-up, attach plant action callback
                if (__instance.starUp && dropped != null)
                {
                    var a = (Plant p) => { p.StarUp(); };
                    dropped.plantAction = a;
                }
                __instance.Die();

                // 7. If not big map, we're done
                if (board == null || !board.boardTag.isBigMap || dropped == null)
                    return false;

                // 8. Ensure dropped card is on-screen in big maps
                Camera cam = Camera.main;
                Transform cardTf = dropped.transform;

                Vector3 worldPos = cardTf.position;
                Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);

                bool inside =
                    viewportPos.x > 0f && viewportPos.x < 1f &&
                    viewportPos.y > 0f && viewportPos.y < 1f;

                if (inside)
                    return false;

                // Move card to screen center
                Vector3 centerViewport = new Vector3(0.5f, 0.5f, 0f);
                Vector3 centerWorld = cam.ViewportToWorldPoint(centerViewport);
                centerWorld.z = 0f;
                cardTf.position = centerWorld;
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError("Exception detected! Using fallback.\n" + e.ToString());
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(CreatePlant))]
    public static class CreatePlant_Patch
    {
        [HarmonyPatch(nameof(CreatePlant.CheckBox))]
        [HarmonyPostfix]
        public static void CheckBox_Postfix(int theBoxColumn, int theBoxRow, PlantType theSeedType, ref bool __result)
        {
            try
            {
                if (theSeedType == Plugin.id_PresentImitater || theSeedType == Plugin.id_HeiTa)
                {
                    __result = true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception occured!\n{ex.Message}");
            }
        }
    }
    [HarmonyPatch(typeof(PlantMixTreeManager), nameof(PlantMixTreeManager.Init))]
    public static class PlantMixTreeManager_Init_Patch
    {
        [HarmonyPostfix]
        public static void AfterInit()
        {
            GameAPP.Instance.StartCoroutine(delay());
            System.Collections.IEnumerator delay()
            {
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                Plugin.OnInit();
            }
        }
    }
}
