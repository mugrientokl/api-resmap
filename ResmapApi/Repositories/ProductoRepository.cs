using Microsoft.EntityFrameworkCore;
using ResmapApi.Models;

namespace ResmapApi.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ResmapdbContext _db;

        public ProductoRepository(ResmapdbContext db)
        {
            _db = db;
        }

        public async Task<List<Producto>> ObtenerTodos()
        {
            return await _db.Productos
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<Producto?> ObtenerPorId(int id)
        {
            return await _db.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Producto> Crear(Producto producto)
        {
            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();

            return producto;
        }

        public async Task<bool> Actualizar(int id, Producto producto)
        {
            var productoExistente = await _db.Productos
                .FindAsync(id);

            if (productoExistente == null)
                return false;

            productoExistente.Codigo = producto.Codigo;
            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.Marca = producto.Marca;
            productoExistente.Precio = producto.Precio;
            productoExistente.Stock = producto.Stock;
            productoExistente.CategoriaId = producto.CategoriaId;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var producto = await _db.Productos
                .FindAsync(id);

            if (producto == null)
                return false;

            _db.Productos.Remove(producto);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}