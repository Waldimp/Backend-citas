using WebCasosSiapp.Models.PRO;

namespace WebCasosSiapp.ViewModels.Responses;

public class CasoResponse
{
    public Caso Caso { get; set; }
    public ProcesoResumen? Proceso { get; set; }
    public List<ClienteResumen>? Clientes { get; set; }
    public Finalizacion? Finalizacion { get; set; }
}