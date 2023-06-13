using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Interfaces;

public interface IObservacion
{
    object Create(NuevoObservacionRequest datos, string user);
    object Index(string pasoId);
}