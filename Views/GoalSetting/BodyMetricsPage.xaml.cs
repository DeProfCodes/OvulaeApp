using OvulaeApp.ViewModels.GoalSetting;
using OvulaeApp.Views.Components.Controls;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Views.PregnancyTracker.Onboarding;

namespace OvulaeApp.Views.GoalSetting
{
    public partial class BodyMetricsPage : ContentPage
    {
        private BodyMetricsViewModel _viewModel;
            
        public BodyMetricsPage()
        {
            InitializeComponent();
            _viewModel = new BodyMetricsViewModel();
            BindingContext = _viewModel;

            MyYearPicker.SetBinding(
                YearPicker.SelectedYearProperty,
                new Binding("SelectedYear", source: _viewModel, mode: BindingMode.TwoWay)
            );
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                _viewModel.SelectedYear = LocalStorageService.UserBodyMetrics.Year > 0 ? LocalStorageService.UserBodyMetrics.Year : 2005;
                _viewModel.WeightStepper = LocalStorageService.UserBodyMetrics.Weight;
                WeightMeasureLabel.Text = LocalStorageService.UserBodyMetrics.WeightUnit ?? "kg";
                _viewModel.HeightStepper = LocalStorageService.UserBodyMetrics.Height;
                HeightMeasureLabel.Text = LocalStorageService.UserBodyMetrics.HeightUnit ?? "cm";

                WeightMeasureLabel.Text = string.IsNullOrEmpty(WeightMeasureLabel.Text) ? "kg" : WeightMeasureLabel.Text;
                HeightMeasureLabel.Text = string.IsNullOrEmpty(HeightMeasureLabel.Text) ? "cm" : HeightMeasureLabel.Text;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to open body metrics page, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while opening body metrics page.", "OK");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            try
            {
                Shell.Current.GoToAsync(nameof(PregnancyOnboardWelcomePage));
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                return base.OnBackButtonPressed();
            }
        }

        private async void YearBorder_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                await MyYearPicker.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Year picker failed to open, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while opening year picker.", "OK");
            }
        }

        private void WeightStepper_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (sender is Border entry)
                {
                    var currentWeight = Convert.ToInt32(Weight.Text);
                    var newWeight = (entry == wDOWN) ? (currentWeight - 1) : (currentWeight + 1);
                    Weight.Text = $"{newWeight}";
                    WeightValLabel.Text = $"{newWeight}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to adjust weight, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while adjusting weight.", "OK");
            }
        }

        private void HeightStepper_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (sender is Border entry)
                {
                    var currentHeight = Convert.ToInt32(Height.Text);
                    var newHeight = (entry == hDOWN) ? (currentHeight - 1) : (currentHeight + 1);
                    Height.Text = $"{newHeight}";
                    HeightValLabel.Text = $"{newHeight}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to adjust height, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while adjusting height.", "OK");
            }
        }

        private void MeasurementType_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (sender is Border entry)
                {
                    if (entry == msKG)
                    {
                        msKG.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msKG.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msKGLabel.TextColor = Colors.White;
                        msLBS.Background = Brush.White;
                        msLBS.Stroke = Brush.White;
                        msLBSLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        WeightMeasureLabel.Text = "kg";
                    }
                    else if (entry == msLBS)
                    {
                        msKG.Background = Brush.White;
                        msKG.Stroke = Brush.White;
                        msKGLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        msLBS.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msLBS.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msLBSLabel.TextColor = Colors.White;
                        WeightMeasureLabel.Text = "lbs";
                    }
                    else if (entry == msCM)
                    {
                        msCM.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msCM.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msCMLabel.TextColor = Colors.White;
                        msIN.Background = Brush.White;
                        msIN.Stroke = Brush.White;
                        msINLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        HeightMeasureLabel.Text = "cm";
                    }
                    else if (entry == msIN)
                    {
                        msCM.Background = Brush.White;
                        msCM.Stroke = Brush.White;
                        msCMLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        msIN.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msIN.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msINLabel.TextColor = Colors.White;
                        HeightMeasureLabel.Text = "ft/in";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to adjust unit measure, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while adjusting unit measure.", "OK");
            }
        }

        private void Entries_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry)
            {
                if (sender == Weight)
                {
                    WeightValLabel.Text = Weight.Text;
                }
                else if (sender == Height)
                {
                    HeightValLabel.Text = Height.Text;
                }
            }
        }

        private void Entries_Unfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry)
            {
                if (sender == Weight)
                {
                    var currentWeight = !string.IsNullOrEmpty(Weight.Text) ? Convert.ToInt32(Weight.Text) : 0;
                    if (currentWeight < 30)
                    {
                        Weight.Text = "30";
                    }
                    WeightValLabel.Text = Weight.Text;
                }
                else if (sender == Height)
                {
                    var currentHeight = !string.IsNullOrEmpty(Height.Text) ? Convert.ToInt32(Height.Text) : 0;
                    if (currentHeight < 30)
                    {
                        Height.Text = "30";
                    }
                    HeightValLabel.Text = Height.Text;
                }
            }
        }

        private async void NextButtonClicked(object sender, EventArgs e)
        {
            try
            {
                LocalStorageService.UserBodyMetrics.Year = _viewModel.SelectedYear;
                LocalStorageService.UserBodyMetrics.Weight = Convert.ToInt32(Weight.Text);
                LocalStorageService.UserBodyMetrics.WeightUnit = WeightMeasureLabel.Text;
                LocalStorageService.UserBodyMetrics.Height = Convert.ToInt32(Height.Text);
                LocalStorageService.UserBodyMetrics.HeightUnit = HeightMeasureLabel.Text;

                await Spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync(nameof(PregnancyOnboardRenderPage));
                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Failed to capture body metrics data, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while capturing body metrics data.", "OK");
            }
        }
    }
}
