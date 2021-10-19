using System;
using System.Reflection;
using OpenTabletDriver.Plugin.Settings;

#nullable enable

namespace OpenTabletDriver.Plugin.Attributes
{
    /// <summary>
    /// Applies the default value to a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultPluginDataAttribute : Attribute
    {
        public DefaultPluginDataAttribute(object? data)
        {
            this.Data = data;
        }

        public object? Data { get; }

        public void Set(object plugin, PropertyInfo targetProperty)
        {
            PluginSettings.ApplyDefaultValue(plugin, targetProperty, Data);
        }
    }
}