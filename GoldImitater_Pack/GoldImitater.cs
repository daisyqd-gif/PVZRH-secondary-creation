namespace GoldImitater
{
    public class GoldImitater : MonoBehaviour
    {
        public static BuffID buff = -1;
        public static int PlantID = 1931;

        public void Start()
        {
            int total = 0;
            var config = GameAPP.config;
            if (config.levelZombieInRandom) total += 2;
            if (config.strongUltiZombieInRandom) total += 2;
            if (config.leaderInRandom) total += 6;
            if (GameAPP.theGameStatus == GameStatus.InGame && plant != null && UnityEngine.Random.Range(1, 101) <= total && (plant.board.boardTag.isSuperRandom || plant.board.boardTag.isIZ))
            {
                plant.StarUp();
                plant.starUp = true;
                plant.UpdateStarIcon();
            }
        }

        public void AnimSpawn()
        {
            ParticleManager.Instance.SetParticle(ParticleType.RandomCloud, plant.axis.transform.position, plant.thePlantRow, lim: true);
            int row = plant.thePlantRow;
            int column = plant.thePlantColumn;
            var axis = plant.axis.transform.position;
            plant.Die();
            var v = UnityEngine.Random.Range(1, 101);
            if (!Lawnf.TravelAdvanced(buff) && !plant.starUp)
            {
                if (v <= 60)
                {
                    RandomPlant(v, 40, 20, column, row);
                }
                else if (v <= 95)
                {
                    RandomZombie(v, 80, 10, 5, axis, row);
                }
                else
                {
                    v = UnityEngine.Random.Range(1, 4);
                    TriggerEvent(v, axis);
                }
            }
            else
            {
                if (v <= 45)
                {
                    RandomPlant(v, 10, 35, column, row);
                }
                else if (v <= 95)
                {
                    RandomZombie(v, 50, 30, 15, axis, row);
                }
                else
                {
                    v = UnityEngine.Random.Range(1, 4);
                    TriggerEvent(v, axis);
                }
            }
        }

        public void SetRandomPlant(PlantType plantType, int column, int row)
        {
            if (plant.board.GetBoxType(column, row) == BoxType.Water)
                CreatePlant.Instance.SetPlant(column, row, PlantType.LilyPad, isFreeSet: true);
            if (plant.board.GetBoxType(column, row) == BoxType.Roof)
                CreatePlant.Instance.SetPlant(column, row, PlantType.Pot, isFreeSet: true);
            TryStarUpPlant(CreatePlant.Instance.SetPlant(column, row, plantType, isFreeSet: true).GetComponent<Plant>());
            if (TypeMgr.IsPuff(plantType))
            {
                TryStarUpPlant(CreatePlant.Instance.SetPlant(column, row, plantType, isFreeSet: true).GetComponent<Plant>());
                TryStarUpPlant(CreatePlant.Instance.SetPlant(column, row, plantType, isFreeSet: true).GetComponent<Plant>());
            }
        }

        public void TryStarUpPlant(Plant plant)
        {
            if (!this.plant.starUp) return;
            plant.StarUp();
            plant.starUp = true;
            plant.UpdateStarIcon();
        }

        public void SetRandomZombie(ZombieType zombieType, float x, int row)
        {
            int nR = row;
            if (zombieType == ZombieType.ZombieBoss || zombieType == ZombieType.ZombieBoss2)
                nR = 0;
            if (zombieType == ZombieType.ZombieBoss)
                GameAPP.Instance.PlayMusic(MusicType.Boss);
            if (zombieType == ZombieType.ZombieBoss2)
                GameAPP.Instance.PlayMusic(MusicType.Boss2);
            var tmp = Board.Instance.isEveStarted;
            Board.Instance.isEveStarted = true;
            try
            {
                var zombie = CreateZombie.Instance.SetZombie(nR, zombieType, x).GetComponent<Zombie>();
                Board.Instance.isEveStarted = tmp;
                if (zombie == null) return;
                float timer = 1f;
                switch (UnityEngine.Random.Range(0, 3))
                {
                    case 0:
                        timer = 0.8f;
                        break;
                    case 1:
                        timer = 1.2f;
                        break;
                }
                int bossMulti = 100;
                bool flag = zombieType == ZombieType.ZombieBoss || zombieType == ZombieType.ZombieBoss2;
                if (flag)
                    if (plant.board.boardTag.isSuperRandom)
                        bossMulti = 10;
                if (plant.starUp) bossMulti *= 10;
                if (flag)
                {
                    zombie.theHealth *= bossMulti;
                    zombie.theMaxHealth *= bossMulti;
                }
                if (zombieType == ZombieType.HorseBoss)
                {
                    zombie.theHealth *= 15;
                    zombie.theMaxHealth *= 15;
                }
                zombie.theHealth = (int)(zombie.theHealth * timer);
                zombie.theMaxHealth = (int)(zombie.theMaxHealth * timer);
                zombie.theFirstArmorHealth = (int)(zombie.theFirstArmorHealth * timer);
                zombie.theFirstArmorMaxHealth = (int)(zombie.theFirstArmorMaxHealth * timer);
                zombie.theSecondArmorHealth = (int)(zombie.theSecondArmorHealth * timer);
                zombie.theSecondArmorMaxHealth = (int)(zombie.theSecondArmorMaxHealth * timer);
            }
            catch (Exception)
            { }
        }

        public void RandomPlant(int v, int normal, int ulti, int column, int row)
        {
            var list = GetPlantsList();
            if (v <= normal)
            {
                list = list.Where(type => !Lawnf.IsUltiPlant(type)).ToList();
                SetRandomPlant(list[UnityEngine.Random.Range(0, list.Count)], column, row);
            }
            else if (v <= (normal + ulti))
            {
                list = list.Where(type => Lawnf.IsUltiPlant(type)).ToList();
                SetRandomPlant(list[UnityEngine.Random.Range(0, list.Count)], column, row);
            }
        }

        public void RandomZombie(int v, int normal, int ulti, int boss, Vector2 axis, int row)
        {
            var list = GetZombiesList();
            if (v <= normal)
            {
                list = list.Where(type => !TypeMgr.UltimateZombie(type) && !TypeMgr.IsBossZombie(type) && type != ZombieType.Nothing).ToList();
                SetRandomZombie(list[UnityEngine.Random.Range(0, list.Count)], axis.x, row);
            }
            else if (v <= (normal + ulti))
            {
                list = list.Where(type => TypeMgr.UltimateZombie(type) && !TypeMgr.IsBossZombie(type) && type != ZombieType.Nothing).ToList();
                SetRandomZombie(list[UnityEngine.Random.Range(0, list.Count)], axis.x, row);
            }
            else if (v <= (normal + ulti + boss))
            {
                list = list.Where(type => TypeMgr.IsBossZombie(type) && type != ZombieType.Nothing).ToList();
                SetRandomZombie(list[UnityEngine.Random.Range(0, list.Count)], axis.x, row);
            }
        }

        public void TriggerEvent(int v, Vector2 axis)
        {
            switch (v)
            {
                case 1:
                    {
                        for (int i = 1; i <= 10; i++)
                            Lawnf.SetDroppedCard(axis, UnityEngine.Random.Range(0, 2) == 0 ? (PlantType)PlantID : PlantType.DiamondImitater);
                        InGameText.Instance.ShowText("模仿十连抽！", 5f);
                    }
                    break;
                case 2:
                    {
                        var mgr = GameAPP.Instance.GetOrAddComponent<TravelMgr>();
                        int value = UnityEngine.Random.Range(1, 101);
                        var data = mgr.data;
                        if (value <= 90)
                        {
                            switch (UnityEngine.Random.Range(0, 2))
                            {
                                case 0:
                                    {
                                        var list = new List<AdvBuff>();
                                        foreach (var (id, _) in TravelDictionary.advancedBuffsText)
                                            if (!data.advBuffs.Contains(id))
                                                list.Add(id);
                                        var advBuff = list[UnityEngine.Random.Range(0, list.Count)];
                                        TravelMgr.Instance.GetNormalBuff(advBuff);
                                        InGameText.Instance.ShowText($"抽到普通词条：{TravelDictionary.advancedBuffsText[advBuff]}", 5f);
                                    }
                                    break;
                                case 1:
                                    {
                                        var list = new List<UltiBuff>();
                                        foreach (var (id, _) in TravelDictionary.ultimateBuffsText)
                                            if (!data.ultiBuffs.Contains(id) && !data.ultiBuffs.Contains(id))
                                                list.Add(id);
                                        var ultiBuff = list[UnityEngine.Random.Range(0, list.Count)];
                                        TravelMgr.Instance.GetUltiBuff(ultiBuff);
                                        InGameText.Instance.ShowText($"抽到强究词条：{TravelDictionary.ultimateBuffsText[ultiBuff]}", 5f);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            var list = new List<TravelDebuff>();
                            foreach (var kvp in TravelDictionary.debuffData)
                            {
                                if (!data.travelDebuffs.Contains(kvp.Key))
                                    list.Add(kvp.Key);
                            }
                            var debuff = list[UnityEngine.Random.Range(0, list.Count)];
                            TravelMgr.Instance.GetDebuff(debuff);
                            InGameText.Instance.ShowText($"抽到僵尸词条：{TravelDictionary.debuffData[debuff].Item1}", 5f);
                        }
                    }
                    break;
                case 3:
                    {
                        for (int i = 1; i <= 20; i++)
                            CreateItem.Instance.SetCoin(0, 0, 1, 0, axis);
                        InGameText.Instance.ShowText("大量阳光！", 5f);
                    }
                    break;
            }
        }

        public List<PlantType> GetPlantsList()
        {
            return GameAPP.resourcesManager.allPlants.ToArray().ToList().Where(x => x != PlantType.Nothing && x != PlantType.MagnetBox &&
                            x != PlantType.MagnetInterface && x != PlantType.Pit && x != PlantType.Refrash && x != PlantType.Extract_single &&
                            x != PlantType.Extract_ten && x != PlantType.VectorPlant).ToList();
        }

        public List<ZombieType> GetZombiesList()
        {
            return GameAPP.resourcesManager.allZombieTypes.ToArray().ToList().Where(x => x != ZombieType.Nothing && x != ZombieType.TrainingDummy && x != ZombieType.VoodooDollZombie &&
                        x != ZombieType.ProjectileZombie).ToList();
        }

        public Imitater plant => gameObject.GetComponent<Imitater>();
    }
}
