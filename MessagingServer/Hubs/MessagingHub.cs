using Microsoft.AspNetCore.SignalR;

namespace MessagingServer.Hubs
{
    public class MessagingHub : Hub
    {
        private string userConnected;

        public string UserConnected { get => userConnected; set => userConnected = value; }

        // Sends the updated user to all connected clients
        public async Task ReloadMessage(string user)
        {
            await Clients.All.SendAsync("ReloadMessageClient", user);
        }

        // Sends a call to the particular user who got new message
        public async Task ReloadMessageForUser(string user)
        {
            // TODO
            await Clients.Client("").SendAsync("ReloadMessageClient", user);
        }

        // Logs in the user
        public async Task LoginUser(string user)
        {
           userConnected = user;
        }
        
    }
}
