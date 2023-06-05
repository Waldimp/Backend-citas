using WebCasosSiapp.Models.PRO;

namespace WebCasosSiapp.ViewModels.Responses;

public class PasoResponse
{
    public string Id { get; set; }
    public Caso? Caso { get; set; }
    public ActividadVersiones? Actividad { get; set; } 
    public List<EstadoPaso>? Estados { get; set; } 
    public List<Responsable>? Responsables { get; set; } 
    public List<Observaciones>? Observaciones { get; set; } 
    public List<Secciones>? Secciones { get; set; } 
    public List<Registro>? Registros { get; set; }

}