namespace Quantum
{
    using Photon.Deterministic;

    public static class TerrainBlockers
    {
        private static readonly FP WestShardMinX = FP.FromString("-24.0");
        private static readonly FP WestShardMaxX = FP.FromString("-20.0");
        private static readonly FP EastShardMinX = FP.FromString("20.0");
        private static readonly FP EastShardMaxX = FP.FromString("24.0");
        private static readonly FP ShardMinY = FP.FromString("-9.0");
        private static readonly FP ShardMaxY = FP.FromString("9.0");
        private static readonly FP BuildBuffer = FP.FromString("1.8");

        public static bool BlocksGroundBuild(FPVector2 position)
        {
            return IsInsideRect(position, WestShardMinX - BuildBuffer, WestShardMaxX + BuildBuffer, ShardMinY - BuildBuffer, ShardMaxY + BuildBuffer) ||
                   IsInsideRect(position, EastShardMinX - BuildBuffer, EastShardMaxX + BuildBuffer, ShardMinY - BuildBuffer, ShardMaxY + BuildBuffer);
        }

        public static bool BlocksGroundMovement(FPVector2 position)
        {
            return IsInsideRect(position, WestShardMinX, WestShardMaxX, ShardMinY, ShardMaxY) ||
                   IsInsideRect(position, EastShardMinX, EastShardMaxX, ShardMinY, ShardMaxY);
        }

        public static bool BlocksGroundBuild(float x, float y)
        {
            return IsInsideRect(x, y, -25.8f, -18.2f, -10.8f, 10.8f) ||
                   IsInsideRect(x, y, 18.2f, 25.8f, -10.8f, 10.8f);
        }

        private static bool IsInsideRect(FPVector2 position, FP minX, FP maxX, FP minY, FP maxY)
        {
            return position.X >= minX &&
                   position.X <= maxX &&
                   position.Y >= minY &&
                   position.Y <= maxY;
        }

        private static bool IsInsideRect(float x, float y, float minX, float maxX, float minY, float maxY)
        {
            return x >= minX &&
                   x <= maxX &&
                   y >= minY &&
                   y <= maxY;
        }
    }
}
