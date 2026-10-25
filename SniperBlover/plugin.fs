#nowarn "3391"
namespace SniperBlover.BepInEx

open BepInEx
open CustomPlantClass
open CustomPlantClass.Main
open CustomizeLib.BepInEx
open System.Reflection
open global.UnityEngine

[<BepInPlugin(MyPluginInfo.PluginGUID, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)>]
type public Plugin() =
    inherit ModPlugin()
    let mutable AssetBundle = null
    let mutable PlantID = new ID 0

    override this.InitializeMod() : unit =
        AssetBundle <- AssetMgr.LoadBundleFromResource(Assembly.GetExecutingAssembly(), "sniperblover", false)
        PlantID <- DataMgr.AllocateID()

    override this.InitializePlants() : unit =

        let mutable plantdata = new BaseCustomPlantData()
        plantdata.PlantId <- PlantID
        plantdata.Prefab <- AssetBundle.GetAsset<GameObject> "SniperBloverPrefab"
        plantdata.Preview <- AssetBundle.GetAsset<GameObject> "SniperBloverPreview"
        plantdata.Fusions <- ListHelper.MirrorTuple((PlantType.SniperPea, PlantType.Blover))
        plantdata.AttackInterval <- 3f
        plantdata.ProduceInterval <- 0f
        plantdata.AttackDamage <- 500
        plantdata.MaxHealth <- 300
        plantdata.Cd <- 3f
        plantdata.Sun <- 700
        plantdata.DefaultBullet <- BulletType.Bullet_pea
        plantdata.CanPF <- false
        plantdata.CanStarUp <- false
        plantdata.CardColor <- CardLevel.Purple
        plantdata.IsRainbowCard <- false
        plantdata.IsUltimatePlant <- true
        plantdata.CardRepeatAmt <- 1
        plantdata.Name <- "浮空狙击射手"

        plantdata.AlmanacEntry <-
            DataMgr.CreateAlmanacEntry(
                "定期狙击一只僵尸，造成高额伤害。",
                recipe = struct ("狙击射手", "三叶草"),
                attackinterval = struct (500, 3f),
                specialeffects =
                    [|
                        "攻击方式：锁定最近的目标，直至其死亡。每第6次攻击爆头，造成100万伤害"
                        "可以消耗机枪射手植物，生成狙击射手卡牌"
                        "下方植物攻击间隔低于3秒时，自身攻速改为与其一致"
                    |],
                variantswitch = struct ("铲除", "三叶草")
            )

        DataMgr.RegisterCustomPlant<SniperPea, SniperBlover> plantdata |> ignore
        CustomCore.TypeMgrExtra.FlyingPlants.Add PlantID

        ModLogger.LogInfo(
            "SniperBlover.BepInEx",
            $"{MyPluginInfo.PluginName} {MyPluginInfo.PluginVersion} loaded successfully."
        )
