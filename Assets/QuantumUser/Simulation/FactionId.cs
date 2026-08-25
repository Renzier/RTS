namespace Quantum
{
    public static class FactionId
    {
        public const int Tech = 0;
        public const int Fantasy = 1;
        public const int Hybrid = 2;

        public static int Normalize(int factionId)
        {
            if (factionId == Fantasy)
            {
                return Fantasy;
            }

            if (factionId == Hybrid)
            {
                return Hybrid;
            }

            return Tech;
        }
    }
}
