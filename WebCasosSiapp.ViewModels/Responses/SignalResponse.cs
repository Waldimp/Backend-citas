using WebCasosSiapp.Models.PRO;

namespace WebCasosSiapp.ViewModels.Responses;

public class SignalResponse
{
    public object Response { get; set; }
    public string? VersionId { get; set; }
    public List<string>? Responsables { get; set; }
}