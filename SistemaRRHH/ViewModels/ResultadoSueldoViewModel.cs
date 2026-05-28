namespace SistemaRRHH.ViewModels
{
    public class ResultadoSueldoViewModel
    {
        public string Rut { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal HorasTrabajadas { get; set; }
        public decimal SueldoBase { get; set; }
        public decimal ValorHora { get; set; }
        public decimal SueldoFinal { get; set; }
    }
}
