using WebCasosSiapp.Models.PRO.Views;

namespace WebCasosSiapp.ViewModels.Responses;

public class HubConnectionResponse
{
    public class DetalleVersionResponse
    {
        public string Nombre { get; set; }
        public List<VwCasoTiempoRes> Abiertos { get; set; }
    }
}