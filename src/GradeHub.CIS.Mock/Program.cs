using SoapCore;
using GradeHub.CIS.Mock.Services;

var builder = WebApplication.CreateBuilder(args);

// Add SOAP Core services
builder.Services.AddSoapCore();
builder.Services.AddSingleton<IGradeService, GradeService>();

var app = builder.Build();

app.UseRouting();

// Map SOAP Endpoint to generate WSDL and handle requests
app.UseEndpoints(endpoints => {
    endpoints.UseSoapEndpoint<IGradeService>("/GradeService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
});

app.Run();
