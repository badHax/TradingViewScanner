using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace TVScanner.API.Hubs
{
    public abstract class MessageHubBase<T> : Hub<T> where T : class
    {
        public new virtual async Task OnConnectedAsync()
        {
            await HandleExpiredJwtToken();

            await base.OnConnectedAsync();
        }

        public async Task HandleExpiredJwtToken()
        {
            var feature = Context.Features.Get<IConnectionHeartbeatFeature>();
            if (feature == null)
            {
                return;
            }

            var context = Context.GetHttpContext();
            if (context == null)
            {
                throw new InvalidOperationException("The HTTP context cannot be resolved.");
            }

            // Extract the authentication ticket from the access token.
            // Note: this operation should be cheap as the authentication result
            // was already computed when SignalR invoked the authentication handler
            // and automatically cached by AuthenticationHandler.AuthenticateAsync().
            var result = await context.AuthenticateAsync("IdentityServerJwt");
            if (result.Ticket == null)
            {
                Context.Abort();

                return;
            }

            feature.OnHeartbeat(state =>
            {
                var (ticket, connection) = ((AuthenticationTicket, HubConnectionContext))state;

                // Ensure the access token token is still valid.
                // If it's not, abort the connection immediately.
                if (ticket.Properties.ExpiresUtc < DateTimeOffset.UtcNow)
                {
                    connection.Abort();
                }
            }, (result.Ticket, Context));
        }
    }
}
