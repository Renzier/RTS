namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class QuillCaptureSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            if (TryGetQuillObjective(f, out EntityRef quillEntity, out Targetable quillTargetable) == false)
            {
                return;
            }

            int capturingPlayer = GetCapturingPlayer(f);
            if (capturingPlayer == int.MinValue)
            {
                return;
            }

            Targetable updatedTargetable = quillTargetable;
            if (capturingPlayer == QuillObjective.NeutralOwner)
            {
                updatedTargetable.OwnerPlayer = QuillObjective.NeutralOwner;
                updatedTargetable.Health = QuillObjective.CaptureRequired;
                f.Set(quillEntity, updatedTargetable);
                return;
            }

            if (updatedTargetable.OwnerPlayer == capturingPlayer)
            {
                updatedTargetable.Health = QuillObjective.CaptureRequired;
                f.Set(quillEntity, updatedTargetable);
                return;
            }

            updatedTargetable.Health -= QuillObjective.CapturePerUnitTick;
            if (updatedTargetable.Health <= 0)
            {
                updatedTargetable.OwnerPlayer = capturingPlayer;
                updatedTargetable.Health = QuillObjective.CaptureRequired;
            }

            f.Set(quillEntity, updatedTargetable);
        }

        private static int GetCapturingPlayer(Frame f)
        {
            int playerIndex = int.MinValue;
            foreach ((EntityRef entity, UnitIdentity unitIdentity) in f.GetComponentIterator<UnitIdentity>())
            {
                if (unitIdentity.UnitKind != UnitKind.Worker && unitIdentity.UnitKind != UnitKind.Hero)
                {
                    continue;
                }

                if (IsPlayerDefeated(f, unitIdentity.OwnerPlayer) || IsDeadUnit(f, entity))
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false ||
                    FPVector2.Distance(transform->Position, QuillObjective.Position) > QuillObjective.CaptureRadius)
                {
                    continue;
                }

                if (playerIndex == int.MinValue)
                {
                    playerIndex = unitIdentity.OwnerPlayer;
                    continue;
                }

                if (playerIndex != unitIdentity.OwnerPlayer)
                {
                    return QuillObjective.NeutralOwner;
                }
            }

            return playerIndex;
        }

        private static bool TryGetQuillObjective(Frame f, out EntityRef quillEntity, out Targetable quillTargetable)
        {
            foreach ((EntityRef entity, Targetable targetable) in f.GetComponentIterator<Targetable>())
            {
                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false ||
                    QuillObjective.IsObjectivePosition(transform->Position) == false)
                {
                    continue;
                }

                quillEntity = entity;
                quillTargetable = targetable;
                return true;
            }

            quillEntity = EntityRef.None;
            quillTargetable = default;
            return false;
        }

        private static bool IsDeadUnit(Frame f, EntityRef entity)
        {
            return f.Unsafe.TryGetPointer<UnitHealth>(entity, out UnitHealth* unitHealth) && unitHealth->IsDead;
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
