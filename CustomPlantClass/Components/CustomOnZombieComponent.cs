#nullable enable

namespace CustomPlantClass
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CustomOnZombieBomb : CustomEffect
    {
        public Zombie zombie { get => transform.GetComponent<Zombie>(); }
        public override CustomEffectUsage Usage { get => CustomEffectUsage.Zombie; }
        public virtual GameObject obj { get => null!; }
        public int Damage { get; set; } = 0;
        public override bool CanCountDown { get; } = true;
        public override bool TimedEffect { get; } = true;
        public override float CycleCountDown { get; set; } = 1f;
        public override float RemoveCountDown { get; set; } = 5f;
        public virtual PlantType FromType { get; } = PlantType.Nothing;
        private GameObject obj_cache = null!;
        public Board board => Board.Instance;
        public override void OnAddEffect()
        {
            if (zombie == null)
            {
                Destroy(this);
                return;
            }
            var parentScale = transform.parent.localScale;
            var localScale = transform.localScale;

            transform.localScale = new Vector3(
                localScale.x * 0.3f / parentScale.x,
                localScale.y * 0.3f / parentScale.y,
                localScale.z * 0.3f / parentScale.z
            );
            if (obj != null)
            {
                obj_cache = Instantiate(obj, zombie.axis.transform.position + new Vector3(0f, 0.95f, 0f), Quaternion.identity, zombie.transform);
            }
        }
        public override void OnTimerZero()
        {
            if (board != null) board.boardAction.CreateCherryExplode(transform.position, PlantMgr.getRow(transform.position.y), CherryBombType.Bullet, Mathf.RoundToInt(Damage / 4));
        }
        public override void OnRemoveEffect()
        {
            if (board != null) board.boardAction.CreateCherryExplode(transform.position, PlantMgr.getRow(transform.position.y), CherryBombType.Normal, Damage);
            if (obj_cache != null && !obj_cache.IsDestroyed()) Destroy(obj_cache);
        }
    }
}
