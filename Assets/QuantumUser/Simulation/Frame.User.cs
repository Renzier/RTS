using Photon.Deterministic;

namespace Quantum {
  internal static class Phase0FrameBootstrap {
    public static FP BuildFixedTickScale(SimulationConfig simulationConfig) {
      if (simulationConfig == null) {
        return FP._1;
      }

      return simulationConfig.Phase0TickScale <= FP._0 ? FP._1 : simulationConfig.Phase0TickScale;
    }
  }
}
