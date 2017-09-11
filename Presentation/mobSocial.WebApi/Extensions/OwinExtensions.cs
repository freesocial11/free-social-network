using Microsoft.Owin;

namespace mobSocial.WebApi.Extensions
{
    public static class OwinExtensions
    {
        public static bool IsApiEndPointRequest(this IOwinRequest request)
        {
            return request.Uri.AbsolutePath.StartsWith("/" + WebApiConfig.ApiPrefix)
                   && !request.Uri.AbsolutePath.Contains($"/signalr")
                   && !request.Uri.AbsolutePath.Contains($"/oauth2/");
        }

        public static bool IsSignalRRequest(this IOwinRequest request)
        {
            return request.Uri.AbsolutePath.Contains($"/signalr");
        }
    }
}