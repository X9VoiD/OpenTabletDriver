using System;
using System.Collections.Generic;
using System.Reflection;
using OpenTabletDriver.Plugin.Attributes;

#nullable enable

namespace OpenTabletDriver.Plugin.Settings
{
    public sealed class PluginSettings : Dictionary<string, object?>
    {
        public PluginSettings(string pluginTypeName)
        {
            PluginTypeName = pluginTypeName;
        }

        public string PluginTypeName { get; }

        public static PluginSettings FromType(Type pluginType)
        {
            var pluginName = pluginType.GetType().FullName;
            if (pluginName == null)
                throw new ArgumentException("Supplied plugin type does not have a valid type name.", nameof(pluginType));

            return new PluginSettings(pluginName);
        }

        public static PluginSettings FromPlugin(object plugin, Type? asPluginType = null)
        {
            string pluginName = asPluginType?.FullName ?? plugin.GetType().FullName!;
            var pluginSettings = new PluginSettings(pluginName);
            var pluginType = plugin.GetType();

            foreach (var propertyInfo in pluginType.GetProperties(BindingFlags.GetProperty | BindingFlags.SetProperty))
            {
                if (propertyInfo.GetCustomAttribute<PropertyAttribute>() != null)
                    pluginSettings.SaveProperty(propertyInfo.Name, propertyInfo.GetValue(plugin));
            }

            return pluginSettings;
        }

        public void SaveProperty(string propertyName, object? setting)
        {
            if (!ContainsKey(propertyName))
            {
                Add(propertyName, setting);
            }
            else
            {
                this[propertyName] = setting;
            }
        }

        public void ClearProperty(string propertyName)
        {
            Remove(propertyName);
        }

        public bool IsApplicableTo(object plugin)
        {
            return plugin.GetType().FullName == PluginTypeName;
        }

        public void ApplyTo(object plugin)
        {
            if (plugin == null)
                return;

            var pluginType = plugin.GetType();
            foreach (var propertyInfo in pluginType.GetProperties(BindingFlags.SetProperty))
            {
                // Check if settings for the property exists
                if (TryGetValue(propertyInfo.Name, out var value))
                {
                    propertyInfo.SetValue(plugin, value);
                }
                // Check if constant default settings for the property exists
                else if (propertyInfo.GetCustomAttribute<DefaultPluginDataAttribute>() is { } defaultValue)
                {
                    defaultValue.Set(plugin, propertyInfo);
                }
                // Check if member-sourced default settings for the property exists
                else if (propertyInfo.GetCustomAttribute<MemberPluginDataAttribute>() is { } memberSourcedValue)
                {
                    memberSourcedValue.Set(plugin, propertyInfo);
                }
            }

            var found = false;
            foreach (var method in pluginType.GetMethods())
            {
                if (method.GetCustomAttribute<OnPluginSettingsAppliedAttribute>() is not null)
                {
                    if (found)
                        throw new InvalidOperationException($"Multiple '{nameof(OnPluginSettingsAppliedAttribute)}' exists for plugin '{pluginType.FullName}'");

                    found = true;
                    method.Invoke(plugin, null);
                }
            }
        }

        internal static void ApplyDefaultValue(object plugin, PropertyInfo targetProperty, object? defaultValue)
        {
            if (defaultValue is not null && targetProperty.PropertyType != defaultValue.GetType())
            {
                var castedValue = Convert.ChangeType(defaultValue, targetProperty.PropertyType);
                targetProperty.SetValue(plugin, castedValue);
            }
            else
            {
                targetProperty.SetValue(plugin, defaultValue);
            }
        }
    }
}