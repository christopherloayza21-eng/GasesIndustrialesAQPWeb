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

        public DbSet<Producto> Productos { get; set; }

        public DbSet<Zona> Zonas { get; set; }

        public DbSet<Cilindro> Cilindros { get; set; }

        public DbSet<Conductor> Conductores { get; set; }

        public DbSet<Vehiculo> Vehiculos { get; set; }

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

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("producto");

                entity.HasKey(e => e.IdProducto);

                entity.Property(e => e.IdProducto)
                    .HasColumnName("id_producto");

                entity.Property(e => e.Codigo)
                    .HasColumnName("codigo");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre");

                entity.Property(e => e.TipoProducto)
                    .HasColumnName("tipo_producto");

                entity.Property(e => e.UnidadMedida)
                    .HasColumnName("unidad_medida");

                entity.Property(e => e.PrecioReferencia)
                    .HasColumnName("precio_referencia");

                entity.Property(e => e.Activo)
                    .HasColumnName("activo");
            });

            modelBuilder.Entity<Zona>(entity =>
            {
                entity.ToTable("zona");

                entity.HasKey(e => e.IdZona);

                entity.Property(e => e.IdZona)
                    .HasColumnName("id_zona");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre");

                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Activo)
                    .HasColumnName("activo");
            });

            modelBuilder.Entity<Cilindro>(entity =>
            {
                entity.ToTable("cilindro");

                entity.HasKey(e => e.IdCilindro);

                entity.Property(e => e.IdCilindro)
                    .HasColumnName("id_cilindro");

                entity.Property(e => e.CodigoCilindro)
                    .HasColumnName("codigo_cilindro");

                entity.Property(e => e.IdProducto)
                    .HasColumnName("id_producto");

                entity.Property(e => e.Capacidad)
                    .HasColumnName("capacidad");

                entity.Property(e => e.PropietarioTipo)
                    .HasColumnName("propietario_tipo");

                entity.Property(e => e.IdClientePropietario)
                    .HasColumnName("id_cliente_propietario");

                entity.Property(e => e.EstadoActual)
                    .HasColumnName("estado_actual");

                entity.Property(e => e.UbicacionActual)
                    .HasColumnName("ubicacion_actual");

                entity.Property(e => e.FechaUltimoMovimiento)
                    .HasColumnName("fecha_ultimo_movimiento");

                entity.Property(e => e.Activo)
                    .HasColumnName("activo");
            });

            modelBuilder.Entity<Conductor>(entity =>
            {
                entity.ToTable("conductor");

                entity.HasKey(e => e.IdConductor);

                entity.Property(e => e.IdConductor)
                    .HasColumnName("id_conductor");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre");

                entity.Property(e => e.Telefono)
                    .HasColumnName("telefono");

                entity.Property(e => e.Activo)
                    .HasColumnName("activo");
            });

            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.ToTable("vehiculo");

                entity.HasKey(e => e.IdVehiculo);

                entity.Property(e => e.IdVehiculo)
                    .HasColumnName("id_vehiculo");

                entity.Property(e => e.Placa)
                    .HasColumnName("placa");

                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Activo)
                    .HasColumnName("activo");
            });
        }
    }
}
