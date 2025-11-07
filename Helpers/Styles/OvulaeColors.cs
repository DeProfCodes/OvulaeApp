using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.Styles
{
    public class OvulaeColors
    {
        public static class COLOR
        {
            // Using Microsoft.Maui.Graphics
            public static Color ThemeClrMain = Color.FromArgb("#B5A4DE");
            public static Color ThemeClrMainLight = Color.FromArgb("#F2EBFC");
            public static Color ThemeClr2 = Color.FromArgb("#CB6CE6");
            public static Color ThemeClr3 = Color.FromArgb("#4506DD");
            public static Color ThemeClrPurple = Color.FromArgb("#DDE3FF");
            public static Color ThemeClrLight = Color.FromArgb("#FFF5FF");
            public static Color ThemeClrLight2 = Color.FromArgb("#FFDAFA");
            public static Color ThemeGray = Color.FromArgb("#DADADA");
            public static Color ThemeGray2 = Color.FromArgb("#828282");
            public static Color ThemeLightGray = Color.FromArgb("#EEEEEE");
        }

        public static class BRUSH
        {
            public static Brush BrushThemeClrMain = new SolidColorBrush(Color.FromArgb("#B5A4DE"));
            public static Brush BrushThemeClrMainLight = new SolidColorBrush(Color.FromArgb("#F2EBFC"));
            public static Brush BrushThemeClr2 = new SolidColorBrush(Color.FromArgb("#CB6CE6"));
            public static Brush BrushThemeClr3 = new SolidColorBrush(Color.FromArgb("#4506DD"));
            public static Brush BrushThemeClrPurple = new SolidColorBrush(Color.FromArgb("#DDE3FF"));
            public static Brush BrushThemeClrLight = new SolidColorBrush(Color.FromArgb("#FFF5FF"));
            public static Brush BrushThemeClrLight2 = new SolidColorBrush(Color.FromArgb("#FFDAFA"));
            public static Brush BrushThemeGray = new SolidColorBrush(Color.FromArgb("#DADADA"));
            public static Brush BrushThemeGray2 = new SolidColorBrush(Color.FromArgb("#828282"));
            public static Brush BrushThemeLightGray = new SolidColorBrush(Color.FromArgb("#EEEEEE"));
        }
    }
}
