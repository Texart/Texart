using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Texart.Api.Tests
{
    [TestFixture]
    internal class TxArgumentsTests
    {
        private struct AllowsExtractionExtractable : TxArguments.IExtractable
        {
            public int Foo { get; private set; }
            void TxArguments.IExtractable.Extract(TxArguments args)
            {
                Foo = args.GetInt("foo");
            }
        }

        [Test]
        public void AllowsNumericGet()
        {
            const string key = "foo";
            const int value = 42;
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, value.ToString() },
                { "extra-key", "extra-value" }
            });
            Assert.AreEqual(value, args.GetByte(key));
            Assert.AreEqual(value, args.GetSByte(key));
            Assert.AreEqual(value, args.GetShort(key));
            Assert.AreEqual(value, args.GetUShort(key));
            Assert.AreEqual(value, args.GetInt(key));
            Assert.AreEqual(value, args.GetUInt(key));
            Assert.AreEqual(value, args.GetLong(key));
            Assert.AreEqual(value, args.GetULong(key));
            Assert.AreEqual(value, args.GetDecimal(key));
            Assert.AreEqual(value, args.GetSingle(key));
            Assert.AreEqual(value, args.GetDouble(key));
        }

        [Test]
        public void HasCorrectSignedNumericGet()
        {
            const string key = "foo";
            const int value = -42;
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, value.ToString() },
                { "extra-key", "extra-value" }
            });
            
            Assert.AreEqual(value, args.GetSByte(key));
            Assert.AreEqual(value, args.GetShort(key));
            Assert.AreEqual(value, args.GetInt(key));
            Assert.AreEqual(value, args.GetLong(key));
            Assert.AreEqual(value, args.GetDecimal(key));
            Assert.AreEqual(value, args.GetSingle(key));
            Assert.AreEqual(value, args.GetDouble(key));

            Assert.Throws<ArgumentException>(() => args.GetByte(key));
            Assert.Throws<ArgumentException>(() => args.GetUShort(key));
            Assert.Throws<ArgumentException>(() => args.GetUInt(key));
            Assert.Throws<ArgumentException>(() => args.GetULong(key));
        }

        [Test]
        public void HasCorrectDecimalGet()
        {
            const string key = "foo";
            const decimal value = 43.25m;
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, value.ToString(CultureInfo.InvariantCulture) },
                { "extra-key", "extra-value" }
            });
            Assert.AreEqual(value, args.GetDecimal(key));
        }

        [Test]
        public void RejectsHexNumeric()
        {
            const string key = "foo";
            const string value = "0xFF";
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, value },
                { "extra-key", "extra-value" }
            });
            Assert.Throws<ArgumentException>(() => args.GetByte(key));
            Assert.Throws<ArgumentException>(() => args.GetSByte(key));
            Assert.Throws<ArgumentException>(() => args.GetShort(key));
            Assert.Throws<ArgumentException>(() => args.GetUShort(key));
            Assert.Throws<ArgumentException>(() => args.GetInt(key));
            Assert.Throws<ArgumentException>(() => args.GetUInt(key));
            Assert.Throws<ArgumentException>(() => args.GetLong(key));
            Assert.Throws<ArgumentException>(() => args.GetULong(key));
            Assert.Throws<ArgumentException>(() => args.GetDecimal(key));
            Assert.Throws<ArgumentException>(() => args.GetSingle(key));
            Assert.Throws<ArgumentException>(() => args.GetDouble(key));
        }

        [Test]
        public void RejectsScientificNumeric()
        {
            const string key = "foo";
            const string value = "1e3";
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, value },
                { "extra-key", "extra-value" }
            });
            Assert.Throws<ArgumentException>(() => args.GetByte(key));
            Assert.Throws<ArgumentException>(() => args.GetSByte(key));
            Assert.Throws<ArgumentException>(() => args.GetShort(key));
            Assert.Throws<ArgumentException>(() => args.GetUShort(key));
            Assert.Throws<ArgumentException>(() => args.GetInt(key));
            Assert.Throws<ArgumentException>(() => args.GetUInt(key));
            Assert.Throws<ArgumentException>(() => args.GetLong(key));
            Assert.Throws<ArgumentException>(() => args.GetULong(key));
            Assert.Throws<ArgumentException>(() => args.GetDecimal(key));
        }

        [Test]
        public void AllowsScientificFloatingPoint()
        {
            const string key = "foo";
            const int value = 1000;
            const string scientificValue = "1e3";
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, scientificValue },
                { "extra-key", "extra-value" }
            });
            Assert.AreEqual(value, args.GetSingle(key));
            Assert.AreEqual(value, args.GetDouble(key));
        }

        [Test]
        public void RejectsOctalNumeric()
        {
            const string key = "foo";
            const int value = 42;
            const string octalValue = "042";
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, octalValue },
                { "extra-key", "extra-value" }
            });
            Assert.AreEqual(value, args.GetByte(key));
            Assert.AreEqual(value, args.GetSByte(key));
            Assert.AreEqual(value, args.GetShort(key));
            Assert.AreEqual(value, args.GetUShort(key));
            Assert.AreEqual(value, args.GetInt(key));
            Assert.AreEqual(value, args.GetUInt(key));
            Assert.AreEqual(value, args.GetLong(key));
            Assert.AreEqual(value, args.GetULong(key));
            Assert.AreEqual(value, args.GetDecimal(key));
            Assert.AreEqual(value, args.GetSingle(key));
            Assert.AreEqual(value, args.GetDouble(key));
        }

        [Test]
        public void AllowsStringGet()
        {
            const string key = "foo";
            const int value = 42;
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, value.ToString() },
                { "extra-key", "extra-value" }
            });
            Assert.AreEqual(value.ToString(), args.GetString(key));
            Assert.AreEqual("extra-value", args.GetString("extra-key"));
        }

        [Test]
        public void HasCorrectBoolGet()
        {
            var args = new TxArguments(new Dictionary<string, string>
            {
                { "b1", "true" },
                { "b2", "TRUE" },
                { "b3", "false" },
                { "b4", "FALSE" },
                { "b5", "1" },
                { "b6", "0" },
                { "foo", "42" },
                { "extra-key", "extra-value" }
            });
            Assert.IsTrue(args.GetBool("b1"));
            Assert.IsTrue(args.GetBool("b2"));
            Assert.IsFalse(args.GetBool("b3"));
            Assert.IsFalse(args.GetBool("b4"));
            Assert.Throws<ArgumentException>(() => args.GetBool("b5"));
            Assert.Throws<ArgumentException>(() => args.GetBool("b6"));

            Assert.Throws<ArgumentException>(() => args.GetBool("foo"));
            Assert.Throws<ArgumentException>(() => args.GetBool("extra-key"));
        }

        [Test]
        public void HasCorrectCharGet()
        {
            var args = new TxArguments(new Dictionary<string, string>
            {
                { "c1", "t" },
                { "c2", "1" },
                { "c3", "t1" },
                { "c4", "" },
                { "foo", "42" },
                { "extra-key", "extra-value" }
            });
            Assert.AreEqual('t', args.GetChar("c1"));
            Assert.AreEqual('1', args.GetChar("c2"));
            Assert.Throws<ArgumentException>(() => args.GetChar("c3"));
            Assert.Throws<ArgumentException>(() => args.GetChar("c4"));

            Assert.Throws<ArgumentException>(() => args.GetChar("foo"));
            Assert.Throws<ArgumentException>(() => args.GetChar("extra-key"));
        }

        [Test]
        public void AllowsGetWithDefault()
        {
            const string missingKey = "foo";
            const int defaultValue = 42;
            var args = new TxArguments(new Dictionary<string, string>
            {
                { "extra-key", "extra-value" }
            });
            Assert.AreEqual(defaultValue, args.GetByte(missingKey, defaultValue));
            Assert.AreEqual(defaultValue, args.GetSByte(missingKey, defaultValue));
            Assert.AreEqual(defaultValue, args.GetShort(missingKey, defaultValue));
            Assert.AreEqual(defaultValue, args.GetUShort(missingKey, defaultValue));
            Assert.AreEqual(defaultValue, args.GetInt(missingKey, defaultValue));
            Assert.AreEqual(defaultValue, args.GetUInt(missingKey, defaultValue));
            Assert.AreEqual(defaultValue, args.GetLong(missingKey, defaultValue));
            Assert.AreEqual(defaultValue, args.GetULong(missingKey, defaultValue));
            Assert.AreEqual(defaultValue, args.GetDecimal(missingKey, defaultValue));
            Assert.AreEqual(defaultValue, args.GetSingle(missingKey, defaultValue));
            Assert.AreEqual(defaultValue, args.GetDouble(missingKey, defaultValue));
            Assert.AreEqual('a', args.GetChar(missingKey, 'a'));
            Assert.AreEqual("a", args.GetString(missingKey, "a"));
            Assert.IsTrue(args.GetBool(missingKey, true));
        }

        [Test]
        public void IgnoresDefaultValueIfKeyExists()
        {
            const string key = "foo";
            const int value = 5;
            const int defaultValue = 42;
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, value.ToString() },
                { "extra-key", "extra-value" }
            });
            Assert.AreEqual(value, args.GetByte(key, defaultValue));
            Assert.AreEqual(value, args.GetSByte(key, defaultValue));
            Assert.AreEqual(value, args.GetShort(key, defaultValue));
            Assert.AreEqual(value, args.GetUShort(key, defaultValue));
            Assert.AreEqual(value, args.GetInt(key, defaultValue));
            Assert.AreEqual(value, args.GetUInt(key, defaultValue));
            Assert.AreEqual(value, args.GetLong(key, defaultValue));
            Assert.AreEqual(value, args.GetULong(key, defaultValue));
            Assert.AreEqual(value, args.GetDecimal(key, defaultValue));
            Assert.AreEqual(value, args.GetSingle(key, defaultValue));
            Assert.AreEqual(value, args.GetDouble(key, defaultValue));
        }

        [Test]
        public void RejectsGetWithDefaultOnParsingError()
        {
            const string key = "foo";
            const string badValue = "abacus";
            const int defaultValue = 42;
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, badValue },
                { "extra-key", "extra-value" }
            });
            Assert.Throws<ArgumentException>(() => args.GetByte(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetSByte(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetShort(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetUShort(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetInt(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetUInt(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetLong(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetULong(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetDecimal(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetSingle(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetDouble(key, defaultValue));
            Assert.Throws<ArgumentException>(() => args.GetChar(key, 'a'));
            Assert.AreEqual(badValue, args.GetString(key, "a"));
            Assert.Throws<ArgumentException>(() => args.GetBool(key, true));
        }

        private delegate TxArguments.LookupResult TryGetFunc<T>(string key, out T value);

        [Test]
        public void AllowsTryGet()
        {
            const string key = "foo";
            const int value = 42;
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, value.ToString() },
                { "c", "c" },
                { "b", "true" },
                { "extra-key", "extra-value" }
            });

            AssertTryGetSuccess<byte>(args.TryGetValue, key, value);
            AssertTryGetSuccess<sbyte>(args.TryGetValue, key, value);
            AssertTryGetSuccess<short>(args.TryGetValue, key, value);
            AssertTryGetSuccess<ushort>(args.TryGetValue, key, value);
            AssertTryGetSuccess<int>(args.TryGetValue, key, value);
            AssertTryGetSuccess<uint>(args.TryGetValue, key, value);
            AssertTryGetSuccess<long>(args.TryGetValue, key, value);
            AssertTryGetSuccess<ulong>(args.TryGetValue, key, value);
            AssertTryGetSuccess<decimal>(args.TryGetValue, key, value);
            AssertTryGetSuccess<float>(args.TryGetValue, key, value);
            AssertTryGetSuccess<double>(args.TryGetValue, key, value);
            AssertTryGetSuccess<char>(args.TryGetValue, "c", 'c');
            AssertTryGetSuccess<string>(args.TryGetValue, "extra-key", "extra-value");
            AssertTryGetSuccess<bool>(args.TryGetValue, "b", true);

            static void AssertTryGetSuccess<T>(TryGetFunc<T> tryGetFunc, string lookupKey, T expectedValue)
            {
                var lookupResult = tryGetFunc(lookupKey, out var v);
                Assert.AreEqual(TxArguments.LookupResult.Success, lookupResult);
                Assert.AreEqual(expectedValue, v);
            }
        }

        [Test]
        public void RejectsMissingKeyTryGet()
        {
            const string key = "foo";
            const int value = 42;
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, value.ToString() },
                { "c", "c" },
                { "b", "true" },
                { "extra-key", "extra-value" }
            });
            const string missingKey = "missing-key";

            AssertTryGetMissing<byte>(args.TryGetValue, missingKey);
            AssertTryGetMissing<sbyte>(args.TryGetValue, missingKey);
            AssertTryGetMissing<short>(args.TryGetValue, missingKey);
            AssertTryGetMissing<ushort>(args.TryGetValue, missingKey);
            AssertTryGetMissing<int>(args.TryGetValue, missingKey);
            AssertTryGetMissing<uint>(args.TryGetValue, missingKey);
            AssertTryGetMissing<long>(args.TryGetValue, missingKey);
            AssertTryGetMissing<ulong>(args.TryGetValue, missingKey);
            AssertTryGetMissing<decimal>(args.TryGetValue, missingKey);
            AssertTryGetMissing<float>(args.TryGetValue, missingKey);
            AssertTryGetMissing<double>(args.TryGetValue, missingKey);
            AssertTryGetMissing<char>(args.TryGetValue, missingKey);
            AssertTryGetMissing<string>(args.TryGetValue, missingKey);
            AssertTryGetMissing<bool>(args.TryGetValue, missingKey);

            static void AssertTryGetMissing<T>(TryGetFunc<T> tryGetFunc, string lookupKey)
            {
                var lookupResult = tryGetFunc(lookupKey, out _);
                Assert.AreEqual(TxArguments.LookupResult.MissingKey, lookupResult);
            }
        }

        [Test]
        public void RejectsParsingErrorTryGet()
        {
            const string key = "foo";
            const string value = "bad-numeric-value";
            var args = new TxArguments(new Dictionary<string, string>
            {
                { key, value },
                { "c", "too-many-chars" },
                { "b", "not-a-bool" },
                { "extra-key", "extra-value" }
            });

            AssertTryGetParsingError<byte>(args.TryGetValue, key);
            AssertTryGetParsingError<sbyte>(args.TryGetValue, key);
            AssertTryGetParsingError<short>(args.TryGetValue, key);
            AssertTryGetParsingError<ushort>(args.TryGetValue, key);
            AssertTryGetParsingError<int>(args.TryGetValue, key);
            AssertTryGetParsingError<uint>(args.TryGetValue, key);
            AssertTryGetParsingError<long>(args.TryGetValue, key);
            AssertTryGetParsingError<ulong>(args.TryGetValue, key);
            AssertTryGetParsingError<decimal>(args.TryGetValue, key);
            AssertTryGetParsingError<float>(args.TryGetValue, key);
            AssertTryGetParsingError<double>(args.TryGetValue, key);
            AssertTryGetParsingError<char>(args.TryGetValue, "c");
            AssertTryGetParsingError<bool>(args.TryGetValue, "b");

            static void AssertTryGetParsingError<T>(TryGetFunc<T> tryGetFunc, string lookupKey)
            {
                var lookupResult = tryGetFunc(lookupKey, out _);
                Assert.AreEqual(TxArguments.LookupResult.ParsingError, lookupResult);
            }
        }

        [Test]
        public void AllowsCustomParseFunc()
        {

        }

        [Test]
        public void HandlesMissingKeys()
        {

        }

        [Test]
        public void HasCorrectEquality()
        {

        }

        [Test]
        public void HasCorrectInequality()
        {

        }

        [Test]
        public void HasCorrectHashCode()
        {

        }

        [Test]
        public void AllowsExtraction()
        {
            var args = new TxArguments(new Dictionary<string, string>{ {"foo", "42"} });
            var e = args.Extract<AllowsExtractionExtractable>();
            Assert.AreEqual(42, e.Foo);
        }
    }
}