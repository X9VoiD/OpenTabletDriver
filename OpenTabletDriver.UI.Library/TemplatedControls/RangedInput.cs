using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace OpenTabletDriver.UI.TemplatedControls;

[TemplatePart("PART_Slider", typeof(Slider))]
public class RangedInput : DoubleInput
{
    public static readonly StyledProperty<double?> MinimumProperty =
        AvaloniaProperty.Register<RangedInput, double?>(nameof(Minimum));

    public static readonly StyledProperty<double?> MaximumProperty =
        AvaloniaProperty.Register<RangedInput, double?>(nameof(Maximum));

    public double? Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double? Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    protected override void HandleValueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is double newValue)
        {
            newValue = Math.Round(newValue, Precision);

            if (Minimum.HasValue && newValue < Minimum.Value)
            {
                newValue = Minimum.Value;
            }
            else if (Maximum.HasValue && newValue > Maximum.Value)
            {
                newValue = Maximum.Value;
            }

            var textString = newValue.ToString(CultureInfo.InvariantCulture);
            Value = newValue;
            Text = textString;
        }
        else
        {
            Text = string.Empty;
        }
    }
}
