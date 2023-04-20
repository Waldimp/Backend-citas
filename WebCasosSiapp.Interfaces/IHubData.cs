namespace WebCasosSiapp.Interfaces;

public interface IHubData
{
    object GetProcessesVersionsList(string user);
    object GetNewActivitiesList(string user);
}