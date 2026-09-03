using System;
using System.Collections.Generic;

namespace ResmapApi.Models;

public partial class DetalleSolicitud
{
    public int Id { get; set; }

    public int SolicitudPedidoId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioReferencial { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual SolicitudesPedido SolicitudPedido { get; set; } = null!;
}
