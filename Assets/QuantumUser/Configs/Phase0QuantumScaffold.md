# Phase 0 Quantum Config Scaffold

This folder is reserved for project-owned Quantum configuration assets.

Create the SDK-backed assets in Unity after Photon Quantum is imported:

- `QuantumDefaultConfigs.asset`
- `QuantumDeterministicSessionConfig.asset`
- `QuantumEditorSettings.asset`

Phase 0 config policy:

- Simulation tick rate: use Quantum's deterministic session config, targeting 60 Hz.
- Math: gameplay code must use Quantum fixed-point types only (`FP`, `FPVector2`, `FPVector3`).
- Systems: no gameplay systems are registered in Phase 0.
