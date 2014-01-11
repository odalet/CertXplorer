using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSTestCapiNet
{
    public class TestBase
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }
    }

    // inspired by http://stackoverflow.com/questions/8213569/assert-exception-from-nunit-to-ms-test
    public static class AssertEx
    {
        public static Exception Throws<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (T ex)
            {
                return ex;
            }
            catch (Exception exception)
            {
                Assert.Fail("Expected an exception of type {0}; actual exception is of type {1}.", typeof(T), exception.GetType());
                return exception;
            }

            Assert.Fail("Expected an exception of type {0}; none was thrown.", typeof(T));
            return null;
        }
    }
}
