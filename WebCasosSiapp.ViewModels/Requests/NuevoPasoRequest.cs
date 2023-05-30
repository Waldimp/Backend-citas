using WebCasosSiapp.Models.PRO;

namespace WebCasosSiapp.ViewModels.Requests;

public class NuevoPasoRequest
{
    public Caso Caso { get; set; }
    public string? ActividadVersionId { get; set; }
    public string? TipoMovimiento { get; set; }
    public ResponsableRequest Responsable { get; set; }
}