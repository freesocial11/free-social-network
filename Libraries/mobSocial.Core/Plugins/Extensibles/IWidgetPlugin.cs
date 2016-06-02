using System.Collections.Generic;

namespace mobSocial.Core.Plugins.Extensibles
{
    /// <summary>
    /// Interface for creating a widget plugin
    /// </summary>
    public interface IWidgetPlugin : IPlugin
    {
        IList<string> GetWidgetLocations();
    }
}