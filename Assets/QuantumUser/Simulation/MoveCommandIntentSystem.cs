namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class MoveCommandIntentSystem : SystemMainThread
    {
        private static readonly FP ResourceCommandRadius = FP.FromString("1.25");
        private static readonly FP AttackCommandRadius = FP.FromString("1.75");

        public override void Update(Frame f)
        {
            if (f.Global->LastCommandHeld == false || f.Global->LastCommandIntent != 1)
            {
                return;
            }

            if (IsPlayerDefeated(f, f.Global->LastInputPlayer))
            {
                return;
            }

            bool hasAttackTarget = TryFindAttackTargetAtCommand(f, out EntityRef attackTargetEntity, out Targetable attackTarget, out FPVector2 attackTargetPosition);
            bool hasResourceTarget = TryFindResourceNodeAtCommand(f, out EntityRef resourceNodeEntity, out ResourceNode resourceNode, out FPVector2 resourceNodePosition);

            f.Global->HasMoveCommandIntent = true;
            f.Global->MoveCommandPlayer = f.Global->LastInputPlayer;
            f.Global->MoveCommandTargetWorld = f.Global->LastPointerWorld;

            foreach ((EntityRef entity, CommandIntentDebug commandIntentDebug) in f.GetComponentIterator<CommandIntentDebug>())
            {
                CommandIntentDebug updatedIntentDebug = commandIntentDebug;
                updatedIntentDebug.HasMoveCommandIntent = true;
                updatedIntentDebug.WasMoveCommandAccepted = false;
                updatedIntentDebug.WasMoveCommandRejected = false;
                updatedIntentDebug.MoveCommandPlayer = f.Global->LastInputPlayer;
                updatedIntentDebug.MoveCommandResult = MoveCommandResult.Pending;
                updatedIntentDebug.MoveCommandTargetWorld = f.Global->LastPointerWorld;
                f.Set(entity, updatedIntentDebug);
            }

            int selectedMoveIndex = 0;
            foreach ((EntityRef entity, Selectable selectable) in f.GetComponentIterator<Selectable>())
            {
                if (selectable.IsSelected == false)
                {
                    continue;
                }

                if (IsOwnedByInputPlayer(f, entity) == false)
                {
                    continue;
                }

                if (IsDeadUnit(f, entity))
                {
                    continue;
                }

                if (IsWorkerBuilding(f, entity))
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<MoveIntent>(entity, out MoveIntent* moveIntent) == false)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<AttackIntent>(entity, out AttackIntent* attackIntent))
                {
                    if (IsCombatUnit(f, entity) && hasAttackTarget && attackTarget.OwnerPlayer != f.Global->LastInputPlayer)
                    {
                        if (HasLiveAttackTarget(f, attackIntent->TargetEntity) && attackIntent->TargetEntity == attackTargetEntity)
                        {
                            selectedMoveIndex++;
                            continue;
                        }

                        ClearGatherIntent(f, entity);

                        attackIntent->HasTarget = true;
                        attackIntent->TargetEntity = attackTargetEntity;
                        attackIntent->TargetWorld = attackTargetPosition;
                        attackIntent->IsInRange = false;

                        moveIntent->HasTarget = true;
                        moveIntent->MovementMode = MovementMode.QuantumNavMesh;
                        moveIntent->TargetWorld = GetAttackApproachPosition(f, entity, attackIntent->AttackRange, attackTarget.TargetRadius, attackTargetPosition) + GetFormationOffset(selectedMoveIndex);
                        selectedMoveIndex++;
                        continue;
                    }

                    if (HasLiveAttackTarget(f, attackIntent->TargetEntity))
                    {
                        attackIntent->HasTarget = false;
                        attackIntent->TargetEntity = EntityRef.None;
                        attackIntent->TargetWorld = FPVector2.Zero;
                        attackIntent->IsInRange = false;
                        attackIntent->CooldownTicksRemaining = 0;
                    }

                    attackIntent->HasTarget = false;
                    attackIntent->TargetEntity = EntityRef.None;
                    attackIntent->TargetWorld = FPVector2.Zero;
                    attackIntent->IsInRange = false;
                }

                if (hasResourceTarget && f.Unsafe.TryGetPointer<GatherIntent>(entity, out GatherIntent* gatherIntent))
                {
                    gatherIntent->HasTarget = true;
                    gatherIntent->TargetNode = resourceNodeEntity;
                    gatherIntent->ResourceKind = resourceNode.ResourceKind;
                    gatherIntent->TargetWorld = resourceNodePosition;

                    moveIntent->HasTarget = true;
                    moveIntent->MovementMode = MovementMode.QuantumNavMesh;
                    moveIntent->TargetWorld = resourceNodePosition + GetFormationOffset(selectedMoveIndex);
                    selectedMoveIndex++;
                    continue;
                }

                ClearGatherIntent(f, entity);

                moveIntent->HasTarget = true;
                moveIntent->MovementMode = MovementMode.QuantumNavMesh;
                moveIntent->TargetWorld = f.Global->LastPointerWorld + GetFormationOffset(selectedMoveIndex);
                selectedMoveIndex++;
            }
        }

        private static bool TryFindAttackTargetAtCommand(Frame f, out EntityRef targetEntity, out Targetable targetable, out FPVector2 targetPosition)
        {
            targetEntity = EntityRef.None;
            targetable = default;
            targetPosition = FPVector2.Zero;

            FP bestDistance = AttackCommandRadius;
            bool found = false;
            foreach ((EntityRef entity, Targetable candidateTargetable) in f.GetComponentIterator<Targetable>())
            {
                if (candidateTargetable.Health <= 0)
                {
                    continue;
                }

                if (IsPlayerDefeated(f, candidateTargetable.OwnerPlayer))
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false)
                {
                    continue;
                }

                FP distance = FPVector2.Distance(transform->Position, f.Global->LastPointerWorld);
                if (distance > bestDistance + candidateTargetable.TargetRadius)
                {
                    continue;
                }

                bestDistance = distance;
                targetEntity = entity;
                targetable = candidateTargetable;
                targetPosition = transform->Position;
                found = true;
            }

            return found;
        }

        private static bool TryFindResourceNodeAtCommand(Frame f, out EntityRef resourceNodeEntity, out ResourceNode resourceNode, out FPVector2 resourceNodePosition)
        {
            resourceNodeEntity = EntityRef.None;
            resourceNode = default;
            resourceNodePosition = FPVector2.Zero;

            FP bestDistance = ResourceCommandRadius;
            bool found = false;
            foreach ((EntityRef entity, ResourceNode candidateNode) in f.GetComponentIterator<ResourceNode>())
            {
                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false)
                {
                    continue;
                }

                FP distance = FPVector2.Distance(transform->Position, f.Global->LastPointerWorld);
                if (distance > bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                resourceNodeEntity = entity;
                resourceNode = candidateNode;
                resourceNodePosition = transform->Position;
                found = true;
            }

            return found;
        }

        private static FPVector2 GetFormationOffset(int selectedMoveIndex)
        {
            FP spacing = FP.FromString("1.1");

            if (selectedMoveIndex == 0)
            {
                return FPVector2.Zero;
            }

            if (selectedMoveIndex == 1)
            {
                return new FPVector2(-spacing, -spacing);
            }

            if (selectedMoveIndex == 2)
            {
                return new FPVector2(spacing, -spacing);
            }

            if (selectedMoveIndex == 3)
            {
                return new FPVector2(-spacing, FP._0);
            }

            if (selectedMoveIndex == 4)
            {
                return new FPVector2(spacing, FP._0);
            }

            return new FPVector2(FP._0, -spacing - spacing);
        }

        private static FPVector2 GetAttackApproachPosition(Frame f, EntityRef attackerEntity, FP attackRange, FP targetRadius, FPVector2 targetPosition)
        {
            if (f.Unsafe.TryGetPointer<Transform2D>(attackerEntity, out Transform2D* attackerTransform) == false)
            {
                return targetPosition;
            }

            FPVector2 awayFromTarget = attackerTransform->Position - targetPosition;
            FP distance = FPVector2.Distance(attackerTransform->Position, targetPosition);
            if (distance <= FP.EN1)
            {
                awayFromTarget = new FPVector2(FP._1, FP._0);
                distance = FP._1;
            }

            FP desiredDistance = attackRange + targetRadius - FP.FromString("0.1");
            if (desiredDistance < targetRadius)
            {
                desiredDistance = targetRadius;
            }

            FP scale = desiredDistance / distance;
            return targetPosition + awayFromTarget * scale;
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

        private static bool IsOwnedByInputPlayer(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity) == false)
            {
                return false;
            }

            return unitIdentity->OwnerPlayer == f.Global->LastInputPlayer;
        }

        private static bool IsWorkerBuilding(Frame f, EntityRef entity)
        {
            return f.Unsafe.TryGetPointer<WorkerBuildIntent>(entity, out WorkerBuildIntent* buildIntent) &&
                   buildIntent->IsBuilding;
        }

        private static bool IsCombatUnit(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity) == false)
            {
                return false;
            }

            return unitIdentity->UnitKind == UnitKind.Hero;
        }

        private static bool IsDeadUnit(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<UnitHealth>(entity, out UnitHealth* unitHealth) == false)
            {
                return false;
            }

            return unitHealth->IsDead;
        }

        private static bool HasLiveAttackTarget(Frame f, EntityRef targetEntity)
        {
            if (targetEntity == EntityRef.None)
            {
                return false;
            }

            if (f.Unsafe.TryGetPointer<Targetable>(targetEntity, out Targetable* targetable) == false)
            {
                return false;
            }

            return targetable->Health > 0;
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
    }
}
