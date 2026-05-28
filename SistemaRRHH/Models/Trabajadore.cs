using System;
using System.Collections.Generic;

namespace SistemaRRHH.Models;

public partial class Trabajadore
{
    public int Id { get; set; }

    public string Rut { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public decimal SueldoBase { get; set; }

    public decimal ValorHora { get; set; }
}
