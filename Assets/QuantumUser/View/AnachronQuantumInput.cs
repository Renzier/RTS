using Photon.Deterministic;
using Quantum;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AnachronQuantumInput : QuantumMonoBehaviour {
  private static readonly Color SelectionFill = new Color(0.1f, 0.7f, 1.0f, 0.18f);
  private static readonly Color SelectionBorder = new Color(0.1f, 0.85f, 1.0f, 0.85f);
  private static readonly Vector3 CameraOffset = new Vector3(0.0f, 10.0f, -12.0f);
  private static readonly Quaternion CameraRotation = Quaternion.Euler(55.0f, 0.0f, 0.0f);
  private const float CameraPanSpeed = 20.0f;
  private const float CameraZoomSpeed = 1.5f;
  private const float MinFieldOfView = 28.0f;
  private const float MaxFieldOfView = 78.0f;
  public static float LastUpgradePressedTime { get; private set; }
  public static float LastRebuildPressedTime { get; private set; }
  public static float LastTrainWorkerPressedTime { get; private set; }
  public static float LastBuildSupplyPressedTime { get; private set; }
  public static float LastDeconstructPressedTime { get; private set; }
  public static float LastDebugDamagePressedTime { get; private set; }
  public static Vector2 CurrentPointerWorld { get; private set; }
  public static bool BuildModeActive { get; private set; }

  private Vector2 _dragStartScreen;
  private Vector2 _pointerScreen;
  private Vector2 _dragStartWorld;
  private Vector2 _pointerWorld;
  private bool _selectHeld;
  private bool _commandHeld;
  private bool _additiveSelectHeld;
  private bool _upgradeQueued;
  private bool _rebuildHeroQueued;
  private bool _trainWorkerQueued;
  private bool _buildSupplyQueued;
  private bool _deconstructQueued;
  private bool _debugDamageQueued;
  private Vector3 _cameraFocus = Vector3.zero;
  private float _fieldOfView = 58.0f;

  private void OnEnable() {
    QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
  }

  private void OnDisable() {
    QuantumCallback.UnsubscribeListener(this);
  }

  private void Start() {
    _cameraFocus = GetStartingCameraFocus();
  }

  private void Update() {
    UpdateCameraControls();
    ConfigureRtsCamera();

    if (UnityEngine.Input.GetMouseButtonDown(0)) {
      _dragStartScreen = UnityEngine.Input.mousePosition;
      _dragStartWorld = ScreenToWorldGround(_dragStartScreen);
    }

    _pointerScreen = UnityEngine.Input.mousePosition;
    _pointerWorld = ScreenToWorldGround(_pointerScreen);
    CurrentPointerWorld = _pointerWorld;
    _selectHeld = UnityEngine.Input.GetMouseButton(0);
    _commandHeld = UnityEngine.Input.GetMouseButton(1);
    _additiveSelectHeld = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
    if (UnityEngine.Input.GetKeyDown(KeyCode.T)) {
      _upgradeQueued = true;
      LastUpgradePressedTime = Time.time;
    }

    if (UnityEngine.Input.GetKeyDown(KeyCode.R)) {
      _rebuildHeroQueued = true;
      LastRebuildPressedTime = Time.time;
    }

    if (UnityEngine.Input.GetKeyDown(KeyCode.B)) {
      if (HasSelectedOwnedWorker()) {
        BuildModeActive = true;
      } else if (HasSelectedOwnedMainBuilding()) {
        _trainWorkerQueued = true;
        LastTrainWorkerPressedTime = Time.time;
      }
    }

    if (UnityEngine.Input.GetKeyDown(KeyCode.C)) {
      if (BuildModeActive && HasSelectedOwnedWorker()) {
        _buildSupplyQueued = true;
        BuildModeActive = false;
        LastBuildSupplyPressedTime = Time.time;
      }
    }

    if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || HasSelectedOwnedWorker() == false) {
      BuildModeActive = false;
    }

    if (UnityEngine.Input.GetKeyDown(KeyCode.X)) {
      _deconstructQueued = true;
      LastDeconstructPressedTime = Time.time;
    }

    if (UnityEngine.Input.GetKeyDown(KeyCode.V)) {
      _debugDamageQueued = true;
      LastDebugDamagePressedTime = Time.time;
    }
  }

  private void PollInput(CallbackPollInput callback) {
    if (callback.IsInputSet) {
      return;
    }

    Quantum.Input input = new Quantum.Input {
      Select = _selectHeld,
      Command = _commandHeld,
      AdditiveSelect = _additiveSelectHeld,
      DragSelect = _selectHeld,
      PointerScreen = ToFPVector2(_pointerScreen),
      DragStartScreen = ToFPVector2(_selectHeld ? _dragStartScreen : _pointerScreen),
      DragEndScreen = ToFPVector2(_pointerScreen),
      PointerWorld = ToFPVector2(_pointerWorld),
      DragStartWorld = ToFPVector2(_selectHeld ? _dragStartWorld : _pointerWorld),
      DragEndWorld = ToFPVector2(_pointerWorld),
      CommandIntent = _commandHeld ? 1 : 0,
      UpgradeIntent = _upgradeQueued ? 1 : _rebuildHeroQueued ? 2 : _trainWorkerQueued ? 3 : _buildSupplyQueued ? 4 : _deconstructQueued ? 5 : _debugDamageQueued ? 6 : 0
    };

    callback.SetInput(input, DeterministicInputFlags.Repeatable);
    _upgradeQueued = false;
    _rebuildHeroQueued = false;
    _trainWorkerQueued = false;
    _buildSupplyQueued = false;
    _deconstructQueued = false;
    _debugDamageQueued = false;
  }

  private void OnGUI() {
    DrawInputStatus();

    if (_selectHeld == false) {
      return;
    }

    Rect selectionRect = GetScreenRect(_dragStartScreen, _pointerScreen);
    if (selectionRect.width < 4 || selectionRect.height < 4) {
      return;
    }

    DrawRect(selectionRect, SelectionFill);
    DrawRect(new Rect(selectionRect.xMin, selectionRect.yMin, selectionRect.width, 1), SelectionBorder);
    DrawRect(new Rect(selectionRect.xMin, selectionRect.yMax - 1, selectionRect.width, 1), SelectionBorder);
    DrawRect(new Rect(selectionRect.xMin, selectionRect.yMin, 1, selectionRect.height), SelectionBorder);
    DrawRect(new Rect(selectionRect.xMax - 1, selectionRect.yMin, 1, selectionRect.height), SelectionBorder);
  }

  private static FPVector2 ToFPVector2(Vector2 value) {
    return new FPVector2(
      FP.FromFloat_UNSAFE(value.x),
      FP.FromFloat_UNSAFE(value.y));
  }

  private static Rect GetScreenRect(Vector2 screenStart, Vector2 screenEnd) {
    Vector2 guiStart = ScreenToGUI(screenStart);
    Vector2 guiEnd = ScreenToGUI(screenEnd);

    float xMin = Mathf.Min(guiStart.x, guiEnd.x);
    float xMax = Mathf.Max(guiStart.x, guiEnd.x);
    float yMin = Mathf.Min(guiStart.y, guiEnd.y);
    float yMax = Mathf.Max(guiStart.y, guiEnd.y);

    return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
  }

  private static Vector2 ScreenToGUI(Vector2 screenPosition) {
    return new Vector2(screenPosition.x, Screen.height - screenPosition.y);
  }

  private static Vector2 ScreenToWorldGround(Vector2 screenPosition) {
    Camera camera = Camera.main;
    if (camera == null) {
      return Vector2.zero;
    }

    Ray ray = camera.ScreenPointToRay(screenPosition);
    UnityEngine.Plane groundPlane = new UnityEngine.Plane(Vector3.up, Vector3.zero);
    if (groundPlane.Raycast(ray, out float distance) == false) {
      return Vector2.zero;
    }

    Vector3 worldPosition = ray.GetPoint(distance);
    return new Vector2(worldPosition.x, worldPosition.z);
  }

  private static void DrawRect(Rect rect, Color color) {
    Color previousColor = GUI.color;
    GUI.color = color;
    GUI.DrawTexture(rect, Texture2D.whiteTexture);
    GUI.color = previousColor;
  }

  private void DrawInputStatus() {
    string mode = _commandHeld ? "Command" : _selectHeld ? "Select" : "Idle";
    if (_additiveSelectHeld) {
      mode += " + Add";
    }

    if (_trainWorkerQueued) {
      mode += " + Train";
    }

    if (_buildSupplyQueued) {
      mode += " + Build";
    }

    if (BuildModeActive) {
      mode += " + Build Mode";
    }

    if (_debugDamageQueued) {
      mode += " + Debug Damage";
    }

    GUI.Label(new Rect(12, 36, 240, 24), $"Input: {mode}");
  }

  private static bool HasSelectedOwnedWorker() {
    QuantumRunner runner = QuantumRunner.Default;
    if (runner == null || runner.Game == null || runner.Game.Frames == null) {
      return false;
    }

    Frame frame = runner.Game.Frames.Verified;
    if (frame == null) {
      return false;
    }

    foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>()) {
      if (unitIdentity.OwnerPlayer != QuantumPhase0LocalSessionController.ActivePlayerSlot || unitIdentity.UnitKind != UnitKind.Worker || IsDeadUnit(frame, entity)) {
        continue;
      }

      foreach ((EntityRef selectableEntity, Selectable selectable) in frame.GetComponentIterator<Selectable>()) {
        if (selectableEntity == entity && selectable.IsSelected) {
          return true;
        }
      }
    }

    return false;
  }

  private static bool HasSelectedOwnedMainBuilding() {
    QuantumRunner runner = QuantumRunner.Default;
    if (runner == null || runner.Game == null || runner.Game.Frames == null) {
      return false;
    }

    Frame frame = runner.Game.Frames.Verified;
    if (frame == null) {
      return false;
    }

    foreach ((EntityRef entity, MainBuilding mainBuilding) in frame.GetComponentIterator<MainBuilding>()) {
      if (mainBuilding.OwnerPlayer != QuantumPhase0LocalSessionController.ActivePlayerSlot || mainBuilding.Health <= 0) {
        continue;
      }

      foreach ((EntityRef selectableEntity, Selectable selectable) in frame.GetComponentIterator<Selectable>()) {
        if (selectableEntity == entity && selectable.IsSelected) {
          return true;
        }
      }
    }

    return false;
  }

  private static bool IsDeadUnit(Frame frame, EntityRef candidateEntity) {
    foreach ((EntityRef entity, UnitHealth unitHealth) in frame.GetComponentIterator<UnitHealth>()) {
      if (entity == candidateEntity) {
        return unitHealth.IsDead;
      }
    }

    return false;
  }

  private void UpdateCameraControls() {
    Vector3 pan = Vector3.zero;

    if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow)) {
      pan.z += 1.0f;
    }

    if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow)) {
      pan.z -= 1.0f;
    }

    if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow)) {
      pan.x += 1.0f;
    }

    if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow)) {
      pan.x -= 1.0f;
    }

    if (pan.sqrMagnitude > 1.0f) {
      pan.Normalize();
    }

    _cameraFocus += pan * (CameraPanSpeed * Time.deltaTime);
    _cameraFocus.x = Mathf.Clamp(_cameraFocus.x, -36.0f, 36.0f);
    _cameraFocus.z = Mathf.Clamp(_cameraFocus.z, -36.0f, 36.0f);

    float scroll = UnityEngine.Input.mouseScrollDelta.y;
    if (Mathf.Abs(scroll) > 0.001f) {
      _fieldOfView = Mathf.Clamp(_fieldOfView - scroll * CameraZoomSpeed * 4.0f, MinFieldOfView, MaxFieldOfView);
    }
  }

  private void ConfigureRtsCamera() {
    Camera camera = Camera.main;
    if (camera == null) {
      return;
    }

    camera.orthographic = false;
    camera.fieldOfView = _fieldOfView;
    camera.transform.position = _cameraFocus + CameraOffset;
    camera.transform.rotation = CameraRotation;
  }

  private static Vector3 GetStartingCameraFocus() {
    int playerSlot = QuantumPhase0LocalSessionController.ActivePlayerSlot;
    if (playerSlot == 1) {
      return new Vector3(-40.0f, 0.0f, -40.0f);
    }

    if (playerSlot == 2) {
      return new Vector3(40.0f, 0.0f, -40.0f);
    }

    if (playerSlot == 3) {
      return new Vector3(-58.0f, 0.0f, 0.0f);
    }

    if (playerSlot == 4) {
      return new Vector3(58.0f, 0.0f, 0.0f);
    }

    if (playerSlot == 5) {
      return new Vector3(-40.0f, 0.0f, 40.0f);
    }

    if (playerSlot == 6) {
      return new Vector3(40.0f, 0.0f, 40.0f);
    }

    if (playerSlot == 7) {
      return new Vector3(0.0f, 0.0f, 55.0f);
    }

    return new Vector3(0.0f, 0.0f, -55.0f);
  }
}
