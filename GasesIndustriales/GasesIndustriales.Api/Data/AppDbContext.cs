using GasesIndustriales.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GasesIndustriales.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("cliente");

                entity.HasKey(e => e.IdCliente);

                entity.Property(e => e.IdCliente)
                    .HasColumnName("id_cliente");

                entity.Property(e => e.RazonSocial)
                    .HasColumnName("razon_social");

                entity.Property(e => e.Ruc)
                    .HasColumnName("ruc");

                entity.Property(e => e.Telefono)
                    .HasColumnName("telefono");

                entity.Property(e => e.Direccion)
                    .HasColumnName("direccion");

                entity.Property(e => e.IdZona)
                    .HasColumnName("id_zona");

                entity.Property(e => e.TipoCliente)
                    .HasColumnName("tipo_cliente");

                entity.Property(e => e.RequiereGarantia)
                    .HasColumnName("requiere_garantia");

                entity.Property(e => e.Activo)
                    .HasColumnName("activo");
            });
        }
    }
}