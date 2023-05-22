using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("Caso", Schema = "PRO")]
public class Caso
{
    public string Id { get; set; }
    public string Codigo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Abierto { get; set; }
    public string? CasoAsociado { get; set; }
    public string? ComentarioApertura { get; set; }
    public string? Estado { get; set; }
    public List<Paso>? Pasos { get; set; }
    public List<CasoCliente>? Clientes { get; set; }
}