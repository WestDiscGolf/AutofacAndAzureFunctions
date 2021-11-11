using System.Net;
using Api.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api;

public class GetWelcome
{
    private readonly IGreeting _greeting;
    private readonly ILogger _logger;
        
    public GetWelcome(
        [HttpRequestDriven] IGreeting greeting,
        ILoggerFactory loggerFactory)
    {
        _greeting = greeting;
        _logger = loggerFactory.CreateLogger<GetWelcome>();
    }

    [Function("GetWelcome")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString($"{_greeting.Speak()} Azure Functions!");

        return response;
    }
}