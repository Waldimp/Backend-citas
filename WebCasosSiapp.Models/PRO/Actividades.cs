using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("Actividad", Schema = "PRO")]
public class Actividades
{
    public string Id { get; set; }
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string NombreColoquial { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string CreadoPor { get; set; }
}