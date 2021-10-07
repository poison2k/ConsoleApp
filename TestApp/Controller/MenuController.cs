using System;
using CommandDotNet;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using TestApp.Common.CommandModels.DiskCommandModels;
using TestApp.Common.Interfaces.Controller;
using TestApp.Common.Interfaces.Services;

namespace TestApp.Controller
{
    [Command(Description ="Performs mathematical calculations")]
    public class MenuController : IMenuController
    {
        [SubCommand]
        public DiskCommandController Disk { get; set; }

        private readonly ITestService _TestService;

        private readonly IStringLocalizer<IMenuController> _localizer;

        public MenuController(ITestService testService, IStringLocalizer<IMenuController> localizer)
        {
            _TestService = testService;
            _localizer = localizer;
        }

        private void ValidateModel(DiskExtendModel diskExtendModel)
        {
            string content = JsonConvert.SerializeObject(diskExtendModel, Formatting.Indented);
            Console.WriteLine(content);
        }
    }
}
