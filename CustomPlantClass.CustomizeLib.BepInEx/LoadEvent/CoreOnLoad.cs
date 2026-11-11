using CustomPlantClass.CustomizeLib.Hook;
using CustomPlantClass.CustomizeLib.ToolInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPlantClass.CustomizeLib.LoadEvent
{
    internal static class CoreOnLoad
    {
        public static void OnLoad()
        {
            //EventListenr.AddListener(ListenerType.OnGameLaunch, ApplyNativeHookTools.RunAll);
        }
    }
}
