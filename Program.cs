using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
             

// Add services to the container.
builder.Services.AddRazorPages(); /* .AddRazorPagesOptions( options =>
            {
                options.Conventions
                       .ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            } *); */
builder.Services.AddServerSideBlazor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
    options => {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.MaxAge = options.ExpireTimeSpan; // optional
        options.SlidingExpiration = true;
    }
);
/* .AddCookie( options => {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.IsEssential = true;
});

/*builder.Services.AddSession(options => {
     options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
}); */

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureApplicationCookie( options =>
{
    options.LoginPath = "/login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.MaxAge = options.ExpireTimeSpan; // optional
    options.SlidingExpiration = true;

    /* options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;

    options.Cookie.Name = "helloWebApp";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict; */
});

builder.Services.AddHttpClient("helloAPI", client => {
    client.BaseAddress = new Uri(configuration["API_URL"]);
 } );

 builder.Services.AddTransient<IApiClient, ApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapBlazorHub();

app.Run();
