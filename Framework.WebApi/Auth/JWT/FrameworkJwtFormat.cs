namespace Framework.WebApi.Auth.JWT
{
    using System;
    using System.IdentityModel.Tokens;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.DataHandler.Encoder;

    public class FrameworkJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string jwtIssuer;
        private readonly string jwtSecret;

        public FrameworkJwtFormat(string jwtIssuer, string jwtSecret)
        {
            this.jwtIssuer = jwtIssuer;
            this.jwtSecret = jwtSecret;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var issued = data.Properties.IssuedUtc;
            if (!issued.HasValue)
            {
                throw new InvalidOperationException("AuthenticaionTicket.Properties.IssuedUtc should be set");
            }

            var expires = data.Properties.ExpiresUtc;
            if (!expires.HasValue)
            {
                throw new InvalidOperationException("AuthenticaionTicket.Properties.ExpiresUtc should be set");
            }

            var jwtSecretByteArray = TextEncodings.Base64Url.Decode(this.jwtSecret);

            var signingCredentials = new SigningCredentials(new InMemorySymmetricSecurityKey(jwtSecretByteArray),
                SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var token = new JwtSecurityToken(this.jwtIssuer, Startup.AllowedAudience, data.Identity.Claims, issued.Value.UtcDateTime,
                expires.Value.UtcDateTime, signingCredentials);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}