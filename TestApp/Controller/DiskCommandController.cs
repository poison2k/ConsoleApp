using CommandDotNet;
using TestApp.Interfaces.Commands;
using TestApp.Interfaces.Services;

namespace TestApp.Controller
{

    [Command(Name ="Disk", Description = "DiskCommandController")]
    public class DiskCommandController : IDiskCommandController
    {
        private readonly ITestService _TestService;

        public DiskCommandController(ITestService testService)
        {
            _TestService = testService;
        }

        [Command(Name = "enlarge")]
        public void enlarge([Option(LongName = "vsd", Description="testparam", ShortName ="v")]string text)
        {
            _TestService.WriteLine(text);
        }

        public void extend(string text)
        {
            _TestService.WriteLine(text);
        }
    }
}
