using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCasosSiapp.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CasoController : Controller
{
    private readonly ICaso _caso;

    public CasoController(ICaso caso)
    {
        _caso = caso;
    }
    
    [HttpPost("NuevoCaso")]
    public object NuevoCaso(NuevoCasoRequest request)
    {
        return _caso.NuevoCaso(request);
    }
    
    [HttpGet("ObtenerCaso/{id}")]
    public object ObtenerCaso(string id)
    {
        return _caso.ObtenerCaso(id);
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