using Avalonia;
using Avalonia.Controls;

namespace OpenTabletDriver.UI.Controls.Input;

public partial class ToggleInput : UserControl
{
    private string? _label;
    private bool _state;

    public static readonly DirectProperty<ToggleInput, string?> LabelProperty = AvaloniaProperty.RegisterDirect<ToggleInput, string?>(
        nameof(Label),
        o => o.Label,
        (o, v) => o.Label = v
    );

    public static readonly DirectProperty<ToggleInput, bool> StateProperty = AvaloniaProperty.RegisterDirect<ToggleInput, bool>(
        nameof(State),
        o => o.State,
        (o, v) => o.State = v
    );

    public string? Label
    {
        set => SetAndRaise(LabelProperty, ref _label, value);
        get => _label;
    }

    public bool State
    {
        set => SetAndRaise(StateProperty, ref _state, value);
        get => _state;
    }

    public ToggleInput()
    {
        InitializeComponent();
    }
}
