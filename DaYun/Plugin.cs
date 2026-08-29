global using BepInEx;
global using HarmonyLib;
global using UnityEngine;
global using CustomPlantClass.Main;
namespace DaYun
{
    [BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
    public class Plugin : ModPlugin
    {
        
    }
    [HarmonyPatch(typeof(BigWallNut),nameof(BigWallNut.OnTriggerStay2D))]
    public static class BigWallNut_OnTriggerStay2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(BigWallNut __instance, Collider2D collision)
        {
            // 1. Must be a zombie
            if (!collision.TryGetComponent<Zombie>(out var zombie))
                return true;

            // 3. Must be same row
            if (zombie.theZombieRow != __instance.thePlantRow)
                return true;

            // 4. Must not be dead
            if (zombie.beforeDying)
                return true;

            if(zombie.theZombieType==ZombieType.HorseBoss || zombie.theZombieType==ZombieType.ZombieBoss || zombie.theZombieType==ZombieType.ZombieBoss2)
            zombie.TakeDamage(999,__instance.Cast<IDamageMaker>(),DamageType.Shieldless,__instance.thePlantType);
            if(TypeMgr.IsBossZombie(zombie.theZombieType)) return true;

            if(TypeMgr.BigZombie(zombie.theZombieType)) zombie.Crashed();
            else zombie.FlyAway();
            return false;
        }
    }
    public static class MyPluginInfo
    {
        public const string PluginGuid = "DaYun.Bepinex";
        public const string PluginName = "DaYun";
        public const string PluginVersion = CustomPlantClass.MyPluginInfo.TargetVersion;
    }
}
