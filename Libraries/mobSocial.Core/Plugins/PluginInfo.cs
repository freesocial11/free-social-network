using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DryIoc;
using mobSocial.Core.Infrastructure.AppEngine;

namespace mobSocial.Core.Plugins
{
    public class PluginInfo
    {
        public string Name { get; set; }

        public string SystemName { get; set; }

        public IList<string> SupportedVersions { get; set; }

        public string Author { get; set; }

        public string AuthorUri { get; set; }

        public string PluginUri { get; set; }

        public string AssemblyDllName { get; set; }

        public Assembly ShadowAssembly { get; set; }

        public FileInfo OriginalAssemblyFileInfo { get; set; }

        public Type PluginType { get; set; }

        public bool IsSystemPlugin { get; set; }

        public bool IsEmbeddedPlugin { get; set; }

        public bool Installed { get; set; }

        public bool Active { get; set; }

        public static PluginInfo Load(string fileName)
        {
            return PluginConfigurator.LoadPluginInfo(fileName);
        }

        public T LoadPluginInstance<T>() where T : class, IPlugin
        {
            var instance = mobSocialEngine.ActiveEngine.IocContainer.Resolve(this.PluginType, IfUnresolved.ReturnDefault);
            if (instance == null)
            {
                instance = Activator.CreateInstance(this.PluginType);
                //register this instance for future access
                mobSocialEngine.ActiveEngine.IocContainer.RegisterInstance(instance, new SingletonReuse());
            }
            var pluginTypedInstance = instance as T;
            if (pluginTypedInstance != null)
                pluginTypedInstance.PluginInfo = this;

            return pluginTypedInstance;
        }
    }
}