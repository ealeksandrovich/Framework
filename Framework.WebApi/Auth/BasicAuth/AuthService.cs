namespace Framework.WebApi.Auth.BasicAuth
{
    using System;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Text;

    public class AuthService
    {
        public IPrincipal AuthenticateRequestBasic(string token)
        {
            //NOTE: logic for Basic auth via Username:Password
            string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(token)).Split(':');
            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0])
                || string.IsNullOrEmpty(credentials[1]))
                return null;

            var identity = new GenericIdentity(credentials[0]);

            return new ClaimsPrincipal(identity);
        }
    }
}
