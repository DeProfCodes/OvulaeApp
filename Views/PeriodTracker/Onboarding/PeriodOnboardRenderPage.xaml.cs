using OvulaeApp.Helpers.Styles;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.ViewModels.PeriodTracker;
using OvulaeApp.Views.Components.Controls;
using OvulaeApp.Views.Components.Shared;
using OvulaeShared.Enums;
using OvulaeShared.Enums.HealthProfile;

namespace OvulaeApp.Views.PeriodTracker.Onboarding
{
    public partial class PeriodOnboardRenderPage : ContentPage
    {
        private readonly PeriodOnboardViewModel vm;

        public PeriodOnboardRenderPage()
        {
            InitializeComponent();

            vm = new PeriodOnboardViewModel(AppSpinner);

            BindingContext = vm;

            MyYearPicker.SetBinding(
                YearPicker.SelectedYearProperty,
                new Binding("SelectedYear", source: vm, mode: BindingMode.TwoWay)
            );

            ShowIntraMarketingPage();
        }

        private void GoBack()
        {
            try
            {
                if (vm.CurrentIndex == 20)
                {
                    var periodIrregularType = EnumHelper.GetEnumValueFromName<PeriodIrregularityType>(CycleExperienceCheck.SelectedItems.FirstOrDefault());
                    LocalStorageService.UserCycleProfile.PeriodIrregularityType = periodIrregularType;
                    if (periodIrregularType == PeriodIrregularityType.OccasionallyIrregular || periodIrregularType == PeriodIrregularityType.FrequentlyIrregular)
                    {
                        vm.JumpToIndex(17);
                    }
                    else
                    {
                        vm.GoBack();
                    }
                }
                else if (vm.CurrentIndex == 13 || vm.CurrentIndex == 14 || vm.CurrentIndex == 15)
                {
                    var hasPCOS = HealthConditionsOptions.SelectedValues.Contains(HealthConditionsTypes.PCOS.GetDisplayDescription());
                    var hasEndo = HealthConditionsOptions.SelectedValues.Contains(HealthConditionsTypes.Endometriosis.GetDisplayDescription());

                    var backIndex = 12;
                    if (vm.CurrentIndex == 13) backIndex = 12;
                    if (vm.CurrentIndex == 14) backIndex = hasPCOS ? 13 : 12;
                    if (vm.CurrentIndex == 15) backIndex = hasEndo ? 14 : (hasPCOS ? 13 : 12);

                    vm.JumpToIndex(backIndex);
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

        protected override bool OnBackButtonPressed()
        {
            try
            {
                if (vm.CurrentIndex > 0)
                {
                    GoBack();
                }
                if (vm.CurrentIndex == 0)
                {
                    Shell.Current.GoToAsync(nameof(PeriodOnboardWelcomePage));
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
            DashboardAdPage.IsVisible = vm.CurrentIndex == 3;
            CalendarAdPage.IsVisible = vm.CurrentIndex == 6;
            ChatBotAdPage.IsVisible = vm.CurrentIndex == 11;
            EducationBooksAdPage.IsVisible = vm.CurrentIndex == 16;
            PartnerShareAdPage.IsVisible = vm.CurrentIndex == 20;
        }

        private void GoBackDotButtonTapped(object sender, TappedEventArgs e)
        {
            GoBack();
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
                    var atLeastOneOptionSelected = select.SelectedValues.Count() > 0 || select.SelectedIndex >= 0;
                    vm.CurrentSlide.IsNextEnabled = atLeastOneOptionSelected;
                }
                else if (sender is SelectComponentCheck selectCheck)
                {
                    var atLeastOneOptionSelected = selectCheck.SelectedItems.Count > 0;
                    vm.CurrentSlide.IsNextEnabled = atLeastOneOptionSelected; 
                }

                HPVExtras.IsVisible = HpvVaccineOption.SelectedIndex == 0;
                VaccineName.IsVisible = HpvVaccineTypeOption.SelectedIndex == 4;
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
                    if (vm.CurrentIndex == 17)
                    {
                        var periodIrregularType = EnumHelper.GetEnumValueFromName<PeriodIrregularityType>(CycleExperienceCheck.SelectedItems.FirstOrDefault());
                        LocalStorageService.UserCycleProfile.PeriodIrregularityType = periodIrregularType;
                        if (periodIrregularType == PeriodIrregularityType.OccasionallyIrregular || periodIrregularType == PeriodIrregularityType.FrequentlyIrregular)
                        {
                            vm.JumpToIndex(20);
                        }
                        else
                        {
                            vm.GoNext();
                        }
                    }
                    else if (vm.CurrentIndex == 12)
                    {
                        var hasPCOS = HealthConditionsOptions.SelectedValues.Contains(HealthConditionsTypes.PCOS.GetDisplayDescription());
                        var hasEndo = HealthConditionsOptions.SelectedValues.Contains(HealthConditionsTypes.Endometriosis.GetDisplayDescription());

                        var jumpIndex = hasPCOS ? 13 : (hasEndo ? 14 : 15);
                        vm.JumpToIndex(jumpIndex);
                    }
                    else if (vm.CurrentIndex == 13)
                    {
                        var hasEndo = HealthConditionsOptions.SelectedValues.Contains(HealthConditionsTypes.Endometriosis.GetDisplayDescription());
                        var jumpIndex = hasEndo ? 14 : 15;
                        vm.JumpToIndex(jumpIndex);
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

                    if (BloodTypeOption.SelectedIndex >= 0)
                    {
                        var bloodType = BloodTypeOption.SelectedValue;
                        LocalStorageService.UserBodyMetrics.BloodGroup = bloodType.Substring(0, bloodType.Length - 1);
                        LocalStorageService.UserBodyMetrics.RhFactor = $"{bloodType[bloodType.Length - 1]}";
                    }
                    if (HealthConditionsOptions.SelectedValues.Count > 0)
                    {
                        LocalStorageService.UserCycleProfile.HealthConditions = HealthConditionsOptions.SelectedValues.Select(s => s).ToList();
                    }
                    if (CycleExperienceCheck.SelectedItems.Count > 0)
                    {
                        var periodIrregularType = EnumHelper.GetEnumValueFromName<PeriodIrregularityType>(CycleExperienceCheck.SelectedItems.FirstOrDefault());
                        LocalStorageService.UserCycleProfile.PeriodIrregularityType = periodIrregularType;
                    }
                    
                    LocalStorageService.UserCycleProfile.LastPeriodDate = LastPeriodDate.SelectedDate;
                    LocalStorageService.UserCycleProfile.CycleLengthDays = PeriodCycleLength.Value;
                    LocalStorageService.UserCycleProfile.PeriodLengthDays = PeriodDuration.Value;

                    LocalStorageService.PeriodTrackerSet = true;

                    LocalStorageService.UserCycleProfile.UsedHPVvaccine = HpvVaccineOption.SelectedIndex == 0;
                    LocalStorageService.UserCycleProfile.HPVName = HpvVaccineOption.SelectedIndex != 4 ? HpvVaccineOption.SelectedValue : HPVCustomName?.Text;
                    LocalStorageService.UserCycleProfile.HPVDate = HPVDate.SelectedDate;

                    LocalStorageService.UserCycleProfile.UsingTampon = TamponsOption.SelectedValue;
                    
                    LocalStorageService.UserCycleProfile.PCOSYears = (PcosDurationYear.Value > 0 || PcosDurationMonth.Value > 0) ? PcosDurationYear.Value : null;
                    LocalStorageService.UserCycleProfile.PCOSMonths = (PcosDurationYear.Value > 0 || PcosDurationMonth.Value > 0) ? PcosDurationMonth.Value : null;
                    LocalStorageService.UserCycleProfile.PCOSDiet = PcosDurationMonth.Value > 0 ? PCOSDiet.SelectedIndex == 0 : null;
                    LocalStorageService.UserCycleProfile.EndometriosisYears = (ENDODurationYear.Value > 0 || ENDODurationMonth.Value > 0) ? ENDODurationYear.Value : null;
                    LocalStorageService.UserCycleProfile.EndometriosisMonths = (ENDODurationYear.Value > 0 || ENDODurationMonth.Value > 0) ? ENDODurationMonth.Value : null;
                    LocalStorageService.UserCycleProfile.EndometriosisDiet =  ENDODiet.SelectedIndex >= 0 ? ENDODiet.SelectedIndex == 0 : null;

                    await AppSpinner.ShowSpinnerAsync();
                    await Shell.Current.GoToAsync(nameof(PeriodOnboardFinishPage));
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
                GoBack();
                ShowIntraMarketingPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failure, error: {ex.Message}");
            }
        }

        private string GetDurationValueChanged(NumericStepper year, NumericStepper month)
        {
            var duration = "";
            if (year.Value > 0)
            {
                duration = $"{year.Value}";
                duration += year.Value == 1 ? " year" : " years";
            }
            if (month.Value > 0)
            {
                if (!string.IsNullOrEmpty(duration))
                {
                    duration += " and ";
                }
                duration += $"{month.Value}";
                duration += month.Value == 1 ? " month" : " months";
            }
            if (!string.IsNullOrEmpty(duration))
            {
                duration = $"*{duration}";
            }
            return duration;
        }

        private void PcosDurationValueChanged(object sender, int e)
        {
            PCOSDurationFullText.Text = GetDurationValueChanged(PcosDurationYear, PcosDurationMonth);
        }

        private void EndoDurationValueChanged(object sender, int e)
        {
            ENDODurationFullText.Text = GetDurationValueChanged(ENDODurationYear, ENDODurationMonth);
        }
    }
}
