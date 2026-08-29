using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using BepInEx;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Core;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Modified.Command;
using Modified.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace Modified
{
	// Token: 0x02000010 RID: 16
	[BepInPlugin("Modified-Plus", "Modified-Plus", "2.2.1")]
	public class Plugin : BasePlugin
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00003A7C File Offset: 0x00001C7C
		
		public override void Load()
		{
			Console.OutputEncoding = Encoding.UTF8;
			Console.InputEncoding = Encoding.UTF8;
            Field0 = base.Log;
			ManualLogSource field = Field0;
			bool flag = true;
			BepInExInfoLogInterpolatedStringHandler bepInExInfoLogInterpolatedStringHandler = new BepInExInfoLogInterpolatedStringHandler(14, 1, out flag);
			if (flag)
			{
				bepInExInfoLogInterpolatedStringHandler.AppendLiteral("Plugin ");
				bepInExInfoLogInterpolatedStringHandler.AppendFormatted<string>("Modified-Plus");
				bepInExInfoLogInterpolatedStringHandler.AppendLiteral(" is loaded!");
			}
			field.LogInfo(bepInExInfoLogInterpolatedStringHandler);
            StaticMethod7("Modified-Plus").PatchAll();
			this.inputThread = StaticMethod9(new ThreadStart(this.Method0));
			this.inputThread.Start();
			if (!isServer)
			{
                StartClient("127.0.0.1", 25502);
			}
			else
			{
                StartServer(25502);
			}
            timer = delay;
			GameAPP.developerMode = isDevMode;
			//float num = 1f;
			Action<string, LogType> action2 = new Action<string, LogType>(this.LogHandler);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003B9C File Offset: 0x00001D9C
		private void LogHandler(string message, LogType type)
		{
			ManualLogSource field = Field0;
			bool flag;
			BepInExInfoLogInterpolatedStringHandler bepInExInfoLogInterpolatedStringHandler = new BepInExInfoLogInterpolatedStringHandler(14, 1, out flag);
			if (flag)
			{
				bepInExInfoLogInterpolatedStringHandler.AppendLiteral("[UniverseLib] ");
				bepInExInfoLogInterpolatedStringHandler.AppendFormatted<string>(message);
			}
			field.LogInfo(bepInExInfoLogInterpolatedStringHandler);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002211 File Offset: 0x00000411
		private void MainPanelUpdate()
		{
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003BD4 File Offset: 0x00001DD4
		public override bool Unload()
		{
			this.keepReading = false;
			if (this.inputThread != null && this.inputThread.IsAlive)
			{
				this.inputThread.Join();
			}
            StopCommunication();
			return base.Unload();
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003C14 File Offset: 0x00001E14
		public static void StartServer(int port)
		{
			Thread thread = new Thread(() =>
			{
				// Create listener
				TcpListener listener = new TcpListener(IPAddress.Any, port);
				listener.Start();

				Plugin.Field0.LogInfo($"Server: {port}");

				try
				{
					// Accept client
					client = listener.AcceptTcpClient();
					Plugin.Field0.LogInfo("User has joined");

					// Get stream
					stream = client.GetStream();

					// Begin reading
					StartListening();
				}
				catch (Exception ex)
				{
					Plugin.Field0.LogError($"Error accepting client: {ex.Message}");
				}
			});

			thread.IsBackground = true;
			thread.Start();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003C4C File Offset: 0x00001E4C
		
		public static void StartClient(string serverIp, int port)
		{
			try
			{
                client = StaticMethod16(serverIp, port);
                stream = client.GetStream();
                StartListening();
				CommandMethods.SendTcpMsg("gamestatus|1");
			}
			catch
			{
                Field0.LogError("请使用启动器启动游戏！！！但可以继续使用快捷键");
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003CC8 File Offset: 0x00001EC8
		
		public static void StartListening()
		{
			Field0.LogInfo("Start Listening");
			isListening = true;

			listenThread = new Thread(Plugin.ListenForMessages);
			listenThread.IsBackground = true;
			listenThread.Start();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003D24 File Offset: 0x00001F24
		
		public static void ListenForMessages()
		{
			byte[] array = new byte[1024];
			try
			{
				while (isListening)
				{
					int num = stream.Read(array, 0, array.Length);
					if (num > 0)
					{
						string @string = Encoding.UTF8.GetString(array, 0, num);
                        commandQueue.Enqueue(@string);
					}
				}
			}
			catch (Exception ex)
			{
				ManualLogSource field = Field0;
				bool flag;
				BepInExInfoLogInterpolatedStringHandler bepInExInfoLogInterpolatedStringHandler = new BepInExInfoLogInterpolatedStringHandler(14, 1, out flag);
				if (flag)
				{
					bepInExInfoLogInterpolatedStringHandler.AppendLiteral("Error: ");
					bepInExInfoLogInterpolatedStringHandler.AppendFormatted<string>(ex.Message);
				}
				field.LogInfo(bepInExInfoLogInterpolatedStringHandler);
                StopCommunication();
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003DC4 File Offset: 0x00001FC4
		
		public static void StopCommunication()
		{
            isListening = false;
			if (listenThread != null && listenThread.IsAlive)
			{
                listenThread.Join();
			}
			NetworkStream networkStream = stream;
			if (networkStream != null)
			{
				networkStream.Close();
			}
			TcpClient tcpClient = client;
			if (tcpClient != null)
			{
				tcpClient.Close();
			}
            Field0.LogInfo("Connection closed");
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003E24 File Offset: 0x00002024
		
		public static void SendMessage(string message)
		{
			if (stream != null && stream.CanWrite)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(message);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
				return;
			}
			if (isDevMode)
			{
				ManualLogSource field = Field0;
				bool flag;
				BepInExInfoLogInterpolatedStringHandler bepInExInfoLogInterpolatedStringHandler = new BepInExInfoLogInterpolatedStringHandler(14, 1, out flag);
				if (flag)
				{
					bepInExInfoLogInterpolatedStringHandler.AppendLiteral("Error Send: ");
					bepInExInfoLogInterpolatedStringHandler.AppendFormatted<string>(message);
				}
				field.LogInfo(bepInExInfoLogInterpolatedStringHandler);
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003E9C File Offset: 0x0000209C
		
		private void Method0()
		{
			while (this.keepReading)
			{
				string t = Console.ReadLine();
				if (!(t.ToLower() == "quit"))
				{
                    commandQueue.Enqueue(t);
				}
				else
				{
					this.keepReading = false;
				}
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000040B0 File Offset: 0x000022B0
		static Harmony StaticMethod7(string A_0)
		{
			return new Harmony(A_0);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000040C4 File Offset: 0x000022C4
		static Thread StaticMethod9(ThreadStart A_0)
		{
			return new Thread(A_0);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000040D8 File Offset: 0x000022D8
		static TcpClient StaticMethod16(string A_0, int A_1)
		{
			return new TcpClient(A_0, A_1);
		}

		// Token: 0x04000030 RID: 48
		internal static ManualLogSource Field0;

		// Token: 0x04000031 RID: 49
		public static bool isServer = false;

		// Token: 0x04000032 RID: 50
		public static bool isDevMode = false;

		// Token: 0x04000033 RID: 51
		public static TcpClient client;

		// Token: 0x04000034 RID: 52
		public static NetworkStream stream;

		// Token: 0x04000035 RID: 53
		public static Thread listenThread;

		// Token: 0x04000036 RID: 54
		public static bool isListening;

		// Token: 0x04000038 RID: 56
		public static AssetBundle ab;

		// Token: 0x04000039 RID: 57
		public static GameObject ModfiedPlantToast;

		// Token: 0x0400003A RID: 58
		public static ConcurrentQueue<string> commandQueue = new();

		// Token: 0x0400003B RID: 59
		public Thread inputThread;

		// Token: 0x0400003C RID: 60
		public bool keepReading = true;

		// Token: 0x0400003D RID: 61
		public static bool anyPlant = false;

		// Token: 0x0400003E RID: 62
		public static bool isTimeStop = false;

		// Token: 0x0400003F RID: 63
		public static bool rowPlant = false;

		// Token: 0x04000040 RID: 64
		public static bool lockSun = false;

		// Token: 0x04000041 RID: 65
		public static bool daveInvincible = false;

		// Token: 0x04000042 RID: 66
		public static bool isRandomBullet = false;

		// Token: 0x04000043 RID: 67
		public static int appointBullet = -1;

		// Token: 0x04000044 RID: 68
		public static int bulletAtk = -1;

		// Token: 0x04000045 RID: 69
		public static bool isGloveNoCD = false;

		// Token: 0x04000046 RID: 70
		public static bool isCardNoCD = false;

		// Token: 0x04000047 RID: 71
		public static bool isHammerNoCD = false;

		// Token: 0x04000048 RID: 72
		public static bool isScaredyDream = false;

		// Token: 0x04000049 RID: 73
		public static bool isSeedRain = false;

		// Token: 0x0400004A RID: 74
		public static bool isPlantInvincible = false;

		// Token: 0x0400004B RID: 75
		public static bool isPlantSeckill = false;

		// Token: 0x0400004C RID: 76
		public static bool isPlantLimHp = false;

		// Token: 0x0400004D RID: 77
		public static bool isPlantAntiRolling = false;

		// Token: 0x0400004E RID: 78
		public static bool isPlantAbnormalShoot = false;

		// Token: 0x0400004F RID: 79
		public static bool isManyPoints = false;

		// Token: 0x04000050 RID: 80
		public static bool isZombieSeckill = false;

		// Token: 0x04000051 RID: 81
		public static bool isZombieInvincible = false;

		// Token: 0x04000052 RID: 82
		public static bool isZombieSea = false;

		// Token: 0x04000053 RID: 83
		public static bool stopZombieStart = false;

		// Token: 0x04000054 RID: 84
		public static bool isMomentZombieDie = false;

		// Token: 0x04000055 RID: 85
		public static bool isTravelPlant = false;

		// Token: 0x04000056 RID: 86
		public static bool isTravelRefresh = false;

		// Token: 0x04000057 RID: 87
		public static bool isReversalPlant = false;

		// Token: 0x04000058 RID: 88
		public static bool isRich = false;

		// Token: 0x04000059 RID: 89
		public static bool isAUnlock = false;

		// Token: 0x0400005A RID: 90
		public static float delay = 1f;

		// Token: 0x0400005B RID: 91
		public static float timer;

		// Token: 0x0400005C RID: 92
		public static int zombieSeaType = -1;

		// Token: 0x0400005D RID: 93
		public static int zombieHpMu = 1;

		// Token: 0x0400005E RID: 94
		public static bool superShooter = false;

		// Token: 0x0400005F RID: 95
		public static bool isAlmanacMgrPlantPut = true;

		// Token: 0x04000060 RID: 96
		public static int almanacMgrPlantId = -1;

		// Token: 0x04000061 RID: 97
		public static bool isAlmanacMgrZombiePut = true;

		// Token: 0x04000062 RID: 98
		public static int almanacMgrZombieId = -1;

		// Token: 0x04000063 RID: 99
		public static bool isThrowerType = false;

		// Token: 0x04000064 RID: 100
		public static bool isManyHitTimes = false;

		// Token: 0x04000065 RID: 101
		public static bool isCobCannonShoop = false;

		// Token: 0x04000066 RID: 102
		public static bool isAbyssMoney = false;

		// Token: 0x04000067 RID: 103
		public static bool isAbyssRefreshCount = false;

		// Token: 0x04000068 RID: 104
		public static bool isAbyssMaxPlantCount = false;

		// Token: 0x04000069 RID: 105
		public static bool isAbyssMaxBuffCount = false;

		// Token: 0x0400006A RID: 106
		public static bool isBannedInAbyss = false;

		// Token: 0x0400006B RID: 107
		public static bool isAbyssSwordStar = false;

		// Token: 0x0400006C RID: 108
		public static bool isSolarSunflower = false;

		// Token: 0x0400006D RID: 109
		public static bool isUltimateMinigun = false;

		// Token: 0x0400006E RID: 110
		public static bool isSameSpeed = false;

		// Token: 0x0400006F RID: 111
		public static bool isCDText = false;

		// Token: 0x04000070 RID: 112
		public static bool isACD = false;

		// Token: 0x04000071 RID: 113
		public static bool isTreasureLucky = false;

		// Token: 0x04000072 RID: 114
		public static int tr = 0;

		// Token: 0x04000073 RID: 115
		public static int to = 0;

		// Token: 0x04000074 RID: 116
		public static int tp = 0;

		// Token: 0x04000075 RID: 117
		public static int tb = 0;

		// Token: 0x04000076 RID: 118
		public static int tg = 0;

		// Token: 0x04000077 RID: 119
		public static int tw = 0;

		// Token: 0x04000078 RID: 120
		public static GardenSkinType bigGardenSkinType = GardenSkinType.None;

		// Token: 0x04000079 RID: 121
		public static int bigGardenSkinId = -1;

		// Token: 0x0400007A RID: 122
		public static List<PlantType> allPlantTypes;

		// Token: 0x02000011 RID: 17
		public enum GardenSkinType
		{
			// Token: 0x0400007C RID: 124
			None,
			// Token: 0x0400007D RID: 125
			Plant,
			// Token: 0x0400007E RID: 126
			Zombie
		}

		// Token: 0x02000012 RID: 18
		[HarmonyPatch(typeof(Board))]
		public class BoardUpdatePatch
		{
			// Token: 0x06000052 RID: 82 RVA: 0x000040EC File Offset: 0x000022EC
			[HarmonyPostfix]
			[HarmonyPatch("Update")]
			
			public static void PostUpdate(Board __instance)
			{
				if (isRich)
				{
					__instance.theMoney = 9999999;
					GameAPP.theMoneyCount = 999999L;
				}
				if (isManyPoints)
				{
					__instance.thePoints = 999999f;
				}
				if (isScaredyDream)
				{
					Board.BoardTag boardTag2 = Board.Instance.boardTag;
					boardTag2.isScaredyDream = isScaredyDream;
					Board.Instance.boardTag = boardTag2;
				}
			}
		}

		// Token: 0x02000013 RID: 19
		[HarmonyPatch(typeof(GameAPP))]
		public class GameAPPPatch
		{
			// Token: 0x06000055 RID: 85 RVA: 0x000042BC File Offset: 0x000024BC
			[HarmonyPostfix]
			[HarmonyPatch("Start")]
			
			public static void PostStart(GameAPP __instance)
			{
                allPlantTypes = new();
				foreach (PlantType item in GameAPP.resourcesManager.allPlants)
				{
                    allPlantTypes.Add(item);
				}
				CommandMethods.InitTravel();
				CommandMethods.SendTcpMsg("gamestatus|1");
			}

			// Token: 0x06000056 RID: 86 RVA: 0x00004318 File Offset: 0x00002518
			[HarmonyPostfix]
			[HarmonyPatch("Update")]
			public static void PostUpdate(GameAPP __instance)
			{
				string message;
				while (commandQueue.TryDequeue(out message))
				{
					CommandMethods.ProcessCommand(message);
				}
				if (lockSun && Board.Instance != null)
				{
					Board.Instance.theSun = 999999;
				}
                SingleKey();
			}

			// Token: 0x06000057 RID: 87 RVA: 0x000043A8 File Offset: 0x000025A8
			
			public static void SingleKey()
			{
				if (Input.GetKeyDown(KeyCode.PageUp))
				{
                    isTimeStop = !isTimeStop;
					if (isTimeStop)
					{
						Time.timeScale = 0f;
						return;
					}
					Time.timeScale = GameAPP.config.gameSpeed;
				}
			}

			// Token: 0x06000059 RID: 89 RVA: 0x00004944 File Offset: 0x00002B44
			
			public static void OpenSelect()
			{
				Transform transform = GameObject.Find("Canvas/InGameUI(Clone)").transform.Find("Bottom");
				transform.gameObject.SetActive(true);
				if ((int)transform.position.x <= 1)
				{
					transform.position = new Vector3(2f, 0f, -100f);
				}
				else
				{
					transform.position = new Vector3(0f, -8.75f, -100f);
				}
				GameObject.Find("Canvas/InGameUI(Clone)/Bottom/SeedLibrary/Start").SetActive(false);
			}
		}
	}
}
