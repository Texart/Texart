using System;
using System.Diagnostics;

#nullable enable

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
        /// </summary>
        ///
        /// <param name="factory">The active factory instance.</param>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <returns>A <see cref="TxPluginResource{T}"/> with <paramref name="factory"/> as the active member.</returns>
        /// <seealso cref="OfGeneratorFactory{T}"/>
        /// <seealso cref="OfRendererFactory{T}"/>
        /// <seealso cref="Redirect{T}"/>
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
        /// <see cref="MemberKind.Redirect"/> and <see cref="TxPluginResource{T}.Redirect"/> set to a <see cref="ResourceRedirect"/>
        /// with <paramref name="locator"/> and <paramref name="argumentsTransformer"/>.
        ///
        /// The <paramref name="locator"/> is an absolute URI identifying the location where a plugin believes the demanded resource can be
        /// found. This includes a <see cref="TxReferenceScheme"/>, <see cref="TxPluginResourceLocator.AssemblyPath"/>,
        /// and <see cref="TxPluginResourceLocator.ResourcePath"/>.
        /// </summary>
        ///
        /// <param name="locator">The redirect locator. See <see cref="ResourceRedirect.Locator"/>.</param>
        /// <param name="argumentsTransformer">
        ///     The argument transformation to pass to the locator. See <see cref="ResourceRedirect.ArgumentsTransformer"/>.
        /// </param>
        /// <returns>A <see cref="TxPluginResource{T}"/> with a <see cref="ResourceRedirect"/> as the active member.</returns>
        /// <seealso cref="OfFactory{T}"/>
        /// <seealso cref="RedirectGenerator"/>
        /// <seealso cref="RedirectRenderer"/>
        public static TxPluginResource<T> Redirect<T>(
            TxPluginResourceLocator locator,
            Func<TxArguments, TxArguments>? argumentsTransformer = null) =>
            new TxPluginResource<T>(new ResourceRedirect(locator, argumentsTransformer));

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="TxPluginResource{T}.ActiveMemberKind"/> set to
        /// <see cref="MemberKind.Redirect"/> and <see cref="TxPluginResource{T}.Redirect"/> set to a <see cref="ResourceRedirect"/>
        /// with <paramref name="locator"/> and <paramref name="argumentsTransformer"/>.
        ///
        /// The <paramref name="locator"/> is an absolute URI identifying the location where a plugin believes the demanded resource can be
        /// found. This includes a <see cref="TxReferenceScheme"/>, <see cref="TxPluginResourceLocator.AssemblyPath"/>,
        /// and <see cref="TxPluginResourceLocator.ResourcePath"/>.
        /// </summary>
        ///
        /// <param name="locator">The redirect locator. See <see cref="ResourceRedirect.Locator"/>.</param>
        /// <param name="argumentsTransformer">
        ///     The argument transformation to pass to the locator. See <see cref="ResourceRedirect.ArgumentsTransformer"/>.
        /// </param>
        /// <returns>A <see cref="TxPluginResource{T}"/> with a <see cref="ResourceRedirect"/> as the active member.</returns>
        /// <seealso cref="OfFactory{T}"/>
        /// <seealso cref="Redirect{T}"/>
        /// <seealso cref="RedirectRenderer"/>
        /// <see cref="ResourceRedirect"/>
        public static TxPluginResource<ITxTextBitmapGenerator> RedirectGenerator(
            TxPluginResourceLocator locator,
            Func<TxArguments, TxArguments>? argumentsTransformer = null) =>
            new TxPluginResource<ITxTextBitmapGenerator>(new ResourceRedirect(locator, argumentsTransformer));

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="TxPluginResource{T}.ActiveMemberKind"/> set to
        /// <see cref="MemberKind.Redirect"/> and <see cref="TxPluginResource{T}.Redirect"/> set to a <see cref="ResourceRedirect"/>
        /// with <paramref name="locator"/> and <paramref name="argumentsTransformer"/>.
        ///
        /// The <paramref name="locator"/> is an absolute URI identifying the location where a plugin believes the demanded resource can be
        /// found. This includes a <see cref="TxReferenceScheme"/>, <see cref="TxPluginResourceLocator.AssemblyPath"/>,
        /// and <see cref="TxPluginResourceLocator.ResourcePath"/>.
        /// </summary>
        ///
        /// <param name="locator">The redirect locator. See <see cref="ResourceRedirect.Locator"/>.</param>
        /// <param name="argumentsTransformer">
        ///     The argument transformation to pass to the locator. See <see cref="ResourceRedirect.ArgumentsTransformer"/>.
        /// </param>
        /// <returns>A <see cref="TxPluginResource{T}"/> with a <see cref="ResourceRedirect"/> as the active member.</returns>
        /// <seealso cref="OfFactory{T}"/>
        /// <seealso cref="Redirect{T}"/>
        /// <seealso cref="RedirectGenerator"/>
        /// <see cref="ResourceRedirect"/>
        public static TxPluginResource<ITxTextBitmapRenderer> RedirectRenderer(
            TxPluginResourceLocator locator,
            Func<TxArguments, TxArguments>? argumentsTransformer = null) =>
            new TxPluginResource<ITxTextBitmapRenderer>(new ResourceRedirect(locator, argumentsTransformer));

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
                case MemberKind.Factory: return new TxPluginResource<T>(resource._factory!);
                case MemberKind.Redirect: return new TxPluginResource<T>(resource._redirect!);
                default:
                    Debug.Fail("Unreachable code!");
                    throw new InvalidOperationException();
            }
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
            /// Kind indicating that <see cref="TxPluginResource{T}.Redirect"/> is the <see cref="TxPluginResource{T}.ActiveMember"/>.
            /// </summary>
            Redirect
        }

        /// <summary>
        /// A value that represents a plugin resource redirection and transformation that modifies the incoming
        /// <see cref="TxArguments"/> that the redirect destination will receive.
        /// </summary>
        public sealed class ResourceRedirect : IEquatable<ResourceRedirect>
        {
            /// <summary>
            /// The redirect destination.
            /// </summary>
            public TxPluginResourceLocator Locator { get; }
            /// <summary>
            /// The transformation applied to incoming arguments before being passed on to the redirect destination
            /// pointed to by <see cref="Locator"/>. If this is <c>null</c>, then the identity transformation
            /// should be applied.
            /// </summary>
            public Func<TxArguments, TxArguments>? ArgumentsTransformer { get; }

            /// <summary>
            /// Creates a redirect.
            /// </summary>
            /// <param name="locator"><see cref="Locator"/>.</param>
            /// <param name="argumentsTransformer"><see cref="ArgumentsTransformer"/>.</param>
            internal ResourceRedirect(TxPluginResourceLocator locator,
                Func<TxArguments, TxArguments>? argumentsTransformer)
            {
                Debug.Assert(locator != null);
                Locator = locator;
                ArgumentsTransformer = argumentsTransformer;
            }

            /// <inheritdoc/>
            public bool Equals(ResourceRedirect other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(Locator, other.Locator) && Equals(ArgumentsTransformer, other.ArgumentsTransformer);
            }

            /// <inheritdoc/>
            public override bool Equals(object obj)
            {
                return ReferenceEquals(this, obj) || obj is ResourceRedirect other && Equals(other);
            }

            /// <inheritdoc/>
            public override int GetHashCode() => HashCode.Combine(Locator, ArgumentsTransformer);

            /// <summary>
            /// Compares two <see cref="ResourceRedirect"/> for equality. Two <see cref="ResourceRedirect"/> instances
            /// are equal iff they have the same <see cref="Locator"/> and <see cref="ArgumentsTransformer"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the equality.</param>
            /// <param name="rhs">The right-hand side of the equality.</param>
            /// <returns>
            ///     <c>true</c> if the field members of <paramref name="lhs"/> and <paramref name="rhs"/>
            ///     are equal, <c>false</c> otherwise.
            /// </returns>
            public static bool operator ==(ResourceRedirect lhs, ResourceRedirect rhs) => Equals(lhs, rhs);

            /// <summary>
            /// Compares two <see cref="ResourceRedirect"/> for equality. See <see cref="op_Equality"/> for details on
            /// what constitutes equality.
            /// </summary>
            /// <param name="lhs">The left-hand side of the inequality.</param>
            /// <param name="rhs">The right-hand side of the inequality.</param>
            /// <returns>
            ///     <c>true</c> if the field members of <paramref name="lhs"/> and <paramref name="rhs"/>
            ///     are different, <c>false</c> otherwise.
            /// </returns>
            public static bool operator !=(ResourceRedirect lhs, ResourceRedirect rhs) => !(lhs == rhs);
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

            /// <inheritdoc/>
            public override string Message =>
                $"Attempted to access {_attemptedKind} but the active member is {_activeKind}";

            /// <summary>
            /// Creates an <see cref="InactiveUnionMemberAccessException"/> with the mismatched (attempted versus
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
    /// <see cref="TxPluginResource"/> members for more details on the semantics of each kind of member.
    /// <see cref="TxPluginResource{T}"/> is immutable.
    /// </summary>
    /// <typeparam name="T">The type of resource that this creates.</typeparam>
    /// <seealso cref="ITxPlugin.LookupGenerator"/>
    /// <seealso cref="ITxPlugin.LookupRenderer"/>
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
                return _factory!;
            }
        }

        /// <summary>
        /// The redirect member of the union if the <see cref="ActiveMemberKind"/> is
        /// <see cref="TxPluginResource.MemberKind.Redirect"/>. Throws an exception otherwise.
        /// </summary>
        /// <exception cref="TxPluginResource.InactiveUnionMemberAccessException">
        ///     If the <see cref="ActiveMemberKind"/> is not <see cref="TxPluginResource.MemberKind.Redirect"/>.
        /// </exception>
        public TxPluginResource.ResourceRedirect Redirect {
            get
            {
                if (ActiveMemberKind != TxPluginResource.MemberKind.Redirect)
                {
                    throw new TxPluginResource.InactiveUnionMemberAccessException(
                        TxPluginResource.MemberKind.Redirect, ActiveMemberKind);
                }
                return _redirect!;
            }
        }

        /// <summary>
        /// The currently active member of the union. This will be one of <see cref="Factory"/> or <see cref="Redirect"/>
        /// depending on <see cref="ActiveMemberKind"/>.
        /// </summary>
        public object ActiveMember
        {
            get
            {
                switch (ActiveMemberKind)
                {
                    case TxPluginResource.MemberKind.Factory: return _factory!;
                    case TxPluginResource.MemberKind.Redirect: return _redirect!;
                    default:
                        Debug.Fail("Unreachable code!");
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="ActiveMemberKind"/> of <see cref="TxPluginResource.MemberKind.Factory"/>.
        /// </summary>
        /// <param name="factory">The active factory instance.</param>
        /// <seealso cref="TxPluginResource.OfFactory{T}"/>
        internal TxPluginResource(TxFactory<T, TxArguments> factory)
        {
            Debug.Assert(factory != null);
            _factory = factory;
            ActiveMemberKind = TxPluginResource.MemberKind.Factory;
        }

        /// <summary>
        /// Creates a <see cref="TxPluginResource{T}"/> with <see cref="ActiveMemberKind"/> of <see cref="TxPluginResource.MemberKind.Redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirect destination.</param>
        /// <seealso cref="TxPluginResource.Redirect{T}"/>
        internal TxPluginResource(TxPluginResource.ResourceRedirect redirect)
        {
            Debug.Assert(redirect != null!);
            _redirect = redirect;
            ActiveMemberKind = TxPluginResource.MemberKind.Redirect;
        }

        /// <summary>
        /// The factory union member, or <c>null</c> if <see cref="ActiveMemberKind"/> is not <see cref="TxPluginResource.MemberKind.Factory"/>.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        internal readonly TxFactory<T, TxArguments>? _factory;
        /// <summary>
        /// The redirect union member, or <c>null</c> if <see cref="ActiveMemberKind"/> is not <see cref="TxPluginResource.MemberKind.Redirect"/>.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        internal readonly TxPluginResource.ResourceRedirect? _redirect;

        /// <inheritdoc/>
        public override string ToString() => $"{typeof(TxPluginResource<T>).Name}({ActiveMemberKind}){{{ActiveMember}}}";

        /// <inheritdoc/>
        public bool Equals(TxPluginResource<T> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ActiveMemberKind != other.ActiveMemberKind) return false;
            switch (ActiveMemberKind)
            {
                case TxPluginResource.MemberKind.Factory: return Equals(_factory, other._factory);
                case TxPluginResource.MemberKind.Redirect: return Equals(_redirect, other._redirect);
                default:
                    Debug.Fail("Unreachable code!");
                    return false;
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is TxPluginResource<T> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(ActiveMemberKind);
            switch (ActiveMemberKind)
            {
                case TxPluginResource.MemberKind.Factory:
                    hashCode.Add(_factory);
                    break;
                case TxPluginResource.MemberKind.Redirect:
                    hashCode.Add(_redirect);
                    break;
                default:
                    Debug.Fail("Unreachable code!");
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