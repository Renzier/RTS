namespace Quantum
{
    public unsafe class SelectionSystem : SystemMainThread
    {
        private static readonly Photon.Deterministic.FP ClickSelectionRadius = Photon.Deterministic.FP.FromString("0.75");
        private static readonly Photon.Deterministic.FP DragSelectionThreshold = Photon.Deterministic.FP.FromString("0.25");

        public override void Update(Frame f)
        {
            if (f.Global->LastSelectHeld == false)
            {
                return;
            }

            bool isDragSelection = Photon.Deterministic.FPVector2.Distance(
                f.Global->LastDragStartWorld,
                f.Global->LastDragEndWorld) >= DragSelectionThreshold;

            FPRect selectionRect = FPRect.FromPoints(
                f.Global->LastDragStartWorld,
                f.Global->LastDragEndWorld);

            foreach ((EntityRef entity, SelectionCandidate candidate) in f.GetComponentIterator<SelectionCandidate>())
            {
                if (f.Unsafe.TryGetPointer<Selectable>(entity, out Selectable* selectable) == false)
                {
                    continue;
                }

                if (f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false)
                {
                    continue;
                }

                if (IsOwnedByInputPlayer(f, entity) == false)
                {
                    selectable->IsSelected = false;
                    continue;
                }

                if (IsDeadOrDestroyed(f, entity))
                {
                    selectable->IsSelected = false;
                    continue;
                }

                bool isInsideSelection = isDragSelection
                    ? selectionRect.Contains(transform->Position)
                    : Photon.Deterministic.FPVector2.Distance(transform->Position, f.Global->LastPointerWorld) <= selectable->SelectionRadius + ClickSelectionRadius;

                if (f.Global->LastAdditiveSelectHeld)
                {
                    selectable->IsSelected = selectable->IsSelected || isInsideSelection;
                }
                else
                {
                    selectable->IsSelected = isInsideSelection;
                }
            }
        }

        private static bool IsOwnedByInputPlayer(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity))
            {
                return unitIdentity->OwnerPlayer == f.Global->LastInputPlayer;
            }

            if (f.Unsafe.TryGetPointer<MainBuilding>(entity, out MainBuilding* mainBuilding))
            {
                return mainBuilding->OwnerPlayer == f.Global->LastInputPlayer;
            }

            if (f.Unsafe.TryGetPointer<SupplyBuilding>(entity, out SupplyBuilding* supplyBuilding))
            {
                return supplyBuilding->OwnerPlayer == f.Global->LastInputPlayer;
            }

            return false;
        }

        private static bool IsDeadOrDestroyed(Frame f, EntityRef entity)
        {
            if (f.Unsafe.TryGetPointer<UnitHealth>(entity, out UnitHealth* unitHealth))
            {
                return unitHealth->IsDead;
            }

            if (f.Unsafe.TryGetPointer<MainBuilding>(entity, out MainBuilding* mainBuilding))
            {
                return mainBuilding->Health <= 0;
            }

            if (f.Unsafe.TryGetPointer<SupplyBuilding>(entity, out SupplyBuilding* supplyBuilding))
            {
                return supplyBuilding->Health <= 0;
            }

            return false;
        }

        private readonly struct FPRect
        {
            private readonly Photon.Deterministic.FP _minX;
            private readonly Photon.Deterministic.FP _maxX;
            private readonly Photon.Deterministic.FP _minY;
            private readonly Photon.Deterministic.FP _maxY;

            private FPRect(
                Photon.Deterministic.FP minX,
                Photon.Deterministic.FP maxX,
                Photon.Deterministic.FP minY,
                Photon.Deterministic.FP maxY)
            {
                _minX = minX;
                _maxX = maxX;
                _minY = minY;
                _maxY = maxY;
            }

            public static FPRect FromPoints(Photon.Deterministic.FPVector2 a, Photon.Deterministic.FPVector2 b)
            {
                return new FPRect(
                    Photon.Deterministic.FPMath.Min(a.X, b.X),
                    Photon.Deterministic.FPMath.Max(a.X, b.X),
                    Photon.Deterministic.FPMath.Min(a.Y, b.Y),
                    Photon.Deterministic.FPMath.Max(a.Y, b.Y));
            }

            public bool Contains(Photon.Deterministic.FPVector2 point)
            {
                return point.X >= _minX &&
                       point.X <= _maxX &&
                       point.Y >= _minY &&
                       point.Y <= _maxY;
            }
        }
    }
}
