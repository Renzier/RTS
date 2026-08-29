namespace Quantum
{
    public unsafe class MainBaseDefeatSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            foreach ((EntityRef playerEntity, PlayerEconomyState economyState) in f.GetComponentIterator<PlayerEconomyState>())
            {
                if (economyState.IsDefeated)
                {
                    continue;
                }

                bool hasAnyMainBase = false;
                bool hasLivingMainBase = false;

                foreach ((EntityRef baseEntity, MainBuilding mainBuilding) in f.GetComponentIterator<MainBuilding>())
                {
                    if (mainBuilding.OwnerPlayer != economyState.PlayerIndex)
                    {
                        continue;
                    }

                    hasAnyMainBase = true;
                    if (mainBuilding.Health > 0)
                    {
                        hasLivingMainBase = true;
                        break;
                    }
                }

                bool isDefeated = hasAnyMainBase && hasLivingMainBase == false;
                if (economyState.IsDefeated == isDefeated)
                {
                    continue;
                }

                PlayerEconomyState updatedState = economyState;
                updatedState.IsDefeated = isDefeated;
                f.Set(playerEntity, updatedState);
            }
        }
    }
}
