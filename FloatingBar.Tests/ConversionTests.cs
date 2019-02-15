using System;
using NUnit.Framework;

namespace FloatingBar.Tests
{
    [TestFixture]
    public class ConversionTests
    {
        [Test]
        public void can_round_trip_a_rational_float () {
            var original = 500.125;

            var subject = Rational32.FromFloat(original);
            Console.WriteLine("Ended up with " + subject);

            var result = subject.ToFloat();

            Assert.That(result, Is.EqualTo(original));
        }

        [Test]
        public void can_convert_an_integer_into_a_rational_into_a_float () {
            var subject = Rational32.FromInt(1234);

            var result = subject.ToFloat();

            Assert.That(result, Is.EqualTo(1234));
        }
        
        [Test]
        public void can_convert_a_large_integer_into_a_rational_into_a_float () {
            var max = (1 << 26) - 1;
            var subject = Rational32.FromInt(max);

            var result = subject.ToFloat();

            Assert.That(result, Is.EqualTo(max));
        }
        
        [Test]
        public void converting_an_oversize_int_results_in_saturation () {
            var max = (1 << 26) - 1;
            var subject = Rational32.FromInt(123456789);

            var result = subject.ToFloat();

            Assert.That(result, Is.EqualTo(max));
        }

        [Test]
        public void converting_an_negative_oversize_int_results_in_saturation () {
            var max = (1 << 26) - 1;
            var subject = Rational32.FromInt(-123456789);

            var result = subject.ToFloat();

            Assert.That(result, Is.EqualTo(-max));
        }
    }
}
