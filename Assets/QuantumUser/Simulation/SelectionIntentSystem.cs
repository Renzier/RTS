namespace Quantum
{
    public unsafe class SelectionIntentSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            int configuredPlayer = GetConfiguredInputPlayer(f);
            Input* input = f.GetPlayerInput(configuredPlayer);
            if (input == null)
            {
                return;
            }

            f.Global->LastInputPlayer = configuredPlayer;
            f.Global->LastCommandIntent = input->CommandIntent;
            f.Global->LastUpgradeIntent = input->UpgradeIntent;
            f.Global->LastSelectHeld = input->Select.IsDown;
            f.Global->LastCommandHeld = input->Command.IsDown;
            f.Global->LastAdditiveSelectHeld = input->AdditiveSelect.IsDown;
            f.Global->LastDragSelectHeld = input->DragSelect.IsDown;
            f.Global->LastPointerScreen = input->PointerScreen;
            f.Global->LastDragStartScreen = input->DragStartScreen;
            f.Global->LastDragEndScreen = input->DragEndScreen;
            f.Global->LastPointerWorld = input->PointerWorld;
            f.Global->LastDragStartWorld = input->DragStartWorld;
            f.Global->LastDragEndWorld = input->DragEndWorld;
        }

        private static int GetConfiguredInputPlayer(Frame f)
        {
            int configuredPlayerSlot = f.RuntimeConfig.Phase0PlayerSlot;
            if (configuredPlayerSlot < 0 || configuredPlayerSlot >= f.MaxPlayerCount)
            {
                return 0;
            }

            return configuredPlayerSlot;
        }
    }
}
