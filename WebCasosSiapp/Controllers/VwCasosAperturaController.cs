using Microsoft.AspNetCore.Mvc;
using WebCasosSiapp.Functions;
using WebCasosSiapp.Interfaces;

namespace WebCasosSiapp.Controllers;
[ApiController]
[Route("api/[controller]")]
public class VwCasosAperturaController : Controller
{
    private readonly IVwCasosApertura _casosApertura;
    
    public VwCasosAperturaController(IVwCasosApertura casosApertura)
    {
        _casosApertura = casosApertura;
    }
    
    [HttpGet]
    public object Index()
    {
        var user = UserJwt.Get(Request.Headers.Authorization);
        return _casosApertura.Index(user);
    }
}