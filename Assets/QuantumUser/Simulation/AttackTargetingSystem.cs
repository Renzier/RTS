namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class AttackTargetingSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            foreach ((EntityRef entity, AttackIntent attackIntent) in f.GetComponentIterator<AttackIntent>())
            {
                if (IsCombatUnit(f, entity) == false)
                {
                    if (attackIntent.HasTarget)
                    {
                        AttackIntent clearedAttackIntent = attackIntent;
                        ClearAttackIntent(ref clearedAttackIntent);
                        f.Set(entity, clearedAttackIntent);
                    }

                    continue;
                }

                if (attackIntent.HasTarget == false)
                {
                    continue;
                }

                AttackIntent updatedAttackIntent = attackIntent;
                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* attackerTransform) == false ||
                    f.Unsafe.TryGetPointer<Transform2D>(attackIntent.TargetEntity, out Transform2D* targetTransform) == false ||
                    f.Unsafe.TryGetPointer<Targetable>(attackIntent.TargetEntity, out Targetable* targetable) == false ||
                    targetable->Health <= 0)
                {
                    ClearAttackIntent(ref updatedAttackIntent);
                    ClearMoveIntent(f, entity);
                    f.Set(entity, updatedAttackIntent);
                    continue;
                }

                FP distance = FPVector2.Distance(attackerTransform->Position, targetTransform->Position);
                updatedAttackIntent.TargetWorld = targetTransform->Position;
                updatedAttackIntent.IsInRange = distance <= attackIntent.AttackRange + targetable->TargetRadius;
                if (updatedAttackIntent.IsInRange)
                {
                    ClearMoveIntent(f, entity);
                }
                else
                {
                    FPVector2 approachPosition = GetAttackApproachPosition(
                        attackerTransform->Position,
                        targetTransform->Position,
                        attackIntent.AttackRange,
                        targetable->TargetRadius);
                    ContinueChasingTarget(f, entity, approachPosition);
                }

                f.Set(entity, updatedAttackIntent);
            }
        }

        private static void ClearAttackIntent(ref AttackIntent attackIntent)
        {
            attackIntent.HasTarget = false;
            attackIntent.TargetEntity = EntityRef.None;
            attackIntent.TargetWorld = FPVector2.Zero;
            attackIntent.IsInRange = false;
            attackIntent.CooldownTicksRemaining = 0;
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

        private static void ContinueChasingTarget(Frame f, EntityRef entity, FPVector2 targetPosition)
        {
            if (f.Unsafe.TryGetPointer<MoveIntent>(entity, out MoveIntent* moveIntent) == false)
            {
                return;
            }

            moveIntent->HasTarget = true;
            moveIntent->MovementMode = MovementMode.StraightLineFallback;
            moveIntent->TargetWorld = targetPosition;
        }

        private static FPVector2 GetAttackApproachPosition(FPVector2 attackerPosition, FPVector2 targetPosition, FP attackRange, FP targetRadius)
        {
            FPVector2 awayFromTarget = attackerPosition - targetPosition;
            FP distance = FPVector2.Distance(attackerPosition, targetPosition);
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

        private static bool IsCombatUnit(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity) == false)
            {
                return false;
            }

            return unitIdentity->UnitKind == UnitKind.Hero;
        }
    }
}
