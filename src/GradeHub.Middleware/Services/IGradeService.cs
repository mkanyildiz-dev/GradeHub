using System.ServiceModel;

namespace GradeHub.Middleware.Services;

[ServiceContract]
public interface IGradeService
{
    [OperationContract]
    string StoreGrade(string studentId, string courseId, string grade);
}
