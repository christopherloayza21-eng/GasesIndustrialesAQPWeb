namespace GasesIndustriales.Api.Dtos.Dashboard
{
    public class DashboardResumenDto
    {
        public int CilindrosDisponibles { get; set; }

        public int CilindrosEnClientes { get; set; }

        public int CilindrosEnProveedor { get; set; }

        public int PedidosPendientes { get; set; }

        public List<MovimientoRecienteDto> MovimientosRecientes { get; set; } = [];
    }
}
