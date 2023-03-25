using CommunityToolkit.Mvvm.Messaging.Messages;
using OpenTabletDriver.UI.Services;

namespace OpenTabletDriver.UI.Messages;

/// <summary>
/// Sent when the daemon connection state changes.
/// </summary>
public sealed class DaemonConnectionChangedMessage : ValueChangedMessage<DaemonState>
{
    public DaemonConnectionChangedMessage(DaemonState value) : base(value)
    {
    }
}
