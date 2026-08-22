namespace GasesIndustriales.Api.Models
{
    public class DetalleEnvioRecarga
    {
        public int IdDetalleEnvio { get; set; }

        public int IdEnvio { get; set; }

        public int IdCilindro { get; set; }

        public DateTime? FechaRetorno { get; set; }

        public string EstadoRetorno { get; set; } = "PENDIENTE";

        public string? Observacion { get; set; }
    }
}
