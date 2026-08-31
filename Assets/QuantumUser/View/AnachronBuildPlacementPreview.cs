using Quantum;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AnachronBuildPlacementPreview : QuantumMonoBehaviour
{
    private static readonly Color ValidColor = new Color(0.15f, 1.0f, 0.45f, 0.42f);
    private static readonly Color InvalidColor = new Color(1.0f, 0.15f, 0.12f, 0.42f);
    private static readonly Color GridColor = new Color(0.68f, 0.82f, 0.88f, 0.24f);
    private const float BuildRange = 5.0f;
    private const float PlacementRadius = 1.35f;
    private const float UnitBlockRadius = 0.85f;
    private const float MapHalfExtent = 68.0f;
    private const float GridExtent = 6.0f;
    private const float GridSpacing = 1.5f;
    private const float GridLineThickness = 0.035f;
    private const float GridLineHeight = 0.04f;
    public static string PlacementStatus { get; private set; } = string.Empty;

    private Renderer _previewRenderer;
    private Transform _gridRoot;

    private void Awake()
    {
        GameObject preview = GameObject.CreatePrimitive(PrimitiveType.Cube);
        preview.name = "SupplyPlacementPreview";
        preview.transform.localScale = new Vector3(2.45f, 0.14f, 2.45f);
        preview.SetActive(false);

        _previewRenderer = preview.GetComponent<Renderer>();
        _previewRenderer.material = new Material(Shader.Find("Standard"));
        ConfigureTransparentMaterial(_previewRenderer.material);
        _previewRenderer.material.color = ValidColor;

        _gridRoot = new GameObject("SupplyPlacementGridPreview").transform;
        _gridRoot.gameObject.SetActive(false);
        CreateGridLines(_gridRoot);
    }

    private void LateUpdate()
    {
        if (_previewRenderer == null)
        {
            return;
        }

        QuantumRunner runner = QuantumRunner.Default;
        if (runner == null || runner.Game == null || runner.Game.Frames == null)
        {
            SetPreviewActive(false);
            PlacementStatus = string.Empty;
            return;
        }

        Frame frame = runner.Game.Frames.Verified;
        if (frame == null ||
            AnachronQuantumInput.BuildModeActive == false ||
            HasSelectedOwnedWorker(frame, out Vector2 workerPosition) == false)
        {
            SetPreviewActive(false);
            PlacementStatus = string.Empty;
            return;
        }

        Vector2 buildPoint = AnachronQuantumInput.CurrentPointerWorld;
        bool isValid = TryGetPlacementStatus(frame, workerPosition, buildPoint, out string placementStatus);
        PlacementStatus = placementStatus;

        SetPreviewActive(true);
        _previewRenderer.transform.position = new Vector3(buildPoint.x, 0.16f, buildPoint.y);
        _previewRenderer.material.color = isValid ? ValidColor : InvalidColor;
        _gridRoot.position = new Vector3(buildPoint.x, GridLineHeight, buildPoint.y);
    }

    private void OnDestroy()
    {
        if (_previewRenderer == null)
        {
            return;
        }

        Destroy(_previewRenderer.material);
        Destroy(_previewRenderer.gameObject);

        if (_gridRoot != null)
        {
            foreach (Transform child in _gridRoot)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    Destroy(childRenderer.material);
                }
            }

            Destroy(_gridRoot.gameObject);
        }
    }

    private void SetPreviewActive(bool isActive)
    {
        _previewRenderer.gameObject.SetActive(isActive);
        if (_gridRoot != null)
        {
            _gridRoot.gameObject.SetActive(isActive);
        }
    }

    private static void CreateGridLines(Transform root)
    {
        int lineCount = Mathf.FloorToInt(GridExtent / GridSpacing);
        for (int i = -lineCount; i <= lineCount; i++)
        {
            float offset = i * GridSpacing;
            CreateGridLine(root, $"GridX_{i}", new Vector3(0.0f, 0.0f, offset), new Vector3(GridExtent * 2.0f, GridLineThickness, GridLineThickness));
            CreateGridLine(root, $"GridZ_{i}", new Vector3(offset, 0.0f, 0.0f), new Vector3(GridLineThickness, GridLineThickness, GridExtent * 2.0f));
        }
    }

    private static void CreateGridLine(Transform root, string name, Vector3 localPosition, Vector3 localScale)
    {
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        line.name = name;
        line.transform.SetParent(root, false);
        line.transform.localPosition = localPosition;
        line.transform.localScale = localScale;

        Collider collider = line.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
            Destroy(collider);
        }

        Renderer renderer = line.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        ConfigureTransparentMaterial(renderer.material);
        renderer.material.color = GridColor;
    }

    private static bool HasSelectedOwnedWorker(Frame frame, out Vector2 workerPosition)
    {
        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (unitIdentity.OwnerPlayer != QuantumPhase0LocalSessionController.ActivePlayerSlot || unitIdentity.UnitKind != UnitKind.Worker || IsSelected(frame, entity) == false || IsDeadUnit(frame, entity))
            {
                continue;
            }

            if (TryGetPosition(frame, entity, out workerPosition))
            {
                return true;
            }
        }

        workerPosition = Vector2.zero;
        return false;
    }

    private static void ConfigureTransparentMaterial(Material material)
    {
        material.SetFloat("_Mode", 3.0f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }

    private static bool TryGetPlacementStatus(Frame frame, Vector2 workerPosition, Vector2 buildPoint, out string status)
    {
        int playerIndex = QuantumPhase0LocalSessionController.ActivePlayerSlot;
        FactionStats stats = FactionStats.ForPlayer(frame, playerIndex);
        if (TryGetPlayerEconomy(frame, playerIndex, out PlayerEconomyState economyState) == false ||
            economyState.Wood < stats.SupplyBuildingWoodCost ||
            economyState.Iron < stats.SupplyBuildingIronCost)
        {
            status = "Need more Salvage or Plate";
            return false;
        }

        if (Vector2.Distance(workerPosition, buildPoint) > BuildRange)
        {
            status = "Too far from builder";
            return false;
        }

        if (buildPoint.x < -MapHalfExtent || buildPoint.x > MapHalfExtent ||
            buildPoint.y < -MapHalfExtent || buildPoint.y > MapHalfExtent)
        {
            status = "Outside build area";
            return false;
        }

        foreach ((EntityRef entity, ResourceNode resourceNode) in frame.GetComponentIterator<ResourceNode>())
        {
            if (IsTooClose(frame, entity, buildPoint, PlacementRadius + 1.25f))
            {
                status = "Too close to resource";
                return false;
            }
        }

        foreach ((EntityRef entity, MainBuilding mainBuilding) in frame.GetComponentIterator<MainBuilding>())
        {
            if (mainBuilding.Health > 0 && IsTooClose(frame, entity, buildPoint, PlacementRadius + 1.6f))
            {
                status = "Too close to building";
                return false;
            }
        }

        foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            if (supplyBuilding.Health > 0 && IsTooClose(frame, entity, buildPoint, PlacementRadius + 1.2f))
            {
                status = "Too close to support";
                return false;
            }
        }

        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (IsTooClose(frame, entity, buildPoint, PlacementRadius + UnitBlockRadius))
            {
                status = "Too close to unit";
                return false;
            }
        }

        status = "Valid placement";
        return true;
    }

    private static bool TryGetPlayerEconomy(Frame frame, int playerIndex, out PlayerEconomyState economyState)
    {
        foreach ((EntityRef entity, PlayerEconomyState candidateState) in frame.GetComponentIterator<PlayerEconomyState>())
        {
            if (candidateState.PlayerIndex == playerIndex)
            {
                economyState = candidateState;
                return true;
            }
        }

        economyState = default;
        return false;
    }

    private static bool IsTooClose(Frame frame, EntityRef entity, Vector2 buildPoint, float blockedDistance)
    {
        if (TryGetPosition(frame, entity, out Vector2 position) == false)
        {
            return false;
        }

        return Vector2.Distance(position, buildPoint) < blockedDistance;
    }

    private static bool TryGetPosition(Frame frame, EntityRef candidateEntity, out Vector2 position)
    {
        foreach ((EntityRef entity, Transform2D transform) in frame.GetComponentIterator<Transform2D>())
        {
            if (entity == candidateEntity)
            {
                Vector3 unityPosition = transform.Position.ToUnityVector3();
                position = new Vector2(unityPosition.x, unityPosition.z);
                return true;
            }
        }

        position = Vector2.zero;
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
}
