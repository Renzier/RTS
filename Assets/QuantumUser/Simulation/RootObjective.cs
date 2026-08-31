namespace Quantum
{
    using Photon.Deterministic;

    public static class RootObjective
    {
        public const int NeutralOwner = -1;
        public const int MaxHealth = 1200;
        public static readonly FPVector2 Position = new FPVector2(FP._0, FP.FromString("36.0"));
        public static readonly FP TargetRadius = FP.FromString("1.1");
        public static readonly FP SelectionRadius = FP.FromString("1.35");

        public static bool IsObjectivePosition(FPVector2 position)
        {
            return FPVector2.Distance(position, Position) <= FP.EN1;
        }
    }
}
