using GasesIndustriales.Api.Data;
using GasesIndustriales.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasesIndustriales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProveedores([FromQuery] bool incluirInactivos = false)
        {
            var query = _context.Proveedores.AsNoTracking();

            if (!incluirInactivos)
            {
                query = query.Where(proveedor => proveedor.Activo);
            }

            return Ok(await query.OrderBy(proveedor => proveedor.RazonSocial).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CrearProveedor(ProveedorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RazonSocial))
            {
                return BadRequest("La razón social es obligatoria.");
            }

            var proveedor = new Proveedor
            {
                RazonSocial = request.RazonSocial.Trim(),
                Ruc = Normalizar(request.Ruc),
                Telefono = Normalizar(request.Telefono),
                Direccion = Normalizar(request.Direccion),
                Activo = true
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProveedores), new { id = proveedor.IdProveedor }, proveedor);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarProveedor(int id, ProveedorRequest request)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor is null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.RazonSocial))
            {
                return BadRequest("La razón social es obligatoria.");
            }

            proveedor.RazonSocial = request.RazonSocial.Trim();
            proveedor.Ruc = Normalizar(request.Ruc);
            proveedor.Telefono = Normalizar(request.Telefono);
            proveedor.Direccion = Normalizar(request.Direccion);
            proveedor.Activo = request.Activo;

            await _context.SaveChangesAsync();

            return Ok(proveedor);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DesactivarProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor is null)
            {
                return NotFound();
            }

            proveedor.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/reactivar")]
        public async Task<IActionResult> ReactivarProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor is null)
            {
                return NotFound();
            }

            proveedor.Activo = true;
            await _context.SaveChangesAsync();

            return Ok(proveedor);
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }
    }

    public class ProveedorRequest
    {
        public string RazonSocial { get; set; } = string.Empty;

        public string? Ruc { get; set; }

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
