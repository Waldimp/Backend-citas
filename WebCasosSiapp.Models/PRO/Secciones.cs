using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("Seccion", Schema = "PRO")]
public class Secciones
{
    public string? Id { get; set; }
    public string? ActividadVersionId { get; set; }
    public string? FormularioId { get; set; }
    public int Orden { get; set; }  
    public string? Etiqueta { get; set; }
    public bool Activo { get; set; }
    public bool Dinamico { get; set; }
    public string? SeccionId { get; set; }
    public string? Metadata { get; set; }
    public List<PerfilSeccion>? PerfilSecciones { get; set; }
}
