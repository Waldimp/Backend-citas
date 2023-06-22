using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RegistroController : Controller
{
    private readonly IRegistro _registro;

    public RegistroController(IRegistro registro)
    {
        _registro = registro;
    }
    
    [HttpPost]
    public object Create(NuevoRegistroRequest datos)
    {
        return _registro.Create(datos);
    }
    
}