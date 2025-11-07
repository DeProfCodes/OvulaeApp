using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace OvulaeApp.Views.Components.Dashboard
{
    public class DashboardTrackerHeroView : SKCanvasView
    {
        public static readonly BindableProperty StartColorProperty =
            BindableProperty.Create(
                nameof(StartColor),
                typeof(Color),
                typeof(DashboardTrackerHeroView),
                Colors.Purple,
                propertyChanged: Redraw);

        public static readonly BindableProperty MiddleColorProperty =
            BindableProperty.Create(
                nameof(MiddleColor),
                typeof(Color),
                typeof(DashboardTrackerHeroView),
                Colors.IndianRed,
                propertyChanged: Redraw);

        public static readonly BindableProperty EndColorProperty =
            BindableProperty.Create(
                nameof(EndColor),
                typeof(Color),
                typeof(DashboardTrackerHeroView),
                Colors.LightBlue,
                propertyChanged: Redraw);

        public static readonly BindableProperty IsPregnancyTrackerProperty =
            BindableProperty.Create(
                nameof(IsPregnancyTracker),
                typeof(bool),
                typeof(DashboardTrackerHeroView),
                false,
                propertyChanged: Redraw);

        private static void Redraw(BindableObject bindable, object oldValue, object newValue)
        {
            ((DashboardTrackerHeroView)bindable).InvalidateSurface();
        }

        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        public Color MiddleColor
        {
            get => (Color)GetValue(MiddleColorProperty);
            set => SetValue(MiddleColorProperty, value);
        }

        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }

        public bool IsPregnancyTracker
        {
            get => (bool)GetValue(IsPregnancyTrackerProperty);
            set => SetValue(IsPregnancyTrackerProperty, value);
        }

        public DashboardTrackerHeroView()
        {
            this.PaintSurface += OnPaintSurface;
            this.IgnorePixelScaling = true;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var width = e.Info.Width;
            var height = e.Info.Height;

            canvas.Clear();

            using var path = new SKPath();
            path.MoveTo(0, 0);
            path.LineTo(0, 310);
            path.QuadTo(width / 2, 360, width, 310);
            path.LineTo(width, 0);
            path.LineTo(0, 0);

            using var shadowPaint = new SKPaint
            {
                Color = SKColors.Black.WithAlpha((byte)(0.25 * 255)),
                ImageFilter = SKImageFilter.CreateDropShadow(0, 4, 10, 10, SKColors.Black.WithAlpha((byte)(0.25 * 255))),
                IsAntialias = true
            };
            canvas.DrawPath(path, shadowPaint);

            using var paint = new SKPaint
            {
                IsAntialias = true
            };

            if (IsPregnancyTracker)
            {
                // Radial gradient with Start, Middle, End
                paint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(width / 2, height / 2),
                    Math.Max(width, height) / 2,
                    new[]
                    {
                        StartColor.ToSKColor(),
                        MiddleColor.ToSKColor(),
                        EndColor.ToSKColor()
                    },
                    new float[] { 0, 0.5f, 1 },
                    SKShaderTileMode.Clamp);
            }
            else
            {
                // Linear gradient with Start and End
                paint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    new SKPoint(width, height),
                    new[]
                    {
                        StartColor.ToSKColor(),
                        EndColor.ToSKColor()
                    },
                    null,
                    SKShaderTileMode.Clamp);
            }

            canvas.DrawPath(path, paint);
        }
    }
}
