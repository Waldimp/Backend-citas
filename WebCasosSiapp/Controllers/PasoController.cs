using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCasosSiapp.Interfaces;

namespace WebCasosSiapp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PasoController : Controller
{
    private readonly IPaso _paso;
    
    public PasoController(IPaso paso)
    {
        _paso = paso;
    }
    
    [HttpGet("MarcarPasoLeido/{PasoId}")]
    public object MarcarPasoLeido(string PasoId)
    {
        return _paso.MarcarPasoLeido(PasoId);
    }
    
    [HttpGet("DatosDePaso/{PasoId}")]
    public object DatosDePaso(string PasoId)
    {
        return _paso.DatosDePaso(PasoId);
    }

}