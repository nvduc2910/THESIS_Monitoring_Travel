using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;


[assembly: OwinStartup(typeof(MonitoringTourSystem.Startup))]

namespace MonitoringTourSystem
{


    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
          //  GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new MyIdProvider());

            app.MapSignalR(); app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                };
                map.RunSignalR(hubConfiguration);
            });
        }

    }
}