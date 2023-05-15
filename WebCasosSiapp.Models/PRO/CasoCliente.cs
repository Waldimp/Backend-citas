using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("CasoCliente", Schema = "PRO")]
public class CasoCliente
{
    public string Id { get; set; }
    public string CasoId { get; set; }
    public int ClienteId { get; set; }
    public PersonasNaturales Cliente { get; set; }
}