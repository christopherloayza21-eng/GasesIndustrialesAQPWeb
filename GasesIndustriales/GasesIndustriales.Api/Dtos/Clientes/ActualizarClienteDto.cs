using System.ComponentModel.DataAnnotations;

namespace GasesIndustriales.Api.Dtos.Clientes
{
    public class ActualizarClienteDto
    {
        [Required]
        [MaxLength(150)]
        public string RazonSocial { get; set; } = string.Empty;

        [MaxLength(11)]
        public string? Ruc { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public int? IdZona { get; set; }

        [Required]
        [RegularExpression("NUEVO|FRECUENTE|EVENTUAL")]
        public string TipoCliente { get; set; } = "EVENTUAL";

        public bool RequiereGarantia { get; set; }

        public bool Activo { get; set; } = true;
    }
}
