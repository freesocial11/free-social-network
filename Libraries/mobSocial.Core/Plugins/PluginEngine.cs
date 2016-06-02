using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using mobSocial.Core.Exception;
using mobSocial.Core.Infrastructure.Utils;
using mobSocial.Core.Plugins;

[assembly: PreApplicationStartMethod(typeof(PluginEngine), "Initialize")]
namespace mobSocial.Core.Plugins
{
    public class PluginEngine
    {
        private const string PluginsFile = "~/App_Data/Plugins.config";

        private static string _pluginsDirectory = "~/Plugins";
        private static string _shadowDirectory = "~/Plugins/loaded"; //this is set in web.config as probe path //used only for medium trust

        public static IList<PluginInfo> Plugins { get; set; }

        private static readonly DirectoryInfo PluginsDirectory;

        private static readonly DirectoryInfo ShadowDirectory;

        private static readonly AspNetHostingPermissionLevel TrustLevel;


        static PluginEngine()
        {
            TrustLevel = ServerHelper.GetCurrentTrustLevel();
            if (TrustLevel == AspNetHostingPermissionLevel.Unrestricted)
            {
                _shadowDirectory = AppDomain.CurrentDomain.DynamicDirectory;
                ShadowDirectory = new DirectoryInfo(_shadowDirectory);
            }
            else
            {
                ShadowDirectory = new DirectoryInfo(HostingEnvironment.MapPath(_shadowDirectory));
                
            }
            
            PluginsDirectory = new DirectoryInfo(HostingEnvironment.MapPath(_pluginsDirectory));

            Plugins = new List<PluginInfo>();
        }
        /// <summary>
        /// Initializes the plugin engine for the application
        /// </summary>
        public static void Initialize()
        {
            //first we load the assemblies which have been compiled into
            LoadAssemblySystemPlugins();

            //now the other plugins
            //first read the plugins list
            var allPluginsSystemNames = PluginConfigurator.ReadConfigFile(HostingEnvironment.MapPath(PluginsFile));

            //create shadow directory
            Directory.CreateDirectory(_shadowDirectory);

            //delete all files from shadow directory if it's /loaded directory
            if (TrustLevel != AspNetHostingPermissionLevel.Unrestricted)
            {
                var allFiles = ShadowDirectory.GetFiles("*", SearchOption.AllDirectories);
                foreach (var file in allFiles)
                {
                    file.Delete();
                }
            }
            //make sure that we don't have null for all pluginnames
            allPluginsSystemNames = allPluginsSystemNames ?? new List<PluginConfigurator.ReadPluginInfo>();

            //load plugins
            var allPlugins = PluginConfigurator.GetAllPluginsInfos(PluginsDirectory, allPluginsSystemNames.ToList());
            foreach (var plugin in allPlugins)
            {
                //discard the plugin which is incompatible
                if(!plugin.SupportedVersions.Contains(GlobalSettings.AppVersion))
                    continue;

                if(string.IsNullOrEmpty(plugin.SystemName))
                    throw new mobSocialException(
                        $"A plugin with empty system name {plugin.Name}({plugin.OriginalAssemblyFileInfo.FullName}) can't be loaded");
                //is this plugin already there
                if(Plugins.Any(x => x.SystemName == plugin.SystemName))
                    throw new mobSocialException(
                        $"A plugin with same name {plugin.Name}({plugin.SystemName}) has already been loaded");

                //add to plugin list
                Plugins.Add(plugin);

                //if plugin is installed, copy it's dlls to shadow
                //if (!plugin.Installed) 
                //    continue;

                //load to build manager
                if (plugin.OriginalAssemblyFileInfo.Directory == null) 
                    continue;

                var allDlls = plugin.OriginalAssemblyFileInfo.Directory.GetFiles("*.dll",
                    SearchOption.AllDirectories);

                //copy all the dlls to shadow directory
                foreach (var dllFile in allDlls)
                {
                    try
                    {
                        var shadowedFilePath = Path.Combine(ShadowDirectory.FullName, dllFile.Name);
                        var shadowedFileInfo = dllFile.CopyTo(shadowedFilePath, true);

                        plugin.ShadowAssembly = Assembly.LoadFrom(shadowedFileInfo.FullName);

                        //add this assembly to buildmanager to load
                        AssemblyLoader.AddToBuildManager(plugin.ShadowAssembly);

                        //assign instances

                    }
                    catch (System.Exception ex)
                    {
                        //TODO: handle the failed case accordingly
                        throw;
                    }
                           
                }

                

                
            }

        }
        /// <summary>
        /// Loads the system plugins compiled into the assembly
        /// </summary>
        public static void LoadAssemblySystemPlugins()
        {
            var allTypes = TypeFinder.ClassesOfType<IPlugin>();
            foreach (var type in allTypes)
            {
                var instance = (IPlugin) Activator.CreateInstance(type);
                Plugins.Add(instance.PluginInfo);
            }

        }

        /// <summary>
        /// Marks a plugin as installed
        /// </summary>
        /// <param name="pluginInfo"></param>
        public static void MarkInstalled(PluginInfo pluginInfo)
        {
            pluginInfo.Installed = true;
            pluginInfo.Active = true;
            PluginConfigurator.WriteConfigFile(Plugins.ToList(), HostingEnvironment.MapPath(PluginsFile));
        }
        /// <summary>
        /// Marks a plugin as uninstalled
        /// </summary>
        /// <param name="pluginInfo"></param>
        public static void MarkUninstalled(PluginInfo pluginInfo)
        {
            pluginInfo.Installed = false;
            pluginInfo.Active = false;
            PluginConfigurator.WriteConfigFile(Plugins.ToList(), HostingEnvironment.MapPath(PluginsFile));
        }

        public static bool IsInstalled(PluginInfo pluginInfo)
        {
            return Plugins.Any(x => x.SystemName == pluginInfo.SystemName);
        }

    }
}