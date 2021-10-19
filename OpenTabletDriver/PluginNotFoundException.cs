using System;

#nullable enable

namespace OpenTabletDriver
{
    public class PluginNotFoundException : Exception
    {
        public PluginNotFoundException()
        {
        }

        public PluginNotFoundException(string? message) : base(message)
        {
        }

        public PluginNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public static PluginNotFoundException Create(string path, Exception? innerException = null)
        {
            return new PluginNotFoundException($"Plugin '{path}' not found", innerException);
        }
    }
}