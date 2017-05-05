using System;
using mobSocial.Core.Exception;

namespace mobSocial.Data.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToRunTimeViewAttribute : Attribute
    {
        public string ViewName { get; set; }

        public ToRunTimeViewAttribute(string viewName)
        {
            if(string.IsNullOrEmpty(viewName))
                throw new mobSocialException("Can not map to an empty or null view name");
            ViewName = viewName;
        }
    }
}