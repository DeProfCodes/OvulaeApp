using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Models.UI;

namespace OvulaeApp.Helpers.UI
{
    public class CommonUIFunctions
    {
        public static void ToggleActionButtonButton(Button actionButton, bool isValidFields)
        {
            actionButton.IsEnabled = isValidFields;
            actionButton.Opacity = isValidFields ? 1 : 0.3;
        }

        public static void SingleSelectBorderOption(BorderSelectOption activate, params BorderSelectOption[] unselectedOptions)
        {
            if (activate != null && activate.OptionContainer != null && activate.OptionLabel != null)
            {
                activate.OptionContainer.Stroke = OvulaeColors.BRUSH.BrushThemeClr2;
                activate.OptionContainer.Background = OvulaeColors.BRUSH.BrushThemeClr2;
                activate.OptionLabel.TextColor = Colors.White;
                activate.OptionLabel.FontAttributes = FontAttributes.Bold;
            }

            foreach (var unselected in unselectedOptions)
            {
                if (unselected.OptionContainer != null && unselected.OptionLabel != null)
                {
                    unselected.OptionContainer.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                    unselected.OptionContainer.Background = Brush.White;
                    unselected.OptionLabel.TextColor = Colors.Black;
                    unselected.OptionLabel.FontAttributes = FontAttributes.None;
                }
            }
        }
    }
}
