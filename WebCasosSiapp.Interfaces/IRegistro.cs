using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Interfaces;

public interface IRegistro
{
    object Create(NuevoRegistroRequest datos);
}