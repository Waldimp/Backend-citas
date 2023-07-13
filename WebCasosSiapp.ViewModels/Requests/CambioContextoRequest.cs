namespace WebCasosSiapp.ViewModels.Requests;

public class CambioContextoRequest
{
    public string Tipo { get; set; }
    public string PasoActualId { get; set; }
    public string? PasoDestinoId { get; set; }
    public string? Estado { get; set; }
    public string? ResponsableId { get; set; }
    public string? Justificacion { get; set; }
}