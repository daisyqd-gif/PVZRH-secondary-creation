namespace SniperBlover.BepInEx

open global.UnityEngine
open CustomizeLib.BepInEx
open System.Linq
open CustomPlantClass

type SniperBlover() =
    inherit MonoBehaviour()
    let mutable prevCD = -114514f
    interface IPlantDieHandler with        
        member this.OnDie(reason: Plant.DieReason) = 
            if reason = Plant.DieReason.ByShovel then
                Lawnf.SetDroppedCard(this.transform.position,PlantType.SniperPea) |> ignore

    member this.plant = this.GetComponent<SniperPea>()

    member this.Awake() =
        this.plant.ac <- this.plant.transform.GetChild(0).gameObject

    member this.Start() =
        if this.plant = null then
            UnityEngine.Object.Destroy this

    member this.AnimShoot_Custom() = this.plant.AnimShoot()

    member this.FixedUpdate() =
        if GameAPP.theGameStatus = GameStatus.InGame && this.plant <> null then
            if prevCD = -114514f then
                prevCD <- this.plant.thePlantAttackInterval
            else
                let otherplant =
                    Lawnf
                        .Get1x1Plants(this.plant.thePlantColumn, this.plant.thePlantRow)
                        .ToSystemList()
                        .FirstOrDefault(fun p -> p <> this.plant)

                if otherplant <> null then
                    this.plant.thePlantAttackInterval <- otherplant.thePlantAttackInterval
                else
                    this.plant.thePlantAttackInterval <- prevCD
