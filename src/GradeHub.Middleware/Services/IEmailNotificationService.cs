namespace GradeHub.Middleware.Services;

public interface IEmailNotificationService
{
    Task SendGradeNotificationAsync(string studentEmail, string courseName, string gradeValue);
}
