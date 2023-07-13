using System.ComponentModel.DataAnnotations.Schema;
using WebCasosSiapp.Models.SIS;

namespace WebCasosSiapp.Models.PRO;

[Table("Responsable", Schema = "PRO")]
public class Responsable
{
    public string Id { get; set; }
    public string PasoId { get; set; }
    public string UsuarioId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? AsignadoPor { get; set; }
    public Usuarios? Usuario  { get; set; }
    public bool? Activo { get; set; }
}