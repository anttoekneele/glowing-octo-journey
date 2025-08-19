using Capstone.UI.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.IdentityModel.Tokens;
using Okta.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect("OpenIdConnect", options =>
{
    options.Authority = builder.Configuration["Okta:OktaDomain"] + "/oauth2/" + builder.Configuration["Okta:AuthorizationServerId"];
    options.ClientId = builder.Configuration["Okta:ClientId"];
    options.ClientSecret = builder.Configuration["Okta:ClientSecret"];
    options.ResponseType = "code";
    options.SaveTokens = true;

    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "groups"
    };

    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = ctx =>
        {
            var accessToken = ctx.SecurityToken.RawData;
            Console.WriteLine("ACCESS TOKEN: " + accessToken);

            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();
builder.Services.AddRazorPages();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddHttpClient("Api", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseAddress = configuration["BaseAddress"];

    if (string.IsNullOrWhiteSpace(baseAddress))
        throw new InvalidOperationException("Api is not configured in appsettings.json.");

    client.BaseAddress = new Uri(baseAddress);
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
