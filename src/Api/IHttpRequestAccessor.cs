using Microsoft.Azure.Functions.Worker.Http;

namespace Api;

public interface IHttpRequestAccessor
{
    HttpRequestData? HttpRequest { get; set; }
}