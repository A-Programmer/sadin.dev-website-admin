using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IdentityModel;
using IdentityModel.Client;
using KSBlazor.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Website.BlazorApp.Handlers;
using Website.BlazorApp.Services.Contracts.GeneralContracts;
using Website.BlazorApp.Services.Implementations.GeneralServices;

var builder = WebApplication.CreateBuilder(args);
IConfiguration Configuration;

if (builder.Environment.IsProduction())
{
    Configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();
}
else
{
    Configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.Development.json")
        .Build();
}

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddComponents();

//TODO : Refactor later

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICookieServices, CookieServices>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IGeneralHttpServices, GeneralHttpServices>();

//Registering HttpClient to access Website Api
builder.Services.AddTransient<AuthenticationDelegatingHandler>();
builder.Services.AddHttpClient("WebsiteApi", client =>
{
    client.BaseAddress = new Uri(Configuration.GetValue<string>("Resources:WebsiteApi:BaseAddress"));
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
}).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

#region Registering HttpClient to access IDP
builder.Services.AddHttpClient("IdPClient", client =>
{
    client.BaseAddress = new Uri(Configuration.GetValue<string>("IdentityConfigs:Authority"));
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});
// Adding Client Credentials
builder.Services.AddSingleton(new ClientCredentialsTokenRequest()
{
    Address = Configuration.GetValue<string>("IdentityConfigs:TokenRequestAddress"),
    ClientId = Configuration.GetValue<string>("IdentityConfigs:ClientId"),
    ClientSecret = Configuration.GetValue<string>("IdentityConfigs:ClientSecret"),
    GrantType = Configuration.GetValue<string>("IdentityConfigs:ResponseType"), //OidcConstants.GrantTypes.ClientCredentials,
    Scope = "Website.api"
});
#endregion 

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
    }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        //TODO : Remove on Production
        options.RequireHttpsMetadata = false;
        
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        options.Authority = Configuration.GetValue<string>("IdentityConfigs:Authority"); // Ids URL
        options.ClientId = Configuration.GetValue<string>("IdentityConfigs:ClientId-Login");
        options.ClientSecret = Configuration.GetValue<string>("IdentityConfigs:ClientSecret");
        options.ResponseType = Configuration.GetValue<string>("IdentityConfigs:ResponseType-Login");
        
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("Website.api");

        options.SaveTokens = true;

        options.GetClaimsFromUserInfoEndpoint = true;
        
        options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Role,"role");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = JwtClaimTypes.Role
        };
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // app.UseExceptionHandler("/Error");
    // app.UseHsts();
}


app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapFallbackToPage("{*path:nonfile}", "/_WebsiteHost")
        .Add(b => ((RouteEndpointBuilder)b).Order = int.MaxValue - 1);
    endpoints.MapFallbackToPage("~/Dashboard/{*clientroutes:nonfile}", "/_DashboardHost")
        .Add(b => ((RouteEndpointBuilder)b).Order = int.MaxValue - 2);
});

app.MapBlazorHub();
app.MapFallbackToPage("/_DashboardHost");

app.Run();
