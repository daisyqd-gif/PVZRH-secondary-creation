using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using Core;
using GameLevel.RogueShooting;
using Il2CppSystem;
//using Il2CppSystem.Collections.Generic;
using Modified.GameData;
using Modified.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ZenGarden;
using Random = UnityEngine.Random;
using Convert = System.Convert;
using Exception = System.Exception;
using Object = UnityEngine.Object;
using Enum = System.Enum;
using Console = System.Console;
using StringSplitOptions = System.StringSplitOptions;
using Type = System.Type;
using HarmonyLib;

namespace Modified.Command
{
    // Token: 0x02000060 RID: 96
    public static class CommandMethods
    {
        // Token: 0x06000180 RID: 384 RVA: 0x00011434 File Offset: 0x0000F634
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ProcessCommand(string message)
        {
            List<string> list = new();
            bool flag = false;
            int num = 0;
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] == '\"')
                {
                    flag = !flag;
                }
                else if (message[i] == ' ')
                {
                    if (!flag)
                    {
                        if (i > num)
                        {
                            string text = message.Substring(num, i - num);
                            if (text.Length >= 2 && text[0] == '\"')
                            {
                                string text2 = text;
                                if (text2[text2.Length - 1] == '\"')
                                {
                                    text = text.Substring(1, text.Length - 2);
                                }
                            }
                            list.Add(text);
                        }
                        num = i + 1;
                    }
                }
            }
            if (num < message.Length)
            {
                string text3 = message.Substring(num);
                if (text3.Length >= 2 && text3[0] == '\"')
                {
                    string text4 = text3;
                    if (text4[text4.Length - 1] == '\"')
                    {
                        text3 = text3.Substring(1, text3.Length - 2);
                    }
                }
                list.Add(text3);
            }

            // clean filter: remove empty/whitespace args
            list = list.Where(s => !string.IsNullOrEmpty(s)).ToList();

            if (list.Count == 0)
            {
                if (Plugin.isDevMode)
                {
                    Plugin.Field0.LogMessage("无效的命令");
                }
                return;
            }
            string text5 = list[0].ToLower();
            MethodInfo[] methods = typeof(CommandMethods).GetMethods(BindingFlags.Static | BindingFlags.Public);
            int j = 0;
            bool flag2;
            while (j < methods.Length)
            {
                MethodInfo methodInfo = methods[j];
                CommandAttribute customAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                if (customAttribute == null || !(customAttribute.CommandName == text5))
                {
                    j++;
                }
                else
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (parameters.Length == list.Count - 1)
                    {
                        try
                        {
                            object[] array = new object[parameters.Length];
                            for (int k = 0; k < parameters.Length; k++)
                            {
                                array[k] = Convert.ChangeType(list[k + 1], parameters[k].ParameterType);
                            }
                            methodInfo.Invoke(null, array);
                        }
                        catch (Exception ex)
                        {
                            if (Plugin.isDevMode)
                            {
                                ManualLogSource field = Plugin.Field0;
                                var bepInExMessageLogInterpolatedStringHandler =
                                    new BepInExMessageLogInterpolatedStringHandler(10, 3, out flag2);
                                if (flag2)
                                {
                                    bepInExMessageLogInterpolatedStringHandler.AppendLiteral("命令执行失败: ");
                                    bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>(ex.Message);
                                    bepInExMessageLogInterpolatedStringHandler.AppendLiteral("\n");
                                    bepInExMessageLogInterpolatedStringHandler.AppendFormatted<Exception>(ex.InnerException);
                                    bepInExMessageLogInterpolatedStringHandler.AppendLiteral("\n");
                                    bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>(ex.Source);
                                }
                                field.LogMessage(bepInExMessageLogInterpolatedStringHandler);
                            }
                        }
                        return;
                    }
                    if (Plugin.isDevMode)
                    {
                        ManualLogSource field2 = Plugin.Field0;
                        var bepInExMessageLogInterpolatedStringHandler =
                            new BepInExMessageLogInterpolatedStringHandler(12, 1, out flag2);
                        if (flag2)
                        {
                            bepInExMessageLogInterpolatedStringHandler.AppendLiteral("参数数量错误用于命令: ");
                            bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>(text5);
                        }
                        field2.LogMessage(bepInExMessageLogInterpolatedStringHandler);
                    }
                    return;
                }
            }
            if (Plugin.isDevMode)
            {
                ManualLogSource field3 = Plugin.Field0;
                var bepInExMessageLogInterpolatedStringHandler =
                    new BepInExMessageLogInterpolatedStringHandler(7, 1, out flag2);
                if (flag2)
                {
                    bepInExMessageLogInterpolatedStringHandler.AppendLiteral("未找到命令: ");
                    bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>(text5);
                }
                field3.LogMessage(bepInExMessageLogInterpolatedStringHandler);
                return;
            }
        }

        // Token: 0x06000181 RID: 385 RVA: 0x00011748 File Offset: 0x0000F948
        [Command("help", "无")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ShowHelp()
        {
            MethodInfo[] methods = typeof(CommandMethods).GetMethods(BindingFlags.Static | BindingFlags.Public);
            string text = "";
            foreach (MethodInfo methodInfo in methods)
            {
                string name = methodInfo.Name;
                CommandAttribute customAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                if (customAttribute != null)
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    int num = parameters.Length;
                    string text2;
                    if (num <= 0)
                    {
                        text2 = "无";
                    }
                    else
                    {
                        // clean: directly select parameter type names
                        text2 = string.Join(", ", parameters.Select(p => p.ParameterType.Name));
                    }
                    string value = text2;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 4);
                    defaultInterpolatedStringHandler.AppendLiteral("命令: ");
                    defaultInterpolatedStringHandler.AppendFormatted(customAttribute.CommandName);
                    defaultInterpolatedStringHandler.AppendLiteral(", 参数个数: ");
                    defaultInterpolatedStringHandler.AppendFormatted<int>(num);
                    defaultInterpolatedStringHandler.AppendLiteral(", 参数类型: ");
                    defaultInterpolatedStringHandler.AppendFormatted(value);
                    defaultInterpolatedStringHandler.AppendLiteral(", 描述：");
                    defaultInterpolatedStringHandler.AppendFormatted(customAttribute.Description);
                    string text3 = defaultInterpolatedStringHandler.ToStringAndClear();
                    text = text + text3 + "\n";
                    Plugin.Field0.LogMessage(text3);
                }
            }
            CommandMethods.SendTcpMsg("msg|" + text);
        }

        // Token: 0x06000182 RID: 386 RVA: 0x00011884 File Offset: 0x0000FA84
        [Command("setp", "在指定位置放置植物，如setp 0 0 900")]
        public static void SetP(int x, int y, int plantType)
        {
            PublicMethods.SetBoardPlant(x, y, plantType, 1);
        }

        // Token: 0x06000183 RID: 387 RVA: 0x0001189C File Offset: 0x0000FA9C
        [Command("setz", "在指定位置放置僵尸，如setz 3 3 20")]
        public static void SetZ(int x, int y, int zombieType, int isMindControl = 0)
        {
            bool charm = isMindControl == 1;
            PublicMethods.SetBoardZombie(x, y, zombieType, charm, 1);
        }

        // Token: 0x06000184 RID: 388 RVA: 0x000118B8 File Offset: 0x0000FAB8
        [Command("summon", "召唤命令，在指定位置放置植物/僵尸")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Summon(string type, int x, int y, int typeId, int count, int isSelected)
        {
            bool flag = isSelected == 1;
            if (!(type == "p"))
            {
                if (type == "z")
                {
                    PublicMethods.SetBoardZombie(x, y, typeId, flag, count);
                    return;
                }
                return;
            }
            else
            {
                if (flag)
                {
                    PublicMethods.PlantInRow(x, y, 256, count);
                    return;
                }
                PublicMethods.PlantInRow(x, y, typeId, count);
                return;
            }
        }

        // Token: 0x06000185 RID: 389 RVA: 0x00011910 File Offset: 0x0000FB10
        [Command("sets", "设置阳光数量，如sets 1000")]
        public static void SetS(int sun)
        {
            Board.Instance.theSun = sun;
        }

        // Token: 0x06000186 RID: 390 RVA: 0x00011928 File Offset: 0x0000FB28
        [Command("setpo", "设置游戏积分，如setpo 1000")]
        public static void SetPO(int num)
        {
            Board.Instance.thePoints = (float)num;
        }

        // Token: 0x06000187 RID: 391 RVA: 0x00011944 File Offset: 0x0000FB44
        [Command("setm", "设置金币数量，如setm 10000")]
        public static void SetM(int money)
        {
            if (Board.Instance != null)
            {
                Board.Instance.theMoney = money;
            }
            GameAPP.theMoneyCount = (long)money;
        }

        // Token: 0x06000188 RID: 392 RVA: 0x00011970 File Offset: 0x0000FB70
        [Command("changeplant", "无")]
        public static void ChangePlant(int row, int col, int hp, int atk, int atks)
        {
            foreach (Plant plant in Board.Instance.boardEntity.plantArray)
            {
                if (plant != null)
                {
                    if (plant.thePlantColumn == col)
                    {
                        if (plant.thePlantRow == row)
                        {
                            if (hp != -1)
                            {
                                plant.thePlantHealth = hp;
                            }
                            if (atk != -1)
                            {
                                plant.attackDamage = atk;
                            }
                            if (atks != -1)
                            {
                                plant.thePlantAttackInterval = (float)atks;
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06000189 RID: 393 RVA: 0x000119EC File Offset: 0x0000FBEC
        [Command("changeallplant", "无")]
        public static void ChangeAllPlant(int hp, int atk, int atks)
        {
            foreach (Plant plant in Board.Instance.boardEntity.plantArray)
            {
                if (plant != null)
                {
                    if (hp != -1)
                    {
                        plant.thePlantHealth = hp;
                    }
                    if (atk != -1)
                    {
                        plant.attackDamage = atk;
                    }
                    if (atks != -1)
                    {
                        plant.thePlantAttackInterval = (float)atks;
                    }
                }
            }
        }

        // Token: 0x0600018A RID: 394 RVA: 0x00011A50 File Offset: 0x0000FC50
        [Command("changezombie", "无")]
        public static void ChangeZomibe(int zombieID, int hp, int shield, int atk, int atks)
        {
            foreach (Zombie zombie in Board.Instance.zombieArray)
            {
                if (zombie != null && (int)zombie.theZombieType == zombieID)
                {
                    if (hp != -1)
                    {
                        zombie.theHealth = hp;
                    }
                    if (shield != -1)
                    {
                        zombie.theFirstArmorHealth = shield;
                    }
                    if (atk != -1)
                    {
                        zombie.theAttackDamage = atk;
                    }
                    if (atks != -1)
                    {
                        zombie.theSpeed = (float)atks;
                    }
                }
            }
        }

        // Token: 0x0600018B RID: 395 RVA: 0x00011AC4 File Offset: 0x0000FCC4
        [Command("changeallzombie", "无")]
        public static void ChangeAllZombie(int hp, int shield, int atk, int atks)
        {
            foreach (Zombie zombie in Board.Instance.zombieArray)
            {
                if (zombie != null)
                {
                    if (hp != -1)
                    {
                        zombie.theHealth = hp;
                    }
                    if (shield != -1)
                    {
                        zombie.theFirstArmorHealth = shield;
                    }
                    if (atk != -1)
                    {
                        zombie.theAttackDamage = atk;
                    }
                    if (atks != -1)
                    {
                        zombie.theSpeed = (float)atks;
                    }
                }
            }
        }

        // Token: 0x0600018C RID: 396 RVA: 0x00011B2C File Offset: 0x0000FD2C
        [Command("getenumvalue", "获取某个枚举的所有枚举值")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GetEnumValue(string name)
        {
            foreach (Object @object in Enum.GetValues(Type.GetType(name)))
            {
                ManualLogSource field = Plugin.Field0;
                bool flag;
                var bepInExInfoLogInterpolatedStringHandler =
                    new BepInExInfoLogInterpolatedStringHandler(1, 2, out flag);
                if (flag)
                {
                    bepInExInfoLogInterpolatedStringHandler.AppendFormatted<int>((int)@object.Unbox<AdvBuff>());
                    bepInExInfoLogInterpolatedStringHandler.AppendLiteral("|");
                    bepInExInfoLogInterpolatedStringHandler.AppendFormatted<string>(@object.ToString());
                }
                field.LogInfo(bepInExInfoLogInterpolatedStringHandler);
            }
        }

        // Token: 0x0600018D RID: 397 RVA: 0x00011BC0 File Offset: 0x0000FDC0
        [Command("travel", "解锁某个旅行词条，如travel 1 1 1")]
        public static void Travel(int type, int travelID, int isCheck)
        {
            bool flag = isCheck == 1;
            TravelMgr instance = TravelMgr.Instance;
            switch (type)
            {
            case 1:
                if (!flag)
                {
                    instance.data.unlockedPlants.Remove((TravelUnlocks)travelID);
                }
                else
                {
                    instance.data.unlockedPlants.Add((TravelUnlocks)travelID);
                }
                if (Plugin.isDevMode && flag)
                {
                    InGameText.Instance.ShowText(TravelDictionary.unlocksText[(TravelUnlocks)travelID] ?? "", 2f, false);
                    return;
                }
                break;
            case 2:
                if (!flag)
                {
                    instance.data.advBuffs.Remove((AdvBuff)travelID);
                }
                else
                {
                    instance.data.advBuffs.Add((AdvBuff)travelID);
                }
                if (Plugin.isDevMode && flag)
                {
                    InGameText.Instance.ShowText(TravelDictionary.advancedBuffsText[(AdvBuff)travelID] ?? "", 2f, false);
                    return;
                }
                break;
            case 3:
                if (!flag)
                {
                    instance.data.ultiBuffs.Remove((UltiBuff)travelID);
                }
                else
                {
                    instance.data.ultiBuffs.Add((UltiBuff)travelID);
                }
                if (Plugin.isDevMode && flag)
                {
                    InGameText.Instance.ShowText(TravelDictionary.ultimateBuffsText[(UltiBuff)travelID] ?? "", 2f, false);
                    return;
                }
                break;
            case 4:
                if (!flag)
                {
                    instance.data.travelDebuffs.Remove((TravelDebuff)travelID);
                }
                else
                {
                    instance.data.travelDebuffs.Add((TravelDebuff)travelID);
                }
                if (Plugin.isDevMode && flag)
                {
                    InGameText.Instance.ShowText(TravelDictionary.debuffData[(TravelDebuff)travelID].Item1 ?? "", 2f, false);
                    return;
                }
                break;
            case 5:
                if (flag)
                {
                    instance.data.investBuffs.Add((InvestBuff)travelID);
                    return;
                }
                instance.data.investBuffs.Remove((InvestBuff)travelID);
                break;
            default:
                return;
            }
        }

        // Token: 0x0600018E RID: 398 RVA: 0x00011D84 File Offset: 0x0000FF84
        [Command("travelstore", "旅行商店无限刷新")]
        public static void TravelStore(int isCheck)
        {
            Plugin.isTravelRefresh = true;
        }

        // Token: 0x0600018F RID: 399 RVA: 0x00011D98 File Offset: 0x0000FF98
        [Command("changeshootingrefresh", "修改刷新次数")]
        public static void ChangeShootingRefresh(int num)
        {
            ShootingManager.Instance.refreshCount = num;
            Plugin.isTravelRefresh = false;
        }

        // Token: 0x06000190 RID: 400 RVA: 0x00011DB8 File Offset: 0x0000FFB8
        [Command("batchtravel", "无")]
        public static void BatchTravel(int type, int isCheck)
        {
            TravelMgr instance = TravelMgr.Instance;
            if (type == 1)
            {
                foreach (object obj in Enum.GetValues(typeof(AdvBuff)))
                {
                    instance.data.advBuffs.Add((AdvBuff)obj);
                }
                foreach (object obj2 in Enum.GetValues(typeof(UltiBuff)))
                {
                    instance.data.ultiBuffs.Add((UltiBuff)obj2);
                    instance.data.ultiBuffs_lv2.Add((UltiBuff)obj2);
                }
                foreach (TravelUnlocks unlock in Enum.GetValues(typeof(TravelUnlocks)))
                {
                    instance.data.unlockedPlants.Add(unlock);
                }
            }
            if (type != 2)
            {
                return;
            }
            foreach (object obj4 in Enum.GetValues(typeof(TravelDebuff)))
            {
                instance.data.travelDebuffs.Add((TravelDebuff)obj4);
            }
        }

        // Token: 0x06000191 RID: 401 RVA: 0x00011F68 File Offset: 0x00010168
        [Command("inittravel", "无")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void InitTravel()
        {
            List<string> list = new();
            TravelMgr instance = TravelMgr.Instance;
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<TravelUnlocks, string> keyValuePair in TravelDictionary.unlocksText)
            {
                try
                {
                    List<string> list2 = list;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 3);
                    defaultInterpolatedStringHandler.AppendLiteral("1_");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair.Key);
                    defaultInterpolatedStringHandler.AppendLiteral(",");
                    defaultInterpolatedStringHandler.AppendFormatted(keyValuePair.Value.Replace(",", "，"));
                    defaultInterpolatedStringHandler.AppendLiteral(",植物解锁,1,");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair.Key);
                    list2.Add(defaultInterpolatedStringHandler.ToStringAndClear());
                }
                catch
                {
                }
            }
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<AdvBuff, string> keyValuePair2 in TravelDictionary.advancedBuffsText)
            {
                try
                {
                    List<string> list3 = list;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 3);
                    defaultInterpolatedStringHandler.AppendLiteral("2_");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair2.Key);
                    defaultInterpolatedStringHandler.AppendLiteral(",");
                    defaultInterpolatedStringHandler.AppendFormatted(keyValuePair2.Value.Replace(",", "，"));
                    defaultInterpolatedStringHandler.AppendLiteral(",加成词条,2,");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair2.Key);
                    list3.Add(defaultInterpolatedStringHandler.ToStringAndClear());
                }
                catch
                {
                }
            }
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<UltiBuff, string> keyValuePair3 in TravelDictionary.ultimateBuffsText)
            {
                try
                {
                    List<string> list4 = list;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 3);
                    defaultInterpolatedStringHandler.AppendLiteral("3_");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair3.Key);
                    defaultInterpolatedStringHandler.AppendLiteral(",");
                    defaultInterpolatedStringHandler.AppendFormatted(keyValuePair3.Value.Replace(",", "，"));
                    defaultInterpolatedStringHandler.AppendLiteral(",究极词条,3,");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair3.Key);
                    list4.Add(defaultInterpolatedStringHandler.ToStringAndClear());
                }
                catch
                {
                }
            }
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<TravelDebuff, Il2CppSystem.ValueTuple<string, ZombieType>> keyValuePair4 in TravelDictionary.debuffData)
            {
                try
                {
                    List<string> list5 = list;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 3);
                    defaultInterpolatedStringHandler.AppendLiteral("4_");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair4.Key);
                    defaultInterpolatedStringHandler.AppendLiteral(",");
                    defaultInterpolatedStringHandler.AppendFormatted(keyValuePair4.Value.Item1.Replace(",", "，"));
                    defaultInterpolatedStringHandler.AppendLiteral(",僵尸词条,4,");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair4.Key);
                    list5.Add(defaultInterpolatedStringHandler.ToStringAndClear());
                }
                catch
                {
                }
            }
            string str = string.Join(";", list);
            CommandMethods.SendTcpMsg("inittravel|" + str);
        }

        // Token: 0x06000192 RID: 402 RVA: 0x00012258 File Offset: 0x00010458
        [Command("gettravel", "获取所有已有词条描述")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GetTravel()
        {
            TravelMgr instance = TravelMgr.Instance;
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<UltiBuff, string> keyValuePair in TravelDictionary.ultimateBuffsText)
            {
                int key = (int)keyValuePair.Key;
                string value = keyValuePair.Value;
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 3);
                defaultInterpolatedStringHandler.AppendLiteral("AddTravelEntry(\"1_");
                defaultInterpolatedStringHandler.AppendFormatted<int>(key);
                defaultInterpolatedStringHandler.AppendLiteral("\", \"");
                defaultInterpolatedStringHandler.AppendFormatted(value);
                defaultInterpolatedStringHandler.AppendLiteral("\", \"植物解锁\", 1, ");
                defaultInterpolatedStringHandler.AppendFormatted<int>(key);
                defaultInterpolatedStringHandler.AppendLiteral(");");
                Console.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
            }
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<AdvBuff, string> keyValuePair2 in TravelDictionary.advancedBuffsText)
            {
                int key2 = (int)keyValuePair2.Key;
                string value2 = keyValuePair2.Value;
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 3);
                defaultInterpolatedStringHandler.AppendLiteral("AddTravelEntry(\"2_");
                defaultInterpolatedStringHandler.AppendFormatted<int>(key2);
                defaultInterpolatedStringHandler.AppendLiteral("\", \"");
                defaultInterpolatedStringHandler.AppendFormatted(value2);
                defaultInterpolatedStringHandler.AppendLiteral("\", \"加成词条\", 2, ");
                defaultInterpolatedStringHandler.AppendFormatted<int>(key2);
                defaultInterpolatedStringHandler.AppendLiteral(");");
                Console.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
            }
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<TravelUnlocks, string> keyValuePair3 in TravelDictionary.unlocksText)
            {
                int key3 = (int)keyValuePair3.Key;
                string value3 = keyValuePair3.Value;
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 3);
                defaultInterpolatedStringHandler.AppendLiteral("AddTravelEntry(\"3_");
                defaultInterpolatedStringHandler.AppendFormatted<int>(key3);
                defaultInterpolatedStringHandler.AppendLiteral("\", \"");
                defaultInterpolatedStringHandler.AppendFormatted(value3);
                defaultInterpolatedStringHandler.AppendLiteral("\", \"究极词条\", 3, ");
                defaultInterpolatedStringHandler.AppendFormatted<int>(key3);
                defaultInterpolatedStringHandler.AppendLiteral(");");
                Console.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
            }
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<TravelDebuff, Il2CppSystem.ValueTuple<string, ZombieType>> keyValuePair4 in TravelDictionary.debuffData)
            {
                int key4 = (int)keyValuePair4.Key;
                string item = keyValuePair4.Value.Item1;
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 3);
                defaultInterpolatedStringHandler.AppendLiteral("AddTravelEntry(\"4_");
                defaultInterpolatedStringHandler.AppendFormatted<int>(key4);
                defaultInterpolatedStringHandler.AppendLiteral("\", \"");
                defaultInterpolatedStringHandler.AppendFormatted(item);
                defaultInterpolatedStringHandler.AppendLiteral("\", \"僵尸词条\", 4, ");
                defaultInterpolatedStringHandler.AppendFormatted<int>(key4);
                defaultInterpolatedStringHandler.AppendLiteral(");");
                Console.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
            }
        }

        // Token: 0x06000193 RID: 403 RVA: 0x000124B8 File Offset: 0x000106B8
        [Command("exporttravel", "获取所有已有词条描述")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ExportTravel()
        {
            ComponentHolderProtocol.GetOrAddComponent<TravelMgr>(GameObject.Find("GameAPP"));
            using (StreamWriter streamWriter = new StreamWriter("TravelEntries.txt"))
            {
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<UltiBuff, string> keyValuePair in TravelDictionary.ultimateBuffsText)
                {
                    int key = (int)keyValuePair.Key;
                    string value = keyValuePair.Value;
                    TextWriter textWriter = streamWriter;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("1_");
                    defaultInterpolatedStringHandler.AppendFormatted<int>(key);
                    defaultInterpolatedStringHandler.AppendLiteral(": ");
                    defaultInterpolatedStringHandler.AppendFormatted(value);
                    textWriter.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
                }
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<AdvBuff, string> keyValuePair2 in TravelDictionary.advancedBuffsText)
                {
                    int key2 = (int)keyValuePair2.Key;
                    string value2 = keyValuePair2.Value;
                    TextWriter textWriter2 = streamWriter;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("2_");
                    defaultInterpolatedStringHandler.AppendFormatted<int>(key2);
                    defaultInterpolatedStringHandler.AppendLiteral(": ");
                    defaultInterpolatedStringHandler.AppendFormatted(value2);
                    textWriter2.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
                }
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<TravelUnlocks, string> keyValuePair3 in TravelDictionary.unlocksText)
                {
                    int key3 = (int)keyValuePair3.Key;
                    string value3 = keyValuePair3.Value;
                    TextWriter textWriter3 = streamWriter;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("3_");
                    defaultInterpolatedStringHandler.AppendFormatted<int>(key3);
                    defaultInterpolatedStringHandler.AppendLiteral(": ");
                    defaultInterpolatedStringHandler.AppendFormatted(value3);
                    textWriter3.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
                }
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<TravelDebuff, Il2CppSystem.ValueTuple<string, ZombieType>> keyValuePair4 in TravelDictionary.debuffData)
                {
                    int key4 = (int)keyValuePair4.Key;
                    string item = keyValuePair4.Value.Item1;
                    TextWriter textWriter4 = streamWriter;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("4_");
                    defaultInterpolatedStringHandler.AppendFormatted<int>(key4);
                    defaultInterpolatedStringHandler.AppendLiteral(": ");
                    defaultInterpolatedStringHandler.AppendFormatted(item);
                    textWriter4.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
                }
            }
            Console.WriteLine("Key-value pairs have been exported to the file.");
        }

        // Token: 0x06000194 RID: 404 RVA: 0x000126C4 File Offset: 0x000108C4
        [Command("create", "根据ID创建物品")]
        public static void Create(int id)
        {
            PublicMethods.SpawnItem(ItemTypes.itemsData[id]);
        }

        // Token: 0x06000195 RID: 405 RVA: 0x000126E0 File Offset: 0x000108E0
        [Command("created", "在指定位置创建资源")]
        public static void Created(string resourcePath, int x, int y)
        {
            PublicMethods.SetRes(resourcePath, (float)x, (float)y);
        }

        // Token: 0x06000196 RID: 406 RVA: 0x000126F8 File Offset: 0x000108F8
        [Command("speed", "设置游戏速度，如speed 10")]
        public static void Speed(float speed)
        {
            Time.timeScale = speed;
        }

        // Token: 0x06000197 RID: 407 RVA: 0x0001270C File Offset: 0x0001090C
        [Command("grow", "修改指定位置的花园植物")]
        public static void Grow(int x, int y, int plantType)
        {
            foreach (GardenPlant gardenPlant in GardenUI.Instance.gardenPlants)
            {
                if (gardenPlant != null)
                {
                    if (x == gardenPlant.data.thePlantColumn)
                    {
                        if (y == gardenPlant.data.thePlantRow)
                        {
                            gardenPlant.data.thePlantType = (PlantType)plantType;
                        }
                    }
                    gardenPlant.data.growStage = 2;
                    gardenPlant.data.needTool = 0;
                    gardenPlant.data.waterLevel = 100;
                    gardenPlant.data.love = 100;
                    gardenPlant.data.nextTime = 20241106992L;
                }
            }
            //GardenData.SaveGardenData(GardenUI.Instance);
        }

        // Token: 0x06000198 RID: 408 RVA: 0x000127C8 File Offset: 0x000109C8
        [Command("opencard", "打开选卡")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OpenCard()
        {
            Transform transform = GameObject.Find("Canvas/InGameUIFHD").transform.Find("Bottom");
            transform.gameObject.SetActive(true);
            if ((int)transform.position.x <= 1)
            {
                transform.position = new Vector3(5f, 0f, -100f);
            }
            else
            {
                transform.position = new Vector3(0f, -8.75f, -100f);
            }
            GameObject.Find("Canvas/InGameUIFHD/Bottom/SeedLibrary/Start").SetActive(false);
        }

        // Token: 0x06000199 RID: 409 RVA: 0x00012854 File Offset: 0x00010A54
        [Command("export", "控制台导出阵容码")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Export()
        {
            Console.WriteLine("阵容代码: " + LineupManager.ExportLineup());
        }

        // Token: 0x0600019A RID: 410 RVA: 0x00012878 File Offset: 0x00010A78
        [Command("exportz", "控制台导出僵尸阵容码")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ExportZ()
        {
            Console.WriteLine("阵容代码: " + LineupManager.ExportZombieLineup());
        }

        // Token: 0x0600019B RID: 411 RVA: 0x0001289C File Offset: 0x00010A9C
        [Command("exportm", "控制台导出僵尸阵容码")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ExportM()
        {
            Console.WriteLine("阵容代码: " + LineupManager.ExportMixLineup());
        }

        // Token: 0x0600019C RID: 412 RVA: 0x000128C0 File Offset: 0x00010AC0
        [Command("dqqp", "批量设置植物")]
        public static void DQQP(string xarr, string yarr, int plantID, int plantCount)
        {
            string[] array = xarr.Trim(new char[]
            {
                '[',
                ']'
            }).Split(',', StringSplitOptions.None);
            string[] array2 = yarr.Trim(new char[]
            {
                '[',
                ']'
            }).Split(',', StringSplitOptions.None);
            foreach (string s in array)
            {
                foreach (string s2 in array2)
                {
                    int x;
                    int y;
                    if (int.TryParse(s, out x) && int.TryParse(s2, out y))
                    {
                        PublicMethods.PlantInRow(x, y, plantID, plantCount);
                    }
                }
            }
        }

        // Token: 0x0600019D RID: 413 RVA: 0x00012960 File Offset: 0x00010B60
        [Command("dqqz", "批量设置僵尸")]
        public static void DQQZ(string xarr, string yarr, int zombieID, int count)
        {
            string[] array = xarr.Trim(new char[]
            {
                '[',
                ']'
            }).Split(',', StringSplitOptions.None);
            string[] array2 = yarr.Trim(new char[]
            {
                '[',
                ']'
            }).Split(',', StringSplitOptions.None);
            foreach (string s in array)
            {
                foreach (string s2 in array2)
                {
                    int x;
                    int y;
                    if (int.TryParse(s, out x) && int.TryParse(s2, out y))
                    {
                        PublicMethods.SetBoardZombie(x, y, zombieID, false, count);
                    }
                }
            }
        }

        // Token: 0x0600019E RID: 414 RVA: 0x00012A00 File Offset: 0x00010C00
        [Command("use", "使用阵容码")]
        public static void Use(string linupCode)
        {
            LineupManager.ImportLineup(linupCode);
        }

        // Token: 0x0600019F RID: 415 RVA: 0x00012A14 File Offset: 0x00010C14
        [Command("usez", "使用僵尸阵容码")]
        public static void UseZ(string linupCode)
        {
            LineupManager.ImportZombieLineup(linupCode);
        }

        // Token: 0x060001A0 RID: 416 RVA: 0x00012A28 File Offset: 0x00010C28
        [Command("usem", "使用僵尸阵容码")]
        public static void UseM(string linupCode)
        {
            LineupManager.ImportMixLineup(linupCode);
        }

        // Token: 0x060001A1 RID: 417 RVA: 0x00012A3C File Offset: 0x00010C3C
        [Command("devmode", "开启开发者模式")]
        public static void DevMode(int isCheck)
        {
            GameAPP.developerMode = (isCheck == 1);
        }

        // Token: 0x060001A2 RID: 418 RVA: 0x00012A54 File Offset: 0x00010C54
        [Command("setplayerbasicdamage", "无")]
        public static void setplayerbasicdamage(int num)
        {
            PlayerShootingManager.Instance.player.basicDamage = (float)num;
        }

        // Token: 0x060001A3 RID: 419 RVA: 0x00012A74 File Offset: 0x00010C74
        [Command("setplayercurrenthealth", "无")]
        public static void setplayercurrenthealth(int num)
        {
            PlayerShootingManager.Instance.player.currentHealth = num;
        }

        // Token: 0x060001A4 RID: 420 RVA: 0x00012A94 File Offset: 0x00010C94
        [Command("setplayerdefence", "无")]
        public static void setplayerdefence(int num)
        {
            PlayerShootingManager.Instance.player.defence = (float)num;
        }

        // Token: 0x060001A5 RID: 421 RVA: 0x00012AB4 File Offset: 0x00010CB4
        [Command("setplayerlevel", "无")]
        public static void setplayerlevel(int num)
        {
            PlayerShootingManager.Instance.player.level = num;
        }

        // Token: 0x060001A6 RID: 422 RVA: 0x00012AD4 File Offset: 0x00010CD4
        [Command("setplayermaxhealth", "无")]
        public static void setplayermaxhealth(int num)
        {
            PlayerShootingManager.Instance.player.maxHealth = num;
        }

        // Token: 0x060001A7 RID: 423 RVA: 0x00012AF4 File Offset: 0x00010CF4
        [Command("setplayermovespeed", "无")]
        public static void setplayermovespeed(int num)
        {
            PlayerShootingManager.Instance.player.moveSpeed = (float)num;
        }

        // Token: 0x060001A8 RID: 424 RVA: 0x00012B14 File Offset: 0x00010D14
        [Command("setplayerrefreshcount", "无")]
        public static void setplayerrefreshcount(int num)
        {
            PlayerShootingManager.Instance.player.refreshCount = num;
        }

        // Token: 0x060001A9 RID: 425 RVA: 0x00012B34 File Offset: 0x00010D34
        [Command("unlimplanthp", "去除植物血量限制")]
        public static void UnLimPlantHp(int isCheck)
        {
            Plugin.isPlantLimHp = (isCheck == 1);
        }

        // Token: 0x060001AA RID: 426 RVA: 0x00012B4C File Offset: 0x00010D4C
        [Command("plantuncrashed", "植物防碾压")]
        public static void PlantUncrashed(int isCheck)
        {
            Plugin.isPlantAntiRolling = (isCheck == 1);
        }

        // Token: 0x060001AB RID: 427 RVA: 0x00012B64 File Offset: 0x00010D64
        [Command("cardnocd", "卡片无CD")]
        public static void CardNoCD(int isCheck)
        {
            Plugin.isCardNoCD = (isCheck == 1);
        }

        // Token: 0x060001AC RID: 428 RVA: 0x00012B7C File Offset: 0x00010D7C
        [Command("glovenocd", "手套无CD")]
        public static void GloveNoCD(int isCheck)
        {
            Plugin.isGloveNoCD = (isCheck == 1);
        }

        // Token: 0x060001AD RID: 429 RVA: 0x00012B94 File Offset: 0x00010D94
        [Command("hammernocd", "锤子无CD")]
        public static void Hammernocd(int isCheck)
        {
            Plugin.isHammerNoCD = (isCheck == 1);
        }

        // Token: 0x060001AE RID: 430 RVA: 0x00012BAC File Offset: 0x00010DAC
        [Command("anyplant", "任意种植")]
        public static void AnyPlant(int isCheck)
        {
            Plugin.anyPlant = (isCheck == 1);
        }

        // Token: 0x060001AF RID: 431 RVA: 0x00012BC4 File Offset: 0x00010DC4
        [Command("rowplant", "排山倒海")]
        public static void RowPlant(int isCheck)
        {
            Plugin.rowPlant = (isCheck == 1);
        }

        // Token: 0x060001B0 RID: 432 RVA: 0x00012BDC File Offset: 0x00010DDC
        [Command("daveinvincible", "戴夫无敌")]
        public static void DaveInvincible(int isCheck)
        {
            Plugin.daveInvincible = (isCheck == 1);
        }

        // Token: 0x060001B1 RID: 433 RVA: 0x00012BF4 File Offset: 0x00010DF4
        [Command("randombullet", "随机子弹")]
        public static void RandomBullet(int isCheck)
        {
            Plugin.isRandomBullet = (isCheck == 1);
        }

        // Token: 0x060001B2 RID: 434 RVA: 0x00012C0C File Offset: 0x00010E0C
        [Command("plantinvincible", "植物无敌")]
        public static void PlantInvincible(int isCheck)
        {
            Plugin.isPlantInvincible = (isCheck == 1);
        }

        // Token: 0x060001B3 RID: 435 RVA: 0x00012C24 File Offset: 0x00010E24
        [Command("zombieinvincible", "僵尸无敌")]
        public static void ZombieInvincible(int isCheck)
        {
            Plugin.isZombieInvincible = (isCheck == 1);
        }

        // Token: 0x060001B4 RID: 436 RVA: 0x00012C3C File Offset: 0x00010E3C
        [Command("zombieseckill", "僵尸秒杀")]
        public static void ZombieSecKill(int isCheck)
        {
            Plugin.isZombieSeckill = (isCheck == 1);
        }

        // Token: 0x060001B5 RID: 437 RVA: 0x00012C54 File Offset: 0x00010E54
        [Command("plantseckill", "植物秒杀")]
        public static void PlantSecKill(int isCheck)
        {
            Plugin.isPlantSeckill = (isCheck == 1);
        }

        // Token: 0x060001B6 RID: 438 RVA: 0x00012C6C File Offset: 0x00010E6C
        [Command("stopoutzombie", "停止出怪")]
        public static void StopOutZombie(int isCheck)
        {
            Plugin.stopZombieStart = (isCheck == 1);
        }

        // Token: 0x060001B7 RID: 439 RVA: 0x00012C84 File Offset: 0x00010E84
        [Command("locksun", "锁定阳光")]
        public static void LockSun(int isCheck)
        {
            Plugin.lockSun = (isCheck == 1);
        }

        // Token: 0x060001B8 RID: 440 RVA: 0x00012C9C File Offset: 0x00010E9C
        [Command("mindallzombie", "魅惑所有僵尸")]
        public static void MindAllZombie()
        {
            foreach (Zombie zombie in Board.Instance.zombieArray)
            {
                if (zombie != null)
                {
                    zombie.SetMindControl(0);
                }
            }
        }

        // Token: 0x060001B9 RID: 441 RVA: 0x00012CDC File Offset: 0x00010EDC
        [Command("killallzombies", "秒杀全部僵尸")]
        public static void KillAllZombies()
        {
            List<Zombie> zombies = [.. Board.Instance.zombieArray];
            foreach(var z in zombies)
            {
                if(z!=null && !z.IsDestroyed() && !z.isMindControlled)
                {
                    z.Die(1);
                }
            }
        }

        // Token: 0x060001BA RID: 442 RVA: 0x00012D24 File Offset: 0x00010F24
        [Command("getboardplants", "获取场上植物")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void GetBoardPlants()
        {
            string text = "";
            foreach (Plant plant in Board.Instance.boardEntity.plantArray)
            {
                if (plant != null)
                {
                    string str = text;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 4);
                    defaultInterpolatedStringHandler.AppendLiteral("{\"plant_id\": ");
                    defaultInterpolatedStringHandler.AppendFormatted<PlantType>(plant.thePlantType);
                    defaultInterpolatedStringHandler.AppendLiteral(",\"plant_row\": ");
                    defaultInterpolatedStringHandler.AppendFormatted<int>(plant.thePlantRow);
                    defaultInterpolatedStringHandler.AppendLiteral(",\"plant_col\": ");
                    defaultInterpolatedStringHandler.AppendFormatted<int>(plant.thePlantColumn);
                    defaultInterpolatedStringHandler.AppendLiteral(",\"plant_health\": ");
                    defaultInterpolatedStringHandler.AppendFormatted<int>(plant.thePlantHealth);
                    defaultInterpolatedStringHandler.AppendLiteral("},\n");
                    text = str + defaultInterpolatedStringHandler.ToStringAndClear();
                }
            }
            // (intentionally no output target here; original code just built the string)
        }

        // Token: 0x060001BB RID: 443 RVA: 0x00012DF8 File Offset: 0x00010FF8
        public static void SetMindZombieColor(int isCheck)
        {
            bool flag = isCheck == 1;
            foreach (Zombie zombie in Board.Instance.zombieArray)
            {
                if (zombie != null && zombie.isMindControlled)
                {
                    if (!flag)
                    {
                        zombie.UpdateColor((Zombie.ZombieColor)2);
                    }
                    else
                    {
                        zombie.UpdateColor(0);
                    }
                }
            }
        }

        // Token: 0x060001BC RID: 444 RVA: 0x00012E50 File Offset: 0x00011050
        public static void IsMomentZombieDie(int isCheck)
        {
            Plugin.isMomentZombieDie = (isCheck == 1);
        }

        // Token: 0x060001BD RID: 445 RVA: 0x00012E68 File Offset: 0x00011068
        [Command("killallplants", "清除所有植物")]
        public static void KillAllPlants()
        {
            foreach (Plant plant in Board.Instance.boardEntity.plantArray)
            {
                if (plant != null)
                {
                    Object.Destroy(plant.gameObject);
                }
            }
        }

        // Token: 0x060001BE RID: 446 RVA: 0x00012EB0 File Offset: 0x000110B0
        [Command("cleanall", "清除场上所有")]
        public static void CleanAll()
        {
            foreach (Zombie zombie in Board.Instance.zombieArray)
            {
                if (zombie != null)
                {
                    Object.Destroy(zombie.gameObject);
                }
            }
            foreach (Plant plant in Board.Instance.boardEntity.plantArray)
            {
                if (plant != null)
                {
                    Object.Destroy(plant.gameObject);
                }
            }
            foreach (Bullet bullet in Board.Instance.boardEntity.bulletArray)
            {
                if (bullet != null)
                {
                    Object.Destroy(bullet.gameObject);
                }
            }
        }

        // Token: 0x060001BF RID: 447 RVA: 0x00012F6C File Offset: 0x0001116C
        [Command("changeres", "临时修改指定资源")]
        public static void ChangeRes(string objName, string path)
        {
            foreach (SpriteRenderer spriteRenderer in Object.FindObjectsOfType<SpriteRenderer>())
            {
                if (spriteRenderer.gameObject.name == objName)
                {
                    Console.WriteLine(spriteRenderer.name);
                    spriteRenderer.sprite = PublicMethods.LoadSpriteFromFile(path);
                }
            }
        }

        // Token: 0x060001C0 RID: 448 RVA: 0x00012FDC File Offset: 0x000111DC
        [Command("setmix", "无")]
        public static void SetMix(int row, int col, int value)
        {
            PublicMethods.SetMixData(row, col, value);
        }

        // Token: 0x060001C1 RID: 449 RVA: 0x00012FF4 File Offset: 0x000111F4
        [Command("road", "无")]
        public static void SetRoadType(int roadType)
        {
            for (int i = 0; i < Board.Instance.rowNum; i++)
            {
                Board.Instance.roadType[i] = (BoxType)roadType;
            }
        }

        // Token: 0x060001C2 RID: 450 RVA: 0x00013028 File Offset: 0x00011228
        [Command("showzombielist", "无")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ShowZombieList()
        {
            ManualLogSource field = Plugin.Field0;
            bool flag;
            var bepInExMessageLogInterpolatedStringHandler =
                new BepInExMessageLogInterpolatedStringHandler(29, 1, out flag);
            if (flag)
            {
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral("========== ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>("InitZombieList");
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral(" 数据调试信息 ==========");
            }
            field.LogMessage(bepInExMessageLogInterpolatedStringHandler);
            ManualLogSource field2 = Plugin.Field0;
            bepInExMessageLogInterpolatedStringHandler =
                new BepInExMessageLogInterpolatedStringHandler(5, 2, out flag);
            if (flag)
            {
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral("├─ ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>("multiplier");
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral(": ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<int>(InitZombieList.multiplier);
            }
            field2.LogMessage(bepInExMessageLogInterpolatedStringHandler);
            ManualLogSource field3 = Plugin.Field0;
            bepInExMessageLogInterpolatedStringHandler =
                new BepInExMessageLogInterpolatedStringHandler(5, 2, out flag);
            if (flag)
            {
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral("├─ ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>("theMaxWave");
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral(": ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<int>(InitZombieList.theMaxWave);
            }
            field3.LogMessage(bepInExMessageLogInterpolatedStringHandler);
            ManualLogSource field4 = Plugin.Field0;
            bepInExMessageLogInterpolatedStringHandler =
                new BepInExMessageLogInterpolatedStringHandler(5, 2, out flag);
            if (flag)
            {
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral("├─ ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>("zombiePoint");
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral(": ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<int>(InitZombieList.zombiePoint);
            }
            field4.LogMessage(bepInExMessageLogInterpolatedStringHandler);
            ManualLogSource field5 = Plugin.Field0;
            bepInExMessageLogInterpolatedStringHandler =
                new BepInExMessageLogInterpolatedStringHandler(11, 2, out flag);
            if (flag)
            {
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral("├─ ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>("zombieList");
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral(" (共 ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<int>(InitZombieList.zombieList.Count);
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral(" 波):");
            }
            field5.LogMessage(bepInExMessageLogInterpolatedStringHandler);
            for (int i = 0; i < InitZombieList.zombieList.Count; i++)
            {
                Il2CppSystem.Collections.Generic.List<ZombieSpawnData> list = InitZombieList.zombieList[i];
                ManualLogSource field6 = Plugin.Field0;
                bepInExMessageLogInterpolatedStringHandler =
                    new BepInExMessageLogInterpolatedStringHandler(20, 2, out flag);
                if (flag)
                {
                    bepInExMessageLogInterpolatedStringHandler.AppendLiteral("│  └─ 第 ");
                    bepInExMessageLogInterpolatedStringHandler.AppendFormatted<int>(i + 1);
                    bepInExMessageLogInterpolatedStringHandler.AppendLiteral(" 波 (共 ");
                    bepInExMessageLogInterpolatedStringHandler.AppendFormatted<int>(list.Count);
                    bepInExMessageLogInterpolatedStringHandler.AppendLiteral(" 种僵尸):");
                }
                field6.LogMessage(bepInExMessageLogInterpolatedStringHandler);
                foreach (ZombieSpawnData zombieSpawnData in list)
                {
                    ManualLogSource field7 = Plugin.Field0;
                    bepInExMessageLogInterpolatedStringHandler =
                        new BepInExMessageLogInterpolatedStringHandler(9, 1, out flag);
                    if (flag)
                    {
                        bepInExMessageLogInterpolatedStringHandler.AppendLiteral("│     ├─ ");
                        bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>(ItemTypes.GetNameById(zombieSpawnData.zombieType.ToString(), ItemTypes.zombies));
                    }
                    field7.LogMessage(bepInExMessageLogInterpolatedStringHandler);
                }
            }
            ManualLogSource field8 = Plugin.Field0;
            bepInExMessageLogInterpolatedStringHandler =
                new BepInExMessageLogInterpolatedStringHandler(11, 2, out flag);
            if (flag)
            {
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral("├─ ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<string>("zombieTypeList");
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral(" (共 ");
                bepInExMessageLogInterpolatedStringHandler.AppendFormatted<int>(InitZombieList.zombieTypeList.Count);
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral(" 种):");
            }
            field8.LogMessage(bepInExMessageLogInterpolatedStringHandler);
            foreach (ZombieType zombieType in InitZombieList.zombieTypeList)
            {
                ManualLogSource field9 = Plugin.Field0;
                bepInExMessageLogInterpolatedStringHandler =
                    new BepInExMessageLogInterpolatedStringHandler(6, 1, out flag);
                if (flag)
                {
                    bepInExMessageLogInterpolatedStringHandler.AppendLiteral("│  ├─ ");
                    BepInExLogInterpolatedStringHandler bepInExLogInterpolatedStringHandler = bepInExMessageLogInterpolatedStringHandler;
                    int num = (int)zombieType;
                    bepInExLogInterpolatedStringHandler.AppendFormatted<string>(ItemTypes.GetNameById(num.ToString(), ItemTypes.zombies));
                }
                field9.LogMessage(bepInExMessageLogInterpolatedStringHandler);
            }
            ManualLogSource field10 = Plugin.Field0;
            bepInExMessageLogInterpolatedStringHandler =
                new BepInExMessageLogInterpolatedStringHandler(28, 0, out flag);
            if (flag)
            {
                bepInExMessageLogInterpolatedStringHandler.AppendLiteral("========== 调试信息结束 ==========");
            }
            field10.LogMessage(bepInExMessageLogInterpolatedStringHandler);
        }

        // Token: 0x060001C3 RID: 451 RVA: 0x00013340 File Offset: 0x00011540
        [Command("changeimg", "临时修改指定资源")]
        public static void ChangeImg(string objName, string path)
        {
            foreach (Image image in Object.FindObjectsOfType<Image>())
            {
                if (image.gameObject.name == objName)
                {
                    image.sprite = PublicMethods.LoadSpriteFromFile(path);
                }
            }
        }

        // Token: 0x060001C4 RID: 452 RVA: 0x000133A8 File Offset: 0x000115A8
        [Command("reversalplant", "无")]
        public static void ReversalPlant()
        {
            Plugin.isReversalPlant = !Plugin.isReversalPlant;
            foreach (Plant plant in Board.Instance.boardEntity.plantArray)
            {
                Transform transform = plant.transform;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        // Token: 0x060001C5 RID: 453 RVA: 0x0001341C File Offset: 0x0001161C
        [Command("createmower", "创建小推车")]
        public static void CreateBoardMower()
        {
            for (int i = 0; i < Board.Instance.rowNum; i++)
            {
                CreateMower.Instance.SetMower(0, -6f, i);
            }
        }

        // Token: 0x060001C6 RID: 454 RVA: 0x00013450 File Offset: 0x00011650
        [Command("cleanmower", "清除所有小推车")]
        public static void CleanMower()
        {
            for (int i = 0; i < Board.Instance.mowerArray.Count; i++)
            {
                if (Board.Instance.mowerArray[i] != null)
                {
                    Board.Instance.mowerArray[i].Die();
                }
            }
        }

        // Token: 0x060001C7 RID: 455 RVA: 0x000134A4 File Offset: 0x000116A4
        [Command("gomower", "启动所有小推车")]
        public static void GoMower()
        {
            foreach (Mower mower in Board.Instance.mowerArray)
            {
                if (mower != null)
                {
                    mower.StartMove();
                }
            }
        }

        // Token: 0x060001C8 RID: 456 RVA: 0x000134E4 File Offset: 0x000116E4
        [Command("randomput", "随机位置放置指定植物")]
        public static void RandomDom(int type, float chance)
        {
            for (int i = 0; i < Board.Instance.rowNum; i++)
            {
                for (int j = 0; j < Board.Instance.columnNum; j++)
                {
                    if ((float)Random.Range(0, 1) < chance)
                    {
                        PublicMethods.SetBoardPlant(j, i, type, 1);
                    }
                }
            }
        }

        // Token: 0x060001C9 RID: 457 RVA: 0x00013530 File Offset: 0x00011730
        [Command("testplant", "测试所有二创植物")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void TestPlant()
        {
            List<PlantType> list = new List<PlantType>();
            foreach (PlantType plantType in GameAPP.resourcesManager.allPlants)
            {
                if (!Enum.IsDefined(typeof(PlantType), plantType))
                {
                    list.Add(plantType);
                }
            }
            for (int i = 0; i < Board.Instance.rowNum; i++)
            {
                for (int j = 0; j < Board.Instance.columnNum; j++)
                {
                    if (CommandMethods.tindex >= list.Count)
                    {
                        Debug.Log("所有二创植物已种完！");
                        CommandMethods.tindex = 0;
                        goto IL_D4;
                    }
                    CreatePlant.Instance.SetPlant(j, i, list[CommandMethods.tindex], null, default(Vector2), true, true, null);
                    CommandMethods.tindex++;
                }
            }
            IL_D4:
            for (int k = 0; k < Board.Instance.rowNum; k++)
            {
                CreateZombie.Instance.SetZombie(k, (ZombieType)54, 9.9f, false);
            }
        }

        // Token: 0x060001CA RID: 458 RVA: 0x00013640 File Offset: 0x00011840
        [Command("findplantid", "导出所有植物的预制体和id")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void FindPlantId()
        {
            string path = Path.Combine(Application.dataPath, "PlantIds.txt");
            List<string> list = new();
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<PlantType, GameObject> keyValuePair in GameAPP.resourcesManager.plantPrefabs)
            {
                if (keyValuePair != null)
                {
                    List<string> list2 = list;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("ID:");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair.Key);
                    defaultInterpolatedStringHandler.AppendLiteral(",");
                    defaultInterpolatedStringHandler.AppendFormatted(keyValuePair.Value.name);
                    list2.Add(defaultInterpolatedStringHandler.ToStringAndClear());
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllLines(path, list);
        }

        // Token: 0x060001CB RID: 459 RVA: 0x000136F4 File Offset: 0x000118F4
        [Command("findzombieid", "导出所有僵尸的预制体和id")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void FindZombieId()
        {
            string path = Path.Combine(Application.dataPath, "ZombieIds.txt");
            List<string> list = new();
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ZombieType, GameObject> keyValuePair in GameAPP.resourcesManager.zombiePrefabs)
            {
                if (keyValuePair != null)
                {
                    List<string> list2 = list;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("ID:");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair.Key);
                    defaultInterpolatedStringHandler.AppendLiteral(",");
                    defaultInterpolatedStringHandler.AppendFormatted(keyValuePair.Value.name);
                    list2.Add(defaultInterpolatedStringHandler.ToStringAndClear());
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllLines(path, list);
        }

        // Token: 0x060001CC RID: 460 RVA: 0x000137A8 File Offset: 0x000119A8
        [Command("findbulletid", "导出所有子弹的预制体和id")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void FindBulletId()
        {
            string path = Path.Combine(Application.dataPath, "BulletIds.txt");
            List<string> list = new();
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<BulletType, GameObject> keyValuePair in GameAPP.resourcesManager.bulletPrefabs)
            {
                if (keyValuePair != null)
                {
                    List<string> list2 = list;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("ID:");
                    defaultInterpolatedStringHandler.AppendFormatted<int>((int)keyValuePair.Key);
                    defaultInterpolatedStringHandler.AppendLiteral(",");
                    defaultInterpolatedStringHandler.AppendFormatted(keyValuePair.Value.name);
                    list2.Add(defaultInterpolatedStringHandler.ToStringAndClear());
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllLines(path, list);
        }

        // Token: 0x060001CD RID: 461 RVA: 0x0001385C File Offset: 0x00011A5C
        [Command("finditemid", "导出所有小物件的预制体和id")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void FindItemId()
        {
            string path = Path.Combine(Application.dataPath, "ItemIds.txt");
            List<string> list = new();
            for (int i = 0; i < GameAPP.itemPrefab.Length; i++)
            {
                if (GameAPP.itemPrefab[i] != null)
                {
                    List<string> list2 = list;
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("ID:");
                    defaultInterpolatedStringHandler.AppendFormatted<int>(i);
                    defaultInterpolatedStringHandler.AppendLiteral(",");
                    defaultInterpolatedStringHandler.AppendFormatted(GameAPP.itemPrefab[i].name);
                    list2.Add(defaultInterpolatedStringHandler.ToStringAndClear());
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllLines(path, list);
        }

        // Token: 0x060001CE RID: 462 RVA: 0x00013914 File Offset: 0x00011B14
        [Command("setdroppedcard", "指定位置掉落卡片")]
        public static void SetDroppedCard(int col, int row, int type)
        {
            Lawnf.SetDroppedCard(new Vector2(Mouse.Instance.GetBoxXFromColumn(col), Mouse.Instance.GetBoxYFromRow(row)), (PlantType)type, 0);
        }

        // Token: 0x060001CF RID: 463 RVA: 0x00013944 File Offset: 0x00011B44
        [Command("changetext", "修改关卡名称")]
        public static void ChangeText(string text)
        {
            TextMeshProUGUI[] array = new TextMeshProUGUI[6];
            int num = 0;
            TextMeshProUGUI levelName = InGameUI.Instance.LevelName1;
            array[num] = ((levelName != null) ? levelName.GetComponent<TextMeshProUGUI>() : null);
            int num2 = 1;
            TextMeshProUGUI levelName2 = InGameUI.Instance.LevelName1;
            array[num2] = ((levelName2 != null) ? levelName2.transform.GetChild(0).GetComponent<TextMeshProUGUI>() : null);
            int num3 = 2;
            TextMeshProUGUI levelName3 = InGameUI.Instance.LevelName2;
            array[num3] = ((levelName3 != null) ? levelName3.GetComponent<TextMeshProUGUI>() : null);
            int num4 = 3;
            TextMeshProUGUI levelName4 = InGameUI.Instance.LevelName2;
            array[num4] = ((levelName4 != null) ? levelName4.transform.GetChild(0).GetComponent<TextMeshProUGUI>() : null);
            int num5 = 4;
            TextMeshProUGUI levelName5 = InGameUI.Instance.LevelName3;
            array[num5] = ((levelName5 != null) ? levelName5.GetComponent<TextMeshProUGUI>() : null);
            int num6 = 5;
            TextMeshProUGUI levelName6 = InGameUI.Instance.LevelName3;
            array[num6] = ((levelName6 != null) ? levelName6.transform.GetChild(0).GetComponent<TextMeshProUGUI>() : null);
            TextMeshProUGUI[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i].text = text;
            }
        }

        // Token: 0x060001D0 RID: 464 RVA: 0x00013A28 File Offset: 0x00011C28
        [Command("sendtcpmsg", "无")]
        public static void SendTcpMsg(string msg)
        {
            Plugin.SendMessage(msg);
        }

        // Token: 0x060001D1 RID: 465 RVA: 0x00013A3C File Offset: 0x00011C3C
        [Command("isabyssmoney", "无限叶绿素")]
        public static void IsAbyssMoney(int isCheck)
        {
            Plugin.isAbyssMoney = (isCheck == 1);
        }

        // Token: 0x060001D2 RID: 466 RVA: 0x00013A54 File Offset: 0x00011C54
        [Command("isabyssrefreshcount", "深渊词条无限刷新")]
        public static void IsAbyssRefreshCount(int isCheck)
        {
            Plugin.isAbyssRefreshCount = (isCheck == 1);
        }

        // Token: 0x060001D3 RID: 467 RVA: 0x00013A6C File Offset: 0x00011C6C
        [Command("isabyssmaxplantcount", "解除植物种植数量限制")]
        public static void IsAbyssMaxPlantCount(int isCheck)
        {
            Plugin.isAbyssMaxPlantCount = (isCheck == 1);
        }

        // Token: 0x060001D4 RID: 468 RVA: 0x00013A84 File Offset: 0x00011C84
        [Command("isabyssmaxbuffcount", "解除最大buff数限制")]
        public static void IsAbyssMaxBuffCount(int isCheck)
        {
            Plugin.isAbyssMaxBuffCount = (isCheck == 1);
        }

        // Token: 0x060001D5 RID: 469 RVA: 0x00013A9C File Offset: 0x00011C9C
        [Command("isbannedinabyss", "解除深渊帝果限制")]
        public static void IsBannedInAbyss(int isCheck)
        {
            Plugin.isBannedInAbyss = (isCheck == 1);
        }

        // Token: 0x060001D6 RID: 470 RVA: 0x00013AB4 File Offset: 0x00011CB4
        [Command("isabyssswordstar", "解除剑仙杨桃限制")]
        public static void IsAbyssSwordStar(int isCheck)
        {
            Plugin.isAbyssSwordStar = (isCheck == 1);
        }

        // Token: 0x060001D7 RID: 471 RVA: 0x00013ACC File Offset: 0x00011CCC
        [Command("issolarsunflower", "解除究极向日葵限制")]
        public static void IsSolarSunflower(int isCheck)
        {
            Plugin.isSolarSunflower = (isCheck == 1);
        }

        // Token: 0x060001D8 RID: 472 RVA: 0x00013AE4 File Offset: 0x00011CE4
        [Command("isultimateminigun", "解除速射机枪限制")]
        public static void IsUltimateMinigun(int isCheck)
        {
            Plugin.isUltimateMinigun = (isCheck == 1);
        }

        // Token: 0x060001D9 RID: 473 RVA: 0x00013AFC File Offset: 0x00011CFC
        [Command("isrich", "成为富哥锁定金币")]
        public static void IsRich(int isCheck)
        {
            Plugin.isRich = (isCheck == 1);
        }

        // Token: 0x060001DA RID: 474 RVA: 0x00013B14 File Offset: 0x00011D14
        [Command("isscaredydream", "开启胆小菇之梦")]
        public static void IsScaredyDream(int isCheck)
        {
            Plugin.isScaredyDream = (isCheck == 1);
        }

        // Token: 0x060001DB RID: 475 RVA: 0x00013B2C File Offset: 0x00011D2C
        [Command("appointbullet", "指定植物子弹")]
        public static void appointBullet(int id)
        {
            Plugin.appointBullet = id;
        }

        // Token: 0x060001DC RID: 476 RVA: 0x00013B40 File Offset: 0x00011D40
        [Command("mousebullet", "开启鼠标子弹")]
        public static void MouseBullet(int isCheck)
        {
        }

        // Token: 0x060001DD RID: 477 RVA: 0x00013B58 File Offset: 0x00011D58
        [Command("isthrowertype", "玉米类必出黄油")]
        public static void IsThrowerType(int isCheck)
        {
            Plugin.isThrowerType = (isCheck == 1);
        }

        // Token: 0x060001DE RID: 478 RVA: 0x00013B70 File Offset: 0x00011D70
        [Command("issamespeed", "同步相同僵尸移速")]
        public static void IsSameSpeed(int isCheck)
        {
            Plugin.isSameSpeed = (isCheck == 1);
        }

        // Token: 0x060001DF RID: 479 RVA: 0x00013B88 File Offset: 0x00011D88
        [Command("ismanyhittimes", "子弹无限穿透")]
        public static void IsManyHitTimes(int isCheck)
        {
            Plugin.isManyHitTimes = (isCheck == 1);
        }

        // Token: 0x060001E0 RID: 480 RVA: 0x00013BA0 File Offset: 0x00011DA0
        [Command("iscobcannonshoop", "加农炮无CD装填")]
        public static void IsCobCannonShoop(int isCheck)
        {
            Plugin.isCobCannonShoop = (isCheck == 1);
        }

        // Token: 0x060001E1 RID: 481 RVA: 0x00013BB8 File Offset: 0x00011DB8
        [Command("setbulletatk", "设置子弹伤害")]
        public static void SetBulletAtk(int atk)
        {
            Plugin.bulletAtk = atk;
        }

        // Token: 0x060001E2 RID: 482 RVA: 0x00013BCC File Offset: 0x00011DCC
        [Command("setabym", "设置叶绿素")]
        public static void SetAbym(int money)
        {
            //AbyssManager.Instance.Money = money;
        }

        // Token: 0x060001E3 RID: 483 RVA: 0x00013BE4 File Offset: 0x00011DE4
        [Command("supershooter", "超射开大送小美")]
        public static void SuperShooter(int isCheck)
        {
            Plugin.superShooter = (isCheck == 1);
        }

        // Token: 0x060001E4 RID: 484 RVA: 0x00013BFC File Offset: 0x00011DFC
        [Command("plantabnormal", "植物变态射速")]
        public static void PlantAbnormal(int isCheck)
        {
            Plugin.isPlantAbnormalShoot = (isCheck == 1);
        }

        // Token: 0x060001E5 RID: 485 RVA: 0x00013C14 File Offset: 0x00011E14
        [Command("manypoints", "无限积分")]
        public static void ManyPoints(int isCheck)
        {
            Plugin.isManyPoints = (isCheck == 1);
        }

        // Token: 0x060001E6 RID: 486 RVA: 0x00013C2C File Offset: 0x00011E2C
        [Command("travelrefresh", "旅行词条无限刷新")]
        public static void TravelRefresh(int isCheck)
        {
            Plugin.isTravelRefresh = (isCheck == 1);
        }

        // Token: 0x060001E7 RID: 487 RVA: 0x00013C44 File Offset: 0x00011E44
        [Command("travelplant", "开启旅行植物普通模式可种植")]
        public static void TravelPlant(int isCheck)
        {
            Plugin.isTravelPlant = (isCheck == 1);
        }

        // Token: 0x060001E8 RID: 488 RVA: 0x00013C5C File Offset: 0x00011E5C
        [Command("mousebulletway", "设置鼠标子弹属性")]
        public static void MouseBulletWay(int type, int way, int atk)
        {
        }

        // Token: 0x060001E9 RID: 489 RVA: 0x00013C7C File Offset: 0x00011E7C
        [Command("iszombiesea", "僵尸海")]
        public static void IsZombieSea(int isCheck)
        {
            Plugin.isZombieSea = (isCheck == 1);
        }

        // Token: 0x060001EA RID: 490 RVA: 0x00013C94 File Offset: 0x00011E94
        [Command("zombieseatype", "僵尸海类型")]
        public static void ZombieSeaType(int type)
        {
            Plugin.zombieSeaType = type;
        }

        // Token: 0x060001EB RID: 491 RVA: 0x00013CA8 File Offset: 0x00011EA8
        [Command("setzombiehpmu", "设置僵尸血量倍率")]
        public static void SetZombieHoMu(int mu)
        {
            Plugin.zombieHpMu = mu;
        }

        // Token: 0x060001EC RID: 492 RVA: 0x00013CBC File Offset: 0x00011EBC
        [Command("exportplantres", "无")]
        public static void ExportPlantRes(int plantID)
        {
            PublicMethods.ExportSpritesPngFromPrefab(GameAPP.resourcesManager.plantPrefabs[(PlantType)plantID]);
        }

        // Token: 0x060001ED RID: 493 RVA: 0x00013CE0 File Offset: 0x00011EE0
        [Command("exportzombieres", "无")]
        public static void ExportZombieRes(int zombieID)
        {
            PublicMethods.ExportSpritesPngFromPrefab(GameAPP.resourcesManager.zombiePrefabs[(ZombieType)zombieID]);
        }

        // Token: 0x060001EE RID: 494 RVA: 0x00013D04 File Offset: 0x00011F04
        [Command("exportplantpreview", "无")]
        public static void ExportPlantPreview()
        {
            PublicMethods.ExportSpritesPngFromPreview();
        }

        // Token: 0x060001EF RID: 495 RVA: 0x00013D18 File Offset: 0x00011F18
        [Command("createitem", "掉落小物件")]
        public static void CreateItemM(int col, int row, int itemID)
        {
            CreateItem.Instance.SetCoin(col, row, itemID, 3, default(Vector3), false);
        }

        // Token: 0x060001F0 RID: 496 RVA: 0x00013D40 File Offset: 0x00011F40
        [Command("setgriditem", "无")]
        public static void SetGridItem(int col, int row, int type)
        {
            GridItem.SetGridItem(col, row, (GridItemType)type, 0);
        }

        // Token: 0x060001F1 RID: 497 RVA: 0x00013D58 File Offset: 0x00011F58
        [Command("setplanthp", "无")]
        public static void SetPlantHp(int plantType, int HP)
        {
            foreach (Plant plant in Lawnf.GetAllPlants())
            {
                if ((int)plant.thePlantType == plantType)
                {
                    plant.thePlantHealth = HP + 1;
                    plant.TakeDamage(0, plant.TryCast<IDamageMaker>(), 0);
                }
            }
        }

        // Token: 0x060001F2 RID: 498 RVA: 0x00013DA4 File Offset: 0x00011FA4
        [Command("setfreezedplant", "无")]
        public static void SetFreezedPlant(int col, int row, int id)
        {
            GridItem gridItem = GridItem.SetGridItem(col, row, GridItemType.IceBlock, 0);
            FreezedPlant freezedPlant = (gridItem != null) ? gridItem.GetComponent<FreezedPlant>() : null;
            if (freezedPlant != null)
            {
                freezedPlant.InitFreezedPlant((PlantType)id);
            }
        }

        // Token: 0x060001F3 RID: 499 RVA: 0x00013DD8 File Offset: 0x00011FD8
        [Command("allplantupgrade", "无")]
        public static void AllPlantUpgrade()
        {
            foreach (Plant plant in Board.Instance.boardEntity.plantArray)
            {
                if (plant != null)
                {
                    plant.Upgrade(3, true, false);
                }
            }
        }

        // Token: 0x060001F4 RID: 500 RVA: 0x00013E20 File Offset: 0x00012020
        [Command("fulltreasureplant", "无")]
        public static void FullTreasurePlant()
        {
            foreach (PlantType a_ in GameAPP.resourcesManager.allPlants)
            {
                TreasureCardData treasureCardData = new TreasureCardData(a_, 40, 40);
                TreasureData.treasureCards.Add(treasureCardData);
            }
        }

        // Token: 0x060001F5 RID: 501 RVA: 0x00013E64 File Offset: 0x00012064
        [Command("changetreasuremoney", "无")]
        public static void ChangeTreasureMoney(int num)
        {
            TreasureData.treasureMoney = num;
        }

        // Token: 0x060001F6 RID: 502 RVA: 0x00013E78 File Offset: 0x00012078
        [Command("changetreasuretime", "无")]
        public static void ChangeTreasureTime(int num)
        {
            TreasureManager.Instance.maxTimer = (float)num;
        }

        // Token: 0x060001F7 RID: 503 RVA: 0x00013E94 File Offset: 0x00012094
        [Command("cleartreasureplant", "无")]
        public static void ClearTreasurePlant()
        {
            TreasureWarehouseMenu instance = TreasureWarehouseMenu.Instance;
            foreach (TreasureCard treasureCard in ((instance != null) ? instance.cards : null))
            {
                if (treasureCard != null)
                {
                    treasureCard.Sell();
                }
            }
        }

        // Token: 0x060001F8 RID: 504 RVA: 0x00013ED4 File Offset: 0x000120D4
        [Command("addtreasurecard", "无")]
        public static void AddTreasureCard(int p, int a, int b)
        {
            TreasureCardData treasureCardData = new TreasureCardData((PlantType)p, a, b);
            TreasureData.treasureCards.Add(treasureCardData);
        }

        // Token: 0x060001F9 RID: 505 RVA: 0x00013EF8 File Offset: 0x000120F8
        [Command("treasurelucky", "无")]
        public static void TreasureLucky(int open, int r, int o, int p, int b, int g, int w)
        {
            Plugin.isTreasureLucky = (open == 1);
            Plugin.tr = r;
            Plugin.to = o;
            Plugin.tp = p;
            Plugin.tb = b;
            Plugin.tg = g;
            Plugin.tw = w;
        }

        // Token: 0x060001FA RID: 506 RVA: 0x00013F38 File Offset: 0x00012138
        [Command("for", "无")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void For(int start, int end, string cmd)
        {
            for (int i = start; i < end; i++)
            {
                CommandMethods.ProcessCommand(cmd.Replace("{i}", i.ToString()));
            }
        }

        // Token: 0x060001FB RID: 507 RVA: 0x00013F68 File Offset: 0x00012168
        [Command("easteregg", "无")]
        public static void EasterEgg()
        {
            switch (Random.Range(0, 4))
            {
            case 0:
                CommandMethods.AllPlantUpgrade();
                return;
            case 1:
                for (int i = 0; i < Board.Instance.rowNum; i++)
                {
                    for (int j = 0; j < Board.Instance.columnNum; j++)
                    {
                        Board.Instance.boardAction.SetDoom(j, i, false, false, default(Vector2), 1800, 0, null, true, PlantType.Nothing);
                    }
                }
                return;
            case 2:
                for (int k = 0; k < Board.Instance.rowNum; k++)
                {
                    for (int l = 0; l < Board.Instance.columnNum; l++)
                    {
                        Board.Instance.boardAction.SetDoom(l, k, true, false, default(Vector2), 1800, 0, null, true, PlantType.Nothing);
                    }
                }
                return;
            case 3:
                for (int m = 0; m < Board.Instance.rowNum; m++)
                {
                    for (int n = 0; n < Board.Instance.columnNum; n++)
                    {
                        Board.Instance.boardAction.SetDoom(n, m, false, true, default(Vector2), 1800, 0, null, true, PlantType.Nothing);
                    }
                }
                return;
            default:
                return;
            }
        }

        // Token: 0x040000EA RID: 234
        public static int tindex;

        // Token: 0x02000061 RID: 97
        [CompilerGenerated]
        [System.Serializable]
        private sealed class Class31
        {
            // (kept only as a proxy/func class, but no ConfuserEx junk methods)
            public static readonly CommandMethods.Class31 Field0 = new CommandMethods.Class31();
        }
    }
}
