using NUnit.Framework;

namespace FloatingBar.Tests
{
    [TestFixture]
    public class OperationTests
    {
        [Test]
        public void reciprocals_work_as_expected () {
            Assert.That(Rational32.FromInt(1).Reciprocal().ToFloat(), Is.EqualTo(1));
            Assert.That(Rational32.FromInt(2).Reciprocal().ToFloat(), Is.EqualTo(0.5));
            Assert.That(Rational32.FromInt(4).Reciprocal().ToFloat(), Is.EqualTo(0.25));
        }

        [Test]
        public void adding_works () {

        }
        
        [Test]
        public void reciprocal_of_zero_is_NAN () {
            Assert.That(Rational32.FromInt(0).Reciprocal().IsNaN(), Is.True);
        }
    }
}