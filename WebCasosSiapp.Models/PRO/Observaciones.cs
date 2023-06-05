using System.ComponentModel.DataAnnotations.Schema;
using WebCasosSiapp.Models.SIS;

namespace WebCasosSiapp.Models.PRO;

[Table("ObservacionPaso", Schema = "PRO")]
public class Observaciones
{
    public string Id { get; set; }
    public string PasoId { get; set; }
    public string CreadoPor { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public string Observacion { get; set; }
}