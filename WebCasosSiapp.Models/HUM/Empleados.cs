using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCasosSiapp.Models.HUM;

[Table("Empleados",Schema ="HUM")]
public class Empleados
{
    [Key]
    public int CodigoEmpleado { get; set; }
    public string CargoFuncional { get; set; }
    public decimal Salario { get; set; }
    public int CategoriaSalario { get; set; }
    public int StatusP { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string Acuerdo { get; set; }
    public string NumeroMarcacion { get; set; }
    public string CuentaBanco { get; set; }
    public string Pensionado { get; set; }
    public string CodigoUsuario { get; set; }
    public string CodigoPlaza { get; set; }
    public int CodigoPersona { get; set; }
    public string CodigoUnidades { get; set; }
    public string CodigoPagaduria { get; set; }
}