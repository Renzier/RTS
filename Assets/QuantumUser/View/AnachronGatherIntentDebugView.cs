using System.Collections.Generic;
using Quantum;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AnachronGatherIntentDebugView : QuantumMonoBehaviour
{
    private static readonly Color WoodTargetColor = new Color(0.1f, 0.9f, 0.25f, 1.0f);
    private static readonly Color IronTargetColor = new Color(0.75f, 0.78f, 0.84f, 1.0f);
    private static readonly Color PanelColor = new Color(0.0f, 0.0f, 0.0f, 0.72f);
    private static readonly Color TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private const float GatherRange = 0.9f;
    private const float DepositRange = 1.25f;

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

        foreach ((EntityRef entity, GatherIntent gatherIntent) in frame.GetComponentIterator<GatherIntent>())
        {
            if (gatherIntent.HasTarget == false)
            {
                continue;
            }

            _seenThisFrame.Add(entity);

            Renderer marker = GetOrCreateMarker(entity);
            marker.transform.position = gatherIntent.TargetWorld.ToUnityVector3() + new Vector3(0.0f, 0.45f, 0.0f);
            marker.material.color = gatherIntent.ResourceKind == ResourceKind.Wood ? WoodTargetColor : IronTargetColor;
        }

        RemoveMissingMarkers();
    }

    private void OnGUI()
    {
        QuantumRunner runner = QuantumRunner.Default;
        if (runner == null || runner.Game == null || runner.Game.Frames == null)
        {
            return;
        }

        Frame frame = runner.Game.Frames.Verified;
        if (frame == null)
        {
            return;
        }

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.normal.textColor = TextColor;

        int workerRows = 0;
        foreach ((EntityRef entity, WorkerResourceCarry carry) in frame.GetComponentIterator<WorkerResourceCarry>())
        {
            workerRows++;
        }

        int resourceRows = 0;
        foreach ((EntityRef entity, ResourceNode node) in frame.GetComponentIterator<ResourceNode>())
        {
            resourceRows++;
        }

        Rect panelRect = new Rect(12, 230, 470, 28 + (workerRows + resourceRows + 1) * 20);
        DrawPanel(panelRect);
        GUI.Label(new Rect(panelRect.x + 10, panelRect.y + 6, 440, 20), "Worker Economy", labelStyle);

        int row = 0;
        foreach ((EntityRef entity, WorkerResourceCarry carry) in frame.GetComponentIterator<WorkerResourceCarry>())
        {
            string resourceName = carry.ResourceKind == ResourceKind.Wood ? "Wood" : carry.ResourceKind == ResourceKind.Iron ? "Iron" : "None";
            string state = GetWorkerState(frame, entity, carry);
            GUI.Label(new Rect(panelRect.x + 10, panelRect.y + 28 + row * 20, 450, 20), $"{entity}: {state}  carry {carry.Amount}/{carry.Capacity} {resourceName}", labelStyle);
            row++;
        }

        foreach ((EntityRef entity, ResourceNode node) in frame.GetComponentIterator<ResourceNode>())
        {
            string resourceName = node.ResourceKind == ResourceKind.Wood ? "Wood" : "Iron";
            GUI.Label(new Rect(panelRect.x + 10, panelRect.y + 28 + row * 20, 450, 20), $"{resourceName} node: {node.AmountRemaining}", labelStyle);
            row++;
        }
    }

    private static string GetWorkerState(Frame frame, EntityRef entity, WorkerResourceCarry carry)
    {
        if (TryGetGatherIntent(frame, entity, out GatherIntent gatherIntent) == false || gatherIntent.HasTarget == false)
        {
            return "Idle";
        }

        if (TryGetTransform(frame, entity, out Transform2D workerTransform) == false)
        {
            return "No transform";
        }

        if (carry.Amount >= carry.Capacity)
        {
            if (TryFindDropoffPosition(frame, entity, carry.ResourceKind, out Vector3 dropoffPosition))
            {
                float depositDistance = Vector3.Distance(workerTransform.Position.ToUnityVector3(), dropoffPosition);
                return depositDistance <= DepositRange ? "Depositing" : "Returning";
            }

            return "Full";
        }

        if (TryGetTransform(frame, gatherIntent.TargetNode, out Transform2D nodeTransform))
        {
            float gatherDistance = Vector3.Distance(workerTransform.Position.ToUnityVector3(), nodeTransform.Position.ToUnityVector3());
            return gatherDistance <= GatherRange ? "Gathering" : "Moving to resource";
        }

        return "Resource missing";
    }

    private static bool TryGetGatherIntent(Frame frame, EntityRef candidateEntity, out GatherIntent gatherIntent)
    {
        foreach ((EntityRef entity, GatherIntent candidateGatherIntent) in frame.GetComponentIterator<GatherIntent>())
        {
            if (entity == candidateEntity)
            {
                gatherIntent = candidateGatherIntent;
                return true;
            }
        }

        gatherIntent = default;
        return false;
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

    private static bool TryFindDropoffPosition(Frame frame, EntityRef workerEntity, int resourceKind, out Vector3 position)
    {
        position = Vector3.zero;
        if (TryGetOwner(frame, workerEntity, out int ownerPlayer) == false)
        {
            return false;
        }

        foreach ((EntityRef entity, ResourceDropoff dropoff) in frame.GetComponentIterator<ResourceDropoff>())
        {
            if (dropoff.OwnerPlayer != ownerPlayer || AcceptsResource(dropoff.AcceptedResourceMask, resourceKind) == false)
            {
                continue;
            }

            if (TryGetTransform(frame, entity, out Transform2D transform) == false)
            {
                continue;
            }

            position = transform.Position.ToUnityVector3();
            return true;
        }

        return false;
    }

    private static bool TryGetOwner(Frame frame, EntityRef workerEntity, out int ownerPlayer)
    {
        foreach ((EntityRef entity, UnitIdentity identity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (entity == workerEntity)
            {
                ownerPlayer = identity.OwnerPlayer;
                return true;
            }
        }

        ownerPlayer = -1;
        return false;
    }

    private static bool AcceptsResource(int acceptedResourceMask, int resourceKind)
    {
        if (resourceKind == ResourceKind.Wood)
        {
            return (acceptedResourceMask & ResourceMask.Wood) != 0;
        }

        if (resourceKind == ResourceKind.Iron)
        {
            return (acceptedResourceMask & ResourceMask.Iron) != 0;
        }

        return false;
    }

    private static void DrawPanel(Rect rect)
    {
        Color previousColor = GUI.color;
        GUI.color = PanelColor;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previousColor;
    }

    private Renderer GetOrCreateMarker(EntityRef entity)
    {
        if (_markers.TryGetValue(entity, out Renderer renderer))
        {
            return renderer;
        }

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = $"GatherIntentTarget_{entity}";
        marker.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        renderer = marker.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
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
