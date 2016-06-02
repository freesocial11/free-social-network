using System.Collections.Generic;

namespace mobSocial.WebApi.Configuration.Mvc.Models
{
    public class RootResponseModel : RootModel
    {
        public bool Success { get; set; }

        public Dictionary<string, List<string>> ErrorMessages { get; set; }

        public Dictionary<string, List<string>> Messages { get; set; }

        public dynamic ResponseData { get; set; }
    }
}