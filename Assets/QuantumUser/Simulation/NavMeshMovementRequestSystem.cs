namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class NavMeshMovementRequestSystem : SystemMainThread
    {
        private static readonly FP SupplyAvoidRadius = FP.FromString("1.8");
        private static readonly FP SupplyTangentPadding = FP.FromString("0.75");
        private static readonly FP MinimumSegmentLength = FP.FromString("0.05");

        public override void Update(Frame f)
        {
            NavMesh navMesh = GetDefaultNavMesh(f);
            if (navMesh == null)
            {
                SetCommandDebug(f, false, MoveCommandResult.NoNavMesh, FPVector2.Zero);
                return;
            }

            foreach ((EntityRef entity, MoveIntent intent) in f.GetComponentIterator<MoveIntent>())
            {
                if (intent.HasTarget == false)
                {
                    continue;
                }

                if (intent.MovementMode != MovementMode.QuantumNavMesh)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<NavMeshPathfinder>(entity, out NavMeshPathfinder* pathfinder) == false)
                {
                    RejectMoveIntent(f, entity, intent, MoveCommandResult.MissingPathfinder);
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false)
                {
                    RejectMoveIntent(f, entity, intent, MoveCommandResult.MissingTransform);
                    continue;
                }

                FPVector2 adjustedTarget = GetAdjustedTargetOutsideSupplyBuildings(f, intent.TargetWorld);
                FPVector3 target = new FPVector3(adjustedTarget.X, FP._0, adjustedTarget.Y);
                pathfinder->SetTarget(f, target, navMesh);

                MoveIntent updatedIntent = intent;
                updatedIntent.HasTarget = false;
                f.Set(entity, updatedIntent);
                SetCommandDebug(f, true, MoveCommandResult.Accepted, adjustedTarget);
            }
        }

        private static FPVector2 GetAdjustedTargetOutsideSupplyBuildings(Frame f, FPVector2 target)
        {
            foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in f.GetComponentIterator<SupplyBuilding>())
            {
                if (BlocksMovement(supplyBuilding) == false ||
                    f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* supplyTransform) == false)
                {
                    continue;
                }

                FPVector2 supplyPosition = supplyTransform->Position;
                if (FPVector2.Distance(target, supplyPosition) < SupplyAvoidRadius)
                {
                    return PushTargetOutsideSupply(target, supplyPosition);
                }
            }

            return target;
        }

        private static bool BlocksMovement(SupplyBuilding supplyBuilding)
        {
            return supplyBuilding.Health > 0 &&
                   supplyBuilding.IsConstructing == false &&
                   supplyBuilding.IsDeconstructing == false;
        }

        private static FPVector2 PushTargetOutsideSupply(FPVector2 target, FPVector2 supplyPosition)
        {
            FPVector2 offset = target - supplyPosition;
            FP distance = FPVector2.Distance(target, supplyPosition);
            if (distance <= MinimumSegmentLength)
            {
                offset = new FPVector2(FP._0, -FP._1);
                distance = FP._1;
            }

            return supplyPosition + offset / distance * (SupplyAvoidRadius + SupplyTangentPadding);
        }

        private static NavMesh GetDefaultNavMesh(Frame f)
        {
            foreach (NavMesh navMesh in f.Map.NavMeshes.Values)
            {
                return navMesh;
            }

            return null;
        }

        private static void RejectMoveIntent(Frame f, EntityRef entity, MoveIntent intent, int result)
        {
            MoveIntent updatedIntent = intent;
            updatedIntent.HasTarget = false;
            f.Set(entity, updatedIntent);
            SetCommandDebug(f, false, result, intent.TargetWorld);
        }

        private static void SetCommandDebug(Frame f, bool accepted, int result, FPVector2 targetWorld)
        {
            foreach ((EntityRef entity, CommandIntentDebug commandIntentDebug) in f.GetComponentIterator<CommandIntentDebug>())
            {
                CommandIntentDebug updatedDebug = commandIntentDebug;
                updatedDebug.HasMoveCommandIntent = true;
                updatedDebug.WasMoveCommandAccepted = accepted;
                updatedDebug.WasMoveCommandRejected = accepted == false;
                updatedDebug.MoveCommandResult = result;
                updatedDebug.MoveCommandTargetWorld = targetWorld;
                f.Set(entity, updatedDebug);
            }
        }

    }
}
