using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using TestApp.Business.Services;
using TestApp.Common.Interfaces;
using TestApp.Common.Interfaces.Repositories;
using TestApp.Common.Interfaces.Services;
using TestApp.Controller;
using TestApp.Data.Repositories;
using TestApp.Services;


namespace TestApp
{
    class Program
    {
        public static ServiceProvider _serviceProvider;

        public static IConfigurationRoot configuration;

        static int Main(string[] args)
        {
            //variables
            int exitcode;

            //Create Logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .CreateLogger();

            //Creating Service  Collection
            Log.Information("Creating service collection");
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection = ConfigureServices(serviceCollection);

            //Build service provider
            Log.Information("Building service provider");
            _serviceProvider = serviceCollection.BuildServiceProvider();

            //Print connection string to demonstrate configuration object is populated
            Console.WriteLine(configuration.GetConnectionString("DataConnection"));

            try
            {
                //Run Testapp 
                Log.Information("Starting Testapp");
                exitcode = _serviceProvider.GetService<App>().Run(args);
                Log.Information("Ending Testapp");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error running service");
                exitcode = 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return exitcode;

        }


        public static IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {

            //Add logging to Servicecollection
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            }));

            serviceCollection.AddLogging(builder => builder
                     .AddConsole()
                     .SetMinimumLevel(LogLevel.Debug));

            //SetUp Configuration Files
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            //SetUp Services
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddTransient<IFooService, FooService>();
            serviceCollection.AddSingleton<IBarService, BarService>();
            serviceCollection.AddSingleton<ITestService, TestService>();
            serviceCollection.AddSingleton<IMenu, Menu>();
            serviceCollection.AddSingleton<DiskCommandController, DiskCommandController>();
            serviceCollection.AddSingleton<IDiskRepository, DiskRepository>();
            
            //Add app
            serviceCollection.AddTransient<App>();

            return serviceCollection;
        }

      }

}
