using System.Web.Mvc;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Services.VerboseReporter;

namespace mobSocial.WebApi.Configuration.Mvc
{
    public abstract class RootController : Controller
    {
        private readonly IVerboseReporterService _verboseReporterService;

        protected RootController()
        {
            _verboseReporterService = mobSocialEngine.ActiveEngine.Resolve<IVerboseReporterService>();
        }

        protected IVerboseReporterService VerboseReporter
        {
            get { return _verboseReporterService; }
        }

      
    }
}