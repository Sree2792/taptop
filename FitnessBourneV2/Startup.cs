using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FitnessBourneV2.Startup))]
namespace FitnessBourneV2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
