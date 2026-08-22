namespace GasesIndustriales.Api.Models
{
    public class Proveedor
    {
        public int IdProveedor { get; set; }

        public string RazonSocial { get; set; } = string.Empty;

        public string? Ruc { get; set; }

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public bool Activo { get; set; }
    }
}
