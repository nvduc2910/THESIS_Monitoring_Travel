using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using MonitoringTourSystem.Infrastructures.Implements;
using MonitoringTourSystem.Infrastructures.Interfaces.Home;
using System.Web.Mvc;

namespace MonitoringTourSystem.App_Start
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            container.RegisterType<IHome, Home>();
            container.RegisterType<IAdmin, Admin>();   
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}