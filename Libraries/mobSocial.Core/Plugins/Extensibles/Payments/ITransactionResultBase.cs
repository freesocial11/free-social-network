using System.Collections.Generic;

namespace mobSocial.Core.Plugins.Extensibles.Payments
{
    public interface ITransactionResultBase
    {
        bool Success { get; set; }

        Dictionary<string, object> ResponseParameters { get; set; }
    }
}