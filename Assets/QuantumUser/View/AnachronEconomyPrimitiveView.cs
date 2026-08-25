using System.Collections.Generic;
using Quantum;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AnachronEconomyPrimitiveView : QuantumMonoBehaviour
{
    private static readonly Color WoodColor = new Color(0.15f, 0.65f, 0.2f, 1.0f);
    private static readonly Color IronColor = new Color(0.55f, 0.58f, 0.62f, 1.0f);
    private static readonly Color MainBuildingColor = new Color(0.2f, 0.45f, 1.0f, 1.0f);

    private readonly Dictionary<EntityRef, Renderer> _views = new Dictionary<EntityRef, Renderer>();
    private readonly HashSet<EntityRef> _seenThisFrame = new HashSet<EntityRef>();

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

        foreach ((EntityRef entity, Transform2D transform) in frame.GetComponentIterator<Transform2D>())
        {
            if (TryGetEconomyColor(frame, entity, out Color color) == false)
            {
                continue;
            }

            _seenThisFrame.Add(entity);

            Renderer view = GetOrCreateView(entity);
            view.transform.position = transform.Position.ToUnityVector3() + new Vector3(0.0f, 0.35f, 0.0f);
            view.material.color = color;
        }

        RemoveMissingViews();
    }

    private Renderer GetOrCreateView(EntityRef entity)
    {
        if (_views.TryGetValue(entity, out Renderer renderer))
        {
            return renderer;
        }

        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        primitive.name = $"EconomyView_{entity}";
        primitive.transform.localScale = new Vector3(1.25f, 0.75f, 1.25f);

        renderer = primitive.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        _views.Add(entity, renderer);
        return renderer;
    }

    private static bool TryGetEconomyColor(Frame frame, EntityRef entity, out Color color)
    {
        foreach ((EntityRef nodeEntity, ResourceNode node) in frame.GetComponentIterator<ResourceNode>())
        {
            if (nodeEntity != entity)
            {
                continue;
            }

            color = node.ResourceKind == ResourceKind.Wood ? WoodColor : IronColor;
            return true;
        }

        foreach ((EntityRef buildingEntity, MainBuilding building) in frame.GetComponentIterator<MainBuilding>())
        {
            if (buildingEntity != entity)
            {
                continue;
            }

            color = MainBuildingColor;
            return true;
        }

        color = default;
        return false;
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
        }
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
    }

    private void OnDestroy()
    {
        ClearViews();
    }
}
