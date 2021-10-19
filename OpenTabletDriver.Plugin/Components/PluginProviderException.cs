using System;

#nullable enable

namespace OpenTabletDriver.Plugin.Components
{
    public class PluginProviderException : Exception
    {
        public PluginProviderException() : this("Invalid types have been found in the plugin types provider.")
        {
        }

        public PluginProviderException(string message) : base(message)
        {
        }

        public PluginProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}