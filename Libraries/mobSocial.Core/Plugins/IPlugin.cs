using mobSocial.Core.Infrastructure.Mvc;

namespace mobSocial.Core.Plugins
{
    public interface IPlugin
    {
        /// <summary>
        /// The plugin info associated with the plugin
        /// </summary>
        PluginInfo PluginInfo { get; set; }

        /// <summary>
        /// Is current plugin a system plugin
        /// </summary>
        bool IsSystemPlugin { get; }

        /// <summary>
        /// Is current plugin an embedded assembly plugin
        /// </summary>
        bool IsEmbeddedPlugin { get; }

        /// <summary>
        /// Override this method to include operations that you wish to be perform on plugin installation
        /// </summary>
        void Install();

        /// <summary>
        /// Override this method to include operations that you wish to be perform on plugin uninstallation
        /// </summary>
        void Uninstall();

        /// <summary>
        /// Gets the configuration page route data
        /// </summary>
        /// <returns></returns>
        RouteData GetConfigurationPageRouteData();

        /// <summary>
        /// Gets the display page route data
        /// </summary>
        /// <returns></returns>
        RouteData GetDisplayPageRouteData();
    }
}