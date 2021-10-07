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
using System.Globalization;
using TestApp.Common.Validators.Disk;
using Microsoft.Extensions.Localization;
using TestApp.Configuration.Constants;
using TestApp.Shared;
using TestApp.Common.Interfaces.Controller;

namespace TestApp
{
    class Program
    {
        public static ServiceProvider _serviceProvider;

        public static IConfigurationRoot configuration;

        private static IStringLocalizer<ErrorMessages> _ErrorLocalizer;

        static int Main(string[] args)
        {
            //variables
            int exitcode;

            //Creating Service  Collection
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection = ConfigureServices(serviceCollection);

            //Read CulturInfo from Appsettings
            var culture = new CultureInfo(configuration.GetSection("Culture").GetSection("CultureInfo").Value.ToString());

            //Set CultureInfo
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            //Build service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            //Set LogLocalizer
            _ErrorLocalizer = GetService<IStringLocalizer<ErrorMessages>>();

            try
            {
                //Run Testapp   
                exitcode = _serviceProvider.GetService<App>().Run(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, _ErrorLocalizer.GetString(ErrorMessageConsts.FatalError));
                exitcode = 666;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return exitcode;
        }

        public static T GetService<T>()
        {
            return Program._serviceProvider.GetService<T>();
        }

        public static IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            //Add Serilog to Servicecollection
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                        .MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .CreateLogger());
            }));
            serviceCollection.AddLogging();

            //add Localization
            serviceCollection.AddLocalization(options => options.ResourcesPath = "Resources");

            serviceCollection.AddLogging();

            //Create Logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .CreateLogger();

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
            //Setup Controller
            serviceCollection.AddSingleton<IMenuController, MenuController>();
            serviceCollection.AddSingleton<DiskCommandController, DiskCommandController>();
            //Setup Repositories
            serviceCollection.AddSingleton<IDiskRepository, DiskRepository>();
            //Setup Validators
            serviceCollection.AddSingleton<DiskExtendModelValidator, DiskExtendModelValidator>();

            //Add app
            serviceCollection.AddTransient<App>();
            return serviceCollection;
        }
    }
}
