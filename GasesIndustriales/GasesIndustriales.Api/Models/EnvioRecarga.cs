namespace GasesIndustriales.Api.Models
{
    public class EnvioRecarga
    {
        public int IdEnvio { get; set; }

        public int IdProveedor { get; set; }

        public DateTime FechaEnvio { get; set; }

        public string? NumeroGuia { get; set; }

        public string Estado { get; set; } = "ENVIADO";

        public string? Observaciones { get; set; }
    }
}
