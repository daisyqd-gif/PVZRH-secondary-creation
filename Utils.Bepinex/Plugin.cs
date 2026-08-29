using BepInEx;
using BepInEx.Unity.IL2CPP;
//using CustomizeLib.BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utils{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Core : BasePlugin
    {
        public const string PluginGuid = "Utils.Bepinex";
        public const string PluginName = "Utils";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            //CustomCore.AddFusion((int)PlantType.SolarSunflower,(int)PlantType.UltimateSunflower,(int)PlantType.SunBomb);
            //CustomCore.AddFusion((int)PlantType.SolarSunflower,(int)PlantType.SunBomb,(int)PlantType.UltimateSunflower);
            //CustomCore.AddFusion((int)PlantType.AbyssSwordStar,(int)PlantType.SwordStar,(int)PlantType.SwordStar);
            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
    [HarmonyPatch(typeof(Board), nameof(Board.Start))]
    public static class DumpBoardTag
    {
        private static string FormatBuffList<T>(Il2CppSystem.Collections.Generic.List<T> list)
        {
            var result = new List<int>();

            foreach (var b in list)
            {
                // IL2CPP enums → underlying int; need (object) first to box
                int id = (int)(object)b;
                result.Add(id);
            }

            result.Sort();
            return "{" + string.Join(", ", result) + "}";
        }

        [HarmonyPostfix]
        public static void Postfix(Board __instance)
        {
            // 1. BoardTag flags
            var tag = __instance.boardTag;
            var fields = typeof(Board.BoardTag).GetFields();
            Debug.Log("BoardTag:");
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(bool))
                {
                    bool v = (bool)f.GetValue(tag);
                    if (v)
                        Debug.Log($"    BoardTag.{f.Name} = true");
                }
            }

            // 2. Scene type and rows
            Debug.Log($"SceneType = {__instance.sceneType}");
            Debug.Log($"RowCount = {__instance.rowNum}");
            Debug.Log($"WaveCount = {__instance.theMaxWave}");

            // 3. Starting sun
            Debug.Log($"StartingSun = {__instance.theSun}");

            var preplants = new List<string>();

            foreach (var plant in __instance.boardEntity.plantArray)
            {
                if (plant == null) continue;

                int col = plant.thePlantColumn;
                int row = plant.thePlantRow;
                PlantType type = plant.thePlantType;

                if(type!=PlantType.Pot)preplants.Add($"({col}, {row}, {type})");
            }

            // Format as { (c,r,type), (c,r,type), ... }
            string formatted = "{" + string.Join(", ", preplants) + "}";
            Debug.Log("PrePlantList = " + formatted);


            // 5. Conveyor belt plants (if any)
            if (__instance.boardTag.isConvey == true)
            {
                Debug.Log("ConveyorPlants = " + FormatBuffList(ConveyManager.Instance.GetCardPool()));
            }

            // 6. Travel buffs (if present)
            try
            {
                var data = TravelMgr.Instance.data;

                var buffs = data?.advBuffs;
                if (buffs != null)
                    Debug.Log("TravelBuffs = " + FormatBuffList(buffs));

                var ultibuffs = data?.ultiBuffs;
                if (ultibuffs != null)
                    Debug.Log("TravelUltimateBuffs = " + FormatBuffList(ultibuffs));

                var debuffs = data?.travelDebuffs;
                if (debuffs != null)
                    Debug.Log("TravelDebuffs = " + FormatBuffList(debuffs));

                var investbuffs = data?.investBuffs;
                if (investbuffs != null)
                    Debug.Log("InvestBuffs = " + FormatBuffList(investbuffs));
            }
            catch {}

            // 7. Zombie waves (from InitZombieList)
            try
            {
                var result = new List<int>();

                foreach (var obj in InitZombieList.zombieTypeList)
                {
                    int id = (int)obj;   // IL2CPP stores enums as boxed ints
                    result.Add(id);
                }

                result.Sort();

                // Format as {1, 2, 3}
                string formatted2 = "Zombies: {" + string.Join(", ", result) + "}";
                Debug.Log(formatted2);
            }
            catch {}
        }
    }
    /*[HarmonyPatch(typeof(SolarSunflower), nameof(SolarSunflower.Start))]
    public static class UnsealSolarSunflower
    {
        [HarmonyPrefix]
        public static bool Prefix(SolarSunflower __instance)
        {
            if(__instance==null || __instance.board==null || GameAPP.theGameStatus!=GameStatus.InGame) return true;
            Board.Instance.OnPlantCreate(__instance);
            __instance.UpdateText();
            __instance.ReplaceSprite();
            __instance.FirstMeet();
            Animator anim=__instance.anim;
            anim.Play("enter");
            return false;
        }
    }
    [HarmonyPatch(typeof(AbyssSwordStar), nameof(AbyssSwordStar.Start))]
    public static class UnsealAbyssSwordStar
    {
        [HarmonyPrefix]
        public static bool Prefix(AbyssSwordStar __instance)
        {
            if(__instance==null || __instance.board==null || GameAPP.theGameStatus!=GameStatus.InGame) return true;
            Board.Instance.OnPlantCreate(__instance);
            __instance.UpdateText();
            __instance.ReplaceSprite();
            __instance.FirstMeet();
            return false;
        }
    }
    [HarmonyPatch(typeof(UltimateMinigun), nameof(UltimateMinigun.Start))]
    public static class UnsealUltimateMinigun
    {
        [HarmonyPrefix]
        public static bool Prefix(UltimateMinigun __instance)
        {
            if(__instance==null || __instance.board==null || GameAPP.theGameStatus!=GameStatus.InGame) return true;
            Board.Instance.OnPlantCreate(__instance);
            __instance.UpdateText();
            __instance.ReplaceSprite();
            __instance.FirstMeet();
            return false;
        }
    }*/
}
