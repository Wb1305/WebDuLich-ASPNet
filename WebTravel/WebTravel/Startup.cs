using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebTravel.Startup))]
namespace WebTravel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
