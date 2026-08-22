using GasesIndustriales.Api.Data;
using GasesIndustriales.Api.Dtos.Productos;
using GasesIndustriales.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasesIndustriales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductos([FromQuery] string? tipo, [FromQuery] bool incluirInactivos = false)
        {
            var query = _context.Productos.AsNoTracking();

            if (!incluirInactivos)
            {
                query = query.Where(producto => producto.Activo);
            }

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                var tipoNormalizado = tipo.Trim().ToUpperInvariant();
                query = query.Where(producto => producto.TipoProducto == tipoNormalizado);
            }

            var productos = await query
                .OrderBy(producto => producto.Nombre)
                .Select(producto => ToResponseDto(producto))
                .ToListAsync();

            return Ok(productos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductoPorId(int id)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .Where(producto => producto.IdProducto == id && producto.Activo)
                .Select(producto => ToResponseDto(producto))
                .FirstOrDefaultAsync();

            if (producto is null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto(CrearProductoDto request)
        {
            var codigo = request.Codigo.Trim().ToUpperInvariant();
            var existeCodigo = await ExisteCodigoActivo(codigo);

            if (existeCodigo)
            {
                return Conflict("Ya existe un producto activo con ese código.");
            }

            var producto = new Producto
            {
                Codigo = codigo,
                Nombre = request.Nombre.Trim(),
                TipoProducto = request.TipoProducto.Trim().ToUpperInvariant(),
                UnidadMedida = request.UnidadMedida.Trim().ToUpperInvariant(),
                PrecioReferencia = request.PrecioReferencia,
                Activo = true
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var response = ToResponseDto(producto);

            return CreatedAtAction(nameof(GetProductoPorId), new { id = producto.IdProducto }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarProducto(int id, ActualizarProductoDto request)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto is null)
            {
                return NotFound();
            }

            var codigo = request.Codigo.Trim().ToUpperInvariant();
            var existeCodigo = await ExisteCodigoActivo(codigo, id);

            if (existeCodigo)
            {
                return Conflict("Ya existe otro producto activo con ese código.");
            }

            producto.Codigo = codigo;
            producto.Nombre = request.Nombre.Trim();
            producto.TipoProducto = request.TipoProducto.Trim().ToUpperInvariant();
            producto.UnidadMedida = request.UnidadMedida.Trim().ToUpperInvariant();
            producto.PrecioReferencia = request.PrecioReferencia;
            producto.Activo = request.Activo;

            await _context.SaveChangesAsync();

            return Ok(ToResponseDto(producto));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DesactivarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto is null)
            {
                return NotFound();
            }

            if (!producto.Activo)
            {
                return NoContent();
            }

            producto.Activo = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/reactivar")]
        public async Task<IActionResult> ReactivarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto is null)
            {
                return NotFound();
            }

            producto.Activo = true;

            await _context.SaveChangesAsync();

            return Ok(ToResponseDto(producto));
        }

        private async Task<bool> ExisteCodigoActivo(string codigo, int? idProductoIgnorado = null)
        {
            return await _context.Productos.AnyAsync(producto =>
                producto.Activo
                && producto.Codigo == codigo
                && (!idProductoIgnorado.HasValue || producto.IdProducto != idProductoIgnorado.Value));
        }

        private static ProductoResponseDto ToResponseDto(Producto producto)
        {
            return new ProductoResponseDto
            {
                IdProducto = producto.IdProducto,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                TipoProducto = producto.TipoProducto,
                UnidadMedida = producto.UnidadMedida,
                PrecioReferencia = producto.PrecioReferencia,
                Activo = producto.Activo
            };
        }
    }
}
