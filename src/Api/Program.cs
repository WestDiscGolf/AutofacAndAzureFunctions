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
                .ConfigureServices(services =>
                {
                    services.AddTransient<IGreeting, HelloService>();
                })
                .Build();

            host.Run();
        }
    }
}