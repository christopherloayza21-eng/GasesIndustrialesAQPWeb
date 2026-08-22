using GasesIndustriales.Api.Data;
using GasesIndustriales.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasesIndustriales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConductoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConductoresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetConductores([FromQuery] bool incluirInactivos = false)
        {
            var query = _context.Conductores.AsNoTracking();

            if (!incluirInactivos)
            {
                query = query.Where(conductor => conductor.Activo);
            }

            return Ok(await query.OrderBy(conductor => conductor.Nombre).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CrearConductor(ConductorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest("El nombre del conductor es obligatorio.");
            }

            var conductor = new Conductor
            {
                Nombre = request.Nombre.Trim(),
                Telefono = Normalizar(request.Telefono),
                Activo = true
            };

            _context.Conductores.Add(conductor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConductores), new { id = conductor.IdConductor }, conductor);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarConductor(int id, ConductorRequest request)
        {
            var conductor = await _context.Conductores.FindAsync(id);

            if (conductor is null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest("El nombre del conductor es obligatorio.");
            }

            conductor.Nombre = request.Nombre.Trim();
            conductor.Telefono = Normalizar(request.Telefono);
            conductor.Activo = request.Activo;

            await _context.SaveChangesAsync();

            return Ok(conductor);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DesactivarConductor(int id)
        {
            var conductor = await _context.Conductores.FindAsync(id);

            if (conductor is null)
            {
                return NotFound();
            }

            conductor.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/reactivar")]
        public async Task<IActionResult> ReactivarConductor(int id)
        {
            var conductor = await _context.Conductores.FindAsync(id);

            if (conductor is null)
            {
                return NotFound();
            }

            conductor.Activo = true;
            await _context.SaveChangesAsync();

            return Ok(conductor);
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }
    }

    public class ConductorRequest
    {
        public string Nombre { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;
    }
}
