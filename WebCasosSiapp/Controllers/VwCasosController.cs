using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCasosSiapp.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.Configurations;
using WebCasosSiapp.Models.PRO.Views;

namespace WebCasosSiapp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VwCasosController : Controller
{
    private readonly IVwCasos _casos;
    private readonly EnvironmentConfig _env;

    public VwCasosController(IVwCasos casos, EnvironmentConfig env)
    {
        _casos = casos;
        _env = env;
    }

    [HttpGet]
    public List<VwCasoResumen>? Index()
    {
        var user = UserJwt.Get(Request.Headers.Authorization);
        return _casos.Index(user);
    }

    [HttpGet("mensaje")]
    public string Mensaje()
    {
        return _env.Message;
    }
}