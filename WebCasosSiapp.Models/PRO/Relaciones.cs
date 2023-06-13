using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("Relacion", Schema = "PRO")]
public class Relaciones
{
    public string Id { get; set; }
    public string ActividadVersionOrigen { get; set; }
    public string ActividadVersionDestino { get; set; }
    public string TipoMovimiento { get; set; }
    public string? Accion { get; set; }
    public bool ResumenDatos { get; set; }
    public string? TipoSeleccion { get; set; }
}

public class RelacionesExt
{
    public string Id { get; set; }
    public string ActividadVersionOrigen { get; set; }
    public string ActividadVersionDestino { get; set; }
    public string TipoMovimiento { get; set; }
    public string? Accion { get; set; }
    public bool ResumenDatos { get; set; }
    public string? TipoSeleccion { get; set; }
 
    // Nombres de las actividades
    public string? NombreActividadDestino { get; set; }
    public string? NombreColoquialActividadDestino { get; set; }
}