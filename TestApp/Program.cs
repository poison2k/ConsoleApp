using CommandDotNet;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using TestApp.Common.Interfaces;
using TestApp.Controller;
using TestApp.Interfaces;
using TestApp.Interfaces.Commands;
using TestApp.Interfaces.Services;
using TestApp.Services;

namespace TestApp
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

        static int Main(string[] args)
        {

            _serviceProvider = ConfigureServices();
            var logger = _serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogInformation("Starting application");

            var bar = _serviceProvider.GetService<IBarService>();


            return new AppRunner<Menu>().UseMicrosoftDependencyInjection(ConfigureServices())
                .UseDefaultMiddleware()
                .Run(args);
        }

        public  static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug));

            serviceCollection.AddTransient<IFooService, FooService>();
            serviceCollection.AddSingleton<IBarService, BarService>();
            serviceCollection.AddSingleton<ITestService, TestService>();
            serviceCollection.AddSingleton<IMenu, Menu>();
            serviceCollection.AddSingleton<DiskCommandController, DiskCommandController>();

            return serviceCollection.BuildServiceProvider();
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
    }
}
