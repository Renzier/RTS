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
            new PlayerSpawn(5, FactionId.Vaelun, 500, 300, 3, 10),
            new PlayerSpawn(6, FactionId.Nimhara, 500, 300, 3, 10),
            new PlayerSpawn(7, FactionId.Virii, 500, 300, 3, 10)
        };

        public static readonly WorkerSpawn[] Workers =
        {
            new WorkerSpawn(1, 0, -2, -53),
            new WorkerSpawn(2, 0, 0, -53),
            new WorkerSpawn(3, 0, 2, -53),
            new WorkerSpawn(4, 1, -38, -38),
            new WorkerSpawn(5, 1, -37, -40),
            new WorkerSpawn(6, 1, -36, -42),
            new WorkerSpawn(7, 2, 38, -38),
            new WorkerSpawn(8, 2, 37, -40),
            new WorkerSpawn(9, 2, 36, -42),
            new WorkerSpawn(10, 3, -55, -2),
            new WorkerSpawn(11, 3, -55, 0),
            new WorkerSpawn(12, 3, -55, 2),
            new WorkerSpawn(13, 4, 55, -2),
            new WorkerSpawn(14, 4, 55, 0),
            new WorkerSpawn(15, 4, 55, 2),
            new WorkerSpawn(16, 5, -38, 38),
            new WorkerSpawn(17, 5, -37, 40),
            new WorkerSpawn(18, 5, -36, 42),
            new WorkerSpawn(19, 6, 38, 38),
            new WorkerSpawn(20, 6, 37, 40),
            new WorkerSpawn(21, 6, 36, 42),
            new WorkerSpawn(22, 7, -2, 53),
            new WorkerSpawn(23, 7, 0, 53),
            new WorkerSpawn(24, 7, 2, 53)
        };

        public static readonly HeroSpawn[] Heroes =
        {
            new HeroSpawn(100, 0, 0, -52),
            new HeroSpawn(101, 1, -36, -36),
            new HeroSpawn(102, 2, 36, -36),
            new HeroSpawn(103, 3, -54, 0),
            new HeroSpawn(104, 4, 54, 0),
            new HeroSpawn(105, 5, -36, 36),
            new HeroSpawn(106, 6, 36, 36),
            new HeroSpawn(107, 7, 0, 52)
        };

        public static readonly AirScoutSpawn[] AirScouts =
        {
            new AirScoutSpawn(200, 0, 4, -52),
            new AirScoutSpawn(201, 1, -34, -38),
            new AirScoutSpawn(202, 2, 34, -38),
            new AirScoutSpawn(203, 3, -54, 4),
            new AirScoutSpawn(204, 4, 54, 4),
            new AirScoutSpawn(205, 5, -34, 38),
            new AirScoutSpawn(206, 6, 34, 38),
            new AirScoutSpawn(207, 7, 4, 52)
        };

        public static readonly MainBaseSpawn[] MainBases =
        {
            new MainBaseSpawn(0, 0, -55),
            new MainBaseSpawn(1, -40, -40),
            new MainBaseSpawn(2, 40, -40),
            new MainBaseSpawn(3, -58, 0),
            new MainBaseSpawn(4, 58, 0),
            new MainBaseSpawn(5, -40, 40),
            new MainBaseSpawn(6, 40, 40),
            new MainBaseSpawn(7, 0, 55)
        };

        public static readonly ResourceNodeSpawn[] ResourceNodes =
        {
            new ResourceNodeSpawn(ResourceKind.Wood, 2400, -8, -45),
            new ResourceNodeSpawn(ResourceKind.Iron, 2000, 8, -45),
            new ResourceNodeSpawn(ResourceKind.Wood, 2400, -47, -31),
            new ResourceNodeSpawn(ResourceKind.Iron, 2000, -31, -47),
            new ResourceNodeSpawn(ResourceKind.Wood, 2400, 47, -31),
            new ResourceNodeSpawn(ResourceKind.Iron, 2000, 31, -47),
            new ResourceNodeSpawn(ResourceKind.Wood, 2400, -49, -8),
            new ResourceNodeSpawn(ResourceKind.Iron, 2000, -49, 8),
            new ResourceNodeSpawn(ResourceKind.Wood, 2400, 49, -8),
            new ResourceNodeSpawn(ResourceKind.Iron, 2000, 49, 8),
            new ResourceNodeSpawn(ResourceKind.Wood, 2400, -47, 31),
            new ResourceNodeSpawn(ResourceKind.Iron, 2000, -31, 47),
            new ResourceNodeSpawn(ResourceKind.Wood, 2400, 47, 31),
            new ResourceNodeSpawn(ResourceKind.Iron, 2000, 31, 47),
            new ResourceNodeSpawn(ResourceKind.Wood, 2400, -8, 45),
            new ResourceNodeSpawn(ResourceKind.Iron, 2000, 8, 45)
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

        public readonly struct AirScoutSpawn
        {
            public readonly int UnitId;
            public readonly int OwnerPlayer;
            public readonly int X;
            public readonly int Y;

            public AirScoutSpawn(int unitId, int ownerPlayer, int x, int y)
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
