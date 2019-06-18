using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Texart.Api;

namespace Texart.Plugins
{
    /// <summary>
    /// Helpers for dealing with pre-built plugin assemblies.
    /// </summary>
    public static class PrebuiltPlugin
    {
        /// <summary>
        /// Searches <paramref name="pluginAssembly"/> for <see cref="ITxPlugin"/> type, with
        /// <see cref="TxPluginAttribute"/> applied to it. The found type is then instantiated by
        /// calling its no-arg constructor (with <see cref="Activator.CreateInstance(Type)"/>.
        /// Only <c>public</c> types are considered.
        /// </summary>
        /// <param name="pluginAssembly">The assembly whose <c>public</c> types to search.</param>
        /// <returns>The constructed <see cref="ITxPlugin"/> instance.</returns>
        /// <exception cref="BadPluginAssemblyException">
        ///     If a valid <see cref="ITxPlugin"/> type could not be found in <paramref name="pluginAssembly"/>,
        ///     or there was an error constructing the found type.
        /// </exception>
        /// <seealso cref="TxPluginAttribute"/>
        public static ITxPlugin GetPluginFromAssembly(Assembly pluginAssembly) =>
            GetPluginFromTypes(pluginAssembly.GetExportedTypes());

        /// <summary>
        /// Helper for <see cref="GetPluginFromAssembly"/> that makes it easier to test.
        /// </summary>
        /// <param name="assemblyTypes">See <see cref="GetPluginFromAssembly"/>.</param>
        /// <returns>See <see cref="GetPluginFromAssembly"/>.</returns>
        internal static ITxPlugin GetPluginFromTypes(IEnumerable<Type> assemblyTypes)
        {
            Debug.Assert(assemblyTypes != null);

            var pluginAttributedTypes = assemblyTypes
                .Where(t => t.GetCustomAttributes(typeof(TxPluginAttribute), false).Any())
                .ToImmutableArray();

            if (pluginAttributedTypes.IsEmpty)
            {
                throw new BadPluginAssemblyException($"No types found that were annotated with {typeof(TxPluginAttribute).Name}");
            }
            if (pluginAttributedTypes.Length > 1)
            {
                var attributedTypeNames = string.Join(", ", pluginAttributedTypes.Select(t => t.Name));
                throw new BadPluginAssemblyException(
                    $"Multiple types found that were annotated with {typeof(TxPluginAttribute).Name}: {attributedTypeNames}");
            }

            var pluginType = pluginAttributedTypes[0];
            if (!typeof(ITxPlugin).IsAssignableFrom(pluginType))
            {
                throw new BadPluginAssemblyException($"{pluginType.Name} does not implement {typeof(ITxPlugin).Name}");
            }

            try
            {
                var plugin = Activator.CreateInstance(pluginType);
                Debug.Assert(plugin is ITxPlugin);
                return (ITxPlugin)plugin;
            }
            catch (TargetInvocationException ex)
            {
                throw new BadPluginAssemblyException(
                    $"The plugin ({pluginType.Name}) constructor threw an exception",
                    ex);
            }
            catch (MissingMemberException ex)
            {
                throw new BadPluginAssemblyException(
                    $"The plugin ({pluginType.Name}) does not have a public, no-args constructor; or the plugin is abstract",
                    ex);
            }
            catch (MemberAccessException ex)
            {
                throw new BadPluginAssemblyException(
                    $"Calling the plugin ({pluginType.Name}) constructor is not permitted",
                    ex);
            }
            catch (Exception ex)
            {
                throw new BadPluginAssemblyException(
                    $"An unknown error happened trying to create a plugin ({pluginType.Name}) instance",
                    ex);
            }
        }

        /// <summary>
        /// An exception that is thrown when searching for <see cref="ITxPlugin"/> within an assembly fails.
        /// </summary>
        public sealed class BadPluginAssemblyException : System.Exception
        {
            /// <summary>
            /// Creates a <see cref="BadPluginAssemblyException"/> with a message.
            /// </summary>
            /// <param name="message">See <see cref="Exception.Message"/>.</param>
            public BadPluginAssemblyException(string message) : base(message)
            {
            }

            /// <summary>
            /// Creates a <see cref="BadPluginAssemblyException"/> with a message, and an inner exception.
            /// </summary>
            /// <param name="message">See <see cref="Exception.Message"/>.</param>
            /// <param name="innerException">See <see cref="Exception.InnerException"/>.</param>
            public BadPluginAssemblyException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }
    }
}