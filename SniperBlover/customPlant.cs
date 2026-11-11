using System.Linq;
using UnityEngine;
using CustomPlantClass;
using CustomPlantClass.Runtime.Core;

namespace SniperBlover.BepInEx
{
	// Token: 0x02000002 RID: 2
	public class SniperBlover : MonoBehaviour, IPlantDieHandler
	{

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		public SniperPea plant
		{
			get
			{
				return GetComponent<SniperPea>();
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002070 File Offset: 0x00000270
		public void Awake()
		{
			plant.ac = plant.transform.GetChild(0).gameObject;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002094 File Offset: 0x00000294
		public void Start()
		{
			if (plant != null)
			{
                Destroy(this);
				return;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020AC File Offset: 0x000002AC
		public Bullet AnimShoot_Custom()
		{
			return plant.AnimShoot();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020BC File Offset: 0x000002BC
		public void FixedUpdate()
		{
			bool flag;
			if (GameAPP.theGameStatus == GameStatus.InGame)
			{
				SniperPea plant = this.plant;
				SniperPea sniperPea = null;
				flag = plant != sniperPea;
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				return;
			}
			if (prevCD == -114514f)
			{
				prevCD = plant.thePlantAttackInterval;
				return;
			}
			Plant otherplant = Lawnf.Get1x1Plants(plant.thePlantColumn, plant.thePlantRow).FirstOrDefault(plant => plant!=null);
			Plant plant2 = otherplant;
			if (plant2 != null)
			{
				plant.thePlantAttackInterval = otherplant.thePlantAttackInterval;
				return;
			}
			plant.thePlantAttackInterval = prevCD;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002178 File Offset: 0x00000378
		void IPlantDieHandler.OnDie(Plant.DieReason reason)
		{
			if (reason == Plant.DieReason.ByShovel)
			{
				DroppedCard droppedCard = Lawnf.SetDroppedCard(transform.position, PlantType.SniperPea, 0);
				return;
			}
		}

		// Token: 0x04000001 RID: 1
		internal float prevCD = -114514f;
	}
}
