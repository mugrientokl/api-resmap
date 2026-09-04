namespace ResmapApi.DTOs
{
    public class SolicitudPedidoCrearDto
    {
        public string? Observacion { get; set; }

        public List<DetalleSolicitudDto> Productos { get; set; }
            = new();
    }
}