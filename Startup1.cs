using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(CustomLogin.Startup1))]

namespace CustomLogin
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {

            app.MapSignalR();
        }
    }
}
