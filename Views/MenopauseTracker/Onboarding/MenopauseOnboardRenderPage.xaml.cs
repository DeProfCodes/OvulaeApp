using System.Threading.Tasks;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.ViewModels.MenopauseTracker;
using OvulaeApp.ViewModels.PeriodTracker;
using OvulaeApp.Views.Components.Controls;
using OvulaeApp.Views.Components.Shared;

namespace OvulaeApp.Views.MenopauseTracker.Onboarding
{
    public partial class MenopauseOnboardRenderPage : ContentPage
    {
        private readonly MenopauseOnboardViewModel vm;

        public MenopauseOnboardRenderPage()
        {
            InitializeComponent();

            vm = new MenopauseOnboardViewModel(AppSpinner);

            BindingContext = vm;

            MyYearPicker.SetBinding(
                YearPicker.SelectedYearProperty,
                new Binding("SelectedYear", source: vm, mode: BindingMode.TwoWay)
            );

            ShowIntraMarketingPage();
        }

        protected override bool OnBackButtonPressed()
        {
            try
            {
                if (vm.CurrentIndex > 0)
                {
                    var isJumpIndex = vm.CurrentIndex == 5 || vm.CurrentIndex == 5;
                    if (isJumpIndex)
                    {
                        if (vm.CurrentIndex == 5)
                            vm.JumpToIndex(1);
                        if (vm.CurrentIndex == 6)
                        {
                            var idx = PeriodsQuestionOption.SelectedIndex == 1 ? 5 : 4;
                            vm.JumpToIndex(idx);
                        }
                    }
                    else
                    {
                        vm.GoBack();
                    }
                    ShowIntraMarketingPage();
                    return true;
                }
                if (vm.CurrentIndex == 0)
                {
                    Shell.Current.GoToAsync(nameof(MenopauseOnboardWelcomePage));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failure, error: {ex.Message}");
            }
            return base.OnBackButtonPressed();
        }

        private void ShowIntraMarketingPage()
        {
            BenefitsAdPage.IsVisible = vm.CurrentIndex == 0;
            DashboardAdPage.IsVisible = vm.CurrentIndex == 6;
            CalendarAdPage.IsVisible = vm.CurrentIndex == 9;
            ChatBotAdPage.IsVisible = vm.CurrentIndex == 12;
            EducationBooksAdPage.IsVisible = vm.CurrentIndex == 15;
            PartnerShareAdPage.IsVisible = vm.CurrentIndex == 18;
        }

        private async void GoBackDotButtonTapped(object sender, TappedEventArgs e)
        {
            try
            {
                var isJumpIndex = vm.CurrentIndex == 5 || vm.CurrentIndex == 5;
                if (isJumpIndex)
                {
                    if (vm.CurrentIndex == 5)
                        vm.JumpToIndex(1);
                    if (vm.CurrentIndex == 6)
                    {
                        var idx = PeriodsQuestionOption.SelectedIndex == 1 ? 5 : 4;
                        vm.JumpToIndex(idx);
                    }
                }
                else
                {
                    vm.GoBack();
                }
                ShowIntraMarketingPage();

                if (vm.CurrentIndex == 0)
                {
                    await AppSpinner.ShowSpinnerAsync();
                    Shell.Current.GoToAsync(nameof(MenopauseOnboardWelcomePage));
                    await AppSpinner.HideSpinnerAsync();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Navigation failure, error: {ex.Message}");
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
                        vm.WeightUnit = "kg";
                    }
                    else if (entry == msLBS)
                    {
                        msKG.Background = Brush.White;
                        msKG.Stroke = Brush.White;
                        msKGLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        msLBS.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msLBS.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msLBSLabel.TextColor = Colors.White;
                        vm.WeightUnit = "lbs";
                    }
                    else if (entry == msCM)
                    {
                        msCM.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msCM.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msCMLabel.TextColor = Colors.White;
                        msIN.Background = Brush.White;
                        msIN.Stroke = Brush.White;
                        msINLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        vm.HeightUnit = "cm";
                    }
                    else if (entry == msIN)
                    {
                        msCM.Background = Brush.White;
                        msCM.Stroke = Brush.White;
                        msCMLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        msIN.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msIN.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msINLabel.TextColor = Colors.White;
                        vm.HeightUnit = "ft/in";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to adjust unit measure, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while adjusting unit measure.", "OK");
            }
        }

        private void MultiSelect_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender is SelectComponent select)
                {
                    var atLeastOneOptionSelected = (select.SelectionType == SelectionType.Multiple && select.SelectedValues.Count() > 0 ) ||
                                                   (select.SelectionType == SelectionType.Single && select.SelectedIndex >= 0);

                    vm.CurrentSlide.IsNextEnabled = atLeastOneOptionSelected;
                }
                else if (sender is SelectComponentCheck selectCheck)
                {
                    var atLeastOneOptionSelected = selectCheck.SelectedItems.Count > 0;
                    vm.CurrentSlide.IsNextEnabled = atLeastOneOptionSelected; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to answer navigating questions, error: {ex.Message}");
            }
        }

