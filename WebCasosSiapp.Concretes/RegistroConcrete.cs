using System.Net;
using ServiceStack;
using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Concretes.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO;
using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Concretes;

public class RegistroConcrete : IRegistro
{
    private readonly DatabaseContext _ctx;

    public RegistroConcrete(DatabaseContext ctx)
    {
        _ctx = ctx;
    }

    public object Create(NuevoRegistroRequest datos)
    {
        try
        {
            var registro = new Registro
            {
                Id = Generals.GetUlid(),
                PasoId = datos.PasoId,
                RegistroId = datos.RegistroId,
                SeccionId = datos.SeccionId
            };

            _ctx.Registro?.Add(registro);
            if (_ctx.SaveChanges() != 1)
                return new HttpError(HttpStatusCode.BadRequest, "Error no se pudo guardar registro");

            var secciones = _ctx.Secciones?.Where(s =>
                s.ActividadVersionId == _ctx.Secciones.Where(s2 => s2.Id == datos.SeccionId)
                .Select(s2 => s2.ActividadVersionId).Single()).OrderBy(s => s.Orden).ToList();
            _ctx.Registro?.Where(r => r.PasoId == datos.PasoId).ToList();
            return new HttpResult(secciones, HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + e.Message);
        }
    }
}