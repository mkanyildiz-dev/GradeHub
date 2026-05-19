using GradeHub.Middleware.Models;
using GradeHub.Middleware.Services;
using Microsoft.AspNetCore.Mvc;

namespace GradeHub.Middleware.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> SubmitGrade([FromBody] GradeSubmission submission)
    {
        _logger.LogInformation("Received grade submission via Controller for student: {StudentEmail}", submission.StudentEmail);

        try
        {
            var success = await _processingService.ProcessGradeAsync(submission);
            
            if (success)
            {
                return Ok(new { message = "Grade processed successfully." });
            }
            
            return BadRequest(new { message = "Failed to process grade." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GradeController.");
            return StatusCode(500, new { message = "An error occurred while processing the grade." });
        }
    }
}
