using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.UI.ViewModels;

namespace OpenTabletDriver.UI.Controls;

public partial class AreaDisplay : UserControl
{
    private static IImmutableSolidColorBrush _boundsBrush = new ImmutableSolidColorBrush(0x0AFFFFFF);
    private static IImmutableSolidColorBrush _mappingBrush = new ImmutableSolidColorBrush(0x800078D7);
    private Rect _maximumBounds;
    private double _scale;
    private double _xOffset;
    private double _yOffset;
    private bool _captured;
    private Point _hitPoint;

    public AreaDisplay()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is AreaDisplayViewModel vm)
        {
            _maximumBounds = ToRect(vm.MaximumBounds);

            _xOffset = Math.Abs(Math.Min(vm.MaximumBounds.X, 0));
            _yOffset = Math.Abs(Math.Min(vm.MaximumBounds.Y, 0));

            this.ContextMenu = new ContextMenu
            {
                ItemsSource = new[]
                {
                    new MenuItem
                    {
                        Header = "Map to...",
                        ItemsSource = CreateQuickMapMenuItems(vm)
                    }
                }
            };

            foreach (var bounds in vm.Bounds)
            {
                var border = CreateFromBounds(bounds);
                AreaCanvas.Children.Add(border);
            }

            var mappingBorder = CreateFromMap(vm.Mapping);
            AreaCanvas.Children.Add(mappingBorder);
        }

        base.OnDataContextChanged(e);
    }

    private MenuItem[] CreateQuickMapMenuItems(AreaDisplayViewModel vm)
    {
        static MenuItem createQuickMapMenuItem(AreaDisplayViewModel vm, Bounds bounds)
        {
            return new MenuItem
            {
                Header = bounds.Name,
                Command = new RelayCommand(() =>
                {
                    vm.Mapping.X = bounds.X;
                    vm.Mapping.Y = bounds.Y;
                    vm.Mapping.Width = bounds.Width;
                    vm.Mapping.Height = bounds.Height;
                })
            };
        }

        var fullVirtualDesktop = createQuickMapMenuItem(vm, vm.MaximumBounds);

        return vm.Bounds.Select(b => createQuickMapMenuItem(vm, b)).Prepend(fullVirtualDesktop).ToArray();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        base.ArrangeOverride(finalSize);

        if (DataContext is AreaDisplayViewModel vm)
        {
            var padding = AreaBorder.Padding;
            var maxCanvasSize = finalSize.Deflate(padding);

            var scaledWidth = maxCanvasSize.Height / vm.MaximumBounds.Height * vm.MaximumBounds.Width;
            var scaledHeight = maxCanvasSize.Width / vm.MaximumBounds.Width * vm.MaximumBounds.Height;

            if (scaledWidth >= maxCanvasSize.Width)
            {
                AreaCanvas.Width = maxCanvasSize.Width;
                AreaCanvas.Height = scaledHeight;
            }
            else
            {
                AreaCanvas.Width = scaledWidth;
                AreaCanvas.Height = maxCanvasSize.Height;
            }

            _scale = AreaCanvas.Bounds.Width / vm.MaximumBounds.Width;

            foreach (var border in AreaCanvas.Children.OfType<Border>())
            {
                var mapping = (Mapping)border.Tag!;
                SetSize(border, mapping, _scale);
                SetPosition(border, mapping, _scale);
            }
        }

        return finalSize;
    }

    private Border CreateFromBounds(Bounds bounds)
    {
        var border = new Border
        {
            Background = _boundsBrush,
            CornerRadius = new CornerRadius(6),
            Tag = bounds
        };

        return border;
    }

    private Border CreateFromMap(Mapping mapping)
    {
        var mapVisual = new Border
        {
            Background = _mappingBrush,
            CornerRadius = new CornerRadius(6),
            BorderBrush = Brushes.White,
            BorderThickness = new Thickness(1),
            Tag = mapping
        };

        mapping.PropertyChanged += (s, e) =>
        {
            this.InvalidateArrange();
        };

        mapVisual.PointerPressed += (object? s, PointerPressedEventArgs e) =>
        {
            var point = e.GetCurrentPoint(mapVisual);
            if (point.Properties.IsLeftButtonPressed)
            {
                _captured = true;
                _hitPoint = point.Position;
                e.Pointer.Capture(mapVisual);
            }
        };

        mapVisual.PointerReleased += (object? s, PointerReleasedEventArgs e) =>
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
            {
                _captured = false;
                e.Pointer.Capture(null);
            }
        };

        mapVisual.PointerMoved += (object? s, PointerEventArgs e) =>
        {
            if (_captured)
            {
                var point = e.GetPosition(AreaCanvas) - _hitPoint;
                var mapping = (Mapping)mapVisual.Tag!;
                mapping.X = (int)Math.Round((point.X / _scale) - _xOffset);
                mapping.Y = (int)Math.Round((point.Y / _scale) - _yOffset);

                RestrictToVisibleBounds(mapping);
            }
        };

        return mapVisual;
    }

    private void SetSize(Border border, Mapping mapping, double scale)
    {
        border.Width = (mapping.Width * scale) - 1;
        border.Height = (mapping.Height * scale) - 1;
    }

    private void SetPosition(Border border, Mapping mapping, double scale)
    {
        Canvas.SetLeft(border, (mapping.X + _xOffset) * scale);
        Canvas.SetTop(border, (mapping.Y + _yOffset) * scale);
    }

    private void RestrictToVisibleBounds(Mapping mapping)
    {
        var mappingRect = ToRect(mapping);
        var centerPoint = mappingRect.Center;

        if (centerPoint.X < _maximumBounds.Left)
        {
            mapping.X = (int)Math.Round(_maximumBounds.Left - mappingRect.Width / 2);
        }
        else if (centerPoint.X > _maximumBounds.Right)
        {
            mapping.X = (int)Math.Round(_maximumBounds.Right - mappingRect.Width / 2);
        }

        if (centerPoint.Y < _maximumBounds.Top)
        {
            mapping.Y = (int)Math.Round(_maximumBounds.Top - mappingRect.Height / 2);
        }
        else if (centerPoint.Y > _maximumBounds.Bottom)
        {
            mapping.Y = (int)Math.Round(_maximumBounds.Bottom - mappingRect.Height / 2);
        }
    }

    private static Rect ToRect(Mapping mapping)
    {
        return new Rect(
            mapping.X,
            mapping.Y,
            mapping.Width,
            mapping.Height
        );
    }
}
