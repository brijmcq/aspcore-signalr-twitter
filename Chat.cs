using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tweetToMap
{
    public class Chat: Hub
    {
        public void SendToAll(string name, string message)
        {
            

            Clients.All.InvokeAsync("sendToAll", name, message);
        }
        public override Task OnConnectedAsync()
        {
        //    Clients.All.InvokeAsync("sendToAll", "system", $"{Context.User.Identity.Name} joined the conversation");
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {

            return base.OnDisconnectedAsync(exception);
        }
    }
}
