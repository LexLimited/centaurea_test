using System.Diagnostics;
using centaurea_test.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
});

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddLogging();
builder.Services.AddResponseCompression();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseMiddleware<StaticFilesCompressionMiddleware>();

app.UseStaticFiles();

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
