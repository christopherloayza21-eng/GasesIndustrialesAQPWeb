using System.ComponentModel.DataAnnotations;

namespace GasesIndustriales.Api.Dtos.Productos
{
    public class CrearProductoDto
    {
        [Required]
        [MaxLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [RegularExpression("GAS|EQUIPO|INSUMO|SERVICIO")]
        public string TipoProducto { get; set; } = "GAS";

        [Required]
        [MaxLength(10)]
        public string UnidadMedida { get; set; } = string.Empty;

        [Range(0, 99999999.99)]
        public decimal? PrecioReferencia { get; set; }
    }
}
