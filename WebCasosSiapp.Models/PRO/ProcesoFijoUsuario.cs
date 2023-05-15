using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("ProcesoFijoUsuario", Schema = "PRO")]
public class ProcesoFijoUsuario
{
    public string Id { get; set; }
    public string UsuarioId { get; set; }
    public string ProcesoId { get; set; }
}