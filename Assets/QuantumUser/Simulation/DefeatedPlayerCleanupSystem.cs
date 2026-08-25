namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class DefeatedPlayerCleanupSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            foreach ((EntityRef entity, UnitIdentity unitIdentity) in f.GetComponentIterator<UnitIdentity>())
            {
                if (IsPlayerDefeated(f, unitIdentity.OwnerPlayer) == false)
                {
                    continue;
                }

                ClearSelectable(f, entity);
                ClearMoveIntent(f, entity);
                ClearGatherIntent(f, entity);
                ClearAttackIntent(f, entity);
                ClearBuildIntent(f, entity);

                if (f.Unsafe.TryGetPointer<Targetable>(entity, out Targetable* targetable))
                {
                    targetable->Health = 0;
                }

                if (f.Unsafe.TryGetPointer<UnitHealth>(entity, out UnitHealth* unitHealth))
                {
                    unitHealth->Health = 0;
                    unitHealth->IsDead = true;
                }
            }
        }

        private static bool IsPlayerDefeated(Frame f, int playerIndex)
        {
            foreach ((EntityRef entity, PlayerEconomyState economyState) in f.GetComponentIterator<PlayerEconomyState>())
            {
                if (economyState.PlayerIndex == playerIndex)
                {
                    return economyState.IsDefeated;
                }
            }

            return false;
        }

        private static void ClearSelectable(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<Selectable>(entity, out Selectable* selectable) == false)
            {
                return;
            }

            selectable->IsSelected = false;
        }

        private static void ClearMoveIntent(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<MoveIntent>(entity, out MoveIntent* moveIntent) == false)
            {
                return;
            }

            moveIntent->HasTarget = false;
            moveIntent->TargetWorld = FPVector2.Zero;
        }

        private static void ClearGatherIntent(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<GatherIntent>(entity, out GatherIntent* gatherIntent) == false)
            {
                return;
            }

            gatherIntent->HasTarget = false;
            gatherIntent->TargetNode = EntityRef.None;
            gatherIntent->ResourceKind = ResourceKind.None;
            gatherIntent->TargetWorld = FPVector2.Zero;
        }

        private static void ClearAttackIntent(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<AttackIntent>(entity, out AttackIntent* attackIntent) == false)
            {
                return;
            }

            attackIntent->HasTarget = false;
            attackIntent->TargetEntity = EntityRef.None;
            attackIntent->TargetWorld = FPVector2.Zero;
            attackIntent->IsInRange = false;
            attackIntent->CooldownTicksRemaining = 0;
        }

        private static void ClearBuildIntent(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<WorkerBuildIntent>(entity, out WorkerBuildIntent* buildIntent) == false)
            {
                return;
            }

            buildIntent->IsBuilding = false;
            buildIntent->TargetBuilding = EntityRef.None;
        }
    }
}
