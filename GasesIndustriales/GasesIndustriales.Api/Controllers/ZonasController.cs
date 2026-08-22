using GasesIndustriales.Api.Data;
using GasesIndustriales.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasesIndustriales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZonasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ZonasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetZonas([FromQuery] bool incluirInactivos = false)
        {
            var query = _context.Zonas.AsNoTracking();

            if (!incluirInactivos)
            {
                query = query.Where(zona => zona.Activo);
            }

            return Ok(await query.OrderBy(zona => zona.Nombre).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CrearZona(ZonaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest("El nombre de la zona es obligatorio.");
            }

            var zona = new Zona
            {
                Nombre = request.Nombre.Trim(),
                Descripcion = Normalizar(request.Descripcion),
                Activo = true
            };

            _context.Zonas.Add(zona);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetZonas), new { id = zona.IdZona }, zona);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarZona(int id, ZonaRequest request)
        {
            var zona = await _context.Zonas.FindAsync(id);

            if (zona is null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest("El nombre de la zona es obligatorio.");
            }

            zona.Nombre = request.Nombre.Trim();
            zona.Descripcion = Normalizar(request.Descripcion);
            zona.Activo = request.Activo;

            await _context.SaveChangesAsync();

            return Ok(zona);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DesactivarZona(int id)
        {
            var zona = await _context.Zonas.FindAsync(id);

            if (zona is null)
            {
                return NotFound();
            }

            zona.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/reactivar")]
        public async Task<IActionResult> ReactivarZona(int id)
        {
            var zona = await _context.Zonas.FindAsync(id);

            if (zona is null)
            {
                return NotFound();
            }

            zona.Activo = true;
            await _context.SaveChangesAsync();

            return Ok(zona);
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }
    }

    public class ZonaRequest
    {
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
