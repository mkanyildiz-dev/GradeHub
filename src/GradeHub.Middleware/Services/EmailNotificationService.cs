using MailKit.Net.Smtp;
using MimeKit;
using Polly;
using Polly.Retry;

namespace GradeHub.Middleware.Services;

public class EmailNotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public EmailNotificationService(ILogger<EmailNotificationService> logger)
    {
        _logger = logger;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3, 
                retryAttempt => TimeSpan.FromSeconds(5), 
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning($"SMTP call failed (Attempt {retryCount}/3). Waiting 5s... Error: {exception.Message}");
                });
    }

    public async Task SendGradeNotificationAsync(string studentEmail, string courseName, string gradeValue)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation($"Attempting to send email to {studentEmail}...");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("GradeHub Integration", "noreply@gradehub.local"));
            message.To.Add(new MailboxAddress("Student", studentEmail));
            message.Subject = $"Grade recorded for {courseName}";

            message.Body = new TextPart("plain")
            {
                Text = $"Your grade for {courseName} has been officially recorded: {gradeValue}"
            };

            using var client = new SmtpClient();
            // Local SMTP like MailHog or Papercut on port 1025
            await client.ConnectAsync("localhost", 1025, false); 
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            _logger.LogInformation($"Email sent successfully to {studentEmail}");
        });
    }
}
