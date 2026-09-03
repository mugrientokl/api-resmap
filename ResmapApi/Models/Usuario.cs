using System;
using System.Collections.Generic;

namespace ResmapApi.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Rut { get; set; }

    public int RolId { get; set; }

    public virtual Role Rol { get; set; } = null!;

    public virtual ICollection<SolicitudesPedido> SolicitudesPedidos { get; set; } = new List<SolicitudesPedido>();
}
