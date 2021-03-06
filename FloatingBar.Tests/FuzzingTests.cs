﻿using System;
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
            double sum = 0.0;
            double upper = 0.0;
            double lower = 2.0;
            for (int i = 0; i < fuzzcount; i++) {
                var original = (float)((rnd.NextDouble() - 0.5) * rnd.Next(1, Rational32.MaxInt / 8));
                var subject = Rational32.FromFloat(original);
                var result = subject.ToFloat();

                // we can only approximate the exact float value
                var scale = original / result;
                sum += scale;

                upper = Math.Max(scale, upper);
                lower = Math.Min(scale, lower);

                Assert.That(scale, Is.LessThan(1.3), $"Original = {original}; Result = {result}; Scale = {scale}; (attempt {i})");
                Assert.That(scale, Is.GreaterThan(0.7), $"Original = {original}; Result = {result}; Scale = {scale}; (attempt {i})");
            }

            Console.WriteLine($"Average = {sum/fuzzcount}; Upper = {upper}; Lower = {lower};");
        }
    }
}