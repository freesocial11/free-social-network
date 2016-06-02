using System.Collections.Generic;
using mobSocial.Core.Plugins;

namespace mobSocial.Services.Plugins
{
    public interface IPluginFinderService
    {
        PluginInfo FindPlugin(string systemName);

        IList<PluginInfo> FindPlugins<T>() where T : IPlugin;
    }
}