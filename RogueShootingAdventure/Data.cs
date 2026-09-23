namespace RogueShootingAdventure
{
    public class EvolutionData
    {
        PlantType type1 { get; }
        PlantType type2 { get; }
        PlantType type3 { get; }

        public EvolutionData(PlantType plantType1, PlantType plantType2, PlantType plantType3)
        {
            type1 = plantType1;
            type2 = plantType2;
            type3 = plantType3;
        }
    }
}