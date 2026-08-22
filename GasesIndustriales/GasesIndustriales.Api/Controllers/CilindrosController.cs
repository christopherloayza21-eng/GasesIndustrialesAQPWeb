using GasesIndustriales.Api.Data;
using GasesIndustriales.Api.Dtos.Cilindros;
using GasesIndustriales.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasesIndustriales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CilindrosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CilindrosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCilindros([FromQuery] bool incluirInactivos = false)
        {
            var query = _context.Cilindros.AsNoTracking();

            if (!incluirInactivos)
            {
                query = query.Where(cilindro => cilindro.Activo);
            }

            var cilindros = await (
                from cilindro in query
                join producto in _context.Productos.AsNoTracking()
                    on cilindro.IdProducto equals producto.IdProducto
                orderby cilindro.CodigoCilindro
                select new CilindroResponseDto
                {
                    IdCilindro = cilindro.IdCilindro,
                    CodigoCilindro = cilindro.CodigoCilindro,
                    IdProducto = cilindro.IdProducto,
                    Producto = producto.Nombre,
                    Capacidad = cilindro.Capacidad,
                    PropietarioTipo = cilindro.PropietarioTipo,
                    IdClientePropietario = cilindro.IdClientePropietario,
                    EstadoActual = cilindro.EstadoActual,
                    UbicacionActual = cilindro.UbicacionActual,
                    FechaUltimoMovimiento = cilindro.FechaUltimoMovimiento,
                    Activo = cilindro.Activo
                })
                .ToListAsync();

            return Ok(cilindros);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCilindroPorId(int id)
        {
            var cilindro = await (
                from item in _context.Cilindros.AsNoTracking()
                join producto in _context.Productos.AsNoTracking()
                    on item.IdProducto equals producto.IdProducto
                where item.IdCilindro == id && item.Activo
                select new CilindroResponseDto
                {
                    IdCilindro = item.IdCilindro,
                    CodigoCilindro = item.CodigoCilindro,
                    IdProducto = item.IdProducto,
                    Producto = producto.Nombre,
                    Capacidad = item.Capacidad,
                    PropietarioTipo = item.PropietarioTipo,
                    IdClientePropietario = item.IdClientePropietario,
                    EstadoActual = item.EstadoActual,
                    UbicacionActual = item.UbicacionActual,
                    FechaUltimoMovimiento = item.FechaUltimoMovimiento,
                    Activo = item.Activo
                })
                .FirstOrDefaultAsync();

            if (cilindro is null)
            {
                return NotFound();
            }

            return Ok(cilindro);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCilindro(CilindroRequest request)
        {
            var validacion = await ValidarCilindro(request);

            if (validacion is not null)
            {
                return validacion;
            }

            var codigo = request.CodigoCilindro.Trim().ToUpperInvariant();

            if (await _context.Cilindros.AnyAsync(cilindro => cilindro.Activo && cilindro.CodigoCilindro == codigo))
            {
                return Conflict("Ya existe un cilindro activo con ese código.");
            }

            var cilindro = new Cilindro
            {
                CodigoCilindro = codigo,
                IdProducto = request.IdProducto,
                Capacidad = request.Capacidad,
                PropietarioTipo = request.PropietarioTipo.Trim().ToUpperInvariant(),
                IdClientePropietario = request.IdClientePropietario,
                EstadoActual = NormalizarEstado(request.EstadoActual),
                UbicacionActual = Normalizar(request.UbicacionActual) ?? "ALMACEN",
                FechaUltimoMovimiento = DateTime.Now,
                Activo = true
            };

            _context.Cilindros.Add(cilindro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCilindroPorId), new { id = cilindro.IdCilindro }, cilindro);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCilindro(int id, CilindroRequest request)
        {
            var cilindro = await _context.Cilindros.FindAsync(id);

            if (cilindro is null)
            {
                return NotFound();
            }

            var validacion = await ValidarCilindro(request);

            if (validacion is not null)
            {
                return validacion;
            }

            var codigo = request.CodigoCilindro.Trim().ToUpperInvariant();

            var existeCodigo = await _context.Cilindros.AnyAsync(item =>
                item.Activo
                && item.CodigoCilindro == codigo
                && item.IdCilindro != id);

            if (existeCodigo)
            {
                return Conflict("Ya existe otro cilindro activo con ese código.");
            }

            cilindro.CodigoCilindro = codigo;
            cilindro.IdProducto = request.IdProducto;
            cilindro.Capacidad = request.Capacidad;
            cilindro.PropietarioTipo = request.PropietarioTipo.Trim().ToUpperInvariant();
            cilindro.IdClientePropietario = request.IdClientePropietario;
            cilindro.EstadoActual = NormalizarEstado(request.EstadoActual);
            cilindro.UbicacionActual = Normalizar(request.UbicacionActual) ?? cilindro.UbicacionActual;
            cilindro.Activo = request.Activo;

            await _context.SaveChangesAsync();

            return Ok(cilindro);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DesactivarCilindro(int id)
        {
            var cilindro = await _context.Cilindros.FindAsync(id);

            if (cilindro is null)
            {
                return NotFound();
            }

            cilindro.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/reactivar")]
        public async Task<IActionResult> ReactivarCilindro(int id)
        {
            var cilindro = await _context.Cilindros.FindAsync(id);

            if (cilindro is null)
            {
                return NotFound();
            }

            cilindro.Activo = true;
            await _context.SaveChangesAsync();

            return Ok(cilindro);
        }

        [HttpGet("{id:int}/movimientos")]
        public async Task<IActionResult> GetHistorial(int id)
        {
            var existe = await _context.Cilindros.AnyAsync(cilindro => cilindro.IdCilindro == id);

            if (!existe)
            {
                return NotFound();
            }

            var movimientos = await (
                from movimiento in _context.MovimientosCilindro.AsNoTracking()
                join cliente in _context.Clientes.AsNoTracking()
                    on movimiento.IdCliente equals cliente.IdCliente into clientesJoin
                from cliente in clientesJoin.DefaultIfEmpty()
                where movimiento.IdCilindro == id
                orderby movimiento.FechaMovimiento descending
                select new
                {
                    movimiento.IdMovimiento,
                    movimiento.IdCilindro,
                    movimiento.IdPedido,
                    movimiento.TipoMovimiento,
                    movimiento.FechaMovimiento,
                    movimiento.IdCliente,
                    Cliente = cliente != null ? cliente.RazonSocial : null,
                    movimiento.IdConductor,
                    movimiento.IdVehiculo,
                    movimiento.Observacion
                })
                .ToListAsync();

            return Ok(movimientos);
        }

        private async Task<IActionResult?> ValidarCilindro(CilindroRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CodigoCilindro))
            {
                return BadRequest("El código del cilindro es obligatorio.");
            }

            if (request.Capacidad is <= 0)
            {
                return BadRequest("La capacidad debe ser mayor a cero.");
            }

            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.IdProducto == request.IdProducto && item.Activo);

            if (producto is null)
            {
                return BadRequest("El producto/gas indicado no existe o está inactivo.");
            }

            if (producto.TipoProducto != "GAS")
            {
                return BadRequest("Un cilindro debe relacionarse con un producto de tipo GAS.");
            }

            var propietarioTipo = request.PropietarioTipo.Trim().ToUpperInvariant();

            if (propietarioTipo is not "EMPRESA" and not "CLIENTE")
            {
                return BadRequest("El propietario debe ser EMPRESA o CLIENTE.");
            }

            if (propietarioTipo == "CLIENTE" && request.IdClientePropietario is null)
            {
                return BadRequest("Si el cilindro pertenece a un cliente, debes indicar el cliente propietario.");
            }

            return null;
        }

        private static string NormalizarEstado(string? estado)
        {
            return string.IsNullOrWhiteSpace(estado) ? "LLENO_ALMACEN" : estado.Trim().ToUpperInvariant();
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }
    }

    public class CilindroRequest
    {
        public string CodigoCilindro { get; set; } = string.Empty;

        public int IdProducto { get; set; }

        public decimal? Capacidad { get; set; }

        public string PropietarioTipo { get; set; } = "EMPRESA";

        public int? IdClientePropietario { get; set; }

        public string EstadoActual { get; set; } = "LLENO_ALMACEN";

        public string? UbicacionActual { get; set; }

        public bool Activo { get; set; } = true;
    }
}
