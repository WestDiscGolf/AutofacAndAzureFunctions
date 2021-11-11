using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class GetWelcome
    {
        private readonly IMyService _myService;
        private readonly ILogger _logger;
        
        public GetWelcome(IMyService myService, ILoggerFactory loggerFactory)
        {
            _myService = myService;
            _logger = loggerFactory.CreateLogger<GetWelcome>();
        }

        [Function("GetWelcome")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"{_myService.Speak()} Welcome to Azure Functions!");

            return response;
        }
    }
}
