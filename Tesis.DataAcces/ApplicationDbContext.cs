using Microsoft.EntityFrameworkCore;
using Tesis.Domain.Entities;
using Tesis.Domain.Models;


namespace Tesis.DataAcces
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {

        public DbSet<IndicadorModel> Indicadores => Set<IndicadorModel>();
        public DbSet<ProcesoModel> Procesos => Set<ProcesoModel>();
        public DbSet<ObjetivoModel> Objetivos => Set<ObjetivoModel>();
        public DbSet<ObjetivoProcesoIndicadorModel> ObjetivoProcesosIndicadores => Set<ObjetivoProcesoIndicadorModel>();
        public DbSet<User> Usuarios => Set<User>();

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

            // Configuración para la entidad intermedia ObjetivoProcesoIndicadorModel
            modelBuilder.Entity<ObjetivoProcesoIndicadorModel>()
                .HasKey(opi => new { opi.ObjetivoId, opi.ProcesoId, opi.IndicadorId });

            modelBuilder.Entity<ObjetivoProcesoIndicadorModel>()
                .HasOne(opi => opi.Objetivo)
                .WithMany(o => o.ObjetivoProcesosIndicadores)
                .HasForeignKey(opi => opi.ObjetivoId);

            modelBuilder.Entity<ObjetivoProcesoIndicadorModel>()
                .HasOne(opi => opi.Proceso)
                .WithMany(p => p.ObjetivoProcesoIndicadores)
                .HasForeignKey(opi => opi.ProcesoId);

            modelBuilder.Entity<ObjetivoProcesoIndicadorModel>()
                .HasOne(opi => opi.Indicador)
                .WithMany(i => i.ObjetivoProcesoIndicadores)
                .HasForeignKey(opi => opi.IndicadorId);

        }
    }
}
