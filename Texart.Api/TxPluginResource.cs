using System;
using System.Diagnostics;

namespace Texart.Api
{
    /// <summary>
    /// Factory functions for creating <see cref="TxPluginResource{T}"/> instances.
    /// </summary>
    public static class TxPluginResource
    {
        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="TxPluginResource{T}.ActiveMemberKind"/> set to
        /// <see cref="MemberKind.Factory"/> and <see cref="TxPluginResource{T}.Factory"/> set to
        /// <paramref name="factory"/>.
        ///
        /// The factory function accepts a <see cref="TxArguments"/> which can be used configure the created instance.
        /// The format of the arguments is implementation-defined.
        ///
        /// </summary>
        /// <param name="factory">The active factory instance.</param>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <returns>A <see cref="TxPluginResource{T}"/> with <paramref name="factory"/> as the active member.</returns>
        /// <seealso cref="OfGeneratorFactory{T}"/>
        /// <seealso cref="OfRendererFactory{T}"/>
        /// <seealso cref="OfLocator{T}(TxPluginResourceLocator)"/>
        /// <seealso cref="OfLocator{T}(TxPluginResourceLocator.RelativeLocator)"/>
        public static TxPluginResource<T> OfFactory<T>(TxFactory<T, TxArguments> factory) =>
            new TxPluginResource<T>(factory);

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="TxPluginResource{T}.ActiveMemberKind"/> set to
        /// <see cref="MemberKind.Factory"/> and <see cref="TxPluginResource{T}.Factory"/> set to
        /// <paramref name="factory"/>.
        /// </summary>
        /// <param name="factory">The active factory instance.</param>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <returns>A <see cref="TxPluginResource{T}"/> with <paramref name="factory"/> as the active member.</returns>
        /// <seealso cref="OfFactory{T}"/>
        /// <seealso cref="OfRendererFactory{T}"/>
        public static TxPluginResource<ITxTextBitmapGenerator> OfGeneratorFactory<T>(TxFactory<T, TxArguments> factory)
            where T : class, ITxTextBitmapGenerator => OfFactory<ITxTextBitmapGenerator>(factory);

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="TxPluginResource{T}.ActiveMemberKind"/> set to
        /// <see cref="MemberKind.Factory"/> and <see cref="TxPluginResource{T}.Factory"/> set to
        /// <paramref name="factory"/>.
        /// </summary>
        /// <param name="factory">The active factory instance.</param>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <returns>A <see cref="TxPluginResource{T}"/> with <paramref name="factory"/> as the active member.</returns>
        /// <seealso cref="OfFactory{T}"/>
        /// <seealso cref="OfGeneratorFactory{T}"/>
        public static TxPluginResource<ITxTextBitmapRenderer> OfRendererFactory<T>(TxFactory<T, TxArguments> factory)
            where T : class, ITxTextBitmapRenderer => OfFactory<ITxTextBitmapRenderer>(factory);

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="TxPluginResource{T}.ActiveMemberKind"/> set to
        /// <see cref="MemberKind.Locator"/> and <see cref="TxPluginResource{T}.Locator"/> set to
        /// <paramref name="locator"/>.
        ///
        /// The locator is an absolute URI identifying the location where a plugin believes the demanded resource can be
        /// found. This includes a <see cref="TxReferenceScheme"/>, <see cref="TxPluginResourceLocator.AssemblyPath"/>,
        /// and <see cref="TxPluginResourceLocator.ResourcePath"/>.
        ///
        /// </summary>
        /// <param name="locator">The active locator.</param>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <returns>A <see cref="TxPluginResource{T}"/> with <paramref name="locator"/> as the active member.</returns>
        /// <seealso cref="OfFactory{T}"/>
        /// <seealso cref="OfLocator{T}(TxPluginResourceLocator.RelativeLocator)"/>
        public static TxPluginResource<T> OfLocator<T>(TxPluginResourceLocator locator) =>
            new TxPluginResource<T>(locator);

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="TxPluginResource{T}.ActiveMemberKind"/> set to
        /// <see cref="MemberKind.RelativeLocator"/> and <see cref="TxPluginResource{T}.RelativeLocator"/> set to
        /// <paramref name="relativeLocator"/>.
        ///
        /// The locator is a relative URI identifying the location where a plugin believes the demanded resource can be
        /// found. The new lookup will be performed on the same <see cref="ITxPlugin"/> instance that returns this.
        ///
        /// </summary>
        /// <param name="relativeLocator">The active relative locator.</param>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <returns>
        ///     A <see cref="TxPluginResource{T}"/> with <paramref name="relativeLocator"/> as the active member.
        /// </returns>
        /// <seealso cref="OfFactory{T}"/>
        /// <seealso cref="OfLocator{T}(TxPluginResourceLocator)"/>
        public static TxPluginResource<T> OfLocator<T>(TxPluginResourceLocator.RelativeLocator relativeLocator) =>
            new TxPluginResource<T>(relativeLocator);

