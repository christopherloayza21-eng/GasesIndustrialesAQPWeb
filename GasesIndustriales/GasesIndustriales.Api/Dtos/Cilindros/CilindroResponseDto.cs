namespace GasesIndustriales.Api.Dtos.Cilindros
{
    public class CilindroResponseDto
    {
        public int IdCilindro { get; set; }

        public string CodigoCilindro { get; set; } = string.Empty;

        public int IdProducto { get; set; }

        public string Producto { get; set; } = string.Empty;

        public decimal? Capacidad { get; set; }

        public string PropietarioTipo { get; set; } = string.Empty;

        public int? IdClientePropietario { get; set; }

        public string EstadoActual { get; set; } = string.Empty;

        public string? UbicacionActual { get; set; }

        public DateTime? FechaUltimoMovimiento { get; set; }

        public bool Activo { get; set; }
    }
}
