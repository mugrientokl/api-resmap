using System;
using System.Collections.Generic;

namespace ResmapApi.Models;

public partial class SolicitudesProveedor
{
    public int Id { get; set; }

    public int ProveedorId { get; set; }

    public int SolicitudPedidoId { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public string Mensaje { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual Proveedore Proveedor { get; set; } = null!;

    public virtual SolicitudesPedido SolicitudPedido { get; set; } = null!;
}
