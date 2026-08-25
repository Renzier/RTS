namespace Quantum
{
    public unsafe class SelectionIntentSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            for (int playerIndex = 0; playerIndex < f.MaxPlayerCount; playerIndex++)
            {
                Input* input = f.GetPlayerInput(playerIndex);

                if (input == null)
                {
                    continue;
                }

                bool selectHeld = input->Select.IsDown;
                bool commandHeld = input->Command.IsDown;
                bool additiveSelectHeld = input->AdditiveSelect.IsDown;
                bool dragSelectHeld = input->DragSelect.IsDown;
                bool hasUpgradeIntent = input->UpgradeIntent != 0;

                if (!selectHeld && !commandHeld && !additiveSelectHeld && !dragSelectHeld && !hasUpgradeIntent)
                {
                    continue;
                }

                f.Global->LastInputPlayer = GetConfiguredInputPlayer(f);
                f.Global->LastCommandIntent = input->CommandIntent;
                f.Global->LastUpgradeIntent = input->UpgradeIntent;
                f.Global->LastSelectHeld = selectHeld;
                f.Global->LastCommandHeld = commandHeld;
                f.Global->LastAdditiveSelectHeld = additiveSelectHeld;
                f.Global->LastDragSelectHeld = dragSelectHeld;
                f.Global->LastPointerScreen = input->PointerScreen;
                f.Global->LastDragStartScreen = input->DragStartScreen;
                f.Global->LastDragEndScreen = input->DragEndScreen;
                f.Global->LastPointerWorld = input->PointerWorld;
                f.Global->LastDragStartWorld = input->DragStartWorld;
                f.Global->LastDragEndWorld = input->DragEndWorld;
            }
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
