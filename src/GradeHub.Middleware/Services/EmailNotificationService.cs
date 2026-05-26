using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Polly;
using Polly.Retry;

namespace GradeHub.Middleware.Services;

public class EmailNotificationService : IEmailNotificationService
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
        var email = _config["Gmail:Email"] ?? throw new InvalidOperationException("Gmail:Email configuration is missing.");
        var password = (_config["Gmail:AppPassword"] ?? throw new InvalidOperationException("Gmail:AppPassword configuration is missing.")).Replace(" ", "");

        await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation($"Attempting to send email to {studentEmail} via SMTP...");

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(email));
            message.To.Add(MailboxAddress.Parse(studentEmail));
            message.Subject = $"Grade recorded for {courseName}";
            message.Body = new TextPart("plain")
            {
                Text = $"Your grade for {courseName} has been officially recorded: {gradeValue}"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(email, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"Email sent successfully to {studentEmail} and disconnected.");
        });
    }
}
