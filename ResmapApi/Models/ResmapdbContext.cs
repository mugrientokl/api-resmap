using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ResmapApi.Models;

public partial class ResmapdbContext : DbContext
{
    public ResmapdbContext()
    {
    }

    public ResmapdbContext(DbContextOptions<ResmapdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<DetalleSolicitud> DetalleSolicituds { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedore> Proveedores { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SolicitudesPedido> SolicitudesPedidos { get; set; }

    public virtual DbSet<SolicitudesProveedor> SolicitudesProveedors { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07EF6FC8E5");

            entity.HasIndex(e => e.Nombre, "UQ_Categorias_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<DetalleSolicitud>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DetalleS__3214EC0721B555A8");

            entity.ToTable("DetalleSolicitud");

            entity.Property(e => e.PrecioReferencial).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetalleSolicituds)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleSolicitud_Producto");

            entity.HasOne(d => d.SolicitudPedido).WithMany(p => p.DetalleSolicituds)
                .HasForeignKey(d => d.SolicitudPedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleSolicitud_Solicitud");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producto__3214EC0703B206C4");

            entity.HasIndex(e => e.Codigo, "UQ_Productos_Codigo").IsUnique();

            entity.Property(e => e.Codigo).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Marca).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Precio).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Categorias");
        });

        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Proveedo__3214EC07EFC6177B");

            entity.HasIndex(e => e.Email, "UQ_Proveedores_Email").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Telefono).HasMaxLength(30);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07067E1288");

            entity.HasIndex(e => e.Nombre, "UQ_Roles_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<SolicitudesPedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Solicitu__3214EC07ED6D2DF8");

            entity.ToTable("SolicitudesPedido");

            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaSolicitud).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Observacion).HasMaxLength(500);

            entity.HasOne(d => d.Usuario).WithMany(p => p.SolicitudesPedidos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudesPedido_Usuarios");
        });

        modelBuilder.Entity<SolicitudesProveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Solicitu__3214EC077C431110");

            entity.ToTable("SolicitudesProveedor");

            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaSolicitud).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Mensaje).HasMaxLength(1000);

            entity.HasOne(d => d.Proveedor).WithMany(p => p.SolicitudesProveedors)
                .HasForeignKey(d => d.ProveedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudesProveedor_Proveedor");

            entity.HasOne(d => d.SolicitudPedido).WithMany(p => p.SolicitudesProveedors)
                .HasForeignKey(d => d.SolicitudPedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudesProveedor_SolicitudPedido");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC07D4F04259");

            entity.HasIndex(e => e.Email, "UQ_Usuarios_Email").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Rut).HasMaxLength(20);

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
