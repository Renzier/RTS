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
            if (normalizedFaction == FactionId.Fantasy)
            {
                return new FactionStats(90, 65, 35, 1, 280, 55, 12, 0, 1800, 650, 120, 70, 4);
            }

            if (normalizedFaction == FactionId.Hybrid)
            {
                return new FactionStats(105, 40, 20, 1, 310, 48, 15, 8, 1500, 500, 90, 45, 6);
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

            return ForFaction(FactionId.Tech);
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
