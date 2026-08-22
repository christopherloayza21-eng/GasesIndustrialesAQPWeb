namespace GasesIndustriales.Api.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }

        public int IdCliente { get; set; }

        public DateTime FechaPedido { get; set; }

        public string? DireccionEntrega { get; set; }

        public int? IdZona { get; set; }

        public int? IdConductor { get; set; }

        public int? IdVehiculo { get; set; }

        public string EstadoPedido { get; set; } = "PENDIENTE";

        public string? Observaciones { get; set; }
    }
}
