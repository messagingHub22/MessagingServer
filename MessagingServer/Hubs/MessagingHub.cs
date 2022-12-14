using Microsoft.AspNetCore.SignalR;

namespace MessagingServer.Hubs
{
    public class MessagingHub : Hub
    {
        // Map usernames to connection ids (For server to user messages)
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();


        // Map usernames to connection ids (For user to user messages)
        private readonly static ConnectionMapping<string> _userConnections =
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

        // Sends a call to the particular user who got new user to user message
        public async Task ReloadUserMessageForUser(string user, string from)
        {
            foreach (var connectionId in _userConnections.GetConnections(user))
            {
                await Clients.Client(connectionId).SendAsync("ReloadClientUserMessage", from);
            }
        }

        public override Task OnConnectedAsync()
        {
            string name = Context.GetHttpContext().Request.Query["username"];
            string userMessageName = Context.GetHttpContext().Request.Query["userMessageName"];

            if (name != null) // name is null when client is server page
            {
                _connections.Add(name, Context.ConnectionId);
            }

            if (userMessageName != null) // userMessageName is null when client is server page
            {
                _userConnections.Add(userMessageName, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string name = Context.GetHttpContext().Request.Query["username"];
            string userMessageName = Context.GetHttpContext().Request.Query["userMessageName"];

            if (name != null)
            {
                _connections.Remove(name, Context.ConnectionId);
            }

            if (userMessageName != null)
            {
                _userConnections.Remove(userMessageName, Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

    }
}
