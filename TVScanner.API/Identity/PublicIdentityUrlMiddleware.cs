using Duende.IdentityServer.Services;

namespace TVScanner.API.Identity
{
    /// <summary>
    /// theproduction versaion of this app is behind a reverse proxy, 
    /// so we need to set the public facing uri as the base uri for the identity server
    /// </summary>
    public class PublicIdentityUrlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _publicFacingUri;

        public PublicIdentityUrlMiddleware(RequestDelegate next, string publicFacingUri)
        {
            _publicFacingUri = publicFacingUri;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var request = context.Request;
            context.RequestServices.GetRequiredService<IServerUrls>().Origin = _publicFacingUri;
            context.RequestServices.GetRequiredService<IServerUrls>().BasePath = request.PathBase.Value!.TrimEnd('/');

            await _next(context);
        }
    }
}
