using Quantum;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AnachronPlayerStateDebugOverlay : QuantumMonoBehaviour
{
    private static readonly Color PanelColor = new Color(0.0f, 0.0f, 0.0f, 0.72f);
    private static readonly Color TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

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

        int playerRows = 0;
        foreach ((EntityRef entity, PlayerEconomyState state) in frame.GetComponentIterator<PlayerEconomyState>())
        {
            playerRows++;
        }

        int baseRows = 0;
        foreach ((EntityRef entity, MainBuilding mainBuilding) in frame.GetComponentIterator<MainBuilding>())
        {
            baseRows++;
        }

        int totalRows = playerRows + baseRows + 1;
        Rect panelRect = new Rect(12, 58, 470, 28 + totalRows * 22);
        DrawPanel(panelRect);
        GUI.Label(new Rect(panelRect.x + 10, panelRect.y + 6, 440, 22), "Player Status", labelStyle);

        int row = 1;
        foreach ((EntityRef entity, PlayerEconomyState state) in frame.GetComponentIterator<PlayerEconomyState>())
        {
            string status = state.IsDefeated ? "Defeated" : "Active";
            string label = $"P{state.PlayerIndex} {GetFactionName(frame, state.PlayerIndex)} {status}: Salvage {state.Wood}  Plate {state.Iron}  Holding {state.FoodUsed}/{state.FoodCap}";
            GUI.Label(new Rect(panelRect.x + 10, panelRect.y + 8 + row * 22, 450, 22), label, labelStyle);
            row++;
        }

        foreach ((EntityRef entity, MainBuilding mainBuilding) in frame.GetComponentIterator<MainBuilding>())
        {
            string label = $"P{mainBuilding.OwnerPlayer} {GetMainBuildingDisplayName(frame, mainBuilding.OwnerPlayer)}: {mainBuilding.Health}/{mainBuilding.MaxHealth}";
            GUI.Label(new Rect(panelRect.x + 10, panelRect.y + 8 + row * 22, 450, 22), label, labelStyle);
            row++;
        }
    }

    private static void DrawPanel(Rect rect)
    {
        Color previousColor = GUI.color;
        GUI.color = PanelColor;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previousColor;
    }

    private static string GetFactionName(Frame frame, int playerIndex)
    {
        foreach ((EntityRef entity, PlayerFactionState factionState) in frame.GetComponentIterator<PlayerFactionState>())
        {
            if (factionState.PlayerIndex != playerIndex)
            {
                continue;
            }

            if (factionState.FactionId == FactionId.Fantasy)
            {
                return "Wrought";
            }

            if (factionState.FactionId == FactionId.Hybrid)
            {
                return "Gharn";
            }

            return "Ardent Concord";
        }

        return "Unknown";
    }

    private static string GetMainBuildingDisplayName(Frame frame, int playerIndex)
    {
        int factionId = GetFactionId(frame, playerIndex);
        if (factionId == FactionId.Fantasy)
        {
            return "Longhold Node";
        }

        if (factionId == FactionId.Hybrid)
        {
            return "Oathpyre";
        }

        return "Ledger House";
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

        return FactionId.Tech;
    }
}
