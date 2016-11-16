namespace Framework.WebApi.Auth.JWT
{
    using System.IdentityModel.Claims;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Owin.Security.OAuth;

    /// <summary>
    /// http://stackoverflow.com/questions/25097221/how-to-authenticate-a-access-token-by-owin-oauthbearerauthentication
    /// </summary>
    public class FrameworkAuthenticationProvider: OAuthBearerAuthenticationProvider
    {
        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            var identity = context.Ticket?.Identity;
            var userIdClaim = identity?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                context.Rejected();
            }
            
            return base.ValidateIdentity(context);
        }

    }
}