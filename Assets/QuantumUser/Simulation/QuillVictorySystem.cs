namespace Quantum
{
    public unsafe class QuillVictorySystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            if (QuillObjective.VictoryEnabled == false ||
                TryGetQuillObjective(f, out EntityRef quillEntity, out Targetable quillTargetable) == false ||
                quillTargetable.OwnerPlayer == QuillObjective.NeutralOwner ||
                IsPlayerDefeated(f, quillTargetable.OwnerPlayer))
            {
                return;
            }

            Targetable updatedTargetable = quillTargetable;
            if (updatedTargetable.MaxHealth != QuillObjective.VictoryHoldTicks)
            {
                updatedTargetable.MaxHealth = QuillObjective.VictoryHoldTicks;
                updatedTargetable.Health = QuillObjective.VictoryHoldTicks;
            }

            if (updatedTargetable.Health > 0)
            {
                updatedTargetable.Health--;
            }

            if (updatedTargetable.Health <= 0)
            {
                DefeatOpponents(f, updatedTargetable.OwnerPlayer);
                updatedTargetable.Health = 0;
            }

            f.Set(quillEntity, updatedTargetable);
        }

        private static void DefeatOpponents(Frame f, int winningPlayer)
        {
            foreach ((EntityRef entity, PlayerEconomyState economyState) in f.GetComponentIterator<PlayerEconomyState>())
            {
                if (economyState.PlayerIndex == winningPlayer || economyState.IsDefeated)
                {
                    continue;
                }

                PlayerEconomyState updatedEconomy = economyState;
                updatedEconomy.IsDefeated = true;
                f.Set(entity, updatedEconomy);
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
