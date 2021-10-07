using CommandDotNet;
using FluentValidation.Attributes;
using TestApp.Common.Validators.Disk;

namespace TestApp.Common.CommandModels.DiskCommandModels
{
    
    [Validator(typeof(DiskExtendModelValidator))]
    public class DiskExtendModel : IArgumentModel
    {
        [OrderByPositionInClass]
        [Option(LongName="text", Description = "Message")]
        public string Text { get; set; }
    }
}
