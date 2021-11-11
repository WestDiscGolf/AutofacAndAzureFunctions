using System.Threading;
using Api.Abstractions;
using Microsoft.Azure.Functions.Worker.Http;

namespace Api;

public class HttpRequestAccessor : IHttpRequestAccessor
{
    private readonly AsyncLocal<ContextHolder> _context = new();

    public HttpRequestData? HttpRequest
    {
        get => _context.Value?.Context;
        set
        {
            var holder = _context.Value;
            if (holder is not null)
            {
                holder.Context = null;
            }

            if (value is not null)
            {
                _context.Value = new ContextHolder { Context = value };
            }
        }
    }

    private class ContextHolder
    {
        public HttpRequestData? Context;
    }
}