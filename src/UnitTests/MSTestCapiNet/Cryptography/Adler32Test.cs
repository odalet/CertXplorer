using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Delta.CapiNet.Cryptography;

namespace MSTestCapiNet.Cryptography
{
    [TestClass]
    public class Adler32Test
    {
        // Test data grabbed from http://en.wikipedia.org/wiki/Adler-32
        private const string test1 = "Wikipedia";
        private const long result1 = 300286872L;

        [TestMethod]
        public void TestCompute()
        {
            var expected = result1;
            var result = Adler32.Compute(test1, Encoding.ASCII);

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result <= (long)uint.MaxValue);
        }
    }
}
