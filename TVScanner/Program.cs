using Majorsoft.Blazor.Components.CssEvents;
using Majorsoft.Blazor.Components.Notifications;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TVScanner;
using TVScanner.Services;
using TVScanner.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = new Uri(builder.Configuration.GetValue<string>(Constants.ServerUrl)!);
builder.Services.AddHttpClient(Constants.ServerUrl, client => client.BaseAddress = apiUrl)
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.ServerUrl));

builder.Services.AddApiAuthorization(options =>
{
    options.ProviderOptions.ConfigurationEndpoint = new Uri(apiUrl, "_configuration/TVScanner.Client").ToString();
});

// notifications
builder.Services.AddCssEvents();
builder.Services.AddNotifications();

builder.Services.AddSingleton(sp => new NotificationService(
    sp.GetRequiredService<IHtmlNotificationService>(),
    sp.GetRequiredService<NavigationManager>()));

// signalr
builder.Services.AddSingleton(sp => new SignalRService(
    sp.GetRequiredService<NavigationManager>(),
    sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.ServerUrl),
    sp.GetRequiredService<IAccessTokenProvider>()
    ));

await builder.Build().RunAsync();
