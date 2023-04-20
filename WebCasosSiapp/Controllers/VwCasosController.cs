using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCasosSiapp.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO.Views;

namespace WebCasosSiapp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VwCasosController : Controller
{
    private readonly IVwCasos _casos;

    public VwCasosController(IVwCasos casos)
    {
        _casos = casos;
    }

    [HttpGet]
    public List<VwCasoResumen>? Index()
    {
        var user = UserJwt.Get(Request.Headers.Authorization);
        return _casos.Index(user);
    }
}