using System.ComponentModel.DataAnnotations.Schema;
using WebCasosSiapp.Models.SIS;

namespace WebCasosSiapp.Models.PRO;

[Table("ActividadVersionPerfil", Schema = "PRO")]
public class ActividadVersionPerfil
{
    public string Id { get; set; }
    public string ActividadVersionId { get; set; }
    public string PerfilId { get; set; }
    public string? Rol { get; set; }
    
    public UsuarioPerfil? Perfil { get; set; }
}