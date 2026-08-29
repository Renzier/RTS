namespace Quantum
{
    using Photon.Deterministic;

    public static class QuillObjective
    {
        public const int NeutralOwner = -1;
        public const int MaxHealth = 2000;
        public static readonly FPVector2 Position = new FPVector2(FP._0, FP.FromString("7.0"));
        public static readonly FP TargetRadius = FP.FromString("1.35");
        public static readonly FP SelectionRadius = FP.FromString("1.55");
    }
}
