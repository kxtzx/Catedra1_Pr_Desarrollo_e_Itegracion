using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Models;

namespace SistemaArriendos.Data;

public partial class DbVehiculosContext : DbContext
{
    public DbVehiculosContext(DbContextOptions<DbVehiculosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Arriendo> Arriendos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Mantencione> Mantenciones { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Arriendo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("arriendos");

            entity.HasIndex(e => e.ClienteId, "ClienteId");

            entity.HasIndex(e => e.VehiculoId, "VehiculoId");

            entity.Property(e => e.PrecioDiario).HasPrecision(10, 2);
            entity.Property(e => e.PrecioTotal).HasPrecision(10, 2);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Arriendos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("arriendos_ibfk_2");

            entity.HasOne(d => d.Vehiculo).WithMany(p => p.Arriendos)
                .HasForeignKey(d => d.VehiculoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("arriendos_ibfk_1");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("clientes");

            entity.HasIndex(e => e.Rut, "Rut").IsUnique();

            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Rut).HasMaxLength(12);
        });

        modelBuilder.Entity<Mantencione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("mantenciones");

            entity.HasIndex(e => e.VehiculoId, "VehiculoId");

            entity.Property(e => e.HorasTrabajadas).HasPrecision(5, 2);
            entity.Property(e => e.RutMecanico).HasMaxLength(12);

            entity.HasOne(d => d.Vehiculo).WithMany(p => p.Mantenciones)
                .HasForeignKey(d => d.VehiculoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mantenciones_ibfk_1");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("vehiculos");

            entity.HasIndex(e => e.Codigo, "Codigo").IsUnique();

            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Modelo).HasMaxLength(50);
            entity.Property(e => e.Patente).HasMaxLength(10);
            entity.Property(e => e.PrecioArriendoDiario).HasPrecision(10, 2);
            entity.Property(e => e.Tipo).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
