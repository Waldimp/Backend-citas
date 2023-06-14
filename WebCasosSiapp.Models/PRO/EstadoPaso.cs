using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;
[Table("EstadoPaso", Schema = "PRO")]
public class EstadoPaso
{
    public string Id { get; set; }
    public string PasoId { get; set; }
    public string Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? AsignadoPor { get; set; }
}