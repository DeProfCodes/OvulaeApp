using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.ViewModels.Components.Controls;

namespace OvulaeApp.Views.Components.Controls
{
    public partial class YearPicker : ContentView
    {
        private YearPickerViewModel YearPickerViewModel { get; set; }

        public YearPicker()
        {
            InitializeComponent();

            YearPickerViewModel = new YearPickerViewModel();
            BindingContext = YearPickerViewModel;

            YearPickerViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(YearPickerViewModel.SelectedYear))
                    SelectedYear = YearPickerViewModel.SelectedYear;
            };

            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedYear))
                    YearPickerViewModel.SelectedYear = SelectedYear;
            };

            this.Opacity = 0;
        }

        public async Task ShowAsync()
        {
            this.IsVisible = true;
            this.Opacity = 0;
            await this.FadeTo(1, 200);
        }

        public async Task HideAsync()
        {
            await this.FadeTo(0, 200);
            this.IsVisible = false;
        }

        private async void YearsCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection?.FirstOrDefault() is int selected)
            {
                YearPickerViewModel.SelectedYear = selected;
                await HideAsync(); 
            }
        }

        public static readonly BindableProperty SelectedYearProperty =
            BindableProperty.Create(
                nameof(SelectedYear),
                typeof(int),
                typeof(YearPicker),
                default(int),
                BindingMode.TwoWay,
                propertyChanged: OnSelectedYearChanged);

        public int SelectedYear
        {
            get => (int)GetValue(SelectedYearProperty);
            set => SetValue(SelectedYearProperty, value);
        }

        private static void OnSelectedYearChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is YearPicker picker && newValue is int newYear)
            {
                if (picker.YearPickerViewModel.SelectedYear != newYear)
                    picker.YearPickerViewModel.SelectedYear = newYear;
            }
        }

        private async void YearSelectTapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (sender is BindableObject bindable && bindable.BindingContext is int selectedYear)
                {
                    YearPickerViewModel.SelectedYear = selectedYear;
                    SelectedYear = selectedYear;
                    await HideAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting a year on a year picker: {ex.Message}");
            }
        }

        private async void CloseButtonTapped(object sender, TappedEventArgs e)
        {
            try
            {
                await HideAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing year picker: {ex.Message}");
            }
        }
    }
}