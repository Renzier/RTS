namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class WorkerRepairSystem : SystemMainThread
    {
        private const int RepairIntervalTicks = 30;
        private const int RepairAmount = 10;
        private const int RepairWoodCost = 2;
        private const int RepairIronCost = 1;
        private static readonly FP RepairRange = FP.FromString("2.75");
        private static readonly FP RepairWorkOffset = FP.FromString("1.85");

        public override void Update(Frame f)
        {
            if (f.Number % RepairIntervalTicks != 0)
            {
                return;
            }

            foreach ((EntityRef workerEntity, WorkerBuildIntent workIntent) in f.GetComponentIterator<WorkerBuildIntent>())
            {
                if (workIntent.IsBuilding == false || workIntent.TargetBuilding == EntityRef.None)
                {
                    continue;
                }

                if (IsConstructionTarget(f, workIntent.TargetBuilding))
                {
                    continue;
                }

                if (IsLiveWorker(f, workerEntity) == false ||
                    TryRepairTarget(f, workIntent.TargetBuilding, out int ownerPlayer) == false)
                {
                    ReleaseWorker(f, workerEntity, workIntent);
                    continue;
                }

                if (IsWorkerInRepairRange(f, workerEntity, workIntent.TargetBuilding) == false)
                {
                    continue;
                }

                if (TrySpendRepairCost(f, ownerPlayer) == false)
                {
                    continue;
                }

                RepairTarget(f, workIntent.TargetBuilding);
                if (IsFullyRepaired(f, workIntent.TargetBuilding))
                {
                    ReleaseWorker(f, workerEntity, workIntent);
                }
            }
        }

        public static bool TryAssignSelectedWorkersToRepair(Frame f, int playerIndex, EntityRef targetEntity)
        {
            if (IsRepairableTarget(f, targetEntity, playerIndex) == false ||
                f.Unsafe.TryGetPointer<Transform2D>(targetEntity, out Transform2D* targetTransform) == false)
            {
                return false;
            }

            bool assignedAny = false;
            int selectedWorkerIndex = 0;
            foreach ((EntityRef entity, Selectable selectable) in f.GetComponentIterator<Selectable>())
            {
                if (selectable.IsSelected == false || CanAssignWorker(f, entity, playerIndex, targetEntity) == false)
                {
                    continue;
                }

                AssignRepairWorker(f, entity, targetEntity, targetTransform->Position + GetWorkerFormationOffset(selectedWorkerIndex));
                selectedWorkerIndex++;
                assignedAny = true;
            }

            return assignedAny;
        }

        private static bool TryRepairTarget(Frame f, EntityRef targetEntity, out int ownerPlayer)
        {
            if (f.Unsafe.TryGetPointer<MainBuilding>(targetEntity, out MainBuilding* mainBuilding))
            {
                ownerPlayer = mainBuilding->OwnerPlayer;
                return mainBuilding->Health > 0 && mainBuilding->Health < mainBuilding->MaxHealth;
            }

            if (f.Unsafe.TryGetPointer<SupplyBuilding>(targetEntity, out SupplyBuilding* supplyBuilding))
            {
                ownerPlayer = supplyBuilding->OwnerPlayer;
                return supplyBuilding->Health > 0 &&
                       supplyBuilding->Health < supplyBuilding->MaxHealth &&
                       supplyBuilding->IsConstructing == false &&
                       supplyBuilding->IsDeconstructing == false;
            }

            ownerPlayer = -1;
            return false;
        }

        private static bool IsRepairableTarget(Frame f, EntityRef targetEntity, int playerIndex)
        {
            return TryRepairTarget(f, targetEntity, out int ownerPlayer) && ownerPlayer == playerIndex;
        }

        private static bool IsWorkerInRepairRange(Frame f, EntityRef workerEntity, EntityRef targetEntity)
        {
            if (f.Unsafe.TryGetPointer<Transform2D>(workerEntity, out Transform2D* workerTransform) == false ||
                f.Unsafe.TryGetPointer<Transform2D>(targetEntity, out Transform2D* targetTransform) == false)
            {
                return false;
            }

            return FPVector2.Distance(workerTransform->Position, targetTransform->Position) <= RepairRange;
        }

        private static bool IsConstructionTarget(Frame f, EntityRef targetEntity)
        {
            return f.Unsafe.TryGetPointer<SupplyBuilding>(targetEntity, out SupplyBuilding* supplyBuilding) &&
                   supplyBuilding->Health > 0 &&
                   supplyBuilding->IsConstructing;
        }

        private static void RepairTarget(Frame f, EntityRef targetEntity)
        {
            if (f.Unsafe.TryGetPointer<MainBuilding>(targetEntity, out MainBuilding* mainBuilding))
            {
                mainBuilding->Health = ClampRepair(mainBuilding->Health, mainBuilding->MaxHealth);
                SyncTargetableHealth(f, targetEntity, mainBuilding->Health, mainBuilding->MaxHealth);
                return;
            }

            if (f.Unsafe.TryGetPointer<SupplyBuilding>(targetEntity, out SupplyBuilding* supplyBuilding))
            {
                supplyBuilding->Health = ClampRepair(supplyBuilding->Health, supplyBuilding->MaxHealth);
                SyncTargetableHealth(f, targetEntity, supplyBuilding->Health, supplyBuilding->MaxHealth);
            }
        }

        private static bool IsFullyRepaired(Frame f, EntityRef targetEntity)
        {
            if (f.Unsafe.TryGetPointer<MainBuilding>(targetEntity, out MainBuilding* mainBuilding))
            {
                return mainBuilding->Health >= mainBuilding->MaxHealth;
            }

            if (f.Unsafe.TryGetPointer<SupplyBuilding>(targetEntity, out SupplyBuilding* supplyBuilding))
            {
                return supplyBuilding->Health >= supplyBuilding->MaxHealth;
            }

            return true;
        }

        private static int ClampRepair(int health, int maxHealth)
        {
            health += RepairAmount;
            return health > maxHealth ? maxHealth : health;
        }

        private static bool TrySpendRepairCost(Frame f, int playerIndex)
        {
            foreach ((EntityRef entity, PlayerEconomyState economyState) in f.GetComponentIterator<PlayerEconomyState>())
            {
                if (economyState.PlayerIndex != playerIndex)
                {
                    continue;
                }

                if (economyState.IsDefeated ||
                    economyState.Wood < RepairWoodCost ||
                    economyState.Iron < RepairIronCost)
                {
                    return false;
                }

                PlayerEconomyState updatedEconomy = economyState;
                updatedEconomy.Wood -= RepairWoodCost;
                updatedEconomy.Iron -= RepairIronCost;
                f.Set(entity, updatedEconomy);
                return true;
            }

            return false;
        }

        private static bool CanAssignWorker(Frame f, EntityRef entity, int playerIndex, EntityRef targetEntity)
        {
            if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity) == false ||
                unitIdentity->OwnerPlayer != playerIndex ||
                unitIdentity->UnitKind != UnitKind.Worker)
            {
                return false;
            }

            if (f.Unsafe.TryGetPointer<UnitHealth>(entity, out UnitHealth* unitHealth) && unitHealth->IsDead)
            {
                return false;
            }

            if (f.Unsafe.TryGetPointer<WorkerBuildIntent>(entity, out WorkerBuildIntent* workIntent) &&
                workIntent->IsBuilding &&
                workIntent->TargetBuilding == targetEntity)
            {
                return false;
            }

            return true;
        }

        private static bool IsLiveWorker(Frame f, EntityRef workerEntity)
        {
            if (f.Unsafe.TryGetPointer<UnitIdentity>(workerEntity, out UnitIdentity* unitIdentity) == false ||
                unitIdentity->UnitKind != UnitKind.Worker)
            {
                return false;
            }

            return f.Unsafe.TryGetPointer<UnitHealth>(workerEntity, out UnitHealth* unitHealth) == false ||
                   unitHealth->IsDead == false;
        }

        private static void AssignRepairWorker(Frame f, EntityRef workerEntity, EntityRef targetEntity, FPVector2 targetPosition)
        {
            f.Set(workerEntity, new WorkerBuildIntent
            {
                IsBuilding = true,
                TargetBuilding = targetEntity
            });

            if (f.Unsafe.TryGetPointer<GatherIntent>(workerEntity, out GatherIntent* gatherIntent))
            {
                gatherIntent->HasTarget = false;
                gatherIntent->TargetNode = EntityRef.None;
                gatherIntent->ResourceKind = ResourceKind.None;
                gatherIntent->TargetWorld = FPVector2.Zero;
            }

            if (f.Unsafe.TryGetPointer<AttackIntent>(workerEntity, out AttackIntent* attackIntent))
            {
                attackIntent->HasTarget = false;
                attackIntent->TargetEntity = EntityRef.None;
                attackIntent->TargetWorld = FPVector2.Zero;
                attackIntent->IsInRange = false;
                attackIntent->CooldownTicksRemaining = 0;
            }

            if (f.Unsafe.TryGetPointer<MoveIntent>(workerEntity, out MoveIntent* moveIntent))
            {
                moveIntent->HasTarget = true;
                moveIntent->MovementMode = MovementMode.QuantumNavMesh;
                moveIntent->TargetWorld = targetPosition + new FPVector2(-RepairWorkOffset, -RepairWorkOffset);
            }
        }

        private static void ReleaseWorker(Frame f, EntityRef workerEntity, WorkerBuildIntent workIntent)
        {
            WorkerBuildIntent updatedIntent = workIntent;
            updatedIntent.IsBuilding = false;
            updatedIntent.TargetBuilding = EntityRef.None;
            f.Set(workerEntity, updatedIntent);

            if (f.Unsafe.TryGetPointer<MoveIntent>(workerEntity, out MoveIntent* moveIntent))
            {
                moveIntent->HasTarget = false;
                moveIntent->MovementMode = MovementMode.StraightLineFallback;
                moveIntent->TargetWorld = FPVector2.Zero;
            }
        }

        private static void SyncTargetableHealth(Frame f, EntityRef entity, int health, int maxHealth)
        {
            if (f.Unsafe.TryGetPointer<Targetable>(entity, out Targetable* targetable) == false)
            {
                return;
            }

            targetable->Health = health;
            targetable->MaxHealth = maxHealth;
        }

        private static FPVector2 GetWorkerFormationOffset(int selectedWorkerIndex)
        {
            FP spacing = FP.FromString("0.65");
            if (selectedWorkerIndex == 1)
            {
                return new FPVector2(spacing, FP._0);
            }

            if (selectedWorkerIndex == 2)
            {
                return new FPVector2(FP._0, spacing);
            }

            if (selectedWorkerIndex == 3)
            {
                return new FPVector2(-spacing, FP._0);
            }

            if (selectedWorkerIndex == 4)
            {
                return new FPVector2(FP._0, -spacing);
            }

            return FPVector2.Zero;
        }
    }
}
