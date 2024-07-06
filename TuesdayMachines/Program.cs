using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Threading.RateLimiting;
using TuesdayMachines.Api;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Services;
using TuesdayMachines.WebSockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery(options =>
{
    options.FormFieldName = "csrfToken";
    options.HeaderName = "X-Csrf-Token-Value";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("pl") };
    options.DefaultRequestCulture = new RequestCulture("en-US", "en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
});

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllersWithViews().AddViewLocalization().AddDataAnnotationsLocalization();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<IMayanGame, MayanGameService>();
builder.Services.AddSingleton<IPlinkoGame, PlinkoGameService>();
builder.Services.AddSingleton<IMinesGame, MinesGameService>();
builder.Services.AddSingleton<IJwtTokenHandler, JwtTokenHandlerService>();
builder.Services.AddSingleton<IAccountsRepository, AccountsRepositoryService>();
builder.Services.AddSingleton<ITwitchApi, TwitchApiService>();
builder.Services.AddSingleton<IUserAuthentication, UserAuthenticationService>();
builder.Services.AddSingleton<IBroadcastersRepository, BroadcastersRepositoryService>();
builder.Services.AddSingleton<IUserFairPlay, UserFairPlayService>();
builder.Services.AddSingleton<IPointsRepository, PointsRepositoryService>();
builder.Services.AddSingleton<ISpinsRepository, SpinsRepositoryService>();
builder.Services.AddSingleton<IGamesRepository, GamesRepositoryService>();
builder.Services.AddSingleton<LiveRouletteService>();
builder.Services.AddSingleton<WebSocketRouletteHandler>();

builder.Services.AddHostedService<ConfigureMongoDbService>();
builder.Services.AddHostedService<LiveRouletteBackgroundService>();
builder.Services.AddHostedService<DatabaseCleanerService>();
builder.Services.AddHostedService<WatchTimeUpdateService>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("spins", httpContext => RateLimitPartition.GetTokenBucketLimiter(partitionKey: httpContext.Connection.RemoteIpAddress.ToString(), factory: _ => new TokenBucketRateLimiterOptions
    {
        TokenLimit = 10,
        TokensPerPeriod = 2,
        ReplenishmentPeriod = TimeSpan.FromSeconds(1)
    }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Headers.TryGetValue("X-Real-IP", out var ip))
    {
        ctx.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ip[0]);
    }

    await next();
});

app.UseStaticFiles();

app.UseRequestLocalization();

app.UseRouting();

app.UseAntiforgery();
app.MapGameEndpoints();
app.MapMayanEndpoints();
app.MapPlinkoEndpoints();
app.MapMinesEndpoints();

app.UseAuthorization();

app.UseRateLimiter();

app.UseWebSockets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}");

app.Run();
