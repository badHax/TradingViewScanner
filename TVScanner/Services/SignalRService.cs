
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using TVScanner.Shared;

namespace TVScanner.Services
{
    public class SignalRService : IAsyncDisposable
    {
        private readonly NavigationManager navigationManager;
        private readonly HttpClient apiHttpClient;
        private readonly IAccessTokenProvider tokenProvider;
        private readonly ConcurrentBag<HubConnection> hubConnections = new();

        public SignalRService(
            NavigationManager navigationManager,
            HttpClient apiHttpClient,
            IAccessTokenProvider tokenProvider)
        {
            this.navigationManager = navigationManager;
            this.apiHttpClient = apiHttpClient;
            this.tokenProvider = tokenProvider;
        }

        public async Task ListenToScanner<T>(string scannerName, Action<T> callback)
            where T : class
        {
            var hubConnection = await CreateHubConnection(scannerName, Constants.ScannerHub, callback);
            await hubConnection.StartAsync();
            hubConnections.Add(hubConnection);
        }

        private async Task<HubConnection> CreateHubConnection<T>(string methodName, string endpoint, Action<T> callback)
            where T : class
        {
            var hubConnection = new HubConnectionBuilder()
               .WithUrl(new Uri(apiHttpClient.BaseAddress, endpoint), options =>
               {
                   options.AccessTokenProvider = GetAccessToken;
               })
               .AddJsonProtocol(cfg =>
               {
                   cfg.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
                   cfg.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
               })
               .WithAutomaticReconnect()
               .Build();

            hubConnection.On(methodName, callback);

            return hubConnection;
        }

        private async Task<string?> GetAccessToken()
        {
            var tokenResult = await tokenProvider.RequestAccessToken();
            AccessToken? token;
            tokenResult.TryGetToken(out token);
            return token?.Value;
        }

        public async ValueTask DisposeAsync()
        {
            if (!hubConnections.IsEmpty)
            {
                foreach (var connection in hubConnections)
                {
                    await connection.DisposeAsync();
                }
            }
        }
    }
}
