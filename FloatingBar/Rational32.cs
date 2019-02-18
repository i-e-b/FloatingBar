using System;
// ReSharper disable BuiltInTypeReferenceStyle

namespace FloatingBar
{
    /// <summary>
    /// 32-bit floating rational number
    /// </summary>
    public struct Rational32
    {
        // Bit fields
        const UInt32 SignBit   = 0x80000000;
        const UInt32 SizeBits  = 0x7C000000;
        const UInt32 ValueBits = 0x03FFFFFF;
        
        /// <summary>
        /// Largest representable integer
        /// </summary>
        public const int MaxInt = 67108863; // Same as `ValueBits`
        const double Sigma = 1.5E-07; // Minimum fractional value
        const int FractionSize = 26;

        // The value itself:
        internal UInt32 Value;

        /// <summary>
        /// Make a rational directly from a bit pattern
        /// </summary>
        internal Rational32(UInt32 bitPattern)
        {
            Value = bitPattern;
        }

        /// <summary>
        /// Build a rational from parts
        /// </summary>
        internal Rational32(bool isNegative, UInt32 numerator, UInt32 denominator)
        {
            var len = 32 - LeadingZeros(denominator) - 1;
            var dfield = (1 << len) - 1;

            Value = (isNegative) ? (SignBit) : 0;
            Value |= (uint)(len << FractionSize);
            Value |= (uint)((((dfield ^ ValueBits) >> len) & numerator) << len);
            Value |= (uint)(denominator & dfield);
        }

        private static int LeadingZeros(uint x)
        {
            uint n = 32;  

            var y = x >> 16;
            if (y != 0) { n = n - 16; x = y; }

            y = x >> 8;
            if (y != 0) { n = n - 8; x = y; }

            y = x >> 4;
            if (y != 0) { n = n - 4; x = y; }

            y = x >> 2;
            if (y != 0) { n = n - 2; x = y; }

            y = x >> 1;
            return (int)((y != 0) ? (n - 2) : (n - x));
        }

        /// <summary>
        /// Return the single invalid rational "Not-a-Number"
        /// </summary>
        public Rational32 NaN { get { return new Rational32(SizeBits); } }

        /// <summary>
        /// Return a rational version of an integer. If no more than the 26 least significant bits of the input are used,
        /// the conversion will be lossless
        /// </summary>
        public static Rational32 FromInt(int i)
        {
            bool neg = i < 0;
            if (neg) i = -i;
            if (i > MaxInt) i = MaxInt;
            return new Rational32(neg, (uint)i, 1);
        }

        /// <summary>
        /// Return an approximation of this rational number
        /// </summary>
        public double ToFloat()
        {
            return (Numerator() / (double)(Denominator())) * Sign();
        }

        /// <summary>
        /// Returns -1 if the rational is negative, 1 if the rational is positive, and 0 for zero or NaN
        /// </summary>
        public int Sign()
        {
            if (Numerator() == 0 || IsNaN()) { return 0; }
            return (Value & SignBit) > 0 ? -1 : 1;
        }

        /// <summary>
        /// Returns true if this Rational is invalid
        /// </summary>
        public bool IsNaN()
        {
            return Value == SizeBits || DenominatorSize() > FractionSize;
        }

        /// <summary>
        /// Returns the reciprocal of this Rational32
        /// </summary>
        public Rational32 Reciprocal()
        {
            if (DenominatorSize() >= 26) return NaN; // would result in overflow
            var num = Numerator();
            if (num == 0) return NaN; // divide by zero

            return new Rational32(IsNegative(), (uint)Denominator(), (uint)num); // swap numerator and denominator
        }

        /// <summary>
        /// Returns true if this is a negative number
        /// </summary>
        public bool IsNegative()
        {
            return Sign() < 0;
        }

        private int DenominatorSize()
        {
            return (int)((Value & SizeBits) >> FractionSize);
        }

        /// <summary>
        /// Returns the denominator of this rational
        /// </summary>
        private int Denominator()
        {
            var dsize = DenominatorSize();
            var denom_region = (1 << dsize) - 1;
            return (int)(Value & denom_region) | (1 << dsize);
        }

        /// <summary> Returns the denominator of this rational, expanded to a 64 bit int </summary>
        public long Denominator64() => Denominator();

        /// <summary>
        /// Returns the numerator of this rational
        /// </summary>
        public int Numerator()
        {
            var dsize = DenominatorSize();

            if (dsize == FractionSize) return 1;

            return (int)((Value & ValueBits) >> dsize);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Numerator()}/{Denominator()}";
        }

        /// <summary> Returns the numerator of this rational, expanded to a 64 bit int </summary>
        public long Numerator64() => Numerator();

        /// <summary>
        /// Add two rationals
        /// </summary>
        public static Rational32 Add(Rational32 a, Rational32 b) {
            // self = a/b, other = c/d
        
            var a_sign = a.Sign();
            var b_sign = b.Sign();

            var num = (a.Numerator64() * a_sign) * b.Denominator64();
            num += a.Denominator64() * (b.Numerator64() * b_sign);

            var den = a.Denominator64() * b.Denominator64();
            var sign = num < 0;

            if (sign) { num = -num; }

            // Try to find an exact result
            var gcd = GCD(num, den);
            num /= gcd;
            den /= gcd;
            
            // Reduce precision until it fits
            while (num > MaxInt || WouldOverflow((uint)num, (uint)den)) {
                gcd = GCD(num, den);
                if (gcd == 1) {
                    num /= 2;
                    den /= 2;
                } else {
                    num /= gcd;
                    den /= gcd;
                }
                if (den < 1) {
                    den = 1;
                    break;
                }
            }

            return new Rational32(sign, (uint)num, (uint)den);
        }

        private static bool WouldOverflow(uint num, uint den)
        {
            var l1 = 32 - LeadingZeros(num);
            var l2 = 32 - LeadingZeros(den);

            var bitsUsed = l1 + l2;

            return bitsUsed > 26;
        }

        private static long GCD(long a, long b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b) a %= b;
                else b %= a;
            }

            return a == 0 ? b : a;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Rational32 FromFloat(double f)
        {
            // The plan: split float into int and frac parts
            // With frac, invert and build rational. Invert that rational and add int part

            var sign = f < 0;
            if (sign) f = -f;

            var num = (long)f;
            var frac = f - num;

            if (Math.Abs(frac) < Sigma) return new Rational32(sign, (uint)num, 1); // close enough to an integer

            var top = new Rational32(sign, (uint)num, 1);
            var inv = new Rational32(sign, 1, (uint)(1/frac));

            return Add(top, inv);
        }
    }
}
