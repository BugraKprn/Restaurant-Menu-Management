using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NLog.Web;
using OrionDAL.Web;
using RestaurantMenu.Library.Manager;
using RestaurantMenu.Library.Services;
using System.Text;

var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
logger.Debug("init main");
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    ConfigurationManager configuration = builder.Configuration;
    IWebHostEnvironment environment = builder.Environment;

    OrionWebApplication orionWebApplication = new OrionWebApplication(configuration);
    orionWebApplication.DbSynchronization = true;
    orionWebApplication.Initialize();

    // Add services to the container.
    builder.Services.AddOptions();
    builder.Services.AddScoped<IProductService, ProductManager>();
    builder.Services.AddScoped<ICategoryService, CategoryManager>();
    builder.Services.AddScoped<ISalesService, SalesManager>();
    builder.Services.AddSingleton<IConfiguration>(configuration);
    builder.Services.AddControllersWithViews();
    builder.Services.AddControllersWithViews().AddNewtonsoftJson(o => o.SerializerSettings.ContractResolver = new DefaultContractResolver());
    var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("K2P3R4N0Y2hlciUyMHdvbmclMjBsb3ZlJTIwLm5ldA=="));
    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = true,
        ValidIssuer = "localhost",
        ValidateAudience = true,
        ValidAudience = "Orion-Arkyazýlým",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true,
    };


    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
    {
        option.LoginPath = "/login";
    });
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    var provider = new FileExtensionContentTypeProvider();
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
                 Path.Combine(Environment.CurrentDirectory, "StaticFiles")),
        RequestPath = new PathString("/StaticFiles"),
        ContentTypeProvider = provider
    });

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=en}/{action=Index}/{id?}");

    app.Run();

}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
