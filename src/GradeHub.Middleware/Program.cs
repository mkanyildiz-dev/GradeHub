using GradeHub.Middleware.Models;
using GradeHub.Middleware.Services;
using Resend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(options =>
{
    options.ApiToken = builder.Configuration["Resend:ApiKey"] ?? string.Empty;
});
builder.Services.AddTransient<IResend, ResendClient>();
builder.Services.AddControllers();
builder.Services.AddTransient<ICisSoapClient, CisSoapClient>();
builder.Services.AddTransient<IEmailNotificationService, EmailNotificationService>();
builder.Services.AddTransient<IGradeProcessingService, GradeProcessingService>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
