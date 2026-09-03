using Microsoft.AspNetCore.Mvc;
using ResmapApi.Models;
using ResmapApi.Repositories;

namespace ResmapApi.Endpoints
{
    public static class ProductoApi
    {
        public static void MapProductoApi(this WebApplication app)
        {
            var grupo = app
                .MapGroup("/api/productos")
                .WithTags("Productos");

            // ==========================================
            // GET - Obtener todos
            // ==========================================

            grupo.MapGet("/", async (IProductoRepository repository) =>
            {
                var productos = await repository.ObtenerTodos();

                return Results.Ok(productos);
            });

            // ==========================================
            // GET - Obtener por ID
            // ==========================================

            grupo.MapGet("/{id:int}", async (
                int id,
                IProductoRepository repository) =>
            {
                var producto = await repository.ObtenerPorId(id);

                if (producto == null)
                {
                    return Results.NotFound(new
                    {
                        mensaje = "Producto no encontrado"
                    });
                }

                return Results.Ok(producto);
            });

            // ==========================================
            // POST - Crear
            // ==========================================

            grupo.MapPost("/", async (
                Producto producto,
                IProductoRepository repository) =>
            {
                var nuevoProducto = await repository.Crear(producto);

                return Results.Created(
                    $"/api/productos/{nuevoProducto.Id}",
                    nuevoProducto
                );
            });

            // ==========================================
            // PUT - Actualizar
            // ==========================================

            grupo.MapPut("/{id:int}", async (
                int id,
                Producto producto,
                IProductoRepository repository) =>
            {
                var actualizado = await repository.Actualizar(
                    id,
                    producto
                );

                if (!actualizado)
                {
                    return Results.NotFound(new
                    {
                        mensaje = "Producto no encontrado"
                    });
                }

                return Results.NoContent();
            });

            // ==========================================
            // DELETE - Eliminar
            // ==========================================

            grupo.MapDelete("/{id:int}", async (
                int id,
                IProductoRepository repository) =>
            {
                var eliminado = await repository.Eliminar(id);

                if (!eliminado)
                {
                    return Results.NotFound(new
                    {
                        mensaje = "Producto no encontrado"
                    });
                }

                return Results.NoContent();
            });
        }
    }
}