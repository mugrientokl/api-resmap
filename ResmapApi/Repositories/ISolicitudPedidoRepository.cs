using ResmapApi.Models;

namespace ResmapApi.Repositories
{
    public interface ISolicitudPedidoRepository
    {
        Task<List<SolicitudesPedido>> ObtenerTodos();

        Task<List<SolicitudesPedido>> ObtenerPorUsuario(int usuarioId);

        Task<SolicitudesPedido?> ObtenerPorId(int id);

        Task<SolicitudesPedido> Crear(SolicitudesPedido solicitud);

        Task<bool> ActualizarEstado(int id, string estado);

        Task<bool> ExisteProducto(int productoId);

        Task<Producto?> ObtenerProducto(int productoId);
    }
}