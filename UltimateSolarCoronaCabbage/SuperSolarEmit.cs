namespace UltimateSolarCoronaCabbage_Remade
{
    public class SuperSolarEmit : CustomParticle
    {
        private float counter=1f;
        private Vector2 fromPos=default;
        private Vector2 currPos=default;
        public Vector2 toPos {get; set;}=default;
        public int Damage {get; set;} = 0;
        public int Row {get; set;} = 0;
        public void FixedUpdate()
        {
            currPos=Vector2.MoveTowards(currPos,toPos,1f);
            SetTarget(fromPos, currPos);
            counter-=Time.deltaTime;
            if (counter <= 0f)
            {
                InstanceManager.Board.boardAction.CreateCherryExplode(
                    toPos,
                    Row,
                    CherryBombType.Sun,   // (CherryBombType)1
                    Damage,
                    Plugin.DataContainer.thePlantType,
                    null,
                    true
                );
                Die();
            }
        }
        public void SetTarget(Vector2 from, Vector2 to)
        {
            if (PlantMgr.IsNotNull(gameObject.GetComponentInChildren<LineRenderer>(), out var line))
            {
                // Ensure the line has exactly 2 points
                if (line.positionCount != 2)
                    line.positionCount = 2;

                // Assign start and end positions
                line.SetPosition(0, from);
                line.SetPosition(1, to);
            }
        }
        public static void SetSuperSolarEmit(Vector2 to,int row, int damage)
        {
            MonoBehaviour solar;
            if (PlantMgr.IsNotNullMonoBehaviour(SuperSolar.Instance, out var ss))
                solar = ss;
            else if (PlantMgr.IsNotNullMonoBehaviour(Solar.Instance, out var s))
                solar = s;
            else
                return;
            var comp=Instantiate(Plugin.DataContainer.superSolarEmit,Board.Instance.transform).GetComponent<SuperSolarEmit>();
            comp.currPos=solar.transform.position;
            comp.SetTarget(comp.currPos,comp.currPos);
            comp.toPos=to;
            comp.fromPos=solar.transform.position;
            comp.Row=row;
            comp.Damage=damage;
        }
    }
}
