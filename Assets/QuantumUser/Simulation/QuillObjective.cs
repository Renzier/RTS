namespace Quantum
{
    using Photon.Deterministic;

    public static class QuillObjective
    {
        public const int NeutralOwner = -1;
        public const int MaxHealth = 2000;
        public const int CaptureRequired = 2000;
        public const int CapturePerUnitTick = 5;
        public const int ResourceTrickleIntervalTicks = 180;
        public const int ResourceTrickleWood = 15;
        public const int ResourceTrickleIron = 8;
        public const bool VictoryEnabled = false;
        public const int VictoryHoldTicks = 1800;
        public static readonly FPVector2 Position = FPVector2.Zero;
        public static readonly FP TargetRadius = FP.FromString("1.35");
        public static readonly FP SelectionRadius = FP.FromString("1.55");
        public static readonly FP CaptureRadius = FP.FromString("5.0");

        public static bool IsObjectivePosition(FPVector2 position)
        {
            return FPVector2.Distance(position, Position) <= FP.EN1;
        }
    }
}
