using System.ServiceModel;
using Polly;
using Polly.Retry;

namespace GradeHub.Middleware.Services;

public class CisSoapClient : ICisSoapClient
{
    private readonly ILogger<CisSoapClient> _logger;
    private readonly string _endpointUrl;
    private readonly AsyncRetryPolicy _retryPolicy;

    public CisSoapClient(ILogger<CisSoapClient> logger, IConfiguration configuration)
    {
        _logger = logger;
        _endpointUrl = configuration.GetValue<string>("CisMock:SoapUrl") ?? "http://localhost:5066/GradeService.asmx";
        
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3, 
                retryAttempt => TimeSpan.FromSeconds(5), 
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning($"SOAP call failed (Attempt {retryCount}/3). Waiting 5s... Error: {exception.Message}");
                });
    }

    public async Task<string> StoreGradeAsync(string studentId, string courseId, string grade)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Sending SOAP request to CIS Mock...");
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(_endpointUrl);
            var client = new GradeServiceClient(binding, endpoint);

            var response = await client.StoreGradeAsync(studentId, courseId, grade);
            client.Close();
            
            _logger.LogInformation("Successfully sent to CIS Mock.");
            return response.Body.StoreGradeResult;
        });
    }
}
