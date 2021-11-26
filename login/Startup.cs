using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(login.Startup))]
namespace login
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
