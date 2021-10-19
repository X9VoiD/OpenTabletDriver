using System;

#nullable enable

namespace OpenTabletDriver.Plugin.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OnPluginSettingsAppliedAttribute : Attribute
    {
    }
}