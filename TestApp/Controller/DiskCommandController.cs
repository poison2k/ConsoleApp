using CommandDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using TestApp.Common.CommandModels.DiskCommandModels;
using TestApp.Common.Interfaces.Controller;
using TestApp.Common.Interfaces.Services;
using TestApp.Configuration.Constants;

namespace TestApp.Controller
{

    [Command(Name = DiskContollerConsts.DiskCommand, Description = DiskContollerConsts.DiskCommandDescription)]
    public class DiskCommandController : IDiskCommandController
    {
        private readonly ITestService _TestService;
        private readonly IConfigurationRoot _config;
        private readonly IStringLocalizer<IDiskCommandController> _Localizer;
        

        public DiskCommandController(ITestService testService, IConfigurationRoot config, IStringLocalizer<DiskCommandController> localizer )
        {
            _TestService = testService;
            _config = config;
            _Localizer = localizer;
        }

        [Command(Name = DiskContollerConsts.EnlargeCommand, ExtendedHelpText = DiskContollerConsts.EnlargeCommandHelpText, Description = DiskContollerConsts.EnlargeCommandDescription)]
        public void enlarge(DiskExtendModel diskEnlargeModel)
        {
            _TestService.WriteLine(_Localizer.GetString(DiskContollerConsts.EnlargeMessage));
        }

        [Command(Name = DiskContollerConsts.ExtendCommand, ExtendedHelpText = DiskContollerConsts.ExtendCommandHelpText, Description = DiskContollerConsts.ExtendCommandDescription)]
        public void extend([Option(Description = CommandDotNetConsts.DiskExtendTextArgument)]string text)
        {
            _TestService.WriteLine(_Localizer.GetString(DiskContollerConsts.ExtendMessage)) ;
        }

        public void ValidateModel(DiskExtendModel diskExtendModel)
        {
            string content = JsonConvert.SerializeObject(diskExtendModel, Formatting.Indented);
            Console.WriteLine(content);
        }
       
    }
}
