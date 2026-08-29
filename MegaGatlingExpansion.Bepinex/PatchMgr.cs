namespace MegaGatlingExpansion
{
    [HarmonyPatch(typeof(Plant))]
    public static class Plant_Patch
    {
        // -------------------------
        //  SearchZombie()
        // -------------------------
        [HarmonyPostfix]
        [HarmonyPatch("SearchZombie")]
        public static void SearchZombie_Postfix(Plant __instance, ref GameObject __result)
        {
            if (!__instance.TryGetComponent<ThreeMegaGatlingPea>(out var mg))
                return;

            __result = CustomSearchZombie(__instance);
        }

        private static GameObject CustomSearchZombie(Plant plant)
        {
            if (plant.board == null || plant.board.zombieArray == null)
                return null;

            int plantRow = plant.thePlantRow;
            float vision = plant.vision;
            Transform axis = plant.axis;

            foreach (Zombie z in plant.board.zombieArray)
            {
                if (z == null)
                    continue;

                int rowDiff = Mathf.Abs(z.theZombieRow - plantRow);
                if (rowDiff > 1)
                    continue;

                Transform zt = z.transform;
                if (zt == null)
                    continue;

                Vector3 pos = zt.position;

                if (pos.x > vision)
                    continue;

                if (axis != null && pos.x < axis.position.x)
                    continue;

                if (plant.SearchUniqueZombie(z))
                    return z.gameObject;
            }

            return null;
        }

        // -------------------------
        //  Die()
        // -------------------------
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Plant.Die))]
        public static bool Die_Prefix(Plant __instance, Plant.DieReason reason)
        {
            if (__instance.TryGetComponent<MegaGatlingPea>(out var p) && p.isPF)
            {
                if (reason == Plant.DieReason.BySteal)
                {
                    Plant g = CreatePlant.Instance.SetPlant(
                        __instance.thePlantColumn,
                        __instance.thePlantRow,
                        __instance.thePlantType,
                        null,
                        default,
                        true,
                        false,
                        null
                    );
                    g.thePlantHealth = __instance.thePlantHealth;
                    g.thePlantMaxHealth = __instance.thePlantMaxHealth;
                    g.thePlantSpeed = __instance.thePlantSpeed;

                    if (g.TryGetComponent<MegaGatlingPea>(out var c))
                    {
                        c.AttributeCount_Custom = p.AttributeCount_Custom;
                    }
                }
                else if (reason == Plant.DieReason.CrashInWater || reason == Plant.DieReason.Default || reason ==Plant.DieReason.ByFreeze)
                {
                    __instance.thePlantHealth = __instance.thePlantMaxHealth;
                    __instance.UpdateText();
                    return false;
                }
            }

            return true;
        }

        // -------------------------
        //  Crashed()
        // -------------------------
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Plant.Crashed))]
        public static bool Crashed_Prefix(Plant __instance)
        {
            if (__instance.TryGetComponent<MegaGatlingPea>(out var p) && p.isPF)
            {
                __instance.thePlantHealth = __instance.thePlantMaxHealth;
                __instance.UpdateText();
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Plant.TakeDamage))]
        public static bool TakeDamage_Prefix(Plant __instance, int damage, int damageType)
        {
            if (__instance.TryGetComponent<MegaGatlingPea>(out var p) && p.isPF)
            {
                __instance.thePlantHealth = __instance.thePlantMaxHealth;
                __instance.UpdateText();
                return false;
            }
            if (damageType == 1 && __instance.thePlantType == PlantTypeExpand.CherryMegaGatlingPea)
            {
                return false;
            }
            return true;
        }

        // -------------------------
        //  PlantShootUpdate()
        // -------------------------
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Plant.PlantShootUpdate))]
        public static bool PlantShootUpdate_Prefix(Plant __instance)
        {
            if (__instance.TryGetComponent<MegaGatlingPea>(out var p) && p.isPF)
                return false;

            return true;
        }

        // -------------------------
        //  UpdateAttackCountDown()
        // -------------------------
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Plant.UpdateAttackCountDown))]
        public static void UpdateAttackCountDown_Postfix(Plant __instance)
        {
            if (__instance.TryGetComponent<MegaGatlingPea>(out var _) && Lawnf.TravelUltimate(UltiBuff.EnumValue3) && __instance.thePlantType == PlantTypeExpand.CherryMegaGatlingPea)
                __instance.thePlantAttackCountDown -= Time.deltaTime;
            if (__instance.TryGetComponent<RedGatlingPea>(out var _))
                __instance.thePlantAttackCountDown -= Time.deltaTime;
        }

        // -------------------------
        //  StarUp()
        // -------------------------
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Plant.StarUp))]
        public static void StarUp_Postfix(Plant __instance)
        {
            if (!__instance.TryGetComponent<MegaGatlingPea>(out var _))
                return;

            __instance.starUp = true;
            __instance.UpdateStarIcon();
        }
    }
    [HarmonyPatch(typeof(FreezedPlant))]
    public static class FreezedPlant_Patch
    {
        [HarmonyPatch(nameof(FreezedPlant.InitFreezedPlant))]
        [HarmonyPrefix]
        public static bool InitFreezedPlant_Prefix(FreezedPlant __instance, PlantType thePlantType)
        {
            if (__instance == null) return true;
            if (PlantMgr.IsTypeIn1x1(__instance.theItemColumn, __instance.theItemRow, (Plant p) =>
            {
                if (p.TryGetComponent<MegaGatlingPea>(out var c) && c.isPF && c.plant.thePlantType == thePlantType)
                {
                    return true;
                }
                return false;
            }))
            {
                __instance.Die();
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(nameof(FreezedPlant.HitBullet))]
        public static bool HitBullet_Prefix(FreezedPlant __instance, Bullet bullet)
            => bullet.TryGetComponent<ElectricPea>(out _);
    }
    [HarmonyPatch(typeof(Money))]
    public static class Money_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Money.ReinforcePlant))]
        public static bool ReinforcePlant_Prefix(Money __instance, ref Plant plant)
        {
            if (plant.TryGetComponent<MegaGatlingPea>(out var component))
            {
                var cost = 1000;//实时计算大招花费

                if (Board.Instance.theMoney < cost)//如果钱不够
                {
                    InGameText.Instance.ShowText($"大招需要{cost}金币", 5);//提示
                    return false;//直接返回
                }
                if (component.isPF) return false;
                Vector2 pos = new Vector2(plant.axis.position.x, plant.axis.position.y + 0.75f);
                ParticleManager.Instance.SetParticle(ParticleType.SuperKillEffect, pos);
                GameAPP.PlaySound(0x42);

                component.StartPF();
                __instance.UsedEvent(plant.thePlantColumn, plant.thePlantRow, cost);
                __instance.OtherSuperSkill(plant);
                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(Bullet))]
    public static class Bullet_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Bullet.Rebound))]
        [HarmonyPriority(Priority.First)]
        public static bool Rebound_Prefix(Bullet __instance, Plant plant)
        {
            if (__instance == null || !__instance) return true;//let il2cpp handle this but it is probably a crash

            if (__instance.GetData<bool>("UnReboundable") || (__instance.theBulletType == BulletType.Bullet_ironPea && __instance.MoveWay == BulletMoveWay.Free)) return false;

            if (__instance.MoveWay == BulletMoveWay.Left || __instance.MoveWay == BulletMoveWay.Split_left) __instance.MoveWay = BulletMoveWay.MoveRight;
            else if (__instance.MoveWay == BulletMoveWay.Free)
            {
                float angle = __instance.transform.eulerAngles.z;
                float flipped = 180f - angle;
                __instance.transform.rotation = Quaternion.Euler(0, 0, flipped);
            }
            else if (__instance.MoveWay == BulletMoveWay.MoveRight) __instance.MoveWay = BulletMoveWay.Left;
            __instance.ToIDamageMaker().Team=Team.Player;
            //__instance.zombieLayer=LayerMaskMgr.BulletLayer;
            int bt = (int)__instance.theBulletType;
            if (plant == null || !plant) return false;

            if (bt == 3 || bt == 0xC2)
            {
                __instance.Damage = 1000;
                plant.TakeDamage(200, 0);
                plant.FlashOnce();
            }
            else if (bt == 0x65)
            {
                plant.TakeDamage(50, 0);
                plant.FlashOnce();
            }
            else if (bt == 0x7E)
            {
                plant.TakeDamage(600, 0);
                plant.FlashOnce();
            }

            if (plant.PotType == PlantType.UmbrellaPot)
            {
                __instance.Damage *= 2;
            }

            if (plant.TryGetComponent<MelonUmbrella>(out var u))
            {
                if (!u.blocking)
                {
                    u.anim.SetTriggerString("block2");
                    u.blocking = true;
                }
                u.storgedDamage += __instance.Damage;
            }

            if (plant.TryGetComponent<RedEmeraldUmbrella>(out var r))
            {
                if (!r.blocking)
                {
                    r.anim.SetTriggerString("block2");
                    r.blocking = true;
                }
            }
            Bullet newBullet = __instance.board.boardAction.FirePeas(__instance, null, 1, __instance.theBulletType);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Bullet.HitLand))]
        public static bool HitLand_Prefix(Bullet __instance) => !__instance.TryGetComponent<ElectricPea>(out var _);
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Bullet.OnTriggerEnter2D))]
        public static bool OnTriggerEnter2D_Prefix(Bullet __instance) => !__instance.TryGetComponent<ElectricPea>(out var _);
    }

    [HarmonyPatch(typeof(Bullet_pea))]
    public static class Bullet_Pea_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static bool HitZombie_Prefix(Bullet_pea __instance, Zombie zombie)
        {
            if (zombie == null || __instance == null)
                return true;

            int profile = (int)__instance.theBulletType;

            switch (profile)
            {
                case 3082:
                    if (__instance.TryGetComponent<ExtremeFirePea>(out var fire))
                        fire.HitZombie(zombie);
                    return true;

                case 3083:
                    if (__instance.TryGetComponent<PrimalPea>(out var primal))
                        primal.HitZombie(zombie);
                    return true;

                case 3084:
                    if (__instance.TryGetComponent<GooPea>(out var goo))
                        goo.HitZombie(zombie);
                    return true;

                case 3086:
                    zombie.SetData("HypnoMarker", true);
                    zombie.SetData("fromHypnoZombie", false);
                    return true;

                case 3087:
                    zombie.SetData("HypnoMarker", true);
                    zombie.SetData("fromHypnoZombie", true);
                    return true;
                case 3088:
                    if (__instance.TryGetComponent<FlameGooPea>(out var flameGooPea))
                        flameGooPea.HitZombie(zombie);
                    return true;

                case 3085:
                    if (__instance.TryGetComponent<ElectricPea>(out var elec))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("HitPlant")]
        public static bool HitPlant_Prefix(Bullet_pea __instance, Plant plant)
        {
            if (plant == null || __instance == null)
                return true;

            int profile = (int)__instance.theBulletType;

            switch (profile)
            {
                case 3082:
                    if (__instance.TryGetComponent<ExtremeFirePea>(out var fire))
                        fire.HitPlant(plant);
                    return true;

                case 3083:
                    if (__instance.TryGetComponent<PrimalPea>(out var primal))
                        primal.HitPlant(plant);
                    return true;

                case 3084:
                    if (__instance.TryGetComponent<GooPea>(out var goo))
                        goo.HitPlant(plant);
                    return true;

                case 3086:
                case 3087:
                    if (__instance.TryGetComponent<HypnoMegaPea>(out var hypnoMegaPea))
                        hypnoMegaPea.HitPlant(plant);
                    return true;
                case 3085:
                    return false;
                case 3088:
                    if (__instance.TryGetComponent<FlameGooPea>(out var flameGooPea))
                        flameGooPea.HitPlant(plant);
                    return true;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Lawnf))]
    public static class Lawnf_Patch
    {
        [HarmonyPatch(nameof(Lawnf.GetPlantCount), new Type[] { typeof(PlantType), typeof(Board) })]
        [HarmonyPostfix]
        public static void GetCount_Postfix(PlantType theSeedType, Board board, ref int __result)
        {
            if (theSeedType == PlantType.UltimateGatling)
                __result += Lawnf.GetPlantCount(PlantTypeExpand.CherryMegaGatlingPea, board);
        }
        [HarmonyPatch(nameof(Lawnf.IsSuperPlant))]
        [HarmonyPostfix]
        public static void IsSuperPlant_Postfix(PlantType thePlantType, ref bool __result)
        {
            if (Plugin.CustomSuperPlants.Contains(thePlantType))
            {
                __result=true;
            }
        }
    }
    [HarmonyPatch(typeof(Zombie))]
    public static class Zombie_Patch
    {
        [HarmonyPatch(nameof(Zombie.Die))]
        [HarmonyPrefix]
        public static void Die_Prefix(Zombie __instance)
        {
            if (__instance.GetData<bool>("HypnoMarker") && !__instance.GetData<bool>("Converted"))
            {
                __instance.SetData("Converted", true);
                float x = __instance.axis.position.x;
                if (__instance.GetData<bool>("fromHypnoZombie")) CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, (ZombieType)9000, x);
                else CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, (ZombieType)9001, x);
            }
        }
        [HarmonyPatch(nameof(Zombie.SetMindControl))]
        [HarmonyPrefix]
        public static bool SetMindControl_Prefix(Zombie __instance)
        {
            if (__instance.theZombieType == (ZombieType)9002 || __instance.theZombieType == (ZombieType)9004 || __instance.theZombieType == (ZombieType)9006) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.Buttered))]
        [HarmonyPrefix]
        public static bool Buttered_Prefix(Zombie __instance)
        {
            if (__instance.theZombieType == (ZombieType)9002 || __instance.theZombieType == (ZombieType)9004 || __instance.theZombieType == (ZombieType)9006) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.KnockBack))]
        [HarmonyPrefix]
        public static bool KnockBack_Prefix(Zombie __instance)
        {
            if (__instance.theZombieType == (ZombieType)9002 || __instance.theZombieType == (ZombieType)9004 || __instance.theZombieType == (ZombieType)9006) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.SetFreeze))]
        [HarmonyPrefix]
        public static bool SetFreeze_Prefix(Zombie __instance)
        {
            if (__instance.theZombieType == (ZombieType)9002 || __instance.theZombieType == (ZombieType)9004 || __instance.theZombieType == (ZombieType)9006) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.SetJalaed))]
        [HarmonyPrefix]
        public static bool SetJalaed_Prefix(Zombie __instance)
        {
            if (__instance.theZombieType == (ZombieType)9002 || __instance.theZombieType == (ZombieType)9004 || __instance.theZombieType == (ZombieType)9006) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.SetCold))]
        [HarmonyPrefix]
        public static bool SetCold_Prefix(Zombie __instance)
        {
            if (__instance.theZombieType == (ZombieType)9002 || __instance.theZombieType == (ZombieType)9004 || __instance.theZombieType == (ZombieType)9006) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.SetPoison))]
        [HarmonyPrefix]
        public static bool SetPoison_Prefix(Zombie __instance)
        {
            if (__instance.theZombieType == (ZombieType)9002 || __instance.theZombieType == (ZombieType)9004 || __instance.theZombieType == (ZombieType)9006) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.TakeDamage))]
        [HarmonyPrefix]
        public static bool TakeDamage_Prefix(Zombie __instance, DmgType theDamageType, int theDamage, PlantType reportType, bool fix)
        {
            if ((__instance.theZombieType == (ZombieType)9004 || __instance.theZombieType == (ZombieType)9006) && (theDamageType == DmgType.Explode || theDamageType == DmgType.Carred)) return false;
            return true;
        }
        [HarmonyPatch(nameof(Zombie.BodyTakeDamage))]
        [HarmonyPrefix]
        public static bool BodyTakeDamage_Prefix(Zombie __instance, ref int theDamage)
        {
            //if (__instance.theZombieType == (ZombieType)9004 || __instance.theZombieType == (ZombieType)9006)
            //{
            //    theDamage = Math.Min(theDamage, 1000);
            //    return true; //I tested the damage and it does work for 9004
            //}
            // Only affect your custom boss zombie
            if (__instance.theZombieType != (ZombieType)9002 && __instance.theZombieType != (ZombieType)9001 && __instance.theZombieType != (ZombieType)9000 && __instance.theZombieType != (ZombieType)9003)
                return true; // run vanilla for all other zombies

            // --- 1. Prevent arm loss entirely ---
            // Setting loseHand = true blocks the arm-loss branch in vanilla code
            __instance.loseHand = true;
            int damageamt;
            if (__instance.theZombieType != (ZombieType)9002) damageamt = theDamage;
            else damageamt = Math.Min(theDamage, 1000);

            // --- 2. Apply damage manually ---
            __instance.theHealth -= damageamt;

            // --- 3. Trigger head loss ONLY at <= 270 HP ---
            if (__instance.theHealth <= 100 && __instance.beforeDying == false)
            {
                __instance.beforeDying = true;

                // Play the real head-loss event
                __instance.LoseHeadEvent();

                // Remove butter if needed
                __instance.UnButtered();

                // Destroy the head object
                __instance.FindAndDestoryZombieHead(__instance.gameObject);
            }

            // --- 4. Skip vanilla BodyTakeDamage ---
            // We handled everything ourselves
            return false;
        }
    }
    [HarmonyPatch(typeof(CreateZombie))]
    public static class CreateZombie_Patch
    {
        [HarmonyPatch(nameof(CreateZombie.SetZombie))]
        [HarmonyPrefix]
        public static bool SetZombie_Prefix(ref int theRow, ref ZombieType theZombieType, ref float theX)
        {
            if (theZombieType == ZombieType.FlagZombie && Plugin.Buff1 != -1 && Lawnf.TravelDebuff((TravelDebuff)Plugin.Buff1))
            {
                CreateZombie.Instance.SetZombie(Random.Range(0, Board.Instance.rowNum), (ZombieType)9002, theX);
                for (int i = 0; i < Board.Instance.rowNum; i++)
                {
                    CreateZombie.Instance.SetZombie(i, (ZombieType)9000);
                }
            }
            if (Random.Range(0, 100) <= 50 && (theZombieType == ZombieType.UltimatePaperZombie) && GameAPP.theGameStatus == GameStatus.InGame)
            {
                theZombieType = (ZombieType)9004;
            }
            return true;
        }
        [HarmonyPatch(nameof(CreateZombie.SetZombieWithMindControl))]
        [HarmonyPrefix]
        public static bool SetZombieWithMindControl_Prefix(ref int theRow, ref ZombieType theZombieType, ref float theX, ref bool withEffect)
        {
            if (theZombieType == ZombieType.FlagZombie && Plugin.Buff1 != -1 && Lawnf.TravelDebuff((TravelDebuff)Plugin.Buff1))
            {
                CreateZombie.Instance.SetZombieWithMindControl(Random.Range(0, Board.Instance.rowNum), (ZombieType)9002, theX);
                for (int i = 0; i < Board.Instance.rowNum; i++)
                {
                    CreateZombie.Instance.SetZombieWithMindControl(i, (ZombieType)9000, Mouse.Instance.GetBoxXFromColumn(-1));
                }
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(UltimateFootballZombie))]
    public static class UltimateFootballZombie_Patch
    {
        [HarmonyPatch(nameof(UltimateFootballZombie.AttackEffect))]
        [HarmonyPrefix]
        public static bool AttackEffect_Prefix(Plant plant)
        {
            if (plant.TryGetComponent<MegaGatlingPea>(out var p) && p.isPF)
            {
                plant.thePlantHealth = plant.thePlantMaxHealth;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(TypeMgr))]
    public static class TypeMgr_Patch
    {
        [HarmonyPatch(nameof(TypeMgr.IsBossZombie))]
        [HarmonyPrefix]
        public static bool IsBossZombie_Prefix(ZombieType theZombieType, ref bool __result)
        {
            if (theZombieType == (ZombieType)9002 || theZombieType == (ZombieType)9004 || theZombieType == (ZombieType)9006)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(UltimateHypnoDoom))]
    public static class UltimateHypnoDoom_Patch
    {
        [HarmonyPatch(nameof(UltimateHypnoDoom.AttributeEvent))]
        [HarmonyPrefix]
        public static bool AttributeEvent_Prefix(UltimateHypnoDoom __instance)
        {
            if (__instance == null) return true;
            if (Random.Range(0, 6) < 1)
            {
                CreateZombie.Instance.SetZombieWithMindControl(__instance.thePlantRow, (ZombieType)9002, Mouse.Instance.GetBoxXFromColumn(__instance.thePlantColumn));
                Doom.SetDoom(__instance.board, __instance.transform.position, DoomType.IceDoom_big);
                __instance.Die(Plant.DieReason.BySelf);
                return false;
            }
            if (Random.Range(0, 6) < 2)
            {
                CreateZombie.Instance.SetZombieWithMindControl(__instance.thePlantRow, (ZombieType)9004, Mouse.Instance.GetBoxXFromColumn(__instance.thePlantColumn));
                Doom.SetDoom(__instance.board, __instance.transform.position, DoomType.IceDoom_big);
                __instance.Die(Plant.DieReason.BySelf);
                return false;
            }
            if (Random.Range(0, 6) < 3)
            {
                CreateZombie.Instance.SetZombieWithMindControl(__instance.thePlantRow, (ZombieType)9006, Mouse.Instance.GetBoxXFromColumn(__instance.thePlantColumn));
                Doom.SetDoom(__instance.board, __instance.transform.position, DoomType.IceDoom_big);
                __instance.Die(Plant.DieReason.BySelf);
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(GarlicTorch), nameof(GarlicTorch.OnTriggerEnter2D))]
    public static class GarlicTorch_OnTriggerEnter2D_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(GarlicTorch __instance, ref Collider2D collision)
        {
            var board = __instance.board;
            if (board == null)
                return true;

            if (collision == null)
                return false; // skip original

            // Try to get Bullet component
            if (!collision.TryGetComponent<Bullet>(out var bullet) || bullet == null)
                return false;

            if (__instance == null || !__instance) return true;

            if (bullet.theBulletType != BulletProfile.GooPea) return true;

            __instance.board.boardAction.FirePeas(bullet, __instance, 2, BulletProfile.FlameGooPea);

            return false; // run vanilla freeze
        }
    }
    [HarmonyPatch(typeof(UltimateDoomScared.__c))]
    public static class UltimateDoomScared___c_Patch
    {
        [HarmonyPatch("_Shoot1_b__3_0")]
        [HarmonyPostfix]
        public static void _Shoot1_b__3_0_Postfix(Plant p, ref bool __result)
        {
            if ((int)p.thePlantType == PlantTypeExpand.ExplodeGatlingBlover)
            {
                __result = true;
            }
        }
    }
    [HarmonyPatch(typeof(UltimateGatlingBlover))]
    public static class UltimateGatlingBlover_Patch
    {
        [HarmonyPatch(nameof(UltimateGatlingBlover.AttributeEvent))]
        [HarmonyPrefix]
        public static bool AttributeEvent_Prefix(UltimateGatlingBlover __instance)
        {
            if (__instance != null && __instance.TryGetComponent<UltimateExplodeGatlingBlover>(out var component))
            {
                component.AnimShoot_Custom();
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(UltimateGatlingBlover.DieEventMustExecute))]
        [HarmonyPrefix]
        public static bool DieEvent_Patch(UltimateGatlingBlover __instance, ref Plant.DieReason reason)
        {
            if (__instance != null && reason == Plant.DieReason.ByShovel)
            {
                if (__instance.thePlantType == PlantTypeExpand.ExplodeGatlingBlover)
                {
                    Lawnf.SetDroppedCard(__instance.shoot.position, PlantTypeExpand.CherryMegaGatlingPea);
                    return false;
                }
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(ZombieBall))]
    public static class ZombieBallIgnoreElectricPea_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ZombieBall.ZombieBallEffect))]
        public static bool ZombieBallEffect_Prefix(Collider2D collision) 
            => collision != null && !collision.TryGetComponent<ElectricPea>(out _);
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ZombieBall.OnTriggerEnter2D))]
        public static bool OnTriggerEnter2D_Prefix(Collider2D collision)
            => collision != null && !collision.TryGetComponent<ElectricPea>(out _);
    }
}