        /// <summary>
        /// Widens the type <typeparamref name="TU"/> to <typeparamref name="T"/>. <see cref="TxPluginResource{T}"/> can
        /// actually be contravariant. However, C# (as of 8.0) does not support contravariant generic type parameters
        /// for classes. The returned value and <paramref name="resource"/> share the same
        /// <see cref="TxPluginResource{T}.ActiveMember"/>.
        /// </summary>
        /// <param name="resource">The resource </param>
        /// <typeparam name="T">The less-derived type to convert to.</typeparam>
        /// <typeparam name="TU">The more-derived type to convert from.</typeparam>
        /// <returns></returns>
        public static TxPluginResource<T> Widen<T, TU>(this TxPluginResource<TU> resource)
            where T : class
            where TU : class, T
        {
            switch (resource.ActiveMemberKind)
            {
                case MemberKind.Factory: return new TxPluginResource<T>(resource._factory);
                case MemberKind.Locator: return new TxPluginResource<T>(resource._locator);
                case MemberKind.RelativeLocator: return new TxPluginResource<T>(resource._relativeLocator);
            }
            Debug.Fail("Unreachable code!");
            throw new InvalidOperationException();
        }

        /// <summary>
        /// A tag used to determine the active member of the union.
        /// </summary>
        /// <seealso cref="TxPluginResource{T}.ActiveMemberKind"/>
        public enum MemberKind
        {
            /// <summary>
            /// Kind indicating that <see cref="TxPluginResource{T}.Factory"/> is the <see cref="TxPluginResource{T}.ActiveMember"/>.
            /// </summary>
            Factory,
            /// <summary>
            /// Kind indicating that <see cref="TxPluginResource{T}.Locator"/> is the <see cref="TxPluginResource{T}.ActiveMember"/>.
            /// </summary>
            Locator,
            /// <summary>
            /// Kind indicating that <see cref="TxPluginResource{T}.RelativeLocator"/> is the <see cref="TxPluginResource{T}.ActiveMember"/>.
            /// </summary>
            RelativeLocator
        }

        /// <summary>
        /// An exception that is thrown if an attempt is made to access an inactive member of the union.
        /// </summary>
        /// <seealso cref="TxPluginResource{T}.ActiveMemberKind"/>
        public sealed class InactiveUnionMemberAccessException : Exception
        {
            /// <summary>
            /// The attempted access kind.
            /// </summary>
            private readonly MemberKind _attemptedKind;
            /// <summary>
            /// The actual active kind.
            /// </summary>
            private readonly MemberKind _activeKind;

            /// <inheritdoc cref="Exception.Message"/>
            public override string Message =>
                $"Attempted to access {_attemptedKind} but the active member is {_activeKind}";

            /// <summary>
            /// Creates an <inheritdoc cref="InactiveUnionMemberAccessException"/> with the mismatched (attempted versus
            /// actual) kinds.
            /// </summary>
            /// <param name="attemptedKind">The attempted access kind.</param>
            /// <param name="activeKind">The actual active kind.</param>
            internal InactiveUnionMemberAccessException(MemberKind attemptedKind, MemberKind activeKind)
            {
                Debug.Assert(attemptedKind != activeKind);
                _attemptedKind = attemptedKind;
                _activeKind = activeKind;
            }
        }
    }

