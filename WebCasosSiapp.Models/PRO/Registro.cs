using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("Registro", Schema = "PRO")]
public class Registro
{
    public string? Id { get; set; }
    public string? PasoId { get; set; }
    public string? SeccionId { get; set; }
    public string RegistroId { get; set; }
}