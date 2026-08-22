using GasesIndustriales.Api.Data;
using GasesIndustriales.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasesIndustriales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VehiculosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetVehiculos([FromQuery] bool incluirInactivos = false)
        {
            var query = _context.Vehiculos.AsNoTracking();

            if (!incluirInactivos)
            {
                query = query.Where(vehiculo => vehiculo.Activo);
            }

            return Ok(await query.OrderBy(vehiculo => vehiculo.Placa).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CrearVehiculo(VehiculoRequest request)
        {
            var placa = NormalizarPlaca(request.Placa);

            if (string.IsNullOrWhiteSpace(placa))
            {
                return BadRequest("La placa es obligatoria.");
            }

            if (await ExistePlacaActiva(placa))
            {
                return Conflict("Ya existe un vehículo activo con esa placa.");
            }

            var vehiculo = new Vehiculo
            {
                Placa = placa,
                Descripcion = Normalizar(request.Descripcion),
                Activo = true
            };

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehiculos), new { id = vehiculo.IdVehiculo }, vehiculo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarVehiculo(int id, VehiculoRequest request)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);

            if (vehiculo is null)
            {
                return NotFound();
            }

            var placa = NormalizarPlaca(request.Placa);

            if (string.IsNullOrWhiteSpace(placa))
            {
                return BadRequest("La placa es obligatoria.");
            }

            if (await ExistePlacaActiva(placa, id))
            {
                return Conflict("Ya existe otro vehículo activo con esa placa.");
            }

            vehiculo.Placa = placa;
            vehiculo.Descripcion = Normalizar(request.Descripcion);
            vehiculo.Activo = request.Activo;

            await _context.SaveChangesAsync();

            return Ok(vehiculo);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DesactivarVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);

            if (vehiculo is null)
            {
                return NotFound();
            }

            vehiculo.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/reactivar")]
        public async Task<IActionResult> ReactivarVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);

            if (vehiculo is null)
            {
                return NotFound();
            }

            vehiculo.Activo = true;
            await _context.SaveChangesAsync();

            return Ok(vehiculo);
        }

        private async Task<bool> ExistePlacaActiva(string placa, int? idVehiculoIgnorado = null)
        {
            return await _context.Vehiculos.AnyAsync(vehiculo =>
                vehiculo.Activo
                && vehiculo.Placa == placa
                && (!idVehiculoIgnorado.HasValue || vehiculo.IdVehiculo != idVehiculoIgnorado.Value));
        }

        private static string NormalizarPlaca(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? string.Empty : valor.Trim().ToUpperInvariant();
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }
    }

    public class VehiculoRequest
    {
        public string Placa { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
