using HydraSim.Domain.Components;
using HydraSim.Domain.Simulation;
using Microsoft.EntityFrameworkCore;

namespace HydraSim.DAL.Data
{
    public class HydraSimDbContext : DbContext
    {
        public HydraSimDbContext(DbContextOptions<HydraSimDbContext> options) : base(options) { }
        public DbSet<HydraulicSimulation> Simulations => Set<HydraulicSimulation>();
        public DbSet<HydraulicComponent> Components => Set<HydraulicComponent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HydraulicSimulation>(b =>
            {
                b.ToTable("Simulations");
                b.HasKey(s => s.Id);
                b.Property(s => s.Name).HasMaxLength(100);
                b.Property(s => s.Description).HasMaxLength(1000);
                b.Property(s => s.ImagePath).HasMaxLength(200);

                b.HasMany(s => s.Components)
                    .WithOne()
                    .HasForeignKey(c => c.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<HydraulicComponent>(b =>
            {
                b.ToTable("Components");
                b.HasKey(c => c.Id);

                b.HasDiscriminator<string>("ComponentType")
                    .HasValue<Pipe>("Pipe")
                    .HasValue<Pump>("Pump")
                    .HasValue<Motor>("Motor")
                    .HasValue<ReliefValve>("ReliefValve")
                    .HasValue<Resistance>("Resistance")
                    .HasValue<Tank>("Tank")
                    .HasValue<PressureGauge>("PressureGauge");
            });
        }
    }
}
