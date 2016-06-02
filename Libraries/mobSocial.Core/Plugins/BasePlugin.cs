using mobSocial.Core.Infrastructure.Mvc;

namespace mobSocial.Core.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        protected BasePlugin()
        {
        }

        public virtual PluginInfo PluginInfo { get; set; }

        /// <summary>
        /// A system plugin can't be deactivated or uninstalled. It's install method is called immediately on application restart
        /// </summary>
        public virtual bool IsSystemPlugin => false;

        public virtual bool IsEmbeddedPlugin { get; set; }

        public virtual void Install()
        {
            PluginEngine.MarkInstalled(this.PluginInfo);
        }

        public virtual void Uninstall()
        {
            PluginEngine.MarkUninstalled(this.PluginInfo);
        }

        public abstract RouteData GetConfigurationPageRouteData();

        public abstract RouteData GetDisplayPageRouteData();
    }
}