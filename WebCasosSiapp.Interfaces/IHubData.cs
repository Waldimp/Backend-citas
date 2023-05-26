namespace WebCasosSiapp.Interfaces;

public interface IHubData
{
    object GetProcessesVersionsList(string user);
    object GetNewActivitiesList(string user);
    object GetDetailActivitiesList(string? user, string? version);
}