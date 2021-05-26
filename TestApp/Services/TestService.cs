using System;
using TestApp.Interfaces.Services;

namespace TestApp.Services
{
    public class TestService : ITestService
    {
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}
