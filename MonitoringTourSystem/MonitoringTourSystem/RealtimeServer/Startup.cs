using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(PlaneSeats.Startup))]

namespace PlaneSeats
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    
                    EnableDetailedErrors = true,
                };
                hubConfiguration.En
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}