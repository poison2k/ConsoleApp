using CommandDotNet;

namespace TestApp.Interfaces.Commands
{
   
    public interface IDiskCommandController
    {
        void extend(string text);

        void enlarge(string text);
    }
}
