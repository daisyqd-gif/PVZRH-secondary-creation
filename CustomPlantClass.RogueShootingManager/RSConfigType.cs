namespace CustomPlantClass.RogueShootingManager
{
    /// <summary>Identifies a built-in plant configuration in Rogue Shooting.</summary>
    public enum RSConfigType
    {
        /// <summary>Configuration identifier for Peashooter.</summary>
        Peashooter,
        /// <summary>Configuration identifier for Cherry Gatling.</summary>
        CherryGatling,
        /// <summary>Configuration identifier for Helmet Gatling.</summary>
        HelmetGatling,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Gatling.</summary>
        UltimateGatling,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Helmet Gatling.</summary>
        UltimateHelmetGatling,
        /// <summary>Configuration identifier for Lantern Split.</summary>
        LanternSplit,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Lantern Split.</summary>
        UltimateLanternSplit,
        /// <summary>Configuration identifier for Sniper Pea.</summary>
        SniperPea,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Doom Sniper.</summary>
        DoomSniper,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Fire Sniper.</summary>
        FireSniper,
        /// <summary>Configuration identifier for Snow Pea Shooter.</summary>
        SnowPeaShooter,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Magic Snow Pea 2.</summary>
        MagicSnowPea2,
        /// <summary>Configuration identifier for Wall Nut.</summary>
        WallNut,
        /// <summary>Configuration identifier for Super Chomper.</summary>
        SuperChomper,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Chomper.</summary>
        UltimateChomper,
        /// <summary>Configuration identifier for Tall Nut.</summary>
        TallNut,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Tall Nut.</summary>
        UltimateTallNut,
        /// <summary>Configuration identifier for Cabbage Nut.</summary>
        CabbageNut,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Melon Nut.</summary>
        MelonNut,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Magnet Nut.</summary>
        MagnetNut,
        /// <summary>Configuration identifier for Potato Mine.</summary>
        PotatoMine,
        /// <summary>Configuration identifier for Pea Mine.</summary>
        PeaMine,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Three Mine.</summary>
        ThreeMine,
        /// <summary>Configuration identifier for Chomper.</summary>
        Chomper,
        /// <summary>Configuration identifier for Cherry Chomper.</summary>
        CherryChomper,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Doom Chomper.</summary>
        DoomChomper,
        /// <summary>Configuration identifier for Big Chomper.</summary>
        BigChomper,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Big Chomper.</summary>
        UltimateBigChomper,
        /// <summary>Configuration identifier for Small Puff.</summary>
        SmallPuff,
        /// <summary>Configuration identifier for Ice Puff.</summary>
        IcePuff,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Snow Gatling Puff.</summary>
        SnowGatlingPuff,
        /// <summary>Configuration identifier for Iron Puff.</summary>
        IronPuff,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for IFV Iron Puff.</summary>
        IFVIronPuff,
        /// <summary>Configuration identifier for Fume Shroom.</summary>
        FumeShroom,
        /// <summary>Configuration identifier for Ice Fume Shroom.</summary>
        IceFumeShroom,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Fume.</summary>
        UltimateFume,
        /// <summary>Configuration identifier for Garlic Fume.</summary>
        GarlicFume,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Poison Fume.</summary>
        UltimatePoisonFume,
        /// <summary>Configuration identifier for Gloom Shroom.</summary>
        GloomShroom,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Gloom.</summary>
        UltimateGloom,
        /// <summary>Configuration identifier for Hypno Shroom.</summary>
        HypnoShroom,
        /// <summary>Configuration identifier for Hypno Nut.</summary>
        HypnoNut,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Hypno Emperor.</summary>
        HypnoEmperor,
        /// <summary>Configuration identifier for Scaredy Shroom.</summary>
        ScaredyShroom,
        /// <summary>Configuration identifier for Super Hypno.</summary>
        SuperHypno,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Hypno.</summary>
        UltimateHypno,
        /// <summary>Configuration identifier for Scaredy Doom.</summary>
        ScaredyDoom,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Doom Scared.</summary>
        UltimateDoomScared,
        /// <summary>Configuration identifier for Squash.</summary>
        Squash,
        /// <summary>Configuration identifier for Squalour.</summary>
        Squalour,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Cattail Lour.</summary>
        CattailLour,
        /// <summary>Configuration identifier for Cherry Squash.</summary>
        CherrySquash,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Nuclear Squash.</summary>
        NuclearSquash,
        /// <summary>Configuration identifier for Three Peater.</summary>
        ThreePeater,
        /// <summary>Configuration identifier for Three Squash.</summary>
        ThreeSquash,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Super Three Peater.</summary>
        SuperThreePeater,
        /// <summary>Configuration identifier for Big Gatling.</summary>
        BigGatling,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Big Gatling.</summary>
        UltimateBigGatling,
        /// <summary>Configuration identifier for Caltrop.</summary>
        Caltrop,
        /// <summary>Configuration identifier for Spike Rock.</summary>
        SpikeRock,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Obsidian Spike.</summary>
        ObsidianSpike,
        /// <summary>Configuration identifier for Caltrop Nut.</summary>
        CaltropNut,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Obsidian Wall Nut.</summary>
        ObsidianWallNut,
        /// <summary>Configuration identifier for Cactus.</summary>
        Cactus,
        /// <summary>Configuration identifier for Doom Cactus.</summary>
        DoomCactus,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Cactus.</summary>
        UltimateCactus,
        /// <summary>Configuration identifier for Star Fruit.</summary>
        StarFruit,
        /// <summary>Configuration identifier for Super Star.</summary>
        SuperStar,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Star.</summary>
        UltimateStar,
        /// <summary>Configuration identifier for Sword Star.</summary>
        SwordStar,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Abyss Sword Star.</summary>
        AbyssSwordStar,
        /// <summary>Configuration identifier for Cabbagepult.</summary>
        Cabbagepult,
        /// <summary>Configuration identifier for Gold Cabbage.</summary>
        GoldCabbage,
        /// <summary>Configuration identifier for Ultimate Cabbage.</summary>
        UltimateCabbage,
        /// <summary>Configuration identifier for Cabbage Cannon.</summary>
        CabbageCannon,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Cabbage Cannon.</summary>
        UltimateCabbageCannon,
        /// <summary>Configuration identifier for Melonpult.</summary>
        Melonpult,
        /// <summary>Configuration identifier for Super Melon.</summary>
        SuperMelon,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Melon.</summary>
        UltimateMelon,
        /// <summary>Configuration identifier for Fire Melon.</summary>
        FireMelon,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Spring.</summary>
        UltimateSpring,
        /// <summary>Configuration identifier for Silver Melon.</summary>
        SilverMelon,
        /// <summary>Configuration identifier for Gold Melon.</summary>
        GoldMelon,
        /// <summary>Configuration identifier for Winter Melon.</summary>
        WinterMelon,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Winter Melon.</summary>
        UltimateWinterMelon,
        /// <summary>Configuration identifier for Cornpult.</summary>
        Cornpult,
        /// <summary>Configuration identifier for Portal Corn.</summary>
        PortalCorn,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Corn.</summary>
        UltimateCorn,
        /// <summary>Configuration identifier for Umbrellaleaf.</summary>
        Umbrellaleaf,
        /// <summary>Configuration identifier for Lantern Umbrella.</summary>
        LanternUmbrella,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Laser Umbrella.</summary>
        LaserUmbrella,
        /// <summary>Configuration identifier for Bamboo.</summary>
        Bamboo,
        /// <summary>Configuration identifier for Lotus Bamboo.</summary>
        LotusBamboo,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Bamboo.</summary>
        UltimateBamboo,
        /// <summary>Configuration identifier for Spruce Shooter.</summary>
        SpruceShooter,
        /// <summary>Configuration identifier for Super Spruce.</summary>
        SuperSpruce,
        //terminal plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Spruce.</summary>
        UltimateSpruce,
        //expert plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Sniper Gatling.</summary>
        UltimateSniperGatling,
        //expert plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Minigun.</summary>
        UltimateMinigun,
        //expert plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Blover.</summary>
        UltimateBlover,
        //expert plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Emerale Blover.</summary>
        EmeraleBlover,
        //expert plant, it is not recommended to add entries here
        /// <summary>Configuration identifier for Ultimate Star Torch.</summary>
        UltimateStarTorch
    }
}