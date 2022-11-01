using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace MessagingServer.Hubs
{
    public class MessagingHub : Hub
    {
        // Map usernames to connection ids
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        // Sends the updated user to all connected clients
        public async Task ReloadMessage(string user)
        {
            await Clients.All.SendAsync("ReloadMessageClient", user);
        }

        // Sends a call to the particular user who got new message
        public async Task ReloadMessageForUser(string user)
        {
            foreach (var connectionId in _connections.GetConnections(user))
            {
                await Clients.Client(connectionId).SendAsync("ReloadClientUser");
            }
        }

        public override Task OnConnectedAsync()
        {
            string name = Context.GetHttpContext().Request.Query["username"];

            if (name != null) // Name is null when client is server page
            {
                _connections.Add(name, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string name = Context.GetHttpContext().Request.Query["username"];

            if (name != null)
            {
                _connections.Remove(name, Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

    }
}
