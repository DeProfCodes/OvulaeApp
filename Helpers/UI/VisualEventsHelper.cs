
using OvulaeApp.Helpers.Styles;

namespace OvulaeApp.Helpers.UI
{
    public class VisualEventsHelper
    {
        public async static Task TapDimEffectGray(object sender)
        {
            if (sender is Border border)
            {
                border.BackgroundColor = Color.FromArgb("#22000000");
                await Task.Delay(50);
                border.BackgroundColor = Colors.Transparent;
            }
        }

        public async static Task TapDimEffectPurple(object sender)
        {
            if (sender is Border border)
            {
                border.BackgroundColor = OvulaeColors.COLOR.ThemeClrPurple;
                await Task.Delay(50);
                border.BackgroundColor = Colors.Transparent;
            }
        }

        public async static Task TapDimEffectPink(object sender)
        {
            if (sender is Border border)
            {
                border.BackgroundColor = OvulaeColors.COLOR.ThemeClrLight2;
                await Task.Delay(50);
                border.BackgroundColor = Colors.Transparent;
            }
        }

        public static async Task TapDimEffect(object sender)
        {
            if (sender is not Border border || border.Background == null)
                return;

            var originalBrush = border.Background;

            Brush dimmedBrush = originalBrush switch
            {
                SolidColorBrush solid => new SolidColorBrush(DarkenColor(solid.Color, 0.3)),
                LinearGradientBrush gradient => CloneAndDarkenGradient(gradient, 0.3),
                _ => originalBrush
            };

            border.Background = dimmedBrush;
            await Task.Delay(50);
            border.Background = originalBrush;
        }

        private static Color DarkenColor(Color color, double amount)
        {
            amount = Math.Clamp(amount, 0, 1);
            return Color.FromRgba(
                color.Red * (1 - amount),
                color.Green * (1 - amount),
                color.Blue * (1 - amount),
                color.Alpha
            );
        }

        private static LinearGradientBrush CloneAndDarkenGradient(LinearGradientBrush original, double amount)
        {
            var newStops = new GradientStopCollection();

            foreach (var stop in original.GradientStops)
            {
                var darkened = DarkenColor(stop.Color, amount);
                newStops.Add(new GradientStop(darkened, stop.Offset));
            }

            return new LinearGradientBrush
            {
                StartPoint = original.StartPoint,
                EndPoint = original.EndPoint,
                GradientStops = newStops
            };
        }
    }
}
