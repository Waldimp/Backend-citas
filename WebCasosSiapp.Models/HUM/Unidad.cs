using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.HUM;

[Table("Unidades", Schema = "HUM")]
public class Unidad
{
    [Key]
    public string CodigoUnidades { get; set; }
    public string Unidades { get; set; }
    public string AbreviaturaUnidad { get; set; }
}