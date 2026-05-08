using System.ServiceModel;

namespace GradeHub.CIS.Mock.Services;

public class GradeService : IGradeService
{
    private readonly ILogger<GradeService> _logger;
    private readonly string _filePath = "grades.csv";

    public GradeService(ILogger<GradeService> logger)
    {
        _logger = logger;
    }

    public string StoreGrade(string studentId, string courseId, string grade)
    {
        _logger.LogInformation("Received grade request for Student: {StudentId}, Course: {CourseId}, Grade: {Grade}", studentId, courseId, grade);

        if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(courseId) || string.IsNullOrWhiteSpace(grade))
        {
            _logger.LogWarning("Invalid input data.");
            throw new FaultException("Invalid input data. All fields are required.");
        }

        try
        {
            var line = $"{DateTime.UtcNow:O},{studentId},{courseId},{grade}\n";
            File.AppendAllText(_filePath, line);
            _logger.LogInformation("Grade stored successfully.");
            return "SUCCESS";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing grade.");
            throw new FaultException("Internal server error while storing grade.");
        }
    }
}
