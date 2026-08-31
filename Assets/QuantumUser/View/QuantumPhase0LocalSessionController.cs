using Quantum;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class QuantumPhase0LocalSessionController : MonoBehaviour {
  private enum StartPlayerSlot {
    P0ArdentConcord = 0,
    P1Wrought = 1,
    P2Gharn = 2,
    P3Seethe = 3,
    P4Veirn = 4
  }

  [SerializeField] private int seed = 1;
  [SerializeField] private StartPlayerSlot startAs = StartPlayerSlot.P0ArdentConcord;
  private const string StartPlayerSlotPreferenceKey = "Anachron.StartPlayerSlot";
  private const int MaxStartPlayerSlot = 4;

  private QuantumRunnerLocalDebug _localDebugRunner;
  public static int ActivePlayerSlot { get; private set; }

  private void Awake() {
    startAs = (StartPlayerSlot)Mathf.Clamp(PlayerPrefs.GetInt(StartPlayerSlotPreferenceKey, (int)startAs), 0, MaxStartPlayerSlot);
    ActivePlayerSlot = (int)startAs;

    _localDebugRunner = GetComponent<QuantumRunnerLocalDebug>();
    if (_localDebugRunner == null) {
      _localDebugRunner = gameObject.AddComponent<QuantumRunnerLocalDebug>();
    }

    _localDebugRunner.UseRandomSeed = false;
    _localDebugRunner.RuntimeConfig = BuildRuntimeConfig(_localDebugRunner.RuntimeConfig);
    _localDebugRunner.LocalPlayers = new[] { BuildRuntimePlayer() };
  }

  private RuntimeConfig BuildRuntimeConfig(RuntimeConfig existingConfig) {
    RuntimeConfig runtimeConfig = existingConfig ?? new RuntimeConfig();
    runtimeConfig.Seed = seed;
    runtimeConfig.Phase0Seed = seed;
    runtimeConfig.Phase0PlayerSlot = (int)startAs;
    return runtimeConfig;
  }

  private RuntimePlayer BuildRuntimePlayer() {
    return new RuntimePlayer {
      Phase0PlayerSlot = 0
    };
  }

  private void OnGUI() {
    Rect panelRect = new Rect(Screen.width - 250, 12, 238, 170);
    DrawPanel(panelRect, new Color(0.02f, 0.025f, 0.03f, 0.88f));
    GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 8, 214, 20), "Start As");
    DrawStartButton(new Rect(panelRect.x + 12, panelRect.y + 34, 214, 22), StartPlayerSlot.P0ArdentConcord, "P0 Ardent Concord");
    DrawStartButton(new Rect(panelRect.x + 12, panelRect.y + 60, 214, 22), StartPlayerSlot.P1Wrought, "P1 Wrought");
    DrawStartButton(new Rect(panelRect.x + 12, panelRect.y + 86, 214, 22), StartPlayerSlot.P2Gharn, "P2 Gharn");
    DrawStartButton(new Rect(panelRect.x + 12, panelRect.y + 112, 214, 22), StartPlayerSlot.P3Seethe, "P3 Seethe");
    DrawStartButton(new Rect(panelRect.x + 12, panelRect.y + 138, 214, 22), StartPlayerSlot.P4Veirn, "P4 Veirn");
  }

  private void DrawStartButton(Rect rect, StartPlayerSlot slot, string label) {
    string buttonLabel = startAs == slot ? $"{label} (active)" : label;
    if (GUI.Button(rect, buttonLabel) == false || startAs == slot) {
      return;
    }

    PlayerPrefs.SetInt(StartPlayerSlotPreferenceKey, (int)slot);
    PlayerPrefs.Save();
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  private static void DrawPanel(Rect rect, Color color) {
    Color previousColor = GUI.color;
    GUI.color = color;
    GUI.DrawTexture(rect, Texture2D.whiteTexture);
    GUI.color = previousColor;
  }
}
