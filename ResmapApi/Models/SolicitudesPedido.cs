using System;
using System.Collections.Generic;

namespace ResmapApi.Models;

public partial class SolicitudesPedido
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public string Estado { get; set; } = null!;

    public string? Observacion { get; set; }

    public virtual ICollection<DetalleSolicitud> DetalleSolicituds { get; set; } = new List<DetalleSolicitud>();

    public virtual ICollection<SolicitudesProveedor> SolicitudesProveedors { get; set; } = new List<SolicitudesProveedor>();

    public virtual Usuario Usuario { get; set; } = null!;
}
