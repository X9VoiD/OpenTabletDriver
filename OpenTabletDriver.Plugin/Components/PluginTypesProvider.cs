using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using OpenTabletDriver.Plugin.Attributes;

#nullable enable

namespace OpenTabletDriver.Plugin.Components
{
    public abstract class PluginTypesProvider
    {
        public static readonly ImmutableHashSet<Type> ImplementablePluginTypes =
            Assembly.GetAssembly(typeof(PluginTypesProvider))!.GetExportedTypes()
                .Where(type => type.GetCustomAttribute<PluginTypeAttribute>() != null)
                .ToImmutableHashSet();

        // Disallow manual modification, all updates should be done by any of the atomic Batched* functions
        private ImmutableArray<Type> _types;

        public event EventHandler? TypesChanged;
        public ImmutableArray<Type> Types => _types;

        internal void AddTypes(IEnumerable<Type> types)
        {
            BatchedAdd(types);
        }

        protected void BatchedUpdate<T>(Func<ImmutableArray<Type>, ImmutableArray<Type>> transformer)
        {
            ImmutableInterlocked.Update(ref _types, transformer);
            VerifyTypes();
            TypesChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void BatchedUpdate<T>(Func<ImmutableArray<Type>, T, ImmutableArray<Type>> transformer, T transformerArgument)
        {
            ImmutableInterlocked.Update(ref _types, transformer, transformerArgument);
            VerifyTypes();
            TypesChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void BatchedAdd(IEnumerable<Type> typesToAdd)
        {
            BatchedUpdate(static (types, addedTypes) => types.AddRange(addedTypes), typesToAdd);
        }

        protected void BatchedAdd(params Type[] typesToAdd)
        {
            BatchedUpdate(static (types, addedTypes) => types.AddRange(addedTypes), typesToAdd);
        }

        protected void BatchedRemove(IEnumerable<Type> typesToRemove)
        {
            BatchedUpdate(static (types, removedTypes) => types.RemoveRange(removedTypes), typesToRemove);
        }

        protected void BatchedRemove(params Type[] typesToRemove)
        {
            BatchedUpdate(static (types, removedTypes) => types.RemoveRange(removedTypes), typesToRemove);
        }

        protected void PopulateFromAssembly(Assembly assembly)
        {
            var assemblyTypes = assembly.GetExportedTypes();
            var pluginImplementations = new List<Type>(assemblyTypes.Length / 2);

            // Do not use LINQ form to avoid excessive captured variable allocations
            foreach (var type in assemblyTypes)
            {
                if (IsPlatformSupported(type)
                    && IsPluginType(type)
                    && !IsPluginIgnored(type))
                {
                    pluginImplementations.Add(type);
                }
            }

            BatchedAdd(pluginImplementations);
        }

        private static bool IsPluginType(Type type)
        {
            if (!(type.IsAbstract || type.IsInterface))
            {
                // Do not use LINQ form to avoid excessive captured variable allocations
                foreach (var implementablePluginType in ImplementablePluginTypes)
                {
                    if (type.IsAssignableTo(implementablePluginType))
                    {
                        return true;
                    }
                    else
                    {
                        foreach (var typeInterface in type.GetInterfaces())
                        {
                            if (typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition() == implementablePluginType)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsPlatformSupported(Type type)
        {
            var attr = type.GetCustomAttribute(typeof(SupportedPlatformAttribute)) as SupportedPlatformAttribute;
            return attr?.IsCurrentPlatform ?? true;
        }

        private static bool IsPluginIgnored(Type type)
        {
            return type.GetCustomAttribute<PluginIgnoreAttribute>() is not null;
        }

        private void VerifyTypes()
        {
#if DEBUG
            foreach (var type in _types)
            {
                foreach (var implementablePluginType in ImplementablePluginTypes)
                {
                    if (!type.IsAssignableTo(implementablePluginType))
                    {
                        throw new PluginProviderException();
                    }
                }
            }
#endif
        }
    }
}