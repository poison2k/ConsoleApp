using Microsoft.Extensions.Configuration;
using CommandDotNet;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using System;
using Serilog.Context;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using CommandDotNet.FluentValidation;
using Microsoft.Extensions.Localization;
using TestApp.Provider;
using TestApp.Shared;
using TestApp.Configuration.Constants;
using TestApp.Controller;

namespace TestApp
{
    public class App 
    {
        private readonly ILogger<App> _logger;
        private readonly IConfigurationRoot _config;
        private readonly IStringLocalizer<LogMessages> _LogLocalizer;

        public App(ILogger<App> logger, IConfigurationRoot config,IStringLocalizer<LogMessages> logLocalizer)
        {
            _LogLocalizer = logLocalizer;
            _logger = logger;
            _config = config;
        }
    
        public int Run(string[] args)
        {
            string logKey = Guid.NewGuid().ToString();
            int exitcode;

            using (LogContext.PushProperty("logKey", logKey))
            {
                exitcode = new AppRunner<MenuController>()
                .Configure(x => x.CustomHelpProvider = new MyCustomHelpProvider(Program.GetService<IStringLocalizerFactory>(), x.AppSettings))
                .UseFluentValidation()
                .UseMicrosoftDependencyInjection(Program._serviceProvider)
                .Run(args);
                
                _logger.LogInformation(_LogLocalizer.GetString(LogMessageConsts.ExitApp));
            }
            return exitcode;        
        }
    }
}
