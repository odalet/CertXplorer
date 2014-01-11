using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Delta.CapiNet.Cryptography;

namespace MSTestCapiNet.Cryptography
{
    [TestClass]
    public class LuhnTest
    {
        private const string visa = "2976064092593252";
        private const string ok1 = "972487086";
        private const string ko1 = "927487086";

        // Found here: http://www.paypalobjects.com/en_US/vhelp/paypalmanager_help/credit_card_numbers.htm
        private static string[] testCards = new string[]
        {
            "378282246310005",
            "371449635398431",
            "378734493671000",
            "5610591081018250",
            "30569309025904",
            "38520000023237",
            "6011111111111117",
            "6011000990139424",
            "3530111333300000",
            "3566002020360505",
            "5555555555554444",
            "5105105105105100",
            "4111111111111111",
            "4012888888881881",
            "4222222222222",
            // According to http://umairj.com/374/how-to-test-credit-card-numbers-using-luhns-algorithm/, the one below
            // does not seem to implement Luhn checksum...
            //"76009244561", 
            "5019717010103742",
            "6331101999990016",
            visa,
            ok1
        };


        [TestMethod]
        public void TestCompute()
        {
            var index = 0;
            foreach (var testCard in testCards)
            {
                var input = testCard.Substring(0, testCard.Length - 1);
                var expected = testCard.Substring(testCard.Length - 1);

                var result = Luhn.Compute(input);

                Assert.AreEqual(expected, result.ToString(), "Failed at index #" + index);
                index++;
            }
        }

        [TestMethod]
        public void TestValidateReturnsTrue()
        {
            var index = 0;
            foreach (var testCard in testCards)
            {
                Assert.AreEqual(true, Luhn.Validate(testCard), "Failed at index #" + index);
                index++;
            }
        }

        [TestMethod]
        public void TestValidateReturnsFalse()
        {
            Assert.AreEqual(false, Luhn.Validate(ko1));
        }

        [TestMethod]
        public void TestEmptyInputThrowsInValidate()
        {
            AssertEx.Throws<ArgumentNullException>(() => Luhn.Validate(""));
        }

        [TestMethod]
        public void TestInvalidInputThrowsInValidateAndCompute()
        {
            AssertEx.Throws<ArgumentException>(() => Luhn.Compute(".123foo456:"));
            AssertEx.Throws<ArgumentException>(() => Luhn.Validate("*789baz012_"));
        }
    }
}
