namespace Framework.WebApi
{
    using System.Web.Http;
    using Auth;
    using Microsoft.Owin.Cors;
    using Owin;
    using Swashbuckle.Application;

    public partial class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseRequestScopeContext();

            var httpConfig = new HttpConfiguration();

            //https://github.com/domaindrivendev/Swashbuckle
            httpConfig.EnableSwagger(c => c.SingleApiVersion("v1", "API")).EnableSwaggerUi();

            httpConfig.MapHttpAttributeRoutes();


            httpConfig.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            httpConfig.MessageHandlers.Add(new BasicAuthHandler());

            appBuilder.UseWebApi(httpConfig);
        }
    }
}
