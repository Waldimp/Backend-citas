namespace WebCasosSiapp.ViewModels.Requests;

public class FinalizarPasoRequest
{
    public string? RelacionId { get; set; }
    public string? ResponsableId { get; set; }
    public string? Estado { get; set; }
    public string? Justificacion { get; set; }
}