    /// <summary>
    /// A union type which is used as the return type of <see cref="ITxPlugin"/> lookup methods. This is required
    /// because C# (as of 8.0) does not support union types. This type allows the return value to be flexible. See
    /// <see cref="TxPluginResource"/> for more details on the semantics of each kind of member.
    /// <see cref="TxPluginResource{T}"/> is immutable.
    /// </summary>
    /// <typeparam name="T">The type of resource that this creates.</typeparam>
    public sealed class TxPluginResource<T> : IEquatable<TxPluginResource<T>>
    {
        /// <summary>
        /// The <see cref="TxPluginResource.MemberKind"/> tag representing the <see cref="TxPluginResource{T}.ActiveMember"/>
        /// in <c>this</c>. There can only be one active member in the union.
        /// </summary>
        public TxPluginResource.MemberKind ActiveMemberKind { get; }

        /// <summary>
        /// The factory member of the union if the <see cref="ActiveMemberKind"/> is
        /// <see cref="TxPluginResource.MemberKind.Factory"/>. Throws an exception otherwise.
        /// </summary>
        /// <exception cref="TxPluginResource.InactiveUnionMemberAccessException">
        ///     If the <see cref="ActiveMemberKind"/> is not <see cref="TxPluginResource.MemberKind.Factory"/>.
        /// </exception>
        public TxFactory<T, TxArguments> Factory {
            get
            {
                if (ActiveMemberKind != TxPluginResource.MemberKind.Factory)
                {
                    throw new TxPluginResource.InactiveUnionMemberAccessException(
                        TxPluginResource.MemberKind.Factory, ActiveMemberKind);
                }
                return _factory;
            }
        }

        /// <summary>
        /// The locator member of the union if the <see cref="ActiveMemberKind"/> is
        /// <see cref="TxPluginResource.MemberKind.Locator"/>. Throws an exception otherwise.
        /// </summary>
        /// <exception cref="TxPluginResource.InactiveUnionMemberAccessException">
        ///     If the <see cref="ActiveMemberKind"/> is not <see cref="TxPluginResource.MemberKind.Locator"/>.
        /// </exception>
        public TxPluginResourceLocator Locator {
            get
            {
                if (ActiveMemberKind != TxPluginResource.MemberKind.Locator)
                {
                    throw new TxPluginResource.InactiveUnionMemberAccessException(
                        TxPluginResource.MemberKind.Locator, ActiveMemberKind);
                }
                return _locator;
            }
        }

        /// <summary>
        /// The relative locator member of the union if the <see cref="ActiveMemberKind"/> is
        /// <see cref="TxPluginResource.MemberKind.RelativeLocator"/>. Throws an exception otherwise.
        /// </summary>
        /// <exception cref="TxPluginResource.InactiveUnionMemberAccessException">
        ///     If the <see cref="ActiveMemberKind"/> is not <see cref="TxPluginResource.MemberKind.RelativeLocator"/>.
        /// </exception>
        public TxPluginResourceLocator.RelativeLocator RelativeLocator {
            get
            {
                if (ActiveMemberKind != TxPluginResource.MemberKind.RelativeLocator)
                {
                    throw new TxPluginResource.InactiveUnionMemberAccessException(
                        TxPluginResource.MemberKind.RelativeLocator, ActiveMemberKind);
                }
                return _relativeLocator;
            }
        }

