namespace WebCasosSiapp.Interfaces;

public interface IPaso
{
    object MarcarPasoLeido(string PasoId);
    object DatosDePaso(string PasoId);
}