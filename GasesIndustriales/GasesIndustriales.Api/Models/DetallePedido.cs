namespace GasesIndustriales.Api.Models
{
    public class DetallePedido
    {
        public int IdDetalle { get; set; }

        public int IdPedido { get; set; }

        public int IdProducto { get; set; }

        public decimal Cantidad { get; set; }

        public decimal? PrecioUnitario { get; set; }

        public decimal? Subtotal { get; set; }
    }
}
