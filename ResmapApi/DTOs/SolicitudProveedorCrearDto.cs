namespace ResmapApi.DTOs
{
    public class SolicitudProveedorCrearDto
    {
        public int ProveedorId { get; set; }

        public int SolicitudPedidoId { get; set; }

        public string Mensaje { get; set; } = string.Empty;
    }
}