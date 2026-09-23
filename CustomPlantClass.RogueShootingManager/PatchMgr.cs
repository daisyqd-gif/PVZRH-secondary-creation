using System.Linq;
using System.Runtime.CompilerServices;
using Il2CppSystem.Linq;
using UI;

namespace CustomPlantClass.RogueShootingManager
{
    [HarmonyPatch(typeof(ShootingManager))]
    public static class ShootingManagerPatch
    {
        [HarmonyPatch(nameof(ShootingManager.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(ShootingManager __instance)
        {
            InjectPlantTypes(__instance);
        }
        [HarmonyPatch(nameof(ShootingManager.Start))]
        [HarmonyPostfix]
		public static void StartAwake_Postfix(ShootingManager __instance)
        {
            InjectPlantTypes(__instance);
        }
		private static void InjectPlantTypes(ShootingManager __instance)
		{
			bool flag = __instance == null;
			if (!flag)
			{
				bool flag2 = !true;
				if (!flag2)
				{
					var restPlants = __instance.RestPlants;
					bool flag3 = restPlants != null;
					if (flag3)
					{
						foreach (PlantType item in RegistryHelper.CustomBasePlants)
						{
							bool flag4 = !restPlants.Contains(item);
							if (flag4)
							{
								restPlants.Add(item);
							}
						}
					}
					var expertPlants = __instance.ExpertPlants;
					bool flag5 = expertPlants != null;
					if (flag5)
					{
						foreach (PlantType item2 in RegistryHelper.experts)
						{
							bool flag6 = !expertPlants.Contains(item2);
							if (flag6)
							{
								expertPlants.Add(item2);
							}
						}
					}
					var allPlants = __instance.AllPlants;
					bool flag7 = allPlants != null;
					if (flag7)
					{
						foreach (PlantType item3 in RegistryHelper.CustomBasePlants)
						{
							bool flag8 = !allPlants.Contains(item3);
							if (flag8)
							{
								allPlants.Add(item3);
							}
						}
					}
				}
			}
		}
        [HarmonyPatch(nameof(ShootingManager.RefisterMissionBuff))]
        [HarmonyPostfix]
        public static void RegisterMissionBuff_Postfix(ShootingManager __instance, ref MultipleChoiceMenu menu)
        {
            // 检测有没有出试炼词条
            foreach (var item in menu.optionDatas)
                if (item.title.Contains("试炼")) return;
            Il2CppSystem.Collections.Generic.List<(string name, string desc, Func<ShootingManager, bool> canGet, Action<ShootingManager> onGet)> availableBuffs = new();
            foreach( var i in RegistryHelper.CustomMissionBuffs )
            {
                if (i.canGet(__instance))
                {
                    availableBuffs.Add(i);
                }
            }
            if(availableBuffs.Count == 0) return;
            var b = availableBuffs.GetRandom();
            var action = () =>
            {
                if (__instance == null) return;
                b.onGet(__instance);
            };
            menu.RegisterOption(b.name, b.desc, action, frameType: Quality.curse);
        }
    }
	[HarmonyPatch(typeof(ShootingAlmanacCatalog))]
	public static class ShootingAlmanacCatalogPatch
	{
		public static ShootingAlmanacDirectory LastGoodDir = null;
		// Token: 0x0600003D RID: 61 RVA: 0x000045CC File Offset: 0x000027CC
		[HarmonyPatch(nameof(ShootingAlmanacCatalog.Build))]
		[HarmonyPrefix]
		[HarmonyPriority(800)]
		public static bool Build_Prefix(ref ShootingAlmanacDirectory __result)
		{
			/*
			if(LastGoodDir != null)
			{
				__result = LastGoodDir;
				return false;
			}
			*/
            if (Config.configs != null)
            {
                foreach( var i in RegistryHelper.roguePlants )
                {
                    if (!Config.configs.ContainsKey(i.Key))
                    {
                        Config.configs.Add(i.Key, i.Value);
                    }
                }
            }
			bool result;
			try
			{
				bool flag = Config.configs == null;
				if (flag)
				{
					result = true;
				}
				else
				{
					bool flag2 = !_gamePlantsLoaded;
					if (flag2)
					{
                        _gamePlantsLoaded = true;
                        _gamePlants = LoadGamePlantWhitelist();
					}
					bool flag3 = _gamePlants == null || _gamePlants.Count == 0;
					if (flag3)
					{
						result = true;
					}
					else
					{
						HashSet<PlantType> hashSet = RegistryHelper.roguePlants.Keys.ToHashSet();
                        _removedConfigs.Clear();
						foreach (PlantType plantType in hashSet)
						{
							bool flag4 = _gamePlants.Contains(plantType);
							if (!flag4)
							{
								try
								{
									bool flag5 = Config.configs.ContainsKey(plantType);
									if (flag5)
									{
										BaseConfig baseConfig = Config.configs[plantType];
										bool flag6 = baseConfig != null;
										if (flag6)
										{
                                            _removedConfigs[plantType] = baseConfig;
											Config.configs.Remove(plantType);
										}
									}
								}
								catch (Exception ex)
								{
									ModLogger.LogError(ex.ToString());
								}
							}
						}
						result = true;
					}
				}
			}
			catch (Exception t)
			{
				ModLogger.LogError(t.ToString());
                RestoreRemovedConfigs();
				result = false;
			}
			return result;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004804 File Offset: 0x00002A04
		[HarmonyPatch("Build")]
		[HarmonyPostfix]
		[HarmonyPriority(0)]
		public static void Build_Postfix(ShootingAlmanacDirectory __result)
		{
			try
			{
                RestoreRemovedConfigs();
				bool flag = __result != null;
				if (flag)
				{
					AlmanacDirectoryInjector.Inject(__result);
				}
				LastGoodDir = __result;
			}
			catch (Exception t)
			{
				ModLogger.LogError(t.Message);
			}
		}
		[HarmonyPatch("Build")]
		[HarmonyFinalizer]
		public static Exception Finalizer(Exception __exception,
										ref ShootingAlmanacDirectory __result)
		{
			if (__exception is InvalidOperationException)
			{
				ModLogger.LogError($"[Almanac] Build threw: {__exception.Message}");

				// If this run failed, fall back to last known good directory
				if (__result == null && LastGoodDir != null)
					__result = LastGoodDir;

				// From managed side, Build "does not throw"
				return null;
			}

			return __exception;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004880 File Offset: 0x00002A80
		private static void RestoreRemovedConfigs()
		{
			try
			{
				bool flag = Config.configs == null;
				if (!flag)
				{
					foreach (KeyValuePair<PlantType, BaseConfig> keyValuePair in _removedConfigs)
					{
						Config.configs[keyValuePair.Key] = keyValuePair.Value;
					}
					_removedConfigs.Clear();
				}
			}
			catch (Exception t)
			{
				ModLogger.LogError(t.Message);
			}
		}
		private static Type GetConfigType(BaseConfig config)
		{
			Type type = config.GetType();
			bool flag = type == typeof(BaseConfig);
			if (flag)
			{
				type = Type.GetType(config.GetIl2CppType().AssemblyQualifiedName);
			}
			bool flag2 = type == typeof(BaseConfig);
			Type result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				result = type;
			}
			return result;
		}
		private static HashSet<PlantType> LoadGamePlantWhitelist()
		{
			HashSet<PlantType> hashSet = new HashSet<PlantType>();
			try
			{
				TextAsset textAsset = Resources.Load<TextAsset>("UI/Data/ShootingAlmanacIndex");
				bool flag = textAsset == null || string.IsNullOrEmpty(textAsset.text);
				if (flag)
				{
					ModLogger.LogError($"The catalog is invalid! Please fix any errors!");
					return hashSet;
				}
				ShootingAlmanacIndexFile shootingAlmanacIndexFile = JsonUtility.FromJson<ShootingAlmanacIndexFile>(textAsset.text);
				bool flag2 = ((shootingAlmanacIndexFile != null) ? shootingAlmanacIndexFile.plants : null) == null;
				if (flag2)
				{
					return hashSet;
				}
				foreach (ShootingAlmanacIndexedPlant shootingAlmanacIndexedPlant in shootingAlmanacIndexFile.plants)
				{
					bool flag3 = shootingAlmanacIndexedPlant == null || string.IsNullOrEmpty(shootingAlmanacIndexedPlant.plant);
					if (!flag3)
					{
						PlantType item;
						bool flag4 = Enum.TryParse(shootingAlmanacIndexedPlant.plant, out item);
						if (flag4)
						{
							hashSet.Add(item);
						}
					}
				}
			}
			catch (Exception e)
			{
                ModLogger.LogError(e.Message);
			}
			return hashSet;
		}

		// Token: 0x0400001A RID: 26
		private static readonly Dictionary<PlantType, BaseConfig> _removedConfigs = new Dictionary<PlantType, BaseConfig>();

		// Token: 0x0400001B RID: 27
		private static HashSet<PlantType> _gamePlants;

		// Token: 0x0400001C RID: 28
		private static bool _gamePlantsLoaded;
	}
	public static class AlmanacDirectoryInjector
	{
		// Token: 0x0600002D RID: 45 RVA: 0x000032E0 File Offset: 0x000014E0
		public static void ResetInjected()
		{
            _injected.Clear();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000032F0 File Offset: 0x000014F0
		public static void Inject(ShootingAlmanacDirectory source)
		{
			bool flag = source == null;
			if (!flag)
			{
				IntPtr pointer;
				try
				{
					pointer = source.Pointer;
				}
				catch
				{
					return;
				}
				bool flag2 = pointer == IntPtr.Zero;
				if (!flag2)
				{
					bool flag3 = !_injected.Add(pointer);
					if (!flag3)
					{
						try
						{
							bool flag4 = !true;
							if (!flag4)
							{
                                InjectPlants(source);
                                InjectRoutes(source);
                                InjectRoots(source);
                                InjectExpertEntries(source);
							}
						}
						catch (Exception t)
						{
							ModLogger.LogError(t.ToString());
						}
					}
				}
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000033D8 File Offset: 0x000015D8
		private static void InjectPlants(ShootingAlmanacDirectory source)
		{
			var plants = source.plants;
			bool flag = plants == null;
			if (!flag)
			{
				foreach (KeyValuePair<PlantType, CustomRogueShootingConfig> keyValuePair in RegistryHelper.RogueConfigs)
				{
					bool flag2 = plants.ContainsKey(keyValuePair.Key);
					if (!flag2)
					{
						Il2CppSystem.Collections.Generic.List<ShootingAlmanacBuff> list = BuildPlantBuffs(keyValuePair.Key);
						bool flag3 = list.Count > 0;
						ShootingAlmanacPlant value = new ShootingAlmanacPlant(keyValuePair.Key, Lawnf.GetName(keyValuePair.Key), keyValuePair.Value.CustomRole, flag3, list);
						plants.Add(keyValuePair.Key, value);
					}
				}
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000034AC File Offset: 0x000016AC
		private static Il2CppSystem.Collections.Generic.List<ShootingAlmanacBuff> BuildPlantBuffs(PlantType type)
		{
			Il2CppSystem.Collections.Generic.List<ShootingAlmanacBuff> list = new Il2CppSystem.Collections.Generic.List<ShootingAlmanacBuff>();
			bool flag = RegistryHelper.RogueBuffs.TryGetValue(type, out var list2);
			if (flag)
			{
				foreach (var customizeShootingBuffData in list2)
				{
                    if(customizeShootingBuffData.CustomTitle == "强化：力量")
                    {
				        list.Add(BuildGenericDamageBuff(type));
                        continue;
                    }
                    else if(customizeShootingBuffData.CustomTitle == "强化：速度")
                    {
				        list.Add(BuildGenericSpeedBuff(type));
                    }
                    else if(customizeShootingBuffData.CustomTitle == "超进化：星辉")
                    {
				        list.Add(BuildGenericStarUpBuff(type));
                    }
					bool flag2 = string.IsNullOrEmpty(customizeShootingBuffData.CustomTitle);
					if (!flag2)
					{
						PlantType plantType = type;
						try
						{
							plantType = customizeShootingBuffData.CustomPlantType;
						}
						catch
						{
						}
						int num = 1;
						try
						{
                            switch (customizeShootingBuffData.CustomBuffType)
                            {
                                case ShootingBuffType.UniqueUpgrade: num = 10; break;
                                case ShootingBuffType.QualitativeChange:
                                case ShootingBuffType.CurseBuff: 
                                default:
                                case ShootingBuffType.SuperUpgrade: num = 1; break;
                                case ShootingBuffType.General: num = 250; break;
                            }
						}
						catch
						{
						}
						Quality quality = 0;
						try
						{
                            switch (customizeShootingBuffData.CustomBuffType)
                            {
                                case ShootingBuffType.UniqueUpgrade: quality = Quality.gold; break;
                                case ShootingBuffType.QualitativeChange: quality = Quality.diamond; break;
                                case ShootingBuffType.SuperUpgrade: quality = Quality.iridescent; break;
                                default:
                                case ShootingBuffType.General: quality = Quality.Default; break;
                                case ShootingBuffType.CurseBuff: quality = Quality.curse; break;
                            }
						}
						catch
						{
						}
						string text = string.Empty;
						try
						{
							text = customizeShootingBuffData.CustomDescription;
						}
						catch
						{
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
						defaultInterpolatedStringHandler.AppendFormatted<PlantType>(type);
						defaultInterpolatedStringHandler.AppendLiteral(":");
						defaultInterpolatedStringHandler.AppendFormatted(customizeShootingBuffData.CustomTitle);
						ShootingAlmanacBuff item = new ShootingAlmanacBuff(defaultInterpolatedStringHandler.ToStringAndClear(), plantType, customizeShootingBuffData.CustomTitle, text, quality, num, string.Empty, false);
						list.Add(item);
					}
				}
			}
			return list;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000036F0 File Offset: 0x000018F0
		private static ShootingAlmanacBuff BuildGenericDamageBuff(PlantType owner)
		{
			string name = Lawnf.GetName(owner);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
			defaultInterpolatedStringHandler.AppendFormatted<PlantType>(owner);
			defaultInterpolatedStringHandler.AppendLiteral(":DamageBuff");
			return new ShootingAlmanacBuff(defaultInterpolatedStringHandler.ToStringAndClear(), owner, "强化：力量", name + "获得30%独立伤害增幅（普通品质为准，实际增幅随品质变化）。", 0, 1000, string.Empty, false);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003754 File Offset: 0x00001954
		private static ShootingAlmanacBuff BuildGenericStarUpBuff(PlantType owner)
		{
			string name = Lawnf.GetName(owner);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendFormatted<PlantType>(owner);
			defaultInterpolatedStringHandler.AppendLiteral(":SpeedBuff");
			return new ShootingAlmanacBuff(defaultInterpolatedStringHandler.ToStringAndClear(), owner, "超进化：星辉", name + "升级成星辉" + name, 0, 50, string.Empty, false);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003754 File Offset: 0x00001954
		private static ShootingAlmanacBuff BuildGenericSpeedBuff(PlantType owner)
		{
			string name = Lawnf.GetName(owner);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendFormatted<PlantType>(owner);
			defaultInterpolatedStringHandler.AppendLiteral(":StarUpBuff");
			return new ShootingAlmanacBuff(defaultInterpolatedStringHandler.ToStringAndClear(), owner, "强化：速度", name + "获得20%速度增幅（普通品质为准，实际增幅随品质变化）。", 0, 50, string.Empty, false);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000037B4 File Offset: 0x000019B4
		private static void InjectRoutes(ShootingAlmanacDirectory source)
		{
			bool flag = !true; //bruh, why is dnspy showing me this
			if (!flag)
			{
				Il2CppSystem.Collections.Generic.Dictionary<PlantType, Il2CppSystem.Collections.Generic.IReadOnlyList<Il2CppSystem.Collections.Generic.List<PlantType>>> routes = source.routes;
				bool flag2 = routes == null;
				if (!flag2)
				{
					foreach (KeyValuePair<PlantType, Il2CppSystem.Collections.Generic.List<PlantType>> keyValuePair in RegistryHelper.evolutionPathways)
					{
						PlantType key = keyValuePair.Key;
						Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> list = BuildRoutes(key);
						ModLogger.LogInfo($"[EVOLUTION] BuildRoutes({key}) produced {routes.Count} routes");
						bool flag3 = list == null || list.Count == 0;
						if (!flag3)
						{
							bool flag4 = !routes.ContainsKey(key);
							if (flag4)
							{
								var readOnlyList = list.TryCast<Il2CppSystem.Collections.Generic.IReadOnlyList<Il2CppSystem.Collections.Generic.List<PlantType>>>();
								bool flag5 = readOnlyList != null;
								if (flag5)
								{
									routes.Add(key, readOnlyList);
								}
								else
								{
									ModLogger.LogWarn("Invalid data detected!");
								}
							}
							else
							{
								var readOnlyList2 = routes[key];
								Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> list2 = new();
                                Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> list3;
                                try
                                {
                                    list3 = readOnlyList2.TryCast<Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>>>();
                                }
                                catch
                                {
                                    list3=null;
                                }
								
								bool flag7 = list3 != null;
								if (flag7)
								{
									for (int i = 0; i < list3.Count; i++)
									{

										list2.Add(list3[i]);
									}
								}
								else
								{
									var readOnlyCollection = readOnlyList2.TryCast<Il2CppSystem.Collections.Generic.IReadOnlyCollection<Il2CppSystem.Collections.Generic.List<PlantType>>>();
									bool flag8 = readOnlyCollection != null;
									if (!flag8)
									{
										ModLogger.LogWarn("Invalid data detected!");
										continue;
									}
									for (int j = 0; j < readOnlyCollection.Count; j++)
									{
										list2.Add(readOnlyList2[j]);
									}
								}
								for (int k = 0; k < list.Count; k++)
								{
									bool flag9 = !ContainsRoute(list2, list[k]);
									if (flag9)
									{
										list2.Add(list[k]);
									}
								}
								var readOnlyList3 = list2.TryCast<Il2CppSystem.Collections.Generic.IReadOnlyList<Il2CppSystem.Collections.Generic.List<PlantType>>>();
								bool flag10 = readOnlyList3 != null;
								if (flag10)
								{
									routes[key] = readOnlyList3;
								}
								else
								{
                                    ModLogger.LogWarn($"TryCast merged routes failed: {key}");
								}
							}
						}
					}
                    ExtendExistingRoutes(source);
				}
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003A7C File Offset: 0x00001C7C
		private static void ExtendExistingRoutes(ShootingAlmanacDirectory source)
		{
            try{
			Il2CppSystem.Collections.Generic.Dictionary<PlantType, Il2CppSystem.Collections.Generic.IReadOnlyList<Il2CppSystem.Collections.Generic.List<PlantType>>> routes = source.routes;
			bool flag = routes == null;
			if (!flag)
			{
				var roots = source.Roots;
				bool flag2 = roots == null;
				if (!flag2)
				{
					int num = 0;
                    Il2CppSystem.Collections.Generic.List<PlantType> list;
                    try{
					list = roots.TryCast<Il2CppSystem.Collections.Generic.List<PlantType>>();
                    }
                        catch
                        {
                            list = null;
                        }
					bool flag3 = list != null;
					if (flag3)
					{
						num = list.Count;
					}
					else
					{
						var readOnlyCollection = roots.TryCast<Il2CppSystem.Collections.Generic.IReadOnlyCollection<PlantType>>();
						bool flag4 = readOnlyCollection == null;
						if (flag4)
						{
							return;
						}
						num = readOnlyCollection.Count;
					}
					for (int i = 0; i < num; i++)
					{
						PlantType key = roots[i];
						bool flag5 = !routes.ContainsKey(key);
						if (!flag5)
						{
							var readOnlyList = routes[key];
							Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> list2 = new();
                            
							Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> list3 = null;
                            try
                                {
                                    list3  = readOnlyList.TryCast<Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>>>();
                                }
                                catch
                                {
                                    
                                }
							bool flag6 = list3 != null;
							if (flag6)
							{
								for (int j = 0; j < list3.Count; j++)
								{
									list2.Add(list3[j]);
								}
							}
							else
							{
								var readOnlyCollection2 = readOnlyList.TryCast<Il2CppSystem.Collections.Generic.IReadOnlyCollection<Il2CppSystem.Collections.Generic.List<PlantType>>>();
								bool flag7 = readOnlyCollection2 == null;
								if (flag7)
								{
									goto IL_376;
								}
								for (int k = 0; k < readOnlyCollection2.Count; k++)
								{
									list2.Add(readOnlyList[k]);
								}
							}
							Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> list4 = new();
							for (int l = 0; l < list2.Count; l++)
							{
								Il2CppSystem.Collections.Generic.List<PlantType> list5 = list2[l];
								bool flag8 = list5.Count == 0;
								if (!flag8)
								{
									PlantType key2 = list5[list5.Count - 1];
									Il2CppSystem.Collections.Generic.List<PlantType> list6;
									bool flag9 = RegistryHelper.evolutionPathways.TryGetValue(key2, out list6) && list6.Count > 0;
									if (flag9)
									{
										foreach (PlantType item in list6)
										{
											Il2CppSystem.Collections.Generic.List<PlantType> list7 = new();
											for (int m = 0; m < list5.Count; m++)
											{
												list7.Add(list5[m]);
											}
											list7.Add(item);
											bool flag10 = !ContainsRoute(list4, list7);
											if (flag10)
											{
												list4.Add(list7);
											}
										}
									}
									else
									{
										bool flag11 = !ContainsRoute(list4, list5);
										if (flag11)
										{
											list4.Add(list5);
										}
										int num2 = 0;
										while (num2 + 1 < list5.Count)
										{
											PlantType plantType = list5[num2];
											Il2CppSystem.Collections.Generic.List<PlantType> list8;
											bool flag12 = !RegistryHelper.evolutionPathways.TryGetValue(plantType, out list8) || list8.Count == 0;
											if (!flag12)
											{
												Il2CppSystem.Collections.Generic.List<PlantType> list9 = new Il2CppSystem.Collections.Generic.List<PlantType>();
												for (int n = 0; n <= num2; n++)
												{
													list9.Add(list5[n]);
												}
												Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> list10 = new();
                                                ExtendPath(plantType, list9, list10);
												for (int num3 = 0; num3 < list10.Count; num3++)
												{
													Il2CppSystem.Collections.Generic.List<PlantType> list11 = list10[num3];
													bool flag13 = ContainsRouteSequence(list2, list11);
													if (!flag13)
													{
														bool flag14 = !ContainsRoute(list4, list11);
														if (flag14)
														{
															list4.Add(list11);
														}
													}
												}
											}
											num2++;
										}
									}
								}
							}
							var readOnlyList2 = list4.TryCast<Il2CppSystem.Collections.Generic.IReadOnlyList<Il2CppSystem.Collections.Generic.List<PlantType>>>();
							bool flag15 = readOnlyList2 != null;
							if (flag15)
							{
								routes[key] = readOnlyList2;
							}
						}
						IL_376:;
					}
				}
			}
            }
            catch(Exception e)
            {
                ModLogger.LogError(e.ToString());
            }
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003E24 File Offset: 0x00002024
		private static bool ContainsRouteSequence(Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> routes, Il2CppSystem.Collections.Generic.List<PlantType> target)
		{
			bool flag = target == null || target.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < routes.Count; i++)
				{
					Il2CppSystem.Collections.Generic.List<PlantType> list = routes[i];
					bool flag2 = list == null || list.Count < target.Count;
					if (!flag2)
					{
						int num = 0;
						while (num + target.Count <= list.Count)
						{
							bool flag3 = true;
							for (int j = 0; j < target.Count; j++)
							{
								bool flag4 = !Equals(list[num + j], target[j]);
								if (flag4)
								{
									flag3 = false;
									break;
								}
							}
							bool flag5 = flag3;
							if (flag5)
							{
								return true;
							}
							num++;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003F1C File Offset: 0x0000211C
		private static void ExtendPath(PlantType node, Il2CppSystem.Collections.Generic.List<PlantType> path, Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> branches)
		{
			bool flag = !RegistryHelper.evolutionPathways.TryGetValue(node, out var list) || list.Count == 0;
			if (!flag)
			{
				foreach (PlantType plantType in list)
				{
					Il2CppSystem.Collections.Generic.List<PlantType> list2 = new();
					for (int i = 0; i < path.Count; i++)
					{
						list2.Add(path[i]);
					}
					list2.Add(plantType);
					bool flag2 = !ContainsRoute(branches, list2);
					if (flag2)
					{
						branches.Add(list2);
					}
                    ExtendPath(plantType, list2, branches);
				}
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003FEC File Offset: 0x000021EC
		private static bool ContainsRoute(Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> routes, Il2CppSystem.Collections.Generic.List<PlantType> target)
		{
			for (int i = 0; i < routes.Count; i++)
			{
				Il2CppSystem.Collections.Generic.List<PlantType> list = routes[i];
				bool flag = list.Count != target.Count;
				if (!flag)
				{
					bool flag2 = true;
					for (int j = 0; j < list.Count; j++)
					{
						if (list[j] != target[j])
						{
							flag2 = false;
							break;
						}
					}
					bool flag4 = flag2;
					if (flag4)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00004094 File Offset: 0x00002294
		private static Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> BuildRoutes(PlantType start)
		{
			Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> result = new();
			HashSet<PlantType> ancestors = new HashSet<PlantType>();
			Il2CppSystem.Collections.Generic.List<PlantType> path = new();
            Dfs(start, ancestors, path, result);
			return result;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000040C4 File Offset: 0x000022C4
		private static void Dfs(PlantType node, HashSet<PlantType> ancestors, Il2CppSystem.Collections.Generic.List<PlantType> path, Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<PlantType>> result)
		{
			bool flag = !ancestors.Add(node);
			if (!flag)
			{
				path.Add(node);
				bool flag2 = RegistryHelper.evolutionPathways.TryGetValue(node, out var list) && list.Count > 0;
				if (flag2)
				{
					foreach (PlantType node2 in list)
					{
						Dfs(node2, ancestors, path, result); // recursive search algorithm
					}
					ModLogger.LogInfo($"[DFS] node={node}, children={string.Join(" , ", list.ToSystemList())}");
				}
				else
				{
					Il2CppSystem.Collections.Generic.List<PlantType> list2 = new();
					foreach( var i in path )
					{
						list2.Add(i);
					}
					result.Add(list2);
					ModLogger.LogInfo($"Completed route: {string.Join(" -> ", path.ToSystemList())}");
				}
				path.RemoveAt(path.Count - 1);
				ancestors.Remove(node);
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000041C8 File Offset: 0x000023C8
		private static void InjectRoots(ShootingAlmanacDirectory source)
		{
            try{
			bool flag = RegistryHelper.CustomBasePlants.Count == 0;
			if (!flag)
			{
				var roots_k__BackingField = source._Roots_k__BackingField;
				Il2CppSystem.Collections.Generic.List<PlantType> list = new();
				bool flag2 = roots_k__BackingField != null;
				if (flag2)
				{
                    Il2CppSystem.Collections.Generic.List<PlantType> list2;
                        try
                        {
					list2 = roots_k__BackingField.TryCast<Il2CppSystem.Collections.Generic.List<PlantType>>();
                        }
                        catch
                        {
                            list2 =  null;
                        }
					bool flag3 = list2 != null;
					if (flag3)
					{
						for (int i = 0; i < list2.Count; i++)
						{
							list.Add(list2[i]);
						}
					}
					else
					{
						var readOnlyCollection = roots_k__BackingField.TryCast<Il2CppSystem.Collections.Generic.IReadOnlyCollection<PlantType>>();
						bool flag4 = readOnlyCollection != null;
						if (!flag4)
						{
							ModLogger.LogWarn("Unable to read original base plant list! Aborting injection.");
							return;
						}
						for (int j = 0; j < readOnlyCollection.Count; j++)
						{
							list.Add(roots_k__BackingField[j]);
						}
					}
				}
				foreach (PlantType item in RegistryHelper.CustomBasePlants)
				{
					bool flag5 = !list.Contains(item);
					if (flag5)
					{
						list.Add(item);
					}
				}
				var readOnlyList = list.TryCast<Il2CppSystem.Collections.Generic.IReadOnlyList<PlantType>>();
				bool flag6 = readOnlyList != null;
				if (flag6)
				{
					source._Roots_k__BackingField = readOnlyList;
				}
				else
				{
					ModLogger.LogWarn("Invalid data detected!");
				}
			}
            }
            catch(Exception e)
            {
                ModLogger.LogError(e.ToString());
            }
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00004338 File Offset: 0x00002538
		private static void InjectExpertEntries(ShootingAlmanacDirectory source)
		{
            try
            {
			bool flag = RegistryHelper.experts.Count == 0;
			if (!flag)
			{
				Il2CppSystem.Collections.Generic.Dictionary<ShootingAlmanacCategory, Il2CppSystem.Collections.Generic.IReadOnlyList<ShootingAlmanacEntry>> categories = source.categories;
				bool flag2 = categories == null;
				if (!flag2)
				{
					bool flag3 = false;
					try
					{
						flag3 = categories.ContainsKey((ShootingAlmanacCategory)1);
					}
					catch
					{
					}
					Il2CppSystem.Collections.Generic.List<ShootingAlmanacEntry> list = new();
					bool flag4 = flag3;
					if (flag4)
					{
						try
						{
							var readOnlyList = categories[(ShootingAlmanacCategory)1];
							bool flag5 = readOnlyList != null;
							if (flag5)
							{
                                Il2CppSystem.Collections.Generic.List<ShootingAlmanacEntry> list2;
                                try{
								list2 = readOnlyList.TryCast<Il2CppSystem.Collections.Generic.List<ShootingAlmanacEntry>>();
                                }
                                    catch
                                    {
                                        list2 = null;
                                    }
								bool flag6 = list2 != null;
								if (flag6)
								{
									for (int i = 0; i < list2.Count; i++)
									{
										list.Add(list2[i]);
									}
								}
								else
								{
									var readOnlyCollection = readOnlyList.TryCast<Il2CppSystem.Collections.Generic.IReadOnlyCollection<ShootingAlmanacEntry>>();
									bool flag7 = readOnlyCollection != null;
									if (flag7)
									{
										for (int j = 0; j < readOnlyCollection.Count; j++)
										{
											list.Add(readOnlyList[j]);
										}
									}
								}
							}
						}
						catch
						{
						}
					}
					HashSet<string> hashSet = new HashSet<string>();
					for (int k = 0; k < list.Count; k++)
					{
						ShootingAlmanacEntry shootingAlmanacEntry = list[k];
						bool flag8 = shootingAlmanacEntry != null && !string.IsNullOrEmpty(shootingAlmanacEntry.Id);
						if (flag8)
						{
							hashSet.Add(shootingAlmanacEntry.Id);
						}
					}
					foreach (PlantType plantType in RegistryHelper.experts)
					{
						string text = $"custom_expert_{plantType}";
						bool flag9 = hashSet.Contains(text);
						if (!flag9)
						{
							string name = Lawnf.GetName(plantType);
							string text2 = RegistryHelper.plantRoles.ContainsKey(plantType) ? RegistryHelper.plantRoles[plantType] : "自定义";
							ShootingAlmanacEntry item = new ShootingAlmanacEntry(text, ShootingAlmanacCategory.Expert, name, text2, string.Empty, 0, plantType, true, "自定义", 0);
							list.Add(item);
						}
					}
					var readOnlyList2 = list.TryCast<Il2CppSystem.Collections.Generic.IReadOnlyList<ShootingAlmanacEntry>>();
					bool flag10 = readOnlyList2 != null;
					if (flag10)
					{
						categories[ShootingAlmanacCategory.Expert] = readOnlyList2;
					}
					else
					{
						ModLogger.LogError("Invalid data found!");
					}
				}
			}
            }
            catch(Exception e)
            {
                ModLogger.LogError(e.ToString());
            }
		}

		// Token: 0x04000019 RID: 25
		private static readonly HashSet<IntPtr> _injected = new HashSet<IntPtr>();
	}
}