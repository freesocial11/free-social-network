using mobSocial.Core.Plugins;

namespace mobSocial.Services.Plugins
{
    public interface IPluginInstallerService
    {
        void Install(PluginInfo pluginInfo);

        void Uninstall(PluginInfo pluginInfo);

        void Activate(PluginInfo pluginInfo);

        void Deactivate(PluginInfo pluginInfo);

    }
}