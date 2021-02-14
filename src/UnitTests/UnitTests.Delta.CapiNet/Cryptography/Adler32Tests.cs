using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Delta.CapiNet.Cryptography
{
    [TestClass, ExcludeFromCodeCoverage]
    public class Adler32Tests
    {
        // Test data grabbed from http://en.wikipedia.org/wiki/Adler-32
        private const string test1 = "Wikipedia";
        private const long result1 = 300286872L;

        [TestMethod]
        public void Wikipedia_is_correctly_encoded()
        {
            var expected = result1;
            var result = Adler32.Compute(test1, Encoding.ASCII);

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result <= uint.MaxValue);
        }
    }
}
