using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.SIS;

[Table("SIS_UsuariosPorPerfil", Schema = "DBO")]
public class UsuarioPerfil
{
    public string? CodigoPerfil { get; set; }
    public string? CodigoUsuario { get; set; }
    public Usuarios? Usuario  { get; set; }
}