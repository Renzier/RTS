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

            EntityRef clickedEntity = EntityRef.None;
            if (isDragSelection == false)
            {
                clickedEntity = FindClickedEntity(f);
            }

            SelectionClass targetSelectionClass = isDragSelection
                ? GetDragSelectionClass(f, selectionRect)
                : GetSelectionClass(f, clickedEntity);
            SelectionClass currentSelectionClass = f.Global->LastAdditiveSelectHeld
                ? GetCurrentSelectionClass(f)
                : SelectionClass.None;
            bool canPreserveSelection = f.Global->LastAdditiveSelectHeld &&
                targetSelectionClass != SelectionClass.None &&
                (currentSelectionClass == SelectionClass.None || currentSelectionClass == targetSelectionClass);

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
                    : entity == clickedEntity;
                if (isInsideSelection && GetSelectionClass(f, entity) != targetSelectionClass)
                {
                    isInsideSelection = false;
                }

                if (canPreserveSelection)
                {
                    selectable->IsSelected = selectable->IsSelected || isInsideSelection;
                }
                else
                {
                    selectable->IsSelected = isInsideSelection;
                }
            }
        }

        private static EntityRef FindClickedEntity(Frame f)
        {
            EntityRef bestEntity = EntityRef.None;
            Photon.Deterministic.FP bestDistance = default;
            bool hasBestEntity = false;

            foreach ((EntityRef entity, SelectionCandidate candidate) in f.GetComponentIterator<SelectionCandidate>())
            {
                if (f.Unsafe.TryGetPointer<Selectable>(entity, out Selectable* selectable) == false ||
                    f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false ||
                    IsOwnedByInputPlayer(f, entity) == false ||
                    IsDeadOrDestroyed(f, entity))
                {
                    continue;
                }

                Photon.Deterministic.FP distance = Photon.Deterministic.FPVector2.Distance(transform->Position, f.Global->LastPointerWorld);
                if (distance > selectable->SelectionRadius + ClickSelectionRadius)
                {
                    continue;
                }

                if (hasBestEntity == false || distance < bestDistance)
                {
                    bestEntity = entity;
                    bestDistance = distance;
                    hasBestEntity = true;
                }
            }

            return bestEntity;
        }

        private static SelectionClass GetDragSelectionClass(Frame f, FPRect selectionRect)
        {
            bool hasStructureSelection = false;

            foreach ((EntityRef entity, SelectionCandidate candidate) in f.GetComponentIterator<SelectionCandidate>())
            {
                if (f.Unsafe.TryGetPointer<Selectable>(entity, out Selectable* selectable) == false ||
                    f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) == false ||
                    IsOwnedByInputPlayer(f, entity) == false ||
                    IsDeadOrDestroyed(f, entity) ||
                    selectionRect.Contains(transform->Position) == false)
                {
                    continue;
                }

                SelectionClass selectionClass = GetSelectionClass(f, entity);
                if (selectionClass == SelectionClass.Unit)
                {
                    return SelectionClass.Unit;
                }

                if (selectionClass == SelectionClass.Structure)
                {
                    hasStructureSelection = true;
                }
            }

            return hasStructureSelection ? SelectionClass.Structure : SelectionClass.None;
        }

        private static SelectionClass GetCurrentSelectionClass(Frame f)
        {
            SelectionClass currentClass = SelectionClass.None;

            foreach ((EntityRef entity, SelectionCandidate candidate) in f.GetComponentIterator<SelectionCandidate>())
            {
                if (f.Unsafe.TryGetPointer<Selectable>(entity, out Selectable* selectable) == false ||
                    selectable->IsSelected == false)
                {
                    continue;
                }

                SelectionClass selectedClass = GetSelectionClass(f, entity);
                if (selectedClass == SelectionClass.None)
                {
                    continue;
                }

                if (currentClass == SelectionClass.None)
                {
                    currentClass = selectedClass;
                    continue;
                }

                if (currentClass != selectedClass)
                {
                    return SelectionClass.None;
                }
            }

            return currentClass;
        }

        private static SelectionClass GetSelectionClass(Frame f, EntityRef entity)
        {
            if (entity == EntityRef.None)
            {
                return SelectionClass.None;
            }

            if (f.Unsafe.TryGetPointer<UnitIdentity>(entity, out UnitIdentity* unitIdentity))
            {
                return SelectionClass.Unit;
            }

            if (f.Unsafe.TryGetPointer<MainBuilding>(entity, out MainBuilding* mainBuilding) ||
                f.Unsafe.TryGetPointer<SupplyBuilding>(entity, out SupplyBuilding* supplyBuilding) ||
                IsQuillObjective(f, entity))
            {
                return SelectionClass.Structure;
            }

            return SelectionClass.None;
        }

        private static bool IsOwnedByInputPlayer(Frame f, EntityRef entity)
        {
            if (IsQuillObjective(f, entity))
            {
                return true;
            }

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

        private static bool IsQuillObjective(Frame f, EntityRef entity)
        {
            return f.Unsafe.TryGetPointer<Transform2D>(entity, out Transform2D* transform) &&
                   QuillObjective.IsObjectivePosition(transform->Position);
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

        private enum SelectionClass
        {
            None,
            Unit,
            Structure
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
