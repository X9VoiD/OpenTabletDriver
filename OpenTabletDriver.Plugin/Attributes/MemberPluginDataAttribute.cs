using System;
using System.Reflection;
using OpenTabletDriver.Plugin.Settings;

#nullable enable

namespace OpenTabletDriver.Plugin.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MemberPluginDataAttribute : Attribute
    {
        public MemberPluginDataAttribute(string memberName)
        {
            MemberName = memberName;
        }

        public string MemberName { get; }

        public void Set(object plugin, PropertyInfo targetProperty)
        {
            var pluginType = plugin.GetType();
            var dataSource = pluginType.GetProperty(MemberName);
            if (dataSource == null)
                throw new InvalidOperationException(
                    $"Plugin '{pluginType.FullName}' does not have a property called '{MemberName}' to apply to '{targetProperty.Name}'");

            var memberSourcedValue = dataSource.GetValue(plugin);
            PluginSettings.ApplyDefaultValue(plugin, targetProperty, memberSourcedValue);
        }
    }
}