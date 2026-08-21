namespace GasesIndustriales.Api.Dtos.Productos
{
    public class ProductoResponseDto
    {
        public int IdProducto { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string TipoProducto { get; set; } = string.Empty;

        public string UnidadMedida { get; set; } = string.Empty;

        public decimal? PrecioReferencia { get; set; }

        public bool Activo { get; set; }
    }
}
