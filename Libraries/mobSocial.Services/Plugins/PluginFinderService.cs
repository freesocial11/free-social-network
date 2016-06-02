using System.Collections.Generic;
using System.Linq;
using DryIoc;
using mobSocial.Core.Plugins;

namespace mobSocial.Services.Plugins
{
    public class PluginFinderService : IPluginFinderService
    {
        public PluginInfo FindPlugin(string systemName)
        {
            var pluginInfo = PluginEngine.Plugins.FirstOrDefault(x => x.SystemName == systemName);
            if (pluginInfo != null)
            {
                if (pluginInfo.PluginType == null)
                {
                    //we need to load the type of plugin
                    var allTypes = pluginInfo.ShadowAssembly.GetLoadedTypes();
                    pluginInfo.PluginType = allTypes.FirstOrDefault(x => typeof(IPlugin).IsAssignableFrom(x));
                }
            }
            return pluginInfo;
            
        }

        public IList<PluginInfo> FindPlugins<T>() where T : IPlugin
        {
            return PluginEngine.Plugins.Where(x => typeof(T).IsAssignableFrom(x.PluginType)).ToList();
        }
    }
}