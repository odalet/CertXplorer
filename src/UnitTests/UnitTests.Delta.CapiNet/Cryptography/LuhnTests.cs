using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Delta.CapiNet.Cryptography
{
    [TestClass, ExcludeFromCodeCoverage]
    public class LuhnTests
    {
        private const string visa = "2976064092593252";
        private const string ok1 = "972487086";
        private const string ko1 = "927487086";

        // Found here: http://www.paypalobjects.com/en_US/vhelp/paypalmanager_help/credit_card_numbers.htm
        private const string test1 = "378282246310005";
        private const string test2 = "371449635398431";
        private const string test3 = "378734493671000";
        private const string test4 = "5610591081018250";
        private const string test5 = "30569309025904";
        private const string test6 = "38520000023237";
        private const string test7 = "6011111111111117";
        private const string test8 = "6011000990139424";
        private const string test9 = "3530111333300000";
        private const string test10 = "3566002020360505";
        private const string test11 = "5555555555554444";
        private const string test12 = "5105105105105100";
        private const string test13 = "4111111111111111";
        private const string test14 = "4012888888881881";
        private const string test15 = "4222222222222";
        // According to http://umairj.com/374/how-to-test-credit-card-numbers-using-luhns-algorithm/, the one below
        // does not seem to implement Luhn checksum...
        // "76009244561", 
        private const string test16 = "5019717010103742";
        private const string test17 = "6331101999990016";        

        [TestMethod]
        [DataRow(visa)]
        [DataRow(ok1)]
        [DataRow(test1)]
        [DataRow(test2)]
        [DataRow(test3)]
        [DataRow(test4)]
        [DataRow(test5)]
        [DataRow(test6)]
        [DataRow(test7)]
        [DataRow(test8)]
        [DataRow(test9)]
        [DataRow(test10)]
        [DataRow(test11)]
        [DataRow(test12)]
        [DataRow(test13)]
        [DataRow(test14)]
        [DataRow(test15)]
        [DataRow(test16)]
        [DataRow(test17)]
        public void Valid_input_has_computation_succeed(string card)
        {
            var input = card.Substring(0, card.Length - 1);
            var expected = card.Substring(card.Length - 1);

            var result = Luhn.Compute(input);

            Assert.AreEqual(expected, result.ToString());
        }

        [TestMethod]
        [DataRow(visa)]
        [DataRow(ok1)]
        [DataRow(test1)]
        [DataRow(test2)]
        [DataRow(test3)]
        [DataRow(test4)]
        [DataRow(test5)]
        [DataRow(test6)]
        [DataRow(test7)]
        [DataRow(test8)]
        [DataRow(test9)]
        [DataRow(test10)]
        [DataRow(test11)]
        [DataRow(test12)]
        [DataRow(test13)]
        [DataRow(test14)]
        [DataRow(test15)]
        [DataRow(test16)]
        [DataRow(test17)]
        public void Valid_input_has_validation_succeed(string input) => Assert.IsTrue(Luhn.Validate(input));

        [TestMethod]
        public void Invalid_input_has_validation_fail() => Assert.IsFalse(Luhn.Validate(ko1));

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Empty_input_has_validate_throw() => Luhn.Validate("");

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Invalid_input_has_compute_throw() => Luhn.Compute(".123foo456:");

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Invalid_input_has_validate_throw() => Luhn.Validate("*789baz012_");
    }
}
