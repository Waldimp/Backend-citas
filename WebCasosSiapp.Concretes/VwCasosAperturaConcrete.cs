using System.Net;
using ServiceStack;
using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.ViewModels.Responses;

namespace WebCasosSiapp.Concretes;

public class VwCasosAperturaConcrete : IVwCasosApertura
{
    private readonly DatabaseContext _ctx;

    public VwCasosAperturaConcrete(DatabaseContext ctx)
    {
        _ctx = ctx;
    }

    public object Index(string? user)
    {
        try
        {
            if (user == null)
            {
                return new HttpError(HttpStatusCode.BadRequest,"Error con el token del usuario.");
            }

            VwCasosAperturaResponse casosAperturaResponse = new VwCasosAperturaResponse();
            casosAperturaResponse.Fijos = _ctx.VwCasosApertura?.Where(vw => vw.Fijo == true && vw.CodigoUsuario == user).ToList();
            casosAperturaResponse.Procesos = _ctx.VwCasosApertura?.Where(vw => vw.Fijo == false && vw.CodigoUsuario == user).ToList();

            return new HttpResult(casosAperturaResponse, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + ex.Message);
        }
    }
}