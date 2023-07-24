using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebCasosSiapp.Models.PRO.Views;

[Table("VW_CasosActividadesNuevas", Schema = "PRO")]
[Keyless]
public class VwCasoActividadNuevo
{
    public string ProcesoId { get; set; }
    public string NombreProceso { get; set; }
    public string VersionProcesoId { get; set; }
    public int Version { get; set; }
    public string ActividadVersionId { get; set; }
    public string NombreActividad { get; set; }
    public string Estado { get; set; }
    public string Expediente { get; set; }
    public DateTime FechaEstado { get; set; }
    public string AsignadoPorId { get; set; }
    public string? NombresAsignadoPor { get; set; }
    public string? ApellidosAsignadoPor { get; set; }
    public string UsuarioIdResponsable { get; set; }
    public string PasoId { get; set; }
    public string CasoId { get; set; }
}