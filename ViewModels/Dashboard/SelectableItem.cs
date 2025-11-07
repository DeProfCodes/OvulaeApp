using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OvulaeApp.Views.Components.Dashboard;

namespace OvulaeApp.ViewModels.Dashboard
{
    public class SelectableItem : INotifyPropertyChanged
    {
        public string Text { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BackgroundColor));
                    OnPropertyChanged(nameof(TextColor));
                    OnPropertyChanged(nameof(FontAttributes));
                    OnPropertyChanged(nameof(FontSize));
                    Parent?.RaiseSelectionChanged(); // Optional
                }
            }
        }

        public MultiSelectLogItemComponent Parent { get; set; }
        public Command ToggleCommand { get; }

        public SelectableItem(string text, MultiSelectLogItemComponent parent, bool isSelected = false)
        {
            Text = text;
            Parent = parent;
            IsSelected = isSelected;
            ToggleCommand = new Command(Toggle);
        }

        private void Toggle()
        {
            if (Parent.IsSingleSelect)
            {
                // Deselect all other items
                foreach (var item in Parent.Items)
                {
                    if (item != this)
                        item.IsSelected = false;
                }
            }

            IsSelected = !IsSelected;
            Parent.RaiseSelectionChanged();
            OnPropertyChanged(nameof(IsSelected)); // notify UI to refresh
        }

        public Brush BackgroundColor => IsSelected
            ? Parent.SelectedColor
            : Parent.UnselectedColor;

        public Color TextColor => IsSelected
            ? Parent.SelectedTextColor
            : Parent.UnselectedTextColor;

        public FontAttributes FontAttributes => IsSelected ? FontAttributes.Bold : FontAttributes.None;

        public double FontSize => Parent.ItemFontSize;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
