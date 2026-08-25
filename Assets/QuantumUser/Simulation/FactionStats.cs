namespace Quantum
{
    public readonly struct FactionStats
    {
        public readonly int WorkerMaxHealth;
        public readonly int HeroMaxHealth;
        public readonly int HeroBaseDamage;
        public readonly int HeroDamagePerTier;
        public readonly int MainBaseMaxHealth;
        public readonly int SupplyBuildingMaxHealth;

        private FactionStats(int workerMaxHealth, int heroMaxHealth, int heroBaseDamage, int heroDamagePerTier, int mainBaseMaxHealth, int supplyBuildingMaxHealth)
        {
            WorkerMaxHealth = workerMaxHealth;
            HeroMaxHealth = heroMaxHealth;
            HeroBaseDamage = heroBaseDamage;
            HeroDamagePerTier = heroDamagePerTier;
            MainBaseMaxHealth = mainBaseMaxHealth;
            SupplyBuildingMaxHealth = supplyBuildingMaxHealth;
        }

        public static FactionStats ForFaction(int factionId)
        {
            int normalizedFaction = FactionId.Normalize(factionId);
            if (normalizedFaction == FactionId.Fantasy)
            {
                return new FactionStats(90, 280, 55, 12, 1800, 650);
            }

            if (normalizedFaction == FactionId.Hybrid)
            {
                return new FactionStats(105, 310, 48, 15, 1500, 500);
            }

            return new FactionStats(115, 330, 42, 18, 1650, 500);
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
