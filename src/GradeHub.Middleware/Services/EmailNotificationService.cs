using Polly;
using Polly.Retry;
using Resend;

namespace GradeHub.Middleware.Services;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly IConfiguration _config;
    private readonly IResend _resend;

    public EmailNotificationService(ILogger<EmailNotificationService> logger, IConfiguration config, IResend resend)
    {
        _logger = logger;
        _config = config;
        _resend = resend;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(5),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning($"Resend call failed (Attempt {retryCount}/3). Waiting 5s... Error: {exception.Message}");
                });
    }

    public async Task SendGradeNotificationAsync(string studentEmail, string courseName, string gradeValue)
    {
        var senderEmail = _config["Resend:SenderEmail"] ?? "noreply@gradehub.local";

        await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation($"Attempting to send email to {studentEmail} via Resend...");

            var message = new EmailMessage
            {
                From = senderEmail,
                Subject = $"Grade recorded for {courseName}",
                TextBody = $"Your grade for {courseName} has been officially recorded: {gradeValue}"
            };
            message.To.Add(studentEmail);

            await _resend.EmailSendAsync(message);

            _logger.LogInformation($"Email sent successfully to {studentEmail}");
        });
    }
}
