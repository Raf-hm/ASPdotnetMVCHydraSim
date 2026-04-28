using HydraSim.Domain.Simulation;

namespace HydraSim.DAL.Repositories
{
    public interface ISimulationRepository
    {
        HydraulicSimulation BuildSimulation(int id);
        void RebuildConnections(HydraulicSimulation simulation, int id);
        void SaveToSession(HydraulicSimulation simulation, int id, Action<string, string> sessionSetter);
        HydraulicSimulation? LoadFromSession(int id, Func<string, string?> sessionGetter);
    }
}
