namespace Quantum
{
    public unsafe class UnitDeathSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            foreach ((EntityRef entity, UnitHealth unitHealth) in f.GetComponentIterator<UnitHealth>())
            {
                bool isDead = unitHealth.Health <= 0;
                if (unitHealth.IsDead != isDead)
                {
                    UnitHealth updatedUnitHealth = unitHealth;
                    updatedUnitHealth.IsDead = isDead;
                    f.Set(entity, updatedUnitHealth);
                }

                if (isDead == false)
                {
                    continue;
                }

                ClearSelectable(f, entity);
                ClearMoveIntent(f, entity);
                ClearGatherIntent(f, entity);
                ClearAttackIntent(f, entity);
                ClearBuildIntent(f, entity);
            }
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
        }

        private static void ClearAttackIntent(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<AttackIntent>(entity, out AttackIntent* attackIntent) == false)
            {
                return;
            }

            attackIntent->HasTarget = false;
            attackIntent->TargetEntity = EntityRef.None;
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
