namespace Quantum
{
    public static class HeroLifecycleResult
    {
        public const int None = 0;
        public const int Active = 1;
        public const int Defeated = 2;
        public const int MissingMainBase = 3;
        public const int RebuildAvailable = 4;
        public const int Rebuilt = 5;
        public const int InsufficientResources = 6;
        public const int RebuildUnavailable = 7;
        public const int RebuildStarted = 8;
        public const int RebuildInProgress = 9;
    }
}
