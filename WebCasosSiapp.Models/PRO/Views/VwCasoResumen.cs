using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebCasosSiapp.Models.PRO.Views;

[Table("VW_CasosResumen", Schema = "PRO")]
[Keyless]
public class VwCasoResumen
{
    public string ProcesoId { get; set; }
    public string NombreProceso { get; set; }
    public string NombreUnidad { get; set; }
    public string VersionProcesoId { get; set; }
    public int Version { get; set; }
    public int Abiertos { get; set; }
    public int Resueltos { get; set; }
    public bool Nuevos { get; set; }
    public string UsuarioIdResponsable { get; set; }
}