namespace Quantum
{
    using System.Collections.Generic;

    public static partial class DeterministicSystemSetup
    {
        static partial void AddSystemsUser(ICollection<SystemBase> systems, RuntimeConfig gameConfig, SimulationConfig simulationConfig, SystemsConfig systemsConfig)
        {
            systems.Add(new TestSelectableBootstrapSystem());
            systems.Add(new SelectionIntentSystem());
            systems.Add(new SelectionSystem());
            systems.Add(new MoveCommandIntentSystem());
            systems.Add(new StraightLineMovementSystem());
            systems.Add(new WorkerDepositSystem());
            systems.Add(new AttackTargetingSystem());
            systems.Add(new NavMeshMovementRequestSystem());
            systems.Add(new WorkerGatherSystem());
            systems.Add(new AttackDamageSystem());
            systems.Add(new UnitDeathSystem());
            systems.Add(new MainBaseDefeatSystem());
            systems.Add(new MainBuildingDestroyedCleanupSystem());
            systems.Add(new DefeatedPlayerCleanupSystem());
            systems.Add(new SupplyBuildingConstructionSystem());
            systems.Add(new WorkerProductionSystem());
            systems.Add(new TechUpgradeSystem());
            systems.Add(new BuildingTierSyncSystem());
            systems.Add(new PrototypeDebugDamageSystem());
            systems.Add(new ArdentConcordRepairSystem());
            systems.Add(new HeroTechCombatStatsSystem());
            systems.Add(new HeroLifecycleSystem());
            systems.Add(new HeroRebuildSystem());
            systems.Add(new SupplyBuildingCollisionSystem());
        }
    }
}
