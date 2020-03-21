using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Maple_Building_Management_App.App_Start.ChatStartup))]

namespace Maple_Building_Management_App.App_Start
{
    public class ChatStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
