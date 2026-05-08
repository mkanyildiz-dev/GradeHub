using MailKit.Net.Smtp;
using MimeKit;
using Polly;
using Polly.Retry;

namespace GradeHub.Middleware.Services;

public class EmailNotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly IConfiguration _config;

    public EmailNotificationService(ILogger<EmailNotificationService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
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
        var apiKey = _config["SendGrid:ApiKey"];
        var senderEmail = _config["SendGrid:SenderEmail"] ?? "noreply@gradehub.local";

        await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation($"Attempting to send email to {studentEmail}...");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("GradeHub Integration", senderEmail));
            message.To.Add(new MailboxAddress("Student", studentEmail));
            message.Subject = $"Grade recorded for {courseName}";

            message.Body = new TextPart("plain")
            {
                Text = $"Your grade for {courseName} has been officially recorded: {gradeValue}"
            };

            using var client = new SmtpClient();
            
            // Connect to SendGrid SMTP
            await client.ConnectAsync("smtp.sendgrid.net", 587, MailKit.Security.SecureSocketOptions.StartTls); 
            
            // Authenticate with SendGrid if API key is provided
            if (!string.IsNullOrEmpty(apiKey))
            {
                // SendGrid always uses the username "apikey"
                await client.AuthenticateAsync("apikey", apiKey);
            }
            else 
            {
                _logger.LogWarning("SendGrid API key is missing from User Secrets.");
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            _logger.LogInformation($"Email sent successfully to {studentEmail}");
        });
    }
}
