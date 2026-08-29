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

            GrantOwnershipBonus(f, quillTargetable.OwnerPlayer);

            AreaControl areaControl = GetAreaControl(f);
            if (areaControl.HasAnyPlayer == false)
            {
                return;
            }

            Targetable updatedTargetable = quillTargetable;
            if (areaControl.IsContested)
            {
                if (updatedTargetable.OwnerPlayer == QuillObjective.NeutralOwner ||
                    areaControl.HasPlayer(updatedTargetable.OwnerPlayer) == false)
                {
                    updatedTargetable.OwnerPlayer = QuillObjective.NeutralOwner;
                    updatedTargetable.Health = QuillObjective.CaptureRequired;
                    updatedTargetable.MaxHealth = QuillObjective.CaptureRequired;
                }

                f.Set(quillEntity, updatedTargetable);
                return;
            }

            if (updatedTargetable.OwnerPlayer == areaControl.SinglePlayer)
            {
                f.Set(quillEntity, updatedTargetable);
                return;
            }

            if (updatedTargetable.MaxHealth != QuillObjective.CaptureRequired)
            {
                updatedTargetable.Health = QuillObjective.CaptureRequired;
                updatedTargetable.MaxHealth = QuillObjective.CaptureRequired;
            }

            updatedTargetable.Health -= QuillObjective.CapturePerUnitTick;
            if (updatedTargetable.Health <= 0)
            {
                updatedTargetable.OwnerPlayer = areaControl.SinglePlayer;
                updatedTargetable.Health = QuillObjective.VictoryHoldTicks;
                updatedTargetable.MaxHealth = QuillObjective.VictoryHoldTicks;
            }

            f.Set(quillEntity, updatedTargetable);
        }

        private static void GrantOwnershipBonus(Frame f, int ownerPlayer)
        {
            if (ownerPlayer == QuillObjective.NeutralOwner ||
                f.Number % QuillObjective.ResourceTrickleIntervalTicks != 0)
            {
                return;
            }

            foreach ((EntityRef entity, PlayerEconomyState economyState) in f.GetComponentIterator<PlayerEconomyState>())
            {
                if (economyState.PlayerIndex != ownerPlayer || economyState.IsDefeated)
                {
                    continue;
                }

                PlayerEconomyState updatedEconomy = economyState;
                updatedEconomy.Wood += QuillObjective.ResourceTrickleWood;
                updatedEconomy.Iron += QuillObjective.ResourceTrickleIron;
                f.Set(entity, updatedEconomy);
                return;
            }
        }

        private static AreaControl GetAreaControl(Frame f)
        {
            AreaControl areaControl = default;
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

                areaControl.AddPlayer(unitIdentity.OwnerPlayer);
            }

            return areaControl;
        }

        private struct AreaControl
        {
            public bool HasAnyPlayer;
            public bool IsContested;
            public int SinglePlayer;
            private int _playerMask;

            public void AddPlayer(int playerIndex)
            {
                if (HasAnyPlayer == false)
                {
                    HasAnyPlayer = true;
                    SinglePlayer = playerIndex;
                    _playerMask = 1 << playerIndex;
                    return;
                }

                int playerBit = 1 << playerIndex;
                if ((_playerMask & playerBit) != 0)
                {
                    return;
                }

                IsContested = true;
                SinglePlayer = int.MinValue;
                _playerMask |= playerBit;
            }

            public bool HasPlayer(int playerIndex)
            {
                if (HasAnyPlayer == false)
                {
                    return false;
                }

                return (_playerMask & (1 << playerIndex)) != 0;
            }
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
