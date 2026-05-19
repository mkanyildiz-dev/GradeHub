using GradeHub.Middleware.Models;

namespace GradeHub.Middleware.Services;

public interface IGradeProcessingService
{
    Task<bool> ProcessGradeAsync(GradeSubmission submission);
}