        private async void NextOnboardBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (vm.CurrentIndex < vm.Slides.Count - 1)
                {
                    var isJumpIndex = (vm.CurrentIndex == 1 && PeriodsQuestionOption.SelectedIndex == 1) || (vm.CurrentIndex == 4);
                    if (isJumpIndex)
                    {
                        if (vm.CurrentIndex == 1) vm.JumpToIndex(5);
                        if (vm.CurrentIndex == 4) vm.JumpToIndex(6);
                    }
                    else
                    {
                        vm.GoNext();
                    }
                    ShowIntraMarketingPage();
                    DotBackButton.IsVisible = vm.CurrentIndex > 0;
                }
                else
                {
                    LocalStorageService.UserBodyMetrics.Year = vm.SelectedYear;
                    LocalStorageService.UserBodyMetrics.Weight = WeightValue.Value;
                    LocalStorageService.UserBodyMetrics.WeightUnit = vm.WeightUnit;
                    LocalStorageService.UserBodyMetrics.Height = HeightValue.Value;
                    LocalStorageService.UserBodyMetrics.HeightUnit = vm.HeightUnit;

                    if (SymptomsCheck.SelectedItems.Count > 0)
                    {
                        LocalStorageService.UserCycleProfile.Symptoms = SymptomsCheck.SelectedItems.Select(s => s).ToList();
                    }
                    if (BloodTypeOption.SelectedIndex >= 0)
                    {
                        var bloodType = BloodTypeOption.SelectedValue;
                        LocalStorageService.UserBodyMetrics.BloodGroup = bloodType.Substring(0, bloodType.Length - 1);
                        LocalStorageService.UserBodyMetrics.RhFactor = $"{bloodType[bloodType.Length - 1]}";
                    }
                    if (TreatmentsVals.SelectedItems.Count > 0)
                    {
                        LocalStorageService.UserCycleProfile.Treatments = TreatmentsVals.SelectedItems.Select(s => s).ToList();
                    }
                    if (HealthConditionsOptions.SelectedValues.Count > 0)
                    {
                        LocalStorageService.UserCycleProfile.HealthConditions = HealthConditionsOptions.SelectedValues.Select(s => s).ToList();
                    }

                    DateTime LMP = LastPeriodDate.SelectedDate;
                    if (PeriodsQuestionOption.SelectedIndex == 1)
                    {
                        LMP = NoPeriodsLMPQuestionOption.SelectedIndex == 0 ? ApproximateLMP.SelectedDate : DateTime.Now.AddYears(-1);
                    }

                    LocalStorageService.UserCycleProfile.LastPeriodDate = LMP;
                    LocalStorageService.UserCycleProfile.CycleLengthDays = PeriodCycleLength.Value;
                    LocalStorageService.UserCycleProfile.PeriodLengthDays = PeriodDuration.Value;
                    LocalStorageService.UserCycleProfile.TrackingStartDate = DateTime.Now;

                    LocalStorageService.PeriodTrackerSet = true;

                    await AppSpinner.ShowSpinnerAsync();
                    await Shell.Current.GoToAsync(nameof(MenopauseOnboardFinishPage));
                    await AppSpinner.HideSpinnerAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to navigate to next page, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating onboarding questions.", "OK");
            }
        }

        private void IntraMarketingPageContainer_BackClicked(object sender, EventArgs e)
        {
            try
            {
                var isJumpIndex = vm.CurrentIndex == 5 || vm.CurrentIndex == 5;
                if (isJumpIndex)
                {
                    if (vm.CurrentIndex == 5) 
                        vm.JumpToIndex(1);
                    if (vm.CurrentIndex == 6)
                    {
                        var idx = PeriodsQuestionOption.SelectedIndex == 1 ? 5 : 4;
                        vm.JumpToIndex(idx);
                    }
                }
                else
                {
                    vm.GoBack();
                }
                ShowIntraMarketingPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failure, error: {ex.Message}");
            }
        }

        private void NoPeriodsLMPQuestionOption_SelectionChanged(object sender, EventArgs e)
        {
            ApproximateLMPContainer.IsVisible = NoPeriodsLMPQuestionOption.SelectedIndex == 0;
            vm.CurrentSlide.IsNextEnabled = true;
        }
    }
}
