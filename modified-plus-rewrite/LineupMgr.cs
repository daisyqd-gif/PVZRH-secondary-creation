using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Modified.Utils
{
	// Token: 0x0200001B RID: 27
	public static class LineupManager
	{
		// Token: 0x06000086 RID: 134 RVA: 0x000053B0 File Offset: 0x000035B0
		public static string CompressString(string text)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			string result;
			using (MemoryStream memoryStream = LineupManager.StaticMethod2())
			{
				using (GZipStream gzipStream = LineupManager.StaticMethod3(memoryStream, CompressionMode.Compress, true))
				{
					gzipStream.Write(bytes, 0, bytes.Length);
				}
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00005424 File Offset: 0x00003624
		public static string DecompressString(string compressedText)
		{
			string @string;
			using (MemoryStream memoryStream = LineupManager.StaticMethod9(Convert.FromBase64String(compressedText)))
			{
				using (GZipStream gzipStream = LineupManager.StaticMethod10(memoryStream, CompressionMode.Decompress))
				{
					using (MemoryStream memoryStream2 = LineupManager.StaticMethod2())
					{
						gzipStream.CopyTo(memoryStream2);
						byte[] bytes = memoryStream2.ToArray();
						@string = Encoding.UTF8.GetString(bytes);
					}
				}
			}
			return @string;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000054B0 File Offset: 0x000036B0
		
		public static string ExportLineup()
		{
			List<string> list = new();
			foreach (Plant plant in Board.Instance.boardEntity.plantArray)
			{
				if (!(plant == null))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
					defaultInterpolatedStringHandler.AppendFormatted(plant.thePlantColumn);
					defaultInterpolatedStringHandler.AppendLiteral(",");
					defaultInterpolatedStringHandler.AppendFormatted(plant.thePlantRow);
					defaultInterpolatedStringHandler.AppendLiteral(",");
					defaultInterpolatedStringHandler.AppendFormatted((int)plant.thePlantType);
					string item = defaultInterpolatedStringHandler.ToStringAndClear();
					list.Add(item);
				}
			}
			return LineupManager.CompressString(string.Join(";", list));
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005564 File Offset: 0x00003764
		public static void ImportLineup(string lineupCode)
		{
			lineupCode = LineupManager.DecompressString(lineupCode);
			string[] array = lineupCode.Split(';', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(',', StringSplitOptions.None);
				int a_;
				int a_2;
				int a_3;
				if (array2.Length == 3 && int.TryParse(array2[0], out a_) && int.TryParse(array2[1], out a_2) && int.TryParse(array2[2], out a_3))
				{
					LineupManager.StaticMethod23(CreatePlant.Instance, a_, a_2, (PlantType)a_3, null, default(Vector2), false, true, null);
				}
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000055E8 File Offset: 0x000037E8
		
		public static string ExportZombieLineup()
		{
			List<string> list = new();
			foreach (Zombie zombie in Board.Instance.zombieArray)
			{
				if (!(zombie == null))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
					defaultInterpolatedStringHandler.AppendFormatted(zombie.theZombieRow);
					defaultInterpolatedStringHandler.AppendLiteral(",");
					defaultInterpolatedStringHandler.AppendFormatted(zombie.axis.position.x);
					defaultInterpolatedStringHandler.AppendLiteral(",");
					defaultInterpolatedStringHandler.AppendFormatted((int)zombie.theZombieType);
					string item = defaultInterpolatedStringHandler.ToStringAndClear();
					list.Add(item);
				}
			}
			return LineupManager.CompressString(string.Join(";", list));
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000056A4 File Offset: 0x000038A4
		public static void ImportZombieLineup(string lineupCode)
		{
			lineupCode = LineupManager.DecompressString(lineupCode);
			string[] array = lineupCode.Split(';', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(',', StringSplitOptions.None);
				if (array2.Length == 3)
				{
					int num;
					float num2;
					int num3;
					if (int.TryParse(array2[0], out num) && float.TryParse(array2[1], out num2) && int.TryParse(array2[2], out num3))
					{
						CreateZombie.Instance.SetZombie(num, (ZombieType)num3, num2, false);
					}
				}
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000571C File Offset: 0x0000391C
		
		public static string ExportMixLineup()
		{
			return LineupManager.ExportLineup() + "|" + LineupManager.ExportZombieLineup();
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005740 File Offset: 0x00003940
		public static void ImportMixLineup(string lineupCode)
		{
			string[] array = lineupCode.Split('|', StringSplitOptions.None);
			LineupManager.ImportLineup(array[0]);
			LineupManager.ImportZombieLineup(array[1]);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00005768 File Offset: 0x00003968
		static MemoryStream StaticMethod2()
		{
			return new();
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005784 File Offset: 0x00003984
		static GZipStream StaticMethod3(Stream A_0, CompressionMode A_1, bool A_2)
		{
			return new GZipStream(A_0, A_1, A_2);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000579C File Offset: 0x0000399C
		static MemoryStream StaticMethod9(byte[] A_0)
		{
			return new MemoryStream(A_0);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000057B0 File Offset: 0x000039B0
		static GZipStream StaticMethod10(Stream A_0, CompressionMode A_1)
		{
			return new GZipStream(A_0, A_1);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000057C4 File Offset: 0x000039C4
		static Plant StaticMethod23(CreatePlant A_0, int A_1, int A_2, PlantType A_3, Plant A_4, Vector2 A_5, bool A_6, bool A_7, Plant A_8)
		{
			return A_0.SetPlant(A_1, A_2, A_3, A_4, A_5, A_6, A_7, A_8);
		}
	}
}
