namespace GasesIndustriales.Api.Models
{
    public class MovimientoCilindro
    {
        public int IdMovimiento { get; set; }

        public int IdCilindro { get; set; }

        public int? IdPedido { get; set; }

        public string TipoMovimiento { get; set; } = string.Empty;

        public DateTime FechaMovimiento { get; set; }

        public int? IdCliente { get; set; }

        public int? IdConductor { get; set; }

        public int? IdVehiculo { get; set; }

        public string? Observacion { get; set; }
    }
}
