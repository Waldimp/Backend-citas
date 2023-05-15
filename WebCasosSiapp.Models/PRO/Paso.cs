using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;
[Table("Paso", Schema = "PRO")]
public class Paso
{
    public string Id { get; set; }
    public string CasoId { get; set; }
    public string ActividadVersionId { get; set; }
    public List<EstadoPaso>? Estados { get; set; }
    public List<Responsable>? Responsable { get; set; }
}