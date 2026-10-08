namespace TestPlugin
open BepInEx.Unity.IL2CPP
open BepInEx
open CustomPlantClass

[<BepInPlugin("FSharpPluginTest", "F#", "1.0.0")>]
type Plugin () =
    inherit BasePlugin()

    override this.Load(): unit =
        ModLogger.LogInfo("F# test plugin", "Load successful!")

