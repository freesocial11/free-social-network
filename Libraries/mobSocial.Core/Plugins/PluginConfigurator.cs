using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace mobSocial.Core.Plugins
{
    public class PluginConfigurator
    {
        public class ReadPluginInfo
        {
            public string SystemName { get; set; }

            public bool Installed { get; set; }

            public bool Active { get; set; }
        }

        public static void WriteConfigFile(IList<PluginInfo> plugins, string configFilePath)
        {
            //in the config files, only installed plugins are written
            var installedPlugins = plugins.Where(x => x.Installed);

            var fileStream = new FileStream(configFilePath, FileMode.Create);

            var xmlWriterSettings = new XmlWriterSettings { Indent = true };

            var xmlWriter = XmlTextWriter.Create(fileStream, xmlWriterSettings);

            //<!?xml>
            xmlWriter.WriteStartDocument();

            //<plugins>
            xmlWriter.WriteStartElement("plugins");

            foreach (var im in installedPlugins)
            {
                //<plugin>
                xmlWriter.WriteStartElement("plugin");

                //attribute active
                xmlWriter.WriteAttributeString("active", im.Active.ToString());

                //attribute installed
                xmlWriter.WriteAttributeString("installed", im.Installed.ToString());

                //plugin name
                xmlWriter.WriteString(im.SystemName);

                //</plugin>
                xmlWriter.WriteEndElement();
            }

            //</plugins>
            xmlWriter.WriteEndElement();

            //close the writer
            xmlWriter.Close();

            //close the stream
            fileStream.Close();

        }

        public static IEnumerable<ReadPluginInfo> ReadConfigFile(string configFilePath)
        {
            var fileFound = File.Exists(configFilePath);
            if (!fileFound)
                return null;

            var readPlugins = new List<ReadPluginInfo>();


            //read the file stream
            var fileStream = new FileStream(configFilePath, FileMode.Open);
            //create the reader
            var xmlReader = XmlReader.Create(fileStream);

            ReadPluginInfo currentInfo = null;

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "plugins")
                            continue;
                        currentInfo = new ReadPluginInfo()
                        {
                            Active = Convert.ToBoolean(xmlReader.GetAttribute("active")),
                            Installed = Convert.ToBoolean(xmlReader.GetAttribute("installed"))
                        };
                        break;
                    case XmlNodeType.Text:
                        if (currentInfo != null)
                            currentInfo.SystemName = xmlReader.Value;
                        break;
                    case XmlNodeType.EndElement:
                        readPlugins.Add(currentInfo);
                        currentInfo = null;
                        break;
                }
            }
            //close the stream
            fileStream.Close();

            return readPlugins;
        }

        public static PluginInfo LoadPluginInfo(string fileName)
        {
            //because this code fires before app startup, we won't be able to use any resolver to read the files
            //TODO: find a better way to do this
            var fileFound = File.Exists(fileName);

            if (!fileFound)
                throw new FileNotFoundException();


            var pluginInfo = new PluginInfo();
            //read the file stream
            var fileStream = new FileStream(fileName, FileMode.Open);
            //create the reader
            var xmlReader = XmlReader.Create(fileStream);
            var currentConfigName = string.Empty;//this stores the element name at any point of time
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name.ToLower() == "config")
                            continue;
                        currentConfigName = xmlReader.Name.ToLower();
                        break;
                    case XmlNodeType.Text:
                        if (currentConfigName != string.Empty)
                        {
                            var value = xmlReader.Value;
                            switch (currentConfigName)
                            {
                                case "name":
                                    pluginInfo.Name = value;
                                    break;
                                case "systemname":
                                    pluginInfo.SystemName = value;
                                    break;
                                case "supportedversions":
                                    pluginInfo.SupportedVersions = value.Split(',').ToList();
                                    break;
                                case "author":
                                    pluginInfo.Author = value;
                                    break;
                                case "authoruri":
                                    pluginInfo.AuthorUri = value;
                                    break;
                                case "pluginuri":
                                    pluginInfo.PluginUri = value;
                                    break;
                                case "assemblydllname":
                                    pluginInfo.AssemblyDllName = value;
                                    break;
                                case "issystemplugin":
                                    pluginInfo.IsSystemPlugin = Convert.ToBoolean(value);
                                    break;
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        currentConfigName = string.Empty;
                        break;
                }
            }
            //close the stream
            fileStream.Close();

            //file info
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.DirectoryName != null)
            {
                var assemblyFilePath = Path.Combine(fileInfo.DirectoryName, pluginInfo.AssemblyDllName);
                pluginInfo.OriginalAssemblyFileInfo = new FileInfo(assemblyFilePath);
            }

            return pluginInfo;
        }

        //loads all the plugin info files
        public static IEnumerable<PluginInfo> GetAllPluginsInfos(DirectoryInfo probeDirectory, IList<ReadPluginInfo> readPlugins)
        {
            var pluginFiles = probeDirectory.GetFiles("plugin.config", SearchOption.AllDirectories);
            var plugins = pluginFiles.Select(file => LoadPluginInfo(file.FullName)).ToList();

            foreach (var plugin in plugins)
            {
                var readPlugin = readPlugins.FirstOrDefault(x => x.SystemName == plugin.SystemName);
                if (readPlugin == null) continue;
                //so this is not a new plugin
                plugin.Installed = readPlugin.Installed;
                plugin.Active = readPlugin.Active;
            }

            return plugins;
        }


    }
}