namespace WebCasosSiapp.Concretes.Functions;

public class Generals
{
    public static string GetUlid()
    {
        var guid = Guid.NewGuid().ToString();
        var codigo = string.Join("", guid.Split('-'));
        return codigo;
    }
}