using Owin;

namespace mobSocial.Core.Startup
{
    /// <summary>
    /// Task for running owin startup tasks
    /// </summary>
    public interface IOwinStartupTask
    {
        void Configuration(IAppBuilder app);

        int Priority { get; }
    }
}