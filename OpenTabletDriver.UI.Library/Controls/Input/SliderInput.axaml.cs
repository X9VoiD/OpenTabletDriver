using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace OpenTabletDriver.UI.Controls.Input;

public partial class SliderInput : UserControl
{
    private string? _label;
    private float _number;
    private float? _min;
    private float? _max;
    private bool _slider;

    private bool _updatingText;
    private string _previousText = string.Empty;

    public static readonly DirectProperty<SliderInput, string?> LabelProperty = AvaloniaProperty.RegisterDirect<SliderInput, string?>(
        nameof(Label),
        o => o.Label,
        (o, v) => o.Label = v
    );

    public static readonly DirectProperty<SliderInput, float> NumberProperty = AvaloniaProperty.RegisterDirect<SliderInput, float>(
        nameof(Number),
        o => o.Number,
        (o, v) => o.Number = v
    );

    public static readonly DirectProperty<SliderInput, float?> MinimumProperty = AvaloniaProperty.RegisterDirect<SliderInput, float?>(
        nameof(Minimum),
        o => o.Minimum,
        (o, v) => o.Minimum = v
    );

    public static readonly DirectProperty<SliderInput, float?> MaximumProperty = AvaloniaProperty.RegisterDirect<SliderInput, float?>(
        nameof(Maximum),
        o => o.Maximum,
        (o, v) => o.Maximum = v
    );

    public static readonly DirectProperty<SliderInput, bool> SliderProperty = AvaloniaProperty.RegisterDirect<SliderInput, bool>(
        nameof(Slider),
        o => o.Slider,
        (o, v) => o.Slider = v
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

    public float? Minimum
    {
        set => SetAndRaise(MinimumProperty, ref _min, value);
        get => _min;
    }

    public float? Maximum
    {
        set => SetAndRaise(MaximumProperty, ref _max, value);
        get => _max;
    }

    public bool Slider
    {
        set => SetAndRaise(SliderProperty, ref _slider, value);
        get => _slider;
    }

    static SliderInput()
    {
        NumberProperty.Changed.AddClassHandler<SliderInput>((o, e) => o.NumberChanged(e));
    }

    public SliderInput()
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

        PART_Text.LostFocus += (sender, e) =>
        {
            if (Slider)
            {
                SetupManualInput(false);
                Debug.WriteLine("SliderInput: Lost focus");
            }
        };

        PART_SliderText.DoubleTapped += (sender, e) =>
        {
            if (Slider)
            {
                SetupManualInput(true);
                Debug.WriteLine("SliderInput: Double tapped");
            }

            e.Handled = true;
        };

        PART_SliderText.PointerEntered += (sender, e) =>
        {
            this.Cursor = new Cursor(StandardCursorType.Ibeam);
        };

        PART_SliderText.PointerExited += (sender, e) =>
        {
            this.Cursor = new Cursor(StandardCursorType.Arrow);
        };
    }

    private void NumberChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_updatingText) return;

        _updatingText = true;
        _previousText = e.NewValue?.ToString() ?? string.Empty;

        if (e.NewValue is float newValue)
        {
            if (Minimum.HasValue && newValue < Minimum.Value)
            {
                newValue = Minimum.Value;
                Number = Minimum.Value;
            }
            else if (Maximum.HasValue && newValue > Maximum.Value)
            {
                newValue = Maximum.Value;
                Number = Maximum.Value;
            }

            var textString = newValue.ToString(CultureInfo.InvariantCulture);

            PART_Text.Text = textString;
        }
        else
        {
            PART_Text.Text = string.Empty;
        }
        _updatingText = false;
    }

    private void SetupManualInput(bool inputtingManually)
    {
        PART_SliderText.Classes.Set("manual", inputtingManually);
        PART_Text.Classes.Set("manual", inputtingManually);
        PART_SliderText.Classes.Set("slider", !inputtingManually);
        PART_Text.Classes.Set("slider", !inputtingManually);

        if (inputtingManually)
        {
            PART_Text.Focus();
        }
    }
}
