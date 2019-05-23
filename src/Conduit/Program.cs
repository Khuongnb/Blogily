using System;
using System.Linq;
using System.Text.RegularExpressions;
using Conduit.Infrastructure;
using Microsoft.EntityFrameworkCore;
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

            var runAsTest = args.Contains("--test");
            var resetDataBase = args.Contains("--resetdb");
            args = args.Where(x => x != "--test" && x!= "--resetdb").ToArray();

            DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder();
            dbOptions.UseSqlite("Filename=realworldtest.db");

            // read database configuration (database provider + database connection) from environment variables
            var startup = new Startup();
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(startup.AppConfig)
                .ConfigureServices(services => { startup.AddNonStaticServices(services); })
                .ConfigureServices(services => { services.AddSingleton(new CommandLineArgs(args)); })
                .ConfigureServices(startup.StaticServicesConfig);

            if (runAsTest)
            { 
                builder.ConfigureServices(services => services.AddSingleton(new ConduitContext(dbOptions.Options)));
                builder.ConfigureLogging((context, structure) =>
                {
                    if (resetDataBase)
                    {
                        structure.Services.BuildServiceProvider().GetRequiredService<ConduitContext>().Database.EnsureDeleted();
                    }
                    structure.Services.BuildServiceProvider().GetRequiredService<ConduitContext>().Database.EnsureCreated();
                });
            }
            else
                builder.ConfigureLogging(startup.RuntimeConfigure);

            builder.RunConsoleAsync();

        }
    }
}
