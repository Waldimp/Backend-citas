using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebCasosSiapp.Functions;
using WebCasosSiapp.Hubs;
using WebCasosSiapp.Interfaces;

namespace WebCasosSiapp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PasoController : Controller
{
    private readonly IPaso _paso;
    private readonly IHubContext<CaseHub> _hub;
    private readonly IHubData _data;

    public PasoController(IPaso paso, IHubContext<CaseHub> hub, IHubData data)
    {
        _paso = paso;
        _hub = hub;
        _data = data;
    }
    
    [HttpGet("MarcarPasoLeido/{PasoId}")]
    public async Task<object> MarcarPasoLeido(string PasoId)
    {
        var user = UserJwt.Get(Request.Headers.Authorization);
        var respuesta = _paso.MarcarPasoLeido(PasoId, user);
        if (respuesta.Responsables == null) return respuesta.Response;
        await SendSignal.Send(_hub, _data, respuesta);
        return respuesta.Response;
    }
    
    [HttpGet("DatosDePaso/{PasoId}")]
    public object? DatosDePaso(string PasoId)
    {
        return _paso.DatosDePaso(PasoId);
    }

}