using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Serilog.Context;
using CommandDotNet;
using TestApp.Services;
using CommandDotNet.IoC.MicrosoftDependencyInjection;

namespace TestApp
{

    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly IConfigurationRoot _config;

        public App(ILogger<App> logger, IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;

        }

        public int Run(string[] args)
        {
            string logKey = Guid.NewGuid().ToString();

            using (LogContext.PushProperty("logKey", logKey))
            {
                Console.WriteLine("Test");
            }
            var exitcode = new AppRunner<Menu>().UseMicrosoftDependencyInjection(Program._serviceProvider)
                .Run(args);

            _logger.LogInformation("Ending Service for ConsoleApp");

            return exitcode;

            
        }
    }
}
