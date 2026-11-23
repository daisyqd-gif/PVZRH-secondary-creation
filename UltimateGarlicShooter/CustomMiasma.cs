namespace UltimateGarlicShooter
{
    [HarmonyPatch(typeof(Miasma))]
    public class Miasma_Patch
    {
        [HarmonyPatch(nameof(Miasma.Update))]
        [HarmonyPrefix]
        public static bool Update_Prefix(ref Miasma __instance)
        {
            if (__instance != null && Board.Instance != null && GameAPP.theGameStatus == GameStatus.InGame && __instance.GetData<bool>("IsCherry"))
            {
                Transform transform = __instance.transform;
                ParticleSystem particleSystem;
                if (transform == null)
                {
                    particleSystem = null;
                }
                else
                {
                    Transform transform2 = transform.Find("Fume_fire").transform;
                    particleSystem = (transform2 != null) ? transform2.GetComponent<ParticleSystem>() : null;
                }
                particleSystem.main.startColor = new Color(0.8f, 0f, 0f, 1f);
                float deltaTime = Time.deltaTime;
                __instance.timer -= deltaTime;
                if (__instance.timer <= 0f)
                {
                    __instance.timer = __instance.maxTimer;
                    int num = 0;
                    Board instance = Board.Instance;
                    var plantsByRow = Lawnf.GetPlantsByRow(instance, __instance.theRow);
                    if (plantsByRow != null)
                    {
                        foreach (Plant plant in plantsByRow)
                        {
                            if (!(plant == null))
                            {
                                PlantType thePlantType = plant.thePlantType;
                                if (thePlantType != PlantType.GarlicThreePeater)
                                {
                                    if (thePlantType != PlantType.GarlicPea)
                                    {
                                        if (thePlantType == PlantType.GarlicRepeater)
                                        {
                                            num += 2;
                                        }
                                        else if (thePlantType != PlantType.GarlicSplit)
                                        {
                                            if (thePlantType != PlantType.GarlicGatling)
                                            {
                                                if (thePlantType == PlantType.GarlicSniper)
                                                {
                                                    num += 5;
                                                }
                                                else if (thePlantType == PlantId)
                                                {
                                                    num += 6;
                                                }
                                            }
                                            else
                                            {
                                                num += 4;
                                            }
                                        }
                                        else
                                        {
                                            num += 3;
                                        }
                                    }
                                    else
                                    {
                                        num++;
                                    }
                                }
                                else
                                {
                                    num += 3;
                                }
                            }
                        }
                    }
                    Transform transform3 = __instance.transform;
                    if (transform3 != null)
                    {
                        Vector3 position = transform3.position;
                        float range = __instance.range;
                        LayerMask zombieLayer = __instance.zombieLayer;
                        Collider2D[] array = Physics2D.OverlapCircleAll(new Vector2(position.x, position.y), range, zombieLayer);
                        if (array != null)
                        {
                            foreach (Collider2D collider2D in array)
                            {
                                Zombie zombie;
                                if (!(collider2D == null) && collider2D.TryGetComponent(out zombie))
                                {
                                    Board.Instance.boardAction.CreateCherryExplode(zombie.ColliderPosition, zombie.theZombieRow, CherryBombType.Bullet, 300, PlantType.Nothing, null, true);
                                    if (zombie != null)
                                    {
                                        zombie.SetPoison(10f);
                                    }
                                    if (zombie != null && zombie.effects.ContainsKey(EffectType.Poison))
                                    {
                                        if (zombie != null)
                                        {
                                            zombie.DamagedByPoison(__instance.dmgMultiplier * 3f);
                                        }
                                    }
                                    else if (zombie != null)
                                    {
                                        zombie.TakeDamage(900, null, DamageType.Shieldless, PlantId, false);
                                    }
                                    if (zombie != null)
                                    {
                                        zombie.TakeDamage(60 * num, null, DamageType.Shieldless, PlantId, false);
                                    }
                                    if (zombie != null && zombie.beforeDying)
                                    {
                                        Vector3 position2 = zombie.transform.position;
                                        Miasma.SetMiasma(new Vector2(position2.x, position2.y), zombie.theZombieRow, instance, true, __instance.big).SetData("IsCherry",true);
                                        if (zombie.theHealth > 0L)
                                        {
                                            zombie.theHealth = 0L;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Transform transform4 = __instance.transform;
                if (transform4 != null)
                {
                    Vector3 position3 = transform4.position;
                    float num2 = Time.deltaTime * 0.2f;
                    position3.x += __instance.randomVector.x * num2;
                    position3.y += __instance.randomVector.y * num2;
                    position3.z += __instance.randomVector.z * num2;
                    transform4.position = position3;
                    __instance.lifeTimer += Time.deltaTime;
                    if (__instance.lifeTimer > __instance.totalLifeTimer * 3f)
                    {
                        Object.Destroy(__instance.gameObject);
                    }
                }
                return false;
            }
            return true;
        }
    }
}