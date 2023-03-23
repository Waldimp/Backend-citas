using Microsoft.AspNetCore.SignalR;

namespace WebCasosSiapp.Hubs;

public class CasosHub : Hub
{
    public CasosHub() {}

    // Unirse al Hub
    public async Task Join()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "");
    }
}