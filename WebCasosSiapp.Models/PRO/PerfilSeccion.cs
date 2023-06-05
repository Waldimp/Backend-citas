using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("PerfilSeccion", Schema = "PRO")]
public class PerfilSeccion
{
    public string? Id { get; set; }
    public string? SeccionId { get; set; }
    public string? PerfilId { get; set; }
}