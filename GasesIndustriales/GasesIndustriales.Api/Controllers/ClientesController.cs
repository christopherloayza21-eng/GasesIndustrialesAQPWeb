using GasesIndustriales.Api.Data;
using GasesIndustriales.Api.Dtos.Clientes;
using GasesIndustriales.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasesIndustriales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetClientes([FromQuery] string? buscar, [FromQuery] bool incluirInactivos = false)
        {
            var query = _context.Clientes.AsNoTracking();

            if (!incluirInactivos)
            {
                query = query.Where(cliente => cliente.Activo);
            }

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var termino = buscar.Trim().ToLower();

                query = query.Where(cliente =>
                    cliente.RazonSocial.ToLower().Contains(termino)
                    || (cliente.Ruc != null && cliente.Ruc.Contains(termino)));
            }

            var clientes = await query
                .OrderBy(cliente => cliente.RazonSocial)
                .Select(cliente => ToResponseDto(cliente))
                .ToListAsync();

            return Ok(clientes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetClientePorId(int id)
        {
            var cliente = await _context.Clientes
                .AsNoTracking()
                .Where(cliente => cliente.IdCliente == id && cliente.Activo)
                .Select(cliente => ToResponseDto(cliente))
                .FirstOrDefaultAsync();

            if (cliente is null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCliente(CrearClienteDto request)
        {
            var existeRuc = await ExisteRucActivo(request.Ruc);

            if (existeRuc)
            {
                return Conflict("Ya existe un cliente activo con ese RUC.");
            }

            var cliente = new Cliente
            {
                RazonSocial = request.RazonSocial.Trim(),
                Ruc = NormalizarTextoOpcional(request.Ruc),
                Telefono = NormalizarTextoOpcional(request.Telefono),
                Direccion = NormalizarTextoOpcional(request.Direccion),
                IdZona = request.IdZona,
                TipoCliente = request.TipoCliente,
                RequiereGarantia = request.RequiereGarantia,
                Activo = true
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            var response = ToResponseDto(cliente);

            return CreatedAtAction(nameof(GetClientePorId), new { id = cliente.IdCliente }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCliente(int id, ActualizarClienteDto request)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente is null)
            {
                return NotFound();
            }

            var existeRuc = await ExisteRucActivo(request.Ruc, id);

            if (existeRuc)
            {
                return Conflict("Ya existe otro cliente activo con ese RUC.");
            }

            cliente.RazonSocial = request.RazonSocial.Trim();
            cliente.Ruc = NormalizarTextoOpcional(request.Ruc);
            cliente.Telefono = NormalizarTextoOpcional(request.Telefono);
            cliente.Direccion = NormalizarTextoOpcional(request.Direccion);
            cliente.IdZona = request.IdZona;
            cliente.TipoCliente = request.TipoCliente;
            cliente.RequiereGarantia = request.RequiereGarantia;
            cliente.Activo = request.Activo;

            await _context.SaveChangesAsync();

            return Ok(ToResponseDto(cliente));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DesactivarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente is null)
            {
                return NotFound();
            }

            if (!cliente.Activo)
            {
                return NoContent();
            }

            cliente.Activo = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/reactivar")]
        public async Task<IActionResult> ReactivarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente is null)
            {
                return NotFound();
            }

            if (cliente.Activo)
            {
                return Ok(ToResponseDto(cliente));
            }

            cliente.Activo = true;

            await _context.SaveChangesAsync();

            return Ok(ToResponseDto(cliente));
        }

        private async Task<bool> ExisteRucActivo(string? ruc, int? idClienteIgnorado = null)
        {
            if (string.IsNullOrWhiteSpace(ruc))
            {
                return false;
            }

            var rucNormalizado = ruc.Trim();

            return await _context.Clientes.AnyAsync(cliente =>
                cliente.Activo
                && cliente.Ruc == rucNormalizado
                && (!idClienteIgnorado.HasValue || cliente.IdCliente != idClienteIgnorado.Value));
        }

        private static string? NormalizarTextoOpcional(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }

        private static ClienteResponseDto ToResponseDto(Cliente cliente)
        {
            return new ClienteResponseDto
            {
                IdCliente = cliente.IdCliente,
                RazonSocial = cliente.RazonSocial,
                Ruc = cliente.Ruc,
                Telefono = cliente.Telefono,
                Direccion = cliente.Direccion,
                IdZona = cliente.IdZona,
                TipoCliente = cliente.TipoCliente,
                RequiereGarantia = cliente.RequiereGarantia,
                Activo = cliente.Activo
            };
        }
    }
}
