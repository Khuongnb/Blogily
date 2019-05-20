using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Conduit
{
    public class CommandLineArgs
    {
        public String[] Command;

        public CommandLineArgs(String[] args)
        {
            Command = args;
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            // read database configuration (database provider + database connection) from environment variables

            var startup = new Startup();
            new HostBuilder()
                .ConfigureAppConfiguration(startup.AppConfig)
                .ConfigureServices(services => { startup.AddNonStaticServices(services); })
                .ConfigureServices(services => { services.AddSingleton(new CommandLineArgs(args)); })
                .ConfigureServices(startup.StaticServicesConfig)
                .RunConsoleAsync();

        }
    }
}
