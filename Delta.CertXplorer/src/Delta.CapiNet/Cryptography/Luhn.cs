using System;
using System.Linq;

namespace Delta.CapiNet.Cryptography
{
    /// <summary>
    /// Implementation of the Luhn checksum algorithm according to
    /// http://fr.wikipedia.org/wiki/Formule_de_Luhn and roughly based 
    /// on code found here: http://www.codeproject.com/Tips/515367/Validate-credit-card-number-with-Mod-10-algorithm
    /// </summary>
    public static class Luhn
    {
        /// <summary>
        /// Given the specified input string, computes the digit that should be appended in 
        /// order to obtain a Luhn compliant string.
        /// Computes the specified input.
        /// </summary>
        /// <param name="input">The input string (must only contain digits).</param>
        /// <returns>The Luhn Checksum.</returns>
        public static int Compute(string input)
        {
            var checksum = 10 - ComputeLuhnSum(input + "0") % 10;
            return checksum == 10 ? 0 : checksum;
        }

        /// <summary>
        /// Validates that the specified input matches the Luhn checksum test.
        /// </summary>
        /// <param name="input">The input string (must only contain digits).</param>
        /// <returns><c>True</c> if the checksum verification succeeds; otherwise, <c>false</c>.</returns>
        public static bool Validate(string input)
        {
            return ComputeLuhnSum(input) % 10 == 0;
        }

        private static int ComputeLuhnSum(string input)
        {
            EnsureInputIsValid(input);

            Func<int, int> reduce = i =>
            {
                while (i >= 10)
                    i = i / 10 + i % 10;
                return i;
            };

#if DEBUG
            var digits = input.Select(c => c - '0').ToArray();
            var revert = digits.Reverse().ToArray();
            var doubled = revert.Select((e, index) => index % 2 == 0 ? e : reduce(e * 2)).ToArray();
            var sum = doubled.Sum();

            return sum;
#else
            return input
                .Select(c => c - '0')
                .Reverse()
                .Select((e, index) => index % 2 == 0 ? e : reduce(e * 2))
                .Sum();
#endif
        }

        private static void EnsureInputIsValid(string input)
        {
            if (string.IsNullOrEmpty(input)) throw new ArgumentNullException("input");

            if (input.Any(c => c < '0' || c > '9')) throw new ArgumentException(
                "The specified input string must contain only digits.", "input");
        }
    }
}
