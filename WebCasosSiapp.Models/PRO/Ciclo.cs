using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("Ciclo", Schema = "PRO")]
public class Ciclo
{
    public string Id { get; set; }
    public string UsuarioId { get; set; }
    public string ActividadVersionId { get; set; }
    public bool Nuevo { get; set; }
    public DateTime CreatedAt { get; set; }
}