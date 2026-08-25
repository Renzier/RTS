namespace Quantum
{
    public unsafe class TechUpgradeSystem : SystemMainThread
    {
        private const int UpgradeIntentTier = 1;
        private const int MaxTechTier = 3;
        private const int BaseWoodCost = 200;
        private const int BaseIronCost = 150;
        private const int UpgradeTicks = 300;

        public override void Update(Frame f)
        {
            TickActiveUpgrades(f);

            if (f.Global->LastUpgradeIntent != UpgradeIntentTier)
            {
                return;
            }

            f.Global->LastUpgradeIntent = 0;
            int playerIndex = f.Global->LastInputPlayer;

            if (TryGetTechState(f, playerIndex, out EntityRef techEntity, out PlayerTechState techState) == false)
            {
                return;
            }

            if (TryGetEconomyState(f, playerIndex, out EntityRef economyEntity, out PlayerEconomyState economyState) == false)
            {
                SetUpgradeResult(f, techEntity, techState, TechUpgradeResult.MissingEconomy);
                return;
            }

            if (economyState.IsDefeated)
            {
                SetUpgradeResult(f, techEntity, techState, TechUpgradeResult.Defeated);
                return;
            }

            if (techState.UpgradeInProgress)
            {
                SetUpgradeResult(f, techEntity, techState, TechUpgradeResult.InProgress);
                return;
            }

            if (techState.TechTier >= MaxTechTier)
            {
                SetUpgradeResult(f, techEntity, techState, TechUpgradeResult.MaxTier);
                return;
            }

            int nextTier = techState.TechTier + 1;
            int woodCost = BaseWoodCost * nextTier;
            int ironCost = BaseIronCost * nextTier;

            if (economyState.Wood < woodCost || economyState.Iron < ironCost)
            {
                SetUpgradeResult(f, techEntity, techState, TechUpgradeResult.InsufficientResources);
                return;
            }

            PlayerEconomyState updatedEconomy = economyState;
            updatedEconomy.Wood -= woodCost;
            updatedEconomy.Iron -= ironCost;
            f.Set(economyEntity, updatedEconomy);

            PlayerTechState updatedTech = techState;
            updatedTech.UpgradeInProgress = true;
            updatedTech.UpgradeTargetTier = nextTier;
            updatedTech.UpgradeTicksRemaining = UpgradeTicks;
            updatedTech.UpgradeTicksTotal = UpgradeTicks;
            updatedTech.LastUpgradeResult = TechUpgradeResult.Started;
            f.Set(techEntity, updatedTech);
        }

        private static void TickActiveUpgrades(Frame f)
        {
            foreach ((EntityRef entity, PlayerTechState techState) in f.GetComponentIterator<PlayerTechState>())
            {
                if (techState.UpgradeInProgress == false)
                {
                    continue;
                }

                PlayerTechState updatedTech = techState;
                if (TryGetEconomyState(f, techState.PlayerIndex, out EntityRef economyEntity, out PlayerEconomyState economyState) && economyState.IsDefeated)
                {
                    updatedTech.UpgradeInProgress = false;
                    updatedTech.UpgradeTargetTier = 0;
                    updatedTech.UpgradeTicksRemaining = 0;
                    updatedTech.UpgradeTicksTotal = 0;
                    updatedTech.LastUpgradeResult = TechUpgradeResult.Defeated;
                    f.Set(entity, updatedTech);
                    continue;
                }

                updatedTech.UpgradeTicksRemaining--;
                if (updatedTech.UpgradeTicksRemaining > 0)
                {
                    f.Set(entity, updatedTech);
                    continue;
                }

                updatedTech.TechTier = updatedTech.UpgradeTargetTier;
                updatedTech.UpgradeInProgress = false;
                updatedTech.UpgradeTargetTier = 0;
                updatedTech.UpgradeTicksRemaining = 0;
                updatedTech.UpgradeTicksTotal = 0;
                updatedTech.LastUpgradeResult = TechUpgradeResult.Upgraded;
                f.Set(entity, updatedTech);
            }
        }

        private static bool TryGetTechState(Frame f, int playerIndex, out EntityRef techEntity, out PlayerTechState techState)
        {
            foreach ((EntityRef entity, PlayerTechState state) in f.GetComponentIterator<PlayerTechState>())
            {
                if (state.PlayerIndex == playerIndex)
                {
                    techEntity = entity;
                    techState = state;
                    return true;
                }
            }

            techEntity = EntityRef.None;
            techState = default;
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

        private static void SetUpgradeResult(Frame f, EntityRef techEntity, PlayerTechState techState, int result)
        {
            PlayerTechState updatedTech = techState;
            if (result != TechUpgradeResult.InProgress)
            {
                updatedTech.UpgradeInProgress = false;
                updatedTech.UpgradeTargetTier = 0;
                updatedTech.UpgradeTicksRemaining = 0;
                updatedTech.UpgradeTicksTotal = 0;
            }
            updatedTech.LastUpgradeResult = result;
            f.Set(techEntity, updatedTech);
        }
    }
}
