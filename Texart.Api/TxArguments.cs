using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

// TODO: Enable nullable references when generics work well with it
#nullable disable

namespace Texart.Api
{
    /// <summary>
    /// A <see cref="TxArguments"/> is used to pass arguments to <see cref="ITxPlugin"/>. The keys and values are
    /// both stored as <see cref="string"/>.
    /// Accessor methods, such as <see cref="GetValue{T}(string,TryParseFunc{T},T)"/> and
    /// <see cref="TryGetValue{T}(string,out T, TryParseFunc{T}"/> are provided to extract and parse values to any type.
    /// Helper methods are provided for accessing primitive types (<see cref="Type.IsPrimitive"/>) and
    /// <see cref="string"/>.
    /// A <see cref="TxArguments"/> is immutable.
    /// </summary>
    public sealed class TxArguments : IEquatable<TxArguments>
    {
        /// <summary>
        /// An <see cref="IExtractable"/> is an object that is able to extract values out of a <see cref="TxArguments"/>
        /// instance.
        ///
        /// This API requires the implementing class to be mutable in order to (meaningfully) extract values from
        /// arguments. Caution should be exercised if the extracted object needs to be stored beyond the scope of its declaration.
        /// If the extracted values need to be persisted, it is recommended to place them into a different (but very similar) type
        /// to <c>this</c> which IS immutable.
        /// </summary>
        public interface IExtractable
        {
            /// <summary>
            /// Extracts arguments from <paramref name="args"/> and modifies <c>this</c> accordingly.
            /// </summary>
            /// <param name="args">Arguments to extract values from.</param>
            void Extract(TxArguments args);
        }

        /// <summary>
        /// Extracts arguments from <c>this</c> via the <see cref="IExtractable.Extract(TxArguments)"/> of
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type that defines how to extract values.</typeparam>
        /// <returns>Extracted arguments</returns>
        /// <seealso cref="IExtractable"/>
        /// <seealso cref="Extract{T}(ref T)"/>
        public T Extract<T>() where T : struct, IExtractable
        {
            var value = new T();
            Extract(ref value);
            return value;
        }

