using Microsoft.EntityFrameworkCore;
using Tesis.Domain.Models;


namespace Tesis.DataAcces
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {

        public DbSet<IndicadorModel> Indicadores => Set<IndicadorModel>();
        public DbSet<ProcesoModel> Procesos => Set<ProcesoModel>();
        public DbSet<ObjetivoModel> Objetivos => Set<ObjetivoModel>();

       // public DbSet<ObjetivoProcesoModel> ObjetivoProcesos => Set<ObjetivoProcesoModel>();
        //public DbSet<ObjetivoIndicadorModel> ObjetivoIndicadores => Set<ObjetivoIndicadorModel>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IndicadorModel>()
                .Property(p => p.MetaCumplirValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IndicadorModel>()
                .Property(p => p.MetaRealValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ObjetivoProcesoModel>()
            .HasKey(op => new { op.ObjetivoId, op.ProcesoId });

            modelBuilder.Entity<ObjetivoProcesoModel>()
                .HasOne(op => op.Objetivo)
                .WithMany(o => o.ObjetivoProcesos)
                .HasForeignKey(op => op.ObjetivoId);

            modelBuilder.Entity<ObjetivoProcesoModel>()
                .HasOne(op => op.Proceso)
                .WithMany(p => p.ObjetivoProcesos)
                .HasForeignKey(op => op.ProcesoId);

        }
    }
}
