using System.Net;
using ServiceStack;
using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO.Views;
using WebCasosSiapp.ViewModels.Responses;

namespace WebCasosSiapp.Concretes;

public class HubDataConcrete : IHubData
{
    private readonly DatabaseContext _ctx;

    public HubDataConcrete(DatabaseContext context)
    {
        _ctx = context;
    }

    /*
     * Function: GetProcessList
     * Get a list of processes versions associates a specific user
     */
    public object GetProcessesVersionsList(string? user)
    {
        // Evaluate if the user is null
        if (user == null) return new HttpError(HttpStatusCode.BadRequest, "No se encontró al usuario");
        
        // Find records per user
        var list = _ctx.VwCasoResumenes?.Where(vc => vc.UsuarioIdResponsable == user).ToList();
        return new HttpResult(list, HttpStatusCode.OK);
    }

    public object GetNewActivitiesList(string? user)
    {
        // Evaluate if the user is null
        if (user == null) return new HttpError(HttpStatusCode.BadRequest, "No se encontró al usuario");
        
        // Find records per user
        var list = _ctx.VwCasoActividadesNuevas?.Where(vc => vc.UsuarioIdResponsable == user)
            .OrderByDescending(vc => vc.FechaEstado).ToList();
        return new HttpResult(list, HttpStatusCode.OK);
    }

    public object GetDetailActivitiesList(string? user, string? version)
    {
        if (user == null || version == null)
            return new HttpError(HttpStatusCode.BadRequest, "No se encontró el usuario o la versión");

        var customOrder = new List<string> {"Nuevo", "Grupo", "En proceso", "Borrador", "En pausa"};

        var list = _ctx.VwCasosTiempoResponsables
            ?.Where(vt =>
                vt.UsuarioIdResponsable == user && vt.VersionProcesoId == version && vt.Estado != "Finalizado")
            .ToList().OrderBy(vt => customOrder.IndexOf(vt.Estado)).ThenByDescending(vt => vt.FechaEstado).ToList();
        
        var listFin = _ctx.VwCasosTiempoResponsables
            ?.Where(vt =>
                vt.UsuarioIdResponsable == user && vt.VersionProcesoId == version && vt.Estado == "Finalizado")
            .ToList().OrderBy(vt => customOrder.IndexOf(vt.Estado)).ThenByDescending(vt => vt.FechaEstado).ToList();

        var nombre = list.Count > 0 ? list[0].NombreProceso : "Proceso";

        var response = new HubConnectionResponse.DetalleVersionResponse
        {
            Nombre = nombre,
            Abiertos = list,
            Cerrados = listFin
        };

        return new HttpResult(response, HttpStatusCode.OK);
    }
}