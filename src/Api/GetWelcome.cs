using System.Net;
using Autofac.Features.AttributeFilters;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api;

public class GetWelcome
{
    private readonly IGreeting _hello;
    private readonly IGreeting _goodbye;
    private readonly ILogger _logger;
        
    public GetWelcome(
        [KeyFilter("hello")] IGreeting hello,
        [KeyFilter("goodbye")] IGreeting goodbye,
        ILoggerFactory loggerFactory)
    {
        _hello = hello;
        _goodbye = goodbye;
        _logger = loggerFactory.CreateLogger<GetWelcome>();
    }

    [Function("GetWelcome")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString($"{_hello.Speak()} and then {_goodbye.Speak()}");

        return response;
    }
}