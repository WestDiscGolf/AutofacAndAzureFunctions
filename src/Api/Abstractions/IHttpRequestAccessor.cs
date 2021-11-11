using Microsoft.Azure.Functions.Worker.Http;

namespace Api.Abstractions;

public interface IHttpRequestAccessor
{
    HttpRequestData? HttpRequest { get; set; }
}