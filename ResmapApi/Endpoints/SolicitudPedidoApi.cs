using System.Security.Claims;
using ResmapApi.DTOs;
using ResmapApi.Models;
using ResmapApi.Repositories;

namespace ResmapApi.Endpoints
{
    public static class SolicitudPedidoApi
    {
        public static void MapSolicitudPedidoApi(
            this WebApplication app)
        {
            var grupo = app
                .MapGroup("/api/v1/solicitudes")
                .WithTags("Solicitudes de Pedido")
                .RequireAuthorization();

            // CLIENTE:
            // Crear una nueva solicitud
            grupo.MapPost("/", async (
                SolicitudPedidoCrearDto solicitudDto,
                ClaimsPrincipal usuario,
                ISolicitudPedidoRepository repository) =>
            {
                var usuarioIdTexto =
                    usuario.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (!int.TryParse(
                    usuarioIdTexto,
                    out int usuarioId))
                {
                    return Results.Unauthorized();
                }

                if (solicitudDto.Productos == null ||
                    solicitudDto.Productos.Count == 0)
                {
                    return Results.BadRequest(new
                    {
                        mensaje =
                            "La solicitud debe contener al menos un producto"
                    });
                }

                var solicitud = new SolicitudesPedido
                {
                    UsuarioId = usuarioId,
                    Estado = "Pendiente",
                    Observacion = solicitudDto.Observacion,
                    FechaSolicitud = DateTime.Now
                };

                foreach (var item in solicitudDto.Productos)
                {
                    if (item.Cantidad <= 0)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje =
                                "La cantidad debe ser mayor que cero",
                            productoId = item.ProductoId
                        });
                    }

                    var producto =
                        await repository.ObtenerProducto(
                            item.ProductoId);

                    if (producto == null)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "El producto no existe",
                            productoId = item.ProductoId
                        });
                    }

                    if (item.Cantidad > producto.Stock)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje =
                                "La cantidad solicitada supera el stock disponible",
                            productoId = item.ProductoId,
                            stockDisponible = producto.Stock
                        });
                    }

                    var detalle = new DetalleSolicitud
                    {
                        ProductoId = producto.Id,
                        Cantidad = item.Cantidad,
                        PrecioReferencial = producto.Precio
                    };

                    solicitud.DetalleSolicituds.Add(detalle);
                }

                var nuevaSolicitud =
                    await repository.Crear(solicitud);

                return Results.Created(
                    $"/api/v1/solicitudes/{nuevaSolicitud.Id}",
                    new
                    {
                        mensaje =
                            "Solicitud creada correctamente",
                        id = nuevaSolicitud.Id,
                        estado = nuevaSolicitud.Estado
                    });
            });

            // CLIENTE:
            // Ver sus propias solicitudes
            grupo.MapGet("/mis-solicitudes", async (
                ClaimsPrincipal usuario,
                ISolicitudPedidoRepository repository) =>
            {
                var usuarioIdTexto =
                    usuario.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (!int.TryParse(
                    usuarioIdTexto,
                    out int usuarioId))
                {
                    return Results.Unauthorized();
                }

                var solicitudes =
                    await repository.ObtenerPorUsuario(
                        usuarioId);

                return Results.Ok(solicitudes);
            });

            // ADMINISTRADOR:
            // Ver todas las solicitudes
            grupo.MapGet("/", async (
                ISolicitudPedidoRepository repository) =>
            {
                var solicitudes =
                    await repository.ObtenerTodos();

                return Results.Ok(solicitudes);
            })
            .RequireAuthorization("Administrador");

            // USUARIO AUTENTICADO:
            // Obtener una solicitud específica
            grupo.MapGet("/{id:int}", async (
                int id,
                ClaimsPrincipal usuario,
                ISolicitudPedidoRepository repository) =>
            {
                var solicitud =
                    await repository.ObtenerPorId(id);

                if (solicitud == null)
                {
                    return Results.NotFound(new
                    {
                        mensaje =
                            "Solicitud no encontrada",
                        id
                    });
                }

                var esAdministrador =
                    usuario.IsInRole("Administrador");

                var usuarioIdTexto =
                    usuario.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                int.TryParse(
                    usuarioIdTexto,
                    out int usuarioId);

                if (!esAdministrador &&
                    solicitud.UsuarioId != usuarioId)
                {
                    return Results.Forbid();
                }

                return Results.Ok(solicitud);
            });

            // ADMINISTRADOR:
            // Cambiar estado de una solicitud
            grupo.MapPut("/{id:int}/estado", async (
                int id,
                SolicitudEstadoDto estadoDto,
                ISolicitudPedidoRepository repository) =>
            {
                var estadosValidos = new[]
                {
                    "Pendiente",
                    "EnRevision",
                    "Aprobada",
                    "Rechazada",
                    "Completada",
                    "Cancelada"
                };

                if (!estadosValidos.Contains(
                    estadoDto.Estado))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "Estado no válido",
                        estadosPermitidos = estadosValidos
                    });
                }

                var actualizado =
                    await repository.ActualizarEstado(
                        id,
                        estadoDto.Estado);

                if (!actualizado)
                {
                    return Results.NotFound(new
                    {
                        mensaje =
                            "Solicitud no encontrada",
                        id
                    });
                }

                return Results.NoContent();
            })
            .RequireAuthorization("Administrador");
        }
    }
}