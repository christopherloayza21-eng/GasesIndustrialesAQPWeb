using GasesIndustriales.Api.Data;
using GasesIndustriales.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasesIndustriales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovimientosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovimientos()
        {
            var movimientos = await (
                from movimiento in _context.MovimientosCilindro.AsNoTracking()
                join cilindro in _context.Cilindros.AsNoTracking()
                    on movimiento.IdCilindro equals cilindro.IdCilindro
                join producto in _context.Productos.AsNoTracking()
                    on cilindro.IdProducto equals producto.IdProducto
                join cliente in _context.Clientes.AsNoTracking()
                    on movimiento.IdCliente equals cliente.IdCliente into clientesJoin
                from cliente in clientesJoin.DefaultIfEmpty()
                orderby movimiento.FechaMovimiento descending
                select new
                {
                    movimiento.IdMovimiento,
                    movimiento.IdCilindro,
                    cilindro.CodigoCilindro,
                    Producto = producto.Nombre,
                    movimiento.IdPedido,
                    movimiento.TipoMovimiento,
                    movimiento.FechaMovimiento,
                    movimiento.IdCliente,
                    Cliente = cliente != null ? cliente.RazonSocial : null,
                    movimiento.IdConductor,
                    movimiento.IdVehiculo,
                    movimiento.Observacion
                })
                .Take(100)
                .ToListAsync();

            return Ok(movimientos);
        }

        [HttpPost("salida-cliente")]
        public async Task<IActionResult> RegistrarSalidaCliente(MovimientoRequest request)
        {
            if (request.IdCliente is null)
            {
                return BadRequest("Para una salida a cliente debes indicar el cliente.");
            }

            if (!await _context.Clientes.AnyAsync(cliente => cliente.IdCliente == request.IdCliente && cliente.Activo))
            {
                return BadRequest("El cliente no existe o está inactivo.");
            }

            return await RegistrarMovimiento(
                request,
                "SALIDA_CLIENTE",
                estadosPermitidos: new[] { "LLENO_ALMACEN" },
                nuevoEstado: "EN_CLIENTE",
                nuevaUbicacion: await ObtenerNombreCliente(request.IdCliente.Value));
        }

        [HttpPost("retorno-cliente")]
        public async Task<IActionResult> RegistrarRetornoCliente(MovimientoRequest request)
        {
            return await RegistrarMovimiento(
                request,
                "RETORNO_CLIENTE",
                estadosPermitidos: new[] { "EN_CLIENTE" },
                nuevoEstado: "VACIO_ALMACEN",
                nuevaUbicacion: "ALMACEN VACIOS");
        }

        [HttpPost("envio-proveedor")]
        public async Task<IActionResult> RegistrarEnvioProveedor(MovimientoRequest request)
        {
            return await RegistrarMovimiento(
                request,
                "ENVIO_RECARGA",
                estadosPermitidos: new[] { "VACIO_ALMACEN" },
                nuevoEstado: "EN_PROVEEDOR",
                nuevaUbicacion: "PROVEEDOR");
        }

        [HttpPost("retorno-recarga")]
        public async Task<IActionResult> RegistrarRetornoRecarga(MovimientoRequest request)
        {
            return await RegistrarMovimiento(
                request,
                "RETORNO_RECARGA",
                estadosPermitidos: new[] { "EN_PROVEEDOR", "EN_RECARGA" },
                nuevoEstado: "LLENO_ALMACEN",
                nuevaUbicacion: "ALMACEN LLENOS");
        }

        private async Task<IActionResult> RegistrarMovimiento(
            MovimientoRequest request,
            string tipoMovimiento,
            string[] estadosPermitidos,
            string nuevoEstado,
            string nuevaUbicacion)
        {
            var cilindro = await _context.Cilindros.FindAsync(request.IdCilindro);

            if (cilindro is null || !cilindro.Activo)
            {
                return NotFound("El cilindro no existe o está inactivo.");
            }

            if (!estadosPermitidos.Contains(cilindro.EstadoActual))
            {
                return BadRequest($"No se puede registrar {tipoMovimiento}. Estado actual: {cilindro.EstadoActual}.");
            }

            if (request.IdConductor.HasValue && !await _context.Conductores.AnyAsync(item => item.IdConductor == request.IdConductor && item.Activo))
            {
                return BadRequest("El conductor no existe o está inactivo.");
            }

            if (request.IdVehiculo.HasValue && !await _context.Vehiculos.AnyAsync(item => item.IdVehiculo == request.IdVehiculo && item.Activo))
            {
                return BadRequest("El vehículo no existe o está inactivo.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            cilindro.EstadoActual = nuevoEstado;
            cilindro.UbicacionActual = nuevaUbicacion;
            cilindro.FechaUltimoMovimiento = DateTime.Now;

            var movimiento = new MovimientoCilindro
            {
                IdCilindro = request.IdCilindro,
                IdPedido = request.IdPedido,
                TipoMovimiento = tipoMovimiento,
                FechaMovimiento = DateTime.Now,
                IdCliente = request.IdCliente,
                IdConductor = request.IdConductor,
                IdVehiculo = request.IdVehiculo,
                Observacion = Normalizar(request.Observacion)
            };

            _context.MovimientosCilindro.Add(movimiento);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetMovimientos), new { id = movimiento.IdMovimiento }, movimiento);
        }

        private async Task<string> ObtenerNombreCliente(int idCliente)
        {
            var cliente = await _context.Clientes
                .AsNoTracking()
                .Where(item => item.IdCliente == idCliente)
                .Select(item => item.RazonSocial)
                .FirstAsync();

            return cliente;
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }
    }

    public class MovimientoRequest
    {
        public int IdCilindro { get; set; }

        public int? IdPedido { get; set; }

        public int? IdCliente { get; set; }

        public int? IdConductor { get; set; }

        public int? IdVehiculo { get; set; }

        public string? Observacion { get; set; }
    }
}
