using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Models.UI
{
    public class BorderSelectOption
    {
        public Border OptionContainer { get; set; }

        public Label OptionLabel { get; set; }

        public bool IsSelected { get; set; }

        public BorderSelectOption(Border OptionContainer, Label OptionLabel, bool IsSelected)
        {
            this.OptionContainer = OptionContainer ?? new();
            this.OptionLabel = OptionLabel ?? new();
            this.IsSelected = IsSelected;
        }
    }
}
