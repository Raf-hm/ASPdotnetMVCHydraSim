using HydraSim.Domain.Components;
using HydraSim.Domain.Simulation;

namespace HydraSim.DAL.Repositories
{
    public interface ISimulationRepository
    {
        Task<HydraulicSimulation?> LoadAsync(int id);
        Task<List<HydraulicSimulation>> ListAsync();
        Task<bool> ResetAsync(int id);
        Task UpdateComponentAsync(HydraulicComponent component);
    }
}
