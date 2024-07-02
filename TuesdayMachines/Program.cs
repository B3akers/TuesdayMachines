using TuesdayMachines.Interfaces;
using TuesdayMachines.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "session";
    options.IdleTimeout = TimeSpan.FromMinutes(15);
});

builder.Services.AddAntiforgery(options =>
{
    options.FormFieldName = "csrfToken";
    options.HeaderName = "X-Csrf-Token-Value";
});

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<IMayanGame, MayanGameService>();
builder.Services.AddSingleton<IAccountsRepository, AccountsRepositoryService>();
builder.Services.AddSingleton<ITwitchApi, TwitchApiService>();
builder.Services.AddSingleton<IUserAuthentication, UserAuthenticationService>();
builder.Services.AddSingleton<IBroadcastersRepository, BroadcastersRepositoryService>();
builder.Services.AddSingleton<IUserFairPlay, UserFairPlayService>();
builder.Services.AddSingleton<IPointsRepository, PointsRepositoryService>();
builder.Services.AddSingleton<ISpinsRepository, SpinsRepositoryService>();
builder.Services.AddSingleton<IGamesRepository, GamesRepositoryService>();

builder.Services.AddHostedService<ConfigureMongoDbService>();
builder.Services.AddHostedService<DatabaseCleanerService>();
builder.Services.AddHostedService<WatchTimeUpdateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseSession();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}");

app.Run();
