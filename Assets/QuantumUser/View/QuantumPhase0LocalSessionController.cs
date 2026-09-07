using System;
using Photon.Client;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class QuantumPhase0LocalSessionController : MonoBehaviour {
  private enum MatchStartMode {
    Local = 0,
    Online = 1
  }

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
  [SerializeField] private int activeFactionCount = 8;
  [SerializeField] private MatchStartMode startMode = MatchStartMode.Local;
  [SerializeField] private string playerName = "Player";
  [SerializeField] private string roomName = "anachron-prototype";
  [SerializeField] private string region = "";
  private const string StartPlayerSlotPreferenceKey = "Anachron.StartPlayerSlot";
  private const string ActiveFactionCountPreferenceKey = "Anachron.ActiveFactionCount";
  private const string MatchStartedPreferenceKey = "Anachron.MatchStarted";
  private const string MatchStartModePreferenceKey = "Anachron.MatchStartMode";
  private const string PlayerNamePreferenceKey = "Anachron.PlayerName";
  private const string RoomNamePreferenceKey = "Anachron.RoomName";
  private const string RegionPreferenceKey = "Anachron.Region";
  private const string RoomSeedPropertyKey = "anachron.seed";
  private const string RoomActiveFactionCountPropertyKey = "anachron.activeFactions";
  private const int MinActiveFactionCount = 2;
  private const int MaxStartPlayerSlot = 7;
  private const int MaxActiveFactionCount = MaxStartPlayerSlot + 1;

  private QuantumRunnerLocalDebug _localDebugRunner;
  private QuantumRunner _onlineRunner;
  private RealtimeClient _onlineClient;
  private IDisposable _pluginDisconnectSubscription;
  private string _setupStatus = "";
  private bool _isStartingOnline;
  private bool _isShuttingDownOnline;
  public static int ActivePlayerSlot { get; private set; }
  public static bool IsSetupOpen { get; private set; }
  public static bool IsOnlineSession { get; private set; }

  private void Awake() {
    activeFactionCount = Mathf.Clamp(PlayerPrefs.GetInt(ActiveFactionCountPreferenceKey, activeFactionCount), MinActiveFactionCount, MaxActiveFactionCount);
    startAs = (StartPlayerSlot)Mathf.Clamp(PlayerPrefs.GetInt(StartPlayerSlotPreferenceKey, (int)startAs), 0, MaxStartPlayerSlot);
    startMode = (MatchStartMode)Mathf.Clamp(PlayerPrefs.GetInt(MatchStartModePreferenceKey, (int)startMode), 0, 1);
    playerName = PlayerPrefs.GetString(PlayerNamePreferenceKey, string.IsNullOrWhiteSpace(playerName) ? BuildDefaultPlayerName() : playerName);
    roomName = PlayerPrefs.GetString(RoomNamePreferenceKey, roomName);
    region = PlayerPrefs.GetString(RegionPreferenceKey, region);
    ClampStartPlayerToActiveFactions();
    ActivePlayerSlot = (int)startAs;
    IsSetupOpen = PlayerPrefs.GetInt(MatchStartedPreferenceKey, 0) == 0;
    IsOnlineSession = IsSetupOpen == false && startMode == MatchStartMode.Online;

    _localDebugRunner = GetComponent<QuantumRunnerLocalDebug>();
    if (_localDebugRunner != null) {
      _localDebugRunner.enabled = IsSetupOpen == false && startMode == MatchStartMode.Local;
    }

    if (IsSetupOpen || startMode != MatchStartMode.Local) {
      return;
    }

    if (_localDebugRunner == null) {
      _localDebugRunner = gameObject.AddComponent<QuantumRunnerLocalDebug>();
    }

    ConfigureLocalRunner();
  }

  private RuntimeConfig BuildRuntimeConfig(RuntimeConfig existingConfig) {
    RuntimeConfig runtimeConfig = existingConfig ?? new RuntimeConfig();
    runtimeConfig.Seed = seed;
    runtimeConfig.Phase0Seed = seed;
    runtimeConfig.Phase0PlayerSlot = (int)startAs;
    runtimeConfig.Phase0ActiveFactionCount = activeFactionCount;
    return runtimeConfig;
  }

  private RuntimePlayer BuildRuntimePlayer() {
    return new RuntimePlayer {
      Phase0PlayerSlot = (int)startAs
    };
  }

  private void ConfigureLocalRunner() {
    _localDebugRunner.enabled = true;
    _localDebugRunner.UseRandomSeed = false;
    _localDebugRunner.RuntimeConfig = BuildRuntimeConfig(_localDebugRunner.RuntimeConfig);
    _localDebugRunner.LocalPlayers = new[] { BuildRuntimePlayer() };
  }

  private async void StartOnlineMatch() {
    if (_isStartingOnline || _onlineRunner != null) {
      return;
    }

    _isStartingOnline = true;
    _setupStatus = "Connecting to Photon...";
    Application.runInBackground = true;

    try {
      PhotonServerSettings serverSettings = null;
      PhotonServerSettings.TryGetGlobal(out serverSettings);
      if (serverSettings == null || string.IsNullOrEmpty(serverSettings.AppSettings.AppIdQuantum)) {
        throw new InvalidOperationException("Photon Quantum AppId is missing. Add it to Quantum Hub or PhotonServerSettings before testing online.");
      }

      RuntimeConfig runtimeConfig = BuildRuntimeConfig(null);
      QuantumMapData mapData = FindAnyObjectByType<QuantumMapData>();
      if (mapData != null) {
        runtimeConfig.Map = mapData.AssetRef;
      }

      if (runtimeConfig.SimulationConfig.Id.IsValid == false && QuantumDefaultConfigs.TryGetGlobal(out QuantumDefaultConfigs defaultConfigs)) {
        runtimeConfig.SimulationConfig = defaultConfigs.SimulationConfig;
      }

      string resolvedPlayerName = string.IsNullOrWhiteSpace(playerName) ? BuildDefaultPlayerName() : playerName.Trim();
      string resolvedClientId = BuildOnlineClientId(resolvedPlayerName);
      string resolvedRoomName = string.IsNullOrWhiteSpace(roomName) ? null : roomName.Trim();
      string resolvedRegion = string.IsNullOrWhiteSpace(region) ? null : region.Trim();

      MatchmakingArguments connectionArguments = new MatchmakingArguments {
        PhotonSettings = new AppSettings(serverSettings.AppSettings) {
          FixedRegion = resolvedRegion
        },
        EmptyRoomTtlInSeconds = serverSettings.EmptyRoomTtlInSeconds,
        EnableCrc = serverSettings.EnableCrc,
        PlayerTtlInSeconds = serverSettings.PlayerTtlInSeconds,
        MaxPlayers = Quantum.Input.MAX_COUNT,
        RoomName = resolvedRoomName,
        PluginName = "QuantumPlugin",
        AuthValues = new AuthenticationValues(resolvedClientId),
        CustomProperties = BuildRoomProperties(),
        CustomLobbyProperties = new[] { RoomSeedPropertyKey, RoomActiveFactionCountPropertyKey }
      };

      _onlineClient = await MatchmakingExtensions.ConnectToRoomAsync(connectionArguments);
      ApplyRoomPropertiesToRuntimeConfig(runtimeConfig);
      _setupStatus = "Starting Quantum session...";

      _pluginDisconnectSubscription = QuantumCallback.SubscribeManual<CallbackPluginDisconnect>(callback => {
        _setupStatus = $"Disconnected: {callback.Reason}";
        IsSetupOpen = true;
      });

      SessionRunner.Arguments sessionRunnerArguments = new SessionRunner.Arguments {
        RunnerFactory = QuantumRunnerUnityFactory.DefaultFactory,
        GameParameters = QuantumRunnerUnityFactory.CreateGameParameters,
        ClientId = resolvedClientId,
        RuntimeConfig = runtimeConfig,
        SessionConfig = QuantumDeterministicSessionConfigAsset.DefaultConfig,
        PlayerCount = Quantum.Input.MAX_COUNT,
        GameMode = DeterministicGameMode.Multiplayer,
        Communicator = new QuantumNetworkCommunicator(_onlineClient),
        DeltaTimeType = SimulationUpdateTime.EngineDeltaTime
      };

      using (new ConnectionServiceScope(_onlineClient)) {
        _onlineRunner = (QuantumRunner)await SessionRunner.StartAsync(sessionRunnerArguments);
      }
      RuntimePlayer runtimePlayer = BuildRuntimePlayer();
      runtimePlayer.PlayerNickname = resolvedPlayerName;
      _onlineRunner.Game.AddPlayer((int)startAs, runtimePlayer);

      ActivePlayerSlot = (int)startAs;
      IsOnlineSession = true;
      IsSetupOpen = false;
      _setupStatus = $"Online room: {_onlineClient.CurrentRoom?.Name}";
    } catch (Exception exception) {
      Debug.LogException(exception);
      _setupStatus = exception.Message;
      await ShutdownOnlineMatchAsync();
      IsSetupOpen = true;
      IsOnlineSession = false;
    } finally {
      _isStartingOnline = false;
    }
  }

  private async System.Threading.Tasks.Task ShutdownOnlineMatchAsync() {
    if (_isShuttingDownOnline) {
      return;
    }

    _isShuttingDownOnline = true;
    try {
      _pluginDisconnectSubscription?.Dispose();
      _pluginDisconnectSubscription = null;

      if (_onlineRunner != null) {
        await _onlineRunner.ShutdownAsync();
        _onlineRunner = null;
      }

      if (_onlineClient != null) {
        await _onlineClient.DisconnectAsync();
        _onlineClient = null;
      }
    } finally {
      _isShuttingDownOnline = false;
    }
  }

  private PhotonHashtable BuildRoomProperties() {
    return new PhotonHashtable {
      { RoomSeedPropertyKey, seed },
      { RoomActiveFactionCountPropertyKey, activeFactionCount }
    };
  }

  private static string BuildOnlineClientId(string displayName) {
    string namePrefix = string.IsNullOrWhiteSpace(displayName) ? "Player" : displayName.Trim();
    return $"{namePrefix}-{Guid.NewGuid():N}";
  }

  private void ApplyRoomPropertiesToRuntimeConfig(RuntimeConfig runtimeConfig) {
    PhotonHashtable roomProperties = _onlineClient?.CurrentRoom?.CustomProperties;
    if (roomProperties == null) {
      return;
    }

    if (roomProperties.TryGetValue(RoomSeedPropertyKey, out object roomSeed) && roomSeed is int resolvedSeed) {
      seed = resolvedSeed;
      runtimeConfig.Seed = resolvedSeed;
      runtimeConfig.Phase0Seed = resolvedSeed;
    }

    if (roomProperties.TryGetValue(RoomActiveFactionCountPropertyKey, out object roomActiveFactionCount) && roomActiveFactionCount is int resolvedActiveFactionCount) {
      activeFactionCount = Mathf.Clamp(resolvedActiveFactionCount, MinActiveFactionCount, MaxActiveFactionCount);
      runtimeConfig.Phase0ActiveFactionCount = activeFactionCount;
      ClampStartPlayerToActiveFactions();
      runtimeConfig.Phase0PlayerSlot = (int)startAs;
    }
  }

  private async void OnDestroy() {
    await ShutdownOnlineMatchAsync();
  }

  private void Update() {
    if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || IsQuitShortcutPressed()) {
      QuitClient();
    }
  }

  private void OnGUI() {
    GUI.depth = IsSetupOpen ? -200 : -90;

    if (IsSetupOpen) {
      DrawSetupScreen();
      return;
    }

    Rect panelRect = new Rect(Screen.width - 184, 12, 172, 72);
    DrawPanel(panelRect, new Color(0.02f, 0.025f, 0.03f, 0.88f));
    if (GUI.Button(new Rect(panelRect.x + 10, panelRect.y + 10, 152, 22), "Match Setup")) {
      IsOnlineSession = false;
      if (_onlineRunner != null || _onlineClient != null) {
        _setupStatus = "Disconnecting...";
        _ = ShutdownOnlineMatchAsync();
      }

      PlayerPrefs.SetInt(MatchStartedPreferenceKey, 0);
      PlayerPrefs.Save();
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    if (GUI.Button(new Rect(panelRect.x + 10, panelRect.y + 40, 152, 22), "Quit Client")) {
      QuitClient();
    }
  }

  private void DrawSetupScreen() {
    DrawPanel(new Rect(0, 0, Screen.width, Screen.height), new Color(0.01f, 0.012f, 0.016f, 0.94f));

    float width = Mathf.Min(520.0f, Screen.width - 48.0f);
    float height = startMode == MatchStartMode.Online ? 570.0f : 510.0f;
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
    GUI.Label(new Rect(panelRect.x + 28, panelRect.y + 62, width - 56, 42), "Set the match roster before deployment.", labelStyle);
    GUI.Label(new Rect(panelRect.x + 28, panelRect.y + 108, width - 56, 20), "Match Mode");
    float modeChoiceWidth = GetChoiceColumnWidth(width, 56.0f, 8.0f);
    DrawModeChoice(new Rect(panelRect.x + 28, panelRect.y + 136, modeChoiceWidth, 26), MatchStartMode.Local, "Local");
    DrawModeChoice(new Rect(panelRect.x + 28 + modeChoiceWidth + 8.0f, panelRect.y + 136, modeChoiceWidth, 26), MatchStartMode.Online, "Online");

    GUI.Label(new Rect(panelRect.x + 28, panelRect.y + 174, width - 56, 20), "Active Factions");

    float factionButtonGap = 6.0f;
    float factionButtonWidth = (width - 56.0f - factionButtonGap * 6.0f) / 7.0f;
    for (int count = MinActiveFactionCount; count <= MaxActiveFactionCount; count++) {
      Rect countRect = new Rect(
        panelRect.x + 28.0f + (count - MinActiveFactionCount) * (factionButtonWidth + factionButtonGap),
        panelRect.y + 202.0f,
        factionButtonWidth,
        24.0f);
      DrawFactionCountChoice(countRect, count);
    }

    GUI.Label(new Rect(panelRect.x + 28, panelRect.y + 236, width - 56, 20), "Start As");

    float columnGap = 16.0f;
    float choiceWidth = (width - 72.0f) * 0.5f;
    float leftX = panelRect.x + 28.0f;
    float rightX = leftX + choiceWidth + columnGap;

    DrawStartChoice(new Rect(leftX, panelRect.y + 264, choiceWidth, 24), StartPlayerSlot.P0ArdentConcord, "P0 Ardent Concord");
    DrawStartChoice(new Rect(rightX, panelRect.y + 264, choiceWidth, 24), StartPlayerSlot.P1Wrought, "P1 Wrought");
    DrawStartChoice(new Rect(leftX, panelRect.y + 294, choiceWidth, 24), StartPlayerSlot.P2Gharn, "P2 Gharn");
    DrawStartChoice(new Rect(rightX, panelRect.y + 294, choiceWidth, 24), StartPlayerSlot.P3Seethe, "P3 Seethe");
    DrawStartChoice(new Rect(leftX, panelRect.y + 324, choiceWidth, 24), StartPlayerSlot.P4Veirn, "P4 Veirn");
    DrawStartChoice(new Rect(rightX, panelRect.y + 324, choiceWidth, 24), StartPlayerSlot.P5Vaelun, "P5 Vaelun");
    DrawStartChoice(new Rect(leftX, panelRect.y + 354, choiceWidth, 24), StartPlayerSlot.P6Nimhara, "P6 Nimhara");
    DrawStartChoice(new Rect(rightX, panelRect.y + 354, choiceWidth, 24), StartPlayerSlot.P7Virii, "P7 Virii");

    float nextY = panelRect.y + 394.0f;
    if (startMode == MatchStartMode.Online) {
      GUI.Label(new Rect(panelRect.x + 28, nextY, 100, 20), "Player");
      playerName = GUI.TextField(new Rect(panelRect.x + 132, nextY, width - 160, 22), playerName);
      nextY += 30.0f;
      GUI.Label(new Rect(panelRect.x + 28, nextY, 100, 20), "Room");
      roomName = GUI.TextField(new Rect(panelRect.x + 132, nextY, width - 160, 22), roomName);
      nextY += 30.0f;
      GUI.Label(new Rect(panelRect.x + 28, nextY, 100, 20), "Region");
      region = GUI.TextField(new Rect(panelRect.x + 132, nextY, width - 160, 22), region);
      nextY += 34.0f;
    }

    GUI.Label(new Rect(panelRect.x + 28, nextY, width - 56, 42), $"Selected: {GetStartPlayerLabel(startAs)} in a {activeFactionCount}-faction {startMode.ToString().ToLowerInvariant()} match", labelStyle);
    nextY += 52.0f;

    bool wasEnabled = GUI.enabled;
    GUI.enabled = wasEnabled && _isStartingOnline == false && _isShuttingDownOnline == false;
    string startLabel = startMode == MatchStartMode.Online ? "Start Online Match" : "Start Local Match";
    if (GUI.Button(new Rect(panelRect.x + 28, nextY, width - 56, 34), startLabel)) {
      SaveSetupPreferences(startMode == MatchStartMode.Local);
      if (startMode == MatchStartMode.Local) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      } else {
        StartOnlineMatch();
      }
    }

    GUI.enabled = wasEnabled;
    nextY += 42.0f;

    if (string.IsNullOrWhiteSpace(_setupStatus) == false) {
      GUI.Label(new Rect(panelRect.x + 28, nextY, width - 56, 28), _setupStatus, labelStyle);
    }

    float bottomButtonWidth = (width - 64.0f) * 0.5f;
    if (GUI.Button(new Rect(panelRect.x + 28, panelRect.y + height - 40, bottomButtonWidth, 24), "Reset To Ardent Concord")) {
      SetStartPlayer(StartPlayerSlot.P0ArdentConcord);
    }

    if (GUI.Button(new Rect(panelRect.x + 36 + bottomButtonWidth, panelRect.y + height - 40, bottomButtonWidth, 24), "Quit Client")) {
      QuitClient();
    }
  }

  private static bool IsQuitShortcutPressed() {
    bool commandHeld = UnityEngine.Input.GetKey(KeyCode.LeftCommand) || UnityEngine.Input.GetKey(KeyCode.RightCommand);
    bool controlHeld = UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
    return UnityEngine.Input.GetKeyDown(KeyCode.Q) && (commandHeld || controlHeld);
  }

  private void QuitClient() {
    _ = ShutdownOnlineMatchAsync();
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }

  private static float GetChoiceColumnWidth(float totalWidth, float horizontalPadding, float gap) {
    return (totalWidth - horizontalPadding - gap) * 0.5f;
  }

  private void DrawModeChoice(Rect rect, MatchStartMode mode, string label) {
    string buttonLabel = startMode == mode ? $"{label}*" : label;
    if (GUI.Button(rect, buttonLabel) == false) {
      return;
    }

    startMode = mode;
    PlayerPrefs.SetInt(MatchStartModePreferenceKey, (int)startMode);
    PlayerPrefs.Save();
  }

  private void DrawFactionCountChoice(Rect rect, int count) {
    string buttonLabel = activeFactionCount == count ? $"{count}*" : count.ToString();
    if (GUI.Button(rect, buttonLabel) == false) {
      return;
    }

    activeFactionCount = Mathf.Clamp(count, MinActiveFactionCount, MaxActiveFactionCount);
    ClampStartPlayerToActiveFactions();
    ActivePlayerSlot = (int)startAs;
    PlayerPrefs.SetInt(ActiveFactionCountPreferenceKey, activeFactionCount);
    PlayerPrefs.SetInt(StartPlayerSlotPreferenceKey, (int)startAs);
    PlayerPrefs.Save();
  }

  private void DrawStartChoice(Rect rect, StartPlayerSlot slot, string label) {
    bool wasEnabled = GUI.enabled;
    GUI.enabled = wasEnabled && (int)slot < activeFactionCount;
    string buttonLabel = startAs == slot ? $"{label} - selected" : label;
    bool wasClicked = GUI.Button(rect, buttonLabel);
    GUI.enabled = wasEnabled;
    if (wasClicked == false) {
      return;
    }

    SetStartPlayer(slot);
  }

  private void SetStartPlayer(StartPlayerSlot slot) {
    startAs = slot;
    ClampStartPlayerToActiveFactions();
    ActivePlayerSlot = (int)startAs;
    PlayerPrefs.SetInt(StartPlayerSlotPreferenceKey, (int)startAs);
    PlayerPrefs.Save();
  }

  private void SaveSetupPreferences(bool matchStarted) {
    PlayerPrefs.SetInt(StartPlayerSlotPreferenceKey, (int)startAs);
    PlayerPrefs.SetInt(ActiveFactionCountPreferenceKey, activeFactionCount);
    PlayerPrefs.SetInt(MatchStartModePreferenceKey, (int)startMode);
    PlayerPrefs.SetString(PlayerNamePreferenceKey, string.IsNullOrWhiteSpace(playerName) ? BuildDefaultPlayerName() : playerName.Trim());
    PlayerPrefs.SetString(RoomNamePreferenceKey, roomName ?? "");
    PlayerPrefs.SetString(RegionPreferenceKey, region ?? "");
    PlayerPrefs.SetInt(MatchStartedPreferenceKey, matchStarted ? 1 : 0);
    PlayerPrefs.Save();
  }

  private void ClampStartPlayerToActiveFactions() {
    int maxAllowedSlot = Mathf.Clamp(activeFactionCount - 1, 0, MaxStartPlayerSlot);
    if ((int)startAs > maxAllowedSlot) {
      startAs = (StartPlayerSlot)maxAllowedSlot;
    }
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

  private static string BuildDefaultPlayerName() {
    return $"Player{UnityEngine.Random.Range(1000, 9999)}";
  }

  private static void DrawPanel(Rect rect, Color color) {
    Color previousColor = GUI.color;
    GUI.color = color;
    GUI.DrawTexture(rect, Texture2D.whiteTexture);
    GUI.color = previousColor;
  }
}
