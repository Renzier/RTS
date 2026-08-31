namespace Quantum
{
    public readonly struct FactionStats
    {
        public readonly int WorkerMaxHealth;
        public readonly int WorkerWoodCost;
        public readonly int WorkerIronCost;
        public readonly int WorkerFoodCost;
        public readonly int HeroMaxHealth;
        public readonly int HeroBaseDamage;
        public readonly int HeroDamagePerTier;
        public readonly int HoldGroundDamageBonus;
        public readonly int MainBaseMaxHealth;
        public readonly int SupplyBuildingMaxHealth;
        public readonly int SupplyBuildingWoodCost;
        public readonly int SupplyBuildingIronCost;
        public readonly int SupplyBuildingFoodProvided;

        private FactionStats(int workerMaxHealth, int workerWoodCost, int workerIronCost, int workerFoodCost, int heroMaxHealth, int heroBaseDamage, int heroDamagePerTier, int holdGroundDamageBonus, int mainBaseMaxHealth, int supplyBuildingMaxHealth, int supplyBuildingWoodCost, int supplyBuildingIronCost, int supplyBuildingFoodProvided)
        {
            WorkerMaxHealth = workerMaxHealth;
            WorkerWoodCost = workerWoodCost;
            WorkerIronCost = workerIronCost;
            WorkerFoodCost = workerFoodCost;
            HeroMaxHealth = heroMaxHealth;
            HeroBaseDamage = heroBaseDamage;
            HeroDamagePerTier = heroDamagePerTier;
            HoldGroundDamageBonus = holdGroundDamageBonus;
            MainBaseMaxHealth = mainBaseMaxHealth;
            SupplyBuildingMaxHealth = supplyBuildingMaxHealth;
            SupplyBuildingWoodCost = supplyBuildingWoodCost;
            SupplyBuildingIronCost = supplyBuildingIronCost;
            SupplyBuildingFoodProvided = supplyBuildingFoodProvided;
        }

        public static FactionStats ForFaction(int factionId)
        {
            int normalizedFaction = FactionId.Normalize(factionId);
            if (normalizedFaction == FactionId.Wrought)
            {
                return new FactionStats(90, 65, 35, 1, 280, 55, 12, 0, 1800, 650, 120, 70, 4);
            }

            if (normalizedFaction == FactionId.Gharn)
            {
                return new FactionStats(105, 40, 20, 1, 310, 48, 15, 8, 1500, 500, 90, 45, 6);
            }

            if (normalizedFaction == FactionId.Seethe)
            {
                return new FactionStats(95, 55, 40, 1, 300, 46, 16, 0, 1550, 520, 110, 65, 5);
            }

            if (normalizedFaction == FactionId.Veirn)
            {
                return new FactionStats(85, 35, 55, 1, 270, 52, 14, 0, 1450, 480, 80, 85, 5);
            }

            return new FactionStats(115, 50, 25, 1, 330, 42, 18, 0, 1650, 500, 100, 50, 5);
        }

        public static FactionStats ForPlayer(Frame f, int playerIndex)
        {
            foreach ((EntityRef entity, PlayerFactionState factionState) in f.GetComponentIterator<PlayerFactionState>())
            {
                if (factionState.PlayerIndex == playerIndex)
                {
                    return ForFaction(factionState.FactionId);
                }
            }

            return ForFaction(FactionId.ArdentConcord);
        }

        public int HeroDamageForTier(int techTier)
        {
            if (techTier < 1)
            {
                techTier = 1;
            }

            return HeroBaseDamage + (techTier - 1) * HeroDamagePerTier;
        }
    }
}
