using System;
using CommandDotNet;
using TestApp.Controller;
using TestApp.Interfaces;
using TestApp.Interfaces.Commands;
using TestApp.Interfaces.Services;

namespace TestApp.Services
{
    [Command(Description ="Performs mathematical calculations")]
    public class Menu : IMenu
    {

        [SubCommand]
        public DiskCommandController Disk { get; set; }

        private readonly ITestService _TestService; 

        public Menu(ITestService testService)
        {
            _TestService = testService;
        }

        [Command(Description = "Writes Text ")]
        public void Write (string text)
        {
            _TestService.WriteLine(text);
        }

        [Command(Description ="Adds two numbers")]
        public void Add(int value1, int value2)
        {
            Console.WriteLine($"Answer: {value1 + value2}");
        }

        [Command(Description ="Subtractes two numbers")]
        public void Substarct(int value1, int value2)
        {
            Console.WriteLine($"Answer: {value1 - value2}");
        }
    }
}
