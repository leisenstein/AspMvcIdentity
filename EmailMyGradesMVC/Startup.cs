using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmailMyGradesMVC.Startup))]
namespace EmailMyGradesMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
