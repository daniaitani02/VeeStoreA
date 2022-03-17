using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VeeStoreA.Startup))]
namespace VeeStoreA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
