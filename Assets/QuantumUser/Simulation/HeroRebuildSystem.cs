namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class HeroRebuildSystem : SystemMainThread
    {
        private const int RebuildHeroIntent = 2;
        private const int RebuildWoodCost = 150;
        private const int RebuildIronCost = 100;
        private const int RebuildTicks = 180;

        public override void Update(Frame f)
        {
            TickActiveRebuilds(f);

            if (f.Global->LastUpgradeIntent != RebuildHeroIntent)
            {
                return;
            }

            f.Global->LastUpgradeIntent = 0;
            int playerIndex = f.Global->LastInputPlayer;

            if (TryGetHeroState(f, playerIndex, out EntityRef heroStateEntity, out PlayerHeroState heroState) == false)
            {
                return;
            }

            if (TryGetEconomyState(f, playerIndex, out EntityRef economyEntity, out PlayerEconomyState economyState) == false)
            {
                SetHeroResult(f, heroStateEntity, heroState, HeroLifecycleResult.RebuildUnavailable);
                return;
            }

            if (economyState.IsDefeated || TryGetMainBasePosition(f, playerIndex, out FPVector2 _) == false)
            {
                SetHeroResult(f, heroStateEntity, heroState, HeroLifecycleResult.MissingMainBase);
                return;
            }

            if (heroState.RebuildInProgress)
            {
                SetHeroResult(f, heroStateEntity, heroState, HeroLifecycleResult.RebuildInProgress);
                return;
            }

            if (heroState.RebuildAvailable == false || heroState.HeroEntity == EntityRef.None)
            {
                SetHeroResult(f, heroStateEntity, heroState, HeroLifecycleResult.RebuildUnavailable);
                return;
            }

            if (economyState.Wood < RebuildWoodCost || economyState.Iron < RebuildIronCost)
            {
                SetHeroResult(f, heroStateEntity, heroState, HeroLifecycleResult.InsufficientResources);
                return;
            }

            PlayerEconomyState updatedEconomy = economyState;
            updatedEconomy.Wood -= RebuildWoodCost;
            updatedEconomy.Iron -= RebuildIronCost;
            f.Set(economyEntity, updatedEconomy);

            PlayerHeroState updatedHeroState = heroState;
            updatedHeroState.HasActiveHero = false;
            updatedHeroState.RebuildAvailable = false;
            updatedHeroState.RebuildInProgress = true;
            updatedHeroState.RebuildTicksRemaining = RebuildTicks;
            updatedHeroState.RebuildTicksTotal = RebuildTicks;
            updatedHeroState.LastHeroResult = HeroLifecycleResult.RebuildStarted;
            f.Set(heroStateEntity, updatedHeroState);
        }

        private static void TickActiveRebuilds(Frame f)
        {
            foreach ((EntityRef heroStateEntity, PlayerHeroState heroState) in f.GetComponentIterator<PlayerHeroState>())
            {
                if (heroState.RebuildInProgress == false)
                {
                    continue;
                }

                PlayerHeroState updatedHeroState = heroState;
                updatedHeroState.RebuildTicksRemaining--;
                if (updatedHeroState.RebuildTicksRemaining > 0)
                {
                    f.Set(heroStateEntity, updatedHeroState);
                    continue;
                }

                if (TryGetMainBasePosition(f, heroState.PlayerIndex, out FPVector2 basePosition) == false ||
                    heroState.HeroEntity == EntityRef.None)
                {
                    updatedHeroState.HasActiveHero = false;
                    updatedHeroState.RebuildAvailable = false;
                    updatedHeroState.RebuildInProgress = false;
                    updatedHeroState.RebuildTicksRemaining = 0;
                    updatedHeroState.RebuildTicksTotal = 0;
                    updatedHeroState.LastHeroResult = HeroLifecycleResult.MissingMainBase;
                    f.Set(heroStateEntity, updatedHeroState);
                    continue;
                }

                FactionStats stats = FactionStats.ForPlayer(f, heroState.PlayerIndex);
                ReviveHero(f, heroState.HeroEntity, basePosition + new FPVector2(FP._0, FP.FromString("-2.0")), stats.HeroMaxHealth);
                GrainStateSystem.MarkGrainLoud(f, heroState.HeroEntity, GrainLoudSource.HeroRebuild);

                updatedHeroState.HasActiveHero = true;
                updatedHeroState.RebuildAvailable = false;
                updatedHeroState.RebuildInProgress = false;
                updatedHeroState.RebuildTicksRemaining = 0;
                updatedHeroState.RebuildTicksTotal = 0;
                updatedHeroState.HeroHealth = stats.HeroMaxHealth;
                updatedHeroState.HeroMaxHealth = stats.HeroMaxHealth;
                updatedHeroState.LastHeroResult = HeroLifecycleResult.Rebuilt;
                f.Set(heroStateEntity, updatedHeroState);
            }
        }

        private static void ReviveHero(Frame f, EntityRef heroEntity, FPVector2 spawnPosition, int heroMaxHealth)
        {
            if (f.Unsafe.TryGetPointer<UnitHealth>(heroEntity, out UnitHealth* unitHealth))
            {
                unitHealth->Health = heroMaxHealth;
                unitHealth->MaxHealth = heroMaxHealth;
                unitHealth->IsDead = false;
            }

            if (f.Unsafe.TryGetPointer<Targetable>(heroEntity, out Targetable* targetable))
            {
                targetable->Health = heroMaxHealth;
                targetable->MaxHealth = heroMaxHealth;
            }

            if (f.Unsafe.TryGetPointer<Transform2D>(heroEntity, out Transform2D* transform))
            {
                transform->Position = spawnPosition;
            }

            if (f.Unsafe.TryGetPointer<Selectable>(heroEntity, out Selectable* selectable))
            {
                selectable->IsSelected = false;
            }

            if (f.Unsafe.TryGetPointer<MoveIntent>(heroEntity, out MoveIntent* moveIntent))
            {
                moveIntent->HasTarget = false;
                moveIntent->TargetWorld = FPVector2.Zero;
            }

            if (f.Unsafe.TryGetPointer<GatherIntent>(heroEntity, out GatherIntent* gatherIntent))
            {
                gatherIntent->HasTarget = false;
                gatherIntent->TargetNode = EntityRef.None;
                gatherIntent->ResourceKind = ResourceKind.None;
                gatherIntent->TargetWorld = FPVector2.Zero;
            }

            if (f.Unsafe.TryGetPointer<AttackIntent>(heroEntity, out AttackIntent* attackIntent))
            {
                attackIntent->HasTarget = false;
                attackIntent->TargetEntity = EntityRef.None;
                attackIntent->TargetWorld = FPVector2.Zero;
                attackIntent->CooldownTicksRemaining = 0;
                attackIntent->IsInRange = false;
            }
        }

        private static bool TryGetHeroState(Frame f, int playerIndex, out EntityRef heroStateEntity, out PlayerHeroState heroState)
        {
            foreach ((EntityRef entity, PlayerHeroState state) in f.GetComponentIterator<PlayerHeroState>())
            {
                if (state.PlayerIndex == playerIndex)
                {
                    heroStateEntity = entity;
                    heroState = state;
                    return true;
                }
            }

            heroStateEntity = EntityRef.None;
            heroState = default;
            return false;
        }

        private static bool TryGetEconomyState(Frame f, int playerIndex, out EntityRef economyEntity, out PlayerEconomyState economyState)
        {
            foreach ((EntityRef entity, PlayerEconomyState state) in f.GetComponentIterator<PlayerEconomyState>())
            {
                if (state.PlayerIndex == playerIndex)
                {
                    economyEntity = entity;
                    economyState = state;
                    return true;
                }
            }

            economyEntity = EntityRef.None;
            economyState = default;
            return false;
        }

        private static bool TryGetMainBasePosition(Frame f, int playerIndex, out FPVector2 position)
        {
            foreach ((EntityRef entity, MainBuilding mainBuilding) in f.GetComponentIterator<MainBuilding>())
            {
                if (mainBuilding.OwnerPlayer != playerIndex || mainBuilding.Health <= 0)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false)
                {
                    continue;
                }

                position = transform->Position;
                return true;
            }

            position = FPVector2.Zero;
            return false;
        }

        private static void SetHeroResult(Frame f, EntityRef heroStateEntity, PlayerHeroState heroState, int result)
        {
            PlayerHeroState updatedHeroState = heroState;
            updatedHeroState.LastHeroResult = result;
            f.Set(heroStateEntity, updatedHeroState);
        }
    }
}
