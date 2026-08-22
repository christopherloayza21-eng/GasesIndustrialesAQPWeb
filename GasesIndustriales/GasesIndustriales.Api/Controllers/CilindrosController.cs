using GasesIndustriales.Api.Data;
using GasesIndustriales.Api.Dtos.Cilindros;
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
        public async Task<IActionResult> GetCilindros()
        {
            var cilindros = await (
                from cilindro in _context.Cilindros.AsNoTracking()
                join producto in _context.Productos.AsNoTracking()
                    on cilindro.IdProducto equals producto.IdProducto
                where cilindro.Activo
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
    }
}
