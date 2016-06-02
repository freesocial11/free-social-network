using mobSocial.Core.Plugins;

namespace mobSocial.Services.Plugins
{
    public class PluginInstallerService : IPluginInstallerService
    {
        public void Install(PluginInfo pluginInfo)
        {
            if (pluginInfo.Installed)
                return;

            var pluginType = pluginInfo.LoadPluginInstance<IPlugin>();
            if(pluginType != null)
                pluginType.Install();
        }

        public void Uninstall(PluginInfo pluginInfo)
        {
            if (!pluginInfo.Installed)
                return;

            var pluginType = pluginInfo.LoadPluginInstance<IPlugin>();
            if (pluginType != null)
                pluginType.Uninstall();
        }

        public void Activate(PluginInfo pluginInfo)
        {
            throw new System.NotImplementedException();
        }

        public void Deactivate(PluginInfo pluginInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}