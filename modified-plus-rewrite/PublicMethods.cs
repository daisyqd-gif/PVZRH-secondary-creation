using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using Core;
using Il2CppInterop.Runtime;
using Il2CppSystem.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
namespace Modified.Utils
{
	// Token: 0x0200001C RID: 28
	public static class PublicMethods
	{
		// Token: 0x06000093 RID: 147 RVA: 0x000057E4 File Offset: 0x000039E4
		
		public static void SetBoardPlant(int x, int y, int id, int num = 1)
		{
			if (id < 0 || num < 1)
			{
				PublicMethods.Log.LogMessage("无效的植物ID或种植数量");
				return;
			}
			if (x == -1)
			{
				for (int i = 0; i < Board.Instance.columnNum; i++)
				{
					PublicMethods.PlantInRow(i, y, id, num);
				}
				return;
			}
			if (y != -1)
			{
				PublicMethods.PlantInRow(x, y, id, num);
				return;
			}
			for (int j = 0; j < Board.Instance.rowNum; j++)
			{
				PublicMethods.PlantInRow(x, j, id, num);
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005858 File Offset: 0x00003A58
		public static void PlantInRow(int x, int y, int id, int num)
		{
			for (int i = 0; i < num; i++)
			{
				PublicMethods.StaticMethod5(CreatePlant.Instance, x, y, (PlantType)id, null, default(Vector2), false, true, null);
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000588C File Offset: 0x00003A8C
		public static void SetBoardZombie(int x, int y, int id, bool charm = false, int num = 1)
		{
			if (charm)
			{
				if (x == -1)
				{
					for (int i = 0; i < Board.Instance.columnNum; i++)
					{
						for (int j = 0; j < num; j++)
						{
							CreateZombie.Instance.SetZombieWithMindControl(y, (ZombieType)id, Mouse.Instance.GetBoxXFromColumn(i), charm);
						}
					}
					return;
				}
				if (y == -1)
				{
					for (int k = 0; k < Board.Instance.rowNum; k++)
					{
						for (int l = 0; l < num; l++)
						{
							CreateZombie.Instance.SetZombieWithMindControl(k, (ZombieType)id, Mouse.Instance.GetBoxXFromColumn(x), charm);
						}
					}
					return;
				}
				for (int m = 0; m < num; m++)
				{
					CreateZombie.Instance.SetZombieWithMindControl(y, (ZombieType)id, Mouse.Instance.GetBoxXFromColumn(x), charm);
				}
				return;
			}
			else
			{
				if (x == -1)
				{
					for (int n = 0; n < 11; n++)
					{
						for (int num2 = 0; num2 < num; num2++)
						{
							CreateZombie.Instance.SetZombie(y, (ZombieType)id, Mouse.Instance.GetBoxXFromColumn(n), false);
						}
					}
					return;
				}
				if (y == -1)
				{
					for (int num3 = 0; num3 < Board.Instance.rowNum; num3++)
					{
						for (int num4 = 0; num4 < num; num4++)
						{
							CreateZombie.Instance.SetZombie(num3, (ZombieType)id, Mouse.Instance.GetBoxXFromColumn(x), false);
						}
					}
					return;
				}
				for (int num5 = 0; num5 < num; num5++)
				{
					CreateZombie.Instance.SetZombie(y, (ZombieType)id, Mouse.Instance.GetBoxXFromColumn(x), false);
				}
				return;
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005A04 File Offset: 0x00003C04
		
		public static void SpawnItem(string resourcePath)
		{
			GameObject gameObject = Resources.Load<GameObject>(resourcePath);
			if (!(gameObject != null))
			{
				ManualLogSource log = PublicMethods.Log;
				bool flag;
				BepInExWarningLogInterpolatedStringHandler bepInExWarningLogInterpolatedStringHandler = PublicMethods.StaticMethod14(25, 1, out flag);
				if (flag)
				{
					bepInExWarningLogInterpolatedStringHandler.AppendLiteral("Failed to load resource: ");
					bepInExWarningLogInterpolatedStringHandler.AppendFormatted(resourcePath);
				}
				log.LogWarning(bepInExWarningLogInterpolatedStringHandler);
				return;
			}
			Object.Instantiate(gameObject, new Vector2(0f, 0f), Quaternion.identity, GameAPP.board.transform);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005A78 File Offset: 0x00003C78
		
		public static void SpawnItemWithPositon(string resourcePath, Vector2 vector)
		{
			GameObject gameObject = Resources.Load<GameObject>(resourcePath);
			if (gameObject != null)
			{
				Object.Instantiate(gameObject, new Vector2(0f, 0f), Quaternion.identity, GameAPP.board.transform);
				return;
			}
			ManualLogSource log = PublicMethods.Log;
			bool flag;
			BepInExWarningLogInterpolatedStringHandler bepInExWarningLogInterpolatedStringHandler = PublicMethods.StaticMethod14(25, 1, out flag);
			if (flag)
			{
				bepInExWarningLogInterpolatedStringHandler.AppendLiteral("Failed to load resource: ");
				bepInExWarningLogInterpolatedStringHandler.AppendFormatted(resourcePath);
			}
			log.LogWarning(bepInExWarningLogInterpolatedStringHandler);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005AEC File Offset: 0x00003CEC
		
		public static void SetRes(string resourcePath, float x, float y)
		{
			GameObject gameObject = Resources.Load<GameObject>(resourcePath);
			if (!(gameObject != null))
			{
				ManualLogSource log = PublicMethods.Log;
				bool flag;
				BepInExWarningLogInterpolatedStringHandler bepInExWarningLogInterpolatedStringHandler = PublicMethods.StaticMethod14(25, 1, out flag);
				if (flag)
				{
					bepInExWarningLogInterpolatedStringHandler.AppendLiteral("Failed to load resource: ");
					bepInExWarningLogInterpolatedStringHandler.AppendFormatted(resourcePath);
				}
				log.LogWarning(bepInExWarningLogInterpolatedStringHandler);
				return;
			}
			Object.Instantiate(gameObject, new Vector2(x, y), Quaternion.identity, GameAPP.board.transform);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005B58 File Offset: 0x00003D58
		public static Sprite LoadSpriteFromFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				byte[] array = File.ReadAllBytes(filePath);
				Texture2D texture2D = PublicMethods.StaticMethod19(2, 2, (TextureFormat)5, false);
				if (ImageConversion.LoadImage(texture2D, array))
				{
					return Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
				}
			}
			return null;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005BC0 File Offset: 0x00003DC0
		public static Texture2D LoadTextureFromFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				byte[] array = File.ReadAllBytes(filePath);
				Texture2D texture2D = PublicMethods.StaticMethod19(2, 2, (TextureFormat)5, false);
				ImageConversion.LoadImage(texture2D, array);
				return texture2D;
			}
			return null;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005BF4 File Offset: 0x00003DF4
		public static float GetBoxXFromColumn(int theColumn)
		{
			return -4.8f + 1.35f * (float)theColumn;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005C10 File Offset: 0x00003E10
		public static float GetBoxYFromRow(int theRow)
		{
			float result;
			if (Board.Instance.rowNum == 5)
			{
				result = 2.3f - 1.67f * (float)theRow;
			}
			else
			{
				result = 2.3f - 1.45f * (float)theRow;
			}
			return result;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005C4C File Offset: 0x00003E4C
		
		public static void ShowPop(string content, string btnContent)
		{
			GameObject gameObject = Resources.Load<GameObject>("UI/MainMenu/HelpMenu");
			GameObject gameObject2 = Object.Instantiate(gameObject, GameAPP.canvas.transform);
			gameObject2.name = gameObject.name;
			Transform transform = gameObject2.transform.Find("window");
			if (transform != null)
			{
				Transform transform2 = transform.Find("text");
				TextMeshProUGUI textMeshProUGUI = (transform2 != null) ? transform2.GetComponent<TextMeshProUGUI>() : null;
				Transform transform3 = transform.Find("text1");
				TextMeshProUGUI textMeshProUGUI2 = (transform3 != null) ? transform3.GetComponent<TextMeshProUGUI>() : null;
				if (textMeshProUGUI != null)
				{
					textMeshProUGUI.text = "";
				}
				if (textMeshProUGUI2 != null)
				{
					textMeshProUGUI2.text = content;
				}
			}
			Transform transform4 = gameObject2.transform.Find("Image");
			transform4.GetComponent<PauseMenu_Btn>().buttonNumber = 6;
			if (transform4 != null)
			{
				Transform transform5 = transform4.Find("text (1)");
				TextMeshProUGUI textMeshProUGUI3 = (transform5 != null) ? transform5.GetComponent<TextMeshProUGUI>() : null;
				Transform transform6 = transform4.Find("text");
				TextMeshProUGUI textMeshProUGUI4 = (transform6 != null) ? transform6.GetComponent<TextMeshProUGUI>() : null;
				if (textMeshProUGUI3 != null)
				{
					textMeshProUGUI3.text = "";
				}
				if (textMeshProUGUI4 != null)
				{
					textMeshProUGUI4.text = btnContent;
				}
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005D70 File Offset: 0x00003F70
		
		public static void AddCardAutoPage(SeedLibrary __instance, int plantId2)
		{
			PlantType plantId=(PlantType)plantId2;
			Transform transform = __instance.transform.Find("CardPagesContainer/NormalCards");
			Transform transform2 = transform.GetChild(transform.childCount - 1);
			if (transform2.childCount % 54 == 0 && transform2.childCount != 0)
			{
				transform2 = Object.Instantiate(transform2.gameObject, transform2.parent).transform;
				for (int i = 0; i < transform2.childCount; i++)
				{
					Object.Destroy(transform2.transform.GetChild(i).gameObject);
				}
			}
			GameObject gameObject = Object.Instantiate(__instance.transform.Find("CardPagesContainer/NormalCards/SampleGrid(Clone)").GetChild(12).gameObject);
			for (int j = 0; j < gameObject.transform.childCount; j++)
			{
				Transform child = gameObject.transform.GetChild(j);
				if (child.name != "PacketBg")
				{
					GameObject gameObject2 = child.gameObject;
					CardUI cardUI = (gameObject2 != null) ? gameObject2.GetComponent<CardUI>() : null;
					if (cardUI != null)
					{
						cardUI.fullCD = PlantDataManager.PlantData_Default[plantId].cd;
						cardUI.thePlantType = plantId;
						cardUI.theSeedCost = PlantDataManager.PlantData_Default[plantId].cost;
						if (j == 1)
						{
							cardUI.theSeedCost *= 2;
						}
						Mouse.Instance.ChangeCardSprite(plantId, cardUI);
					}
				}
				else
				{
					child.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = GameAPP.resourcesManager.plantPreviews[plantId].GetComponent<SpriteRenderer>().sprite;
					child.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlantDataManager.PlantData_Default[plantId].cost.ToString();
				}
			}
			gameObject.name = GameAPP.resourcesManager.plantPrefabs[plantId].name.Substring(0, GameAPP.resourcesManager.plantPrefabs[plantId].name.Length - 6);
			if (transform2 != null)
			{
				gameObject.transform.SetParent(transform2, false);
				if (transform2.name != "Page1")
				{
					transform2.gameObject.SetActive(false);
				}
			}
		}

    // Backing storage for MixData (2048 columns, unlimited rows)
    private static readonly int[,] MixGrid = new int[4096, 2048];

    // Backing storage for ZombieList
    private static readonly int[,] ZombieGrid = new int[4096, 2048];

    // -------------------------
    // GetMixData
    // -------------------------
    public static int GetMixData(int row, int col)
    {
        if (row < 0 || row >= MixGrid.GetLength(0)) return -1;
        if (col < 0 || col >= MixGrid.GetLength(1)) return -1;

        return MixGrid[row, col];
    }

    // -------------------------
    // SetMixData
    // -------------------------
    public static void SetMixData(int row, int col, int value)
    {
        if (row < 0 || row >= MixGrid.GetLength(0)) return;
        if (col < 0 || col >= MixGrid.GetLength(1)) return;

        MixGrid[row, col] = value;
    }

    // -------------------------
    // GetZombieList
    // -------------------------
    public static int GetZombieList(int row, int col)
    {
        if (row < 0 || row >= ZombieGrid.GetLength(0)) return -1;
        if (col < 0 || col >= ZombieGrid.GetLength(1)) return -1;

        return ZombieGrid[row, col];
    }

		// Token: 0x060000A3 RID: 163 RVA: 0x00006164 File Offset: 0x00004364
		
		public static void ExportSpritesPngFromPrefab(GameObject prefab)
		{
			foreach (SpriteRenderer spriteRenderer in prefab.GetComponentsInChildren<SpriteRenderer>())
			{
				string text = "export/" + prefab.name;
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				string path = Path.Combine(text, spriteRenderer.name + ".png");
				if (spriteRenderer.sprite != null && spriteRenderer.sprite.texture != null)
				{
					PublicMethods.SaveTexture2DFromRenderTexture(spriteRenderer.sprite.texture, path);
				}
				else
				{
					Debug.LogWarning("SpriteRenderer '" + spriteRenderer.name + "' does not have a readable texture.");
				}
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006220 File Offset: 0x00004420
		
		public static void ExportSpritesPngFromPreview()
		{
			foreach (KeyValuePair<PlantType, GameObject> keyValuePair in GameAPP.resourcesManager.plantPreviews)
			{
				foreach (SpriteRenderer spriteRenderer in keyValuePair.Value.GetComponentsInChildren<SpriteRenderer>())
				{
					string text = "previews";
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					string path = text;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
					defaultInterpolatedStringHandler.AppendFormatted((int)keyValuePair.Key);
					defaultInterpolatedStringHandler.AppendLiteral(".png");
					string path2 = Path.Combine(path, defaultInterpolatedStringHandler.ToStringAndClear());
					if (spriteRenderer.sprite != null && spriteRenderer.sprite.texture != null)
					{
						PublicMethods.SaveTexture2DFromRenderTexture(spriteRenderer.sprite.texture, path2);
					}
					else
					{
						Debug.LogWarning("SpriteRenderer '" + spriteRenderer.name + "' does not have a readable texture.");
					}
				}
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000631C File Offset: 0x0000451C
		
		public static void SaveTexture2DToPng(Texture texture, string path)
		{
			if (texture != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
				defaultInterpolatedStringHandler.AppendFormatted(texture.name);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(texture.width);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(texture.height);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(texture.wrapMode);
				Debug.Log(defaultInterpolatedStringHandler.ToStringAndClear());
				Texture2D texture2D = PublicMethods.StaticMethod19(texture.width, texture.height, (TextureFormat)4, false);
				Graphics.CopyTexture(texture, texture2D);
				byte[] bytes = ImageConversion.EncodeToPNG(texture2D);
				File.WriteAllBytes(path, bytes);
				Debug.Log("Saved texture to " + path);
				return;
			}
			Debug.LogWarning("Texture is null or not readable.");
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000063FC File Offset: 0x000045FC
		
		public static void SaveTexture2DFromRenderTexture(Texture texture, string path)
		{
			RenderTexture renderTexture = PublicMethods.StaticMethod73(texture.width, texture.height, 24);
			Graphics.Blit(texture, renderTexture);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			Texture2D texture2D = PublicMethods.StaticMethod19(texture.width, texture.height, (TextureFormat)4, false);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)texture.width, (float)texture.height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			byte[] bytes = ImageConversion.EncodeToPNG(texture2D);
			File.WriteAllBytes(path, bytes);
			Debug.Log("Saved texture to " + path);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x0000649C File Offset: 0x0000469C
		
		public static AssetBundle LoadAssetBundle(string base64String, string path = "")
		{
			AssetBundle assetBundle = null;
			if (!string.IsNullOrEmpty(base64String))
			{
				assetBundle = AssetBundle.LoadFromMemory(Convert.FromBase64String(base64String));
			}
			if (assetBundle == null && !string.IsNullOrEmpty(path))
			{
				assetBundle = AssetBundle.LoadFromFile(path);
			}
			if (assetBundle == null)
			{
				Debug.LogError("Failed to load AssetBundle.");
			}
			return assetBundle;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000064F8 File Offset: 0x000046F8
		
		public static Sprite LoadSpriteFromBase64(string base64String)
		{
			Sprite result;
			try
			{
				byte[] array = Convert.FromBase64String(base64String);
				Texture2D texture2D = PublicMethods.StaticMethod19(2, 2, (TextureFormat)5, false);
				if (!ImageConversion.LoadImage(texture2D, array))
				{
					goto IL_75;
				}
				result = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
			}
			catch (Exception ex)
			{
				Debug.LogError("Error loading sprite from Base64: " + ex.Message);
				goto IL_75;
			}
			return result;
			IL_75:
			return null;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x0000658C File Offset: 0x0000478C
		
		public static Sprite GetLogo()
		{
			return PublicMethods.LoadSpriteFromBase64("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");
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000065A4 File Offset: 0x000047A4
		
		public static Sprite GetQrCode()
		{
			return PublicMethods.LoadSpriteFromBase64("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[...string is too long...]");
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000057C4 File Offset: 0x000039C4
		static Plant StaticMethod5(CreatePlant A_0, int A_1, int A_2, PlantType A_3, Plant A_4, Vector2 A_5, bool A_6, bool A_7, Plant A_8)
		{
			return A_0.SetPlant(A_1, A_2, A_3, A_4, A_5, A_6, A_7, A_8);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000065D4 File Offset: 0x000047D4
		static BepInExWarningLogInterpolatedStringHandler StaticMethod14(int A_0, int A_1, out bool A_2)
		{
			return new BepInExWarningLogInterpolatedStringHandler(A_0, A_1, out A_2);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000065EC File Offset: 0x000047EC
		static Texture2D StaticMethod19(int A_0, int A_1, TextureFormat A_2, bool A_3)
		{
			return new Texture2D(A_0, A_1, A_2, A_3);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00006604 File Offset: 0x00004804
		static RenderTexture StaticMethod73(int A_0, int A_1, int A_2)
		{
			return new RenderTexture(A_0, A_1, A_2);
		}

		// Token: 0x0400008B RID: 139
		public static ManualLogSource Log = Plugin.Field0;
	}
}
