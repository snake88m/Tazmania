using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Tazmania.Entities;
using Tazmania.Interfaces.Services;
using Tazmania.Helpers;

namespace Tazmania.WebService.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        readonly IDatabankService _databankService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IDatabankService databankService)
            : base(options, logger, encoder, clock)
        {
            _databankService = databankService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }

            User? user;
            string[] credentials;

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Invalid Authorization Header {Request.Headers["Authorization"]}");
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            user = await _databankService.FetchUser(credentials[0], credentials[1]);

            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid User");
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
