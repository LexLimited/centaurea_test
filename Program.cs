using System.Diagnostics;
using System.Text.Json.Serialization;
using CentaureaTest.Converters;
using CentaureaTest.Data;
using CentaureaTest.Middlewares;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new DataGridFieldSignatureConverter());
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
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/account/login";
        options.LogoutPath = "/account/logout";
    });

builder.Services.AddLogging();
builder.Services.AddResponseCompression();
builder.Services.AddDbContext<ApplicationDbContext>(options =>{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
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
