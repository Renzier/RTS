namespace Quantum
{
    public unsafe class BuildingTierSyncSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            foreach ((EntityRef buildingEntity, MainBuilding mainBuilding) in f.GetComponentIterator<MainBuilding>())
            {
                if (f.Unsafe.TryGetPointer<BuildingTier>(buildingEntity, out BuildingTier* buildingTier) == false)
                {
                    continue;
                }

                int ownerTechTier = GetTechTier(f, mainBuilding.OwnerPlayer);
                if (ownerTechTier <= 0 || buildingTier->Tier == ownerTechTier)
                {
                    continue;
                }

                buildingTier->Tier = ownerTechTier;
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

            return 0;
        }
    }
}
