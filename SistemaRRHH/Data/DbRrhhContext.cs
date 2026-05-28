using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SistemaRRHH.Models;

namespace SistemaRRHH.Data;

public partial class DbRrhhContext : DbContext
{
    public DbRrhhContext(DbContextOptions<DbRrhhContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Trabajadore> Trabajadores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Trabajadore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("trabajadores");

            entity.HasIndex(e => e.Rut, "Rut").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Rut).HasMaxLength(12);
            entity.Property(e => e.SueldoBase).HasPrecision(10, 2);
            entity.Property(e => e.ValorHora).HasPrecision(10, 2);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
