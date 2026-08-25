using Quantum;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AnachronCommandIntentDebugView : QuantumMonoBehaviour
{
    private static readonly Color AcceptedCommandColor = new Color(0.25f, 0.55f, 1.0f, 1.0f);
    private static readonly Color RejectedCommandColor = new Color(1.0f, 0.2f, 0.15f, 1.0f);

    private Renderer _markerRenderer;

    private void LateUpdate()
    {
        QuantumRunner runner = QuantumRunner.Default;
        if (runner == null || runner.Game == null || runner.Game.Frames == null)
        {
            SetMarkerVisible(false);
            return;
        }

        Frame frame = runner.Game.Frames.Verified;
        if (frame == null || TryGetCommandTarget(frame, out Vector3 targetPosition, out bool isRejected) == false)
        {
            SetMarkerVisible(false);
            return;
        }

        Renderer marker = GetOrCreateMarker();
        marker.transform.position = targetPosition;
        marker.material.color = isRejected ? RejectedCommandColor : AcceptedCommandColor;
        SetMarkerVisible(true);
    }

    private static bool TryGetCommandTarget(Frame frame, out Vector3 targetPosition, out bool isRejected)
    {
        foreach ((EntityRef entity, CommandIntentDebug commandIntentDebug) in frame.GetComponentIterator<CommandIntentDebug>())
        {
            if (commandIntentDebug.HasMoveCommandIntent)
            {
                targetPosition = commandIntentDebug.MoveCommandTargetWorld.ToUnityVector3();
                isRejected = commandIntentDebug.WasMoveCommandRejected;
                return true;
            }
        }

        targetPosition = default;
        isRejected = false;
        return false;
    }

    private Renderer GetOrCreateMarker()
    {
        if (_markerRenderer != null)
        {
            return _markerRenderer;
        }

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "MoveCommandIntentMarker";
        marker.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);

        _markerRenderer = marker.GetComponent<Renderer>();
        _markerRenderer.material = new Material(Shader.Find("Standard"));
        _markerRenderer.material.color = AcceptedCommandColor;
        return _markerRenderer;
    }

    private void SetMarkerVisible(bool isVisible)
    {
        if (_markerRenderer != null)
        {
            _markerRenderer.gameObject.SetActive(isVisible);
        }
    }

    private void OnDestroy()
    {
        if (_markerRenderer == null)
        {
            return;
        }

        Destroy(_markerRenderer.material);
        Destroy(_markerRenderer.gameObject);
    }
}
