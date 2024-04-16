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

var apiUrl = builder.Configuration.GetValue<string>(Constants.ServerUrl);
builder.Services.AddHttpClient(Constants.ServerUrl, client => client.BaseAddress = new Uri(apiUrl!))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.ServerUrl));

builder.Services.AddApiAuthorization(options =>
{
    options.ProviderOptions.ConfigurationEndpoint = apiUrl + "/_configuration/TVScanner.Client";
});

// notifications
builder.Services.AddCssEvents();
builder.Services.AddNotifications();
builder.Services.AddScoped(sp => new NotificationService(
    sp.GetRequiredService<IHtmlNotificationService>(),
    sp.GetRequiredService<NavigationManager>()));

await builder.Build().RunAsync();
