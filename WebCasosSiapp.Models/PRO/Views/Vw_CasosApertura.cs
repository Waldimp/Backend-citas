using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebCasosSiapp.Models.PRO.Views;

[Table("VW_CasosApertura", Schema = "PRO")]
[Keyless]
public class Vw_CasosApertura
{
    public string ProcesoId { get; set; }
    public string NombreProceso { get; set; }
    public string VersionProcesoId { get; set; }
    public int NumeroVersion { get; set; }
    public string NombreActividad { get; set; }
    public string PerfilId { get; set; }
    public string CodigoUsuario { get; set; }
    public string NombresUsuario { get; set; }
    public string ApellidosUsuario { get; set; }
    public bool Fijo { get; set; }
    public string Unidades { get; set; }
}