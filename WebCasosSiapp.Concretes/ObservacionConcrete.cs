using System.Net;
using ServiceStack;
using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Concretes.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO;
using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Concretes;

public class ObservacionConcrete: IObservacion
{
    private readonly DatabaseContext _ctx;

    public ObservacionConcrete(DatabaseContext ctx)
    {
        _ctx = ctx;
    }
    
    public object Create(NuevoObservacionRequest datos, string user)
    {
        try
        {
            var observacion = new Observaciones
            {
                Id = Generals.GetUlid(),
                Observacion = datos.Observacion,
                PasoId = datos.PasoId,
                FechaCreacion = DateTime.Now,
                CreadoPor = user
            };

            _ctx.Observaciones?.Add(observacion);
            if (_ctx.SaveChanges() != 1)
                return new HttpError(HttpStatusCode.BadRequest, "Error no se pudo guardar registro");

            return new HttpResult(observacion, HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + e.Message);
        }
    }
    
    public object Index(string pasoId)
    {
        try
        {
            List<Observaciones> observaciones = _ctx.Observaciones.Where(o => o.PasoId == pasoId)
                .OrderByDescending(o => o.FechaCreacion).ToList();

            return new HttpResult(observaciones, HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + e.Message);
        }
    }
}