using ResmapApi.Models;

namespace ResmapApi.Repositories
{
    public interface IProductoRepository
    {
        Task<List<Producto>> ObtenerTodos();
        Task<Producto?> ObtenerPorId(int id);
        Task<Producto> Crear(Producto producto);
        Task<bool> Actualizar(int id, Producto producto);
        Task<bool> Eliminar(int id);
    }
}