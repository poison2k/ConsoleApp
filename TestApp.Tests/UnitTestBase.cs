using Moq;
using NUnit.Framework;

namespace TestApp.Tests
{
    public class UnitTestBase
    {
        public MockRepository MockRepository { get; private set; }

        [SetUp]
        public void UnitTestBaseSetUp()
        {
            MockRepository = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Empty };
        }

        [TearDown]
        public void VerifyAndTearDown()
        {
            MockRepository.VerifyAll();
        }
    }

}
