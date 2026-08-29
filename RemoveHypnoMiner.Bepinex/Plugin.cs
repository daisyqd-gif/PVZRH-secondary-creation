﻿using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomPlantClass.Main;
[assembly:CustomMod("RemoveHypnoMiner")]
namespace RemoveHypnoMiner
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class DisableMinerPlugin : BasePlugin
    {
        public const string PluginGuid = "RemoveHypnoMiner.Bepinex";
        public const string PluginName = "RemoveHypnoMiner";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;

        public override void Load()
        {
            DataMgr.AddCustomOnZombieSpawnEvent(ZombieType.HypnoJalapenoPickaxeZombie,ZombieType.RandomZombie,(Zombie z)=>{z.Die(1);return true;});
            DataMgr.AddCustomOnZombieSpawnEvent(ZombieType.Jackbox_c,ZombieType.Jackbox_b,(Zombie z)=>{z.Die(1);return true;});
            DataMgr.AddCustomOnZombieSpawnEvent(ZombieType.UltimateSwordZombie,ZombieType.EternalZombie_c,(Zombie z)=>{z.Die(1);return true;});

            Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }
}
