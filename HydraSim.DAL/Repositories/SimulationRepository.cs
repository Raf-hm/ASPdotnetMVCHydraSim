using HydraSim.DAL.Data;
using HydraSim.Domain.Simulation;
using Microsoft.EntityFrameworkCore;

namespace HydraSim.DAL.Repositories
{
    public class SimulationRepository : ISimulationRepository
    {
        private readonly HydraSimDbContext _db;

        public SimulationRepository(HydraSimDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<HydraulicSimulation?> LoadAsync(int id)
        {
            var simulation = await _db.Simulations
                .Include(s => s.Components)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (simulation == null) return null;

            simulation.AutoConnect();
            return simulation;
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
        public Task<List<HydraulicSimulation>> ListAsync()
        {
            return _db.Simulations
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async Task<bool> ResetAsync(int id)
        {
            var existing = await _db.Simulations
                .Include(s => s.Components)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existing == null) return false;

            var template = SeedData.BuildTemplate(id);
            if (template == null) return false;

            var toRemove = existing.Components.ToList();
            existing.Components.Clear();
            _db.Components.RemoveRange(toRemove);

            foreach (var c in template.Components)
            {
                c.SimulationId = existing.Id;
                existing.Components.Add(c);
            }

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
