namespace Quantum
{
    public unsafe class PrototypeDebugDamageSystem : SystemMainThread
    {
        private const int DebugDamageIntent = 6;
        private const int DebugDamageAmount = 250;

        public override void Update(Frame f)
        {
            if (f.Global->LastUpgradeIntent != DebugDamageIntent)
            {
                return;
            }

            f.Global->LastUpgradeIntent = 0;
            DamageSelectedBuilding(f, f.Global->LastInputPlayer);
        }

        private static void DamageSelectedBuilding(Frame f, int playerIndex)
        {
            if (TryDamageSelectedMainBuilding(f, playerIndex))
            {
                return;
            }

            TryDamageSelectedSupplyBuilding(f, playerIndex);
        }

        private static bool TryDamageSelectedMainBuilding(Frame f, int playerIndex)
        {
            foreach ((EntityRef entity, MainBuilding mainBuilding) in f.GetComponentIterator<MainBuilding>())
            {
                if (mainBuilding.OwnerPlayer != playerIndex ||
                    mainBuilding.Health <= 0 ||
                    IsSelected(f, entity) == false)
                {
                    continue;
                }

                MainBuilding updatedBuilding = mainBuilding;
                updatedBuilding.Health = ApplyDamage(updatedBuilding.Health);
                f.Set(entity, updatedBuilding);
                SyncTargetableHealth(f, entity, updatedBuilding.Health, updatedBuilding.MaxHealth);
                return true;
            }

            return false;
        }

        private static bool TryDamageSelectedSupplyBuilding(Frame f, int playerIndex)
        {
            foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in f.GetComponentIterator<SupplyBuilding>())
            {
                if (supplyBuilding.OwnerPlayer != playerIndex ||
                    supplyBuilding.Health <= 0 ||
                    IsSelected(f, entity) == false)
                {
                    continue;
                }

                SupplyBuilding updatedBuilding = supplyBuilding;
                updatedBuilding.Health = ApplyDamage(updatedBuilding.Health);
                f.Set(entity, updatedBuilding);
                SyncTargetableHealth(f, entity, updatedBuilding.Health, updatedBuilding.MaxHealth);
                return true;
            }

            return false;
        }

        private static int ApplyDamage(int health)
        {
            int damagedHealth = health - DebugDamageAmount;
            return damagedHealth < 1 ? 1 : damagedHealth;
        }

        private static bool IsSelected(Frame f, EntityRef candidateEntity)
        {
            if (f.Unsafe.TryGetPointer<Selectable>(candidateEntity, out Selectable* selectable) == false)
            {
                return false;
            }

            return selectable->IsSelected;
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
    }
}
