using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("PasoReapertura", Schema = "PRO")]
public class PasoReapertura
{
    public string Id { get; set; }
    public string PasoId { get; set; }
    public string Justificacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string CreadoPor { get; set; }
}