using System;
using System.Collections.Generic;

namespace SistemaArriendos.Models;

public partial class Arriendo
{
    public int Id { get; set; }

    public int VehiculoId { get; set; }

    public int ClienteId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal PrecioDiario { get; set; }

    public decimal PrecioTotal { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Vehiculo Vehiculo { get; set; } = null!;
}
