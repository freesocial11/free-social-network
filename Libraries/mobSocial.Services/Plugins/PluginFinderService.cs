using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Plugins;
using mobSocial.Services.Extensions;

namespace mobSocial.Services.Plugins
{
    public class PluginFinderService : IPluginFinderService
    {
        public PluginInfo FindPlugin(string systemName)
        {
            var pluginInfo = PluginEngine.Plugins.FirstOrDefault(x => x.SystemName == systemName);
            if (pluginInfo != null)
            {
                pluginInfo.InitializePluginType();
            }
            return pluginInfo;
            
        }

        public IList<PluginInfo> FindPlugins<T>() where T : IPlugin
        {
            return PluginEngine.Plugins.Where(x => typeof(T).IsAssignableFrom(x.PluginType)).ToList();
        }
    }
}