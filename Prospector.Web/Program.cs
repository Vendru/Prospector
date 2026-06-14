using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Prospector.Web.Components;
using Prospector.Web.Data;
using Prospector.Web.Endpoints;
using Prospector.Web.Models;
using Prospector.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Blazor (Server + WASM)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();

// EF Core + SQLite
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? "Data Source=prospector.db";
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(connStr));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
{
    o.SignIn.RequireConfirmedAccount = false;
    o.Password.RequireDigit = true;
    o.Password.RequiredLength = 6;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Strict;
    o.ExpireTimeSpan = TimeSpan.FromDays(14);
    o.SlidingExpiration = true;
    o.LoginPath = "/account/login";
    o.LogoutPath = "/account/logout";
});

// MudBlazor
builder.Services.AddMudServices();

// Domain services
builder.Services.AddScoped<IPlayerService, PlayerDbService>();

// CFBD API integration
var cfbdKey = builder.Configuration["Cfbd:ApiKey"] ?? "";
builder.Services.AddHttpClient<Prospector.Web.Services.CfbdService>(client =>
{
    client.BaseAddress = new Uri("https://api.collegefootballdata.com/");
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cfbdKey);
});

// HttpClient for server-side prerendering of WASM components
builder.Services.AddScoped<Prospector.Web.Client.Services.PlayerApiService>();
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<IHttpContextAccessor>();
    var ctx = nav.HttpContext!;
    var scheme = ctx.Request.Scheme;
    var host = ctx.Request.Host.Value;
    return new HttpClient { BaseAddress = new Uri($"{scheme}://{host}") };
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Map API endpoints
app.MapPlayerEndpoints();
app.MapAuthEndpoints();
app.MapDashboardEndpoints();
app.MapCfbdEndpoints();

// Map Blazor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Prospector.Web.Client.Components.Pages.Dashboard).Assembly);

app.Run();
