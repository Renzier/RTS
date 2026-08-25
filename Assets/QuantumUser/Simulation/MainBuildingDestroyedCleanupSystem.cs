namespace Quantum
{
    public unsafe class MainBuildingDestroyedCleanupSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            foreach ((EntityRef entity, MainBuilding mainBuilding) in f.GetComponentIterator<MainBuilding>())
            {
                if (mainBuilding.Health > 0)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Selectable>(entity, out Selectable* selectable))
                {
                    selectable->IsSelected = false;
                }

                if (f.Unsafe.TryGetPointer<Targetable>(entity, out Targetable* targetable))
                {
                    targetable->Health = 0;
                }
            }
        }
    }
}
