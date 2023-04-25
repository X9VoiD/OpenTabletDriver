using System.Globalization;
using Avalonia;
using Avalonia.Controls;

namespace OpenTabletDriver.UI.Controls.Input;

public partial class FloatInput : UserControl
{
    private string? _label;
    private float _number;

    private bool _updatingText;
    private string _previousText = string.Empty;

    public static readonly DirectProperty<FloatInput, string?> LabelProperty = AvaloniaProperty.RegisterDirect<FloatInput, string?>(
        nameof(Label),
        o => o.Label,
        (o, v) => o.Label = v
    );

    public static readonly DirectProperty<FloatInput, float> NumberProperty = AvaloniaProperty.RegisterDirect<FloatInput, float>(
        nameof(Number),
        o => o.Number,
        (o, v) => o.Number = v
    );

    public string? Label
    {
        set => SetAndRaise(LabelProperty, ref _label, value);
        get => _label;
    }

    public float Number
    {
        set => SetAndRaise(NumberProperty, ref _number, value);
        get => _number;
    }

    static FloatInput()
    {
        NumberProperty.Changed.AddClassHandler<FloatInput>((o, e) => o.NumberChanged(e));
    }

    public FloatInput()
    {
        InitializeComponent();

        PART_Text.TextChanging += (sender, e) =>
        {
            e.Handled = true;
            if (_updatingText) return;

            if (string.IsNullOrEmpty(PART_Text.Text))
            {
                // special case for empty string
                // not doing this results to having 0 in textbox when input is cleared
                _updatingText = true;
                _previousText = string.Empty;
                Number = 0;
                _updatingText = false;
            }
            else if (StringUtility.TryParseFloat(PART_Text.Text, out float result))
            {
                _previousText = PART_Text.Text;
                Number = result;
            }
            else
            {
                _updatingText = true;
                PART_Text.Text = _previousText;
                _updatingText = false;
            }
        };
    }

    private void NumberChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_updatingText) return;

        _previousText = e.NewValue?.ToString() ?? string.Empty;

        _updatingText = true;
        PART_Text.Text = e.NewValue is float newValue
            ? newValue.ToString(CultureInfo.InvariantCulture)
            : string.Empty;
        _updatingText = false;
    }
}
