using System.ComponentModel.DataAnnotations;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Installation
{
    public class InstallationRequestModel : RootModel
    {
        public string ConnectionString { get; set; }

        [Required]
        public string AdminEmail { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string ConfirmPassword { get; set; }

        public string DatabaseName { get; set; }

        public string ServerUrl { get; set; }

        public bool CreateDatabaseIfNotExist { get; set; }

        public string DatabaseUserName { get; set; }

        public string DatabasePassword { get; set; }

        public bool IsConnectionString { get; set; }

        public bool InstallSampleData { get; set; }

    }
}