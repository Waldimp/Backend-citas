using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Interfaces;

public interface ICaso
{
    object NuevoCaso(NuevoCasoRequest request);
    object ObtenerCaso(string id);
}