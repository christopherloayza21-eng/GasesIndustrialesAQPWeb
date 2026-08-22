using GasesIndustriales.Api.Data;
using GasesIndustriales.Api.Dtos.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasesIndustriales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var cilindrosDisponibles = await _context.Cilindros
                .AsNoTracking()
                .CountAsync(cilindro =>
                    cilindro.Activo
                    && (cilindro.EstadoActual == "LLENO_ALMACEN" || cilindro.EstadoActual == "VACIO_ALMACEN"));

            var cilindrosEnClientes = await _context.Cilindros
                .AsNoTracking()
                .CountAsync(cilindro => cilindro.Activo && cilindro.EstadoActual == "EN_CLIENTE");

            var cilindrosEnProveedor = await _context.Cilindros
                .AsNoTracking()
                .CountAsync(cilindro =>
                    cilindro.Activo
                    && (cilindro.EstadoActual == "EN_PROVEEDOR" || cilindro.EstadoActual == "EN_RECARGA"));

            var pedidosPendientes = await _context.Pedidos
                .AsNoTracking()
                .CountAsync(pedido => pedido.EstadoPedido == "PENDIENTE" || pedido.EstadoPedido == "ASIGNADO");

            var movimientosRecientes = await (
                from movimiento in _context.MovimientosCilindro.AsNoTracking()
                join cilindro in _context.Cilindros.AsNoTracking()
                    on movimiento.IdCilindro equals cilindro.IdCilindro
                join producto in _context.Productos.AsNoTracking()
                    on cilindro.IdProducto equals producto.IdProducto
                join cliente in _context.Clientes.AsNoTracking()
                    on movimiento.IdCliente equals cliente.IdCliente into clientes
                from cliente in clientes.DefaultIfEmpty()
                orderby movimiento.FechaMovimiento descending
                select new MovimientoRecienteDto
                {
                    IdMovimiento = movimiento.IdMovimiento,
                    CodigoCilindro = cilindro.CodigoCilindro,
                    Producto = producto.Nombre,
                    TipoMovimiento = movimiento.TipoMovimiento,
                    FechaMovimiento = movimiento.FechaMovimiento,
                    Cliente = cliente != null ? cliente.RazonSocial : null,
                    Observacion = movimiento.Observacion
                })
                .Take(10)
                .ToListAsync();

            var resumen = new DashboardResumenDto
            {
                CilindrosDisponibles = cilindrosDisponibles,
                CilindrosEnClientes = cilindrosEnClientes,
                CilindrosEnProveedor = cilindrosEnProveedor,
                PedidosPendientes = pedidosPendientes,
                MovimientosRecientes = movimientosRecientes
            };

            return Ok(resumen);
        }
    }
}
