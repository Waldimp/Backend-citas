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
        foreach (var responsable in respuesta.Responsables)
        {
            var resResumen = _data.GetProcessesVersionsList(responsable.UsuarioId);
            var grupoResumen = "pvl" + responsable.UsuarioId;
            await _hub.Clients.Group(grupoResumen).SendAsync("getProcessesVersionsList", resResumen);

            var resNuevo = _data.GetNewActivitiesList(responsable.UsuarioId);
            var grupoNuevo = "nal" + responsable.UsuarioId;
            await _hub.Clients.Group(grupoNuevo).SendAsync("getNewActivitiesList", resNuevo);

            if (respuesta.VersionId != null)
            {
                var resDetalle = _data.GetDetailActivitiesList(responsable.UsuarioId, respuesta.VersionId);
                var grupoDetalle = "dpv" + responsable.UsuarioId + "**" + respuesta.VersionId;
                await _hub.Clients.Group(grupoDetalle).SendAsync("getDetailProcessesVersionList", resDetalle);
            }
        }
        return respuesta.Response;
    }
    
    [HttpPost("FinalizarPaso/{pasoId}")]
    public object FinalizarPaso( string pasoId,  FinalizarPasoRequest request)
    {
        var user = UserJwt.Get(Request.Headers.Authorization);
        return _caso.FinalizarPaso(pasoId, request, user);
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