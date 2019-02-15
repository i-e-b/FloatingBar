using System;
using NUnit.Framework;
// ReSharper disable PossibleNullReferenceException

namespace FloatingBar.Tests
{
    [TestFixture]
    public class FuzzingTests
    {
        readonly Random rnd = new Random();
        const int fuzzcount = 1000;

        [Test]
        public void floating_point_conversion () {
            for (int i = 0; i < fuzzcount; i++) {
                var original = (rnd.NextDouble() - 0.5) * rnd.Next(1, Rational32.MaxInt / 8);
                var subject = Rational32.FromFloat(original);
                var result = subject.ToFloat();

                // we can only approximate the exact float value
                var scale = original / result;

                Assert.That(scale, Is.LessThan(1.3), $"Original = {original}; Result = {result}; Scale = {scale}; (attempt {i})");
                Assert.That(scale, Is.GreaterThan(0.7), $"Original = {original}; Result = {result}; Scale = {scale}; (attempt {i})");
            }
        }
    }
}