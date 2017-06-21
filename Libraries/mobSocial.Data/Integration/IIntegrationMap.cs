using System.Collections.Generic;

namespace mobSocial.Data.Integration
{
    public interface IIntegrationMap<T>
    {
        string SourceTableName { get; }

        Dictionary<string, string> SourceToDestinationColumnMapping { get; }

        string WhereString { get; }
    }
}