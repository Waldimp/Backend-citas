using Microsoft.AspNetCore.Mvc;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO.Views;

namespace WebCasosSiapp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VwCasesController : Controller
{
    private readonly IVwCases _cases;

    public VwCasesController(IVwCases cases)
    {
        _cases = cases;
    }

    [HttpGet]
    public List<VwCaso>? Index()
    {
        return _cases.Index();
    }
}