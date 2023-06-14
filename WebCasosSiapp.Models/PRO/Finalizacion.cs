using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;
[Table("Finalizacion", Schema = "PRO")]
public class Finalizacion
{
    public string Id { get; set; }
    public string PasoId { get; set; }
    public string Justificacion { get; set; }
    public string Estado { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? CreadoPor { get; set; }
}