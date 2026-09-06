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
    P4Veirn = 4,
    P5Vaelun = 5,
    P6Nimhara = 6,
    P7Virii = 7
  }

  [SerializeField] private int seed = 1;
  [SerializeField] private StartPlayerSlot startAs = StartPlayerSlot.P0ArdentConcord;
  private const string StartPlayerSlotPreferenceKey = "Anachron.StartPlayerSlot";
  private const string MatchStartedPreferenceKey = "Anachron.MatchStarted";
  private const int MaxStartPlayerSlot = 7;

  private QuantumRunnerLocalDebug _localDebugRunner;
  public static int ActivePlayerSlot { get; private set; }
  public static bool IsSetupOpen { get; private set; }

  private void Awake() {
    startAs = (StartPlayerSlot)Mathf.Clamp(PlayerPrefs.GetInt(StartPlayerSlotPreferenceKey, (int)startAs), 0, MaxStartPlayerSlot);
    ActivePlayerSlot = (int)startAs;
    IsSetupOpen = PlayerPrefs.GetInt(MatchStartedPreferenceKey, 0) == 0;

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
      Phase0PlayerSlot = (int)startAs
    };
  }

  private void OnGUI() {
    GUI.depth = IsSetupOpen ? -200 : -90;

    if (IsSetupOpen) {
      DrawSetupScreen();
      return;
    }

    Rect panelRect = new Rect(Screen.width - 184, 12, 172, 42);
    DrawPanel(panelRect, new Color(0.02f, 0.025f, 0.03f, 0.88f));
    if (GUI.Button(new Rect(panelRect.x + 10, panelRect.y + 10, 152, 22), "Match Setup")) {
      PlayerPrefs.SetInt(MatchStartedPreferenceKey, 0);
      PlayerPrefs.Save();
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }

  private void DrawSetupScreen() {
    DrawPanel(new Rect(0, 0, Screen.width, Screen.height), new Color(0.01f, 0.012f, 0.016f, 0.94f));

    float width = Mathf.Min(520.0f, Screen.width - 48.0f);
    float height = 418.0f;
    Rect panelRect = new Rect((Screen.width - width) * 0.5f, Mathf.Max(32.0f, (Screen.height - height) * 0.5f), width, height);
    DrawPanel(panelRect, new Color(0.05f, 0.058f, 0.07f, 0.96f));

    GUIStyle titleStyle = new GUIStyle(GUI.skin.label) {
      alignment = TextAnchor.MiddleCenter,
      fontSize = 24,
      fontStyle = FontStyle.Bold,
      normal = { textColor = new Color(0.72f, 0.94f, 1.0f, 1.0f) }
    };

    GUIStyle labelStyle = new GUIStyle(GUI.skin.label) {
      wordWrap = true,
      normal = { textColor = Color.white }
    };

    GUI.Label(new Rect(panelRect.x + 24, panelRect.y + 18, width - 48, 34), "Ashenspar Quill-Waist", titleStyle);
    GUI.Label(new Rect(panelRect.x + 28, panelRect.y + 62, width - 56, 42), "Choose your starting faction, then enter the live RTS prototype. Opponent count and map size arrive in the next setup sprints.", labelStyle);
    GUI.Label(new Rect(panelRect.x + 28, panelRect.y + 112, width - 56, 20), "Start As");

    float columnGap = 16.0f;
    float choiceWidth = (width - 72.0f) * 0.5f;
    float leftX = panelRect.x + 28.0f;
    float rightX = leftX + choiceWidth + columnGap;

    DrawStartChoice(new Rect(leftX, panelRect.y + 140, choiceWidth, 24), StartPlayerSlot.P0ArdentConcord, "P0 Ardent Concord");
    DrawStartChoice(new Rect(rightX, panelRect.y + 140, choiceWidth, 24), StartPlayerSlot.P1Wrought, "P1 Wrought");
    DrawStartChoice(new Rect(leftX, panelRect.y + 170, choiceWidth, 24), StartPlayerSlot.P2Gharn, "P2 Gharn");
    DrawStartChoice(new Rect(rightX, panelRect.y + 170, choiceWidth, 24), StartPlayerSlot.P3Seethe, "P3 Seethe");
    DrawStartChoice(new Rect(leftX, panelRect.y + 200, choiceWidth, 24), StartPlayerSlot.P4Veirn, "P4 Veirn");
    DrawStartChoice(new Rect(rightX, panelRect.y + 200, choiceWidth, 24), StartPlayerSlot.P5Vaelun, "P5 Vaelun");
    DrawStartChoice(new Rect(leftX, panelRect.y + 230, choiceWidth, 24), StartPlayerSlot.P6Nimhara, "P6 Nimhara");
    DrawStartChoice(new Rect(rightX, panelRect.y + 230, choiceWidth, 24), StartPlayerSlot.P7Virii, "P7 Virii");

    GUI.Label(new Rect(panelRect.x + 28, panelRect.y + 270, width - 56, 42), $"Selected: {GetStartPlayerLabel(startAs)}", labelStyle);

    if (GUI.Button(new Rect(panelRect.x + 28, panelRect.y + 324, width - 56, 34), "Start Match")) {
      PlayerPrefs.SetInt(StartPlayerSlotPreferenceKey, (int)startAs);
      PlayerPrefs.SetInt(MatchStartedPreferenceKey, 1);
      PlayerPrefs.Save();
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    if (GUI.Button(new Rect(panelRect.x + 28, panelRect.y + 366, width - 56, 24), "Reset To Ardent Concord")) {
      SetStartPlayer(StartPlayerSlot.P0ArdentConcord);
    }
  }

  private void DrawStartChoice(Rect rect, StartPlayerSlot slot, string label) {
    string buttonLabel = startAs == slot ? $"{label} - selected" : label;
    if (GUI.Button(rect, buttonLabel) == false) {
      return;
    }

    SetStartPlayer(slot);
  }

  private void SetStartPlayer(StartPlayerSlot slot) {
    startAs = slot;
    ActivePlayerSlot = (int)startAs;
    PlayerPrefs.SetInt(StartPlayerSlotPreferenceKey, (int)slot);
    PlayerPrefs.Save();
  }

  private static string GetStartPlayerLabel(StartPlayerSlot slot) {
    if (slot == StartPlayerSlot.P1Wrought) {
      return "P1 Wrought";
    }

    if (slot == StartPlayerSlot.P2Gharn) {
      return "P2 Gharn";
    }

    if (slot == StartPlayerSlot.P3Seethe) {
      return "P3 Seethe";
    }

    if (slot == StartPlayerSlot.P4Veirn) {
      return "P4 Veirn";
    }

    if (slot == StartPlayerSlot.P5Vaelun) {
      return "P5 Vaelun";
    }

    if (slot == StartPlayerSlot.P6Nimhara) {
      return "P6 Nimhara";
    }

    if (slot == StartPlayerSlot.P7Virii) {
      return "P7 Virii";
    }

    return "P0 Ardent Concord";
  }

  private static void DrawPanel(Rect rect, Color color) {
    Color previousColor = GUI.color;
    GUI.color = color;
    GUI.DrawTexture(rect, Texture2D.whiteTexture);
    GUI.color = previousColor;
  }
}
