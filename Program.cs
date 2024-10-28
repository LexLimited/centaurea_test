using System.Diagnostics;
using System.Text.Json.Serialization;
using CentaureaTest.Auth;
using CentaureaTest.Converters;
using CentaureaTest.Data;
using CentaureaTest.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new DataGridFieldSignatureConverter());
        options.JsonSerializerOptions.Converters.Add(new DataGridValueConverter());
    });
    
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
});

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
    {
        options.AccessDeniedPath = "/accessdenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.Name = "CentaureaCookie";
        options.Cookie.Path = "/";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.LoginPath = "/auth/login";
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.AccessDeniedPath = "/accessdenied";
        options.LogoutPath = "/auth/logout";
    });

builder.Services.AddLogging();
builder.Services.AddResponseCompression();

builder.Services.AddDbContext<ApplicationDbContext>(options =>{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Seed the roles and the superuser
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await AuthDbContext.SeedRoleAndSuperuser(serviceProvider);
}

app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseMiddleware<StaticFilesCompressionMiddleware>();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/app"))
    .ExcludeFromDescription();

Process? p = null;
if (app.Environment.IsDevelopment())
{
    var PlatformIsWindows = OperatingSystem.IsWindows();
    p = Process.Start(new ProcessStartInfo()
    {
        FileName = PlatformIsWindows ? "cmd" : "pnpm",
        Arguments = $"{(PlatformIsWindows ? "/c pnpm " : "")}vite",
        WorkingDirectory = "ClientApp",
        RedirectStandardError = true,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        UseShellExecute = false
    });
}
else
{
    app.UseDefaultFiles(new DefaultFilesOptions { RequestPath = "/app" });
    app.UseStaticFiles(new StaticFileOptions { RequestPath = "/app" });
    app.MapFallbackToFile("index.html");
}

app.MapReverseProxy();

app.Run();

p?.Kill();
