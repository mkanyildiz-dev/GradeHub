namespace GradeHub.Middleware.Services;

public interface ICisSoapClient
{
    Task<string> StoreGradeAsync(string studentId, string courseId, string grade);
}
