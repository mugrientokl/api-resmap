using Microsoft.EntityFrameworkCore;
using ResmapApi.Models;

namespace ResmapApi.Repositories
{
    public class SolicitudProveedorRepository
        : ISolicitudProveedorRepository
    {
        private readonly ResmapdbContext _db;

        public SolicitudProveedorRepository(
            ResmapdbContext db)
        {
            _db = db;
        }

        public async Task<List<SolicitudesProveedor>>
            ObtenerTodos()
        {
            return await _db.SolicitudesProveedors
                .Include(s => s.Proveedor)
                .Include(s => s.SolicitudPedido)
                .ToListAsync();
        }

        public async Task<SolicitudesProveedor?>
            ObtenerPorId(int id)
        {
            return await _db.SolicitudesProveedors
                .Include(s => s.Proveedor)
                .Include(s => s.SolicitudPedido)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SolicitudesProveedor> Crear(
            SolicitudesProveedor solicitud)
        {
            _db.SolicitudesProveedors.Add(solicitud);

            await _db.SaveChangesAsync();

            return solicitud;
        }

        public async Task<bool> ActualizarEstado(
            int id,
            string estado)
        {
            var solicitud =
                await _db.SolicitudesProveedors
                    .FindAsync(id);

            if (solicitud == null)
                return false;

            solicitud.Estado = estado;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteProveedor(
            int proveedorId)
        {
            return await _db.Proveedores
                .AnyAsync(p => p.Id == proveedorId);
        }

        public async Task<bool> ExisteSolicitudPedido(
            int solicitudPedidoId)
        {
            return await _db.SolicitudesPedidos
                .AnyAsync(s => s.Id == solicitudPedidoId);
        }
    }
}