        /// <summary>
        /// Extracts arguments from <c>this</c> via the <see cref="IExtractable.Extract(TxArguments)"/> method of
        /// <paramref name="value"/>. This will place the values into an existing value, referenced by <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The destination into which extracted argument values will be placed.</param>
        /// <typeparam name="T">The type that defines how to extract values.</typeparam>
        /// <seealso cref="IExtractable"/>
        /// <seealso cref="Extract{T}()"/>
        public void Extract<T>(ref T value) where T : struct, IExtractable => value.Extract(this);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="sbyte"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="sbyte"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="sbyte"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="sbyte"/>.
        /// </exception>
        /// <seealso cref="GetSByte(string,sbyte)"/>
        /// <seealso cref="TryGetValue(string,out sbyte)"/>
        public sbyte GetSByte(string key) => GetValue<sbyte>(key, sbyte.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="byte"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="byte"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="byte"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="byte"/>.
        /// </exception>
        /// <seealso cref="GetByte(string,byte)"/>
        /// <seealso cref="TryGetValue(string,out byte)"/>
        public byte GetByte(string key) => GetValue<byte>(key, byte.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="short"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="short"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="short"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="short"/>.
        /// </exception>
        /// <seealso cref="GetShort(string,short)"/>
        /// <seealso cref="TryGetValue(string,out short)"/>
        public short GetShort(string key) => GetValue<short>(key, short.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="ushort"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="ushort"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="ushort"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="ushort"/>.
        /// </exception>
        /// <seealso cref="GetUShort(string,ushort)"/>
        /// <seealso cref="TryGetValue(string,out ushort)"/>
        public ushort GetUShort(string key) => GetValue<ushort>(key, ushort.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="int"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="int"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="int"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="int"/>.
        /// </exception>
        /// <seealso cref="GetInt(string,int)"/>
        /// <seealso cref="TryGetValue(string,out int)"/>
        public int GetInt(string key) => GetValue<int>(key, int.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="uint"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="uint"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="uint"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="uint"/>.
        /// </exception>
        /// <seealso cref="GetUInt(string,uint)"/>
        /// <seealso cref="TryGetValue(string,out uint)"/>
        public uint GetUInt(string key) => GetValue<uint>(key, uint.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="long"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="long"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="long"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="long"/>.
        /// </exception>
        /// <seealso cref="GetLong(string,long)"/>
        /// <seealso cref="TryGetValue(string,out long)"/>
        public long GetLong(string key) => GetValue<long>(key, long.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="ulong"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="ulong"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="ulong"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="ulong"/>.
        /// </exception>
        /// <seealso cref="GetULong(string,ulong)"/>
        /// <seealso cref="TryGetValue(string,out ulong)"/>
        public ulong GetULong(string key) => GetValue<ulong>(key, ulong.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="char"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="char"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="char"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="char"/>.
        /// </exception>
        /// <seealso cref="GetChar(string,char)"/>
        /// <seealso cref="TryGetValue(string,out char)"/>
        public char GetChar(string key) => GetValue<char>(key, char.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="float"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="float"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="float"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="float"/>.
        /// </exception>
        /// <seealso cref="GetSingle(string,float)"/>
        /// <seealso cref="TryGetValue(string,out float)"/>
        public float GetSingle(string key) => GetValue<float>(key, float.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="double"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="double"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="double"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="double"/>.
        /// </exception>
        /// <seealso cref="GetDouble(string,double)"/>
        /// <seealso cref="TryGetValue(string,out double)"/>
        public double GetDouble(string key) => GetValue<double>(key, double.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="bool"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="bool"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="bool"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="bool"/>.
        /// </exception>
        /// <seealso cref="GetBool(string,bool)"/>
        /// <seealso cref="TryGetValue(string,out bool)"/>
        public bool GetBool(string key) => GetValue<bool>(key, bool.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="decimal"/>. If the key is not found, or the
        /// associated value cannot be converted to <see cref="decimal"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="decimal"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <see cref="decimal"/>.
        /// </exception>
        /// <seealso cref="GetDecimal(string,decimal)"/>
        /// <seealso cref="TryGetValue(string,out decimal)"/>
        public decimal GetDecimal(string key) => GetValue<decimal>(key, decimal.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="string"/>.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with <paramref name="key"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">If <paramref name="key"/> does not exist.</exception>
        /// <seealso cref="GetString(string,string)"/>
        /// <seealso cref="TryGetValue(string,out string)"/>
        public string GetString(string key)
        {
            if (key is null) { throw new ArgumentNullException(nameof(key)); }
            return AsImmutableDictionary.TryGetValue(key, out var value)
                ? value
                : throw new BadLookupException(LookupResult.MissingKey, $"No key found: \"{key}\"");
        }

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <typeparamref name="T"/>. If the key is not found, or the
        /// associated value cannot be converted to <typeparamref name="T"/> (via <paramref name="tryParse"/>,
        /// an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="tryParse">The conversion function from <c>string</c> to <typeparamref name="T"/>.</param>
        /// <typeparam name="T">The desired return type.</typeparam>
        /// <returns>The value associated with <paramref name="key"/>, converted to <typeparamref name="T"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> does not exist,
        ///     or the associated value cannot not be converted to <typeparamref name="T"/>.
        /// </exception>
        /// <seealso cref="GetValue{T}(string,TryParseFunc{T},T)"/>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetSByte(string)"/>
        /// <seealso cref="GetByte(string)"/>
        /// <seealso cref="GetShort(string)"/>
        /// <seealso cref="GetUShort(string)"/>
        /// <seealso cref="GetInt(string)"/>
        /// <seealso cref="GetUInt(string)"/>
        /// <seealso cref="GetLong(string)"/>
        /// <seealso cref="GetULong(string)"/>
        /// <seealso cref="GetChar(string)"/>
        /// <seealso cref="GetSingle(string)"/>
        /// <seealso cref="GetDouble(string)"/>
        /// <seealso cref="GetBool(string)"/>
        public T GetValue<T>(string key, TryParseFunc<T> tryParse)
        {
            if (key is null) { throw new ArgumentNullException(nameof(key)); }
            Debug.Assert(tryParse != null);
            var lookupResult = TryGetValue(key, out var value, tryParse);
            switch (lookupResult.Type)
            {
                case LookupResultType.Success:
                    return value;
                case LookupResultType.MissingKey:
                    throw new BadLookupException(LookupResult.MissingKey,$"No key found: \"{key}\"");
                case LookupResultType.ParsingError:
                    throw new BadLookupException(
                        LookupResult.ParsingError,
                        $"Value for key, \"{key}\", could not be parsed as {typeof(T).FullName}: {AsImmutableDictionary[key]}");
            }
            return value;
        }

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="sbyte"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="sbyte"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="sbyte"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="sbyte"/>.
        /// </exception>
        /// <seealso cref="GetSByte(string)"/>
        /// <seealso cref="TryGetValue(string,out sbyte)"/>
        public sbyte GetSByte(string key, sbyte defaultValue) => GetValue(key, sbyte.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="byte"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="byte"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="byte"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="byte"/>.
        /// </exception>
        /// <seealso cref="GetByte(string)"/>
        /// <seealso cref="TryGetValue(string,out byte)"/>
        public byte GetByte(string key, byte defaultValue) => GetValue(key, byte.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="short"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="short"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="short"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="short"/>.
        /// </exception>
        /// <seealso cref="GetShort(string)"/>
        /// <seealso cref="TryGetValue(string,out short)"/>
        public short GetShort(string key, short defaultValue) => GetValue(key, short.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="ushort"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="ushort"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="ushort"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="ushort"/>.
        /// </exception>
        /// <seealso cref="GetUShort(string)"/>
        /// <seealso cref="TryGetValue(string,out ushort)"/>
        public ushort GetUShort(string key, ushort defaultValue) => GetValue(key, ushort.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="int"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="int"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="int"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="int"/>.
        /// </exception>
        /// <seealso cref="GetInt(string)"/>
        /// <seealso cref="TryGetValue(string,out int)"/>
        public int GetInt(string key, int defaultValue) => GetValue(key, int.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="uint"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="uint"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="uint"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="uint"/>.
        /// </exception>
        /// <seealso cref="GetUInt(string)"/>
        /// <seealso cref="TryGetValue(string,out uint)"/>
        public uint GetUInt(string key, uint defaultValue) => GetValue(key, uint.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="long"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="long"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="long"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="long"/>.
        /// </exception>
        /// <seealso cref="GetLong(string)"/>
        /// <seealso cref="TryGetValue(string,out long)"/>
        public long GetLong(string key, long defaultValue) => GetValue(key, long.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="ulong"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="ulong"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="ulong"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="ulong"/>.
        /// </exception>
        /// <seealso cref="GetULong(string)"/>
        /// <seealso cref="TryGetValue(string,out ulong)"/>
        public ulong GetULong(string key, ulong defaultValue) => GetValue(key, ulong.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="char"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="char"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="char"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="char"/>.
        /// </exception>
        /// <seealso cref="GetChar(string)"/>
        /// <seealso cref="TryGetValue(string,out char)"/>
        public char GetChar(string key, char defaultValue) => GetValue(key, char.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="float"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="float"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="float"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="float"/>.
        /// </exception>
        /// <seealso cref="GetSingle(string)"/>
        /// <seealso cref="TryGetValue(string,out float)"/>
        public float GetSingle(string key, float defaultValue) => GetValue(key, float.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="double"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="double"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="double"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="double"/>.
        /// </exception>
        /// <seealso cref="GetDouble(string)"/>
        /// <seealso cref="TryGetValue(string,out double)"/>
        public double GetDouble(string key, double defaultValue) => GetValue(key, double.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="bool"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="bool"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="bool"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="bool"/>.
        /// </exception>
        /// <seealso cref="GetBool(string)"/>
        /// <seealso cref="TryGetValue(string,out bool)"/>
        public bool GetBool(string key, bool defaultValue) => GetValue(key, bool.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="decimal"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <see cref="decimal"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/>, converted to <see cref="decimal"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <see cref="decimal"/>.
        /// </exception>
        /// <seealso cref="GetDecimal(string)"/>
        /// <seealso cref="TryGetValue(string,out decimal)"/>
        public decimal GetDecimal(string key, decimal defaultValue) => GetValue(key, decimal.TryParse, defaultValue);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="string"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <returns>The value associated with <paramref name="key"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="GetString(string)"/>
        /// <seealso cref="TryGetValue(string,out string)"/>
        public string GetString(string key, string defaultValue)
        {
            if (key is null) { throw new ArgumentNullException(nameof(key)); }
            return AsImmutableDictionary.TryGetValue(key, out var value)
                ? value
                : defaultValue;
        }

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <typeparamref name="T"/>. If the key is not found, then
        /// <paramref name="defaultValue"/> is returned. If the key exists, but the associated value cannot be converted
        /// to <typeparamref name="T"/> (via <paramref name="tryParse"/>, an exception is thrown.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="tryParse">The conversion function from <c>string</c> to <typeparamref name="T"/>.</param>
        /// <param name="defaultValue">The default value to return if <paramref name="key"/> is not found.</param>
        /// <typeparam name="T">The desired return type.</typeparam>
        /// <returns>The value associated with <paramref name="key"/>, converted to <typeparamref name="T"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="BadLookupException">
        ///     If <paramref name="key"/> exists but the associated value cannot not be converted to <typeparamref name="T"/>.
        /// </exception>
        /// <seealso cref="GetValue{T}(string,TryParseFunc{T})"/>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetSByte(string,sbyte)"/>
        /// <seealso cref="GetByte(string,byte)"/>
        /// <seealso cref="GetShort(string,short)"/>
        /// <seealso cref="GetUShort(string,ushort)"/>
        /// <seealso cref="GetInt(string,int)"/>
        /// <seealso cref="GetUInt(string,uint)"/>
        /// <seealso cref="GetLong(string,long)"/>
        /// <seealso cref="GetULong(string,ulong)"/>
        /// <seealso cref="GetChar(string,char)"/>
        /// <seealso cref="GetSingle(string,float)"/>
        /// <seealso cref="GetDouble(string,double)"/>
        /// <seealso cref="GetBool(string,bool)"/>
        public T GetValue<T>(string key, TryParseFunc<T> tryParse, T defaultValue)
        {
            if (key is null) { throw new ArgumentNullException(nameof(key)); }
            Debug.Assert(tryParse != null);
            var lookupResult = TryGetValue(key, out var value, tryParse);
            switch (lookupResult.Type)
            {
                case LookupResultType.Success:
                    return value;
                case LookupResultType.MissingKey:
                    return defaultValue;
                case LookupResultType.ParsingError:
                    throw new BadLookupException(
                        LookupResult.ParsingError,
                        $"Value for key, \"{key}\", could not be parsed as {typeof(T).FullName}: {AsImmutableDictionary[key]}");
            }
            return value;
        }

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="sbyte"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="sbyte"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="sbyte"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="sbyte"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetSByte(string)"/>
        public LookupResult TryGetValue(string key, out sbyte value) =>
            TryGetValue(key, out value, sbyte.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="byte"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="byte"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="byte"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="byte"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetByte(string)"/>
        public LookupResult TryGetValue(string key, out byte value) =>
            TryGetValue(key, out value, byte.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="short"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="short"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="short"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="short"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetShort(string)"/>
        public LookupResult TryGetValue(string key, out short value) =>
            TryGetValue(key, out value, short.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="ushort"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="ushort"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="ushort"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="ushort"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetUShort(string)"/>
        public LookupResult TryGetValue(string key, out ushort value) =>
            TryGetValue(key, out value, ushort.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="int"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="int"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="int"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="int"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetInt(string)"/>
        public LookupResult TryGetValue(string key, out int value) =>
            TryGetValue(key, out value, int.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="uint"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="uint"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="uint"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="uint"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetUInt(string)"/>
        public LookupResult TryGetValue(string key, out uint value) =>
            TryGetValue(key, out value, uint.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="long"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="long"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="long"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="long"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetLong(string)"/>
        public LookupResult TryGetValue(string key, out long value) =>
            TryGetValue(key, out value, long.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="ulong"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="ulong"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="ulong"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="ulong"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetULong(string)"/>
        public LookupResult TryGetValue(string key, out ulong value) =>
            TryGetValue(key, out value, ulong.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="char"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="char"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="char"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="char"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetChar(string)"/>
        public LookupResult TryGetValue(string key, out char value) =>
            TryGetValue(key, out value, char.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="float"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="float"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="float"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="float"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetSingle(string)"/>
        public LookupResult TryGetValue(string key, out float value) =>
            TryGetValue(key, out value, float.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="double"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="double"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="double"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="double"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetDouble(string)"/>
        public LookupResult TryGetValue(string key, out double value) =>
            TryGetValue(key, out value, double.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="bool"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="bool"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="bool"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="bool"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetBool(string)"/>
        public LookupResult TryGetValue(string key, out bool value) =>
            TryGetValue(key, out value, bool.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="decimal"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <see cref="decimal"/>, <paramref name="value"/> is set to
        /// <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <see cref="decimal"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <see cref="decimal"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetDecimal(string)"/>
        public LookupResult TryGetValue(string key, out decimal value) =>
            TryGetValue(key, out value, decimal.TryParse);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <see cref="string"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The value associated with <paramref name="key"/></param>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="TryGetValue{T}(string,out T,TryParseFunc{T})"/>
        /// <seealso cref="GetString(string)"/>
        public LookupResult TryGetValue(string key, out string value)
        {
            if (key is null) { throw new ArgumentNullException(nameof(key)); }
            return AsImmutableDictionary.TryGetValue(key, out value)
                ? LookupResult.Success
                : LookupResult.MissingKey;
        }

        /// <summary>
        /// A function that models parsers like <see cref="int.TryParse(ReadOnlySpan{char},out int)"/> for any
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <param name="value">The parsed value.</param>
        /// <typeparam name="T">The parsed value type.</typeparam>
        public delegate bool TryParseFunc<T>(string str, out T value);

        /// <summary>
        /// Retrieves the value at <paramref name="key"/> as <typeparamref name="T"/>.
        /// If the key is not found, then <paramref name="value"/> is set to <c>default</c> and
        /// <see cref="LookupResult.MissingKey"/> is returned.
        /// If the key is found, but cannot be converted to <typeparamref name="T"/> (via <paramref name="tryParse"/>),
        /// <paramref name="value"/> is set to <c>default</c> and <see cref="LookupResult.ParsingError"/> is returned.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The parsed value associated with <paramref name="key"/></param>
        /// <param name="tryParse">The conversion function from <c>string</c> to <typeparamref name="T"/>.</param>
        /// <typeparam name="T">The desired value type.</typeparam>
        /// <returns>
        ///     <see cref="LookupResult.Success"/> if the key exists, and can be converted to <typeparamref name="T"/>.
        ///     <see cref="LookupResult.ParsingError"/> if the key exists, but cannot be converted to <typeparamref name="T"/>.
        ///     <see cref="LookupResult.MissingKey"/> if the key does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        /// <seealso cref="GetValue{T}(string,TryParseFunc{T})"/>
        public LookupResult TryGetValue<T>(string key, out T value, TryParseFunc<T> tryParse)
        {
            if (key is null) { throw new ArgumentNullException(nameof(key)); }
            Debug.Assert(tryParse != null);
            if (AsImmutableDictionary.TryGetValue(key, out var stringValue))
            {
                return tryParse(stringValue, out value)
                    ? LookupResult.Success
                    : LookupResult.ParsingError;
            }
            value = default;
            return LookupResult.MissingKey;
        }

        /// <summary>
        /// The result type of a lookup operation on <see cref="TxArguments"/>.
        /// </summary>
        /// <see cref="LookupResult"/>
        public enum LookupResultType
        {
            /// <summary>
            /// Represents a successful lookup operation.
            /// </summary>
            Success = 0,
            /// <summary>
            /// Represents a lookup operation where the provided key was not found.
            /// </summary>
            MissingKey,
            /// <summary>
            /// Represents a lookup operation where the provided key was found, but could not be converted to the
            /// requested type.
            /// </summary>
            ParsingError
        }
        /// <summary>
        /// The result of a lookup operation on <see cref="TxArguments"/>. This type is implicitly convertible
        /// to <c>bool</c>.
        /// </summary>
        public readonly struct LookupResult : IEquatable<LookupResult>
        {
            /// <summary>
            /// The lookup result type.
            /// </summary>
            public LookupResultType Type { get; }

            /// <summary>
            /// See <see cref="LookupResultType.Success"/>.
            /// </summary>
            public static LookupResult Success => new LookupResult(LookupResultType.Success);
            /// <summary>
            /// See <see cref="LookupResultType.MissingKey"/>.
            /// </summary>
            public static LookupResult MissingKey => new LookupResult(LookupResultType.MissingKey);
            /// <summary>
            /// See <see cref="LookupResultType.ParsingError"/>.
            /// </summary>
            public static LookupResult ParsingError => new LookupResult(LookupResultType.ParsingError);

            /// <summary>
            /// Determines whether or not <see cref="Type"/> is <see cref="LookupResultType.Success"/>.
            /// </summary>
            /// <param name="result">The instance to convert to <c>bool</c></param>
            /// <returns>
            ///     <c>true</c> if <see cref="Type"/> is <see cref="LookupResultType.Success"/>,
            ///     <c>false</c> otherwise.
            /// </returns>
            public static bool operator true(LookupResult result) => result.Type == LookupResultType.Success;
            /// <summary>
            /// Determines if <see cref="Type"/> is NOT <see cref="LookupResultType.Success"/>.
            /// </summary>
            /// <param name="result">The instance to convert to <c>bool</c></param>
            /// <returns>
            ///     <c>false</c> if <see cref="Type"/> is <see cref="LookupResultType.Success"/>,
            ///     <c>true</c> otherwise.
            /// </returns>
            public static bool operator false(LookupResult result) => result.Type != LookupResultType.Success;

            /// <summary>
            /// Constructs a <see cref="LookupResult"/> with a backing <see cref="LookupResultType"/>.
            /// </summary>
            /// <param name="type">The backing result type.</param>
            private LookupResult(LookupResultType type)
            {
                Type = type;
            }

            /// <inheritdoc/>
            public bool Equals(LookupResult other) => Type == other.Type;

            /// <inheritdoc/>
            public override bool Equals(object obj) => obj is LookupResult other && Equals(other);

            /// <inheritdoc/>
            public override int GetHashCode() => (int) Type;

            /// <inheritdoc/>
            public override string ToString() => $"LookupResult{{{Type.ToString()}}}";

            /// <summary>
            /// Compares two <see cref="LookupResult"/>s for equality.
            /// </summary>
            /// <param name="left">The left-hand side of the equality.</param>
            /// <param name="right">The right-hand side of the equality.</param>
            /// <returns>
            ///     <c>true</c> if <paramref name="left"/> is equal to <paramref name="right"/>,
            ///     <c>false</c> otherwise.
            /// </returns>
            public static bool operator ==(LookupResult left, LookupResult right) => left.Equals(right);

            /// <summary>
            /// Compares two <see cref="LookupResult"/>s for inequality.
            /// </summary>
            /// <param name="left">The left-hand side of the inequality.</param>
            /// <param name="right">The right-hand side of the inequality.</param>
            /// <returns>
            ///     <c>false</c> if <paramref name="left"/> is equal to <paramref name="right"/>,
            ///     <c>true</c> otherwise.
            /// </returns>
            public static bool operator !=(LookupResult left, LookupResult right) => !(left == right);
        }

        /// <summary>
        /// The backing arguments dictionary.
        /// </summary>
        public ImmutableDictionary<string, string> AsImmutableDictionary { get; }

        /// <summary>
        /// Creates a <see cref="TxArguments"/> with the provided arguments dictionary.
        /// </summary>
        /// <param name="arguments">The underlying arguments dictionary.</param>
        public TxArguments(ImmutableDictionary<string, string> arguments)
        {
            AsImmutableDictionary = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        /// <summary>
        /// Creates a <see cref="TxArguments"/> with the provided arguments dictionary.
        /// </summary>
        /// <param name="arguments">The underlying arguments dictionary.</param>
        public TxArguments(Dictionary<string, string> arguments)
        {
            AsImmutableDictionary = arguments?.ToImmutableDictionary() ?? throw new ArgumentNullException(nameof(arguments));
        }

        /// <summary>
        /// An empty <see cref="TxArguments"/> instance without any arguments.
        /// </summary>
        /// <seealso cref="ImmutableDictionary{TKey,TValue}.Empty"/>
        public static readonly TxArguments Empty = new TxArguments(
            ImmutableDictionary<string, string>.Empty);

        /// <inheritdoc/>
        public bool Equals(TxArguments other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other) || DictEquals(AsImmutableDictionary, other.AsImmutableDictionary);

            static bool DictEquals<TK, TV>(ImmutableDictionary<TK, TV> lhs, ImmutableDictionary<TK, TV> rhs)
            {
                if (lhs.Count != rhs.Count)
                {
                    return false;
                }
                var lhsValues = lhs.OrderBy(kv => kv.Key);
                var rhsValues = rhs.OrderBy(kv => kv.Key);
                return lhsValues.SequenceEqual(rhsValues);
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is TxArguments other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var orderedKeyValues = AsImmutableDictionary.OrderBy(kv => kv.Key);
            var hashCode = new HashCode();
            foreach (var keyValue in orderedKeyValues)
            {
                hashCode.Add(keyValue);
            }
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares two <see cref="TxArguments"/>s for equality. Two <see cref="TxArguments"/>s are
        /// only considered equal iff <see cref="AsImmutableDictionary"/> contains the same key-value pairs.
        /// </summary>
        /// <param name="left">The left-hand side of the equality.</param>
        /// <param name="right">The right-hand side of the equality.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left"/> is equal to <paramref name="right"/>,
        ///     <c>false</c> otherwise.
        /// </returns>
        public static bool operator ==(TxArguments left, TxArguments right) => Equals(left, right);

        /// <summary>
        /// Compares two <see cref="TxArguments"/>s for inequality. Two <see cref="TxArguments"/>s are
        /// only considered unequal iff <see cref="AsImmutableDictionary"/> contains different key-value pairs.
        /// </summary>
        /// <param name="left">The left-hand side of the inequality.</param>
        /// <param name="right">The right-hand side of the inequality.</param>
        /// <returns>
        ///     <c>false</c> if <paramref name="left"/> is equal to <paramref name="right"/>,
        ///     <c>true</c> otherwise.
        /// </returns>
        public static bool operator !=(TxArguments left, TxArguments right) => !(left == right);

        /// <summary>
        /// An exception that is thrown when a lookup within <see cref="TxArguments"/> fails. For example,
        /// when <see cref="TxArguments.GetValue{T}(string,TryParseFunc{T})"/> is called with a non-existent
        /// key, or parsing the associated value fails.
        /// </summary>
        public sealed class BadLookupException : System.Exception
        {
            /// <summary>
            /// The lookup that caused this exception.
            /// </summary>
            public LookupResult LookupResult { get; }

            /// <summary>
            /// Creates a <see cref="BadLookupException"/> from a lookup result and a message.
            /// </summary>
            /// <param name="lookupResult">See <see cref="LookupResult"/>.</param>
            /// <param name="message">See <see cref="Exception.Message"/>.</param>
            public BadLookupException(LookupResult lookupResult, string message) : base(message)
            {
                Debug.Assert(lookupResult != LookupResult.Success);
                LookupResult = lookupResult;
            }

            /// <summary>
            /// Creates a <see cref="BadLookupException"/> from a lookup result, a message, and an inner exception.
            /// </summary>
            /// <param name="lookupResult">See <see cref="LookupResult"/>.</param>
            /// <param name="message">See <see cref="Exception.Message"/>.</param>
            /// <param name="innerException">See <see cref="Exception.InnerException"/>.</param>
            public BadLookupException(LookupResult lookupResult, string message, Exception innerException)
                : base(message, innerException)
            {
                Debug.Assert(lookupResult != LookupResult.Success);
                LookupResult = lookupResult;
            }
        }
    }
}