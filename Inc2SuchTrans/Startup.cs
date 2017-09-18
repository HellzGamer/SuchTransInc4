using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Inc2SuchTrans.Startup))]
namespace Inc2SuchTrans
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
