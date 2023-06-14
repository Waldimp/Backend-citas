using WebCasosSiapp.Models.PRO;
using WebCasosSiapp.ViewModels.Requests;
using WebCasosSiapp.ViewModels.Responses;

namespace WebCasosSiapp.Interfaces;

public interface ICaso
{
    SignalResponse NuevoCaso(NuevoCasoRequest request);
    object ObtenerCaso(string id);
    object FijarProcesoUsuario(string ProcesoId, string UsuarioId);
    object EliminarProcesoFijoUsuario(string ProcesoId, string UsuarioId);
    public Ciclo AsignacionCiclica(string ActividadVersionId);
    object FinalizarPaso(string pasoId, FinalizarPasoRequest request, string UsuarioId);
}