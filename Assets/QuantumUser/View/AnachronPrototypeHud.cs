using Photon.Deterministic;
using Quantum;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AnachronPrototypeHud : QuantumMonoBehaviour
{
    private static readonly Color PanelColor = new Color(0.02f, 0.025f, 0.03f, 0.88f);
    private static readonly Color VictoryPanelColor = new Color(0.02f, 0.18f, 0.12f, 0.9f);
    private static readonly Color DefeatPanelColor = new Color(0.22f, 0.04f, 0.04f, 0.9f);
    private static readonly Color ProgressTrackColor = new Color(0.08f, 0.09f, 0.1f, 0.95f);
    private static readonly Color ProgressFillColor = new Color(0.2f, 0.9f, 0.75f, 0.95f);
    private static readonly Color HealthFillColor = new Color(0.28f, 0.9f, 0.45f, 0.95f);
    private static readonly Color DamagedHealthFillColor = new Color(0.95f, 0.74f, 0.22f, 0.95f);
    private static readonly Color CriticalHealthFillColor = new Color(0.95f, 0.22f, 0.18f, 0.95f);
    private static readonly Color TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private static readonly Color HeaderTextColor = new Color(0.65f, 0.95f, 1.0f, 1.0f);
    private const int RowHeight = 18;
    private const int RightPanelWidth = 370;
    private const int RightPanelContentWidth = 346;
    private const int MaxTechTier = 3;
    private const int BaseWoodUpgradeCost = 200;
    private const int BaseIronUpgradeCost = 150;
    private const float ActionNotificationSeconds = 2.25f;
    private int _lastUpgradeSignal = int.MinValue;
    private int _lastHeroSignal = int.MinValue;
    private float _lastSeenUpgradePressTime;
    private float _lastSeenRebuildPressTime;
    private float _lastSeenTrainWorkerPressTime;
    private float _lastSeenBuildSupplyPressTime;
    private float _lastSeenDeconstructPressTime;
    private float _lastSeenDebugDamagePressTime;
    private string _actionNotificationText = string.Empty;
    private float _actionNotificationUntil;

    private void OnGUI()
    {
        GUI.depth = -100;

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

        DrawMatchBanner(frame);

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.normal.textColor = TextColor;

        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.normal.textColor = HeaderTextColor;
        headerStyle.fontStyle = FontStyle.Bold;

        UpdateActionNotification(frame);
        DrawActionNotification(headerStyle);

        int rowCount = CountHudRows(frame);
        DrawPanel(new Rect(12, 58, 430, 22 + rowCount * RowHeight));
        GUI.Label(new Rect(24, 64, 390, RowHeight), "Prototype Status", headerStyle);

        int row = 1;
        foreach ((EntityRef entity, PlayerEconomyState state) in frame.GetComponentIterator<PlayerEconomyState>())
        {
            string status = state.IsDefeated ? "DEFEATED" : "Active";
            GUI.Label(new Rect(24, 64 + row * RowHeight, 390, RowHeight), $"P{state.PlayerIndex} {GetFactionName(frame, state.PlayerIndex)} {status}: S {state.Wood}  P {state.Iron}  H {state.FoodUsed}/{state.FoodCap}  T{GetTechTier(frame, state.PlayerIndex)}", labelStyle);
            row++;
        }

        foreach ((EntityRef entity, PlayerTechState techState) in frame.GetComponentIterator<PlayerTechState>())
        {
            GUI.Label(new Rect(24, 64 + row * RowHeight, 390, RowHeight), $"P{techState.PlayerIndex} Tech: T{techState.TechTier}  {GetUpgradeProgressLabel(techState)}", labelStyle);
            row++;
        }

        foreach ((EntityRef entity, PlayerHeroState heroState) in frame.GetComponentIterator<PlayerHeroState>())
        {
            GUI.Label(new Rect(24, 64 + row * RowHeight, 390, RowHeight), $"P{heroState.PlayerIndex} Hero: {GetHeroStatusLabel(heroState)} L{heroState.HeroLevel} HP {heroState.HeroHealth}/{heroState.HeroMaxHealth} {GetHeroResultName(heroState.LastHeroResult)}", labelStyle);
            row++;
        }

        GUI.Label(new Rect(24, 64 + row * RowHeight, 390, RowHeight), $"Main Buildings: {GetBaseSummary(frame)}", labelStyle);
        row++;

        GUI.Label(new Rect(24, 64 + row * RowHeight, 390, RowHeight), $"{AnachronPrototypeScenario.ScenarioName}: {CountResources(frame)} resources  {CountBases(frame)} bases  {CountSupplyBuildings(frame)} support", labelStyle);
        row += 2;
        GUI.Label(new Rect(24, 64 + row * RowHeight, 390, RowHeight), "Player Units", headerStyle);
        row++;

        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (IsOwnedByLocalPlayer(frame, entity) == false)
            {
                continue;
            }

            GUI.Label(new Rect(24, 64 + row * RowHeight, 390, RowHeight), $"{GetShortEntityName(entity)} {GetUnitDisplayName(frame, unitIdentity)}: {GetUnitState(frame, entity)} HP {GetUnitHealth(frame, entity)} {GetAttackLabel(frame, entity)} {GetCarryLabel(frame, entity)} {GetTellLabel(frame, entity, unitIdentity.OwnerPlayer)}", labelStyle);
            row++;
        }

        DrawSelectedBuildingPanel(frame, headerStyle, labelStyle);
        DrawSelectedSupplyPanel(frame, headerStyle, labelStyle);
        DrawSelectedQuillObjectivePanel(frame, headerStyle, labelStyle);
        DrawSelectedWorkerBuildPanel(frame, headerStyle, labelStyle);
        DrawSupplyWorldTimers(frame, labelStyle);
        DrawWorldHealthLabels(frame, labelStyle);
    }

    private void UpdateActionNotification(Frame frame)
    {
        if (AnachronQuantumInput.LastUpgradePressedTime > _lastSeenUpgradePressTime)
        {
            _lastSeenUpgradePressTime = AnachronQuantumInput.LastUpgradePressedTime;
            _actionNotificationText = "Upgrade command sent";
            _actionNotificationUntil = Time.time + ActionNotificationSeconds;
        }

        if (AnachronQuantumInput.LastRebuildPressedTime > _lastSeenRebuildPressTime)
        {
            _lastSeenRebuildPressTime = AnachronQuantumInput.LastRebuildPressedTime;
            _actionNotificationText = "Hero rebuild command sent";
            _actionNotificationUntil = Time.time + ActionNotificationSeconds;
        }

        if (AnachronQuantumInput.LastTrainWorkerPressedTime > _lastSeenTrainWorkerPressTime)
        {
            _lastSeenTrainWorkerPressTime = AnachronQuantumInput.LastTrainWorkerPressedTime;
            _actionNotificationText = "Train worker command sent";
            _actionNotificationUntil = Time.time + ActionNotificationSeconds;
        }

        if (AnachronQuantumInput.LastBuildSupplyPressedTime > _lastSeenBuildSupplyPressTime)
        {
            _lastSeenBuildSupplyPressTime = AnachronQuantumInput.LastBuildSupplyPressedTime;
            _actionNotificationText = $"Build {GetSupplyBuildingDisplayName(frame, GetLocalPlayerIndex())} command sent";
            _actionNotificationUntil = Time.time + ActionNotificationSeconds;
        }

        if (AnachronQuantumInput.LastDeconstructPressedTime > _lastSeenDeconstructPressTime)
        {
            _lastSeenDeconstructPressTime = AnachronQuantumInput.LastDeconstructPressedTime;
            _actionNotificationText = "Deconstruct command sent";
            _actionNotificationUntil = Time.time + ActionNotificationSeconds;
        }

        if (AnachronQuantumInput.LastDebugDamagePressedTime > _lastSeenDebugDamagePressTime)
        {
            _lastSeenDebugDamagePressTime = AnachronQuantumInput.LastDebugDamagePressedTime;
            _actionNotificationText = "Debug damage command sent";
            _actionNotificationUntil = Time.time + ActionNotificationSeconds;
        }

        if (TryGetTechState(frame, 0, out PlayerTechState techState))
        {
            int signal = techState.TechTier * 100 + techState.LastUpgradeResult;
            if (_lastUpgradeSignal == int.MinValue)
            {
                _lastUpgradeSignal = signal;
            }
            else if (_lastUpgradeSignal != signal)
            {
                _lastUpgradeSignal = signal;
                _actionNotificationText = GetUpgradeNotification(techState);
                _actionNotificationUntil = Time.time + ActionNotificationSeconds;
            }
        }

        if (TryGetHeroState(frame, 0, out PlayerHeroState heroState))
        {
            int signal = heroState.LastHeroResult * 100000 + heroState.HeroHealth;
            if (_lastHeroSignal == int.MinValue)
            {
                _lastHeroSignal = signal;
            }
            else if (_lastHeroSignal != signal && IsHeroNotificationResult(heroState.LastHeroResult))
            {
                _lastHeroSignal = signal;
                _actionNotificationText = GetHeroNotification(heroState);
                _actionNotificationUntil = Time.time + ActionNotificationSeconds;
            }
            else
            {
                _lastHeroSignal = signal;
            }
        }
    }

    private void DrawActionNotification(GUIStyle headerStyle)
    {
        if (Time.time > _actionNotificationUntil || string.IsNullOrEmpty(_actionNotificationText))
        {
            return;
        }

        GUIStyle notificationStyle = new GUIStyle(headerStyle);
        notificationStyle.alignment = TextAnchor.MiddleCenter;
        notificationStyle.fontSize = 20;

        Rect rect = new Rect((Screen.width - 520) * 0.5f, 74, 520, 38);
        DrawPanel(rect, PanelColor);
        GUI.Label(rect, _actionNotificationText, notificationStyle);
    }

    private static int CountHudRows(Frame frame)
    {
        int rows = 1;
        rows += CountEconomyStates(frame);
        rows += CountTechStates(frame);
        rows += CountHeroStates(frame);
        rows += 4;
        rows += CountPlayerUnits(frame);
        return rows;
    }

    private static int CountEconomyStates(Frame frame)
    {
        int count = 0;
        foreach ((EntityRef entity, PlayerEconomyState state) in frame.GetComponentIterator<PlayerEconomyState>())
        {
            count++;
        }

        return count;
    }

    private static int CountTechStates(Frame frame)
    {
        int count = 0;
        foreach ((EntityRef entity, PlayerTechState state) in frame.GetComponentIterator<PlayerTechState>())
        {
            count++;
        }

        return count;
    }

    private static int CountHeroStates(Frame frame)
    {
        int count = 0;
        foreach ((EntityRef entity, PlayerHeroState state) in frame.GetComponentIterator<PlayerHeroState>())
        {
            count++;
        }

        return count;
    }

    private static int CountPlayerUnits(Frame frame)
    {
        int count = 0;
        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (IsOwnedByLocalPlayer(frame, entity))
            {
                count++;
            }
        }

        return count;
    }

    private static int CountResources(Frame frame)
    {
        int count = 0;
        foreach ((EntityRef entity, ResourceNode node) in frame.GetComponentIterator<ResourceNode>())
        {
            count++;
        }

        return count;
    }

    private static string GetBaseSummary(Frame frame)
    {
        string summary = string.Empty;
        foreach ((EntityRef entity, MainBuilding mainBuilding) in frame.GetComponentIterator<MainBuilding>())
        {
            if (summary.Length > 0)
            {
                summary += "  ";
            }

            summary += $"P{mainBuilding.OwnerPlayer} {GetMainBuildingDisplayName(frame, mainBuilding.OwnerPlayer)} T{GetBuildingTier(frame, entity)} {mainBuilding.Health}/{mainBuilding.MaxHealth}";
        }

        return summary;
    }

    private static int GetBuildingTier(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, BuildingTier buildingTier) in frame.GetComponentIterator<BuildingTier>())
        {
            if (entity == candidateEntity)
            {
                return buildingTier.Tier;
            }
        }

        return 1;
    }

    private static void DrawSelectedBuildingPanel(Frame frame, GUIStyle headerStyle, GUIStyle labelStyle)
    {
        if (TryGetSelectedMainBuilding(frame, out EntityRef mainEntity, out MainBuilding mainBuilding, out int buildingTier) == false)
        {
            return;
        }

        Rect panelRect = new Rect(Screen.width - RightPanelWidth - 20, 58, RightPanelWidth, 158);
        DrawPanel(panelRect);

        int nextTier = buildingTier + 1;
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 8, RightPanelContentWidth, RowHeight), $"{GetMainBuildingDisplayName(frame, mainBuilding.OwnerPlayer)} - T{buildingTier}", headerStyle);
        DrawHealthBar(new Rect(panelRect.x + 12, panelRect.y + 32, RightPanelContentWidth, 18), mainBuilding.Health, mainBuilding.MaxHealth, labelStyle);
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 56, RightPanelContentWidth, RowHeight), GetWorkerProductionLabel(frame, mainBuilding.OwnerPlayer), labelStyle);

        if (TryGetTechState(frame, mainBuilding.OwnerPlayer, out PlayerTechState techState) && techState.UpgradeInProgress)
        {
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 80, RightPanelContentWidth, RowHeight), GetTellLabel(frame, mainEntity, mainBuilding.OwnerPlayer), labelStyle);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 102, RightPanelContentWidth, RowHeight), $"Upgrading to T{techState.UpgradeTargetTier}", labelStyle);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 124, RightPanelContentWidth, RowHeight), $"{FormatTicksAsSeconds(techState.UpgradeTicksRemaining)} remaining  Debug damage: V", labelStyle);
            DrawProgressBar(new Rect(panelRect.x + 12, panelRect.y + 146, RightPanelContentWidth, 10), techState.UpgradeTicksTotal, techState.UpgradeTicksRemaining);
            return;
        }

        if (buildingTier >= MaxTechTier)
        {
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 80, RightPanelContentWidth, RowHeight), GetTellLabel(frame, mainEntity, mainBuilding.OwnerPlayer), labelStyle);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 102, RightPanelContentWidth, RowHeight), "Upgrade: Max Tier", labelStyle);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 124, RightPanelContentWidth, RowHeight), "Debug damage: press V", labelStyle);
            return;
        }

        int woodCost = BaseWoodUpgradeCost * nextTier;
        int ironCost = BaseIronUpgradeCost * nextTier;
        bool canAfford = TryGetEconomyState(frame, mainBuilding.OwnerPlayer, out PlayerEconomyState economyState) &&
                         economyState.Wood >= woodCost &&
                         economyState.Iron >= ironCost;
        string status = canAfford ? "Ready - press T" : $"Need {GetResourceShortfall(economyState, woodCost, ironCost)}";

        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 80, RightPanelContentWidth, RowHeight), GetTellLabel(frame, mainEntity, mainBuilding.OwnerPlayer), labelStyle);
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 102, RightPanelContentWidth, RowHeight), $"Upgrade to T{nextTier}: {FormatResourcePair(woodCost, ironCost)}", labelStyle);
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 124, RightPanelContentWidth, RowHeight), $"{status}  Debug damage: V", labelStyle);
    }

    private static void DrawSelectedSupplyPanel(Frame frame, GUIStyle headerStyle, GUIStyle labelStyle)
    {
        if (TryGetSelectedSupplyBuilding(frame, out EntityRef supplyEntity, out SupplyBuilding supplyBuilding) == false)
        {
            return;
        }

        bool isTimedAction = supplyBuilding.IsConstructing || supplyBuilding.IsDeconstructing;
        Rect panelRect = new Rect(Screen.width - RightPanelWidth - 20, 314, RightPanelWidth, isTimedAction ? 162 : 154);
        DrawPanel(panelRect);

        string supplyName = GetSupplyBuildingDisplayName(frame, supplyBuilding.OwnerPlayer);
        string title = supplyBuilding.IsConstructing ? $"{supplyName} Foundation" : supplyBuilding.IsDeconstructing ? $"Deconstructing {supplyName}" : supplyName;
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 8, RightPanelContentWidth, RowHeight), title, headerStyle);
        DrawHealthBar(new Rect(panelRect.x + 12, panelRect.y + 32, RightPanelContentWidth, 18), supplyBuilding.Health, supplyBuilding.MaxHealth, labelStyle);
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 56, RightPanelContentWidth, RowHeight), $"+{supplyBuilding.FoodProvided} Holding", labelStyle);

        if (supplyBuilding.IsConstructing)
        {
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 80, RightPanelContentWidth, RowHeight), GetTellLabel(frame, supplyEntity, supplyBuilding.OwnerPlayer), labelStyle);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 102, RightPanelContentWidth, RowHeight), $"Constructing: {FormatTicksAsSeconds(supplyBuilding.BuildTicksRemaining)} remaining  Builders: {CountActiveBuilders(frame, supplyEntity)}", labelStyle);
            DrawProgressBar(new Rect(panelRect.x + 12, panelRect.y + 122, RightPanelContentWidth, 8), supplyBuilding.BuildTicksTotal, supplyBuilding.BuildTicksRemaining);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 134, RightPanelContentWidth, RowHeight), $"Cancel: full refund - press X  Debug damage: V", labelStyle);
            return;
        }

        if (supplyBuilding.IsDeconstructing)
        {
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 80, RightPanelContentWidth, RowHeight), GetTellLabel(frame, supplyEntity, supplyBuilding.OwnerPlayer), labelStyle);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 102, RightPanelContentWidth, RowHeight), $"Deconstructing: {FormatTicksAsSeconds(supplyBuilding.DeconstructTicksRemaining)} remaining", labelStyle);
            DrawProgressBar(new Rect(panelRect.x + 12, panelRect.y + 122, RightPanelContentWidth, 8), supplyBuilding.DeconstructTicksTotal, supplyBuilding.DeconstructTicksRemaining);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 134, RightPanelContentWidth, RowHeight), $"Refund: {FormatResourcePair(supplyBuilding.WoodCost * 80 / 100, supplyBuilding.IronCost * 80 / 100)}", labelStyle);
            return;
        }

        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 80, RightPanelContentWidth, RowHeight), GetTellLabel(frame, supplyEntity, supplyBuilding.OwnerPlayer), labelStyle);
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 102, RightPanelContentWidth, RowHeight), $"Deconstruct: {FormatResourcePair(supplyBuilding.WoodCost * 80 / 100, supplyBuilding.IronCost * 80 / 100)}", labelStyle);
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 124, RightPanelContentWidth, RowHeight), "Press X  Debug damage: V", labelStyle);
    }

    private static void DrawSelectedQuillObjectivePanel(Frame frame, GUIStyle headerStyle, GUIStyle labelStyle)
    {
        if (TryGetSelectedQuillObjective(frame, out Targetable quillTargetable) == false)
        {
            return;
        }

        Rect panelRect = new Rect(Screen.width - RightPanelWidth - 20, 456, RightPanelWidth, 112);
        DrawPanel(panelRect);

        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 8, RightPanelContentWidth, RowHeight), "Quill-Waist Landmark", headerStyle);
        DrawHealthBar(new Rect(panelRect.x + 12, panelRect.y + 32, RightPanelContentWidth, 18), quillTargetable.Health, quillTargetable.MaxHealth, labelStyle);
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 56, RightPanelContentWidth, RowHeight), GetQuillObjectiveStatus(frame, quillTargetable), labelStyle);
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 78, RightPanelContentWidth, RowHeight), GetQuillObjectiveBonusLabel(quillTargetable), labelStyle);
    }

    private static void DrawSupplyWorldTimers(Frame frame, GUIStyle labelStyle)
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            if ((supplyBuilding.IsConstructing == false && supplyBuilding.IsDeconstructing == false) ||
                TryGetTransform(frame, entity, out Transform2D transform) == false)
            {
                continue;
            }

            Vector3 worldPosition = transform.Position.ToUnityVector3() + new Vector3(0.0f, 1.1f, 0.0f);
            Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);
            if (screenPosition.z <= 0.0f)
            {
                continue;
            }

            float x = screenPosition.x - 54.0f;
            float y = Screen.height - screenPosition.y - 28.0f;
            Rect labelRect = new Rect(x, y, 108.0f, 18.0f);
            Rect barRect = new Rect(x, y + 18.0f, 108.0f, 7.0f);

            DrawPanel(new Rect(x - 4.0f, y - 2.0f, 116.0f, 31.0f), PanelColor);
            int totalTicks = supplyBuilding.IsDeconstructing ? supplyBuilding.DeconstructTicksTotal : supplyBuilding.BuildTicksTotal;
            int ticksRemaining = supplyBuilding.IsDeconstructing ? supplyBuilding.DeconstructTicksRemaining : supplyBuilding.BuildTicksRemaining;
            GUI.Label(labelRect, FormatTicksAsSeconds(ticksRemaining), labelStyle);
            DrawProgressBar(barRect, totalTicks, ticksRemaining);
        }
    }

    private static void DrawWorldHealthLabels(Frame frame, GUIStyle labelStyle)
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        GUIStyle healthLabelStyle = new GUIStyle(labelStyle);
        healthLabelStyle.alignment = TextAnchor.MiddleCenter;
        healthLabelStyle.fontStyle = FontStyle.Bold;
        healthLabelStyle.fontSize = 11;

        foreach ((EntityRef entity, Transform2D transform) in frame.GetComponentIterator<Transform2D>())
        {
            if (TryGetWorldHealth(frame, entity, out int health, out int maxHealth, out bool isBuilding, out bool isHero) == false ||
                maxHealth <= 0 ||
                health <= 0)
            {
                continue;
            }

            float yOffset = isBuilding ? 1.65f : isHero ? 1.05f : 0.72f;
            Vector3 worldPosition = transform.Position.ToUnityVector3() + new Vector3(0.0f, yOffset, 0.0f);
            Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);
            if (screenPosition.z <= 0.0f)
            {
                continue;
            }

            float labelWidth = isBuilding ? 86.0f : 64.0f;
            Rect labelRect = new Rect(screenPosition.x - labelWidth * 0.5f, Screen.height - screenPosition.y - 9.0f, labelWidth, 18.0f);
            DrawPanel(labelRect, new Color(0.0f, 0.0f, 0.0f, 0.58f));
            GUI.Label(labelRect, $"{health}/{maxHealth}", healthLabelStyle);
        }
    }

    private static string GetWorkerProductionLabel(Frame frame, int playerIndex)
    {
        string workerName = GetWorkerDisplayName(frame, playerIndex);
        FactionStats stats = FactionStats.ForPlayer(frame, playerIndex);
        if (TryGetEconomyState(frame, playerIndex, out PlayerEconomyState economyState) == false)
        {
            return $"Train {workerName}: economy missing";
        }

        if (economyState.FoodUsed + stats.WorkerFoodCost > economyState.FoodCap)
        {
            return $"Train {workerName}: need Holding";
        }

        if (economyState.Wood < stats.WorkerWoodCost || economyState.Iron < stats.WorkerIronCost)
        {
            return $"Train {workerName}: need {GetResourceShortfall(economyState, stats.WorkerWoodCost, stats.WorkerIronCost)}";
        }

        return $"Train {workerName}: {FormatResourcePair(stats.WorkerWoodCost, stats.WorkerIronCost)} - press B";
    }

    private static string GetWorkerDisplayName(Frame frame, int playerIndex)
    {
        int factionId = GetFactionId(frame, playerIndex);
        if (factionId == FactionId.Wrought)
        {
            return "Wright";
        }

        if (factionId == FactionId.Gharn)
        {
            return "Sinterjack";
        }

        if (factionId == FactionId.Seethe)
        {
            return "Harrowmouth";
        }

        if (factionId == FactionId.Veirn)
        {
            return "Cauled";
        }

        if (factionId == FactionId.Vaelun)
        {
            return "Hollowguard";
        }

        return "Keelwatch Ranker";
    }

    private static string GetBuildingWorkTargetName(Frame frame, EntityRef targetEntity)
    {
        foreach ((EntityRef entity, MainBuilding mainBuilding) in frame.GetComponentIterator<MainBuilding>())
        {
            if (entity == targetEntity)
            {
                return GetMainBuildingDisplayName(frame, mainBuilding.OwnerPlayer);
            }
        }

        foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            if (entity == targetEntity)
            {
                return GetSupplyBuildingDisplayName(frame, supplyBuilding.OwnerPlayer);
            }
        }

        return "building";
    }

    private static bool IsRepairWorkTarget(Frame frame, EntityRef targetEntity)
    {
        foreach ((EntityRef entity, MainBuilding mainBuilding) in frame.GetComponentIterator<MainBuilding>())
        {
            if (entity == targetEntity)
            {
                return mainBuilding.Health > 0 && mainBuilding.Health < mainBuilding.MaxHealth;
            }
        }

        foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            if (entity == targetEntity)
            {
                return supplyBuilding.Health > 0 &&
                       supplyBuilding.Health < supplyBuilding.MaxHealth &&
                       supplyBuilding.IsConstructing == false &&
                       supplyBuilding.IsDeconstructing == false;
            }
        }

        return false;
    }

    private static bool TryGetSelectedMainBuilding(Frame frame, out EntityRef mainEntity, out MainBuilding mainBuilding, out int buildingTier)
    {
        foreach ((EntityRef entity, MainBuilding candidateMainBuilding) in frame.GetComponentIterator<MainBuilding>())
        {
            if (IsSelected(frame, entity) == false || candidateMainBuilding.OwnerPlayer != GetLocalPlayerIndex())
            {
                continue;
            }

            mainBuilding = candidateMainBuilding;
            mainEntity = entity;
            buildingTier = GetBuildingTier(frame, entity);
            return true;
        }

        mainEntity = EntityRef.None;
        mainBuilding = default;
        buildingTier = 1;
        return false;
    }

    private static bool TryGetSelectedSupplyBuilding(Frame frame, out EntityRef supplyEntity, out SupplyBuilding supplyBuilding)
    {
        foreach ((EntityRef entity, SupplyBuilding candidateSupplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            if (IsSelected(frame, entity) == false || candidateSupplyBuilding.OwnerPlayer != GetLocalPlayerIndex())
            {
                continue;
            }

            supplyEntity = entity;
            supplyBuilding = candidateSupplyBuilding;
            return true;
        }

        supplyEntity = EntityRef.None;
        supplyBuilding = default;
        return false;
    }

    private static int CountActiveBuilders(Frame frame, EntityRef supplyEntity)
    {
        int count = 0;
        foreach ((EntityRef entity, WorkerBuildIntent buildIntent) in frame.GetComponentIterator<WorkerBuildIntent>())
        {
            if (buildIntent.IsBuilding == false || buildIntent.TargetBuilding != supplyEntity)
            {
                continue;
            }

            if (IsDeadUnit(frame, entity))
            {
                continue;
            }

            count++;
        }

        return count;
    }

    private static bool TryGetWorldHealth(Frame frame, EntityRef candidateEntity, out int health, out int maxHealth, out bool isBuilding, out bool isHero)
    {
        foreach ((EntityRef entity, UnitHealth unitHealth) in frame.GetComponentIterator<UnitHealth>())
        {
            if (entity == candidateEntity)
            {
                health = unitHealth.Health;
                maxHealth = unitHealth.MaxHealth;
                isBuilding = false;
                isHero = IsHero(frame, entity);
                return true;
            }
        }

        foreach ((EntityRef entity, MainBuilding mainBuilding) in frame.GetComponentIterator<MainBuilding>())
        {
            if (entity == candidateEntity)
            {
                health = mainBuilding.Health;
                maxHealth = mainBuilding.MaxHealth;
                isBuilding = true;
                isHero = false;
                return true;
            }
        }

        foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            if (entity == candidateEntity)
            {
                health = supplyBuilding.Health;
                maxHealth = supplyBuilding.MaxHealth;
                isBuilding = true;
                isHero = false;
                return true;
            }
        }

        foreach ((EntityRef entity, Targetable targetable) in frame.GetComponentIterator<Targetable>())
        {
            if (entity == candidateEntity && IsQuillObjective(frame, entity))
            {
                health = targetable.Health;
                maxHealth = targetable.MaxHealth;
                isBuilding = true;
                isHero = false;
                return true;
            }
        }

        health = 0;
        maxHealth = 0;
        isBuilding = false;
        isHero = false;
        return false;
    }

    private static bool TryGetSelectedQuillObjective(Frame frame, out Targetable quillTargetable)
    {
        foreach ((EntityRef entity, Targetable targetable) in frame.GetComponentIterator<Targetable>())
        {
            if (IsQuillObjective(frame, entity) == false || IsSelected(frame, entity) == false)
            {
                continue;
            }

            quillTargetable = targetable;
            return true;
        }

        quillTargetable = default;
        return false;
    }

    private static bool IsQuillObjective(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, Transform2D transform) in frame.GetComponentIterator<Transform2D>())
        {
            if (entity == candidateEntity)
            {
                return QuillObjective.IsObjectivePosition(transform.Position);
            }
        }

        return false;
    }

    private static string GetQuillObjectiveStatus(Frame frame, Targetable quillTargetable)
    {
        string owner = quillTargetable.OwnerPlayer == QuillObjective.NeutralOwner
            ? "Neutral"
            : GetFactionName(frame, quillTargetable.OwnerPlayer);

        string progressLabel = quillTargetable.OwnerPlayer == QuillObjective.NeutralOwner
            ? "capture"
            : HasEnemyInQuillRadius(frame, quillTargetable.OwnerPlayer)
                ? "contested hold"
                : "victory hold";

        return $"{owner}: {progressLabel} {quillTargetable.Health}/{quillTargetable.MaxHealth}";
    }

    private static string GetQuillObjectiveBonusLabel(Targetable quillTargetable)
    {
        if (quillTargetable.OwnerPlayer == QuillObjective.NeutralOwner)
        {
            return "Bonus inactive while neutral";
        }

        return $"Ownership buff: +{QuillObjective.ResourceTrickleWood} Salvage / +{QuillObjective.ResourceTrickleIron} Plate every {FormatTicksAsSeconds(QuillObjective.ResourceTrickleIntervalTicks)}";
    }

    private static bool HasEnemyInQuillRadius(Frame frame, int ownerPlayer)
    {
        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (unitIdentity.OwnerPlayer == ownerPlayer ||
                unitIdentity.UnitKind != UnitKind.Worker && unitIdentity.UnitKind != UnitKind.Hero ||
                IsPlayerDefeated(frame, unitIdentity.OwnerPlayer) ||
                IsDeadUnit(frame, entity))
            {
                continue;
            }

            if (TryGetTransform(frame, entity, out Transform2D transform) &&
                FPVector2.Distance(transform.Position, QuillObjective.Position) <= QuillObjective.CaptureRadius)
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetEconomyState(Frame frame, int playerIndex, out PlayerEconomyState economyState)
    {
        foreach ((EntityRef entity, PlayerEconomyState candidateEconomyState) in frame.GetComponentIterator<PlayerEconomyState>())
        {
            if (candidateEconomyState.PlayerIndex == playerIndex)
            {
                economyState = candidateEconomyState;
                return true;
            }
        }

        economyState = default;
        return false;
    }

    private static string GetResourceShortfall(PlayerEconomyState economyState, int woodCost, int ironCost)
    {
        int woodShortfall = woodCost - economyState.Wood;
        int ironShortfall = ironCost - economyState.Iron;
        if (woodShortfall < 0)
        {
            woodShortfall = 0;
        }

        if (ironShortfall < 0)
        {
            ironShortfall = 0;
        }

        return FormatResourcePair(woodShortfall, ironShortfall);
    }

    private static string FormatResourcePair(int salvage, int plate)
    {
        return $"{salvage} Salvage / {plate} Plate";
    }

    private static int GetTechTier(Frame frame, int playerIndex)
    {
        foreach ((EntityRef entity, PlayerTechState techState) in frame.GetComponentIterator<PlayerTechState>())
        {
            if (techState.PlayerIndex == playerIndex)
            {
                return techState.TechTier;
            }
        }

        return 0;
    }

    private static string GetFactionName(Frame frame, int playerIndex)
    {
        foreach ((EntityRef entity, PlayerFactionState factionState) in frame.GetComponentIterator<PlayerFactionState>())
        {
            if (factionState.PlayerIndex != playerIndex)
            {
                continue;
            }

            if (factionState.FactionId == FactionId.Wrought)
            {
                return "Wrought";
            }

            if (factionState.FactionId == FactionId.Gharn)
            {
                return "Gharn";
            }

            if (factionState.FactionId == FactionId.Seethe)
            {
                return "Seethe";
            }

            if (factionState.FactionId == FactionId.Veirn)
            {
                return "Veirn";
            }

            if (factionState.FactionId == FactionId.Vaelun)
            {
                return "Vaelun";
            }

            return "Ardent Concord";
        }

        return "Unknown";
    }

    private static string GetMainBuildingDisplayName(Frame frame, int playerIndex)
    {
        int factionId = GetFactionId(frame, playerIndex);
        if (factionId == FactionId.Wrought)
        {
            return "Longhold Node";
        }

        if (factionId == FactionId.Gharn)
        {
            return "Oathpyre";
        }

        if (factionId == FactionId.Seethe)
        {
            return "Reading Kiln";
        }

        if (factionId == FactionId.Veirn)
        {
            return "Ledger Furnace";
        }

        if (factionId == FactionId.Vaelun)
        {
            return "Ration Vault";
        }

        return "Ledger House";
    }

    private static string GetSupplyBuildingDisplayName(Frame frame, int playerIndex)
    {
        int factionId = GetFactionId(frame, playerIndex);
        if (factionId == FactionId.Wrought)
        {
            return "Count Relay";
        }

        if (factionId == FactionId.Gharn)
        {
            return "Tally Stone";
        }

        if (factionId == FactionId.Seethe)
        {
            return "Pattern Archive";
        }

        if (factionId == FactionId.Veirn)
        {
            return "Keth House";
        }

        if (factionId == FactionId.Vaelun)
        {
            return "Appetite Tender";
        }

        return "Countersign Post";
    }

    private static string GetTellLabel(Frame frame, EntityRef entity, int playerIndex)
    {
        string tell = GetFactionTellLabel(frame, playerIndex);
        if (IsGrainLoud(frame, entity))
        {
            return $"Tell: {tell}  Grain-loud";
        }

        return $"Tell: {tell}";
    }

    private static string GetFactionTellLabel(Frame frame, int playerIndex)
    {
        int factionId = GetFactionId(frame, playerIndex);
        if (factionId == FactionId.Wrought)
        {
            return "Count";
        }

        if (factionId == FactionId.Gharn)
        {
            return "Burr";
        }

        if (factionId == FactionId.Seethe)
        {
            return "Pattern";
        }

        if (factionId == FactionId.Veirn)
        {
            return "Keth";
        }

        if (factionId == FactionId.Vaelun)
        {
            return "Want";
        }

        return "Countersign";
    }

    private static bool IsGrainLoud(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, GrainState grainState) in frame.GetComponentIterator<GrainState>())
        {
            if (entity == candidateEntity &&
                grainState.IsGrainLoud &&
                grainState.GrainLoudTicksRemaining > 0)
            {
                return true;
            }
        }

        return false;
    }

    private static string GetUpgradeProgressLabel(PlayerTechState techState)
    {
        if (techState.UpgradeInProgress)
        {
            return $"Upgrading to T{techState.UpgradeTargetTier}  {FormatTicksAsSeconds(techState.UpgradeTicksRemaining)} left";
        }

        return GetUpgradeResultName(techState.LastUpgradeResult);
    }

    private static string GetHeroStatusLabel(PlayerHeroState heroState)
    {
        if (heroState.RebuildInProgress)
        {
            return $"Rebuilding {FormatTicksAsSeconds(heroState.RebuildTicksRemaining)} left";
        }

        if (heroState.HasActiveHero)
        {
            return "Active";
        }

        if (heroState.RebuildAvailable)
        {
            return "Rebuild";
        }

        return "Inactive";
    }

    private static string FormatTicksAsSeconds(int ticks)
    {
        if (ticks < 0)
        {
            ticks = 0;
        }

        int wholeSeconds = (ticks + 59) / 60;
        return $"{wholeSeconds}s";
    }

    private static bool TryGetTechState(Frame frame, int playerIndex, out PlayerTechState techState)
    {
        foreach ((EntityRef entity, PlayerTechState candidateTechState) in frame.GetComponentIterator<PlayerTechState>())
        {
            if (candidateTechState.PlayerIndex == playerIndex)
            {
                techState = candidateTechState;
                return true;
            }
        }

        techState = default;
        return false;
    }

    private static bool TryGetHeroState(Frame frame, int playerIndex, out PlayerHeroState heroState)
    {
        foreach ((EntityRef entity, PlayerHeroState candidateHeroState) in frame.GetComponentIterator<PlayerHeroState>())
        {
            if (candidateHeroState.PlayerIndex == playerIndex)
            {
                heroState = candidateHeroState;
                return true;
            }
        }

        heroState = default;
        return false;
    }

    private static string GetUpgradeNotification(PlayerTechState techState)
    {
        if (techState.LastUpgradeResult == TechUpgradeResult.Upgraded)
        {
            return $"Upgrade complete: Tier {techState.TechTier}";
        }

        if (techState.LastUpgradeResult == TechUpgradeResult.Started)
        {
            return $"Upgrade started: T{techState.UpgradeTargetTier} ({FormatTicksAsSeconds(techState.UpgradeTicksRemaining)} left)";
        }

        if (techState.LastUpgradeResult == TechUpgradeResult.InProgress)
        {
            return $"Upgrade already in progress ({FormatTicksAsSeconds(techState.UpgradeTicksRemaining)} left)";
        }

        if (techState.LastUpgradeResult == TechUpgradeResult.MaxTier)
        {
            return "Upgrade blocked: max tier";
        }

        if (techState.LastUpgradeResult == TechUpgradeResult.InsufficientResources)
        {
            return "Upgrade failed: need resources";
        }

        if (techState.LastUpgradeResult == TechUpgradeResult.Defeated)
        {
            return "Upgrade blocked: player defeated";
        }

        if (techState.LastUpgradeResult == TechUpgradeResult.MissingEconomy)
        {
            return "Upgrade failed: economy missing";
        }

        return "Upgrade command received";
    }

    private static string GetHeroNotification(PlayerHeroState heroState)
    {
        if (heroState.LastHeroResult == HeroLifecycleResult.Rebuilt)
        {
            return "Hero rebuilt";
        }

        if (heroState.LastHeroResult == HeroLifecycleResult.RebuildStarted)
        {
            return $"Hero rebuild started ({FormatTicksAsSeconds(heroState.RebuildTicksRemaining)} left)";
        }

        if (heroState.LastHeroResult == HeroLifecycleResult.RebuildInProgress)
        {
            return $"Hero rebuild already in progress ({FormatTicksAsSeconds(heroState.RebuildTicksRemaining)} left)";
        }

        if (heroState.LastHeroResult == HeroLifecycleResult.InsufficientResources)
        {
            return "Hero rebuild failed: need resources";
        }

        if (heroState.LastHeroResult == HeroLifecycleResult.RebuildUnavailable)
        {
            return "Hero rebuild unavailable";
        }

        if (heroState.LastHeroResult == HeroLifecycleResult.MissingMainBase)
        {
            return "Hero rebuild blocked: no main base";
        }

        if (heroState.LastHeroResult == HeroLifecycleResult.Defeated)
        {
            return "Hero rebuild blocked: player defeated";
        }

        if (heroState.LastHeroResult == HeroLifecycleResult.RebuildAvailable)
        {
            return "Hero down: press R to rebuild";
        }

        return "Hero command received";
    }

    private static bool IsHeroNotificationResult(int result)
    {
        return result == HeroLifecycleResult.Rebuilt ||
               result == HeroLifecycleResult.RebuildStarted ||
               result == HeroLifecycleResult.RebuildInProgress ||
               result == HeroLifecycleResult.InsufficientResources ||
               result == HeroLifecycleResult.RebuildUnavailable ||
               result == HeroLifecycleResult.MissingMainBase ||
               result == HeroLifecycleResult.Defeated ||
               result == HeroLifecycleResult.RebuildAvailable;
    }

    private static string GetUpgradeResultName(int result)
    {
        if (result == TechUpgradeResult.Upgraded)
        {
            return "Upgraded";
        }

        if (result == TechUpgradeResult.Started)
        {
            return "Started";
        }

        if (result == TechUpgradeResult.InProgress)
        {
            return "In Progress";
        }

        if (result == TechUpgradeResult.MaxTier)
        {
            return "Max Tier";
        }

        if (result == TechUpgradeResult.MissingEconomy)
        {
            return "Missing Economy";
        }

        if (result == TechUpgradeResult.InsufficientResources)
        {
            return "Need Resources";
        }

        if (result == TechUpgradeResult.Defeated)
        {
            return "Defeated";
        }

        return "None";
    }

    private static string GetHeroResultName(int result)
    {
        if (result == HeroLifecycleResult.Active)
        {
            return "Active";
        }

        if (result == HeroLifecycleResult.Defeated)
        {
            return "Defeated";
        }

        if (result == HeroLifecycleResult.MissingMainBase)
        {
            return "No Base";
        }

        if (result == HeroLifecycleResult.RebuildAvailable)
        {
            return "Rebuild Ready";
        }

        if (result == HeroLifecycleResult.Rebuilt)
        {
            return "Rebuilt";
        }

        if (result == HeroLifecycleResult.RebuildStarted)
        {
            return "Rebuild Started";
        }

        if (result == HeroLifecycleResult.RebuildInProgress)
        {
            return "Rebuilding";
        }

        if (result == HeroLifecycleResult.InsufficientResources)
        {
            return "Need Resources";
        }

        if (result == HeroLifecycleResult.RebuildUnavailable)
        {
            return "Unavailable";
        }

        return "None";
    }

    private static int CountBases(Frame frame)
    {
        int count = 0;
        foreach ((EntityRef entity, MainBuilding mainBuilding) in frame.GetComponentIterator<MainBuilding>())
        {
            count++;
        }

        return count;
    }

    private static int CountSupplyBuildings(Frame frame)
    {
        int count = 0;
        foreach ((EntityRef entity, SupplyBuilding supplyBuilding) in frame.GetComponentIterator<SupplyBuilding>())
        {
            count++;
        }

        return count;
    }

    private static void DrawSelectedWorkerBuildPanel(Frame frame, GUIStyle headerStyle, GUIStyle labelStyle)
    {
        if (TryGetSelectedOwnedWorker(frame, out EntityRef workerEntity) == false)
        {
            return;
        }

        bool isBuilding = TryGetWorkerBuildIntent(frame, workerEntity, out WorkerBuildIntent buildIntent) && buildIntent.IsBuilding;
        Rect panelRect = new Rect(Screen.width - RightPanelWidth - 20, 224, RightPanelWidth, isBuilding || AnachronQuantumInput.BuildModeActive ? 106 : 88);
        DrawPanel(panelRect);

        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 8, RightPanelContentWidth, RowHeight), "Selected Worker", headerStyle);
        if (isBuilding)
        {
            if (IsRepairWorkTarget(frame, buildIntent.TargetBuilding))
            {
                GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 30, RightPanelContentWidth, RowHeight), $"Repairing {GetBuildingWorkTargetName(frame, buildIntent.TargetBuilding)}", labelStyle);
                GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 52, RightPanelContentWidth, RowHeight), "Repair cost: 2 Salvage / 1 Plate", labelStyle);
                return;
            }

            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 30, RightPanelContentWidth, RowHeight), $"Building {GetSupplyBuildingDisplayName(frame, GetLocalPlayerIndex())}", labelStyle);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 52, RightPanelContentWidth, RowHeight), "Cancel construction: full refund - press X", labelStyle);
            return;
        }

        if (AnachronQuantumInput.BuildModeActive)
        {
            FactionStats stats = FactionStats.ForPlayer(frame, GetLocalPlayerIndex());
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 30, RightPanelContentWidth, RowHeight), $"{GetSupplyBuildingDisplayName(frame, GetLocalPlayerIndex())}: {FormatResourcePair(stats.SupplyBuildingWoodCost, stats.SupplyBuildingIronCost)}", labelStyle);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 52, RightPanelContentWidth, RowHeight), $"+{stats.SupplyBuildingFoodProvided} Holding - press C to place", labelStyle);
            GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 74, RightPanelContentWidth, RowHeight), AnachronBuildPlacementPreview.PlacementStatus, labelStyle);
            return;
        }

        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 30, RightPanelContentWidth, RowHeight), "Build: press B", labelStyle);
        GUI.Label(new Rect(panelRect.x + 12, panelRect.y + 52, RightPanelContentWidth, RowHeight), $"{GetSupplyBuildingDisplayName(frame, GetLocalPlayerIndex())} available", labelStyle);
    }

    private static bool HasSelectedOwnedWorker(Frame frame)
    {
        return TryGetSelectedOwnedWorker(frame, out EntityRef workerEntity);
    }

    private static bool TryGetSelectedOwnedWorker(Frame frame, out EntityRef workerEntity)
    {
        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (unitIdentity.OwnerPlayer == GetLocalPlayerIndex() &&
                unitIdentity.UnitKind == UnitKind.Worker &&
                IsSelected(frame, entity) &&
                IsDeadUnit(frame, entity) == false)
            {
                workerEntity = entity;
                return true;
            }
        }

        workerEntity = EntityRef.None;
        return false;
    }

    private static void DrawMatchBanner(Frame frame)
    {
        bool localPlayerDefeated = IsPlayerDefeated(frame, GetLocalPlayerIndex());
        int activePlayers = CountActivePlayers(frame);
        if (localPlayerDefeated == false && activePlayers > 1)
        {
            return;
        }

        string message = GetMatchBannerMessage(frame, localPlayerDefeated);
        Color panelColor = localPlayerDefeated ? DefeatPanelColor : VictoryPanelColor;

        GUIStyle bannerStyle = new GUIStyle(GUI.skin.label);
        bannerStyle.alignment = TextAnchor.MiddleCenter;
        bannerStyle.fontSize = 28;
        bannerStyle.fontStyle = FontStyle.Bold;
        bannerStyle.normal.textColor = TextColor;

        Rect rect = new Rect((Screen.width - 560) * 0.5f, 18, 560, 48);
        DrawPanel(rect, panelColor);
        GUI.Label(rect, message, bannerStyle);
    }

    private static string GetMatchBannerMessage(Frame frame, bool localPlayerDefeated)
    {
        if (TryGetQuillVictoryOwner(frame, out int quillOwner))
        {
            if (quillOwner == GetLocalPlayerIndex())
            {
                return "VICTORY - Quill-Waist Held";
            }

            if (localPlayerDefeated)
            {
                return "DEFEAT - Enemy Held the Quill-Waist";
            }
        }

        return localPlayerDefeated ? "DEFEAT - Main Building Destroyed" : "VICTORY - Enemy Main Buildings Destroyed";
    }

    private static bool TryGetQuillVictoryOwner(Frame frame, out int ownerPlayer)
    {
        foreach ((EntityRef entity, Targetable targetable) in frame.GetComponentIterator<Targetable>())
        {
            if (IsQuillObjective(frame, entity) == false ||
                targetable.OwnerPlayer == QuillObjective.NeutralOwner ||
                targetable.Health > 0)
            {
                continue;
            }

            ownerPlayer = targetable.OwnerPlayer;
            return true;
        }

        ownerPlayer = QuillObjective.NeutralOwner;
        return false;
    }

    private static int CountActivePlayers(Frame frame)
    {
        int count = 0;
        foreach ((EntityRef entity, PlayerEconomyState economyState) in frame.GetComponentIterator<PlayerEconomyState>())
        {
            if (economyState.IsDefeated == false)
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsPlayerDefeated(Frame frame, int playerIndex)
    {
        foreach ((EntityRef entity, PlayerEconomyState economyState) in frame.GetComponentIterator<PlayerEconomyState>())
        {
            if (economyState.PlayerIndex == playerIndex)
            {
                return economyState.IsDefeated;
            }
        }

        return false;
    }

    private static string GetUnitState(Frame frame, EntityRef entity)
    {
        if (IsDeadUnit(frame, entity))
        {
            return "Dead";
        }

        if (TryGetWorkerBuildIntent(frame, entity, out WorkerBuildIntent buildIntent) && buildIntent.IsBuilding)
        {
            if (IsRepairWorkTarget(frame, buildIntent.TargetBuilding))
            {
                return $"Repairing {GetBuildingWorkTargetName(frame, buildIntent.TargetBuilding)}";
            }

            return TryGetUnitOwner(frame, entity, out int ownerPlayer) ? $"Building {GetSupplyBuildingDisplayName(frame, ownerPlayer)}" : "Building support";
        }

        if (TryGetGatherIntent(frame, entity, out GatherIntent gatherIntent) == false || gatherIntent.HasTarget == false)
        {
            return TryGetAttackIntent(frame, entity, out AttackIntent attackIntent) && attackIntent.HasTarget
                ? attackIntent.IsInRange ? $"Attacking cd {attackIntent.CooldownTicksRemaining}" : "Moving to target"
                : "Idle";
        }

        if (TryGetWorkerCarry(frame, entity, out WorkerResourceCarry carry) == false)
        {
            return "Idle";
        }

        if (carry.Amount >= carry.Capacity)
        {
            return "Returning";
        }

        if (TryGetTransform(frame, entity, out Transform2D workerTransform) && TryGetTransform(frame, gatherIntent.TargetNode, out Transform2D nodeTransform))
        {
            float distance = Vector3.Distance(workerTransform.Position.ToUnityVector3(), nodeTransform.Position.ToUnityVector3());
            return distance <= 0.9f ? "Gathering" : "Moving to resource";
        }

        return "Resource missing";
    }

    private static string GetCarryLabel(Frame frame, EntityRef entity)
    {
        if (TryGetWorkerCarry(frame, entity, out WorkerResourceCarry carry) == false)
        {
            return string.Empty;
        }

        string resourceName = carry.ResourceKind == ResourceKind.Wood ? "Salvage" : carry.ResourceKind == ResourceKind.Iron ? "Plate" : "None";
        return $"{carry.Amount}/{carry.Capacity} {resourceName}";
    }

    private static string GetAttackLabel(Frame frame, EntityRef entity)
    {
        if (TryGetAttackIntent(frame, entity, out AttackIntent attackIntent) == false || IsHero(frame, entity) == false)
        {
            return string.Empty;
        }

        return $"DMG {attackIntent.Damage}";
    }

    private static string GetUnitDisplayName(Frame frame, UnitIdentity unitIdentity)
    {
        int factionId = GetFactionId(frame, unitIdentity.OwnerPlayer);
        if (unitIdentity.UnitKind == UnitKind.Hero)
        {
            if (factionId == FactionId.Wrought)
            {
                return "Wrought Overseer";
            }

            if (factionId == FactionId.Gharn)
            {
                return "Tally Captain";
            }

            if (factionId == FactionId.Seethe)
            {
                return "The Incipit";
            }

            if (factionId == FactionId.Veirn)
            {
                return "Ordal Executor";
            }

            if (factionId == FactionId.Vaelun)
            {
                return "Nightshear";
            }

            return "Concord Marshal";
        }

        if (factionId == FactionId.Wrought)
        {
            return "Wright";
        }

        if (factionId == FactionId.Gharn)
        {
            return "Sinterjack";
        }

        if (factionId == FactionId.Seethe)
        {
            return "Harrowmouth";
        }

        if (factionId == FactionId.Veirn)
        {
            return "Cauled";
        }

        if (factionId == FactionId.Vaelun)
        {
            return "Hollowguard";
        }

        return "Keelwatch Ranker";
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

        return FactionId.ArdentConcord;
    }

    private static string GetUnitHealth(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, UnitHealth unitHealth) in frame.GetComponentIterator<UnitHealth>())
        {
            if (entity == candidateEntity)
            {
                return $"{unitHealth.Health}/{unitHealth.MaxHealth}";
            }
        }

        return "-";
    }

    private static bool IsDeadUnit(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, UnitHealth unitHealth) in frame.GetComponentIterator<UnitHealth>())
        {
            if (entity == candidateEntity)
            {
                return unitHealth.IsDead;
            }
        }

        return false;
    }

    private static bool IsOwnedByLocalPlayer(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (entity == candidateEntity)
            {
                return unitIdentity.OwnerPlayer == GetLocalPlayerIndex();
            }
        }

        return false;
    }

    private static int GetLocalPlayerIndex()
    {
        return QuantumPhase0LocalSessionController.ActivePlayerSlot;
    }

    private static bool TryGetUnitOwner(Frame frame, EntityRef candidateEntity, out int ownerPlayer)
    {
        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (entity == candidateEntity)
            {
                ownerPlayer = unitIdentity.OwnerPlayer;
                return true;
            }
        }

        ownerPlayer = 0;
        return false;
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

    private static bool IsHero(Frame frame, EntityRef candidateEntity)
    {
        foreach ((EntityRef entity, UnitIdentity unitIdentity) in frame.GetComponentIterator<UnitIdentity>())
        {
            if (entity == candidateEntity)
            {
                return unitIdentity.UnitKind == UnitKind.Hero;
            }
        }

        return false;
    }

    private static string GetShortEntityName(EntityRef entity)
    {
        string value = entity.ToString();
        int separatorIndex = value.IndexOf('.');
        return separatorIndex >= 0 && separatorIndex + 1 < value.Length ? $"E{value.Substring(separatorIndex + 1)}" : value;
    }

    private static bool TryGetAttackIntent(Frame frame, EntityRef candidateEntity, out AttackIntent attackIntent)
    {
        foreach ((EntityRef entity, AttackIntent candidateAttackIntent) in frame.GetComponentIterator<AttackIntent>())
        {
            if (entity == candidateEntity)
            {
                attackIntent = candidateAttackIntent;
                return true;
            }
        }

        attackIntent = default;
        return false;
    }

    private static bool TryGetWorkerCarry(Frame frame, EntityRef candidateEntity, out WorkerResourceCarry carry)
    {
        foreach ((EntityRef entity, WorkerResourceCarry candidateCarry) in frame.GetComponentIterator<WorkerResourceCarry>())
        {
            if (entity == candidateEntity)
            {
                carry = candidateCarry;
                return true;
            }
        }

        carry = default;
        return false;
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

    private static bool TryGetWorkerBuildIntent(Frame frame, EntityRef candidateEntity, out WorkerBuildIntent buildIntent)
    {
        foreach ((EntityRef entity, WorkerBuildIntent candidateBuildIntent) in frame.GetComponentIterator<WorkerBuildIntent>())
        {
            if (entity == candidateEntity)
            {
                buildIntent = candidateBuildIntent;
                return true;
            }
        }

        buildIntent = default;
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

    private static void DrawPanel(Rect rect)
    {
        DrawPanel(rect, PanelColor);
    }

    private static void DrawPanel(Rect rect, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previousColor;
    }

    private static void DrawProgressBar(Rect rect, int totalTicks, int ticksRemaining)
    {
        float normalized = 0.0f;
        if (totalTicks > 0)
        {
            normalized = Mathf.Clamp01(1.0f - (float)ticksRemaining / totalTicks);
        }

        DrawPanel(rect, ProgressTrackColor);
        DrawPanel(new Rect(rect.x, rect.y, rect.width * normalized, rect.height), ProgressFillColor);
    }

    private static void DrawHealthBar(Rect rect, int health, int maxHealth, GUIStyle labelStyle)
    {
        float normalized = 0.0f;
        if (maxHealth > 0)
        {
            normalized = Mathf.Clamp01((float)health / maxHealth);
        }

        Color fillColor = HealthFillColor;
        if (normalized <= 0.33f)
        {
            fillColor = CriticalHealthFillColor;
        }
        else if (normalized <= 0.66f)
        {
            fillColor = DamagedHealthFillColor;
        }

        DrawPanel(rect, ProgressTrackColor);
        DrawPanel(new Rect(rect.x, rect.y, rect.width * normalized, rect.height), fillColor);

        GUIStyle healthStyle = new GUIStyle(labelStyle);
        healthStyle.alignment = TextAnchor.MiddleCenter;
        healthStyle.fontStyle = FontStyle.Bold;
        healthStyle.fontSize = 14;
        healthStyle.normal.textColor = TextColor;
        GUI.Label(rect, $"HP {health} / {maxHealth}", healthStyle);
    }
}
