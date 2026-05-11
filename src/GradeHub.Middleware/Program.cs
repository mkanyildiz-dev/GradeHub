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
builder.Services.AddSingleton<CisSoapClient>();
builder.Services.AddTransient<EmailNotificationService>();

var app = builder.Build();

app.UseRouting();

// Map REST Endpoint
app.MapPost("/api/grades", async (GradeSubmission submission, CisSoapClient soapClient, EmailNotificationService emailService, ILogger<Program> logger) =>
{
    logger.LogInformation("Received grade submission via REST API for student: {StudentEmail}", submission.StudentEmail);

    try
    {
        // 1. Send to CIS Mock via SOAP
        var soapResult = await soapClient.StoreGradeAsync(submission.StudentEmail, submission.CourseName, submission.GradeValue);

        // 2. Send Email via SMTP
        if (soapResult == "SUCCESS")
        {
            await emailService.SendGradeNotificationAsync(submission.StudentEmail, submission.CourseName, submission.GradeValue);
        }

        return Results.Ok(new { message = "Grade processed successfully." });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to process grade submission.");
        return Results.Problem("An error occurred while processing the grade.", statusCode: 500);
    }
});

app.Run();
