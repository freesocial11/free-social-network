using System.Linq;
using DryIoc;
using mobSocial.Core.Plugins;

namespace mobSocial.Services.Extensions
{
    public static class PluginInfoExtensions
    {
        public static void InitializePluginType(this PluginInfo pluginInfo)
        {
            if (pluginInfo.PluginType == null)
            {
                //we need to load the type of plugin
                var allTypes = pluginInfo.ShadowAssembly.GetLoadedTypes();
                pluginInfo.PluginType = allTypes.FirstOrDefault(x => typeof(IPlugin).IsAssignableFrom(x));
            }
        }
    }
}