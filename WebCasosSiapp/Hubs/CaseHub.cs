using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Hubs;

public class CaseHub : Hub
{
    private readonly IHubData _hubData;

    public CaseHub(IHubData hubData)
    {
        _hubData = hubData;
    }

    // Join to general hub; with it you can get the list of processes
    public async void Join(string group)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
    }
    
    // Remove to hub connection
    public async Task Remove(string user)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, user);
    }

    // Join to specific hub; with it can get the list of cases by process
    public async Task SpecificJoin(string user, string versionProcessId)
    {
        var hubName = user + "**" + versionProcessId;
        await Groups.AddToGroupAsync(Context.ConnectionId, hubName);
    }

    // Get list of processes version per user
    public async Task GetProcessesVersionsList(string user)
    {
        var response = _hubData.GetProcessesVersionsList(user);
        var group = "pvl" + user;
        await Clients.Group(group).SendAsync("getProcessesVersionsList", response);
    }
    
    // Get list of new activities per user
    public async Task GetNewActivitiesList(string user)
    {
        var response = _hubData.GetNewActivitiesList(user);
        var group = "nal" + user;
        await Clients.Group(group).SendAsync("getNewActivitiesList", response);
    }
    
     public async Task GetDetailProcessesVersionList(HubConectionRequest req)
    {
        var response = _hubData.GetDetailActivitiesList(req.Usuario, req.Version);
        var group = "dpv" + req.Usuario + "**" + req.Version;
        await Clients.Group(group).SendAsync("getDetailProcessesVersionList", response);
    }
}