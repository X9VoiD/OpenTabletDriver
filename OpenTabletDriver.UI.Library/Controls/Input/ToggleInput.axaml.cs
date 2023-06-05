using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace OpenTabletDriver.UI.Controls.Input;

public partial class ToggleInput : UserControl
{
    private string? _label;
    private bool _state;

    public static readonly DirectProperty<ToggleInput, string?> LabelProperty = AvaloniaProperty.RegisterDirect<ToggleInput, string?>(
        nameof(Label),
        o => o.Label,
        (o, v) => o.Label = v,
        defaultBindingMode: BindingMode.TwoWay
    );

    public static readonly DirectProperty<ToggleInput, bool> ValueProperty = AvaloniaProperty.RegisterDirect<ToggleInput, bool>(
        nameof(Value),
        o => o.Value,
        (o, v) => o.Value = v,
        defaultBindingMode: BindingMode.TwoWay
    );

    public string? Label
    {
        set => SetAndRaise(LabelProperty, ref _label, value);
        get => _label;
    }

    public bool Value
    {
        set => SetAndRaise(ValueProperty, ref _state, value);
        get => _state;
    }

    public ToggleInput()
    {
        InitializeComponent();
    }
}
