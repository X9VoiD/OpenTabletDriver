using System.Windows.Input;
using Eto.Forms;
using Eto.SkiaDraw;
using SkiaSharp;

namespace OpenTabletDriver.UX.Controls
{
    public class ClickablePanel : Panel
    {
        public ICommand? Command { get; set; }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            Cursor = new Cursor(CursorType.Pointer);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Cursor = new Cursor(CursorType.Default);
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Buttons == MouseButtons.Primary)
                Command?.Execute(null);
            base.OnMouseDown(e);
        }
    }

    public class CirclePlaceholder : SkiaDrawable
    {
        private readonly SKPaint _paint;
        private float? _radius;

        public float? Radius
        {
            get => _radius;
            set
            {
                if (_radius != value)
                {
                    _radius = value;
                    RadiusChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public float StrokeWidth
        {
            get => _paint.StrokeWidth;
            set
            {
                if (_paint.StrokeWidth != value)
                {
                    _paint.StrokeWidth = value;
                    Invalidate();
                }
            }
        }

        public EventHandler? RadiusChanged;

        public CirclePlaceholder()
        {
            _paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.LightSlateGray,
                StrokeWidth = 3,
                IsAntialias = true
            };
        }

        protected override void OnPaint(SKPaintEventArgs e)
        {
            var info = e.Info;
            var width = info.Width;
            var height = info.Height;

            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            var radius = Radius is float r
                ? r
                : (Math.Min(width, height) / 2) - StrokeWidth;

            canvas.DrawCircle(width / 2, height / 2, radius, _paint);
        }
    }
}
