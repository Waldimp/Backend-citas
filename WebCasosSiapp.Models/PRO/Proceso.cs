using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.PRO;

[Table("Proceso", Schema = "PRO")]
public class Proceso
{
    public string Id { get; set; }
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string CodigoUnidad { get; set; }
    public bool Temporal { get; set; }
    public DateTime? FechaInicial { get; set; }
    public DateTime? FechaFinal { get; set; }
    public bool ComentarioApertura { get; set; }
    public bool ValidacionApertura { get; set; }
    public bool Activo { get; set; }
    public bool UsuarioCliente { get; set; }
    public bool Interno { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string CreadoPor { get; set; }
}