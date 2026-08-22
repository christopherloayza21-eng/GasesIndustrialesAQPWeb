namespace GasesIndustriales.Api.Models
{
    public class Zona
    {
        public int IdZona { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
