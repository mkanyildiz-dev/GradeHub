using GradeHub.Middleware.Models;

namespace GradeHub.Middleware.Services;

public class GradeProcessingService : IGradeProcessingService
{
    private readonly ICisSoapClient _soapClient;
    private readonly IEmailNotificationService _emailService;
    private readonly ILogger<GradeProcessingService> _logger;

    public GradeProcessingService(
        ICisSoapClient soapClient,
        IEmailNotificationService emailService,
        ILogger<GradeProcessingService> logger)
    {
        _soapClient = soapClient;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<bool> ProcessGradeAsync(GradeSubmission submission)
    {
        _logger.LogInformation("Processing grade submission for student: {StudentEmail}", submission.StudentEmail);

        try
        {
            // 1. Send to CIS Mock via SOAP
            var soapResult = await _soapClient.StoreGradeAsync(submission.StudentEmail, submission.CourseName, submission.GradeValue);

            // 2. Send Email via SMTP
            if (soapResult == "SUCCESS")
            {
                await _emailService.SendGradeNotificationAsync(submission.StudentEmail, submission.CourseName, submission.GradeValue);
                return true;
            }

            _logger.LogWarning("CIS Mock returned non-success result: {Result}", soapResult);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process grade submission.");
            throw;
        }
    }
}
