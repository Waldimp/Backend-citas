using WebCasosSiapp.Models.PRO.Views;

namespace WebCasosSiapp.Interfaces;

public interface IVwCasos
{
    List<VwCasoResumen>? Index(string? user);
}