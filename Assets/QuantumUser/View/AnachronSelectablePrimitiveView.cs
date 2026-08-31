using System.Collections.Generic;
using Quantum;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AnachronSelectablePrimitiveView : QuantumMonoBehaviour
{
    private static readonly Color IdleColor = new Color(0.72f, 0.78f, 0.82f, 1.0f);
    private static readonly Color SelectedColor = new Color(0.1f, 0.95f, 0.45f, 1.0f);
    private static readonly Color EnemyUnitColor = new Color(0.95f, 0.18f, 0.18f, 1.0f);
    private static readonly Color HeroColor = new Color(1.0f, 0.55f, 0.16f, 1.0f);
    private static readonly Color EnemyHeroColor = new Color(1.0f, 0.12f, 0.55f, 1.0f);
    private static readonly Color ArdentConcordColor = new Color(0.42f, 0.55f, 0.64f, 1.0f);
    private static readonly Color WroughtColor = new Color(0.82f, 0.72f, 0.34f, 1.0f);
    private static readonly Color GharnColor = new Color(0.76f, 0.16f, 0.09f, 1.0f);
    private static readonly Color SeetheColor = new Color(0.28f, 0.66f, 0.42f, 1.0f);
    private static readonly Color VeirnColor = new Color(0.92f, 0.28f, 0.12f, 1.0f);
    private static readonly Color VaelunColor = new Color(0.08f, 0.09f, 0.12f, 1.0f);
    private static readonly Color NimharaColor = new Color(0.42f, 0.72f, 0.86f, 1.0f);
    private static readonly Color ViriiColor = new Color(0.62f, 0.58f, 0.52f, 1.0f);
    private static readonly Color WoodColor = new Color(0.05f, 0.85f, 0.22f, 1.0f);
    private static readonly Color IronColor = new Color(0.75f, 0.78f, 0.86f, 1.0f);
    private static readonly Color BaseColor = new Color(0.1f, 0.45f, 1.0f, 1.0f);
    private static readonly Color EnemyTargetColor = new Color(0.95f, 0.2f, 0.18f, 1.0f);
    private static readonly Color DeadUnitColor = new Color(0.08f, 0.08f, 0.08f, 1.0f);
    private static readonly Color DamagedBaseColor = new Color(1.0f, 0.6f, 0.08f, 1.0f);
    private static readonly Color DestroyedBaseColor = new Color(0.12f, 0.12f, 0.12f, 1.0f);
    private static readonly Color SupplyBuildingColor = new Color(0.25f, 0.95f, 0.85f, 1.0f);
    private static readonly Color ConcordFoundationColor = new Color(0.78f, 0.68f, 0.42f, 1.0f);
    private static readonly Color WroughtFoundationColor = new Color(0.38f, 0.36f, 0.26f, 1.0f);
    private static readonly Color GharnFoundationColor = new Color(0.36f, 0.12f, 0.08f, 1.0f);
    private static readonly Color SeetheFoundationColor = new Color(0.24f, 0.44f, 0.34f, 1.0f);
    private static readonly Color VeirnFoundationColor = new Color(0.34f, 0.08f, 0.04f, 1.0f);
    private static readonly Color VaelunFoundationColor = new Color(0.18f, 0.18f, 0.22f, 1.0f);
    private static readonly Color NimharaFoundationColor = new Color(0.22f, 0.36f, 0.42f, 1.0f);
    private static readonly Color ViriiFoundationColor = new Color(0.26f, 0.24f, 0.22f, 1.0f);
    private static readonly Color SupplyDeconstructionColor = new Color(0.95f, 0.38f, 0.12f, 1.0f);
    private static readonly Color ActiveTargetColor = new Color(1.0f, 0.08f, 0.08f, 1.0f);
    private static readonly Color AttackerMarkerColor = new Color(1.0f, 0.62f, 0.08f, 1.0f);
    private static readonly Color GroundColor = new Color(0.16f, 0.19f, 0.18f, 1.0f);
    private static readonly Color CameraSkyColor = new Color(0.05f, 0.07f, 0.09f, 1.0f);
    private static readonly Color MereBoundaryColor = new Color(0.03f, 0.16f, 0.2f, 1.0f);
    private static readonly Color ShardRidgeColor = new Color(0.22f, 0.24f, 0.22f, 1.0f);
    private static readonly Color QuillMarkerColor = new Color(0.78f, 0.68f, 0.42f, 1.0f);
    private static readonly Color QuillObjectiveColor = new Color(0.95f, 0.82f, 0.38f, 1.0f);
    private static readonly Color GrainLoudTint = new Color(0.32f, 0.82f, 1.0f, 1.0f);
    private static readonly Color HealthBarBackColor = new Color(0.01f, 0.012f, 0.012f, 1.0f);
    private static readonly Color HealthBarGoodColor = new Color(0.1f, 0.9f, 0.2f, 1.0f);
    private static readonly Color HealthBarWarnColor = new Color(1.0f, 0.75f, 0.12f, 1.0f);
    private static readonly Color HealthBarLowColor = new Color(1.0f, 0.12f, 0.08f, 1.0f);
    private const float UnitHealthBarWidth = 1.15f;
    private const float HeroHealthBarWidth = 1.65f;
    private const float BaseHealthBarWidth = 2.15f;
    private const float HealthBarHeight = 0.12f;
    private const float HealthBarDepth = 0.06f;

    private readonly Dictionary<EntityRef, Renderer> _views = new Dictionary<EntityRef, Renderer>();
    private readonly Dictionary<EntityRef, PrimitiveType> _viewPrimitiveTypes = new Dictionary<EntityRef, PrimitiveType>();
    private readonly Dictionary<EntityRef, HealthBarView> _healthBars = new Dictionary<EntityRef, HealthBarView>();
    private readonly Dictionary<EntityRef, Renderer> _attackerMarkers = new Dictionary<EntityRef, Renderer>();
    private readonly Dictionary<EntityRef, Renderer> _targetMarkers = new Dictionary<EntityRef, Renderer>();
    private readonly HashSet<EntityRef> _seenThisFrame = new HashSet<EntityRef>();
    private readonly HashSet<EntityRef> _seenHealthBarsThisFrame = new HashSet<EntityRef>();
    private readonly HashSet<EntityRef> _seenAttackerMarkersThisFrame = new HashSet<EntityRef>();
    private readonly HashSet<EntityRef> _seenTargetMarkersThisFrame = new HashSet<EntityRef>();
    private int _resourceNodeCount;
    private int _mainBuildingCount;

    private void Awake()
    {
        if (GetComponent<AnachronPrototypeHud>() == null)
        {
            gameObject.AddComponent<AnachronPrototypeHud>();
        }

        if (GetComponent<AnachronBuildPlacementPreview>() == null)
        {
            gameObject.AddComponent<AnachronBuildPlacementPreview>();
        }

        ConfigurePrototypeBattlefield();
    }

    private void LateUpdate()
    {
        QuantumRunner runner = QuantumRunner.Default;
        if (runner == null || runner.Game == null || runner.Game.Frames == null)
        {
            ClearViews();
            return;
        }

        Frame frame = runner.Game.Frames.Verified;
        if (frame == null)
        {
            ClearViews();
            return;
        }

        _seenThisFrame.Clear();
        _seenHealthBarsThisFrame.Clear();
        _seenAttackerMarkersThisFrame.Clear();
        _seenTargetMarkersThisFrame.Clear();
        _resourceNodeCount = 0;
        _mainBuildingCount = 0;

        foreach ((EntityRef entity, Transform2D transform) in frame.GetComponentIterator<Transform2D>())
        {
            bool isSelectable = HasSelectionCandidate(frame, entity);
            bool isEconomyEntity = TryGetEconomyColor(frame, entity, out Color economyColor);
            if (isSelectable == false && isEconomyEntity == false)
            {
                continue;
            }

            _seenThisFrame.Add(entity);

            PrimitiveType primitiveType = GetPrimitiveType(frame, entity, isEconomyEntity);
            Renderer view = GetOrCreateView(entity, primitiveType);
            bool isDeadUnit = IsDeadUnit(frame, entity);
            view.transform.position = transform.Position.ToUnityVector3() + GetViewPositionOffset(frame, entity, isEconomyEntity);
            view.transform.rotation = Quaternion.Euler(0.0f, -transform.Rotation.AsFloat, 0.0f);
            view.transform.localScale = GetViewScale(frame, entity, isEconomyEntity, isDeadUnit);
            view.material.color = ApplyGrainLoudTint(frame, entity, GetViewColor(frame, entity, isEconomyEntity, economyColor, isDeadUnit));

            UpdateHealthBar(frame, entity, transform.Position.ToUnityVector3(), isEconomyEntity, isDeadUnit);
        }

        UpdateTargetMarkers(frame);
        RemoveMissingViews();
        RemoveMissingHealthBars();
        RemoveMissingAttackerMarkers();
        RemoveMissingTargetMarkers();
    }

    private Renderer GetOrCreateView(EntityRef entity, PrimitiveType primitiveType)
    {
        if (_views.TryGetValue(entity, out Renderer renderer) &&
            _viewPrimitiveTypes.TryGetValue(entity, out PrimitiveType existingPrimitiveType) &&
            existingPrimitiveType == primitiveType)
        {
            return renderer;
        }

        if (renderer != null)
        {
            Destroy(renderer.material);
            Destroy(renderer.gameObject);
            _views.Remove(entity);
            _viewPrimitiveTypes.Remove(entity);
        }

        GameObject primitive = GameObject.CreatePrimitive(primitiveType);
        primitive.name = $"SelectableView_{entity}";
        primitive.transform.localScale = new Vector3(0.8f, 0.18f, 0.8f);

        renderer = primitive.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        _views.Add(entity, renderer);
        _viewPrimitiveTypes.Add(entity, primitiveType);
        return renderer;
    }

    private HealthBarView GetOrCreateHealthBar(EntityRef entity)
    {
        if (_healthBars.TryGetValue(entity, out HealthBarView healthBar))
        {
            return healthBar;
        }

        GameObject root = new GameObject($"HealthBar_{entity}");

        GameObject back = GameObject.CreatePrimitive(PrimitiveType.Cube);
        back.name = "Back";
        back.transform.SetParent(root.transform, false);
        back.transform.localScale = new Vector3(UnitHealthBarWidth, HealthBarHeight, HealthBarDepth);

        Renderer backRenderer = back.GetComponent<Renderer>();
        backRenderer.material = new Material(Shader.Find("Standard"));
        backRenderer.material.color = HealthBarBackColor;

        GameObject fill = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fill.name = "Fill";
        fill.transform.SetParent(root.transform, false);
        fill.transform.localScale = new Vector3(UnitHealthBarWidth, HealthBarHeight, HealthBarDepth);

        Renderer fillRenderer = fill.GetComponent<Renderer>();
        fillRenderer.material = new Material(Shader.Find("Standard"));

        healthBar = new HealthBarView(root, backRenderer, fillRenderer);
        _healthBars.Add(entity, healthBar);
        return healthBar;
    }

    private Renderer GetOrCreateTargetMarker(EntityRef entity)
    {
        if (_targetMarkers.TryGetValue(entity, out Renderer renderer))
        {
            return renderer;
        }

        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        primitive.name = $"AttackTargetMarker_{entity}";
        primitive.transform.localScale = new Vector3(0.42f, 0.16f, 0.42f);

        renderer = primitive.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = ActiveTargetColor;
        _targetMarkers.Add(entity, renderer);
        return renderer;
    }

    private Renderer GetOrCreateAttackerMarker(EntityRef entity)
    {
        if (_attackerMarkers.TryGetValue(entity, out Renderer renderer))
        {
            return renderer;
        }

        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        primitive.name = $"AttackerMarker_{entity}";
        primitive.transform.localScale = new Vector3(1.35f, 0.035f, 1.35f);

        renderer = primitive.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = AttackerMarkerColor;
        _attackerMarkers.Add(entity, renderer);
        return renderer;
    }

    private static void ConfigurePrototypeBattlefield()
    {
        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = CameraSkyColor;
        }

        GameObject ground = GameObject.Find("AnachronNavMeshGround");
        if (ground == null)
        {
            return;
        }

        Renderer renderer = ground.GetComponent<Renderer>();
        if (renderer == null)
        {
            return;
        }

        ground.transform.localScale = new Vector3(8.0f, 1.0f, 8.0f);
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = GroundColor;

        ConfigureKilnfallLandmarks();
    }

    private static void ConfigureKilnfallLandmarks()
    {
        if (GameObject.Find("AshensparViewLandmarks") != null)
        {
            return;
        }

        GameObject root = new GameObject("AshensparViewLandmarks");
        CreateLandmarkPrimitive(root.transform, "MereNorth", PrimitiveType.Cube, new Vector3(0.0f, -0.12f, 42.0f), new Vector3(82.0f, 0.04f, 6.0f), MereBoundaryColor);
        CreateLandmarkPrimitive(root.transform, "MereSouth", PrimitiveType.Cube, new Vector3(0.0f, -0.12f, -42.0f), new Vector3(82.0f, 0.04f, 6.0f), MereBoundaryColor);
        CreateLandmarkPrimitive(root.transform, "MereEast", PrimitiveType.Cube, new Vector3(42.0f, -0.12f, 0.0f), new Vector3(6.0f, 0.04f, 82.0f), MereBoundaryColor);
        CreateLandmarkPrimitive(root.transform, "MereWest", PrimitiveType.Cube, new Vector3(-42.0f, -0.12f, 0.0f), new Vector3(6.0f, 0.04f, 82.0f), MereBoundaryColor);
        CreateLandmarkPrimitive(root.transform, "QuillWaistSpire", PrimitiveType.Cylinder, new Vector3(0.0f, 2.1f, 22.0f), new Vector3(0.8f, 2.1f, 0.8f), QuillMarkerColor);
        CreateLandmarkPrimitive(root.transform, "QuillWaistRing", PrimitiveType.Cylinder, new Vector3(0.0f, 0.08f, 22.0f), new Vector3(4.4f, 0.06f, 4.4f), ShardRidgeColor);
        CreateLandmarkPrimitive(root.transform, "CentralQuillWaistRing", PrimitiveType.Cylinder, new Vector3(0.0f, 0.06f, 7.0f), new Vector3(2.8f, 0.04f, 2.8f), QuillMarkerColor);
        CreateLandmarkPrimitive(root.transform, "ShardRidgeWest", PrimitiveType.Cube, new Vector3(-16.0f, 0.08f, 18.0f), new Vector3(8.0f, 0.16f, 1.0f), ShardRidgeColor);
        CreateLandmarkPrimitive(root.transform, "ShardRidgeEast", PrimitiveType.Cube, new Vector3(16.0f, 0.08f, 18.0f), new Vector3(8.0f, 0.16f, 1.0f), ShardRidgeColor);
    }

    private static void CreateLandmarkPrimitive(Transform parent, string name, PrimitiveType primitiveType, Vector3 position, Vector3 scale, Color color)
    {
        GameObject primitive = GameObject.CreatePrimitive(primitiveType);
        primitive.name = name;
        primitive.transform.SetParent(parent, false);
        primitive.transform.position = position;
        primitive.transform.localScale = scale;

        Collider collider = primitive.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
            Destroy(collider);
        }

        Renderer landmarkRenderer = primitive.GetComponent<Renderer>();
        landmarkRenderer.material = new Material(Shader.Find("Standard"));
        landmarkRenderer.material.color = color;
    }

    private static bool HasSelectionCandidate(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, SelectionCandidate candidate) in frame.GetComponentIterator<SelectionCandidate>())
        {
            if (entity == candidateEntity)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSelected(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, Selectable selectable) in frame.GetComponentIterator<Selectable>())
        {
            if (entity == candidateEntity)
            {
                return selectable.IsSelected;
            }
        }

        return false;
    }

    private static Color GetUnitColor(Frame frame, EntityRef entity, bool isDeadUnit)
    {
        if (isDeadUnit)
        {
            return DeadUnitColor;
        }

        if (IsSelected(frame, entity))
        {
            return SelectedColor;
        }

        int ownerPlayer = GetOwnerPlayer(frame, entity);
        if (IsHero(frame, entity))
        {
            return ownerPlayer == QuantumPhase0LocalSessionController.ActivePlayerSlot ? HeroColor : GetFactionColor(frame, ownerPlayer);
        }

        return ownerPlayer == QuantumPhase0LocalSessionController.ActivePlayerSlot ? IdleColor : GetFactionColor(frame, ownerPlayer);
    }

    private static Color ApplyGrainLoudTint(Frame frame, EntityRef entity, Color baseColor)
    {
        foreach ((EntityRef grainEntity, GrainState grainState) in frame.GetComponentIterator<GrainState>())
        {
            if (grainEntity == entity && grainState.IsGrainLoud && grainState.GrainLoudTicksRemaining > 0)
            {
                return Color.Lerp(baseColor, GrainLoudTint, 0.45f);
            }
        }

        return baseColor;
    }

    private static Vector3 GetUnitScale(Frame frame, EntityRef entity, bool isDeadUnit)
    {
        if (IsQuillObjective(frame, entity))
        {
            return new Vector3(0.75f, 1.35f, 0.75f);
        }

        if (isDeadUnit)
        {
            return new Vector3(0.8f, 0.05f, 0.8f);
        }

        int factionId = GetFactionId(frame, GetOwnerPlayer(frame, entity));
        bool isHero = IsHero(frame, entity);
        if (factionId == FactionId.Wrought)
        {
            return isHero ? new Vector3(0.95f, 1.25f, 0.95f) : new Vector3(0.72f, 0.5f, 0.72f);
        }

        if (factionId == FactionId.Gharn)
        {
            return isHero ? new Vector3(1.05f, 0.95f, 1.05f) : new Vector3(0.82f, 0.36f, 0.82f);
        }

        if (factionId == FactionId.Seethe)
        {
            return isHero ? new Vector3(0.86f, 1.32f, 0.86f) : new Vector3(0.68f, 0.72f, 0.68f);
        }

        if (factionId == FactionId.Veirn)
        {
            return isHero ? new Vector3(0.82f, 1.12f, 0.82f) : new Vector3(0.62f, 0.62f, 0.62f);
        }

        if (factionId == FactionId.Vaelun)
        {
            return isHero ? new Vector3(1.08f, 0.76f, 1.08f) : new Vector3(0.9f, 0.48f, 0.9f);
        }

        if (factionId == FactionId.Nimhara)
        {
            return isHero ? new Vector3(0.78f, 1.18f, 0.78f) : new Vector3(0.64f, 0.54f, 0.64f);
        }

        if (factionId == FactionId.Virii)
        {
            return isHero ? new Vector3(0.86f, 0.86f, 0.86f) : new Vector3(0.66f, 0.44f, 0.66f);
        }

        return isHero ? new Vector3(0.9f, 0.82f, 0.9f) : new Vector3(0.7f, 0.24f, 0.7f);
    }

    private static Vector3 GetEconomyScale(Frame frame, EntityRef entity)
    {
        foreach ((EntityRef supplyEntity, SupplyBuilding supplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            if (supplyEntity == entity)
            {
                int factionId = GetFactionId(frame, supplyBuilding.OwnerPlayer);
                if (factionId == FactionId.Wrought)
                {
                    if (supplyBuilding.IsDeconstructing)
                    {
                        return new Vector3(1.18f, 0.72f, 1.18f);
                    }

                    return supplyBuilding.IsConstructing ? new Vector3(1.28f, 0.2f, 1.28f) : new Vector3(1.18f, 1.05f, 1.18f);
                }

                if (factionId == FactionId.Gharn)
                {
                    if (supplyBuilding.IsDeconstructing)
                    {
                        return new Vector3(1.45f, 0.36f, 1.45f);
                    }

                    return supplyBuilding.IsConstructing ? new Vector3(1.45f, 0.18f, 1.45f) : new Vector3(1.45f, 0.62f, 1.45f);
                }

                if (factionId == FactionId.Seethe)
                {
                    if (supplyBuilding.IsDeconstructing)
                    {
                        return new Vector3(1.08f, 0.54f, 1.08f);
                    }

                    return supplyBuilding.IsConstructing ? new Vector3(1.18f, 0.2f, 1.18f) : new Vector3(1.08f, 0.92f, 1.08f);
                }

                if (factionId == FactionId.Veirn)
                {
                    if (supplyBuilding.IsDeconstructing)
                    {
                        return new Vector3(0.98f, 0.82f, 0.98f);
                    }

                    return supplyBuilding.IsConstructing ? new Vector3(1.12f, 0.18f, 1.12f) : new Vector3(0.98f, 1.18f, 0.98f);
                }

                if (factionId == FactionId.Vaelun)
                {
                    if (supplyBuilding.IsDeconstructing)
                    {
                        return new Vector3(1.32f, 0.34f, 1.32f);
                    }

                    return supplyBuilding.IsConstructing ? new Vector3(1.5f, 0.14f, 1.5f) : new Vector3(1.32f, 0.5f, 1.32f);
                }

                if (factionId == FactionId.Nimhara)
                {
                    if (supplyBuilding.IsDeconstructing)
                    {
                        return new Vector3(1.22f, 0.48f, 1.22f);
                    }

                    return supplyBuilding.IsConstructing ? new Vector3(1.34f, 0.16f, 1.34f) : new Vector3(1.22f, 0.74f, 1.22f);
                }

                if (factionId == FactionId.Virii)
                {
                    if (supplyBuilding.IsDeconstructing)
                    {
                        return new Vector3(1.05f, 0.4f, 1.05f);
                    }

                    return supplyBuilding.IsConstructing ? new Vector3(1.28f, 0.12f, 1.28f) : new Vector3(1.05f, 0.64f, 1.05f);
                }

                if (supplyBuilding.IsDeconstructing)
                {
                    return new Vector3(1.25f, 0.42f, 1.25f);
                }

                return supplyBuilding.IsConstructing ? new Vector3(1.45f, 0.18f, 1.45f) : new Vector3(1.25f, 0.68f, 1.25f);
            }
        }

        int buildingTier = GetBuildingTier(frame, entity);
        if (buildingTier > 1)
        {
            float size = 1.35f + (buildingTier - 1) * 0.22f;
            return new Vector3(size, 1.0f, size);
        }

        return new Vector3(1.35f, 1.0f, 1.35f);
    }

    private static PrimitiveType GetPrimitiveType(Frame frame, EntityRef entity, bool isEconomyEntity)
    {
        if (IsQuillObjective(frame, entity))
        {
            return PrimitiveType.Cylinder;
        }

        if (isEconomyEntity)
        {
            if (TryGetMainBuildingOwner(frame, entity, out int ownerPlayer) == false)
            {
                return PrimitiveType.Cube;
            }

            int factionId = GetFactionId(frame, ownerPlayer);
            if (factionId == FactionId.Wrought)
            {
                return PrimitiveType.Cube;
            }

            if (factionId == FactionId.Gharn)
            {
                return PrimitiveType.Cylinder;
            }

            if (factionId == FactionId.Seethe)
            {
                return PrimitiveType.Cylinder;
            }

            if (factionId == FactionId.Veirn)
            {
                return PrimitiveType.Cylinder;
            }

            if (factionId == FactionId.Vaelun)
            {
                return PrimitiveType.Cube;
            }

            if (factionId == FactionId.Nimhara)
            {
                return PrimitiveType.Cylinder;
            }

            if (factionId == FactionId.Virii)
            {
                return PrimitiveType.Sphere;
            }

            return PrimitiveType.Cube;
        }

        int unitFactionId = GetFactionId(frame, GetOwnerPlayer(frame, entity));
        if (unitFactionId == FactionId.Wrought)
        {
            return PrimitiveType.Cylinder;
        }

        if (unitFactionId == FactionId.Gharn)
        {
            return PrimitiveType.Capsule;
        }

        if (unitFactionId == FactionId.Seethe)
        {
            return PrimitiveType.Sphere;
        }

        if (unitFactionId == FactionId.Veirn)
        {
            return PrimitiveType.Capsule;
        }

        if (unitFactionId == FactionId.Vaelun)
        {
            return PrimitiveType.Cube;
        }

        if (unitFactionId == FactionId.Nimhara)
        {
            return PrimitiveType.Cylinder;
        }

        if (unitFactionId == FactionId.Virii)
        {
            return PrimitiveType.Sphere;
        }

        return PrimitiveType.Cube;
    }

    private static Vector3 GetViewPositionOffset(Frame frame, EntityRef entity, bool isEconomyEntity)
    {
        if (IsQuillObjective(frame, entity))
        {
            return new Vector3(0.0f, 0.85f, 0.0f);
        }

        return isEconomyEntity ? new Vector3(0.0f, 0.5f, 0.0f) : Vector3.zero;
    }

    private static Vector3 GetViewScale(Frame frame, EntityRef entity, bool isEconomyEntity, bool isDeadUnit)
    {
        return isEconomyEntity ? GetEconomyScale(frame, entity) : GetUnitScale(frame, entity, isDeadUnit);
    }

    private static Color GetViewColor(Frame frame, EntityRef entity, bool isEconomyEntity, Color economyColor, bool isDeadUnit)
    {
        if (IsQuillObjective(frame, entity))
        {
            return IsSelected(frame, entity) ? SelectedColor : QuillObjectiveColor;
        }

        return isEconomyEntity ? economyColor : GetUnitColor(frame, entity, isDeadUnit);
    }

    private static bool TryGetMainBuildingOwner(Frame frame, EntityRef candidateEntity, out int ownerPlayer)
    {
        foreach ((EntityRef entity, MainBuilding building) in frame.GetComponentIterator<MainBuilding>())
        {
            if (entity == candidateEntity)
            {
                ownerPlayer = building.OwnerPlayer;
                return true;
            }
        }

        foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            if (entity == candidateEntity)
            {
                ownerPlayer = supplyBuilding.OwnerPlayer;
                return true;
            }
        }

        ownerPlayer = 0;
        return false;
    }

    private static int GetBuildingTier(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, BuildingTier buildingTier) in frame.GetComponentIterator<BuildingTier>())
        {
            if (entity == candidateEntity)
            {
                return buildingTier.Tier;
            }
        }

        return 1;
    }

    private void UpdateHealthBar(Frame frame, EntityRef entity, Vector3 worldPosition, bool isEconomyEntity, bool isDeadUnit)
    {
        if (TryGetHealth(frame, entity, out int health, out int maxHealth) == false || maxHealth <= 0)
        {
            RemoveHealthBar(entity);
            return;
        }

        _seenHealthBarsThisFrame.Add(entity);

        HealthBarView healthBar = GetOrCreateHealthBar(entity);
        float normalizedHealth = Mathf.Clamp01((float)health / maxHealth);
        bool isHero = IsHero(frame, entity);
        bool isQuillObjective = IsQuillObjective(frame, entity);
        float barWidth = isEconomyEntity || isQuillObjective ? BaseHealthBarWidth : isHero ? HeroHealthBarWidth : UnitHealthBarWidth;
        float yOffset = isQuillObjective ? 1.95f : isEconomyEntity ? 1.65f : isHero ? 1.05f : 0.72f;
        healthBar.Root.transform.position = worldPosition + new Vector3(0.0f, yOffset, 0.0f);
        healthBar.Root.transform.rotation = Quaternion.Euler(55.0f, 0.0f, 0.0f);
        healthBar.Root.SetActive(isDeadUnit == false || health > 0);

        healthBar.Back.transform.localScale = new Vector3(barWidth, HealthBarHeight, HealthBarDepth);
        healthBar.Fill.transform.localScale = new Vector3(barWidth * normalizedHealth, HealthBarHeight, HealthBarDepth);
        healthBar.Fill.transform.localPosition = new Vector3(-(barWidth - barWidth * normalizedHealth) * 0.5f, 0.018f, -0.004f);
        healthBar.Fill.material.color = GetHealthColor(normalizedHealth);
    }

    private static bool TryGetHealth(Frame frame, EntityRef candidateEntity, out int health, out int maxHealth)
    {
        foreach ((EntityRef entity, UnitHealth unitHealth) in frame.GetComponentIterator<UnitHealth>())
        {
            if (entity == candidateEntity)
            {
                health = unitHealth.Health;
                maxHealth = unitHealth.MaxHealth;
                return true;
            }
        }

        foreach ((EntityRef entity, MainBuilding mainBuilding) in frame.GetComponentIterator<MainBuilding>())
        {
            if (entity == candidateEntity)
            {
                health = mainBuilding.Health;
                maxHealth = mainBuilding.MaxHealth;
                return true;
            }
        }

        foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            if (entity == candidateEntity)
            {
                health = supplyBuilding.Health;
                maxHealth = supplyBuilding.MaxHealth;
                return true;
            }
        }

        foreach ((EntityRef entity, Targetable targetable) in frame.GetComponentIterator<Targetable>())
        {
            if (entity == candidateEntity && IsQuillObjective(frame, entity))
            {
                health = targetable.Health;
                maxHealth = targetable.MaxHealth;
                return true;
            }
        }

        health = 0;
        maxHealth = 0;
        return false;
    }

    private static Color GetHealthColor(float normalizedHealth)
    {
        if (normalizedHealth <= 0.3f)
        {
            return HealthBarLowColor;
        }

        if (normalizedHealth <= 0.6f)
        {
            return HealthBarWarnColor;
        }

        return HealthBarGoodColor;
    }

    private static bool IsHero(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (entity == candidateEntity)
            {
                return unitIdentity.UnitKind == UnitKind.Hero;
            }
        }

        return false;
    }

    private static bool IsOwnedByLocalPlayer(Frame frame, EntityRef candidateEntity)
    {
        return GetOwnerPlayer(frame, candidateEntity) == QuantumPhase0LocalSessionController.ActivePlayerSlot;
    }

    private static int GetOwnerPlayer(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (entity == candidateEntity)
            {
                return unitIdentity.OwnerPlayer;
            }
        }

        return 0;
    }

    private static bool IsQuillObjective(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, Targetable targetable) in frame.GetComponentIterator<Targetable>())
        {
            if (entity == candidateEntity)
            {
                return TryGetTransform(frame, entity, out Transform2D transform) &&
                       QuillObjective.IsObjectivePosition(transform.Position);
            }
        }

        return false;
    }

    private static bool IsDeadUnit(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, UnitHealth unitHealth) in frame.GetComponentIterator<UnitHealth>())
        {
            if (entity == candidateEntity)
            {
                return unitHealth.IsDead;
            }
        }

        return false;
    }

    private bool TryGetEconomyColor(Frame frame, EntityRef candidateEntity, out Color color)
    {
        foreach ((EntityRef entity, ResourceNode node) in frame.GetComponentIterator<ResourceNode>())
        {
            if (entity == candidateEntity)
            {
                _resourceNodeCount++;
                color = node.ResourceKind == ResourceKind.Wood ? WoodColor : IronColor;
                return true;
            }
        }

        foreach ((EntityRef entity, MainBuilding building) in frame.GetComponentIterator<MainBuilding>())
        {
            if (entity == candidateEntity)
            {
                _mainBuildingCount++;
                color = GetBaseColor(frame, building);
                return true;
            }
        }

        foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            if (entity == candidateEntity)
            {
                color = GetSupplyColor(frame, supplyBuilding);
                return true;
            }
        }

        color = default;
        return false;
    }

    private static Color GetSupplyColor(Frame frame, SupplyBuilding supplyBuilding)
    {
        if (supplyBuilding.Health <= 0)
        {
            return DestroyedBaseColor;
        }

        if (supplyBuilding.IsDeconstructing)
        {
            return SupplyDeconstructionColor;
        }

        if (supplyBuilding.IsConstructing)
        {
            return GetSupplyFoundationColor(frame, supplyBuilding.OwnerPlayer);
        }

        return SupplyBuildingColor;
    }

    private static Color GetSupplyFoundationColor(Frame frame, int ownerPlayer)
    {
        int factionId = GetFactionId(frame, ownerPlayer);
        if (factionId == FactionId.Wrought)
        {
            return WroughtFoundationColor;
        }

        if (factionId == FactionId.Gharn)
        {
            return GharnFoundationColor;
        }

        if (factionId == FactionId.Seethe)
        {
            return SeetheFoundationColor;
        }

        if (factionId == FactionId.Veirn)
        {
            return VeirnFoundationColor;
        }

        if (factionId == FactionId.Vaelun)
        {
            return VaelunFoundationColor;
        }

        if (factionId == FactionId.Nimhara)
        {
            return NimharaFoundationColor;
        }

        if (factionId == FactionId.Virii)
        {
            return ViriiFoundationColor;
        }

        return ConcordFoundationColor;
    }

    private static Color GetBaseColor(Frame frame, MainBuilding building)
    {
        if (building.Health <= 0)
        {
            return DestroyedBaseColor;
        }

        if (building.Health < building.MaxHealth)
        {
            return DamagedBaseColor;
        }

        return GetFactionColor(frame, building.OwnerPlayer);
    }

    private static Color GetFactionColor(Frame frame, int playerIndex)
    {
        int factionId = GetFactionId(frame, playerIndex);
        if (factionId == FactionId.Wrought)
        {
            return WroughtColor;
        }

        if (factionId == FactionId.Gharn)
        {
            return GharnColor;
        }

        if (factionId == FactionId.Seethe)
        {
            return SeetheColor;
        }

        if (factionId == FactionId.Veirn)
        {
            return VeirnColor;
        }

        if (factionId == FactionId.Vaelun)
        {
            return VaelunColor;
        }

        if (factionId == FactionId.Nimhara)
        {
            return NimharaColor;
        }

        if (factionId == FactionId.Virii)
        {
            return ViriiColor;
        }

        return ArdentConcordColor;
    }

    private static int GetFactionId(Frame frame, int playerIndex)
    {
        foreach ((EntityRef entity, PlayerFactionState factionState) in frame.GetComponentIterator<PlayerFactionState>())
        {
            if (factionState.PlayerIndex == playerIndex)
            {
                return FactionId.Normalize(factionState.FactionId);
            }
        }

        return FactionId.ArdentConcord;
    }

    private void UpdateTargetMarkers(Frame frame)
    {
        foreach ((EntityRef entity, AttackIntent attackIntent) in frame.GetComponentIterator<AttackIntent>())
        {
            if (attackIntent.HasTarget == false)
            {
                continue;
            }

            if (TryGetTransform(frame, attackIntent.TargetEntity, out Transform2D targetTransform) == false)
            {
                continue;
            }

            if (TryGetTransform(frame, entity, out Transform2D attackerTransform))
            {
                _seenAttackerMarkersThisFrame.Add(entity);
                Renderer attackerMarker = GetOrCreateAttackerMarker(entity);
                attackerMarker.transform.position = attackerTransform.Position.ToUnityVector3() + new Vector3(0.0f, 0.055f, 0.0f);
                attackerMarker.transform.localScale = IsHero(frame, entity) ? new Vector3(1.6f, 0.035f, 1.6f) : new Vector3(1.25f, 0.035f, 1.25f);
            }

            _seenTargetMarkersThisFrame.Add(attackIntent.TargetEntity);
            Renderer marker = GetOrCreateTargetMarker(attackIntent.TargetEntity);
            marker.transform.position = targetTransform.Position.ToUnityVector3() + new Vector3(0.0f, 0.12f, 0.0f);
            marker.transform.localScale = IsHero(frame, attackIntent.TargetEntity) ? new Vector3(1.35f, 0.055f, 1.35f) : new Vector3(1.05f, 0.055f, 1.05f);
        }
    }

    private void RemoveMissingViews()
    {
        List<EntityRef> staleEntities = null;
        foreach (EntityRef entity in _views.Keys)
        {
            if (_seenThisFrame.Contains(entity))
            {
                continue;
            }

            staleEntities ??= new List<EntityRef>();
            staleEntities.Add(entity);
        }

        if (staleEntities == null)
        {
            return;
        }

        foreach (EntityRef entity in staleEntities)
        {
            Renderer renderer = _views[entity];
            Destroy(renderer.material);
            Destroy(renderer.gameObject);
            _views.Remove(entity);
            _viewPrimitiveTypes.Remove(entity);
        }
    }

    private void RemoveMissingHealthBars()
    {
        List<EntityRef> staleEntities = null;
        foreach (EntityRef entity in _healthBars.Keys)
        {
            if (_seenHealthBarsThisFrame.Contains(entity))
            {
                continue;
            }

            staleEntities ??= new List<EntityRef>();
            staleEntities.Add(entity);
        }

        if (staleEntities == null)
        {
            return;
        }

        foreach (EntityRef entity in staleEntities)
        {
            RemoveHealthBar(entity);
        }
    }

    private void RemoveMissingAttackerMarkers()
    {
        List<EntityRef> staleEntities = null;
        foreach (EntityRef entity in _attackerMarkers.Keys)
        {
            if (_seenAttackerMarkersThisFrame.Contains(entity))
            {
                continue;
            }

            staleEntities ??= new List<EntityRef>();
            staleEntities.Add(entity);
        }

        if (staleEntities == null)
        {
            return;
        }

        foreach (EntityRef entity in staleEntities)
        {
            Renderer renderer = _attackerMarkers[entity];
            Destroy(renderer.material);
            Destroy(renderer.gameObject);
            _attackerMarkers.Remove(entity);
        }
    }

    private void RemoveHealthBar(EntityRef entity)
    {
        if (_healthBars.TryGetValue(entity, out HealthBarView healthBar) == false)
        {
            return;
        }

        Destroy(healthBar.Back.material);
        Destroy(healthBar.Fill.material);
        Destroy(healthBar.Root);
        _healthBars.Remove(entity);
    }

    private void RemoveMissingTargetMarkers()
    {
        List<EntityRef> staleEntities = null;
        foreach (EntityRef entity in _targetMarkers.Keys)
        {
            if (_seenTargetMarkersThisFrame.Contains(entity))
            {
                continue;
            }

            staleEntities ??= new List<EntityRef>();
            staleEntities.Add(entity);
        }

        if (staleEntities == null)
        {
            return;
        }

        foreach (EntityRef entity in staleEntities)
        {
            Renderer renderer = _targetMarkers[entity];
            Destroy(renderer.material);
            Destroy(renderer.gameObject);
            _targetMarkers.Remove(entity);
        }
    }

    private static bool TryGetTransform(Frame frame, EntityRef candidateEntity, out Transform2D transform)
    {
        foreach ((EntityRef entity, Transform2D candidateTransform) in frame.GetComponentIterator<Transform2D>())
        {
            if (entity == candidateEntity)
            {
                transform = candidateTransform;
                return true;
            }
        }

        transform = default;
        return false;
    }

    private void ClearViews()
    {
        foreach (Renderer renderer in _views.Values)
        {
            if (renderer != null)
            {
                Destroy(renderer.material);
                Destroy(renderer.gameObject);
            }
        }

        _views.Clear();
        _viewPrimitiveTypes.Clear();

        foreach (HealthBarView healthBar in _healthBars.Values)
        {
            if (healthBar.Root != null)
            {
                Destroy(healthBar.Back.material);
                Destroy(healthBar.Fill.material);
                Destroy(healthBar.Root);
            }
        }

        _healthBars.Clear();

        foreach (Renderer renderer in _attackerMarkers.Values)
        {
            if (renderer != null)
            {
                Destroy(renderer.material);
                Destroy(renderer.gameObject);
            }
        }

        _attackerMarkers.Clear();

        foreach (Renderer renderer in _targetMarkers.Values)
        {
            if (renderer != null)
            {
                Destroy(renderer.material);
                Destroy(renderer.gameObject);
            }
        }

        _targetMarkers.Clear();
    }

    private void OnDestroy()
    {
        ClearViews();
    }

    private readonly struct HealthBarView
    {
        public readonly GameObject Root;
        public readonly Renderer Back;
        public readonly Renderer Fill;

        public HealthBarView(GameObject root, Renderer back, Renderer fill)
        {
            Root = root;
            Back = back;
            Fill = fill;
        }
    }
}
