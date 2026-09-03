using System;
using System.Collections.Generic;

namespace ResmapApi.Models;

public partial class Proveedore
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<SolicitudesProveedor> SolicitudesProveedors { get; set; } = new List<SolicitudesProveedor>();
}
