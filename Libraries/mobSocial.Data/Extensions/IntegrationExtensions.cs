using System.Collections.Generic;
using System.Linq;
using System.Text;
using mobSocial.Core.Exception;
using mobSocial.Data.Database.Attributes;
using mobSocial.Data.Integration;

namespace mobSocial.Data.Extensions
{
    public static class IntegrationExtensions
    {
        public static string GetView<T>(this IIntegrationMap<T> map)
        {
            //check if entity uses views at runtime e.g. User table
            var runTimeViewAttribute = typeof(T).GetCustomAttributes(typeof(ToRunTimeViewAttribute), true)
                .FirstOrDefault() as ToRunTimeViewAttribute;
            if (runTimeViewAttribute == null)
            {
                throw new mobSocialException("Can't generate view for empty view name");
            }
            var viewName = runTimeViewAttribute.ViewName;
            const string viewQuery = "CREATE VIEW {0} AS SELECT {1} FROM {2} {3} GO";

            var selectBuilder = new List<string>();
            foreach (var sourceDestinationPair in map.SourceToDestinationColumnMapping)
            {
                selectBuilder.Add($"[{sourceDestinationPair.Key}] AS [{sourceDestinationPair.Value}]");
            }

            var select = string.Join(",", selectBuilder);
            var where = !string.IsNullOrEmpty(map.WhereString) ? $"WHERE {map.WhereString}" : "";

            var view = string.Format(viewQuery, viewName, select, map.SourceTableName, where);
            return view;

        }
    }
}