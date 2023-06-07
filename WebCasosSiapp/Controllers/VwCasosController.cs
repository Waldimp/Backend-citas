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
    private readonly EnvironmentConfig _env;
    private readonly IHubData _hubData;

    public VwCasosController(EnvironmentConfig env, IHubData hubData)
    {
        _env = env;
        _hubData = hubData;
    }

    [HttpGet("mensaje/{usuario}")]
    public object Mensaje(string usuario)
    {
        return _hubData.GetDetailActivitiesList(usuario, "5c5a827eaeb2449ca560228f0d60d8c3");
    }
}