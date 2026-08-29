using System;
using System.Runtime.CompilerServices;

namespace Modified.Command
{
	// Token: 0x02000065 RID: 101
	[AttributeUsage(AttributeTargets.Method)]
	public class CommandAttribute : Attribute
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060001CE RID: 462 RVA: 0x00012788 File Offset: 0x00010988
		public string CommandName { get; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060001CF RID: 463 RVA: 0x0001279C File Offset: 0x0001099C
		public string Description { get; }

		// Token: 0x060001D0 RID: 464 RVA: 0x000127B0 File Offset: 0x000109B0
		public CommandAttribute(string A_1, string A_2 = "无")
		{
			this.CommandName = A_1;
			this.Description = A_2;
		}

		// Token: 0x04000113 RID: 275
		[CompilerGenerated]
		private readonly string Field0;

		// Token: 0x04000114 RID: 276
		[CompilerGenerated]
		private readonly string Field1;
	}
}
