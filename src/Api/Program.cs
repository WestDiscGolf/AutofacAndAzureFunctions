using Api;
using Api.Abstractions;
using Api.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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