using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using TVScanner.API.Extensions;
using TVScanner.API.Hubs;
using TVScanner.API.Identity;
using TVScanner.Data;
using TVScanner.Data.AccessProvider;
using TVScanner.Jobs;
using TVScanner.Shared;
using TVScanner.Shared.Configuration;
using TVScanner.Shared.Logging;
using TVScanner.Shared.Notifications;
using TVScanner.Shared.Scanner;
using static TVScanner.API.Extensions.SignalRExtensions;

var apiAppName = "TVScanner.API";
var dbContextString = "TVScannerContext";

var builder = WebApplication.CreateBuilder(args);

// configuration
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddOptions<AppConfig>()
    .Bind(builder.Configuration.GetSection("AppConfig"))
    .ValidateDataAnnotations();

var clientApp = builder.Configuration["ClientApp:Host"] ?? throw new InvalidOperationException("ClientApp not found");
var clientAppName = builder.Configuration["ClientApp:Name"] ?? throw new InvalidOperationException("ClientAppName not found");
var identityAuthority = builder.Configuration["IdentityServer:Authority"] ?? throw new InvalidOperationException("IdentityServer:Authority not found");

// named http clients
builder.Services.AddHttpClient(nameof(NotificationService), client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AppConfig:NotificationConfig:PushUrlBase"]!);
});
builder.Services.AddHttpClient(nameof(ScanService), client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AppConfig:ScannerConfig:Url"]!);
});

// database
var connectionString = builder.Configuration.GetConnectionString(dbContextString)
    ?? throw new InvalidOperationException($"Connection string '{dbContextString}' not found.");

builder.Services.AddDbContextFactory<TVScannerContext>(options => options.UseNpgsql(connectionString));

// authentication
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<TVScannerContext>();
builder.Services
    .AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, TVScannerContext>(config =>
    {
        var allowedScopes = new string[] { "openid", "profile", apiAppName };
        config.IdentityResources.Add(new IdentityResource(apiAppName, allowedScopes));
        config.Clients.AddSPA(clientAppName, options =>
        {
            options.WithRedirectUri($"{clientApp}/authentication/login-callback")
                .WithLogoutRedirectUri($"{clientApp}/authentication/logout-callback")
                .WithClientId(clientAppName)
                .WithScopes(allowedScopes);
        });
    });

builder.Services.AddTransient<IIssuerNameService, CustomIssuerNameService>();

builder.Services
    .AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<TVScannerContext>();

builder.Services.AddSingleton<IScanService, ScanService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddScoped<IScanRecordManager, ScanRecordManager>();
builder.Services.AddScoped<ITaskDelayer, TaskDelayer>();
builder.Services.AddScoped<IAbstractLogger, ApplicationLogger>();

// add background services
builder.Services.AddHostedService<HighOfDayScanner>();
builder.Services.AddHostedService<RelativeVolumeScanner>();
builder.Services.AddHostedService<PurgeOldRecords>();

// storage
builder.Services.AddScoped<IRepositoryContext, RepositoryContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICache, InMemoryCache>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// cors
builder.Services.AddSingleton<ICorsPolicyService>((container) =>
{
    var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
    return new DefaultCorsPolicyService(logger)
    {
        AllowedOrigins = { clientApp, "https://localhost:7105" },
    };
});

builder.Services.AddSignalRMessages();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

var app = builder.Build();

app.MigrateDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(b => b
    .WithOrigins(clientApp, "https://localhost:7105")
       .AllowAnyHeader()
          .AllowAnyMethod()
             .AllowCredentials());

app.UseMiddleware<PublicIdentityUrlMiddleware>(identityAuthority);

app.UseStaticFiles();

//app.UseHttpsRedirection(); // running on docker, behind a proxy so https is configured there

app.UseRouting();
app.UseResponseCaching();

app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.MapHub<ScannerHub>("/scannerHub");
app.Services.GetRequiredService<SignalRMessageBuilder>(); // for instantiating SignalRMessageBuilder

app.Run();
