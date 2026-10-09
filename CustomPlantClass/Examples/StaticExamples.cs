using CustomPlantClass.Runtime.Tasks;
using Il2CppInterop.Runtime;

namespace CustomPlantClass.Examples
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class StaticExamples : MonoBehaviour
    {
        [OnLoad]
        public static void OnLoad()
        {
            PluginBehaviour.AddComponentToPlugin<StaticExamples>();
        }
        public static void UltimateGatling_StarShoot(BaseCustomPlant plant, BulletType theBulletType)
        {
            plant.StartCoroutine(StarShoot_UltimateGatling_Internal(plant, theBulletType));
        }
        private static IEnumerator StarShoot_UltimateGatling_Internal(BaseCustomPlant plant, BulletType theBulletType)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    PlantMgr.SetBullet(plant._plant, theBulletType, BulletMoveWay.SuperGatling, new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)), Random.Range(-15f, 15f)).normalSpeed = Random.Range(12f, 14f);
                }
                yield return new WaitForFixedUpdate();
            }
        }
        public static void UltimatePlantern_Shrink
        (
            Board board,
            Vector2 position,
            GameObject glow,
            float growTargetScale = 40f,
            float growStep = 0.4f,
            int frameDelayMs = 8,
            float previewShrinkFactor = 0.95f,
            float previewLerpFactor = 0.024f,
            float previewSpinRadians = 0.0001396263f,
            float fadeFactor = 0.95f,
            float minAlpha = 0.01f,
            int doomChancePerTile = 10
        )
        {
            glow = glow ?? new GameObject("Ulti_Glow",[Il2CppType.From(typeof(SpriteRenderer)),Il2CppType.From(typeof(SortingGroup))]);
            glow.GetComponent<SpriteRenderer>().sprite=Resources.Load<Sprite>("board/award/Glow");
            glow.GetComponent<SortingGroup>().sortingLayerName="particle11";
            glow.GetComponent<SortingGroup>();
            Shrink_Internal(
                board, glow, position,
                growTargetScale, growStep,
                frameDelayMs,
                previewShrinkFactor, previewLerpFactor, previewSpinRadians,
                fadeFactor, minAlpha, doomChancePerTile
            );
        }

        private static async void Shrink_Internal
        (
            Board board,
            GameObject glow,
            Vector2 position,
            float growTargetScale,
            float growStep,
            int frameDelayMs,
            float previewShrinkFactor,
            float previewLerpFactor,
            float previewSpinRadians,
            float fadeFactor,
            float minAlpha,
            int doomChancePerTile
        )
        {
            try
            {
                CancellationToken cancellationToken = board.CreateCancellationToken();
                SpriteRenderer glowRenderer = glow.GetComponent<SpriteRenderer>();
                SortingGroup sortingGroup = glow.GetComponent<SortingGroup>();
                var previews = new List<GameObject>();

                glow.transform.SetParent(board.transform);
                sortingGroup.enabled = true;
                GameAPP.PlaySound(SoundType.Portal);

                while (glow.transform.localScale.x > 0.1f)
                {
                    glow.transform.localScale -= Vector3.one * 0.04f;

                    List<Zombie> eligibleZombies = new List<Zombie>([..Lawnf.GetAllZombies()])
                        .Where(zombie => zombie != null
                            && !TypeMgr.IsBossZombie(zombie.theZombieType)
                            && !zombie.isMindControlled)
                        .ToList();

                    if (eligibleZombies.Count > 0)
                    {
                        Zombie zombie = eligibleZombies.GetRandomItem();
                        GameObject preview = CreateZombie.CreateZombiePreview(
                            zombie.theZombieType,
                            Color.white,
                            board.transform,
                            zombie.axis.position
                        );
                        zombie.Die(1);

                        if (preview != null)
                        {
                            previews.Add(preview);
                        }
                    }

                    for (int i = previews.Count - 1; i >= 0; i--)
                    {
                        GameObject preview = previews[i];
                        if (preview == null)
                        {
                            previews.RemoveAt(i);
                            continue;
                        }

                        Transform previewTransform = preview.transform;
                        Vector3 target = new(position.x, previewTransform.position.y, position.y);
                        previewTransform.position = Vector3.Lerp(
                            previewTransform.position,
                            target,
                            previewLerpFactor
                        );
                        previewTransform.localScale *= previewShrinkFactor;
                        previewTransform.Rotate(0f, 0f, previewSpinRadians);
                    }

                    await DelayTask.DelayScaled(frameDelayMs/1000,()=>Time.timeScale,cancellationToken);
                }

                foreach (GameObject preview in previews)
                {
                    if (preview != null)
                    {
                        Destroy(preview);
                    }
                }
                previews.Clear();

                sortingGroup.sortingLayerName = "up";
                Color glowColor = glowRenderer.color;
                glowColor.a = 1f;
                glowRenderer.color = glowColor;

                while (glow.transform.localScale.x < growTargetScale)
                {
                    glow.transform.localScale += Vector3.one * growStep;

                    foreach (Zombie zombie in Lawnf.GetAllZombies())
                    {
                        if (zombie != null)
                        {
                            zombie.Die(1);
                        }
                    }

                    GameAPP.PlaySound(SoundType.DoomShroom);
                    if (doomChancePerTile > 0)
                    {
                        for (int column = 0; column < board.columnNum; column++)
                        {
                            for (int row = 0; row < board.rowNum; row++)
                            {
                                if (Random.Range(0, doomChancePerTile) == 0)
                                {
                                    Vector2 tilePosition = Lawnf.GetPlantPosition(
                                        board,
                                        column,
                                        row,
                                        PlantType.Pot
                                    );
                                    Doom.SetDoom(board, tilePosition, DoomType.IceDoom_big);
                                }
                            }
                        }
                    }
                    await DelayTask.DelayScaled(frameDelayMs/1000,()=>Time.timeScale,cancellationToken);
                }

                glow.transform.SetParent(board.transform);
                while (glowRenderer.color.a > minAlpha)
                {
                    Color color = glowRenderer.color;
                    color.a *= fadeFactor;
                    glowRenderer.color = color;
                    await DelayTask.DelayScaled(frameDelayMs/1000,()=>Time.timeScale,cancellationToken);
                }

                Destroy(glow.gameObject);
            }
            catch (Exception exception)
            {
                ModLogger.LogError(exception.ToString());
            }
        }
        public static void KillAllZombies()
        {
            foreach( var i in Lawnf.GetAllZombies(Board.Instance).ToSystemList()) 
            {
                i.Die(1);
            }
        }
        public static void CreateRadiation(Board board, Vector2 pos, PlantType fromType = PlantType.NuclearDoomCherry, int damage = 3600)
        {
            GameObject gameObject = Instantiate(Resources.Load<GameObject>("plants/cherrybomb/nucleardoomcherry/Radiation"),pos,Quaternion.identity,board.transform);
            Radiation component = gameObject.GetComponent<Radiation> ();
            component.fromType = fromType;
            component.damage = damage;
        }
        public static (Crater,Doom) CreateNuclearCherry(Board board, int row, int col, int damage = 3600, bool enableRadiation = true, PlantType fromType = PlantType.NuclearDoomCherry,int bulletOffsetDegree = 10, bool isCrater = true)
        {
            Crater crater = board.boardAction.SetDoom (col, row, isCrater,false,default,0,0,null,false,fromType,0);
            Vector2 pos = new(Mouse.Instance.GetBoxXFromColumn(col),Mouse.Instance.GetBoxYFromRow(row));
            Doom doom = Doom.SetDoom (board, pos, DoomType.Nuclear, null, true);
            if(enableRadiation)
                CreateRadiation(board,pos,fromType,damage);
            for( int i = 0 ; i < 360 ; i += bulletOffsetDegree)
            {
                Bullet bullet = CreateBullet.Instance.SetBullet (pos.x, pos.y, row, BulletType.Bullet_nuclear, BulletMoveWay.Free, false);
                bullet.Damage = damage;
                bullet.transform.Rotate(0,0,i);
                bullet.normalSpeed = 10f;
                bullet.fromType = fromType;
            }
            return (crater,doom);
        }
    }

    public static class UltimateTorchBehaviour
    {
        public static Dictionary<BulletType, BulletType> FireTypes = new();
        public static void AddBulletToPool(BulletType fromType, BulletType toType) => FireTypes[fromType] = toType;
    }
    public static class SuperTorchBehaviour
    {
        public static Dictionary<BulletType, (BulletType, int)> FireTypes = new();
        public static void AddBulletToPool(BulletType fromType, BulletType toType, int DmgMultiplier) => FireTypes[fromType] = (toType, DmgMultiplier);
    }
}