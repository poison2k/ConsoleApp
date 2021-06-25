using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using TestApp.Common.Interfaces;
using TestApp.Controller;
using TestApp.Interfaces;
using TestApp.Interfaces.Services;
using TestApp.Services;


namespace TestApp
{
    class Program
    {
        public static ServiceProvider _serviceProvider;

        public static IConfigurationRoot configuration;

        static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .CreateLogger();
            int exitcode;

            try
            {
                //Start!
                exitcode = MainAsync(args);
            }
            catch
            {
                exitcode =  1;
            }

            return exitcode;
          
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null) {
                return;
            }
            if(_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }

        private static int MainAsync(string[] args)
        {
            int exitcode = 0;
            Log.Information("Creating service collection");
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection = ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            //Create service provider
            Log.Information("Building service provider");
          

            //Print connection string to demonstrate configuration object is populated
            Console.WriteLine(configuration.GetConnectionString("DataConnection"));

            try
            {
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

            //Add logging
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            }));

            serviceCollection.AddLogging(builder => builder
                     .AddConsole()
                     .SetMinimumLevel(LogLevel.Debug));

            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
            serviceCollection.AddTransient<IFooService, FooService>();
            serviceCollection.AddSingleton<IBarService, BarService>();
            serviceCollection.AddSingleton<ITestService, TestService>();
            serviceCollection.AddSingleton<IMenu, Menu>();
            serviceCollection.AddSingleton<DiskCommandController, DiskCommandController>();
            //Add app
            serviceCollection.AddTransient<App>();

            return serviceCollection;
        }
    }

}
