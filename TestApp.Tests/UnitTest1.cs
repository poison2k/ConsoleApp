using Moq;
using NUnit.Framework;
using System;
using System.IO;
using TestApp.Business.Services;
using TestApp.Common.Interfaces.Repositories;
using TestApp.Tests;
using Serilog;
using Microsoft.Extensions.Logging;

namespace Tests
{
    [TestFixture]
    public class Tests : UnitTestBase
    {

        TestService testService;
        Mock<IDiskRepository> diskRepositoryMock;
        StringWriter consoleOutput;
        Mock<ILogger<TestService>> logger;

        [SetUp]
        public void Setup()
        {
            diskRepositoryMock = new Mock<IDiskRepository>();
            logger = new Mock<ILogger<TestService>>();
            testService = new TestService(diskRepositoryMock.Object, logger.Object);
            consoleOutput = new StringWriter();
           
        }

        [Test]
        public void Test1()
        {
            //arange
            diskRepositoryMock
                .Setup(x => x.DiskSpace("test"))
                .Returns("TEST")
                .Verifiable();
            Console.SetOut(consoleOutput);

            //act
            testService.WriteLine("test");
            
            //asert
            diskRepositoryMock.Verify(p => p.DiskSpace("test"));
            Assert.AreEqual("TEST" + Environment.NewLine, consoleOutput.ToString());
        }
    }
}