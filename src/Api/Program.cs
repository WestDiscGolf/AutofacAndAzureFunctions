using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
                    builder.RegisterType<HelloService>().As<IGreeting>();
                })
                .ConfigureServices(services =>
                {
                    
                })
                .Build();

            host.Run();
        }
    }
}