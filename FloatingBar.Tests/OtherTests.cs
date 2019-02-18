using System;
using NUnit.Framework;

namespace FloatingBar.Tests
{
    [TestFixture]
    public class OtherTests
    {
        readonly Random rnd = new Random();
        const int fuzzcount = 1000;

        [Test]
        public void truncating_doubles () {
            for (int i = 0; i < fuzzcount; i++) {
                var original = (rnd.NextDouble() - 0.5) * rnd.Next(1, Rational32.MaxInt);

                var encoded = BitConverter.DoubleToInt64Bits(original);
                encoded = (encoded >> 8) & 0x00FF_FFFF_FFFF_FFFFL; // reduce to 42 bits

                var result = BitConverter.Int64BitsToDouble(encoded << 8);

                // we can only approximate the exact float value
                var scale = original / result;

                Console.WriteLine($"Original = {original}; Result = {result}; Scale = {scale}; (attempt {i})");
            }
        }
    }
}