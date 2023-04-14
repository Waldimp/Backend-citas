using Microsoft.AspNetCore.Mvc;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO.Views;

namespace WebCasosSiapp.Controllers;

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
    public List<VwCaso>? Index()
    {
        return _casos.Index();
    }
}