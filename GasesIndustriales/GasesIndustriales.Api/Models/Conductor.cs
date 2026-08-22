namespace GasesIndustriales.Api.Models
{
    public class Conductor
    {
        public int IdConductor { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public bool Activo { get; set; }
    }
}
