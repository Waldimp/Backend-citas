using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("ActividadVersion", Schema = "PRO")]
public class ActividadVersiones
{
    public string Id { get; set; }
    public bool MedirTiempo { get; set; }
    public int? TiempoIdeal { get; set; }
    public int Relevancia { get; set; }
    public string TipoActividad { get; set; }
    public bool Observaciones { get; set; }
    public bool Activo { get; set; }
    public bool? Favorable { get; set; }
    public string TipoEtapa { get; set; }
    public string Movimiento { get; set; }
    public bool? Iterativo { get; set; }
    public string VersionProcesoId { get; set; }
    
    public string ActividadId { get; set; }
    
}