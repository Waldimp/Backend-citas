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

            Caso caso = _ctx.Caso.First(c => c.Id == _ctx.Paso.Where(p => p.Id == datos.PasoId).Select(p => p.CasoId).Single());
            // Obtener todos los pasos y a partir de ellos obtener los registros de todos los pasos
            List<Paso> pasosCaso = _ctx.Paso.Where(p => p.CasoId == caso.Id).ToList();
            List<Registro> registros = new List<Registro>();
            foreach (var reg in pasosCaso.Select(pasoC => _ctx.Registro.Where(r => r.PasoId == pasoC.Id).ToList()))
            {
                registros.AddRange(reg);
            }
            
            return new HttpResult(registros, HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + e.Message);
        }
    }
}