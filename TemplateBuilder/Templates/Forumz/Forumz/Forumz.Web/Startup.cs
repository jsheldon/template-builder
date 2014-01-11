using System.Web.Mvc;
using Forumz.Ifx.IoC;
using Forumz.Web.Core.Startup;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Startup))]
namespace Forumz.Web.Core.Startup
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = Bootstrapper.Init();
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));

            ConfigureAuth(app);
        }
    }
}
