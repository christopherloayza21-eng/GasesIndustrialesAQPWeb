namespace GasesIndustriales.Api.Dtos.Dashboard
{
    public class MovimientoRecienteDto
    {
        public int IdMovimiento { get; set; }

        public string CodigoCilindro { get; set; } = string.Empty;

        public string Producto { get; set; } = string.Empty;

        public string TipoMovimiento { get; set; } = string.Empty;

        public DateTime FechaMovimiento { get; set; }

        public string? Cliente { get; set; }

        public string? Observacion { get; set; }
    }
}
