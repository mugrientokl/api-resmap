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

        // ==========================================
        // OBTENER TODOS
        // ==========================================

        public async Task<List<Producto>> ObtenerTodos()
        {
            return await _db.Productos.ToListAsync();
        }

        // ==========================================
        // OBTENER POR ID
        // ==========================================

        public async Task<Producto?> ObtenerPorId(int id)
        {
            return await _db.Productos
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // ==========================================
        // CREAR
        // ==========================================

        public async Task<Producto> Crear(Producto producto)
        {
            _db.Productos.Add(producto);

            await _db.SaveChangesAsync();

            return producto;
        }

        // ==========================================
        // ACTUALIZAR
        // ==========================================

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

        // ==========================================
        // ELIMINAR
        // ==========================================

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

        // ==========================================
        // COMPROBAR CÓDIGO
        // ==========================================

        public async Task<bool> ExisteCodigo(
            string codigo,
            int? idExcluir = null)
        {
            var consulta = _db.Productos
                .Where(p => p.Codigo == codigo);

            if (idExcluir.HasValue)
            {
                consulta = consulta
                    .Where(p => p.Id != idExcluir.Value);
            }

            return await consulta.AnyAsync();
        }

        // ==========================================
        // COMPROBAR CATEGORÍA
        // ==========================================

        public async Task<bool> ExisteCategoria(int categoriaId)
        {
            return await _db.Categorias
                .AnyAsync(c => c.Id == categoriaId);
        }
    }
}