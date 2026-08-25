namespace Quantum
{
    public unsafe class HeroTechCombatStatsSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            foreach ((EntityRef entity, UnitIdentity unitIdentity) in f.GetComponentIterator<UnitIdentity>())
            {
                if (unitIdentity.UnitKind != UnitKind.Hero)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<AttackIntent>(entity, out AttackIntent* attackIntent) == false)
                {
                    continue;
                }

                int techTier = GetTechTier(f, unitIdentity.OwnerPlayer);
                int damage = FactionStats.ForPlayer(f, unitIdentity.OwnerPlayer).HeroDamageForTier(techTier);
                if (attackIntent->Damage == damage)
                {
                    continue;
                }

                attackIntent->Damage = damage;
            }
        }

        private static int GetTechTier(Frame f, int ownerPlayer)
        {
            foreach ((EntityRef entity, PlayerTechState techState) in f.GetComponentIterator<PlayerTechState>())
            {
                if (techState.PlayerIndex == ownerPlayer)
                {
                    return techState.TechTier;
                }
            }

            return 1;
        }
    }
}
