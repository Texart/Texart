using System.Collections.Generic;
using NUnit.Framework;
using SkiaSharp;

namespace Texart.Api.Tests
{
    [TestFixture]
    public class TxBitmapTests
    {
        [Test]
        public void HasCorrectPerfectSamplingFactors()
        {
            AssertSetEquals(
                new [] { 1, 2, 4, 5, 8, 10, 20, 25, 40, 50, 100, 125, 200, 250, 500, 1000 },
                TxBitmap.PerfectSamplingFactors(1000, 1000));
            AssertSetEquals(
                new[] { 1, 2, 4, 5, 10, 20, 25, 50, 100, 125, 250, 500 },
                TxBitmap.PerfectSamplingFactors(1000, 500));
            // primes
            AssertSetEquals(
                new[] { 1 },
                TxBitmap.PerfectSamplingFactors(79, 89));
            // same primes
            AssertSetEquals(
                new[] { 1, 79 },
                TxBitmap.PerfectSamplingFactors(79, 79));
        }

        private static void AssertSetEquals<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            Assert.IsTrue(new HashSet<T>(a).SetEquals(new HashSet<T>(b)));
        }

        [Test]
        public void ChoosesBestSamplingFactorByEuclideanDistance()
        {
            Assert.AreEqual(
                4, // closer to 250 than 500
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(374, 374), 1.0f,
                    TxBitmap.SamplingFactorCloseness.EuclideanDistance));
            Assert.AreEqual(
                2, // closer to 500 than 250
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(376, 376), 1.0f,
                    TxBitmap.SamplingFactorCloseness.EuclideanDistance));
            Assert.AreEqual(
                8, // perfect match
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(125, 125), 1.0f,
                    TxBitmap.SamplingFactorCloseness.EuclideanDistance));
            Assert.AreEqual(
                10, // 100 is middle of 50 and 150
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(50, 150), 1.0f,
                    TxBitmap.SamplingFactorCloseness.EuclideanDistance));
        }

        [Test]
        public void ChoosesBestSamplingFactorBySmallerThanTarget()
        {
            Assert.AreEqual(
                4, // 250 since 500 is too large
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(499, 499), 1.0f,
                    TxBitmap.SamplingFactorCloseness.SmallerThanTarget));
            Assert.AreEqual(
                2, // 500 since 1000 is too large
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(501, 501), 1.0f,
                    TxBitmap.SamplingFactorCloseness.SmallerThanTarget));
            Assert.AreEqual(
                2, // perfect match
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(500, 500), 1.0f,
                    TxBitmap.SamplingFactorCloseness.SmallerThanTarget));
            Assert.AreEqual(
                4, // 250 since 500 is too large for 499
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(501, 499), 1.0f,
                    TxBitmap.SamplingFactorCloseness.SmallerThanTarget));

            // Font size scaling

            Assert.AreEqual(
                4, // 250 since 500 is too large
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(100, 100),
                    new SKSizeI(499, 499), 10.0f,
                    TxBitmap.SamplingFactorCloseness.SmallerThanTarget));
            Assert.AreEqual(
                2, // 500 since 1000 is too large
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(250, 250),
                    new SKSizeI(501, 501), 4.0f,
                    TxBitmap.SamplingFactorCloseness.SmallerThanTarget));
            Assert.AreEqual(
                2, // perfect match
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(40, 40),
                    new SKSizeI(500, 500), 25.0f,
                    TxBitmap.SamplingFactorCloseness.SmallerThanTarget));

            Assert.AreEqual(
                1000, // no smaller dimension is possible
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(0, 0), 1.0f,
                    TxBitmap.SamplingFactorCloseness.SmallerThanTarget));
        }

        [Test]
        public void ChoosesBestSamplingFactorByLargerThanTarget()
        {
            Assert.AreEqual(
                2, // 500 since 250 is too small
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(499, 499), 1.0f,
                    TxBitmap.SamplingFactorCloseness.LargerThanTarget));
            Assert.AreEqual(
                1, // 1000 since 500 is too small
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(501, 501), 1.0f,
                    TxBitmap.SamplingFactorCloseness.LargerThanTarget));
            Assert.AreEqual(
                2, // perfect match
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(500, 500), 1.0f,
                    TxBitmap.SamplingFactorCloseness.LargerThanTarget));
            Assert.AreEqual(
                1, // 1000 since 500 is too small for 501
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(499, 501), 1.0f,
                    TxBitmap.SamplingFactorCloseness.LargerThanTarget));

            // Font size scaling

            Assert.AreEqual(
                2, // 500 since 250 is too small
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(100, 100),
                    new SKSizeI(499, 499), 10.0f,
                    TxBitmap.SamplingFactorCloseness.LargerThanTarget));
            Assert.AreEqual(
                1, // 1000 since 500 is too small
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(125, 125),
                    new SKSizeI(501, 501), 8.0f,
                    TxBitmap.SamplingFactorCloseness.LargerThanTarget));
            Assert.AreEqual(
                2, // perfect match
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(40, 40),
                    new SKSizeI(500, 500), 25.0f,
                    TxBitmap.SamplingFactorCloseness.LargerThanTarget));

            Assert.AreEqual(
                1, // no larger dimension is possible
                TxBitmap.BestSamplingFactor(
                    new SKSizeI(1000, 1000),
                    new SKSizeI(1001, 1001), 1.0f,
                    TxBitmap.SamplingFactorCloseness.LargerThanTarget));
        }
    }
}