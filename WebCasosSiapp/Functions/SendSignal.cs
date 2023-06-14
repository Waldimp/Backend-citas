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
            var resResumen = data.GetProcessesVersionsList(responsable.UsuarioId);
            var grupoResumen = "pvl" + responsable.UsuarioId;
            await hub.Clients.Group(grupoResumen).SendAsync("getProcessesVersionsList", resResumen);

            var resNuevo = data.GetNewActivitiesList(responsable.UsuarioId);
            var grupoNuevo = "nal" + responsable.UsuarioId;
            await hub.Clients.Group(grupoNuevo).SendAsync("getNewActivitiesList", resNuevo);

            if (res.VersionId != null)
            {
                var resDetalle = data.GetDetailActivitiesList(responsable.UsuarioId, res.VersionId);
                var grupoDetalle = "dpv" + responsable.UsuarioId + "**" + res.VersionId;
                await hub.Clients.Group(grupoDetalle).SendAsync("getDetailProcessesVersionList", resDetalle);
            }
        }
    }
}