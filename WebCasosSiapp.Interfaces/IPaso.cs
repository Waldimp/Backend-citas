using WebCasosSiapp.ViewModels.Responses;

namespace WebCasosSiapp.Interfaces;

public interface IPaso
{
    SignalResponse MarcarPasoLeido(string PasoId, string? user);
    SignalResponse AutoasignarPaso(string PasoId, string? user);
    object DatosDePaso(string PasoId);
}