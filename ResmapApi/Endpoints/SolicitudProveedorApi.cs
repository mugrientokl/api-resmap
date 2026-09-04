using ResmapApi.DTOs;
using ResmapApi.Models;
using ResmapApi.Repositories;

namespace ResmapApi.Endpoints
{
    public static class SolicitudProveedorApi
    {
        public static void MapSolicitudProveedorApi(
            this WebApplication app)
        {
            var grupo = app
                .MapGroup("/api/solicitudes-proveedor")
                .WithTags("Solicitudes a Proveedores")
                .RequireAuthorization("Administrador");

            // OBTENER TODAS
            grupo.MapGet("/", async (
                ISolicitudProveedorRepository repository) =>
            {
                var solicitudes =
                    await repository.ObtenerTodos();

                return Results.Ok(solicitudes);
            });

            // OBTENER POR ID
            grupo.MapGet("/{id:int}", async (
                int id,
                ISolicitudProveedorRepository repository) =>
            {
                var solicitud =
                    await repository.ObtenerPorId(id);

                if (solicitud == null)
                {
                    return Results.NotFound(new
                    {
                        mensaje =
                            "Solicitud a proveedor no encontrada",
                        id
                    });
                }

                return Results.Ok(solicitud);
            });

            // CREAR SOLICITUD A PROVEEDOR
            grupo.MapPost("/", async (
                SolicitudProveedorCrearDto solicitudDto,
                ISolicitudProveedorRepository repository) =>
            {
                if (solicitudDto.ProveedorId <= 0)
                {
                    return Results.BadRequest(new
                    {
                        mensaje =
                            "El proveedor es obligatorio"
                    });
                }

                if (solicitudDto.SolicitudPedidoId <= 0)
                {
                    return Results.BadRequest(new
                    {
                        mensaje =
                            "La solicitud de pedido es obligatoria"
                    });
                }

                if (string.IsNullOrWhiteSpace(
                    solicitudDto.Mensaje))
                {
                    return Results.BadRequest(new
                    {
                        mensaje =
                            "El mensaje es obligatorio"
                    });
                }

                if (!await repository.ExisteProveedor(
                    solicitudDto.ProveedorId))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "El proveedor no existe",
                        proveedorId =
                            solicitudDto.ProveedorId
                    });
                }

                if (!await repository.ExisteSolicitudPedido(
                    solicitudDto.SolicitudPedidoId))
                {
                    return Results.BadRequest(new
                    {
                        mensaje =
                            "La solicitud de pedido no existe",
                        solicitudPedidoId =
                            solicitudDto.SolicitudPedidoId
                    });
                }

                var solicitud = new SolicitudesProveedor
                {
                    ProveedorId =
                        solicitudDto.ProveedorId,

                    SolicitudPedidoId =
                        solicitudDto.SolicitudPedidoId,

                    FechaSolicitud = DateTime.Now,

                    Mensaje = solicitudDto.Mensaje,

                    Estado = "Pendiente"
                };

                var nuevaSolicitud =
                    await repository.Crear(solicitud);

                return Results.Created(
                    $"/api/solicitudes-proveedor/{nuevaSolicitud.Id}",
                    new
                    {
                        mensaje =
                            "Solicitud al proveedor creada correctamente",
                        id = nuevaSolicitud.Id,
                        estado = nuevaSolicitud.Estado
                    });
            });

            // CAMBIAR ESTADO
            grupo.MapPut("/{id:int}/estado", async (
                int id,
                SolicitudProveedorEstadoDto estadoDto,
                ISolicitudProveedorRepository repository) =>
            {
                var estadosValidos = new[]
                {
                    "Pendiente",
                    "Enviada",
                    "Respondida",
                    "Aceptada",
                    "Rechazada"
                };

                if (!estadosValidos.Contains(
                    estadoDto.Estado))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "Estado no válido",
                        estadosPermitidos =
                            estadosValidos
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
                            "Solicitud a proveedor no encontrada",
                        id
                    });
                }

                return Results.NoContent();
            });
        }
    }
}