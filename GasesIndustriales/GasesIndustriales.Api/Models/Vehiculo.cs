namespace GasesIndustriales.Api.Models
{
    public class Vehiculo
    {
        public int IdVehiculo { get; set; }

        public string Placa { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
