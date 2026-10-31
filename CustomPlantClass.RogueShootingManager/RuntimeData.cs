using Core;

namespace CustomPlantClass.RogueShootingManager
{
    /// <summary>Applies a registered curse and reverses it when its condition is met.</summary>
    public class CustomShootingCurseComponent : MonoBehaviour
    {
        BuffID reversed;
        BuffID curseBuff;
        Func<Plant,bool> canreverse;
        Action onreverse;
        PlantType thePlantType;
        bool done = false;
        /// <summary>Initializes the curse and its reversal callbacks.</summary>
        /// <param name="curseBuff">The curse buff to remove when reversed.</param>
        /// <param name="reversed">The buff granted when the curse is reversed.</param>
        /// <param name="canreverse">Determines whether the plant can reverse the curse.</param>
        /// <param name="onreverse">Runs after the curse is reversed.</param>
        /// <param name="thePlantType">The plant type monitored for reversal.</param>
        public void Init(BuffID curseBuff, BuffID reversed,Func<Plant,bool> canreverse,Action onreverse,PlantType thePlantType)
        {
            this.curseBuff = curseBuff;
            this.reversed=reversed;
            this.canreverse = canreverse;
            this.thePlantType = thePlantType;
            this.onreverse = onreverse;
        }
        /// <summary>Checks whether the configured plant has reversed its curse.</summary>
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