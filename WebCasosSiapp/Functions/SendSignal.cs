using Microsoft.AspNetCore.SignalR;
using WebCasosSiapp.Hubs;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.ViewModels.Responses;

namespace WebCasosSiapp.Functions;

public class SendSignal
{
    public static async Task Send(IHubContext<CaseHub> hub, IHubData data, SignalResponse res)
    {
        foreach (var responsable in res.Responsables)
        {
            System.Diagnostics.Debug.WriteLine("Responsable xx -------------->" + responsable);
            var resResumen = data.GetProcessesVersionsList(responsable);
            var grupoResumen = "pvl" + responsable;
            await hub.Clients.Group(grupoResumen).SendAsync("getProcessesVersionsList", resResumen);

            var resNuevo = data.GetNewActivitiesList(responsable);
            var grupoNuevo = "nal" + responsable;
            await hub.Clients.Group(grupoNuevo).SendAsync("getNewActivitiesList", resNuevo);

            if (res.VersionId != null)
            {
                var resDetalle = data.GetDetailActivitiesList(responsable, res.VersionId);
                var grupoDetalle = "dpv" + responsable + "**" + res.VersionId;
                await hub.Clients.Group(grupoDetalle).SendAsync("getDetailProcessesVersionList", resDetalle);
            }
        }
    }
}