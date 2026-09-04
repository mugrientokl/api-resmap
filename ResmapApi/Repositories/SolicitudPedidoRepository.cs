using Microsoft.EntityFrameworkCore;
using ResmapApi.Models;

namespace ResmapApi.Repositories
{
    public class SolicitudPedidoRepository : ISolicitudPedidoRepository
    {
        private readonly ResmapdbContext _db;

        public SolicitudPedidoRepository(ResmapdbContext db)
        {
            _db = db;
        }

        public async Task<List<SolicitudesPedido>> ObtenerTodos()
        {
            return await _db.SolicitudesPedidos
                .Include(s => s.Usuario)
                .Include(s => s.DetalleSolicituds)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();
        }

        public async Task<List<SolicitudesPedido>> ObtenerPorUsuario(
            int usuarioId)
        {
            return await _db.SolicitudesPedidos
                .Include(s => s.DetalleSolicituds)
                    .ThenInclude(d => d.Producto)
                .Where(s => s.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<SolicitudesPedido?> ObtenerPorId(int id)
        {
            return await _db.SolicitudesPedidos
                .Include(s => s.Usuario)
                .Include(s => s.DetalleSolicituds)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SolicitudesPedido> Crear(
            SolicitudesPedido solicitud)
        {
            _db.SolicitudesPedidos.Add(solicitud);

            await _db.SaveChangesAsync();

            return solicitud;
        }

        public async Task<bool> ActualizarEstado(
            int id,
            string estado)
        {
            var solicitud =
                await _db.SolicitudesPedidos.FindAsync(id);

            if (solicitud == null)
                return false;

            solicitud.Estado = estado;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteProducto(int productoId)
        {
            return await _db.Productos
                .AnyAsync(p => p.Id == productoId);
        }

        public async Task<Producto?> ObtenerProducto(int productoId)
        {
            return await _db.Productos
                .FirstOrDefaultAsync(p => p.Id == productoId);
        }
    }
}