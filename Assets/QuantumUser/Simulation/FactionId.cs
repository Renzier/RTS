namespace Quantum
{
    public static class FactionId
    {
        public const int ArdentConcord = 0;
        public const int Wrought = 1;
        public const int Gharn = 2;
        public const int Seethe = 3;
        public const int Veirn = 4;
        public const int Vaelun = 5;

        public const int Tech = ArdentConcord;
        public const int Fantasy = Wrought;
        public const int Hybrid = Gharn;

        public static int Normalize(int factionId)
        {
            if (factionId == Wrought)
            {
                return Wrought;
            }

            if (factionId == Gharn)
            {
                return Gharn;
            }

            if (factionId == Seethe)
            {
                return Seethe;
            }

            if (factionId == Veirn)
            {
                return Veirn;
            }

            if (factionId == Vaelun)
            {
                return Vaelun;
            }

            return ArdentConcord;
        }
    }
}
