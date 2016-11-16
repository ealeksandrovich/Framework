namespace Framework.WebApi.Auth.JWT
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Domain.Entitities;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.OAuth;

    public class FrameworkAuthorizationProvider: OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //validate user and password
            //var isValid = userService.ValidateCredentials(context.UserName, context.Password);
            //if (!isValid)
            //{
            //    context.SetError("invalid_grant", "The user name or password is incorrect.");
            //    return Task.FromResult<object>(null);
            //}

            //var user = userService.GetByEmail(context.UserName);
            var user = new UserEntity();

            var ticket = CreateAuthTicket(user);
            context.Validated(ticket);

            return Task.FromResult<object>(null);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var userIdClaim = context.Ticket.Identity.Claims.FirstOrDefault(i => i.Type == "sub");
            long userId;
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out userId))
            {
                context.SetError("invalid_grant", "The refresh token is invalid.");
                return Task.FromResult<object>(null);
            }

            //var user = userService.GetById(userId);

            //if (user == null)
            //{
            //    context.SetError("invalid_grant", "The refresh token is invalid.");
            //    return Task.FromResult<object>(null);
            //}

            var ticket = CreateAuthTicket(new UserEntity());
            context.Validated(ticket);

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Creates authentication ticket.
        /// http://self-issued.info/docs/draft-ietf-oauth-json-web-token.html#Claims
        /// The “sub” (subject) and “unique_claim” claims represent the user name this token issued for.
        /// The “role” claim represents the roles for the user.
        /// The “iss” (issuer) claim represents the Authorization server (Token Issuer) party.
        /// The “aud” (audience) claim represents the recipients that the JWT is intended for (Relying Party – Resource Server). More on this unique string later in this post.
        /// The “exp” (expiration time) claim represents the expiration time of the JWT, this claim contains UNIX time value.
        /// The “nbf” (not before) claim represent the time which this JWT must not be used before, this claim contains UNIX time vale.
        /// </summary>
        private static AuthenticationTicket CreateAuthTicket(UserEntity user)
        {
            var identity = new ClaimsIdentity("JWT");

            identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));
            identity.AddClaim(new Claim("sub", user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));

            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                {
                    "audience", Startup.AllowedAudience
                }
            });

            return new AuthenticationTicket(identity, properties);
        }
    }
}
