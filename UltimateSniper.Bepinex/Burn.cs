
#nullable enable
using CustomPlantClass.Runtime;

namespace UltimateSniper
{
    public class Burn : MonoBehaviour
    {
        public int damage = 0;
        public float deathTime = 20f;
        public float burnTime = 1f;
        public Board? board;
        public float columnX = 3.5f;
        public bool IsSpecial = false;

        public void AddDamage(int damage) => this.damage += damage;

        public void Start()
        {
            board = Board.Instance;
            if (board == null)
            {
                Destroy(gameObject);
                return;
            }
            ;
            columnX = (Mouse.Instance.GetBoxXFromColumn(board.columnNum - 1) - Mouse.Instance.GetBoxXFromColumn(0)) / board.columnNum;
        }

        public void Update()
        {
            if (GameAPP.theGameStatus == GameStatus.InGame)
            {
                deathTime -= Time.deltaTime;
                if (deathTime <= 0f)
                {
                    if (IsSpecial && Utils.IsGameRunning() && board != null)
                    {
                        Doom.SetDoom(board, transform.position, DoomType.Fire);
                    }
                    Destroy(gameObject);
                    return;
                }
                burnTime -= Time.deltaTime;
                if (burnTime <= 0f)
                {
                    BurnEvent();
                    burnTime = 1f;
                    return;
                }
            }
        }

        public void BurnEvent()
        {
            var zombies = Physics2D.OverlapCircleAll(transform.position, columnX * 1f, LayerMask.GetMask("Zombie"));
            foreach (var collider in zombies)
            {
                if (collider == null || collider.IsDestroyed() || collider.gameObject == null || collider.gameObject.IsDestroyed()) continue;
                if (!collider.gameObject.TryGetComponent<Zombie>(out var zombie) || zombie == null || zombie.IsDestroyed()) continue;
                zombie.TakeDamage(DmgType.Carred, damage, UltimateMegaGatlingPea.PLANT_ID);
            }
        }

        public static bool HasZombieBurn(Zombie zombie) => zombie.GetComponentsInChildren<Burn>().Count > 0;

        public static Burn? TryAddZombieBurn(Zombie zombie, bool isSpecial = false)
        {
            if (zombie.gameObject == null || zombie == null || zombie.IsDestroyed()) return null;
            var component = zombie.GetComponentInChildren<Burn>();
            if (component != null)
            {
                component.deathTime += 20;
                if (isSpecial && component.IsSpecial && Utils.IsGameRunning() && component.board != null)
                {
                    Doom.SetDoom(component.board, component.transform.position, DoomType.Fire);
                    component.IsSpecial = false;
                }
                else component.IsSpecial = isSpecial;
                component.damage *= 2;
                return component;

            }
            var result = Instantiate(Plugin.ZombieBurn, zombie.axis.transform.position + new Vector3(0f, 0.95f, 0f), Quaternion.identity, zombie.transform);
            return result.GetComponent<Burn>();
        }
    }
    // Token: 0x02000011 RID: 17
    public class ZombieBurn_2 : MonoBehaviour
    {
        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600007D RID: 125 RVA: 0x00004494 File Offset: 0x00002694
        private Zombie Prop0 => GetComponent<Zombie>();

        // Token: 0x04000038 RID: 56
        public int BurnLevel=0;

        // Token: 0x04000039 RID: 57
        public float BurnTimer = 1f;

        [ResetOnBoardDestroy(0.75f)]
        public static float Radius = 0.75f;
        public static bool ZhiBian => Lawnf.TravelAdvanced(Plugin.Buff2);

        public DamageType damageType = DamageType.Carred;
        // Token: 0x0600007E RID: 126 RVA: 0x000044A8 File Offset: 0x000026A8
        public void Update()
        {
            if (Utils.IsGameRunning() && Prop0 != null && !Prop0.beforeDying)
            {
                if (Lawnf.TravelAdvanced(Plugin.Buff0))
                {
                    if (BurnLevel > 54)
                    {
                        BurnLevel = 54;
                    }
                }
                else if (BurnLevel > 18)
                {
                    BurnLevel = 18;
                }
                BurnTimer -= Time.deltaTime;
                if (BurnTimer <= 0f)
                {
                    // Visual effect
                    CreateParticle.SetParticle(33, Prop0.axis.position, Prop0.theZombieRow, true);

                    // Determine burn radius based on TravelAdvanced
                    float radius = Lawnf.TravelAdvanced(Plugin.Buff0) ? Radius*2 : Radius;

                    // Get all colliders in radius
                    Collider2D[] hits = Physics2D.OverlapCircleAll(
                        Prop0.col.bounds.center,
                        radius,
                        LayerMask.GetMask("Zombie") // adjust if needed
                    );

                    foreach (Collider2D hit in hits)
                    {
                        Zombie zombie = hit.GetComponent<Zombie>();
                        if (zombie == null)
                            continue;

                        if (zombie.isMindControlled || Prop0.beforeDying)
                            continue;
                        int dmg = BurnLevel * 100;
                        if(UltimateFlameGatling_Remade.IsRogue) dmg*=100;
                        // Apply burn damage
                        zombie.TakeDamage(
                            damageType,
                            dmg,
                            Plugin.UFlameGatling,
                            false
                        );

                        // Reapply burn component (spread)
                        if(ZhiBian)zombie.AddBurn();
                    }
                    if(Lawnf.TravelAdvanced(Plugin.Buff2))BurnTimer = 0.25f;
                    else BurnTimer = 1f;
                }
            }
        }
        public static void AddBurn(Zombie self, int level = 1)
        {
            if (ReferenceEquals(self, null)) 
                return;

            if (self.IsDestroyed()) 
                return;

            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            self.GetOrAddComponent<ZombieBurn_2>().BurnLevel += level;
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
    public static class ExtensionMgr
    {
        public static void AddBurn(this Zombie self, int level = 1) => ZombieBurn_2.AddBurn(self,level);
    }//*/
}