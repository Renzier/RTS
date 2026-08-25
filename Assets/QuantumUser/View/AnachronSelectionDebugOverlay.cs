using Quantum;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AnachronSelectionDebugOverlay : QuantumMonoBehaviour
{
    private static readonly Color MarkerFill = new Color(1.0f, 0.9f, 0.2f, 0.95f);
    private static readonly Color SelectedMarkerFill = new Color(0.1f, 1.0f, 0.45f, 0.95f);
    private static readonly Color MarkerRing = new Color(0.05f, 0.05f, 0.05f, 0.9f);
    private static readonly Color SelectedMarkerRing = new Color(0.05f, 0.65f, 0.2f, 0.95f);
    private static readonly Color CountPanelColor = new Color(0.02f, 0.025f, 0.03f, 0.82f);
    private static readonly Color CountTextColor = new Color(0.9f, 0.95f, 1.0f, 1.0f);

    private const float MarkerSize = 14.0f;

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

        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        int markerCount = 0;
        int selectedCount = 0;
        foreach ((EntityRef entity, SelectionCandidate candidate) in frame.GetComponentIterator<SelectionCandidate>())
        {
            markerCount++;
            bool isSelected = IsSelected(frame, entity);
            if (isSelected)
            {
                selectedCount++;
            }

            if (TryGetWorldPosition(frame, entity, out Vector3 worldPosition) == false)
            {
                continue;
            }

            Vector2 guiPosition = ScreenToGUI(camera.WorldToScreenPoint(worldPosition));
            DrawMarker(guiPosition, isSelected);
        }

        DrawRect(new Rect(12, 58, 236, 24), CountPanelColor);

        GUIStyle countStyle = new GUIStyle(GUI.skin.label);
        countStyle.normal.textColor = CountTextColor;
        GUI.Label(new Rect(20, 60, 220, 20), $"Selectables: {markerCount}  Selected: {selectedCount}", countStyle);
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

    private static bool TryGetWorldPosition(Frame frame, EntityRef candidateEntity, out Vector3 worldPosition)
    {
        foreach ((EntityRef entity, Transform2D transform) in frame.GetComponentIterator<Transform2D>())
        {
            if (entity == candidateEntity)
            {
                worldPosition = transform.Position.ToUnityVector3();
                return true;
            }
        }

        worldPosition = default;
        return false;
    }

    private static Vector2 ScreenToGUI(Vector2 screenPosition)
    {
        return new Vector2(screenPosition.x, Screen.height - screenPosition.y);
    }

    private static void DrawMarker(Vector2 center, bool isSelected)
    {
        float ringSize = MarkerSize + 6.0f;
        DrawRect(new Rect(center.x - ringSize * 0.5f, center.y - ringSize * 0.5f, ringSize, ringSize), isSelected ? SelectedMarkerRing : MarkerRing);
        DrawRect(new Rect(center.x - MarkerSize * 0.5f, center.y - MarkerSize * 0.5f, MarkerSize, MarkerSize), isSelected ? SelectedMarkerFill : MarkerFill);
    }

    private static void DrawRect(Rect rect, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previousColor;
    }
}
