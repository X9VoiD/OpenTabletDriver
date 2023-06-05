using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace OpenTabletDriver.UI.Controls.Input;

public partial class DoubleInput : UserControl
{
    private string? _label;
    private double _number;
    private string? _unit;
    private int _precision = 2;

    private bool _updatingText;
    private string _previousText = string.Empty;

    public static readonly DirectProperty<DoubleInput, string?> LabelProperty = AvaloniaProperty.RegisterDirect<DoubleInput, string?>(
        nameof(Label),
        o => o.Label,
        (o, v) => o.Label = v,
        defaultBindingMode: BindingMode.TwoWay
    );

    public static readonly DirectProperty<DoubleInput, double> ValueProperty = AvaloniaProperty.RegisterDirect<DoubleInput, double>(
        nameof(Value),
        o => o.Value,
        (o, v) => o.Value = v,
        defaultBindingMode: BindingMode.TwoWay
    );

    public static readonly DirectProperty<DoubleInput, string?> UnitProperty = AvaloniaProperty.RegisterDirect<DoubleInput, string?>(
        nameof(Unit),
        o => o.Unit,
        (o, v) => o.Unit = v,
        defaultBindingMode: BindingMode.TwoWay
    );

    public static readonly DirectProperty<DoubleInput, int> PrecisionProperty = AvaloniaProperty.RegisterDirect<DoubleInput, int>(
        nameof(Precision),
        o => o.Precision,
        (o, v) => o.Precision = v,
        unsetValue: 2,
        defaultBindingMode: BindingMode.TwoWay
    );

    public string? Label
    {
        set => SetAndRaise(LabelProperty, ref _label, value);
        get => _label;
    }

    public double Value
    {
        set => SetAndRaise(ValueProperty, ref _number, value);
        get => _number;
    }

    public string? Unit
    {
        set => SetAndRaise(UnitProperty, ref _unit, value);
        get => _unit;
    }

    public int Precision
    {
        set => SetAndRaise(PrecisionProperty, ref _precision, value);
        get => _precision;
    }

    static DoubleInput()
    {
        ValueProperty.Changed.AddClassHandler<DoubleInput>((o, e) => o.NumberChanged(e));
    }

    public DoubleInput()
    {
        InitializeComponent();

        PART_Text.Text = "0";

        PART_Text.TextChanging += (sender, e) =>
        {
            e.Handled = true;
            if (_updatingText) return;

            _updatingText = true;
            if (string.IsNullOrEmpty(PART_Text.Text))
            {
                // special case for empty string
                // not doing this results to having 0 in textbox when input is cleared
                _previousText = string.Empty;
                Value = 0;
            }
            else if (StringUtility.TryParseDouble(PART_Text.Text, out double result))
            {
                result = Math.Round(result, Precision);
                var resultStr = !PART_Text.Text.EndsWith(".")
                    ? result.ToString(CultureInfo.InvariantCulture)
                    : PART_Text.Text;
                _previousText = PART_Text.Text = resultStr;
                Value = result;
            }
            else
            {
                PART_Text.Text = _previousText;
            }
            _updatingText = false;
        };
    }

    private void NumberChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_updatingText) return;

        _previousText = e.NewValue?.ToString() ?? "0";

        _updatingText = true;
        PART_Text.Text = e.NewValue is double newValue
            ? newValue.ToString(CultureInfo.InvariantCulture)
            : "0";
        _updatingText = false;
    }
}
