using WebCasosSiapp.Models.PRO;

namespace WebCasosSiapp.ViewModels.Responses;

public class PasoResponse
{
    public string Id { get; set; }
    public Caso? Caso { get; set; }
    public ActividadVersionesExt? Actividad { get; set; } 
    public List<EstadoPaso>? Estados { get; set; } 
    public List<Responsable>? Responsables { get; set; } 
    public List<Observaciones>? Observaciones { get; set; } 
    public List<Secciones>? Secciones { get; set; } 
    public List<Registro>? Registros { get; set; }
    public ProcesoResumen? Proceso { get; set; }
    public List<ClienteResumen>? Clientes { get; set; }
}

public class ProcesoResumen
{
    public string Id { get; set; }
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Unidad { get; set; }
    public bool Interno { get; set; }
    public int Version { get; set; }
}

public class ClienteResumen
{
    public int CodigoPersona { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string Sexo { get; set; }
    public string Documento { get; set; }
    public string Foto { get; set; }
    public string CasoId { get; set; }
}