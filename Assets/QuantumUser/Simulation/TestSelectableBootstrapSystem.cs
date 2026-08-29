using Photon.Deterministic;

namespace Quantum
{
    public unsafe class TestSelectableBootstrapSystem : SystemSignalsOnly
    {
        public override void OnInit(Frame f)
        {
            foreach (AnachronPrototypeScenario.PlayerSpawn player in AnachronPrototypeScenario.Players)
            {
                CreatePlayerState(f, player);
            }

            foreach (AnachronPrototypeScenario.WorkerSpawn worker in AnachronPrototypeScenario.Workers)
            {
                CreateTestUnit(f, worker);
            }

            foreach (AnachronPrototypeScenario.HeroSpawn hero in AnachronPrototypeScenario.Heroes)
            {
                EntityRef heroEntity = CreateHero(f, hero);
                LinkHeroState(f, hero.OwnerPlayer, heroEntity);
            }

            foreach (AnachronPrototypeScenario.MainBaseSpawn mainBase in AnachronPrototypeScenario.MainBases)
            {
                CreateMainBuilding(f, mainBase);
            }

            foreach (AnachronPrototypeScenario.ResourceNodeSpawn resourceNode in AnachronPrototypeScenario.ResourceNodes)
            {
                CreateResourceNode(f, resourceNode);
            }

            CreateQuillObjective(f);
        }

        private static void CreatePlayerState(Frame f, AnachronPrototypeScenario.PlayerSpawn player)
        {
            EntityRef entity = f.Create();
            f.Set(entity, new PlayerEconomyState
            {
                PlayerIndex = player.PlayerIndex,
                Wood = player.StartingWood,
                Iron = player.StartingIron,
                FoodUsed = player.StartingFoodUsed,
                FoodCap = player.StartingFoodCap,
                IsDefeated = false
            });

            f.Set(entity, new PlayerFactionState
            {
                PlayerIndex = player.PlayerIndex,
                FactionId = player.PlayerFactionId
            });

            f.Set(entity, new PlayerTechState
            {
                PlayerIndex = player.PlayerIndex,
                TechTier = 1,
                UpgradeInProgress = false,
                UpgradeTargetTier = 0,
                UpgradeTicksRemaining = 0,
                UpgradeTicksTotal = 0,
                LastUpgradeResult = TechUpgradeResult.None
            });

            f.Set(entity, new PlayerHeroState
            {
                PlayerIndex = player.PlayerIndex,
                HeroEntity = EntityRef.None,
                HasActiveHero = true,
                RebuildAvailable = false,
                HeroLevel = 1,
                HeroHealth = FactionStats.ForFaction(player.PlayerFactionId).HeroMaxHealth,
                HeroMaxHealth = FactionStats.ForFaction(player.PlayerFactionId).HeroMaxHealth,
                RebuildInProgress = false,
                RebuildTicksRemaining = 0,
                RebuildTicksTotal = 0,
                LastHeroResult = HeroLifecycleResult.None
            });
        }

