using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Api;

public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureFunctionsWorkerDefaults(worker =>
            {
                worker.UseMiddleware<HttpRequestMiddleware>();
            })
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterType<DefaultGreetingService>().As<IGreeting>();
                builder.RegisterType<HelloService>().Keyed<IGreeting>("hello");
                builder.RegisterType<GoodByeService>().Keyed<IGreeting>("goodbye");
                builder.RegisterType<MyService>().As<IMyService>().WithAttributeFiltering();
                builder.RegisterType<GetWelcome>().WithAttributeFiltering();
            })
            .ConfigureServices(services =>
            {
                services.Replace(ServiceDescriptor.Singleton<IFunctionActivator, ServiceBasedFunctionActivator>());
                services.AddSingleton<IHttpRequestAccessor, HttpRequestAccessor>();
                
                // note: can be registered with the basic IServiceCollection or with the AutoFac ContainerBuilder above.
                //services.AddTransient<IGreeting, DefaultGreetingService>();
            })
            .Build();

        host.Run();
    }
}

public class DefaultGreetingService : IGreeting
{
    public string Speak() => "Not coming or going :-/";
}

public class HttpRequestMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        // determine the type, the default is Microsoft.Azure.Functions.Worker.Context.Features.GrpcFunctionBindingsFeature
        (Type featureType, object featureInstance) = context.Features.SingleOrDefault(x => x.Key.Name == "IFunctionBindingsFeature");

        // find the input binding of the function which has been invoked and then find the associated parameter of the function for the data we want
        var inputData = featureType.GetProperties().SingleOrDefault(p => p.Name == "InputData")?.GetValue(featureInstance) as IReadOnlyDictionary<string, object>;
        var requestData = inputData?.Values.SingleOrDefault(obj => obj is HttpRequestData) as HttpRequestData;

        if (requestData is not null)
        {
            // set the request data on the accessor from DI
            var accessor = context.InstanceServices.GetRequiredService<IHttpRequestAccessor>();
            accessor.HttpRequest = requestData;
        }

        await next(context);
    }
}

public interface IHttpRequestAccessor
{
    HttpRequestData? HttpRequest { get; set; }
}

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

public class HttpHeaderDrivenAttribute : ParameterFilterAttribute
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