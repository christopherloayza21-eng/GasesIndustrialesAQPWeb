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

        public DbSet<Pedido> Pedidos { get; set; }

        public DbSet<DetallePedido> DetallesPedido { get; set; }

        public DbSet<MovimientoCilindro> MovimientosCilindro { get; set; }

        public DbSet<Proveedor> Proveedores { get; set; }

        public DbSet<EnvioRecarga> EnviosRecarga { get; set; }

        public DbSet<DetalleEnvioRecarga> DetallesEnvioRecarga { get; set; }

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

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("pedido");

                entity.HasKey(e => e.IdPedido);

                entity.Property(e => e.IdPedido)
                    .HasColumnName("id_pedido");

                entity.Property(e => e.IdCliente)
                    .HasColumnName("id_cliente");

                entity.Property(e => e.FechaPedido)
                    .HasColumnName("fecha_pedido");

                entity.Property(e => e.DireccionEntrega)
                    .HasColumnName("direccion_entrega");

                entity.Property(e => e.IdZona)
                    .HasColumnName("id_zona");

                entity.Property(e => e.IdConductor)
                    .HasColumnName("id_conductor");

                entity.Property(e => e.IdVehiculo)
                    .HasColumnName("id_vehiculo");

                entity.Property(e => e.EstadoPedido)
                    .HasColumnName("estado_pedido");

                entity.Property(e => e.Observaciones)
                    .HasColumnName("observaciones");
            });

            modelBuilder.Entity<MovimientoCilindro>(entity =>
            {
                entity.ToTable("movimiento_cilindro");

                entity.HasKey(e => e.IdMovimiento);

                entity.Property(e => e.IdMovimiento)
                    .HasColumnName("id_movimiento");

                entity.Property(e => e.IdCilindro)
                    .HasColumnName("id_cilindro");

                entity.Property(e => e.IdPedido)
                    .HasColumnName("id_pedido");

                entity.Property(e => e.TipoMovimiento)
                    .HasColumnName("tipo_movimiento");

                entity.Property(e => e.FechaMovimiento)
                    .HasColumnName("fecha_movimiento");

                entity.Property(e => e.IdCliente)
                    .HasColumnName("id_cliente");

                entity.Property(e => e.IdConductor)
                    .HasColumnName("id_conductor");

                entity.Property(e => e.IdVehiculo)
                    .HasColumnName("id_vehiculo");

                entity.Property(e => e.Observacion)
                    .HasColumnName("observacion");
            });

            modelBuilder.Entity<DetallePedido>(entity =>
            {
                entity.ToTable("detalle_pedido");

                entity.HasKey(e => e.IdDetalle);

                entity.Property(e => e.IdDetalle)
                    .HasColumnName("id_detalle");

                entity.Property(e => e.IdPedido)
                    .HasColumnName("id_pedido");

                entity.Property(e => e.IdProducto)
                    .HasColumnName("id_producto");

                entity.Property(e => e.Cantidad)
                    .HasColumnName("cantidad");

                entity.Property(e => e.PrecioUnitario)
                    .HasColumnName("precio_unitario");

                entity.Property(e => e.Subtotal)
                    .HasColumnName("subtotal");
            });

            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.ToTable("proveedor");

                entity.HasKey(e => e.IdProveedor);

                entity.Property(e => e.IdProveedor)
                    .HasColumnName("id_proveedor");

                entity.Property(e => e.RazonSocial)
                    .HasColumnName("razon_social");

                entity.Property(e => e.Ruc)
                    .HasColumnName("ruc");

                entity.Property(e => e.Telefono)
                    .HasColumnName("telefono");

                entity.Property(e => e.Direccion)
                    .HasColumnName("direccion");

                entity.Property(e => e.Activo)
                    .HasColumnName("activo");
            });

            modelBuilder.Entity<EnvioRecarga>(entity =>
            {
                entity.ToTable("envio_recarga");

                entity.HasKey(e => e.IdEnvio);

                entity.Property(e => e.IdEnvio)
                    .HasColumnName("id_envio");

                entity.Property(e => e.IdProveedor)
                    .HasColumnName("id_proveedor");

                entity.Property(e => e.FechaEnvio)
                    .HasColumnName("fecha_envio");

                entity.Property(e => e.NumeroGuia)
                    .HasColumnName("numero_guia");

                entity.Property(e => e.Estado)
                    .HasColumnName("estado");

                entity.Property(e => e.Observaciones)
                    .HasColumnName("observaciones");
            });

            modelBuilder.Entity<DetalleEnvioRecarga>(entity =>
            {
                entity.ToTable("detalle_envio_recarga");

                entity.HasKey(e => e.IdDetalleEnvio);

                entity.Property(e => e.IdDetalleEnvio)
                    .HasColumnName("id_detalle_envio");

                entity.Property(e => e.IdEnvio)
                    .HasColumnName("id_envio");

                entity.Property(e => e.IdCilindro)
                    .HasColumnName("id_cilindro");

                entity.Property(e => e.FechaRetorno)
                    .HasColumnName("fecha_retorno");

                entity.Property(e => e.EstadoRetorno)
                    .HasColumnName("estado_retorno");

                entity.Property(e => e.Observacion)
                    .HasColumnName("observacion");
            });
        }
    }
}