        private static void CreateTestUnit(Frame f, AnachronPrototypeScenario.WorkerSpawn worker)
        {
            EntityRef entity = f.Create();
            FactionStats stats = FactionStats.ForPlayer(f, worker.OwnerPlayer);

            f.Set(entity, new UnitIdentity
            {
                UnitId = worker.UnitId,
                OwnerPlayer = worker.OwnerPlayer,
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
                OwnerPlayer = worker.OwnerPlayer,
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
                Position = worker.Position,
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

        private static EntityRef CreateHero(Frame f, AnachronPrototypeScenario.HeroSpawn hero)
        {
            EntityRef entity = f.Create();
            FactionStats stats = FactionStats.ForPlayer(f, hero.OwnerPlayer);

            f.Set(entity, new UnitIdentity
            {
                UnitId = hero.UnitId,
                OwnerPlayer = hero.OwnerPlayer,
                UnitKind = UnitKind.Hero
            });

            f.Set(entity, new UnitHealth
            {
                Health = stats.HeroMaxHealth,
                MaxHealth = stats.HeroMaxHealth,
                IsDead = false
            });

            f.Set(entity, new Targetable
            {
                OwnerPlayer = hero.OwnerPlayer,
                Health = stats.HeroMaxHealth,
                MaxHealth = stats.HeroMaxHealth,
                TargetRadius = FP.FromString("0.95")
            });

            f.Set(entity, new Selectable
            {
                IsSelected = false,
                SelectionRadius = FP.FromString("1.15")
            });

            f.Set(entity, new Transform2D
            {
                Position = hero.Position,
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

            f.Set(entity, new AttackIntent
            {
                HasTarget = false,
                TargetEntity = EntityRef.None,
                TargetWorld = FPVector2.Zero,
                AttackRange = FP.FromString("3.0"),
                Damage = stats.HeroDamageForTier(1),
                CooldownTicksRemaining = 0,
                CooldownTicks = 55,
                IsInRange = false
            });

            NavMeshPathfinder pathfinder = NavMeshPathfinder.Create(f, entity, null);
            f.Set(entity, pathfinder);
            f.Set(entity, new NavMeshSteeringAgent());

            return entity;
        }

        private static void LinkHeroState(Frame f, int ownerPlayer, EntityRef heroEntity)
        {
            foreach ((EntityRef entity, PlayerHeroState heroState) in f.GetComponentIterator<PlayerHeroState>())
            {
                if (heroState.PlayerIndex != ownerPlayer)
                {
                    continue;
                }

                PlayerHeroState updatedHeroState = heroState;
                updatedHeroState.HeroEntity = heroEntity;
                f.Set(entity, updatedHeroState);
                return;
            }
        }

        private static void CreateMainBuilding(Frame f, AnachronPrototypeScenario.MainBaseSpawn mainBase)
        {
            EntityRef entity = f.Create();
            FactionStats stats = FactionStats.ForPlayer(f, mainBase.OwnerPlayer);

            f.Set(entity, new Transform2D
            {
                Position = mainBase.Position,
                Rotation = FP._0
            });

            f.Set(entity, new ResourceDropoff
            {
                OwnerPlayer = mainBase.OwnerPlayer,
                AcceptedResourceMask = ResourceMask.All
            });

            f.Set(entity, new MainBuilding
            {
                OwnerPlayer = mainBase.OwnerPlayer,
                Health = stats.MainBaseMaxHealth,
                MaxHealth = stats.MainBaseMaxHealth
            });

            f.Set(entity, new BuildingTier
            {
                Tier = 1
            });

            f.Set(entity, new Targetable
            {
                OwnerPlayer = mainBase.OwnerPlayer,
                Health = stats.MainBaseMaxHealth,
                MaxHealth = stats.MainBaseMaxHealth,
                TargetRadius = FP.FromString("1.4")
            });

            f.Set(entity, new Selectable
            {
                IsSelected = false,
                SelectionRadius = FP.FromString("1.45")
            });

            f.Set(entity, new SelectionCandidate
            {
                ScreenPosition = FPVector2.Zero
            });
        }

        private static void CreateResourceNode(Frame f, AnachronPrototypeScenario.ResourceNodeSpawn resourceNode)
        {
            EntityRef entity = f.Create();

            f.Set(entity, new Transform2D
            {
                Position = resourceNode.Position,
                Rotation = FP._0
            });

            f.Set(entity, new ResourceNode
            {
                ResourceKind = resourceNode.ResourceKind,
                AmountRemaining = resourceNode.AmountRemaining,
                HarvestBatchSize = 5
            });
        }

        private static void CreateQuillObjective(Frame f)
        {
            EntityRef entity = f.Create();

            f.Set(entity, new Transform2D
            {
                Position = QuillObjective.Position,
                Rotation = FP._0
            });

            f.Set(entity, new Targetable
            {
                OwnerPlayer = QuillObjective.NeutralOwner,
                Health = QuillObjective.MaxHealth,
                MaxHealth = QuillObjective.MaxHealth,
                TargetRadius = QuillObjective.TargetRadius
            });

            f.Set(entity, new Selectable
            {
                IsSelected = false,
                SelectionRadius = QuillObjective.SelectionRadius
            });

            f.Set(entity, new SelectionCandidate
            {
                ScreenPosition = FPVector2.Zero
            });
        }
    }
}
