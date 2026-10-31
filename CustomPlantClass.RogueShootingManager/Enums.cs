namespace CustomPlantClass.RogueShootingManager
{
    /// <summary>Classifies a plant by its role in a Rogue Shooting run.</summary>
    public enum Roles
    {
        /// <summary>Deals damage to enemies.</summary>
        Attacker = 0,
        /// <summary>Provides utility or support.</summary>
        Supporter = 1,
        /// <summary>Protects the player's defenses.</summary>
        Defense = 2,
        /// <summary>Provides an explosive or one-use effect.</summary>
        Insta = 3,
        /// <summary>Produces resources.</summary>
        Producer = 4
    }
    /// <summary>Classifies a buff by its source or upgrade category.</summary>
    public enum ShootingBuffType
    {
        /// <summary>A plant-specific upgrade.</summary>
        UniqueUpgrade = 0,
        /// <summary>A qualitative change to a plant.</summary>
        QualitativeChange = 1,
        /// <summary>A super-upgrade for a plant.</summary>
        SuperUpgrade = 2,
        /// <summary>A general-purpose buff.</summary>
        General = 3,
        /// <summary>A curse effect.</summary>
        CurseBuff = 4,
        /// <summary>A buff offered as part of a mission or trial.</summary>
        MissionBuff = 5
    }
}