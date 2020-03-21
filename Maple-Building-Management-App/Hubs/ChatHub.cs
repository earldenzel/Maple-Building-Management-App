using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Maple_Building_Management_App.Models;
using Microsoft.AspNet.SignalR;

namespace Maple_Building_Management_App.Hubs
{
    public class ChatHub : Hub
    {
        static List<Chatter> SignalRUsers = new List<Chatter>();
        
        public override Task OnDisconnected(bool stopCalled)
        {
            var item = SignalRUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                SignalRUsers.Remove(item);
            }
            Clients.All.addDisconnectionNotice(item.UserName);
            Clients.All.refreshClientList();

            return base.OnDisconnected(stopCalled);
        }

        public void Send(string name, string title, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(fullTitle(name, title), message);
        }

        public void Exit(string name, string title)
        {
            Clients.All.addDisconnectionNotice(fullTitle(name, title));
            Clients.All.refreshClientList();

        }

        public void Enter(string name, string title)
        {
            Clients.All.addNewPersonToPage(fullTitle(name, title));
            Clients.All.refreshClientList();

            var id = Context.ConnectionId;

            if (SignalRUsers.Count(x => x.ConnectionId == id) == 0)
            {
                SignalRUsers.Add(new Chatter { ConnectionId = id, UserName = fullTitle(name, title) });
            }
        }

        //return list of all active connections
        public List<string> GetAllActiveConnections()
        {
            List<string> users = new List<string>();
            foreach (Chatter chatter in SignalRUsers)
            {
                users.Add(chatter.UserName);
            }
            return users;
        }

        private string fullTitle(string name, string title)
        {
            return name + " (" + title + ")";
        }
    }
}