using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.Realtime.Hubs;

/// <summary>
/// Хаб доставки
/// </summary>
[Authorize]
public sealed class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value;

        await Clients.Caller.SendAsync("Connected", new
        {
            userId,
            serverTime = DateTimeOffset.UtcNow,
        });

        await base.OnConnectedAsync();
    }
}
