using Photon.Deterministic;

namespace Quantum
{
    public unsafe class StraightLineMovementSystem : SystemMainThread
    {
        private static readonly FP StopDistance = FP.FromString("0.05");
        private static readonly FP MoveSpeed = FP._2;

        public override void Update(Frame f)
        {
            foreach ((EntityRef entity, MoveIntent intent) in f.GetComponentIterator<MoveIntent>())
            {
                if (intent.HasTarget == false)
                {
                    continue;
                }

                if (intent.MovementMode != MovementMode.StraightLineFallback)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false)
                {
                    continue;
                }

                FPVector2 toTarget = intent.TargetWorld - transform->Position;
                FP distance = FPVector2.Distance(transform->Position, intent.TargetWorld);
                if (distance <= StopDistance)
                {
                    MoveIntent updatedIntent = intent;
                    updatedIntent.HasTarget = false;
                    f.Set(entity, updatedIntent);
                    transform->Position = intent.TargetWorld;
                    continue;
                }

                FP step = MoveSpeed * f.DeltaTime;
                if (step >= distance)
                {
                    transform->Position = intent.TargetWorld;

                    MoveIntent updatedIntent = intent;
                    updatedIntent.HasTarget = false;
                    f.Set(entity, updatedIntent);
                    continue;
                }

                FPVector2 direction = toTarget / distance;
                transform->Position += direction * step;
            }
        }
    }
}
