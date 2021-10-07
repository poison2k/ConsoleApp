using FluentValidation;
using TestApp.Common.CommandModels.DiskCommandModels;
using Microsoft.Extensions.Localization;

namespace TestApp.Common.Validators.Disk
{
    public class DiskExtendModelValidator : AbstractValidator<DiskExtendModel>
    {
        public readonly IStringLocalizer<DiskExtendModel> _Localizer;
        public DiskExtendModelValidator(IStringLocalizer<DiskExtendModel> localizer)
        {
            _Localizer = localizer;
            RuleFor(x => x.Text).MinimumLength(2).WithMessage(_Localizer.GetString("test"));
        }
       
    }
}
