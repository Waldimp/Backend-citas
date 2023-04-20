using System.Net;
using ServiceStack;
using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Interfaces;

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
}