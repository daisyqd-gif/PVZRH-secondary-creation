using CustomPlantClass.RogueShootingManager;
using GameLevel.RogueShooting;

namespace FreezingCherry.BepInEx
{
    public class RSCore
    {
        public static BaseBuff? SynergySuperUpgrade;
        public RSCore(Plugin plugin)
        {
            var ubuff1 = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = Plugin.Ultimate_PlantID,
                CustomTitle = "强化：寒霜核能樱桃",
                CustomDescription = "寒霜核能樱桃 +1",
                CustomBuffType = ShootingBuffType.UniqueUpgrade,
                CustomOnGet = delegate
                {
                    if(ShootingManager.Instance.TryGetPlant(Plugin.Ultimate_PlantID,out var plant))
                        plant.shootingLevel++;
                },
            });
            var qbuff1 = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = Plugin.Ultimate_PlantID,
                CustomTitle = "质变：凝冰之力",
                CustomDescription = "获得的能量x2",
                CustomBuffType = ShootingBuffType.QualitativeChange,
                CustomOnGet = delegate
                {
                   TravelMgr.Instance.GetNormalBuff(Plugin.buff1);
                },
            });
            var sbuff1 = RegistryHelper.MakeBuffType(new()
            {
                CustomPlantType = Plugin.Ultimate_PlantID,
                CustomTitle = "超进化：连携",
                CustomDescription = "解锁词条：霜陨子母弹",
                CustomBuffType = ShootingBuffType.SuperUpgrade,
                CustomOnGet = delegate
                {
                   TravelMgr.Instance.GetNormalBuff(Plugin.synergyBuff);
                },
            });
            var cfg = RegistryHelper.MakeConfigType(new()
            {
                CustomPlantType = Plugin.Ultimate_PlantID,
                CustomBuffs = () => 
                    ShootingManager.Instance != null && ShootingManager.Instance.CurrentPlants.Contains(PlantType.NuclearSquash) ?
                    [new DamageBuff(Plugin.Ultimate_PlantID),ubuff1,qbuff1,sbuff1]:
                    [new DamageBuff(Plugin.Ultimate_PlantID),ubuff1,qbuff1],
                CustomReinforcePlant = null,
                CustomRole = RegistryHelper.GetStringFromRole(Roles.Supporter),
            });
            RegistryHelper.AddCustomExpertPlant(Plugin.Ultimate_PlantID,cfg);
        }
    }
}