namespace Framework.WebApi.Auth.BasicAuth
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;

    public class BasicAuthHandler : DelegatingHandler
    {
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Basic";
        private const string RequestContextMessageProperty = "MS_RequestContext";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authService = request.GetDependencyScope().GetService(typeof(AuthService)) as AuthService;

            var context = request.Properties[RequestContextMessageProperty] as HttpRequestContext;

            if (authService != null && context != null)
            {
                //NOTE: can be changed to a custom header and so on.
                AuthenticationHeaderValue authValue = request.Headers.Authorization;

                if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter))
                {
                    var principal = authService.AuthenticateRequestBasic(authValue.Parameter);

                    if (principal != null)
                    {
                        context.Principal = principal;
                    }
                }
            }
            return base.SendAsync(request, cancellationToken).ContinueWith(ContinuationAction, cancellationToken);
        }

        private HttpResponseMessage ContinuationAction(Task<HttpResponseMessage> task)
        {
            if (task.Result.StatusCode == HttpStatusCode.Unauthorized && !task.Result.Headers.Contains(BasicAuthResponseHeader))
            {
                task.Result.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
            }
            return task.Result;
        }
    }
}
