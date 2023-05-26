namespace WebCasosSiapp.ViewModels.Requests;

public class NuevoCasoRequest
{
    public string ProcesoId { get; set; }
    public string? ComentarioApertura { get; set; }
    public ResponsableRequest Responsable { get; set; }
    public List<int>? Clientes { get; set; }
}