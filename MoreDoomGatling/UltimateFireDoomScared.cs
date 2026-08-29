namespace MoreDoomGatling
{
    public class UltimateFireDoomScared : MonoBehaviour
    {
        public static ID PLANT_ID=3151;
        public UltimateDoomScared plant => gameObject.GetComponent<UltimateDoomScared>();
        public int counter=0;
        public bool isPF=false;
        public void Start()
        {
            plant.shoot=gameObject.transform.FindChild("Shoot");
            if(plant.board!=null && GameAPP.theGameStatus == GameStatus.InGame)
            {
                for(int i=0; i<plant.board.rowNum; i++)
                {
                    plant.board.boardAction.CreateFireLine(i);
                }
            }
        }
        public BulletType GetBulletType()
        {
            if(counter>=96 || (counter>=48 && Lawnf.TravelAdvanced(AdvBuff.EnumValue3)) || Random.Range(0, 100)<5)
            {
                if (!Lawnf.TravelAdvanced(AdvBuff.EnumValue2))
                {
                    plant.thePlantAttackCountDown = 3f;
                }
                counter=0;
                return Bullet_fireDoom.BULLET_ID_BIG;
            }
            else
            {
                counter++;
                return Bullet_fireDoom.BULLET_ID;
            }
        }
        public void Update()
        {
            if(plant==null || plant.board==null) return;
            if (plant.coordination > 0.0)
            {
                var fvar2=Time.deltaTime;
                plant.coordination=plant.coordination - fvar2*0.3f;
            }
        }
        public virtual IEnumerator SuperShoot_Custom()
        {
            if (plant.shoot == null)
            {
                yield break;
            }
            isPF = true;
            plant.invincible = true;
            plant.uncrashable = true;
            plant.flashCountDown = 5f;
            plant.isFlashing = true;
            plant.anim.SetBoolString("shooting",true);

            for (int i = 0; i < 500; i++)
            {
                Vector3 pos = plant.shoot.position;
                Bullet b=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y, plant.thePlantRow,
                    GetBulletType(), BulletMoveWay.Right_free
                );
                switch (b.theBulletType)
                {
                    case (BulletType)3151: b.Damage=plant.attackDamage*6; b.theStatus = BulletStatus.Doom_big; break;
                    default: b.Damage=plant.attackDamage; break;
                }
                b.transform.Rotate(0,0,Random.Range(-15f,15f));
                Bullet b1=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y+0.3f, plant.thePlantRow,
                    GetBulletType(), BulletMoveWay.Right_free
                );
                switch (b1.theBulletType)
                {
                    case (BulletType)3151: b1.Damage=plant.attackDamage*6; b1.theStatus = BulletStatus.Doom_big; break;
                    default: b1.Damage=plant.attackDamage; break;
                }
                b1.transform.Rotate(0,0,Random.Range(-15f,15f));
                Bullet b2=CreateBullet.Instance.SetBullet(
                    pos.x, pos.y-0.3f, plant.thePlantRow,
                    GetBulletType(), BulletMoveWay.Right_free
                );
                switch (b2.theBulletType)
                {
                    case (BulletType)3151: b2.Damage=plant.attackDamage*6; b2.theStatus = BulletStatus.Doom_big; break;
                    default: b2.Damage=plant.attackDamage; break;
                }
                b2.transform.Rotate(0,0,Random.Range(-15f,15f));

                if (Time.timeScale > 0f)
                {
                    yield return new WaitForFixedUpdate();
                }
                else
                    yield return null;
            }
            plant.invincible = false;
            plant.uncrashable = false;
            plant.isFlashing = false;
            isPF = false;
            plant.anim.SetBoolString("shooting",false);
        }
        public Bullet AnimShoot_Custom()
        {
            // 1. Internal timer logic (field at +0x114)
            plant.thePlantAttackInterval=Math.Max(0.2f,plant.thePlantAttackInterval-0.1f);

            // 2. TravelAdvanced(3006) gate
            if (Lawnf.TravelAdvanced(AdvBuff.EnumValue3006))
            {
                int col = plant.thePlantColumn;
                int row = plant.thePlantRow;

                var plants = Lawnf.Get1x1Plants(col, row);

                Func<Plant, bool> sysPredicate = p =>
                {
                    if (p == null)
                        return false;

                    try
                    {
                        // Call the patched managed predicate, NOT the IL2CPP delegate
                        return UltimateDoomScared.__c.__9._Shoot1_b__3_0(p);
                    }
                    catch
                    {
                        // Fallback to original behavior (exact IL2CPP logic)
                        return p.thePlantType == PlantType.UltimateGatlingBlover;
                    }
                };

                // Convert IL2CPP list → System list
                var sysPlants = plants.ToSystemList();

                // Use System LINQ (compatible with other mods)
                Plant target = sysPlants.FirstOrDefault(sysPredicate);

                if (target != null)
                {
                    int i = 0;

                    if (plant.coordination > 0f)
                    {
                        while (i < plant.coordination)
                        {
                            // random offset
                            float rx = Random.Range(-1f, 1f);
                            float ry = Random.Range(-1f, 1f);
                            Vector2 offset = new Vector2(rx, ry);

                            // this calls the doom logic (normal vs big, etc.)
                            Bullet b = SetBullet(offset);

                            if (i > 9)
                            {
                                if (b == null)
                                    return null;

                                // reduce damage for extra shots beyond 5
                                float delta = (plant.coordination - 5f) * plant.attackDamage * -0.3f;
                                b.Damage = b.Damage - (int)delta;
                                break;
                            }

                            if (b == null)
                                return null;

                            // first 5 shots use base damage
                            b.Damage = plant.attackDamage;

                            i++;
                        }
                    }

                    // coordination grows slightly each time
                    plant.coordination += 0.1f;
                }
            }

            // 4. Play sound (RandomRangeInt(3,5) → Range(3,6))
            int soundId = Random.Range(3, 6);
            GameAPP.PlaySound(soundId, 0.5f, 1f);

            // 5. Final bullet with zero offset
            Bullet finalBullet = SetBullet(Vector2.zero);
            return finalBullet;
        }
        public Bullet SetBullet(Vector2 offset)
        {
            // Must have a shoot transform
            if (plant.shoot == null)
                return null;

            // Position = shoot position + offset
            Vector3 pos = plant.shoot.position;
            float x = pos.x + offset.x;
            float y = pos.y + offset.y;

            Bullet b=CreateBullet.Instance.SetBullet(
                x, y, plant.thePlantRow,
                GetBulletType(), BulletMoveWay.MoveRight
            );
            switch (b.theBulletType)
            {
                case (BulletType)3151: b.Damage=plant.attackDamage*6; b.theStatus = BulletStatus.Doom_big; break;
                default: b.Damage=plant.attackDamage; break;
            }
            Bullet b1=CreateBullet.Instance.SetBullet(
                x, y, plant.thePlantRow,
                GetBulletType(), BulletMoveWay.Right_free
            );
            switch (b1.theBulletType)
            {
                case (BulletType)3151: b1.Damage=plant.attackDamage*6; b1.theStatus = BulletStatus.Doom_big; break;
                default: b1.Damage=plant.attackDamage; break;
            }
            b1.transform.Rotate(0,0,15f);
            Bullet b2=CreateBullet.Instance.SetBullet(
                x, y, plant.thePlantRow,
                GetBulletType(), BulletMoveWay.Right_free
            );
            switch (b2.theBulletType)
            {
                case (BulletType)3151: b2.Damage=plant.attackDamage*6; b2.theStatus = BulletStatus.Doom_big; break;
                default: b2.Damage=plant.attackDamage; break;
            }
            b2.transform.Rotate(0,0,-15f);
            if(Random.Range(0,100)<5 && !isPF) plant.StartCoroutine(SuperShoot_Custom());
            return b;
        }
        public void OnDestroy()
        {
            if(plant.board!=null && GameAPP.theGameStatus == GameStatus.InGame)
            {
                for(int i=0; i<plant.board.rowNum; i++)
                {
                    plant.board.boardAction.CreateFireLine(i);
                }
            }
        }
    }
    [HarmonyPatch(typeof(UltimateGatlingBlover.__c),nameof(UltimateGatlingBlover.__c._AttributeEvent_b__4_0))]
    public static class UltimateGatlingBlover___c__AttributeEvent_b__4_0_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Plant p, ref bool __result)
        {
            if (p == null)
                return true;

            if (p.thePlantType == UltimateFireDoomScared.PLANT_ID)
            {
                __result=true;
                return false;
            }
            
            return true;
        }
    }
}