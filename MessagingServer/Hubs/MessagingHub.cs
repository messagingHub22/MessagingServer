using Microsoft.AspNetCore.SignalR;

namespace MessagingServer.Hubs
{
    public class MessagingHub : Hub
    {
        public async Task ReloadMessage(string user)
        {
            await Clients.All.SendAsync("ReloadMessageClient", user);
        }
    }
}
