using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Helpers.Styles;

namespace OvulaeApp.ViewModels.Settings
{
    public class EditFieldProperty : BaseViewModel
    {
        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        private Color _color2;
        public Color Color2
        {
            get => _color2;
            set
            {
                _color2 = value;
                OnPropertyChanged();
            }
        }

        public void Update(string newValue)
        {
            Value = newValue;
            Color = Colors.Red;
        }

        public EditFieldProperty(string value)
        {
            Value = value;
            Color = OvulaeColors.COLOR.ThemeClr3;
        }
    }
}
