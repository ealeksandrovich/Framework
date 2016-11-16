namespace Framework.WebApi
{
    using System;
    using System.Configuration;
    using System.Web.Http;
    using Auth.BasicAuth;
    using Auth.JWT;
    using Microsoft.Owin;
    using Microsoft.Owin.Cors;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Jwt;
    using Microsoft.Owin.Security.OAuth;
    using Owin;
    using Swashbuckle.Application;

    public partial class Startup
    {
        public const string AllowedAudience = "099153c2625149bc8ecb3e85e03f001";

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

            //httpConfig.MessageHandlers.Add(new BasicAuthHandler());

            // JSON Web Token in ASP.NET Web API 2 using Owin (step by step tutorial)
            // http://bitoftech.net/2014/10/27/json-web-token-asp-net-web-api-2-jwt-owin-authorization-server/
            //var jwtIssuer = "http://localhost";
            //var jwtSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw";


            appBuilder.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth2/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new FrameworkAuthorizationProvider(),
                AccessTokenFormat = new FrameworkJwtFormat(ConfigurationManager.AppSettings["jwtIssuer"],
                        ConfigurationManager.AppSettings["jwtSecret"])
            });

            appBuilder.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                Provider = new FrameworkAuthenticationProvider(),
                AllowedAudiences = new[] { AllowedAudience },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
               {
                    new SymmetricKeyIssuerSecurityTokenProvider(ConfigurationManager.AppSettings["jwtIssuer"],
                        ConfigurationManager.AppSettings["jwtSecret"])
               }
            });

            appBuilder.UseWebApi(httpConfig);
        }
    }
}
