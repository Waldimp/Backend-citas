using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCasosSiapp.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ObservacionController : Controller
{
    private readonly IObservacion _observacion;

    public ObservacionController(IObservacion observacion)
    {
        _observacion = observacion;
    }

    [HttpPost]
    public object Create(NuevoObservacionRequest datos)
    {
        var user = UserJwt.Get(Request.Headers.Authorization);
        return _observacion.Create(datos, user);
    }
}