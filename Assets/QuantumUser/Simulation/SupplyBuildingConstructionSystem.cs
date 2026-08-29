namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class SupplyBuildingConstructionSystem : SystemMainThread
    {
        private const int BuildSupplyIntent = 4;
        private const int DeconstructSupplyIntent = 5;
        private const int SupplyBuildTicks = 600;
        private const int SupplyDeconstructTicks = 300;
        private const int ConstructionCancelRefundPercent = 100;
        private const int CompletedDeconstructRefundPercent = 80;
        private static readonly FP BuildRange = FP.FromString("5.0");
        private static readonly FP PlacementRadius = FP.FromString("1.35");
        private static readonly FP SupplyObstacleRadius = FP.FromString("1.35");
        private static readonly FP BuilderWorkOffset = FP.FromString("1.85");
        private static readonly FP UnitBlockRadius = FP.FromString("0.85");
        private static readonly FP MapHalfExtent = FP.FromString("38.0");

        public override void Update(Frame f)
        {
            TickSupplyConstruction(f);

            if (f.Global->LastUpgradeIntent == DeconstructSupplyIntent)
            {
                f.Global->LastUpgradeIntent = 0;
                DeconstructSelectedSupply(f, f.Global->LastInputPlayer);
                return;
            }

            if (f.Global->LastUpgradeIntent != BuildSupplyIntent)
            {
                return;
            }

            f.Global->LastUpgradeIntent = 0;
            int playerIndex = f.Global->LastInputPlayer;
            FactionStats stats = FactionStats.ForPlayer(f, playerIndex);

            if (TryGetEconomyState(f, playerIndex, out EntityRef economyEntity, out PlayerEconomyState economyState) == false ||
                economyState.IsDefeated ||
                economyState.Wood < stats.SupplyBuildingWoodCost ||
                economyState.Iron < stats.SupplyBuildingIronCost)
            {
                return;
            }

            if (TryGetSelectedWorkerNearBuildPoint(f, playerIndex, f.Global->LastPointerWorld, out EntityRef builderEntity) == false ||
                IsValidSupplyPlacement(f, f.Global->LastPointerWorld) == false)
            {
                return;
            }

            PlayerEconomyState updatedEconomy = economyState;
            updatedEconomy.Wood -= stats.SupplyBuildingWoodCost;
            updatedEconomy.Iron -= stats.SupplyBuildingIronCost;
            f.Set(economyEntity, updatedEconomy);

            EntityRef supplyEntity = CreateSupplyBuilding(f, playerIndex, f.Global->LastPointerWorld);
            AssignBuilder(f, builderEntity, supplyEntity, f.Global->LastPointerWorld);
        }

        private static void TickSupplyConstruction(Frame f)
        {
            foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in f.GetComponentIterator<SupplyBuilding>())
            {
                if (supplyBuilding.Health <= 0)
                {
                    RemoveSupplyNavigationObstacle(f, entity);
                    RefundDestroyedConstruction(f, entity, supplyBuilding);
                    ReleaseBuildersForCompletedOrDestroyedSupply(f, entity);
                    continue;
                }

                if (supplyBuilding.IsDeconstructing)
                {
                    RemoveSupplyNavigationObstacle(f, entity);
                    TickSupplyDeconstruction(f, entity, supplyBuilding);
                    continue;
                }

                if (supplyBuilding.IsConstructing == false)
                {
                    EnsureSupplyNavigationObstacle(f, entity);
                    continue;
                }

                SupplyBuilding updatedSupply = supplyBuilding;
                if (updatedSupply.BuildTicksRemaining > 0)
                {
                    updatedSupply.BuildTicksRemaining -= GetActiveBuilderCount(f, entity);
                    if (updatedSupply.BuildTicksRemaining < 0)
                    {
                        updatedSupply.BuildTicksRemaining = 0;
                    }
                }

                if (updatedSupply.BuildTicksRemaining <= 0)
                {
                    updatedSupply.IsConstructing = false;
                    updatedSupply.Health = updatedSupply.MaxHealth;

                    if (updatedSupply.HasGrantedFood == false &&
                        TryGetEconomyState(f, updatedSupply.OwnerPlayer, out EntityRef economyEntity, out PlayerEconomyState economyState))
                    {
                        PlayerEconomyState updatedEconomy = economyState;
                        updatedEconomy.FoodCap += updatedSupply.FoodProvided;
                        f.Set(economyEntity, updatedEconomy);
                        updatedSupply.HasGrantedFood = true;
                    }

                    ReleaseBuildersForCompletedOrDestroyedSupply(f, entity);
                }

                f.Set(entity, updatedSupply);
                SyncSupplyNavigationObstacle(f, entity, updatedSupply);

                if (f.Unsafe.TryGetPointer<Targetable>(entity, out Targetable* targetable))
                {
                    targetable->Health = updatedSupply.Health;
                    targetable->MaxHealth = updatedSupply.MaxHealth;
                }
            }
        }

        private static EntityRef CreateSupplyBuilding(Frame f, int ownerPlayer, FPVector2 position)
        {
            EntityRef entity = f.Create();
            FactionStats stats = FactionStats.ForPlayer(f, ownerPlayer);

            f.Set(entity, new Transform2D
            {
                Position = position,
                Rotation = FP._0
            });

            f.Set(entity, new SupplyBuilding
            {
                OwnerPlayer = ownerPlayer,
                FoodProvided = stats.SupplyBuildingFoodProvided,
                Health = 1,
                MaxHealth = stats.SupplyBuildingMaxHealth,
                IsConstructing = true,
                HasGrantedFood = false,
                BuildTicksRemaining = SupplyBuildTicks,
                BuildTicksTotal = SupplyBuildTicks,
                WoodCost = stats.SupplyBuildingWoodCost,
                IronCost = stats.SupplyBuildingIronCost,
                HasRefunded = false,
                IsDeconstructing = false,
                DeconstructTicksRemaining = 0,
                DeconstructTicksTotal = 0
            });

            f.Set(entity, new Targetable
            {
                OwnerPlayer = ownerPlayer,
                Health = 1,
                MaxHealth = stats.SupplyBuildingMaxHealth,
                TargetRadius = FP.FromString("1.0")
            });

            f.Set(entity, new Selectable
            {
                IsSelected = false,
                SelectionRadius = FP.FromString("1.15")
            });

            f.Set(entity, new SelectionCandidate
            {
                ScreenPosition = FPVector2.Zero
            });

            return entity;
        }

        private static void DeconstructSelectedSupply(Frame f, int playerIndex)
        {
            if (TryGetSupplyFromSelectedBuilder(f, playerIndex, out EntityRef builderSupplyEntity))
            {
                DeconstructSupply(f, builderSupplyEntity, playerIndex);
                return;
            }

            foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in f.GetComponentIterator<SupplyBuilding>())
            {
                if (supplyBuilding.OwnerPlayer != playerIndex || IsSelected(f, entity) == false)
                {
                    continue;
                }

                DeconstructSupply(f, entity, playerIndex);
                return;
            }
        }

        private static bool TryGetSupplyFromSelectedBuilder(Frame f, int playerIndex, out EntityRef supplyEntity)
        {
            foreach ((EntityRef entity, WorkerBuildIntent buildIntent) in f.GetComponentIterator<WorkerBuildIntent>())
            {
                if (buildIntent.IsBuilding == false || buildIntent.TargetBuilding == EntityRef.None || IsSelected(f, entity) == false)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity) == false ||
                    unitIdentity->OwnerPlayer != playerIndex ||
                    unitIdentity->UnitKind != UnitKind.Worker)
                {
                    continue;
                }

                supplyEntity = buildIntent.TargetBuilding;
                return true;
            }

            supplyEntity = EntityRef.None;
            return false;
        }

        private static void DeconstructSupply(Frame f, EntityRef supplyEntity, int playerIndex)
        {
            if (f.Unsafe.TryGetPointer<SupplyBuilding>(supplyEntity, out SupplyBuilding* supplyBuilding) == false ||
                supplyBuilding->OwnerPlayer != playerIndex ||
                supplyBuilding->Health <= 0)
            {
                return;
            }

            if (supplyBuilding->IsConstructing)
            {
                RemoveSupplyNavigationObstacle(f, supplyEntity);
                RefundSupplyCost(f, *supplyBuilding, ConstructionCancelRefundPercent);
                ReleaseBuildersForCompletedOrDestroyedSupply(f, supplyEntity);
                f.Destroy(supplyEntity);
                return;
            }

            if (supplyBuilding->IsDeconstructing)
            {
                return;
            }

            supplyBuilding->IsDeconstructing = true;
            supplyBuilding->DeconstructTicksRemaining = SupplyDeconstructTicks;
            supplyBuilding->DeconstructTicksTotal = SupplyDeconstructTicks;
            supplyBuilding->HasRefunded = false;
            RemoveSupplyNavigationObstacle(f, supplyEntity);
        }

        private static void TickSupplyDeconstruction(Frame f, EntityRef entity, SupplyBuilding supplyBuilding)
        {
            SupplyBuilding updatedSupply = supplyBuilding;
            if (updatedSupply.DeconstructTicksRemaining > 0)
            {
                updatedSupply.DeconstructTicksRemaining--;
            }

            if (updatedSupply.DeconstructTicksRemaining > 0)
            {
                f.Set(entity, updatedSupply);
                return;
            }

            if (updatedSupply.HasRefunded == false)
            {
                RefundSupplyCost(f, updatedSupply, CompletedDeconstructRefundPercent);
                updatedSupply.HasRefunded = true;
            }

            if (updatedSupply.HasGrantedFood)
            {
                RemoveGrantedFood(f, updatedSupply.OwnerPlayer, updatedSupply.FoodProvided);
            }

            ReleaseBuildersForCompletedOrDestroyedSupply(f, entity);
            RemoveSupplyNavigationObstacle(f, entity);
            f.Destroy(entity);
        }

        private static void SyncSupplyNavigationObstacle(Frame f, EntityRef entity, SupplyBuilding supplyBuilding)
        {
            if (supplyBuilding.Health > 0 &&
                supplyBuilding.IsConstructing == false &&
                supplyBuilding.IsDeconstructing == false)
            {
                EnsureSupplyNavigationObstacle(f, entity);
                return;
            }

            RemoveSupplyNavigationObstacle(f, entity);
        }

        private static void EnsureSupplyNavigationObstacle(Frame f, EntityRef entity)
        {
            if (f.Has<NavMeshAvoidanceObstacle>(entity))
            {
                return;
            }

            f.Set(entity, new NavMeshAvoidanceObstacle
            {
                Radius = SupplyObstacleRadius,
                Velocity = FPVector2.Zero
            });
        }

        private static void RemoveSupplyNavigationObstacle(Frame f, EntityRef entity)
        {
            if (f.Has<NavMeshAvoidanceObstacle>(entity) == false)
            {
                return;
            }

            f.Remove<NavMeshAvoidanceObstacle>(entity);
        }

        private static void RefundDestroyedConstruction(Frame f, EntityRef supplyEntity, SupplyBuilding supplyBuilding)
        {
            if (supplyBuilding.IsConstructing == false || supplyBuilding.HasRefunded)
            {
                return;
            }

            RefundSupplyCost(f, supplyBuilding, ConstructionCancelRefundPercent);

            SupplyBuilding updatedSupply = supplyBuilding;
            updatedSupply.HasRefunded = true;
            f.Set(supplyEntity, updatedSupply);
        }

        private static void RefundSupplyCost(Frame f, SupplyBuilding supplyBuilding, int refundPercent)
        {
            if (TryGetEconomyState(f, supplyBuilding.OwnerPlayer, out EntityRef economyEntity, out PlayerEconomyState economyState) == false)
            {
                return;
            }

            PlayerEconomyState updatedEconomy = economyState;
            updatedEconomy.Wood += supplyBuilding.WoodCost * refundPercent / 100;
            updatedEconomy.Iron += supplyBuilding.IronCost * refundPercent / 100;
            f.Set(economyEntity, updatedEconomy);
        }

        private static void RemoveGrantedFood(Frame f, int ownerPlayer, int foodProvided)
        {
            if (TryGetEconomyState(f, ownerPlayer, out EntityRef economyEntity, out PlayerEconomyState economyState) == false)
            {
                return;
            }

            PlayerEconomyState updatedEconomy = economyState;
            updatedEconomy.FoodCap -= foodProvided;
            if (updatedEconomy.FoodCap < 0)
            {
                updatedEconomy.FoodCap = 0;
            }

            f.Set(economyEntity, updatedEconomy);
        }

        private static void AssignBuilder(Frame f, EntityRef builderEntity, EntityRef supplyEntity, FPVector2 buildPoint)
        {
            f.Set(builderEntity, new WorkerBuildIntent
            {
                IsBuilding = true,
                TargetBuilding = supplyEntity
            });

            if (f.Unsafe.TryGetPointer<GatherIntent>(builderEntity, out GatherIntent* gatherIntent))
            {
                gatherIntent->HasTarget = false;
                gatherIntent->TargetNode = EntityRef.None;
                gatherIntent->ResourceKind = ResourceKind.None;
                gatherIntent->TargetWorld = FPVector2.Zero;
            }

            if (f.Unsafe.TryGetPointer<AttackIntent>(builderEntity, out AttackIntent* attackIntent))
            {
                attackIntent->HasTarget = false;
                attackIntent->TargetEntity = EntityRef.None;
                attackIntent->TargetWorld = FPVector2.Zero;
                attackIntent->IsInRange = false;
                attackIntent->CooldownTicksRemaining = 0;
            }

            if (f.Unsafe.TryGetPointer<MoveIntent>(builderEntity, out MoveIntent* moveIntent))
            {
                moveIntent->HasTarget = true;
                moveIntent->MovementMode = MovementMode.QuantumNavMesh;
                moveIntent->TargetWorld = buildPoint + new FPVector2(-BuilderWorkOffset, -BuilderWorkOffset);
            }
        }

        public static bool TryAssignSelectedWorkersToConstruction(Frame f, int playerIndex, EntityRef supplyEntity)
        {
            if (f.Unsafe.TryGetPointer<SupplyBuilding>(supplyEntity, out SupplyBuilding* supplyBuilding) == false ||
                supplyBuilding->OwnerPlayer != playerIndex ||
                supplyBuilding->Health <= 0 ||
                supplyBuilding->IsConstructing == false)
            {
                return false;
            }

            if (f.Unsafe.TryGetPointer<Transform2D>(supplyEntity, out Transform2D* supplyTransform) == false)
            {
                return false;
            }

            bool assignedAny = false;
            int selectedWorkerIndex = 0;
            foreach ((EntityRef entity, Selectable selectable) in f.GetComponentIterator<Selectable>())
            {
                if (selectable.IsSelected == false)
                {
                    continue;
                }

                if (CanAssignWorkerToConstruction(f, entity, playerIndex, supplyEntity) == false)
                {
                    continue;
                }

                AssignBuilder(f, entity, supplyEntity, supplyTransform->Position + GetBuilderFormationOffset(selectedWorkerIndex));
                selectedWorkerIndex++;
                assignedAny = true;
            }

            return assignedAny;
        }

        private static void ReleaseBuildersForCompletedOrDestroyedSupply(Frame f, EntityRef supplyEntity)
        {
            foreach ((EntityRef workerEntity, WorkerBuildIntent buildIntent) in f.GetComponentIterator<WorkerBuildIntent>())
            {
                if (buildIntent.IsBuilding == false || buildIntent.TargetBuilding != supplyEntity)
                {
                    continue;
                }

                WorkerBuildIntent updatedIntent = buildIntent;
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
        }

        private static int GetActiveBuilderCount(Frame f, EntityRef supplyEntity)
        {
            int count = 0;
            foreach ((EntityRef workerEntity, WorkerBuildIntent buildIntent) in f.GetComponentIterator<WorkerBuildIntent>())
            {
                if (buildIntent.IsBuilding == false || buildIntent.TargetBuilding != supplyEntity)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<UnitIdentity>(workerEntity, out UnitIdentity* unitIdentity) == false ||
                    unitIdentity->UnitKind != UnitKind.Worker)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<UnitHealth>(workerEntity, out UnitHealth* unitHealth) && unitHealth->IsDead)
                {
                    continue;
                }

                count++;
            }

            if (count < 1)
            {
                return 1;
            }

            return count;
        }

        private static bool CanAssignWorkerToConstruction(Frame f, EntityRef entity, int playerIndex, EntityRef supplyEntity)
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

            if (f.Unsafe.TryGetPointer<WorkerBuildIntent>(entity, out WorkerBuildIntent* buildIntent) &&
                buildIntent->IsBuilding &&
                buildIntent->TargetBuilding == supplyEntity)
            {
                return false;
            }

            return true;
        }

        private static FPVector2 GetBuilderFormationOffset(int selectedWorkerIndex)
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

        private static bool TryGetSelectedWorkerNearBuildPoint(Frame f, int playerIndex, FPVector2 buildPoint, out EntityRef builderEntity)
        {
            foreach ((EntityRef entity, Selectable selectable) in f.GetComponentIterator<Selectable>())
            {
                if (selectable.IsSelected == false)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity) == false ||
                    unitIdentity->OwnerPlayer != playerIndex ||
                    unitIdentity->UnitKind != UnitKind.Worker)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<UnitHealth>(entity, out UnitHealth* unitHealth) && unitHealth->IsDead)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<WorkerBuildIntent>(entity, out WorkerBuildIntent* buildIntent) && buildIntent->IsBuilding)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false)
                {
                    continue;
                }

                if (FPVector2.Distance(transform->Position, buildPoint) <= BuildRange)
                {
                    builderEntity = entity;
                    return true;
                }
            }

            builderEntity = EntityRef.None;
            return false;
        }

        private static bool IsSelected(Frame f, EntityRef entity)
        {
            return f.Unsafe.TryGetPointer<Selectable>(entity, out Selectable* selectable) &&
                   selectable->IsSelected;
        }

        private static bool IsValidSupplyPlacement(Frame f, FPVector2 buildPoint)
        {
            if (buildPoint.X < -MapHalfExtent || buildPoint.X > MapHalfExtent ||
                buildPoint.Y < -MapHalfExtent || buildPoint.Y > MapHalfExtent)
            {
                return false;
            }

            foreach ((EntityRef entity, ResourceNode resourceNode) in f.GetComponentIterator<ResourceNode>())
            {
                if (IsTooCloseToTransform(f, entity, buildPoint, PlacementRadius + FP.FromString("1.25")))
                {
                    return false;
                }
            }

            foreach ((EntityRef entity, MainBuilding mainBuilding) in f.GetComponentIterator<MainBuilding>())
            {
                if (mainBuilding.Health > 0 && IsTooCloseToTransform(f, entity, buildPoint, PlacementRadius + FP.FromString("1.6")))
                {
                    return false;
                }
            }

            foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in f.GetComponentIterator<SupplyBuilding>())
            {
                if (supplyBuilding.Health > 0 && IsTooCloseToTransform(f, entity, buildPoint, PlacementRadius + FP.FromString("1.2")))
                {
                    return false;
                }
            }

            foreach ((EntityRef entity, UnitIdentity unitIdentity) in f.GetComponentIterator<UnitIdentity>())
            {
                if (IsTooCloseToTransform(f, entity, buildPoint, PlacementRadius + UnitBlockRadius))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsTooCloseToTransform(Frame f, EntityRef entity, FPVector2 buildPoint, FP blockedDistance)
        {
            if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false)
            {
                return false;
            }

            return FPVector2.Distance(transform->Position, buildPoint) < blockedDistance;
        }

        private static bool TryGetEconomyState(Frame f, int playerIndex, out EntityRef economyEntity, out PlayerEconomyState economyState)
        {
            foreach ((EntityRef entity, PlayerEconomyState state) in f.GetComponentIterator<PlayerEconomyState>())
            {
                if (state.PlayerIndex == playerIndex)
                {
                    economyEntity = entity;
                    economyState = state;
                    return true;
                }
            }

            economyEntity = EntityRef.None;
            economyState = default;
            return false;
        }
    }
}
