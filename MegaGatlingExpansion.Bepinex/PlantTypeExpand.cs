namespace MegaGatlingExpansion
{
    public readonly struct PlantTypeExpand
    {
        public readonly int Value;

        public PlantTypeExpand(int value)
        {
            Value = value;
        }

        public static implicit operator PlantTypeExpand(int value)
            => new PlantTypeExpand(value);

        public static implicit operator PlantTypeExpand(ID value)
            => new PlantTypeExpand(value);

        public static implicit operator int(PlantTypeExpand type)
            => type.Value;

        public static implicit operator PlantType(PlantTypeExpand type)
            => (PlantType)type.Value;

        public static implicit operator ID(PlantTypeExpand type)
            => (ID)type.Value;

        public static implicit operator PlantTypeExpand(PlantType type)
            => new PlantTypeExpand((int)type);

        public override int GetHashCode() => Value;
        public override bool Equals(object obj)
            => obj is PlantTypeExpand other && other.Value == Value;

        public static readonly PlantTypeExpand MegaGatlingPea = DataMgr.AllocateID();/*13080;*/
        public static readonly PlantTypeExpand IceMegaGatlingPea = DataMgr.AllocateID();/*13081;*/
        public static readonly PlantTypeExpand FireMegaGatlingPea = DataMgr.AllocateID();/*13082;*/
        public static readonly PlantTypeExpand PrimalMegaGatlingPea = DataMgr.AllocateID();/*13083;*/
        public static readonly PlantTypeExpand GooMegaGatlingPea = DataMgr.AllocateID();/*13084;*/
        public static readonly PlantTypeExpand ElectricMegaGatlingPea = DataMgr.AllocateID();/*13085;*/
        public static readonly PlantTypeExpand DoomMegaGatlingPea = DataMgr.AllocateID();/*13086;*/
        public static readonly PlantTypeExpand CherryMegaGatlingPea = DataMgr.AllocateID();/*13087;*/
        public static readonly PlantTypeExpand ThreeMegaGatlingPea = DataMgr.AllocateID();/*13088;*/
        public static readonly PlantTypeExpand WildMegaGatlingPea = DataMgr.AllocateID();/*13089;*/
        public static readonly PlantTypeExpand UltimateDoomMegaGatlingPea = DataMgr.AllocateID();/*13090;*/
        public static readonly PlantTypeExpand RegularCherryMegaGatlingPea = DataMgr.AllocateID();/*13091;*/
        public static readonly PlantTypeExpand HypnoMegaGatlingPea = DataMgr.AllocateID();/*13092;*/
        public static readonly PlantTypeExpand ChronoMegaGatlingPea = DataMgr.AllocateID();/*13093;*/
        public static readonly PlantTypeExpand SunMegaGatlingPea = DataMgr.AllocateID();/*13094;*/
        public static readonly PlantTypeExpand ExplodeGatlingBlover = DataMgr.AllocateID();/*13095;*/
        public static readonly PlantTypeExpand StarMegaGatlingPea = DataMgr.AllocateID();/*13095;*/
    }
}
