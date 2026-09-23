using Core;

namespace CustomPlantClass.RogueShootingManager
{
    public class CustomShootingCurseComponent : MonoBehaviour
    {
        BuffID reversed;
        BuffID curseBuff;
        Func<Plant,bool> canreverse;
        Action onreverse;
        PlantType thePlantType;
        bool done = false;
        public void Init(BuffID curseBuff, BuffID reversed,Func<Plant,bool> canreverse,Action onreverse,PlantType thePlantType)
        {
            this.curseBuff = curseBuff;
            this.reversed=reversed;
            this.canreverse = canreverse;
            this.thePlantType = thePlantType;
            this.onreverse = onreverse;
        }
        public void FixedUpdate()
        {
            if(ShootingManager.Instance.TryGetPlant(thePlantType,out Plant plant) && canreverse(plant) && !done)
            {
                done = true;
                TravelMgr.Instance.GetNormalBuff(reversed);
                if(onreverse!=null)onreverse();
                if (TravelMgr.Instance.data.advBuffs.Contains(curseBuff))
                {
                    TravelMgr.Instance.data.advBuffs.Remove(curseBuff);
                }
                InGameText.Instance.ShowText($"{Lawnf.GetName(thePlantType)}的诅咒效果已反转",1.5f,true);
                Destroy(this);
            }
        }
    }
}