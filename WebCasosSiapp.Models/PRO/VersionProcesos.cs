using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("VersionProceso", Schema = "PRO")]
public class VersionProcesos
{
    public string Id { get; set; }
    public string ProcesoId { get; set; }
    public int NumeroVersion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string CreadoPor { get; set; }
}