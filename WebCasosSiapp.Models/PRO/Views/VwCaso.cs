using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebCasosSiapp.Models.PRO.Views;

[Table("VW_CasosMain", Schema = "PRO")]
[Keyless]
public class VwCaso
{
#nullable enable
    public string? ProcesoId { get; set; }
    public string? NombreProceso { get; set; }
    public string? UnidadId { get; set; }
    public string? NombreUnidad { get; set; }
    public string? VersionProcesoId { get; set; }
    public int? Version { get; set; }
    public string? CasoId { get; set; }
    public string? Expediente { get; set; }
    public int? ClienteId { get; set; }
    public string? DuiCliente { get; set; }
    public string? NombresCliente { get; set; }
    public string? ApellidosCliente { get; set; }
    public DateTime? FechaApertura { get; set; }
    public bool? Abierto { get; set; }
    public string? PasoId { get; set; }
    public string? ActividadVersionId { get; set; }
    public string? NombreActividad { get; set; }
    public string? NombreColoquialActividad { get; set; }
    public bool? MedirTiempo { get; set; }
    public int? TiempoIdeal { get; set; }
    public int? Relevancia { get; set; }
    public string? EstadoId { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaEstado { get; set; }
    public string? ResponsableId { get; set; }
    public string? UsuarioIdResponsable { get; set; }
    public string? NombresResponsable { get; set; }
    public string? ApellidosResponsable { get; set; }
    public DateTime? FechaAsignacion { get; set; }
    public string? AsignadoPorId { get; set; }
    public string? NombresAsignadoPor { get; set; }
    public string? ApellidosAsignadoPor { get; set; }
    public string? PerfilIdSupervisorProceso { get; set; }
    public string? UsuarioIdSupervisorProceso { get; set; }
    public string? NombresSupervisorProceso { get; set; }
    public string? ApellidosSupervisorProceso { get; set; }
    public string? PerfilIdSupervisorActividad { get; set; }
    public string? UsuarioIdSupervisorActividad { get; set; }
    public string? NombresSupervisorActividad { get; set; }
    public string? ApellidosSupervisorActividad { get; set; }
    public string? RolSupervisorActividad { get; set; }
    #nullable disable
}