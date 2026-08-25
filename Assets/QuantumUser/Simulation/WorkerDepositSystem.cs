namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class WorkerDepositSystem : SystemMainThread
    {
        private static readonly FP DepositRange = FP.FromString("2.25");

        public override void Update(Frame f)
        {
            foreach ((EntityRef entity, WorkerResourceCarry carry) in f.GetComponentIterator<WorkerResourceCarry>())
            {
                if (IsWorkerBuilding(f, entity))
                {
                    continue;
                }

                if (carry.Amount < carry.Capacity)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<GatherIntent>(entity, out GatherIntent* gatherIntent) == false)
                {
                    continue;
                }

                if (gatherIntent->HasTarget == false)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity) == false)
                {
                    continue;
                }

                if (IsPlayerDefeated(f, unitIdentity->OwnerPlayer))
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* workerTransform) == false)
                {
                    continue;
                }

                if (TryFindDropoff(f, unitIdentity->OwnerPlayer, carry.ResourceKind, out FPVector2 dropoffPosition) == false)
                {
                    continue;
                }

                FP distanceToDropoff = FPVector2.Distance(workerTransform->Position, dropoffPosition);
                if (distanceToDropoff > DepositRange)
                {
                    RequestMove(f, entity, dropoffPosition);
                    continue;
                }

                DepositCarry(f, unitIdentity->OwnerPlayer, carry);
                ClearCarry(f, entity, carry);

                if (f.Unsafe.TryGetPointer<Transform2D>(gatherIntent->TargetNode, out Transform2D* nodeTransform))
                {
                    RequestMove(f, entity, nodeTransform->Position);
                }
            }
        }

        private static bool TryFindDropoff(Frame f, int ownerPlayer, int resourceKind, out FPVector2 dropoffPosition)
        {
            dropoffPosition = FPVector2.Zero;

            foreach ((EntityRef entity, ResourceDropoff dropoff) in f.GetComponentIterator<ResourceDropoff>())
            {
                if (dropoff.OwnerPlayer != ownerPlayer)
                {
                    continue;
                }

                if (AcceptsResource(dropoff.AcceptedResourceMask, resourceKind) == false)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false)
                {
                    continue;
                }

                dropoffPosition = transform->Position;
                return true;
            }

            return false;
        }

        private static bool AcceptsResource(int acceptedResourceMask, int resourceKind)
        {
            if (resourceKind == ResourceKind.Wood)
            {
                return (acceptedResourceMask & ResourceMask.Wood) != 0;
            }

            if (resourceKind == ResourceKind.Iron)
            {
                return (acceptedResourceMask & ResourceMask.Iron) != 0;
            }

            return false;
        }

        private static void DepositCarry(Frame f, int ownerPlayer, WorkerResourceCarry carry)
        {
            foreach ((EntityRef entity, PlayerEconomyState state) in f.GetComponentIterator<PlayerEconomyState>())
            {
                if (state.PlayerIndex != ownerPlayer)
                {
                    continue;
                }

                PlayerEconomyState updatedState = state;
                if (carry.ResourceKind == ResourceKind.Wood)
                {
                    updatedState.Wood += carry.Amount;
                }
                else if (carry.ResourceKind == ResourceKind.Iron)
                {
                    updatedState.Iron += carry.Amount;
                }

                f.Set(entity, updatedState);
                return;
            }
        }

        private static void ClearCarry(Frame f, EntityRef entity, WorkerResourceCarry carry)
        {
            WorkerResourceCarry updatedCarry = carry;
            updatedCarry.ResourceKind = ResourceKind.None;
            updatedCarry.Amount = 0;
            f.Set(entity, updatedCarry);
        }

        private static void RequestMove(Frame f, EntityRef entity, FPVector2 targetWorld)
        {
            if (f.Unsafe.TryGetPointer<MoveIntent>(entity, out MoveIntent* moveIntent) == false)
            {
                return;
            }

            moveIntent->HasTarget = true;
            moveIntent->MovementMode = MovementMode.QuantumNavMesh;
            moveIntent->TargetWorld = targetWorld;
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

        private static bool IsWorkerBuilding(Frame f, EntityRef entity)
        {
            return f.Unsafe.TryGetPointer<WorkerBuildIntent>(entity, out WorkerBuildIntent* buildIntent) &&
                   buildIntent->IsBuilding;
        }
    }
}
