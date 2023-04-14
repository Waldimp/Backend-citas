using Microsoft.AspNetCore.SignalR;
using WebCasosSiapp.Interfaces;

namespace WebCasosSiapp.Hubs;

public class CaseHub : Hub
{
    private readonly IHubData _hubData;

    public CaseHub(IHubData hubData)
    {
        _hubData = hubData;
    }

    // Join to general hub; with it you can get the list of processes
    public async Task Join(string user)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, user);
    }
    
    // Join to specific hub; with it can get the list of cases by process
    public async Task SpecificJoin(string user, string processId)
    {
        var hubName = user + "**" + processId;
        await Groups.AddToGroupAsync(Context.ConnectionId, hubName);
    }
    
}