        /// <summary>
        /// The currently active member of the union. This will be one of <see cref="Factory"/>, <see cref="Locator"/>,
        /// or <see cref="RelativeLocator"/> - depending on <see cref="ActiveMemberKind"/>.
        /// </summary>
        public object ActiveMember
        {
            get
            {
                switch (ActiveMemberKind)
                {
                    case TxPluginResource.MemberKind.Factory: return _factory;
                    case TxPluginResource.MemberKind.Locator: return _locator;
                    case TxPluginResource.MemberKind.RelativeLocator: return _relativeLocator;
                }
                Debug.Fail("Unreachable code!");
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="ActiveMemberKind"/> of <see cref="TxPluginResource.MemberKind.Factory"/>.
        /// </summary>
        /// <param name="factory">The active factory instance.</param>
        /// <seealso cref="TxPluginResource.OfFactory{T}"/>
        internal TxPluginResource(TxFactory<T, TxArguments> factory)
        {
            _factory = factory;
            ActiveMemberKind = TxPluginResource.MemberKind.Factory;
        }

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="ActiveMemberKind"/> of <see cref="TxPluginResource.MemberKind.Locator"/>.
        /// </summary>
        /// <param name="locator">The active locator instance.</param>
        /// <seealso cref="TxPluginResource.OfLocator{T}(TxPluginResourceLocator)"/>
        internal TxPluginResource(TxPluginResourceLocator locator)
        {
            _locator = locator;
            ActiveMemberKind = TxPluginResource.MemberKind.Locator;
        }

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="ActiveMemberKind"/> of <see cref="TxPluginResource.MemberKind.RelativeLocator"/>.
        /// </summary>
        /// <param name="relativeResourceLocatorLocator">The active relative locator instance.</param>
        /// <seealso cref="TxPluginResource.OfLocator{T}(TxPluginResourceLocator.RelativeLocator)"/>
        internal TxPluginResource(TxPluginResourceLocator.RelativeLocator relativeResourceLocatorLocator)
        {
            _relativeLocator = relativeResourceLocatorLocator;
            ActiveMemberKind = TxPluginResource.MemberKind.RelativeLocator;
        }

        /// <summary>
        /// The factory union member, or <c>null</c> if <see cref="ActiveMemberKind"/> is not <see cref="TxPluginResource.MemberKind.Factory"/>.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        internal readonly TxFactory<T, TxArguments> _factory;
        /// <summary>
        /// The locator union member, or <c>null</c> if <see cref="ActiveMemberKind"/> is not <see cref="TxPluginResource.MemberKind.Locator"/>.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        internal readonly TxPluginResourceLocator _locator;
        /// <summary>
        /// The relative locator union member, or <c>null</c> if <see cref="ActiveMemberKind"/> is not <see cref="TxPluginResource.MemberKind.RelativeLocator"/>.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        internal readonly TxPluginResourceLocator.RelativeLocator _relativeLocator;

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"{typeof(TxPluginResource<T>).Name}({ActiveMemberKind}){{{ActiveMember}}}";

        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(TxPluginResource<T> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ActiveMemberKind != other.ActiveMemberKind) return false;
            switch (ActiveMemberKind)
            {
                case TxPluginResource.MemberKind.Factory: return Equals(_factory, other._factory);
                case TxPluginResource.MemberKind.Locator: return Equals(_locator, other._locator);
                case TxPluginResource.MemberKind.RelativeLocator: return Equals(_relativeLocator, other._relativeLocator);
            }
            Debug.Fail("Unreachable code!");
            return false;
        }

        /// <inheritdoc cref="object.Equals(object)"/>
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is TxPluginResource<T> other && Equals(other);

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(ActiveMemberKind);
            switch (ActiveMemberKind)
            {
                case TxPluginResource.MemberKind.Factory:
                    hashCode.Add(_factory);
                    break;
                case TxPluginResource.MemberKind.Locator:
                    hashCode.Add(_locator);
                    break;
                case TxPluginResource.MemberKind.RelativeLocator:
                    hashCode.Add(_relativeLocator);
                    break;
            }
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares two <see cref="TxPluginResource{T}"/> for equality. Two <see cref="TxPluginResource{T}"/> instances
        /// are equal iff they have the same <see cref="ActiveMemberKind"/> and their
        /// </summary>
        /// <param name="lhs">The left-hand side of the equality.</param>
        /// <param name="rhs">The right-hand side of the equality.</param>
        /// <returns>
        ///     <c>true</c> if the underlying union members of <paramref name="lhs"/> and <paramref name="rhs"/>
        ///     are equal, <c>false</c> otherwise.
        /// </returns>
        public static bool operator ==(TxPluginResource<T> lhs, TxPluginResource<T> rhs) => Equals(lhs, rhs);

        /// <summary>
        /// Compares two <see cref="TxPluginResource{T}"/> for equality. See <see cref="op_Equality"/> for details on
        /// what constitutes equality.
        /// </summary>
        /// <param name="lhs">The left-hand side of the inequality.</param>
        /// <param name="rhs">The right-hand side of the inequality.</param>
        /// <returns>
        ///     <c>true</c> if the underlying union members of <paramref name="lhs"/> and <paramref name="rhs"/>
        ///     are different, <c>false</c> otherwise.
        /// </returns>
        public static bool operator !=(TxPluginResource<T> lhs, TxPluginResource<T> rhs) => !(lhs == rhs);
    }
}