using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomPlantClass.CustomizeLib
{
    public static class SkinBehaviourMgr
    {
        public static void Init()
        {
            ClassInjector.RegisterTypeInIl2Cpp<UltimateWinterMelonSkin>();
        }
    }

    public class UltimateWinterMelonSkin : MonoBehaviour
    {
        #nullable disable
        public UltimateWinterMelon plant;
        #nullable enable

        public void ResetSummon()
        {
            plant.summoning = false;
        }
    }
}
