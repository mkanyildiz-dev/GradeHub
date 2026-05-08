using System.ServiceModel;

namespace GradeHub.CIS.Mock.Services;

[ServiceContract]
public interface IGradeService
{
    [OperationContract]
    string StoreGrade(string studentId, string courseId, string grade);
}
