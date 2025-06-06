﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tesis.DataAcces;

#nullable disable

namespace Tesis.DataAcces.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250429005221_AddUserRole")]
    partial class AddUserRole
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Tesis.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("Tesis.Domain.Models.IndicadorModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Evaluacion")
                        .HasColumnType("int");

                    b.Property<bool>("IsMetaCumplirPorcentage")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMetaRealPorcentage")
                        .HasColumnType("bit");

                    b.Property<string>("MetaCumplir")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("MetaCumplirValue")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("MetaReal")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("MetaRealValue")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("ProcesoId")
                        .HasColumnType("int");

                    b.Property<int>("Tipo")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProcesoId");

                    b.ToTable("Indicadores");
                });

            modelBuilder.Entity("Tesis.Domain.Models.ObjetivoIndicadorModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IndicadorId")
                        .HasColumnType("int");

                    b.Property<int>("ObjetivoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IndicadorId");

                    b.HasIndex("ObjetivoId");

                    b.ToTable("ObjetivoIndicadorModel");
                });

            modelBuilder.Entity("Tesis.Domain.Models.ObjetivoModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Evaluacion")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Objetivos");
                });

            modelBuilder.Entity("Tesis.Domain.Models.ObjetivoProcesoIndicadorModel", b =>
                {
                    b.Property<int>("ObjetivoId")
                        .HasColumnType("int");

                    b.Property<int>("ProcesoId")
                        .HasColumnType("int");

                    b.Property<int>("IndicadorId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("ObjetivoId", "ProcesoId", "IndicadorId");

                    b.HasIndex("IndicadorId");

                    b.HasIndex("ProcesoId");

                    b.ToTable("ObjetivoProcesosIndicadores");
                });

            modelBuilder.Entity("Tesis.Domain.Models.ObjetivoProcesoModel", b =>
                {
                    b.Property<int>("ObjetivoId")
                        .HasColumnType("int");

                    b.Property<int>("ProcesoId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("ObjetivoId", "ProcesoId");

                    b.HasIndex("ProcesoId");

                    b.ToTable("ObjetivoProcesoModel");
                });

            modelBuilder.Entity("Tesis.Domain.Models.ProcesoModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Evaluacion")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Procesos");
                });

            modelBuilder.Entity("Tesis.Domain.Models.IndicadorModel", b =>
                {
                    b.HasOne("Tesis.Domain.Models.ProcesoModel", "Proceso")
                        .WithMany("Indicadores")
                        .HasForeignKey("ProcesoId");

                    b.Navigation("Proceso");
                });

            modelBuilder.Entity("Tesis.Domain.Models.ObjetivoIndicadorModel", b =>
                {
                    b.HasOne("Tesis.Domain.Models.IndicadorModel", "Indicador")
                        .WithMany("ObjetivoIndicadores")
                        .HasForeignKey("IndicadorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tesis.Domain.Models.ObjetivoModel", "Objetivo")
                        .WithMany("ObjetivoIndicadores")
                        .HasForeignKey("ObjetivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Indicador");

                    b.Navigation("Objetivo");
                });

            modelBuilder.Entity("Tesis.Domain.Models.ObjetivoProcesoIndicadorModel", b =>
                {
                    b.HasOne("Tesis.Domain.Models.IndicadorModel", "Indicador")
                        .WithMany("ObjetivoProcesoIndicadores")
                        .HasForeignKey("IndicadorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tesis.Domain.Models.ObjetivoModel", "Objetivo")
                        .WithMany("ObjetivoProcesosIndicadores")
                        .HasForeignKey("ObjetivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tesis.Domain.Models.ProcesoModel", "Proceso")
                        .WithMany("ObjetivoProcesoIndicadores")
                        .HasForeignKey("ProcesoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Indicador");

                    b.Navigation("Objetivo");

                    b.Navigation("Proceso");
                });

            modelBuilder.Entity("Tesis.Domain.Models.ObjetivoProcesoModel", b =>
                {
                    b.HasOne("Tesis.Domain.Models.ObjetivoModel", "Objetivo")
                        .WithMany("ObjetivoProcesos")
                        .HasForeignKey("ObjetivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tesis.Domain.Models.ProcesoModel", "Proceso")
                        .WithMany("ObjetivoProcesos")
                        .HasForeignKey("ProcesoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Objetivo");

                    b.Navigation("Proceso");
                });

            modelBuilder.Entity("Tesis.Domain.Models.IndicadorModel", b =>
                {
                    b.Navigation("ObjetivoIndicadores");

                    b.Navigation("ObjetivoProcesoIndicadores");
                });

            modelBuilder.Entity("Tesis.Domain.Models.ObjetivoModel", b =>
                {
                    b.Navigation("ObjetivoIndicadores");

                    b.Navigation("ObjetivoProcesos");

                    b.Navigation("ObjetivoProcesosIndicadores");
                });

            modelBuilder.Entity("Tesis.Domain.Models.ProcesoModel", b =>
                {
                    b.Navigation("Indicadores");

                    b.Navigation("ObjetivoProcesoIndicadores");

                    b.Navigation("ObjetivoProcesos");
                });
#pragma warning restore 612, 618
        }
    }
}
