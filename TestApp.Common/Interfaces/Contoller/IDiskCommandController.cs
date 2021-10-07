using TestApp.Common.CommandModels.DiskCommandModels;

namespace TestApp.Common.Interfaces.Controller
{
    public interface IDiskCommandController
    {
        void extend(string text);

        void enlarge(DiskExtendModel diskEnlargeModel);
    }
}
