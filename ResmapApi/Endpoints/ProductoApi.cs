using ResmapApi.DTOs;
using ResmapApi.Models;
using ResmapApi.Repositories;

namespace ResmapApi.Endpoints
{
    public static class ProductoApi
    {
        public static void MapProductoApi(this WebApplication app)
        {
            var grupo = app
                .MapGroup("/api/v1/productos")
                .WithTags("Productos")
                .RequireAuthorization();

            // ==========================================
            // GET - OBTENER TODOS
            // ==========================================

            grupo.MapGet("/", async (IProductoRepository repository) =>
            {
                var productos = await repository.ObtenerTodos();

                var resultado = productos.Select(p => new ProductoRespuestaDto
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Marca = p.Marca,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    CategoriaId = p.CategoriaId
                });

                return Results.Ok(resultado);
            });

            // ==========================================
            // GET - OBTENER POR ID
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
                        mensaje = "Producto no encontrado",
                        id = id
                    });
                }

                var resultado = new ProductoRespuestaDto
                {
                    Id = producto.Id,
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Marca = producto.Marca,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    CategoriaId = producto.CategoriaId
                };

                return Results.Ok(resultado);
            });

            // ==========================================
            // POST - CREAR
            // ==========================================

            grupo.MapPost("/", async (
                ProductoCrearDto producto,
                IProductoRepository repository) =>
            {
                // Validar código
                if (string.IsNullOrWhiteSpace(producto.Codigo))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El código del producto es obligatorio"
                    });
                }

                // Validar nombre
                if (string.IsNullOrWhiteSpace(producto.Nombre))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El nombre del producto es obligatorio"
                    });
                }

                // Validar precio
                if (producto.Precio < 0)
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El precio no puede ser negativo"
                    });
                }

                // Validar stock
                if (producto.Stock < 0)
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El stock no puede ser negativo"
                    });
                }

                // Validar categoría
                if (!await repository.ExisteCategoria(producto.CategoriaId))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "La categoría indicada no existe",
                        categoriaId = producto.CategoriaId
                    });
                }

                // Validar código duplicado
                if (await repository.ExisteCodigo(producto.Codigo))
                {
                    return Results.Conflict(new
                    {
                        mensaje = "Ya existe un producto con ese código",
                        codigo = producto.Codigo
                    });
                }

                var nuevoProducto = new Producto
                {
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Marca = producto.Marca,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    CategoriaId = producto.CategoriaId
                };

                nuevoProducto = await repository.Crear(nuevoProducto);

                var resultado = new ProductoRespuestaDto
                {
                    Id = nuevoProducto.Id,
                    Codigo = nuevoProducto.Codigo,
                    Nombre = nuevoProducto.Nombre,
                    Descripcion = nuevoProducto.Descripcion,
                    Marca = nuevoProducto.Marca,
                    Precio = nuevoProducto.Precio,
                    Stock = nuevoProducto.Stock,
                    CategoriaId = nuevoProducto.CategoriaId
                };

                return Results.Created(
                    $"/api/v1/productos/{resultado.Id}",
                    resultado
                );
            }).RequireAuthorization("Administrador");

            // ==========================================
            // PUT - ACTUALIZAR
            // ==========================================

            grupo.MapPut("/{id:int}", async (
                int id,
                ProductoActualizarDto producto,
                IProductoRepository repository) =>
            {
                // Validar existencia
                var productoExistente =
                    await repository.ObtenerPorId(id);

                if (productoExistente == null)
                {
                    return Results.NotFound(new
                    {
                        mensaje = "Producto no encontrado",
                        id = id
                    });
                }

                // Validar código
                if (string.IsNullOrWhiteSpace(producto.Codigo))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El código del producto es obligatorio"
                    });
                }

                // Validar nombre
                if (string.IsNullOrWhiteSpace(producto.Nombre))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El nombre del producto es obligatorio"
                    });
                }

                // Validar precio
                if (producto.Precio < 0)
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El precio no puede ser negativo"
                    });
                }

                // Validar stock
                if (producto.Stock < 0)
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El stock no puede ser negativo"
                    });
                }

                // Validar categoría
                if (!await repository.ExisteCategoria(producto.CategoriaId))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "La categoría indicada no existe",
                        categoriaId = producto.CategoriaId
                    });
                }

                // Validar código duplicado
                if (await repository.ExisteCodigo(
                    producto.Codigo,
                    id))
                {
                    return Results.Conflict(new
                    {
                        mensaje = "Ya existe otro producto con ese código",
                        codigo = producto.Codigo
                    });
                }

                var productoActualizado = new Producto
                {
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Marca = producto.Marca,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    CategoriaId = producto.CategoriaId
                };

                await repository.Actualizar(
                    id,
                    productoActualizado
                );

                return Results.NoContent();
            }).RequireAuthorization("Administrador");

            // ==========================================
            // DELETE - ELIMINAR
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
                        mensaje = "Producto no encontrado",
                        id = id
                    });
                }

                return Results.NoContent();
            }).RequireAuthorization("Administrador");
        }
    }
}