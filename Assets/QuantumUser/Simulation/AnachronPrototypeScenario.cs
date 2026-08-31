namespace Quantum
{
    using Photon.Deterministic;

    public static class AnachronPrototypeScenario
    {
        public const string ScenarioName = "Ashenspar Quill-Waist";

        public static readonly PlayerSpawn[] Players =
        {
            new PlayerSpawn(0, FactionId.ArdentConcord, 500, 300, 3, 10),
            new PlayerSpawn(1, FactionId.Wrought, 500, 300, 3, 10),
            new PlayerSpawn(2, FactionId.Gharn, 500, 300, 3, 10),
            new PlayerSpawn(3, FactionId.Seethe, 500, 300, 3, 10),
            new PlayerSpawn(4, FactionId.Veirn, 500, 300, 3, 10),
            new PlayerSpawn(5, FactionId.Vaelun, 500, 300, 3, 10)
        };

        public static readonly WorkerSpawn[] Workers =
        {
            new WorkerSpawn(1, 0, -2, -15),
            new WorkerSpawn(2, 0, 0, -15),
            new WorkerSpawn(3, 0, 2, -15),
            new WorkerSpawn(4, 1, -19, 12),
            new WorkerSpawn(5, 1, -17, 12),
            new WorkerSpawn(6, 1, -15, 12),
            new WorkerSpawn(7, 2, 15, 12),
            new WorkerSpawn(8, 2, 17, 12),
            new WorkerSpawn(9, 2, 19, 12),
            new WorkerSpawn(10, 3, -2, 29),
            new WorkerSpawn(11, 3, 0, 29),
            new WorkerSpawn(12, 3, 2, 29),
            new WorkerSpawn(13, 4, -31, -3),
            new WorkerSpawn(14, 4, -29, -3),
            new WorkerSpawn(15, 4, -27, -3),
            new WorkerSpawn(16, 5, 27, -3),
            new WorkerSpawn(17, 5, 29, -3),
            new WorkerSpawn(18, 5, 31, -3)
        };

        public static readonly HeroSpawn[] Heroes =
        {
            new HeroSpawn(100, 0, 0, -16),
            new HeroSpawn(101, 1, -17, 11),
            new HeroSpawn(102, 2, 17, 11),
            new HeroSpawn(103, 3, 0, 28),
            new HeroSpawn(104, 4, -29, -4),
            new HeroSpawn(105, 5, 29, -4)
        };

        public static readonly MainBaseSpawn[] MainBases =
        {
            new MainBaseSpawn(0, 0, -13),
            new MainBaseSpawn(1, -17, 14),
            new MainBaseSpawn(2, 17, 14),
            new MainBaseSpawn(3, 0, 31),
            new MainBaseSpawn(4, -29, -1),
            new MainBaseSpawn(5, 29, -1)
        };

        public static readonly ResourceNodeSpawn[] ResourceNodes =
        {
            new ResourceNodeSpawn(ResourceKind.Wood, 2500, -7, 1),
            new ResourceNodeSpawn(ResourceKind.Iron, 2200, 7, 1),
            new ResourceNodeSpawn(ResourceKind.Wood, 1800, -15, -5),
            new ResourceNodeSpawn(ResourceKind.Iron, 1600, 15, -5),
            new ResourceNodeSpawn(ResourceKind.Wood, 2200, -14, 18),
            new ResourceNodeSpawn(ResourceKind.Iron, 1800, 14, 18)
        };

        public readonly struct PlayerSpawn
        {
            public readonly int PlayerIndex;
            public readonly int PlayerFactionId;
            public readonly int StartingWood;
            public readonly int StartingIron;
            public readonly int StartingFoodUsed;
            public readonly int StartingFoodCap;

            public PlayerSpawn(int playerIndex, int factionId, int startingWood, int startingIron, int startingFoodUsed, int startingFoodCap)
            {
                PlayerIndex = playerIndex;
                PlayerFactionId = Quantum.FactionId.Normalize(factionId);
                StartingWood = startingWood;
                StartingIron = startingIron;
                StartingFoodUsed = startingFoodUsed;
                StartingFoodCap = startingFoodCap;
            }
        }

        public readonly struct WorkerSpawn
        {
            public readonly int UnitId;
            public readonly int OwnerPlayer;
            public readonly int X;
            public readonly int Y;

            public WorkerSpawn(int unitId, int ownerPlayer, int x, int y)
            {
                UnitId = unitId;
                OwnerPlayer = ownerPlayer;
                X = x;
                Y = y;
            }

            public FPVector2 Position => new FPVector2(X, Y);
        }

        public readonly struct HeroSpawn
        {
            public readonly int UnitId;
            public readonly int OwnerPlayer;
            public readonly int X;
            public readonly int Y;

            public HeroSpawn(int unitId, int ownerPlayer, int x, int y)
            {
                UnitId = unitId;
                OwnerPlayer = ownerPlayer;
                X = x;
                Y = y;
            }

            public FPVector2 Position => new FPVector2(X, Y);
        }

        public readonly struct MainBaseSpawn
        {
            public readonly int OwnerPlayer;
            public readonly int X;
            public readonly int Y;

            public MainBaseSpawn(int ownerPlayer, int x, int y)
            {
                OwnerPlayer = ownerPlayer;
                X = x;
                Y = y;
            }

            public FPVector2 Position => new FPVector2(X, Y);
        }

        public readonly struct ResourceNodeSpawn
        {
            public readonly int ResourceKind;
            public readonly int AmountRemaining;
            public readonly int X;
            public readonly int Y;

            public ResourceNodeSpawn(int resourceKind, int amountRemaining, int x, int y)
            {
                ResourceKind = resourceKind;
                AmountRemaining = amountRemaining;
                X = x;
                Y = y;
            }

            public FPVector2 Position => new FPVector2(X, Y);
        }
    }
}
