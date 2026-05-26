using GradeHub.Middleware.Models;
using GradeHub.Middleware.Services;
using Microsoft.AspNetCore.Mvc;

namespace GradeHub.Middleware.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("api/grades")]
public class GradeController : ControllerBase
{
    private readonly IGradeProcessingService _processingService;
    private readonly ILogger<GradeController> _logger;

    public GradeController(IGradeProcessingService processingService, ILogger<GradeController> logger)
    {
        _processingService = processingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitGrades([FromBody] List<GradeSubmission> submissions)
    {
        if (submissions == null || !submissions.Any())
        {
            return BadRequest(new { message = "No grade submissions provided." });
        }

        _logger.LogInformation("Received {Count} grade submissions via Controller.", submissions.Count);

        var results = new List<object>();
        var allSuccess = true;

        foreach (var submission in submissions)
        {
            try
            {
                var success = await _processingService.ProcessGradeAsync(submission);
                results.Add(new { email = submission.StudentEmail, course = submission.CourseName, success = success });
                if (!success)
                {
                    allSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing grade for student: {StudentEmail}", submission.StudentEmail);
                results.Add(new { email = submission.StudentEmail, course = submission.CourseName, success = false, error = ex.Message });
                allSuccess = false;
            }
        }

        if (allSuccess)
        {
            return Ok(new { message = "All grades processed successfully.", details = results });
        }

        return StatusCode(207, new { message = "Some grade submissions failed to process.", details = results });
    }
}
