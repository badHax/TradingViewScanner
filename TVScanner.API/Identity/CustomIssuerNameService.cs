using Duende.IdentityServer.Hosting;
using Duende.IdentityServer.Services;

namespace TVScanner.API.Identity
{
    public class CustomIssuerNameService : IIssuerNameService
    {
        private readonly IConfiguration configuration;

        public CustomIssuerNameService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public Task<string> GetCurrentAsync()
        {
            return Task.FromResult(
                configuration["IdentityServer:Authority"] ??
                throw new InvalidOperationException("configuration IdentityServer:Authority is missing"));
        }
    }
}
