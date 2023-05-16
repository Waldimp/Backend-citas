using WebCasosSiapp.Models.PRO.Views;

namespace WebCasosSiapp.ViewModels.Responses;

public class VwCasosAperturaResponse
{
    public List<Vw_CasosApertura> Fijos { get; set; } 
    public List<Vw_CasosApertura> Procesos { get; set; } 
}