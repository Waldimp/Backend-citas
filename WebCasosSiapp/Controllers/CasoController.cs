using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebCasosSiapp.Functions;
using WebCasosSiapp.Hubs;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CasoController : Controller
{
    private readonly ICaso _caso;
    private readonly IHubContext<CaseHub> _hub;
    private readonly IHubData _data;

    public CasoController(ICaso caso, IHubContext<CaseHub> hub, IHubData data)
    {
        _caso = caso;
        _hub = hub;
        _data = data;
    }
    
    [HttpPost("NuevoCaso")]
    public async Task<object> NuevoCaso(NuevoCasoRequest request)
    {
        var respuesta = _caso.NuevoCaso(request);
        if (respuesta.Responsables == null) return respuesta.Response;
        await SendSignal.Send(_hub, _data, respuesta);
        return respuesta.Response;
    }
    
    [HttpPost("FinalizarPaso/{pasoId}")]
    public async Task<object> FinalizarPaso( string pasoId,  FinalizarPasoRequest request)
    {
        var user = UserJwt.Get(Request.Headers.Authorization);
        var respuesta = _caso.FinalizarPaso(pasoId, request, user);
        System.Diagnostics.Debug.WriteLine("Debug baby -----------------> " + respuesta.Response.ToString());
        if (respuesta.Responsables == null) return respuesta.Response;
        await SendSignal.Send(_hub, _data, respuesta);
        return respuesta.Response;
    }

    [HttpPost("CambiarContexto")]
    public async Task<object> CambiarContexto(CambioContextoRequest datos)
    {
        var user = UserJwt.Get(Request.Headers.Authorization);
        var respuesta = _caso.CambioContexto(datos, user);
        if (respuesta.Responsables == null) return respuesta.Response;
        await SendSignal.Send(_hub, _data, respuesta);
        return respuesta.Response;
    }

    [HttpGet("ObtenerCaso/{id}")]
    public object ObtenerCaso(string id)
    {
        return _caso.ObtenerCaso(id);
    }
    
    [HttpGet("TestAsignacionCiclica/{ActividadVersionId}")]
    public object TestAsignacionCiclica(string ActividadVersionId)
    {
        return _caso.AsignacionCiclica(ActividadVersionId);
    }
    
    [HttpPost("FijarProcesoUsuario/{ProcesoId}")]
    public object FijarProcesoUsuario(string ProcesoId)
    {
        var user = UserJwt.Get(Request.Headers.Authorization);
        return _caso.FijarProcesoUsuario(ProcesoId, user);
    }
    
    [HttpDelete("EliminarProcesoFijoUsuario/{ProcesoId}")]
    public object EliminarProcesoFijoUsuario(string ProcesoId)
    {
        var user = UserJwt.Get(Request.Headers.Authorization);
        return _caso.EliminarProcesoFijoUsuario(ProcesoId, user);
    }
}