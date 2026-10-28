namespace Lab11.ViewModels;

public class EventoListItemViewModel
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public int Aforo { get; set; }
    public decimal Precio { get; set; }
    public string Modalidad { get; set; } = string.Empty;
    public string CategoriaNombre { get; set; } = string.Empty;
    public int InscritosConfirmados { get; set; }
    public int CuposDisponibles => Aforo - InscritosConfirmados;
}
