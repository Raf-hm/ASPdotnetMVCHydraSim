using HydraSim.Domain.Simulation;

namespace HydraSim.DAL.Repositories
{
    public interface ISimulationRepository
    {
        Task<HydraulicSimulation?> LoadAsync(int id);
        Task<List<HydraulicSimulation>> ListAsync();
        Task SaveChangesAsync();
        Task<bool> ResetAsync(int id);
    }
}
