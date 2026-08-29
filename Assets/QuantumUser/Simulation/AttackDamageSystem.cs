namespace Quantum
{
    public unsafe class AttackDamageSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            foreach ((EntityRef attackerEntity, AttackIntent attackIntent) in f.GetComponentIterator<AttackIntent>())
            {
                if (IsCombatUnit(f, attackerEntity) == false)
                {
                    continue;
                }

                if (IsAttackerDefeated(f, attackerEntity))
                {
                    continue;
                }

                if (attackIntent.HasTarget == false)
                {
                    continue;
                }

                AttackIntent updatedAttackIntent = attackIntent;
                if (updatedAttackIntent.CooldownTicksRemaining > 0)
                {
                    updatedAttackIntent.CooldownTicksRemaining--;
                    f.Set(attackerEntity, updatedAttackIntent);
                    continue;
                }

                if (updatedAttackIntent.IsInRange == false)
                {
                    f.Set(attackerEntity, updatedAttackIntent);
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Targetable>(attackIntent.TargetEntity, out Targetable* targetable) == false ||
                    targetable->Health <= 0)
                {
                    ClearAttackIntent(ref updatedAttackIntent);
                    ClearMoveIntent(f, attackerEntity);
                    f.Set(attackerEntity, updatedAttackIntent);
                    continue;
                }

                targetable->Health -= GetAttackDamage(f, attackerEntity, updatedAttackIntent.Damage);
                if (targetable->Health < 0)
                {
                    targetable->Health = 0;
                }

                if (f.Unsafe.TryGetPointer<MainBuilding>(attackIntent.TargetEntity, out MainBuilding* mainBuilding))
                {
                    mainBuilding->Health = targetable->Health;
                }

                if (f.Unsafe.TryGetPointer<SupplyBuilding>(attackIntent.TargetEntity, out SupplyBuilding* supplyBuilding))
                {
                    supplyBuilding->Health = targetable->Health;
                }

                if (f.Unsafe.TryGetPointer<UnitHealth>(attackIntent.TargetEntity, out UnitHealth* unitHealth))
                {
                    unitHealth->Health = targetable->Health;
                    unitHealth->IsDead = targetable->Health <= 0;
                }

                TrySetRetaliationTarget(f, attackIntent.TargetEntity, attackerEntity);

                updatedAttackIntent.CooldownTicksRemaining = updatedAttackIntent.CooldownTicks;
                if (targetable->Health <= 0)
                {
                    ClearAttackIntent(ref updatedAttackIntent);
                    ClearMoveIntent(f, attackerEntity);
                }

                f.Set(attackerEntity, updatedAttackIntent);
            }
        }

        private static void TrySetRetaliationTarget(Frame f, EntityRef defenderEntity, EntityRef attackerEntity)
        {
            if (f.Unsafe.TryGetPointer<AttackIntent>(defenderEntity, out AttackIntent* defenderAttackIntent) == false)
            {
                return;
            }

            if (IsCombatUnit(f, defenderEntity) == false)
            {
                return;
            }

            if (defenderAttackIntent->HasTarget && HasLiveTarget(f, defenderAttackIntent->TargetEntity))
            {
                return;
            }

            if (f.Unsafe.TryGetPointer<Targetable>(defenderEntity, out Targetable* defenderTargetable) == false ||
                f.Unsafe.TryGetPointer<Targetable>(attackerEntity, out Targetable* attackerTargetable) == false ||
                defenderTargetable->Health <= 0 ||
                attackerTargetable->Health <= 0 ||
                IsPlayerDefeated(f, defenderTargetable->OwnerPlayer) ||
                IsPlayerDefeated(f, attackerTargetable->OwnerPlayer) ||
                defenderTargetable->OwnerPlayer == attackerTargetable->OwnerPlayer)
            {
                return;
            }

            if (f.Unsafe.TryGetPointer<Transform2D>(attackerEntity, out Transform2D* attackerTransform) == false)
            {
                return;
            }

            defenderAttackIntent->HasTarget = true;
            defenderAttackIntent->TargetEntity = attackerEntity;
            defenderAttackIntent->TargetWorld = attackerTransform->Position;
            defenderAttackIntent->IsInRange = false;

            ClearMoveIntent(f, defenderEntity);
            ClearGatherIntent(f, defenderEntity);
        }

        private static bool HasLiveTarget(Frame f, EntityRef targetEntity)
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

        private static bool IsCombatUnit(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity) == false)
            {
                return false;
            }

            return unitIdentity->UnitKind == UnitKind.Hero;
        }

        private static int GetAttackDamage(Frame f, EntityRef attackerEntity, int baseDamage)
        {
            if (IsHoldingGround(f, attackerEntity) == false ||
                f.Unsafe.TryGetPointer<UnitIdentity>(attackerEntity, out UnitIdentity* unitIdentity) == false)
            {
                return baseDamage;
            }

            return baseDamage + FactionStats.ForPlayer(f, unitIdentity->OwnerPlayer).HoldGroundDamageBonus;
        }

        private static bool IsHoldingGround(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<MoveIntent>(entity, out MoveIntent* moveIntent) == false)
            {
                return true;
            }

            return moveIntent->HasTarget == false;
        }

        private static bool IsAttackerDefeated(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity) == false)
            {
                return true;
            }

            return IsPlayerDefeated(f, unitIdentity->OwnerPlayer);
        }

        private static void ClearAttackIntent(ref AttackIntent attackIntent)
        {
            attackIntent.HasTarget = false;
            attackIntent.TargetEntity = EntityRef.None;
            attackIntent.TargetWorld = Photon.Deterministic.FPVector2.Zero;
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
            moveIntent->TargetWorld = Photon.Deterministic.FPVector2.Zero;
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
            gatherIntent->TargetWorld = Photon.Deterministic.FPVector2.Zero;
        }
    }
}
