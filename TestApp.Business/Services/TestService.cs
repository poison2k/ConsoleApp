using Microsoft.Extensions.Logging;
using Serilog;
using System;
using TestApp.Common.Interfaces.Repositories;
using TestApp.Common.Interfaces.Services;

namespace TestApp.Business.Services
{
    public class TestService : ITestService
    {

        private readonly IDiskRepository _DiskRepository;

        private readonly ILogger<ITestService> _logger;

        public TestService(IDiskRepository diskRepository, ILogger<ITestService> logger)
        {
            _DiskRepository = diskRepository;
            _logger = logger;
        }
        public void WriteLine(string text)
        {
            var result = _DiskRepository.DiskSpace(text);
            _logger.LogInformation("InDiskSpaceFuncrion");
            Console.WriteLine(result);
        }
    }
}
