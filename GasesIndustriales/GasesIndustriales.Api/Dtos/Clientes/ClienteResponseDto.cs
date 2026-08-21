namespace GasesIndustriales.Api.Dtos.Clientes
{
    public class ClienteResponseDto
    {
        public int IdCliente { get; set; }

        public string RazonSocial { get; set; } = string.Empty;

        public string? Ruc { get; set; }

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public int? IdZona { get; set; }

        public string TipoCliente { get; set; } = string.Empty;

        public bool RequiereGarantia { get; set; }

        public bool Activo { get; set; }
    }
}
