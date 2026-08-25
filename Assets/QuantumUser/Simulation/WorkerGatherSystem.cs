namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class WorkerGatherSystem : SystemMainThread
    {
        private static readonly FP GatherRange = FP.FromString("0.9");
        private const int HarvestPerSecond = 1;
        private const int FallbackSimulationRate = 60;

        public override void Update(Frame f)
        {
            foreach ((EntityRef entity, GatherIntent gatherIntent) in f.GetComponentIterator<GatherIntent>())
            {
                if (gatherIntent.HasTarget == false)
                {
                    continue;
                }

                if (IsDefeatedWorker(f, entity))
                {
                    ClearGatherIntent(f, entity, gatherIntent);
                    continue;
                }

                if (IsWorkerBuilding(f, entity))
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* workerTransform) == false)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<WorkerResourceCarry>(entity, out WorkerResourceCarry* carry) == false)
                {
                    continue;
                }

                if (carry->Amount >= carry->Capacity)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(gatherIntent.TargetNode, out Transform2D* nodeTransform) == false)
                {
                    ClearGatherIntent(f, entity, gatherIntent);
                    continue;
                }

                if (f.Unsafe.TryGetPointer<ResourceNode>(gatherIntent.TargetNode, out ResourceNode* node) == false)
                {
                    ClearGatherIntent(f, entity, gatherIntent);
                    continue;
                }

                if (node->AmountRemaining <= 0)
                {
                    ClearGatherIntent(f, entity, gatherIntent);
                    continue;
                }

                FP distance = FPVector2.Distance(workerTransform->Position, nodeTransform->Position);
                if (distance > GatherRange)
                {
                    continue;
                }

                int harvestInterval = f.UpdateRate;
                if (harvestInterval <= 0)
                {
                    harvestInterval = FallbackSimulationRate;
                }

                if (f.Number % harvestInterval != 0)
                {
                    continue;
                }

                int harvestAmount = HarvestPerSecond;

                int availableCapacity = carry->Capacity - carry->Amount;
                if (harvestAmount > availableCapacity)
                {
                    harvestAmount = availableCapacity;
                }

                if (harvestAmount > node->AmountRemaining)
                {
                    harvestAmount = node->AmountRemaining;
                }

                if (harvestAmount <= 0)
                {
                    continue;
                }

                carry->ResourceKind = node->ResourceKind;
                carry->Amount += harvestAmount;
                node->AmountRemaining -= harvestAmount;
            }
        }

        private static void ClearGatherIntent(Frame f, EntityRef entity, GatherIntent gatherIntent)
        {
            GatherIntent clearedIntent = gatherIntent;
            clearedIntent.HasTarget = false;
            clearedIntent.TargetNode = EntityRef.None;
            clearedIntent.ResourceKind = ResourceKind.None;
            clearedIntent.TargetWorld = FPVector2.Zero;
            f.Set(entity, clearedIntent);
        }

        private static bool IsDefeatedWorker(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity) == false)
            {
                return false;
            }

            foreach ((EntityRef playerEntity, PlayerEconomyState economyState) in f.GetComponentIterator<PlayerEconomyState>())
            {
                if (economyState.PlayerIndex == unitIdentity->OwnerPlayer)
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
