using HydraSim.Domain.Auth;
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
        public DbSet<User> Users => Set<User>();

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

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("Users");
                b.HasKey(u => u.Id);
                b.Property(u => u.Email).HasMaxLength(255).IsRequired();
                b.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
                b.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}
