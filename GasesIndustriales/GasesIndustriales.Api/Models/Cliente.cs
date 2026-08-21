namespace GasesIndustriales.Api.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }

        public string RazonSocial { get; set; } = string.Empty;

        public string? Ruc { get; set; }

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public int? IdZona { get; set; }

        public string TipoCliente { get; set; } = "EVENTUAL";

        public bool RequiereGarantia { get; set; }

        public bool Activo { get; set; }
    }
}