using ResmapApi.Models;

namespace ResmapApi.Repositories
{
    public interface ISolicitudProveedorRepository
    {
        Task<List<SolicitudesProveedor>> ObtenerTodos();

        Task<SolicitudesProveedor?> ObtenerPorId(int id);

        Task<SolicitudesProveedor> Crear(
            SolicitudesProveedor solicitud);

        Task<bool> ActualizarEstado(
            int id,
            string estado);

        Task<bool> ExisteProveedor(int proveedorId);

        Task<bool> ExisteSolicitudPedido(
            int solicitudPedidoId);
    }
}