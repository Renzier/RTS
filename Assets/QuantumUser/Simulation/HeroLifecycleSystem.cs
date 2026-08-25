namespace Quantum
{
    public unsafe class HeroLifecycleSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            foreach ((EntityRef entity, PlayerHeroState heroState) in f.GetComponentIterator<PlayerHeroState>())
            {
                if (TryGetEconomyState(f, heroState.PlayerIndex, out PlayerEconomyState economyState) == false)
                {
                    continue;
                }

                bool hasLivingMainBase = HasLivingMainBase(f, heroState.PlayerIndex);
                PlayerHeroState updatedHeroState = heroState;
                SyncHeroHealthFromEntity(f, ref updatedHeroState);

                if (economyState.IsDefeated)
                {
                    updatedHeroState.HasActiveHero = false;
                    updatedHeroState.RebuildAvailable = false;
                    updatedHeroState.RebuildInProgress = false;
                    updatedHeroState.RebuildTicksRemaining = 0;
                    updatedHeroState.RebuildTicksTotal = 0;
                    updatedHeroState.LastHeroResult = HeroLifecycleResult.Defeated;
                    f.Set(entity, updatedHeroState);
                    continue;
                }

                if (hasLivingMainBase == false)
                {
                    updatedHeroState.HasActiveHero = false;
                    updatedHeroState.RebuildAvailable = false;
                    updatedHeroState.RebuildInProgress = false;
                    updatedHeroState.RebuildTicksRemaining = 0;
                    updatedHeroState.RebuildTicksTotal = 0;
                    updatedHeroState.LastHeroResult = HeroLifecycleResult.MissingMainBase;
                    f.Set(entity, updatedHeroState);
                    continue;
                }

                if (updatedHeroState.RebuildInProgress)
                {
                    updatedHeroState.HasActiveHero = false;
                    updatedHeroState.RebuildAvailable = false;
                    updatedHeroState.LastHeroResult = HeroLifecycleResult.RebuildInProgress;
                    f.Set(entity, updatedHeroState);
                    continue;
                }

                if (updatedHeroState.HeroHealth <= 0)
                {
                    updatedHeroState.HasActiveHero = false;
                    updatedHeroState.RebuildAvailable = true;
                    updatedHeroState.LastHeroResult = HeroLifecycleResult.RebuildAvailable;
                    f.Set(entity, updatedHeroState);
                    continue;
                }

                updatedHeroState.HasActiveHero = true;
                updatedHeroState.RebuildAvailable = false;
                if (updatedHeroState.LastHeroResult != HeroLifecycleResult.Rebuilt)
                {
                    updatedHeroState.LastHeroResult = HeroLifecycleResult.Active;
                }
                f.Set(entity, updatedHeroState);
            }
        }

        private static bool TryGetEconomyState(Frame f, int playerIndex, out PlayerEconomyState economyState)
        {
            foreach ((EntityRef entity, PlayerEconomyState state) in f.GetComponentIterator<PlayerEconomyState>())
            {
                if (state.PlayerIndex == playerIndex)
                {
                    economyState = state;
                    return true;
                }
            }

            economyState = default;
            return false;
        }

        private static bool HasLivingMainBase(Frame f, int playerIndex)
        {
            foreach ((EntityRef entity, MainBuilding mainBuilding) in f.GetComponentIterator<MainBuilding>())
            {
                if (mainBuilding.OwnerPlayer == playerIndex && mainBuilding.Health > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static void SyncHeroHealthFromEntity(Frame f, ref PlayerHeroState heroState)
        {
            if (heroState.HeroEntity == EntityRef.None)
            {
                return;
            }

            if (f.Unsafe.TryGetPointer<UnitHealth>(heroState.HeroEntity, out UnitHealth* unitHealth) == false)
            {
                return;
            }

            heroState.HeroHealth = unitHealth->Health;
            heroState.HeroMaxHealth = unitHealth->MaxHealth;
        }
    }
}
