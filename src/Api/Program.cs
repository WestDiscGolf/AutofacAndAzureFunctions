using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Microsoft.Azure.Functions.Worker;
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
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterType<HelloService>().Keyed<IGreeting>("hello");
                builder.RegisterType<GoodByeService>().Keyed<IGreeting>("goodbye");
                builder.RegisterType<MyService>().As<IMyService>().WithAttributeFiltering();
                builder.RegisterType<GetWelcome>().WithAttributeFiltering();
            })
            .ConfigureServices(services =>
            {
                services.Replace(ServiceDescriptor.Singleton<IFunctionActivator, ServiceBasedFunctionActivator>());
            })
            .Build();

        host.Run();
    }
}