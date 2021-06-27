using CommandDotNet;
using Microsoft.Extensions.Configuration;
using System.Linq;
using TestApp.Common.Interfaces.Services;

namespace TestApp.Controller
{

    [Command(Name ="Disk", Description = "DiskCommandController")]
    public class DiskCommandController : IDiskCommandController
    {
        private readonly ITestService _TestService;
        private readonly IConfigurationRoot _config;

        public DiskCommandController(ITestService testService, IConfigurationRoot config)
        {
            _TestService = testService;
            _config = config;
        }

        [Command(Name = "enlarge")]
        public void enlarge([Option(LongName = "vsd", Description="testparam", ShortName ="v")]string text)
        {
            _TestService.WriteLine(_config.GetConnectionString("DataConnection"));
        }

        [Command(Name = "extend")]
        public void extend(string text)
        {
            _TestService.WriteLine(_config.GetSection("EmailAdresses").GetChildren().ToList().FirstOrDefault().Value) ;
        }
    }
}
