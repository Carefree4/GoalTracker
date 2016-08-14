using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoalTracker.Startup))]
namespace GoalTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
