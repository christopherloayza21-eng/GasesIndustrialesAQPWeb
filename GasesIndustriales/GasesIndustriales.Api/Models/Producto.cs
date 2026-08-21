namespace GasesIndustriales.Api.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string TipoProducto { get; set; } = "GAS";

        public string UnidadMedida { get; set; } = string.Empty;

        public decimal? PrecioReferencia { get; set; }

        public bool Activo { get; set; }
    }
}
