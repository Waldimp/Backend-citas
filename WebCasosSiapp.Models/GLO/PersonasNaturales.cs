using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("PersonasNaturales", Schema = "GLO")]
public class PersonasNaturales
{
    public int CodigoPersona { get; set; }
    public string ConDui { get; set; }
    public string CodigoNumeroDui { get; set; }
    public string Nombre1 { get; set; }
    public string? Nombre2 { get; set; }
    public string? Nombre3 { get; set; }
    public string Apellido1 { get; set; }
    public string? Apellido2 { get; set; }
    public string? ApellidoCasada { get; set; }
    public string? ConocidoPor { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string CodigoUbicacionNacimiento { get; set; }
    public string? NombrePadre { get; set; }
    public string? NombreMadre { get; set; }
    public string? CodigoEstadoFamiliar { get; set; }
    public string? NombreConyugue { get; set; }
    public string Sexo { get; set; }
    public DateTime? FechaEmisionDui { get; set; }
    public string? CodigoUbicacionDui { get; set; }
    public string? CodigoOcupacion { get; set; }
    public string CodigoUbicacionNacionalidad { get; set; }
    public string? HuellasDactilares { get; set; }
    public string? CodigoNivelAcademico { get; set; }
}