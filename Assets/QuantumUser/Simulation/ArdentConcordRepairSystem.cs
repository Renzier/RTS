namespace Quantum
{
    public unsafe class ArdentConcordRepairSystem : SystemMainThread
    {
        private const int RepairIntervalTicks = 60;
        private const int RepairAmount = 5;

        public override void Update(Frame f)
        {
            if (f.Number % RepairIntervalTicks != 0)
            {
                return;
            }

            RepairMainBuildings(f);
            RepairCompletedSupplyBuildings(f);
        }

        private static void RepairMainBuildings(Frame f)
        {
            foreach ((EntityRef entity, MainBuilding mainBuilding) in f.GetComponentIterator<MainBuilding>())
            {
                if (IsArdentConcord(f, mainBuilding.OwnerPlayer) == false ||
                    mainBuilding.Health <= 0 ||
                    mainBuilding.Health >= mainBuilding.MaxHealth)
                {
                    continue;
                }

                MainBuilding updatedBuilding = mainBuilding;
                updatedBuilding.Health = ClampRepair(updatedBuilding.Health, updatedBuilding.MaxHealth);
                f.Set(entity, updatedBuilding);
                SyncTargetableHealth(f, entity, updatedBuilding.Health, updatedBuilding.MaxHealth);
            }
        }

        private static void RepairCompletedSupplyBuildings(Frame f)
        {
            foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in f.GetComponentIterator<SupplyBuilding>())
            {
                if (IsArdentConcord(f, supplyBuilding.OwnerPlayer) == false ||
                    supplyBuilding.Health <= 0 ||
                    supplyBuilding.Health >= supplyBuilding.MaxHealth ||
                    supplyBuilding.IsConstructing ||
                    supplyBuilding.IsDeconstructing)
                {
                    continue;
                }

                SupplyBuilding updatedBuilding = supplyBuilding;
                updatedBuilding.Health = ClampRepair(updatedBuilding.Health, updatedBuilding.MaxHealth);
                f.Set(entity, updatedBuilding);
                SyncTargetableHealth(f, entity, updatedBuilding.Health, updatedBuilding.MaxHealth);
            }
        }

        private static int ClampRepair(int health, int maxHealth)
        {
            health += RepairAmount;
            return health > maxHealth ? maxHealth : health;
        }

        private static void SyncTargetableHealth(Frame f, EntityRef entity, int health, int maxHealth)
        {
            if (f.Unsafe.TryGetPointer<Targetable>(entity, out Targetable* targetable) == false)
            {
                return;
            }

            targetable->Health = health;
            targetable->MaxHealth = maxHealth;
        }

        private static bool IsArdentConcord(Frame f, int playerIndex)
        {
            foreach ((EntityRef entity, PlayerFactionState factionState) in f.GetComponentIterator<PlayerFactionState>())
            {
                if (factionState.PlayerIndex == playerIndex)
                {
                    return FactionId.Normalize(factionState.FactionId) == FactionId.Tech;
                }
            }

            return playerIndex == 0;
        }
    }
}
