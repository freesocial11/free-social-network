using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using mobSocial.WebApi.Configuration.Mvc.UI;

namespace mobSocial.WebApi.Configuration.Mvc.Results
{
    public class HtmlActionResult : IHttpActionResult
    {
        private const string ViewDirectory = @"~/Views/";
        private readonly string _viewName;
        private readonly object _model;

        public HtmlActionResult(string viewName, object model = null)
        {
            _viewName = ViewDirectory + viewName + ".cshtml";
            _model = model;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var content = ViewRenderer.RenderView(_viewName, _model);
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
        }
    }
}