namespace Quantum
{
    public static class MoveCommandResult
    {
        public const int Pending = 0;
        public const int Accepted = 1;
        public const int NoNavMesh = 2;
        public const int MissingPathfinder = 3;
        public const int MissingTransform = 4;
        public const int InvalidStartPosition = 5;
        public const int InvalidEndPosition = 6;
        public const int NoPathFound = 7;
        public const int PathTooLong = 8;
        public const int BlockedByTerrain = 9;
    }
}
