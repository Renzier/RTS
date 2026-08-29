namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class WorkerProductionSystem : SystemMainThread
    {
        private const int TrainWorkerIntent = 3;

        public override void Update(Frame f)
        {
            if (f.Global->LastUpgradeIntent != TrainWorkerIntent)
            {
                return;
            }

            f.Global->LastUpgradeIntent = 0;
            int playerIndex = f.Global->LastInputPlayer;
            FactionStats stats = FactionStats.ForPlayer(f, playerIndex);

            if (TryGetEconomyState(f, playerIndex, out EntityRef economyEntity, out PlayerEconomyState economyState) == false ||
                economyState.IsDefeated ||
                economyState.Wood < stats.WorkerWoodCost ||
                economyState.Iron < stats.WorkerIronCost ||
                economyState.FoodUsed + stats.WorkerFoodCost > economyState.FoodCap)
            {
                return;
            }

            if (TryGetWorkerSpawnPosition(f, playerIndex, out FPVector2 spawnPosition) == false)
            {
                return;
            }

            PlayerEconomyState updatedEconomy = economyState;
            updatedEconomy.Wood -= stats.WorkerWoodCost;
            updatedEconomy.Iron -= stats.WorkerIronCost;
            updatedEconomy.FoodUsed += stats.WorkerFoodCost;
            f.Set(economyEntity, updatedEconomy);

            CreateWorker(f, playerIndex, GetNextUnitId(f), spawnPosition);
        }

        private static void CreateWorker(Frame f, int ownerPlayer, int unitId, FPVector2 position)
        {
            EntityRef entity = f.Create();
            FactionStats stats = FactionStats.ForPlayer(f, ownerPlayer);

            f.Set(entity, new UnitIdentity
            {
                UnitId = unitId,
                OwnerPlayer = ownerPlayer,
                UnitKind = UnitKind.Worker
            });

            f.Set(entity, new WorkerResourceCarry
            {
                ResourceKind = ResourceKind.None,
                Amount = 0,
                Capacity = 10
            });

            f.Set(entity, new UnitHealth
            {
                Health = stats.WorkerMaxHealth,
                MaxHealth = stats.WorkerMaxHealth,
                IsDead = false
            });

            f.Set(entity, new Targetable
            {
                OwnerPlayer = ownerPlayer,
                Health = stats.WorkerMaxHealth,
                MaxHealth = stats.WorkerMaxHealth,
                TargetRadius = FP.FromString("0.75")
            });

            f.Set(entity, new Selectable
            {
                IsSelected = false,
                SelectionRadius = FP._1
            });

            f.Set(entity, new Transform2D
            {
                Position = position,
                Rotation = FP._0
            });

            f.Set(entity, new SelectionCandidate
            {
                ScreenPosition = FPVector2.Zero
            });

            f.Set(entity, new CommandIntentDebug
            {
                HasMoveCommandIntent = false,
                WasMoveCommandAccepted = false,
                WasMoveCommandRejected = false,
                MoveCommandPlayer = -1,
                MoveCommandResult = 0,
                MoveCommandTargetWorld = FPVector2.Zero
            });

            f.Set(entity, new MoveIntent
            {
                HasTarget = false,
                MovementMode = MovementMode.QuantumNavMesh,
                TargetWorld = FPVector2.Zero
            });

            f.Set(entity, new GatherIntent
            {
                HasTarget = false,
                TargetNode = EntityRef.None,
                ResourceKind = ResourceKind.None,
                TargetWorld = FPVector2.Zero
            });

            f.Set(entity, new AttackIntent
            {
                HasTarget = false,
                TargetEntity = EntityRef.None,
                TargetWorld = FPVector2.Zero,
                AttackRange = FP.FromString("2.25"),
                Damage = 25,
                CooldownTicksRemaining = 0,
                CooldownTicks = 60,
                IsInRange = false
            });

            NavMeshPathfinder pathfinder = NavMeshPathfinder.Create(f, entity, null);
            f.Set(entity, pathfinder);
            f.Set(entity, new NavMeshSteeringAgent());
        }

        private static int GetNextUnitId(Frame f)
        {
            int maxUnitId = 0;
            foreach ((EntityRef entity, UnitIdentity unitIdentity) in f.GetComponentIterator<UnitIdentity>())
            {
                if (unitIdentity.UnitId > maxUnitId)
                {
                    maxUnitId = unitIdentity.UnitId;
                }
            }

            return maxUnitId + 1;
        }

        private static bool TryGetWorkerSpawnPosition(Frame f, int playerIndex, out FPVector2 spawnPosition)
        {
            foreach ((EntityRef entity, MainBuilding mainBuilding) in f.GetComponentIterator<MainBuilding>())
            {
                if (mainBuilding.OwnerPlayer != playerIndex || mainBuilding.Health <= 0 || IsSelected(f, entity) == false)
                {
                    continue;
                }

                if (TryGetTransform(f, entity, out Transform2D selectedBaseTransform))
                {
                    spawnPosition = selectedBaseTransform.Position + GetSpawnOffset(f, playerIndex);
                    return true;
                }
            }

            foreach ((EntityRef entity, MainBuilding mainBuilding) in f.GetComponentIterator<MainBuilding>())
            {
                if (mainBuilding.OwnerPlayer != playerIndex || mainBuilding.Health <= 0)
                {
                    continue;
                }

                if (TryGetTransform(f, entity, out Transform2D baseTransform))
                {
                    spawnPosition = baseTransform.Position + GetSpawnOffset(f, playerIndex);
                    return true;
                }
            }

            spawnPosition = FPVector2.Zero;
            return false;
        }

        private static FPVector2 GetSpawnOffset(Frame f, int playerIndex)
        {
            int workerCount = CountWorkers(f, playerIndex);
            int slot = workerCount % 5;
            FP x = FP.FromString("-2.0") + FP.FromString(slot.ToString());
            return new FPVector2(x, FP.FromString("2.2"));
        }

        private static int CountWorkers(Frame f, int playerIndex)
        {
            int count = 0;
            foreach ((EntityRef entity, UnitIdentity unitIdentity) in f.GetComponentIterator<UnitIdentity>())
            {
                if (unitIdentity.OwnerPlayer == playerIndex && unitIdentity.UnitKind == UnitKind.Worker)
                {
                    count++;
                }
            }

            return count;
        }

        private static bool IsSelected(Frame f, EntityRef candidateEntity)
        {
            foreach ((EntityRef entity, Selectable selectable) in f.GetComponentIterator<Selectable>())
            {
                if (entity == candidateEntity)
                {
                    return selectable.IsSelected;
                }
            }

            return false;
        }

        private static bool TryGetTransform(Frame f, EntityRef candidateEntity, out Transform2D transform)
        {
            foreach ((EntityRef entity, Transform2D candidateTransform) in f.GetComponentIterator<Transform2D>())
            {
                if (entity == candidateEntity)
                {
                    transform = candidateTransform;
                    return true;
                }
            }

            transform = default;
            return false;
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
