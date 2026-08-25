namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class SupplyBuildingCollisionSystem : SystemMainThread
    {
        private static readonly FP SupplyBlockRadius = FP.FromString("1.35");
        private static readonly FP UnitClearanceRadius = FP.FromString("0.45");
        private static readonly FP MinimumSeparation = FP.FromString("0.05");
        private static readonly FP SlideBias = FP.FromString("0.65");
        private static readonly FP MaxCorrectionPerTick = FP.FromString("0.12");

        public override void Update(Frame f)
        {
            foreach ((EntityRef unitEntity, UnitIdentity unitIdentity) in f.GetComponentIterator<UnitIdentity>())
            {
                if (f.Unsafe.TryGetPointer<Transform2D>(unitEntity, out Transform2D* unitTransform) == false ||
                    IsDeadUnit(f, unitEntity))
                {
                    continue;
                }

                foreach ((EntityRef supplyEntity, SupplyBuilding supplyBuilding) in f.GetComponentIterator<SupplyBuilding>())
                {
                    if (BlocksMovement(supplyBuilding) == false ||
                        f.Unsafe.TryGetPointer<Transform2D>(supplyEntity, out Transform2D* supplyTransform) == false)
                    {
                        continue;
                    }

                    PushUnitOutOfSupply(f, unitEntity, unitTransform, supplyTransform->Position, unitIdentity.OwnerPlayer);
                }
            }
        }

        private static bool BlocksMovement(SupplyBuilding supplyBuilding)
        {
            return supplyBuilding.Health > 0 &&
                   supplyBuilding.IsConstructing == false &&
                   supplyBuilding.IsDeconstructing == false;
        }

        private static void PushUnitOutOfSupply(Frame f, EntityRef unitEntity, Transform2D* unitTransform, FPVector2 supplyPosition, int ownerPlayer)
        {
            FPVector2 offset = unitTransform->Position - supplyPosition;
            FP distance = FPVector2.Distance(unitTransform->Position, supplyPosition);
            FP blockedDistance = SupplyBlockRadius + UnitClearanceRadius;

            if (distance >= blockedDistance)
            {
                return;
            }

            if (distance <= MinimumSeparation)
            {
                offset = GetFallbackDirection(ownerPlayer);
                distance = FP._1;
            }

            FPVector2 direction = GetSlideDirection(f, unitEntity, unitTransform->Position, supplyPosition, offset / distance);
            FPVector2 desiredPosition = supplyPosition + direction * blockedDistance;
            unitTransform->Position = MoveToward(unitTransform->Position, desiredPosition, MaxCorrectionPerTick);
        }

        private static FPVector2 GetSlideDirection(Frame f, EntityRef unitEntity, FPVector2 unitPosition, FPVector2 supplyPosition, FPVector2 radialDirection)
        {
            if (f.Unsafe.TryGetPointer<NavMeshPathfinder>(unitEntity, out NavMeshPathfinder* pathfinder) == false)
            {
                return radialDirection;
            }

            FPVector2 target = new FPVector2(pathfinder->Target.X, pathfinder->Target.Z);
            FPVector2 toTarget = target - unitPosition;
            FP targetDistance = FPVector2.Distance(unitPosition, target);
            if (targetDistance <= MinimumSeparation)
            {
                return radialDirection;
            }

            FPVector2 targetDirection = toTarget / targetDistance;
            FP side = Cross(radialDirection, targetDirection) >= FP._0 ? FP._1 : -FP._1;
            FPVector2 tangentDirection = new FPVector2(-radialDirection.Y * side, radialDirection.X * side);
            FPVector2 slideDirection = radialDirection + tangentDirection * SlideBias;
            FP slideDistance = FPVector2.Distance(FPVector2.Zero, slideDirection);
            if (slideDistance <= MinimumSeparation)
            {
                return radialDirection;
            }

            return slideDirection / slideDistance;
        }

        private static bool IsDeadUnit(Frame f, EntityRef entity)
        {
            return f.Unsafe.TryGetPointer<UnitHealth>(entity, out UnitHealth* unitHealth) &&
                   unitHealth->IsDead;
        }

        private static FPVector2 GetFallbackDirection(int ownerPlayer)
        {
            if (ownerPlayer == 1)
            {
                return new FPVector2(-FP._1, FP._0);
            }

            if (ownerPlayer == 2)
            {
                return new FPVector2(FP._1, FP._0);
            }

            return new FPVector2(FP._0, -FP._1);
        }

        private static FPVector2 MoveToward(FPVector2 current, FPVector2 target, FP maxDistance)
        {
            FPVector2 delta = target - current;
            FP distance = FPVector2.Distance(current, target);
            if (distance <= maxDistance || distance <= MinimumSeparation)
            {
                return target;
            }

            return current + delta / distance * maxDistance;
        }

        private static FP Cross(FPVector2 a, FPVector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
    }
}
