using System.ComponentModel.DataAnnotations;

namespace mobSocial.WebApi.Models.Plugin
{
    public class PluginInfoModel
    {
        [Required]
        public string SystemName { get; set; }
    }
}