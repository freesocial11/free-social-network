using System;

namespace mobSocial.Data.Attributes
{
    /// <summary>
    /// Specifies a field that can be used for token replacements processing
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TokenFieldAttribute : Attribute
    {
    }
}
