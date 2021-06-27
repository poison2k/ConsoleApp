using Microsoft.Extensions.Configuration;
using CommandDotNet;
using TestApp.Services;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using System;
using Serilog.Context;
using Microsoft.Extensions.Logging;

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
            int exitcode;

            using (LogContext.PushProperty("logKey", logKey))
            {
                exitcode = new AppRunner<Menu>()
                .UseMicrosoftDependencyInjection(Program._serviceProvider)
                .Run(args);

                _logger.LogInformation("Ending Service for ConsoleApp");
            }
           

            return exitcode;

            
        }
    }
}
