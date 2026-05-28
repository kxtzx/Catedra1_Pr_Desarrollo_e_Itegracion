using System;
using System.Collections.Generic;

namespace SistemaMantencion.Models;

public partial class Mantencione
{
    public int Id { get; set; }

    public int VehiculoId { get; set; }

    public DateOnly FechaMantencion { get; set; }

    public string RutMecanico { get; set; } = null!;

    public decimal HorasTrabajadas { get; set; }

    public virtual Vehiculo Vehiculo { get; set; } = null!;
}
