using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.UI
{
    public static class ColorUtils
    {
        public static Color GetWeekMarkerTextColor(Color background)
        {
            var bgLum = ComputeLuminance(background);

            // Priority fallback list
            var candidateColors = new[]
            {
            Color.FromArgb("#CB6CE6"), // ThemeClr2 (priority)
            Color.FromArgb("#B5A4DE"), // ThemeClrMain
            Color.FromArgb("#4506DD"), // ThemeClr3
            Color.FromArgb("#00BFA5"), // Bright Teal
            Color.FromArgb("#FF7043"), // Bright Coral
            Colors.White                // Last fallback
        };

            foreach (var color in candidateColors)
            {
                double contrast = ContrastRatio(bgLum, ComputeLuminance(color));
                if (contrast >= 3.5) // acceptable for small text
                    return color;
            }

            // If all else fails, return white.
            return Colors.White;
        }

        private static double ComputeLuminance(Color color)
        {
            double r = color.Red <= 0.03928 ? color.Red / 12.92 : Math.Pow((color.Red + 0.055) / 1.055, 2.4);
            double g = color.Green <= 0.03928 ? color.Green / 12.92 : Math.Pow((color.Green + 0.055) / 1.055, 2.4);
            double b = color.Blue <= 0.03928 ? color.Blue / 12.92 : Math.Pow((color.Blue + 0.055) / 1.055, 2.4);
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        private static double ContrastRatio(double lum1, double lum2)
        {
            double L1 = Math.Max(lum1, lum2);
            double L2 = Math.Min(lum1, lum2);
            return (L1 + 0.05) / (L2 + 0.05);
        }
    }
}
