using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace OpenTabletDriver.UI.Controls.Input;

public partial class SliderInput : UserControl
{
    private string? _label;
    private double _number;
    private double? _min;
    private double? _max;
    private bool _slider = true;
    private string? _unit;
    private int _precision = 2;

    private bool _updatingText;
    private string _previousText = string.Empty;

    public static readonly DirectProperty<SliderInput, string?> LabelProperty = AvaloniaProperty.RegisterDirect<SliderInput, string?>(
        nameof(Label),
        o => o.Label,
        (o, v) => o.Label = v,
        defaultBindingMode: BindingMode.TwoWay
    );

    public static readonly DirectProperty<SliderInput, double> ValueProperty = AvaloniaProperty.RegisterDirect<SliderInput, double>(
        nameof(Value),
        o => o.Value,
        (o, v) => o.Value = v,
        defaultBindingMode: BindingMode.TwoWay
    );

    public static readonly DirectProperty<SliderInput, double?> MinimumProperty = AvaloniaProperty.RegisterDirect<SliderInput, double?>(
        nameof(Minimum),
        o => o.Minimum,
        (o, v) => o.Minimum = v,
        defaultBindingMode: BindingMode.TwoWay
    );

    public static readonly DirectProperty<SliderInput, double?> MaximumProperty = AvaloniaProperty.RegisterDirect<SliderInput, double?>(
        nameof(Maximum),
        o => o.Maximum,
        (o, v) => o.Maximum = v,
        defaultBindingMode: BindingMode.TwoWay
    );

    public static readonly DirectProperty<SliderInput, bool> ShowSliderProperty = AvaloniaProperty.RegisterDirect<SliderInput, bool>(
        nameof(ShowSlider),
        o => o.ShowSlider,
        (o, v) => o.ShowSlider = v,
        unsetValue: true,
        defaultBindingMode: BindingMode.TwoWay
    );

    public static readonly DirectProperty<SliderInput, string?> UnitProperty = AvaloniaProperty.RegisterDirect<SliderInput, string?>(
        nameof(Unit),
        o => o.Unit,
        (o, v) => o.Unit = v,
        defaultBindingMode: BindingMode.TwoWay
    );

    public static readonly DirectProperty<SliderInput, int> PrecisionProperty = AvaloniaProperty.RegisterDirect<SliderInput, int>(
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

    public double? Minimum
    {
        set => SetAndRaise(MinimumProperty, ref _min, value);
        get => _min;
    }

    public double? Maximum
    {
        set => SetAndRaise(MaximumProperty, ref _max, value);
        get => _max;
    }

    public bool ShowSlider
    {
        set => SetAndRaise(ShowSliderProperty, ref _slider, value);
        get => _slider;
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

    static SliderInput()
    {
        ValueProperty.Changed.AddClassHandler<SliderInput>((o, e) => o.NumberChanged(e));
    }

    public SliderInput()
    {
        InitializeComponent();

        PART_Text.Text = "0";
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
                Value = 0;
                _updatingText = false;
            }
            else if (StringUtility.TryParseDouble(PART_Text.Text, out double result))
            {
                _previousText = PART_Text.Text;
                Value = result;
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
        if (_updatingText)
            return;

        _updatingText = true;
        _previousText = e.NewValue?.ToString() ?? string.Empty;

        if (e.NewValue is double newValue)
        {
            newValue = Math.Round(newValue, (int)Precision);

            if (Minimum.HasValue && newValue < Minimum.Value)
            {
                newValue = Minimum.Value;
                Value = Minimum.Value;
            }
            else if (Maximum.HasValue && newValue > Maximum.Value)
            {
                newValue = Maximum.Value;
                Value = Maximum.Value;
            }

            var textString = newValue.ToString(CultureInfo.InvariantCulture);

            PART_Slider.Value = newValue;
            PART_Text.Text = textString;
        }
        else
        {
            PART_Text.Text = string.Empty;
        }

        _updatingText = false;
    }
}
