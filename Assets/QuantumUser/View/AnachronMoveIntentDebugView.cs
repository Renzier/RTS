using System.Collections.Generic;
using Quantum;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AnachronMoveIntentDebugView : QuantumMonoBehaviour
{
    private static readonly Color TargetColor = new Color(0.15f, 0.45f, 1.0f, 1.0f);

    private readonly Dictionary<EntityRef, Renderer> _markers = new Dictionary<EntityRef, Renderer>();
    private readonly HashSet<EntityRef> _seenThisFrame = new HashSet<EntityRef>();

    private void LateUpdate()
    {
        QuantumRunner runner = QuantumRunner.Default;
        if (runner == null || runner.Game == null || runner.Game.Frames == null)
        {
            ClearMarkers();
            return;
        }

        Frame frame = runner.Game.Frames.Verified;
        if (frame == null)
        {
            ClearMarkers();
            return;
        }

        _seenThisFrame.Clear();

        foreach ((EntityRef entity, MoveIntent moveIntent) in frame.GetComponentIterator<MoveIntent>())
        {
            if (moveIntent.HasTarget == false)
            {
                continue;
            }

            _seenThisFrame.Add(entity);

            Renderer marker = GetOrCreateMarker(entity);
            marker.transform.position = moveIntent.TargetWorld.ToUnityVector3();
        }

        RemoveMissingMarkers();
    }

    private Renderer GetOrCreateMarker(EntityRef entity)
    {
        if (_markers.TryGetValue(entity, out Renderer renderer))
        {
            return renderer;
        }

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.name = $"MoveIntentTarget_{entity}";
        marker.transform.localScale = new Vector3(0.35f, 0.08f, 0.35f);

        renderer = marker.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = TargetColor;
        _markers.Add(entity, renderer);
        return renderer;
    }

    private void RemoveMissingMarkers()
    {
        List<EntityRef> staleEntities = null;
        foreach (EntityRef entity in _markers.Keys)
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
            Renderer renderer = _markers[entity];
            Destroy(renderer.material);
            Destroy(renderer.gameObject);
            _markers.Remove(entity);
        }
    }

    private void ClearMarkers()
    {
        foreach (Renderer renderer in _markers.Values)
        {
            if (renderer != null)
            {
                Destroy(renderer.material);
                Destroy(renderer.gameObject);
            }
        }

        _markers.Clear();
    }

    private void OnDestroy()
    {
        ClearMarkers();
    }
}
