using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Hosting;

namespace Api
{
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
                })
                .ConfigureServices(services =>
                {
                    
                })
                .Build();

            host.Run();
        }
    }
}