using System.Linq;
using System.Reflection;
using Api.Abstractions;
using Autofac;
using Autofac.Features.AttributeFilters;

namespace Api;

public class HttpRequestDrivenAttribute : ParameterFilterAttribute
{
    private const string HeaderName = "X-Greeting";

    public override object? ResolveParameter(ParameterInfo parameter, IComponentContext context)
    {
        if (TryResolveServiceKey(context, out var key))
        {
            if (context.TryResolveNamed(key, parameter.ParameterType, out var instance))
            {
                return instance;
            }
        }

        // as the "CanResolveParameter" method would return false, it will only come in here if the method resolves true
        return null;
    }

    public override bool CanResolveParameter(ParameterInfo parameter, IComponentContext context)
    {
        if (TryResolveServiceKey(context, out var key))
        {
            return context.IsRegisteredWithKey(key, parameter.ParameterType);
        }

        return false;
    }

    private bool TryResolveServiceKey(IComponentContext context, out string? key)
    {
        key = null;
        if (context.TryResolve<IHttpRequestAccessor>(out var httpRequestAccessor) 
            && httpRequestAccessor.HttpRequest is not null)
        {
            if (httpRequestAccessor.HttpRequest.Headers.TryGetValues(HeaderName, out var values)
                && values.Any())
            {
                key = values.First();
            }
        }

        return !string.IsNullOrWhiteSpace(key);
    }
}