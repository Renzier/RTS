namespace Quantum
{
    public unsafe class GrainStateSystem : SystemMainThread
    {
        public const int ExposureTicks = 180;

        public override void Update(Frame f)
        {
            foreach ((EntityRef entity, GrainState grainState) in f.GetComponentIterator<GrainState>())
            {
                if (grainState.IsGrainLoud == false || grainState.GrainLoudTicksRemaining <= 0)
                {
                    Deactivate(f, entity, grainState);
                    continue;
                }

                GrainState updatedState = grainState;
                updatedState.GrainLoudTicksRemaining--;
                if (updatedState.GrainLoudTicksRemaining <= 0)
                {
                    Deactivate(f, entity, updatedState);
                    continue;
                }

                f.Set(entity, updatedState);
            }
        }

        public static void MarkGrainLoud(Frame f, EntityRef entity, int source)
        {
            if (entity == EntityRef.None)
            {
                return;
            }

            f.Set(entity, new GrainState
            {
                IsGrainLoud = true,
                GrainLoudTicksRemaining = ExposureTicks,
                GrainLoudSource = source
            });
        }

        private static void Deactivate(Frame f, EntityRef entity, GrainState grainState)
        {
            if (grainState.IsGrainLoud == false &&
                grainState.GrainLoudTicksRemaining == 0 &&
                grainState.GrainLoudSource == GrainLoudSource.None)
            {
                return;
            }

            grainState.IsGrainLoud = false;
            grainState.GrainLoudTicksRemaining = 0;
            grainState.GrainLoudSource = GrainLoudSource.None;
            f.Set(entity, grainState);
        }
    }
}
